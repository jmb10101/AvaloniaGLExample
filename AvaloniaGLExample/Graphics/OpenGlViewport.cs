//

using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using ReactiveUI;

namespace AvaloniaGLExample.Graphics;

public class OpenGlViewport : OpenGlControlBase, IDisposable
{
    private readonly CompositeDisposable disposables = new ();
    private readonly Stopwatch renderTimer = new ();
    private readonly Stopwatch updateTimer = new ();
    private readonly Subject<(TimeSpan, GlInterface, int)> renderAsObservable = new ();
    private readonly Subject<TimeSpan> updateAsObservable = new ();
    private readonly Subject<(GlInterface, int)> initAsObservable = new ();
    private readonly float[] framerateBuffer = new float[60];
    private int currentFrame;
    private TimeSpan time = TimeSpan.Zero;
    private TimeSpan deltaTime = TimeSpan.Zero;
    private float targetFps = 60f;
    private bool initialized;
    private IDisposable? updateLoop;

    public OpenGlViewport()
    {
        this.InitializeIfNeeded();
        
        this.renderTimer.Start();
        this.updateTimer.Start();
        this.initAsObservable.DisposeWith(this.disposables);
        this.updateAsObservable.DisposeWith(this.disposables);
    }
    
    public static readonly DirectProperty<OpenGlViewport, TimeSpan> TimeProperty =
        AvaloniaProperty.RegisterDirect<OpenGlViewport, TimeSpan>(
            "Time", o => o.Time, (o, v) => o.Time = v);

    public static readonly DirectProperty<OpenGlViewport, float> TargetFpsProperty =
        AvaloniaProperty.RegisterDirect<OpenGlViewport, float>(
            "TargetFps", o => o.TargetFps, (o, v) => o.TargetFps = v);
    
    public TimeSpan Time
    {
        get => this.time;
        set => this.SetAndRaise(TimeProperty, ref this.time, value);
    }
    
    public float TargetFps
    {
        get => this.targetFps;
        set
        {
            this.SetAndRaise(TargetFpsProperty, ref this.targetFps, value);
            this.updateLoop?.Dispose();
            this.updateLoop = Observable
                .Interval(TimeSpan.FromSeconds(1 / this.TargetFps), RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    this.updateTimer.Stop();
                    var deltaTime = this.updateTimer.Elapsed;
                    this.updateTimer.Restart();
                    this.updateAsObservable.OnNext(deltaTime);
                })
                .DisposeWith(this.disposables);
        }
    }
    
    public int AverageFps { get; private set; }

    public IObservable<(TimeSpan deltaTime, GlInterface gl, int frameBuffer)> RenderAsObservable
        => this.renderAsObservable;
    
    public IObservable<TimeSpan> UpdateAsObservable
        => this.updateAsObservable;

    public IObservable<(GlInterface gl, int frameBuffer)> InitAsObservable
        => this.initAsObservable;

    /// <inheritdoc/>
    public void Dispose()
    {
        this.disposables.Dispose();
    }

    /// <summary>
    /// Triggers a resize on the control. This should be called from the Render loop.
    /// </summary>
    public void Resize(GlInterface gl, double width, double height)
    {
        this.Width = width;
        this.Height = height;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            gl.Viewport(0, 0, (int) width * 2, (int) height * 2);
        }
        else
        {
            gl.Viewport(0, 0, (int) width, (int) height);
        }
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    protected override void OnOpenGlInit(GlInterface gl, int fb)
    {
        GL.LoadBindings(new AvaloniaContext(gl));
        gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (!this.initialized)
        {
            this.initialized = true;
            this.initAsObservable.OnNext((gl, fb));
            this.initAsObservable.OnCompleted();
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
            return;
        }

        this.renderTimer.Stop();
        this.deltaTime = this.renderTimer.Elapsed;
        this.Time += this.deltaTime;
        this.framerateBuffer[this.currentFrame++] = 1 / (float)this.deltaTime.TotalSeconds;
        this.renderTimer.Restart();

        if (this.currentFrame == this.framerateBuffer.Length)
        {
            this.currentFrame = 0;
            this.AverageFps = (int)this.framerateBuffer.Average();
        }
        
        this.renderAsObservable.OnNext((this.deltaTime, gl, fb));
        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
    }
    
    private class AvaloniaContext : IBindingsContext
    {
        private readonly GlInterface glInterface;
        
        public AvaloniaContext(GlInterface glInterface)
        {
            this.glInterface = glInterface;
        }

        public IntPtr GetProcAddress(string procName) => this.glInterface.GetProcAddress(procName);
    }
}