//

using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace AvaloniaGLExample.Graphics;

/// <summary>
/// A simple camera class.
/// </summary>
public class Camera
{
    private Vector3 lookTarget = Vector3.Zero;
    private Vector3 worldUp = Vector3.UnitY;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class.
    /// </summary>
    public Camera()
    {
    }

    /// <summary>
    /// Gets or sets the position of the camera.
    /// </summary>
    public Vector3 Position { get; set; } = new Vector3(0, 0, 10);

    /// <summary>
    /// Gets or sets the target that the camera is looking at.
    /// </summary>
    public Vector3 LookTarget
    {
        get => this.lookTarget;
        set
        {
            // Prevent any changes that would cause the forward direction and world up directions to be parallel.
            var newLookDirection = (value - this.Position).Normalized();
            if (Math.Abs(Vector3.Dot(this.worldUp, newLookDirection)) > 0.9f)
            {
                return;
            }
            
            this.lookTarget = value;
        }
    }

    /// <summary>
    /// Gets or sets the aspect ratio.
    /// </summary>
    public float AspectRatio { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the field of view.
    /// </summary>
    public float FieldOfView { get; set; } = 45.0f;

    /// <summary>
    /// Gets or sets the near plane.
    /// </summary>
    public float NearPlaneDistance { get; set; } = 0.1f;

    /// <summary>
    /// Gets or sets the far plane.
    /// </summary>
    public float FarPlaneDistance { get; set; } = 100.0f;

    /// <summary>
    /// Gets the normalized forward direction.
    /// </summary>
    public Vector3 Forward => (this.LookTarget - this.Position).Normalized();

    /// <summary>
    /// Gets the normalized right direction.
    /// </summary>
    public Vector3 Right => Vector3.Cross(this.worldUp, this.Forward).Normalized();

    /// <summary>
    /// Gets the normalized up direction.
    /// </summary>
    public Vector3 Up => Vector3.Cross(this.Forward, this.Right).Normalized();

    /// <summary>
    /// Gets the view transform calculated from a look at matrix.
    /// </summary>
    public Matrix4 ViewTransform => Matrix4.LookAt(
        this.Position,
        this.LookTarget,
        this.worldUp);
    
    /// <summary>
    /// Gets the perspective projection transform.
    /// </summary>
    public Matrix4 ProjectionTransform => Matrix4.CreatePerspectiveFieldOfView(
        MathHelper.DegreesToRadians(this.FieldOfView),
        this.AspectRatio,
        this.NearPlaneDistance,
        this.FarPlaneDistance);

    /// <summary>
    /// Rotates the camera.
    /// </summary>
    /// <param name="rotation">A quaternion representing the rotation to perform.</param>
    public void Rotate(Quaternion rotation)
    {
        this.LookTarget = this.Position + (rotation * this.Forward);
    }
}
