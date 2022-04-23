//

using System;
using OpenTK.Mathematics;

namespace AvaloniaGLExample.Graphics;

/// <summary>
/// Static utility methods for vectors.
/// </summary>
public static class VectorUtilities
{
    public static string ToFormattedString(this Vector3 v, string format) => $"({v.X.ToString(format)}, {v.Y.ToString(format)}, {v.Z.ToString(format)})";
}
