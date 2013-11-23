﻿// SEVENENGINE LISCENSE:
// You are free to use, modify, and distribute any or all code segments/files for any purpose
// including commercial use under the following condition: any code using or originally taken 
// from the SevenEngine project must include citation to its original author(s) located at the
// top of each source code file, or you may include a reference to the SevenEngine project as
// a whole but you must include the current SevenEngine official website URL and logo.
// - Thanks.  :)  (support: seven@sevenengine.com)

// Author(s):
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com
// Last Edited: 11-22-13

// This file contains the following classes:
// - Array
//   - ArrayException
// - ArrayCyclic
//   - ArrayCyclicException

using System;
using System.Threading;
using SevenEngine.DataStructures.Interfaces;

namespace SevenEngine.DataStructures
{
  #region Array

  /// <summary>Implements a standard array that inherits InterfaceTraversable.</summary>
  /// <typeparam name="Type">The generic type within the structure.</typeparam>
  public class Array<Type> : InterfaceTraversable<Type>
  {
    private Type[] _array;

    private Object _lock;
    private int _readers;
    private int _writers;

    /// <summary>The length of the array.</summary>
    public int Length { get { return _array.Length; } }

    /// <summary>Allows indexed access of the array.</summary>
    /// <param name="index">The index of the array to get/set.</param>
    /// <returns>The value at the desired index.</returns>
    public Type this[int index]
    {
      get
      {
        ReaderLock();
        if (index < 0 || index > _array.Length)
          throw new ArrayException("index out of bounds.");
        Type returnValue = _array[index];
        ReaderUnlock();
        return returnValue;
      }
      set
      {
        WriterLock();
        if (index < 0 || index > _array.Length)
          throw new ArrayException("index out of bounds.");
        _array[index] = value;
        WriterUnlock();
      }
    }

    /// <summary>Constructs an array that implements a traversal delegate function 
    /// which is an optimized "foreach" implementation.</summary>
    /// <param name="size">The length of the array in memory.</param>
    public Array(int size)
    {
      if (size < 0)
        throw new ArrayException("size of the array must be non-negative.");
      _array = new Type[size];
      _lock = new Object();
      _readers = 0;
      _writers = 0;
    }

    public delegate bool TraversalFunction(Type node);
    /// <summary>Allows a traversal of an array like "foreach",
    /// but it is optomized to be faster than "foreach".</summary>
    /// <param name="traversalFunction">The delegate function to 
    /// perform each iteration.</param>
    public void Traversal(Func<Type, bool> traversalFunction)
    {
      ReaderLock();
      int index = 0;
      while (index < _array.Length) { traversalFunction(_array[index++]); }
      ReaderUnlock();
    }

    /// <summary>Thread safe enterance for readers.</summary>
    private void ReaderLock() { lock (_lock) { while (!(_writers == 0)) Monitor.Wait(_lock); _readers++; } }
    /// <summary>Thread safe exit for readers.</summary>
    private void ReaderUnlock() { lock (_lock) { _readers--; Monitor.Pulse(_lock); } }
    /// <summary>Thread safe enterance for writers.</summary>
    private void WriterLock() { lock (_lock) { while (!(_writers == 0) && !(_readers == 0)) Monitor.Wait(_lock); _writers++; } }
    /// <summary>Thread safe exit for readers.</summary>
    private void WriterUnlock() { lock (_lock) { _writers--; Monitor.PulseAll(_lock); } }

    /// <summary>This is used for throwing AVL Tree exceptions only to make debugging faster.</summary>
    private class ArrayException : Exception { public ArrayException(string message) : base(message) { } }
  }

  #endregion

  #region ArrayCyclic

  /// <summary>Implements a cyclic array (allows overwriting) that inherits InterfaceTraversable.</summary>
  /// <typeparam name="Type">The generic type within the structure.</typeparam>
  public class ArrayCyclic<Type> : InterfaceTraversable<Type>
  {
    private Type[] _array;
    int _start;
    int _count;

    private Object _lock;
    private int _readers;
    private int _writers;

    /// <summary>The length of the array.</summary>
    public int Length { get { return _array.Length; } }

    /// <summary>Allows indexed access of the array.</summary>
    /// <param name="index">The index of the array to get/set.</param>
    /// <returns>The value at the desired index.</returns>
    public Type this[int index]
    {
      get
      {
        ReaderLock();
        if (index < 0 || index > _array.Length)
        {
          ReaderUnlock();
          throw new ArrayCyclicException("index out of bounds.");
        }
        Type returnValue = _array[(index + _start) % _array.Length];
        ReaderUnlock();
        return returnValue;
      }
      set
      {
        WriterLock();
        if (index < 0 || index > _array.Length ||
          (index > _start + _count && index < ((_start + _count)) % _array.Length))
        {
          WriterUnlock();
          throw new ArrayCyclicException("index out of bounds.");
        }
        _array[(index + _start) % _array.Length] = value;
        WriterUnlock();
      }
    }

    /// <summary>Constructs an array that implements a traversal delegate function 
    /// which is an optimized "foreach" implementation.</summary>
    /// <param name="size">The length of the array in memory.</param>
    public ArrayCyclic(int size)
    {
      if (size < 0)
        throw new ArrayCyclicException("size of the array must be non-negative.");
      _array = new Type[size];
      _start = 0;
      _count = 0;
      _lock = new Object();
      _readers = 0;
      _writers = 0;
    }

    public delegate bool TraversalFunction(Type node);
    /// <summary>Allows a traversal of an array like "foreach",
    /// but it is optomized to be faster than "foreach".</summary>
    /// <param name="traversalFunction">The delegate function to 
    /// perform each iteration.</param>
    public void Traversal(Func<Type, bool> traversalFunction)
    {
      ReaderLock();
      int index = 0;
      while (index < _array.Length) { traversalFunction(_array[index++]); }
      ReaderUnlock();
    }

    /// <summary>Thread safe enterance for readers.</summary>
    private void ReaderLock() { lock (_lock) { while (!(_writers == 0)) Monitor.Wait(_lock); _readers++; } }
    /// <summary>Thread safe exit for readers.</summary>
    private void ReaderUnlock() { lock (_lock) { _readers--; Monitor.Pulse(_lock); } }
    /// <summary>Thread safe enterance for writers.</summary>
    private void WriterLock() { lock (_lock) { while (!(_writers == 0) && !(_readers == 0)) Monitor.Wait(_lock); _writers++; } }
    /// <summary>Thread safe exit for readers.</summary>
    private void WriterUnlock() { lock (_lock) { _writers--; Monitor.PulseAll(_lock); } }

    /// <summary>This is used for throwing AVL Tree exceptions only to make debugging faster.</summary>
    private class ArrayCyclicException : Exception { public ArrayCyclicException(string message) : base(message) { } }
  }

  #endregion
}