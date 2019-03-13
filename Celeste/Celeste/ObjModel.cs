// Decompiled with JetBrains decompiler
// Type: Celeste.ObjModel
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Celeste
{
  public class ObjModel : IDisposable
  {
    public List<ObjModel.Mesh> Meshes = new List<ObjModel.Mesh>();
    public VertexBuffer Vertices;
    private VertexPositionTexture[] verts;

    private bool ResetVertexBuffer()
    {
      if (this.Vertices != null && !this.Vertices.IsDisposed && !this.Vertices.GraphicsDevice.IsDisposed)
        return false;
      this.Vertices = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof (VertexPositionTexture), this.verts.Length, BufferUsage.None);
      this.Vertices.SetData<VertexPositionTexture>(this.verts);
      return true;
    }

    public void ReassignVertices()
    {
      if (this.ResetVertexBuffer())
        return;
      this.Vertices.SetData<VertexPositionTexture>(this.verts);
    }

    public void Draw(Effect effect)
    {
      this.ResetVertexBuffer();
      Engine.Graphics.GraphicsDevice.SetVertexBuffer(this.Vertices);
      foreach (EffectPass pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();
        Engine.Graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.Vertices.VertexCount / 3);
      }
    }

    public void Dispose()
    {
      this.Vertices.Dispose();
      this.Meshes = (List<ObjModel.Mesh>) null;
    }

    public static ObjModel Create(string filename)
    {
      Path.GetDirectoryName(filename);
      ObjModel objModel = new ObjModel();
      ObjModel.Mesh mesh = (ObjModel.Mesh) null;
      List<VertexPositionTexture> vertexPositionTextureList = new List<VertexPositionTexture>();
      List<Vector3> vector3List = new List<Vector3>();
      List<Vector2> vector2List = new List<Vector2>();
      using (StreamReader streamReader = new StreamReader(filename))
      {
        string str1;
        while ((str1 = streamReader.ReadLine()) != null)
        {
          string[] strArray1 = str1.Split(' ');
          if (strArray1.Length != 0)
          {
            string str2 = strArray1[0];
            if (str2 == "o")
            {
              if (mesh != null)
                mesh.VertexCount = vertexPositionTextureList.Count - mesh.VertexStart;
              mesh = new ObjModel.Mesh();
              mesh.Name = strArray1[1];
              mesh.VertexStart = vertexPositionTextureList.Count;
              objModel.Meshes.Add(mesh);
            }
            else if (str2 == "v")
            {
              Vector3 vector3 = new Vector3(ObjModel.Float(strArray1[1]), ObjModel.Float(strArray1[2]), ObjModel.Float(strArray1[3]));
              vector3List.Add(vector3);
            }
            else if (str2 == "vt")
            {
              Vector2 vector2 = new Vector2(ObjModel.Float(strArray1[1]), ObjModel.Float(strArray1[2]));
              vector2List.Add(vector2);
            }
            else if (str2 == "f")
            {
              for (int index = 1; index < Math.Min(4, strArray1.Length); ++index)
              {
                VertexPositionTexture vertexPositionTexture = new VertexPositionTexture();
                string[] strArray2 = strArray1[index].Split('/');
                if (strArray2[0].Length > 0)
                  vertexPositionTexture.Position = vector3List[int.Parse(strArray2[0]) - 1];
                if (strArray2[1].Length > 0)
                  vertexPositionTexture.TextureCoordinate = vector2List[int.Parse(strArray2[1]) - 1];
                vertexPositionTextureList.Add(vertexPositionTexture);
              }
            }
          }
        }
      }
      if (mesh != null)
        mesh.VertexCount = vertexPositionTextureList.Count - mesh.VertexStart;
      objModel.verts = vertexPositionTextureList.ToArray();
      objModel.ResetVertexBuffer();
      return objModel;
    }

    private static float Float(string data)
    {
      return float.Parse(data, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public class Mesh
    {
      public string Name = "";
      public ObjModel Model;
      public int VertexStart;
      public int VertexCount;
    }
  }
}

