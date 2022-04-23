//

using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AvaloniaGLExample.Graphics;

/// <summary>
/// A wrapper class for an OpenGL shader program.
/// </summary>
public class Shader
{
    private readonly Dictionary<string, int> uniformLocations;
    
    public Shader(string vertPath, string fragPath)
    {
        var shaderSource = File.ReadAllText(vertPath);
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, shaderSource);
        CompileShader(vertexShader);

        shaderSource = File.ReadAllText(fragPath);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, shaderSource);
        CompileShader(fragmentShader);

        this.Handle = GL.CreateProgram();
        GL.AttachShader(this.Handle, vertexShader);
        GL.AttachShader(this.Handle, fragmentShader);
        GL.LinkProgram(this.Handle);
        
        // Check for linking errors.
        GL.GetProgram(this.Handle, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            throw new Exception($"Error occurred whilst linking Program({this.Handle}).");
        }
        
        GL.DetachShader(this.Handle, vertexShader);
        GL.DetachShader(this.Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

        // Cache the uniform locations.
        GL.GetProgram(this.Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        this.uniformLocations = new Dictionary<string, int>();
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(this.Handle, i, out _, out _);
            var location = GL.GetUniformLocation(this.Handle, key);
            this.uniformLocations.Add(key, location);
        }
    }
    
    /// <summary>
    /// Gets the handle to the shader program.
    /// </summary>
    public int Handle { get; }
    
    /// <summary>
    /// A wrapper that calls GL.UseProgram().
    /// </summary>
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    
    /// <summary>
    /// Gets the attribute location of the attribute name, if using dynamic referencing.
    /// </summary>
    /// <param name="attribName">The attribute name.</param>
    /// <returns>The location of the attribute.</returns>
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }
    
    /// <summary>
    /// Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetInt(string name, int data)
    {
        GL.UseProgram(this.Handle);
        GL.Uniform1(this.uniformLocations[name], data);
    }

    /// <summary>
    /// Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetFloat(string name, float data)
    {
        GL.UseProgram(this.Handle);
        GL.Uniform1(this.uniformLocations[name], data);
    }

    /// <summary>
    /// Set a uniform Matrix4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    /// <remarks>
    ///   <para>
    ///   The matrix is transposed before being sent to the shader.
    ///   </para>
    /// </remarks>
    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(this.Handle);
        GL.UniformMatrix4(this.uniformLocations[name], true, ref data);
    }

    /// <summary>
    /// Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(this.Handle);
        GL.Uniform3(this.uniformLocations[name], data);
    }
    
    /// <summary>
    /// Set a uniform Vector4 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <param name="data">The data to set.</param>
    public void SetVector4(string name, Vector4 data)
    {
        GL.UseProgram(this.Handle);
        GL.Uniform4(this.uniformLocations[name], data);
    }
    
    private static void CompileShader(int shader)
    {
        GL.CompileShader(shader);

        // Check for compilation errors.
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code == (int) All.True)
        {
            return;
        }
        
        // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
        var infoLog = GL.GetShaderInfoLog(shader);
        throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
    }
}
