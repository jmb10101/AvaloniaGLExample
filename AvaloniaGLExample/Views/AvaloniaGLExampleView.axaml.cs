//

using System;
using System.Reactive.Linq;
using System.Reflection.PortableExecutable;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ReactiveUI;
using AvaloniaGLExample.Graphics;
using AvaloniaGLExample.ViewModels;

namespace AvaloniaGLExample.Views;

public partial class AvaloniaGLExampleView : ReactiveUserControl<AvaloniaGLExampleViewModel>
{
    float[] vertices = {
        -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,   0.0f, 0.0f,
        0.5f, -0.5f, -0.5f,   1.0f, 0.0f, 0.0f,   1.0f, 0.0f,
        0.5f,  0.5f, -0.5f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,
        0.5f,  0.5f, -0.5f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 0.0f,   0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,   0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,   1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,

        0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,1.0f, 1.0f,
        0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,
        0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,
        0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,0.0f, 0.0f,
        0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,   1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,1.0f, 1.0f,
        0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,   1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,0.0f, 1.0f,
        0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   1.0f, 0.0f, 0.0f, 0.0f, 1.0f
    };

    private readonly uint[] indices =
    {
        0, 1, 3,
        1, 2, 3,
    };
    
    private int vertexBufferObject;
    private int elementBufferObject;
    private int vertexArrayObject;
    private Shader? shader;
    private Texture? texture0;
    private Texture? texture1;
    private Vector2? cameraRotation = null;
    private Vector2? cameraPan = null;

    public AvaloniaGLExampleView()
    {
        this.InitializeComponent();
        AvaloniaXamlLoader.Load(this);

        this.WhenActivated(disposables =>
        {
            this
                .OneWayBind(
                    this.ViewModel,
                    vm => vm.Diagnostics,
                    v => v.DiagnosticsLabel.Text,
                    vmDiagnostics => $"{vmDiagnostics};\n{this.Viewport.AverageFps} fps")
                .DisposeWith(disposables);

            this
                .Events().PointerPressed
                .Select(args => args.GetCurrentPoint(this.Viewport))
                .Where(args => args.Position.X >= 0 && args.Position.Y >= 0 && args.Position.X <= this.Viewport.Width && args.Position.Y <= this.Viewport.Height)
                .Subscribe(args =>
                {
                    var position = new Vector2((float)args.Position.X, (float)args.Position.Y);
                    if (args.Properties.IsRightButtonPressed)
                    {
                        this.cameraRotation = position;
                    }
                    else if (args.Properties.IsMiddleButtonPressed)
                    {
                        this.cameraPan = position;
                    }
                })
                .DisposeWith(disposables);
            this
                .Events().PointerLeave
                .Merge(this.Events().PointerReleased)
                .Subscribe(args =>
                {
                    this.cameraRotation = null;
                    this.cameraPan = null;
                })
                .DisposeWith(disposables);
            this
                .Events().PointerWheelChanged
                .Select(args => (delta:args.Delta, point:args.GetPosition(this.Viewport)))
                .Where(args => args.point.X >= 0 && args.point.Y >= 0 && args.point.X <= this.Viewport.Width && args.point.Y <= this.Viewport.Height)
                .Subscribe(args => this.ViewModel?.DollyCamera((float)args.delta.Y))
                .DisposeWith(disposables);
            this
                .Events().PointerMoved
                .Select(args => (properties:args.GetCurrentPoint(this.Viewport).Properties, point:args.GetPosition(this.Viewport)))
                .Where(args =>
                {
                    var inBounds = args.point.X >= 0 && args.point.Y >= 0 && args.point.X <= this.Viewport.Width &&args.point.Y <= this.Viewport.Height;
                    if (!inBounds)
                    {
                        this.cameraRotation = null;
                        this.cameraPan = null;
                    }
                    
                    return inBounds;
                })
                .Subscribe(args =>
                {
                    var position = new Vector2((float)args.point.X, (float)args.point.Y);
                    if (this.cameraRotation.HasValue)
                    {
                        var delta = position - this.cameraRotation.Value;
                        this.cameraRotation = position;
                        delta.X /= (float)this.Viewport.Width;
                        delta.Y /= -1f * (float)this.Viewport.Height;
                        this.ViewModel?.RotateCamera(delta);
                    }
                    else if (this.cameraPan.HasValue)
                    {
                        var delta = position - this.cameraPan.Value;
                        this.cameraPan = position;
                        delta.X /= (float)this.Viewport.Width;
                        delta.Y /= -1f * (float)this.Viewport.Height;
                        this.ViewModel?.PanCamera(delta);
                    }
                })
                .DisposeWith(disposables);
        });

        this.Viewport.InitAsObservable
            .Subscribe(args=>
            {
                var gl = args.gl;
                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                gl.ClearColor(0.06f, 0.06f, 0.06f, 1.0f);
        
                this.vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, this.vertices.Length * sizeof(float), this.vertices, BufferUsageHint.StaticDraw);

                this.vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(this.vertexArrayObject);
        
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
        
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
        
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);
        
                this.elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, this.indices.Length * sizeof(uint), this.indices, BufferUsageHint.StaticDraw);
        
