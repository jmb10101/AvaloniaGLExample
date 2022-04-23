//

using System;
using ReactiveUI;
using OpenTK.Mathematics;
using AvaloniaGLExample.Graphics;
using AvaloniaGLExample.Utilities;

namespace AvaloniaGLExample.ViewModels;

public class AvaloniaGLExampleViewModel : ReactiveObject
{
    private readonly UpdateTimer timer = new ();
    private string diagnostics;
    private float camPanSpeed = 20f;
    private float camRotateSpeed = 1f;
    private float camDollySpeed = 1f;

    public AvaloniaGLExampleViewModel()
    {
        this.Camera.Position = new Vector3(-2, -2,2);
        this.Camera.LookTarget = new Vector3(0, 0, 0);
    }
    
    public string Diagnostics
    {
        get => this.diagnostics;
        private set => this.RaiseAndSetIfChanged(ref this.diagnostics, value);
    }

    public UpdateTimer Timer => this.timer;

    public Camera Camera { get; private set; } = new Camera();

    /// <summary>
    /// Updates the simulation.
    /// </summary>
    /// <param name="deltaTime">The duration of time since the last update.</param>
    public void Update(TimeSpan deltaTime)
    {
        this.Timer.Update(deltaTime);
        this.Diagnostics = $"{this.Timer.Time:hh\\:mm\\:ss}; {this.Timer.AverageUpdateRate:0} hz\n" +
                           $"Camera: Position={this.Camera.Position.ToFormattedString("0.0")}; Forward={this.Camera.Forward.ToFormattedString("0.0")}";
    }

    public void RotateCamera(Vector2 normalizedInputDelta)
    {
        // Apply aspect ratio to normalized input to account for viewport dimensions.
        normalizedInputDelta.X *= this.Camera.AspectRatio;
        
        var xRotation = Quaternion.FromAxisAngle(Vector3.UnitY, -normalizedInputDelta.X * this.camRotateSpeed);
        var yRotation = Quaternion.FromAxisAngle(this.Camera.Right, normalizedInputDelta.Y * this.camRotateSpeed);
        this.Camera.Rotate(xRotation * yRotation);
    }

    public void PanCamera(Vector2 normalizedInputDelta)
    {
        // Apply aspect ratio to normalized input to account for viewport dimensions.
        normalizedInputDelta.X *= this.Camera.AspectRatio;

        var forward = this.Camera.Forward;
        this.Camera.Position += this.Camera.Right * normalizedInputDelta.X * this.camPanSpeed;
        this.Camera.Position += this.Camera.Up * normalizedInputDelta.Y * this.camPanSpeed;
        this.Camera.LookTarget = this.Camera.Position + forward;
    }

    public void DollyCamera(float delta)
    {
        var forward = this.Camera.Forward;
        this.Camera.Position += this.Camera.Forward * delta * this.camDollySpeed;
        this.Camera.LookTarget = this.Camera.Position + forward;
    }
}