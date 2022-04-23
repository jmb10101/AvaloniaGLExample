//

using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AvaloniaGLExample.Graphics;

/// <summary>
/// A texture wrapper class for OpenGL.
/// </summary>
public class Texture
{
    public Texture(string path)
    {
        this.Handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, this.Handle);
        
        var image = Image.Load<Rgba32>(path);
        var pixels = new List<byte>(4 * image.Width * image.Height);
        image.ProcessPixelRows(pixelAccessor =>
        {
            for (int y = 0; y < pixelAccessor.Height; y++)
            {
                var row = pixelAccessor.GetRowSpan(y);
                for (int x = 0; x < pixelAccessor.Width; x++)
                {
                    pixels.Add(row[x].R);
                    pixels.Add(row[x].G);
                    pixels.Add(row[x].B);
                    pixels.Add(row[x].A);
                }
            }
        });
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        
        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            image.Width,
            image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            pixels.ToArray());

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }
    
    /// <summary>
    /// Gets the handle to the texture.
    /// </summary>
    public int Handle { get; }

    /// <summary>
    /// Binds the texture.
    /// </summary>
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, this.Handle);
    }
}
