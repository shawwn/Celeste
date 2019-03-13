// Decompiled with JetBrains decompiler
// Type: Celeste.StarsBG
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class StarsBG : Backdrop
  {
    private const int StarCount = 100;
    private StarsBG.Star[] stars;
    private Color[] colors;
    private List<List<MTexture>> textures;
    private float falling;
    private Vector2 center;

    public StarsBG()
    {
      this.textures = new List<List<MTexture>>();
      this.textures.Add(GFX.Game.GetAtlasSubtextures("bgs/02/stars/a"));
      this.textures.Add(GFX.Game.GetAtlasSubtextures("bgs/02/stars/b"));
      this.textures.Add(GFX.Game.GetAtlasSubtextures("bgs/02/stars/c"));
      this.center = Vector2.op_Division(new Vector2((float) this.textures[0][0].Width, (float) this.textures[0][0].Height), 2f);
      this.stars = new StarsBG.Star[100];
      for (int index = 0; index < this.stars.Length; ++index)
        this.stars[index] = new StarsBG.Star()
        {
          Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(180f)),
          Timer = Calc.Random.NextFloat(6.283185f),
          Rate = 2f + Calc.Random.NextFloat(2f),
          TextureSet = Calc.Random.Next(this.textures.Count)
        };
      this.colors = new Color[8];
      for (int index = 0; index < this.colors.Length; ++index)
        this.colors[index] = Color.op_Multiply(Color.op_Multiply(Color.get_Teal(), 0.7f), (float) (1.0 - (double) index / (double) this.colors.Length));
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if (!this.Visible)
        return;
      Level level = scene as Level;
      for (int index = 0; index < this.stars.Length; ++index)
        this.stars[index].Timer += Engine.DeltaTime * this.stars[index].Rate;
      if (!level.Session.Dreaming)
        return;
      this.falling += Engine.DeltaTime * 12f;
    }

    public override void Render(Scene scene)
    {
      Draw.Rect(0.0f, 0.0f, 320f, 180f, Color.get_Black());
      Level level = scene as Level;
      Color color = Color.get_White();
      int num = 100;
      if (level.Session.Dreaming)
        color = Color.op_Multiply(Color.get_Teal(), 0.7f);
      else
        num /= 2;
      for (int index1 = 0; index1 < num; ++index1)
      {
        List<MTexture> texture = this.textures[this.stars[index1].TextureSet];
        int index2 = (int) ((Math.Sin((double) this.stars[index1].Timer) + 1.0) / 2.0 * (double) texture.Count) % texture.Count;
        Vector2 position = this.stars[index1].Position;
        MTexture mtexture = texture[index2];
        if (level.Session.Dreaming)
        {
          ref __Null local1 = ref position.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local1 = ^(float&) ref local1 - level.Camera.Y;
          ref __Null local2 = ref position.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local2 = ^(float&) ref local2 + this.falling * this.stars[index1].Rate;
          ref __Null local3 = ref position.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local3 = ^(float&) ref local3 % 180f;
          if (position.Y < 0.0)
          {
            ref __Null local4 = ref position.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local4 = ^(float&) ref local4 + 180f;
          }
          for (int index3 = 0; index3 < this.colors.Length; ++index3)
            mtexture.Draw(Vector2.op_Subtraction(position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) index3)), this.center, this.colors[index3]);
        }
        mtexture.Draw(position, this.center, color);
      }
    }

    private struct Star
    {
      public Vector2 Position;
      public int TextureSet;
      public float Timer;
      public float Rate;
    }
  }
}
