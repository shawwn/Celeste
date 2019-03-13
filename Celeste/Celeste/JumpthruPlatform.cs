// Decompiled with JetBrains decompiler
// Type: Celeste.JumpthruPlatform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class JumpthruPlatform : JumpThru
  {
    private int columns;
    private string overrideTexture;

    public JumpthruPlatform(Vector2 position, int width, string overrideTexture)
      : base(position, width, true)
    {
      this.columns = width / 8;
      this.Depth = -60;
      this.overrideTexture = overrideTexture;
    }

    public JumpthruPlatform(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Attr("texture", "default"))
    {
    }

    public override void Awake(Scene scene)
    {
      string str = AreaData.Get(scene).Jumpthru;
      if (!string.IsNullOrEmpty(this.overrideTexture) && !this.overrideTexture.Equals("default"))
        str = this.overrideTexture;
      string lower = str.ToLower();
      if (!(lower == "dream"))
      {
        if (!(lower == "temple") && !(lower == "templeb"))
        {
          if (!(lower == "core"))
          {
            if (lower == "wood" || lower == "cliffside")
              ;
            this.SurfaceSoundIndex = 5;
          }
          else
            this.SurfaceSoundIndex = 3;
        }
        else
          this.SurfaceSoundIndex = 8;
      }
      else
        this.SurfaceSoundIndex = 32;
      MTexture mtexture = GFX.Game["objects/jumpthru/" + str];
      int num1 = mtexture.Width / 8;
      for (int index = 0; index < this.columns; ++index)
      {
        int num2;
        int num3;
        if (index == 0)
        {
          num2 = 0;
          num3 = this.CollideCheck<Solid>(this.Position + new Vector2(-1f, 0.0f)) ? 0 : 1;
        }
        else if (index == this.columns - 1)
        {
          num2 = num1 - 1;
          num3 = this.CollideCheck<Solid>(this.Position + new Vector2(1f, 0.0f)) ? 0 : 1;
        }
        else
        {
          num2 = 1 + Calc.Random.Next(num1 - 2);
          num3 = Calc.Random.Choose<int>(0, 1);
        }
        Monocle.Image image = new Monocle.Image(mtexture.GetSubtexture(num2 * 8, num3 * 8, 8, 8, (MTexture) null));
        image.X = (float) (index * 8);
        this.Add((Component) image);
      }
    }
  }
}

