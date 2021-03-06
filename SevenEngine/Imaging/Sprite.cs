﻿// Seven
// https://github.com/53V3N1X/SevenEngine
// LISCENSE: See "LISCENSE.txt" in th root project directory.
// SUPPORT: See "README.txt" in the root project directory.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Seven.Mathematics;

namespace SevenEngine.Imaging
{
  /// <summary>Represents a single image with world transformation values.</summary>
  public class Sprite
  {
    // Every sprite uses the same vertex positions
    private static readonly float[] _verteces = new float[] {
      1f, 1f, 0f,   0f, 1f, 0f,   1f, 0f, 0f,
      0f, 0f, 0f,   1f, 0f, 0f,   0f, 1f, 0f };
    private static readonly int _vertexCount = 6;
    private static readonly float[] _textureMappingsDefault = new float[] {
      1f,0f,  0f,0f,  1f,1f,
      0f,1f,  1f,1f,  0f,0f };
    private Vector<float> _position;
    private Texture _texture;
    private Vector<float> _scale;
    private float _rotation;
    private static int _gpuVertexBufferHandle;
    private static int _gpuTextureMappingBufferHandleDefault;
    private int _gpuTextureMappingBufferHandle;

    /// <summary>The handle to the memory of the texture buffer on the GPU.</summary>
    internal int GpuVertexBufferHandle { get { return _gpuVertexBufferHandle; } }
    /// <summary>The handle to the memory of the texture buffer on the GPU.</summary>
    internal int GPUTextureCoordinateBufferHandle { get { return _gpuTextureMappingBufferHandle; } }
    /// <summary>Returns 6, because sprites always have 6 verteces.</summary>
    internal int VertexCount { get { return _vertexCount; } }
    /// <summary>Get and set the position of the sprite.</summary>
    public Vector<float> Position { get { return _position; } set { _position = value; } }
    /// <summary>Get and set the size of the sprite.</summary>
    public Vector<float> Scale 
    { 
      get { return _scale; } 
      set
      {
        if (value.Dimensions != 2) 
          throw new Exception("the scale vector of a sprite can only have two components."); 
        _scale = value;
      }
    }
    /// <summary>Get and set the rotation angle of the sprite.</summary>
    public float Rotation { get { return _rotation; } set { _rotation = value; } }
    /// <summary>Get and set the texture the sprite is mapping to.</summary>
    public Texture Texture { get { return _texture; } set { _texture = value; } }

    /// <summary>Creates an instance of a sprite.</summary>
    /// <param name="texture">The texture to have this sprite mapped to.</param>
    public Sprite(Texture texture)
    {
      if (_gpuVertexBufferHandle == 0)
        GenerateVertexBuffer(_verteces);
      _position = new Vector<float>(0, 0, -10);
      _scale = new Vector<float>(1, 1);
      _rotation = 0f;
      _texture = texture;
      if (_gpuTextureMappingBufferHandleDefault == 0)
      {
        GenerateTextureCoordinateBuffer(_textureMappingsDefault);
        _gpuTextureMappingBufferHandleDefault = _gpuTextureMappingBufferHandle;
      }
      else
        _gpuTextureMappingBufferHandle = _gpuTextureMappingBufferHandleDefault;
    }

    /// <summary>Creates an instance of a sprite.</summary>
    /// <param name="texture">The texture to have this sprite mapped to.</param>
    /// <param name="textureMappings">The texture mappings for this sprite.</param>
    public Sprite(Texture texture, float[] textureMappings)
    {
      if (_gpuVertexBufferHandle == 0)
        GenerateVertexBuffer(_verteces);
      _position = new Vector<float>(0, 0, -10);
      _scale = new Vector<float>(1, 1);
      _rotation = 0f;
      _texture = texture;
      if (textureMappings.Length != 12)
        throw new Exception("Invalid number of texture coordinates in sprite constructor.");
      GenerateTextureCoordinateBuffer(textureMappings);
    }

    #region Buffer Generators

    /// <summary>Generates the vertex buffer that all sprites will use.</summary>
    private void GenerateVertexBuffer(float[] verteces)
    {
      // Declare the buffer
      GL.GenBuffers(1, out _gpuVertexBufferHandle);
      // Select the new buffer
      GL.BindBuffer(BufferTarget.ArrayBuffer, _gpuVertexBufferHandle);
      // Initialize the buffer values
      GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(verteces.Length * sizeof(float)), verteces, BufferUsageHint.StaticDraw);
      // Quick error checking
      int bufferSize;
      GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
      if (verteces.Length * sizeof(float) != bufferSize)
        throw new ApplicationException("Vertex array not uploaded correctly");
      // Deselect the new buffer
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    /// <summary>Generates the texture coordinate buffer that sprite will default to.</summary>
    private void GenerateTextureCoordinateBuffer(float[] textureMappings)
    {
      // Declare the buffer
      GL.GenBuffers(1, out _gpuTextureMappingBufferHandle);
      // Select the new buffer
      GL.BindBuffer(BufferTarget.ArrayBuffer, _gpuTextureMappingBufferHandle);
      // Initialize the buffer values
      GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(textureMappings.Length * sizeof(float)), textureMappings, BufferUsageHint.StaticDraw);
      // Quick error checking
      int bufferSize;
      GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
      if (textureMappings.Length * sizeof(float) != bufferSize)
        throw new ApplicationException("Texture mapping array not uploaded correctly");
      // Deselect the new buffer
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    #endregion
  }
}