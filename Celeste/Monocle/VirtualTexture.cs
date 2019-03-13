﻿// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualTexture
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Monocle
{
  public class VirtualTexture : VirtualAsset
  {
    internal static readonly byte[] buffer = new byte[67108864];
    internal static readonly byte[] bytes = new byte[524288];
    private const int ByteArraySize = 524288;
    private const int ByteArrayCheckSize = 524256;
    public Texture2D Texture;
    private Color color;

    public string Path { get; private set; }

    public bool IsDisposed
    {
      get
      {
        if (this.Texture != null && !((GraphicsResource) this.Texture).get_IsDisposed())
          return ((GraphicsResource) this.Texture).get_GraphicsDevice().get_IsDisposed();
        return true;
      }
    }

    internal VirtualTexture(string path)
    {
      this.Name = this.Path = path;
      this.Reload();
    }

    internal VirtualTexture(string name, int width, int height, Color color)
    {
      this.Name = name;
      this.Width = width;
      this.Height = height;
      this.color = color;
      this.Reload();
    }

    internal override void Unload()
    {
      if (this.Texture != null && !((GraphicsResource) this.Texture).get_IsDisposed())
        ((GraphicsResource) this.Texture).Dispose();
      this.Texture = (Texture2D) null;
    }

    internal override unsafe void Reload()
    {
      this.Unload();
      if (string.IsNullOrEmpty(this.Path))
      {
        this.Texture = new Texture2D(Engine.Instance.get_GraphicsDevice(), this.Width, this.Height);
        Color[] colorArray = new Color[this.Width * this.Height];
        fixed (Color* colorPtr = colorArray)
        {
          for (int index = 0; index < colorArray.Length; ++index)
            colorPtr[index] = this.color;
        }
        this.Texture.SetData<Color>((M0[]) colorArray);
      }
      else
      {
        string extension = System.IO.Path.GetExtension(this.Path);
        if (extension == ".data")
        {
          using (FileStream fileStream = File.OpenRead(System.IO.Path.Combine(Engine.ContentDirectory, this.Path)))
          {
            fileStream.Read(VirtualTexture.bytes, 0, 524288);
            int startIndex = 0;
            int int32_1 = BitConverter.ToInt32(VirtualTexture.bytes, startIndex);
            int int32_2 = BitConverter.ToInt32(VirtualTexture.bytes, startIndex + 4);
            bool flag = VirtualTexture.bytes[startIndex + 8] == (byte) 1;
            int index1 = startIndex + 9;
            int num1 = int32_1 * int32_2 * 4;
            int index2 = 0;
            fixed (byte* numPtr1 = VirtualTexture.bytes)
              fixed (byte* numPtr2 = VirtualTexture.buffer)
              {
                while (index2 < num1)
                {
                  int num2 = (int) numPtr1[index1] * 4;
                  if (flag)
                  {
                    byte num3 = numPtr1[index1 + 1];
                    if (num3 > (byte) 0)
                    {
                      numPtr2[index2] = numPtr1[index1 + 4];
                      numPtr2[index2 + 1] = numPtr1[index1 + 3];
                      numPtr2[index2 + 2] = numPtr1[index1 + 2];
                      numPtr2[index2 + 3] = num3;
                      index1 += 5;
                    }
                    else
                    {
                      numPtr2[index2] = (byte) 0;
                      numPtr2[index2 + 1] = (byte) 0;
                      numPtr2[index2 + 2] = (byte) 0;
                      numPtr2[index2 + 3] = (byte) 0;
                      index1 += 2;
                    }
                  }
                  else
                  {
                    numPtr2[index2] = numPtr1[index1 + 3];
                    numPtr2[index2 + 1] = numPtr1[index1 + 2];
                    numPtr2[index2 + 2] = numPtr1[index1 + 1];
                    numPtr2[index2 + 3] = byte.MaxValue;
                    index1 += 4;
                  }
                  if (num2 > 4)
                  {
                    int index3 = index2 + 4;
                    for (int index4 = index2 + num2; index3 < index4; index3 += 4)
                    {
                      numPtr2[index3] = numPtr2[index2];
                      numPtr2[index3 + 1] = numPtr2[index2 + 1];
                      numPtr2[index3 + 2] = numPtr2[index2 + 2];
                      numPtr2[index3 + 3] = numPtr2[index2 + 3];
                    }
                  }
                  index2 += num2;
                  if (index1 > 524256)
                  {
                    int offset = 524288 - index1;
                    for (int index3 = 0; index3 < offset; ++index3)
                      numPtr1[index3] = numPtr1[index1 + index3];
                    fileStream.Read(VirtualTexture.bytes, offset, 524288 - offset);
                    index1 = 0;
                  }
                }
              }
            this.Texture = new Texture2D(Engine.Graphics.get_GraphicsDevice(), int32_1, int32_2);
            this.Texture.SetData<byte>((M0[]) VirtualTexture.buffer, 0, num1);
          }
        }
        else if (extension == ".png")
        {
          using (FileStream fileStream = File.OpenRead(System.IO.Path.Combine(Engine.ContentDirectory, this.Path)))
            this.Texture = Texture2D.FromStream(Engine.Graphics.get_GraphicsDevice(), (Stream) fileStream);
          int length = this.Texture.get_Width() * this.Texture.get_Height();
          Color[] colorArray = new Color[length];
          this.Texture.GetData<Color>((M0[]) colorArray, 0, length);
          fixed (Color* colorPtr = colorArray)
          {
            for (int index = 0; index < length; ++index)
            {
              ((Color) (IntPtr) (colorPtr + index)).set_R((byte) ((double) ((Color) (IntPtr) (colorPtr + index)).get_R() * ((double) ((Color) (IntPtr) (colorPtr + index)).get_A() / (double) byte.MaxValue)));
              ((Color) (IntPtr) (colorPtr + index)).set_G((byte) ((double) ((Color) (IntPtr) (colorPtr + index)).get_G() * ((double) ((Color) (IntPtr) (colorPtr + index)).get_A() / (double) byte.MaxValue)));
              ((Color) (IntPtr) (colorPtr + index)).set_B((byte) ((double) ((Color) (IntPtr) (colorPtr + index)).get_B() * ((double) ((Color) (IntPtr) (colorPtr + index)).get_A() / (double) byte.MaxValue)));
            }
          }
          this.Texture.SetData<Color>((M0[]) colorArray, 0, length);
        }
        else if (extension == ".xnb")
        {
          this.Texture = (Texture2D) Engine.Instance.get_Content().Load<Texture2D>(this.Path.Replace(".xnb", ""));
        }
        else
        {
          using (FileStream fileStream = File.OpenRead(System.IO.Path.Combine(Engine.ContentDirectory, this.Path)))
            this.Texture = Texture2D.FromStream(Engine.Graphics.get_GraphicsDevice(), (Stream) fileStream);
        }
        this.Width = this.Texture.get_Width();
        this.Height = this.Texture.get_Height();
      }
    }

    public override void Dispose()
    {
      this.Unload();
      this.Texture = (Texture2D) null;
      VirtualContent.Remove((VirtualAsset) this);
    }
  }
}
