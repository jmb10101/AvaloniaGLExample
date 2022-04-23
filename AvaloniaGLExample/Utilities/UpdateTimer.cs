//

using System;
using System.Collections.Generic;
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

namespace AvaloniaGLExample.Utilities;

public class UpdateTimer : ReactiveObject
{
    private readonly IList<float> buffer;
    private int currentIndex;
    private TimeSpan time = TimeSpan.Zero;
    private float averageUpdateRate;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTimer"/> class.
    /// </summary>
    /// <param name="bufferSize">The number of updates to store in a buffer used to calculate the average update rate.</param>
    public UpdateTimer(int bufferSize = 60)
    {
        if (bufferSize < 1)
        {
            throw new ArgumentException("The bufferSize must be greater than 0.", nameof(bufferSize));
        }
        
        this.buffer = new float[bufferSize];
    }
    
    /// <summary>
    /// Gets the total time since this timer was created.
    /// </summary>
    public TimeSpan Time
    {
        get => this.time;
        private set => this.RaiseAndSetIfChanged(ref this.time, value);
    }
    
    /// <summary>
    /// Gets the average update rate measured in Hz.
    /// </summary>
    public float AverageUpdateRate
    {
        get => this.averageUpdateRate;
        private set => this.RaiseAndSetIfChanged(ref this.averageUpdateRate, value);
    }

    /// <summary>
    /// Updates the timer.
    /// </summary>
    /// <param name="deltaTime">The time duration to increase the timer.</param>
    public void Update(TimeSpan deltaTime)
    {
        this.Time += deltaTime;
        this.buffer[this.currentIndex++] = (float)deltaTime.TotalSeconds;
        if (this.currentIndex == this.buffer.Count)
        {
            this.currentIndex = 0;
            this.AverageUpdateRate = 1 / this.buffer.Average();
        }
    }
}