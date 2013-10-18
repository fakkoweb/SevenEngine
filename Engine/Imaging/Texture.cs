﻿using System.Runtime.InteropServices;

namespace Engine.Imaging
{
  [StructLayout(LayoutKind.Sequential)]
  public class Texture
  {
    protected int _existingReferences;

    protected string _id;
    protected int _handle;
    protected int _width;
    protected int _height;

    /// <summary>The number of existing hardware instances of this model reference.</summary>
    public int ExistingReferences { get { return _existingReferences; } set { _existingReferences = value; } }

    /// <summary>The string id associated with this specific texture when it was loaded.</summary>
    public string Id { get { return _id; } set { _id = value; } }
    /// <summary>The handle of the texture on the GPU.</summary>
    public int Handle { get { return _handle; } set { _handle = value; } }
    /// <summary>The width of the texture.</summary>
    public int Width { get { return _width; } set { _width = value; } }
    /// <summary>The height of the texture.</summary>
    public int Height { get { return _height; } set { _height = value; } }

    public Texture(string id, int handle, int width, int height)
    {
      _id = id;
     _handle = handle;
      _width = width;
      _height = height;
      _existingReferences = 0;
    }
  }
}