                this.shader = new Shader("Assets/Shaders/shader.vert", "Assets/Shaders/shader.frag");
                this.shader.Use();

                this.texture0 = new Texture("Assets/Textures/container.jpg");
                this.texture0.Use(TextureUnit.Texture0);
                this.shader.SetInt("texture0", 0);
                this.texture1 = new Texture("Assets/Textures/awesomeface.png");
                this.texture1.Use(TextureUnit.Texture1);
                this.shader.SetInt("texture1", 1);

                var w = (float)this.Viewport.Width;
                var h = (float)this.Viewport.Height;
                this.ViewModel!.Camera.AspectRatio = w / h;
            });
        
        this.Viewport.RenderAsObservable
            .Subscribe(args =>
            {
                // Resize to fit container if needed.
                var (deltaTime, gl, fb) = args;
                var parent = this.Viewport.Parent;
                if (parent != null)
                {
                    var parentBounds = parent.TransformedBounds;
                    var viewportBounds = this.Viewport.TransformedBounds;
                    if (parentBounds.HasValue
                        && viewportBounds.HasValue
                        && parentBounds.Value.Bounds != viewportBounds.Value.Bounds)
                    {
                        var w = parentBounds.Value.Bounds.Width;
                        var h = parentBounds.Value.Bounds.Height;
                        this.Viewport.Resize(gl, w, h);
                        this.ViewModel!.Camera.AspectRatio = (float)w / (float)h;
                        return;
                    }
                }

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.BindVertexArray(this.vertexArrayObject);

                this.shader?.Use();
                this.shader?.SetMatrix4("view", this.ViewModel!.Camera.ViewTransform);
                this.shader?.SetMatrix4("projection", this.ViewModel!.Camera.ProjectionTransform);

                var heat = (float)(Math.Sin(this.Viewport.Time.TotalSeconds) + 1) / 2;
                

                for (int x = 0; x < 10; x++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        var modelTransform = Matrix4.Identity;
                        modelTransform *= Matrix4.CreateTranslation(x, 0, -z);
                        this.shader?.SetMatrix4("model", modelTransform);
                        //this.shader?.SetFloat("vertColorSaturation", 0f);
                        //this.shader?.SetFloat("textureOpacity", 1f);
                        this.texture0?.Use(TextureUnit.Texture0);
                        this.texture1?.Use(TextureUnit.Texture1);
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                    
                        // this.shader?.SetMatrix4("model", modelTransform * Matrix4.CreateScale(1.001f, 1.001f, 1.001f));
                        // this.shader?.SetFloat("vertColorSaturation", heat * 0.6f);
                        // this.shader?.SetFloat("textureOpacity", 0.5f);
                        // this.texture1?.Use(TextureUnit.Texture0);
                        // GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                    }
                }
                
                // Avalonia requires active texture0.
                GL.ActiveTexture(TextureUnit.Texture0);
            });

        this.Viewport.UpdateAsObservable
            .Subscribe(deltaTime => this.ViewModel?.Update(deltaTime));
    }
    
    public OpenGlViewport Viewport => this.FindControl<OpenGlViewport>("ViewportCtrl");
    public TextBlock DiagnosticsLabel => this.FindControl<TextBlock>("DiagnosticsLabelCtrl");
}