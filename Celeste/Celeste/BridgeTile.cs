// Decompiled with JetBrains decompiler
// Type: Celeste.BridgeTile
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class BridgeTile : JumpThru
  {
    private List<Monocle.Image> images;
    private Vector2 shakeOffset;
    private float shakeTimer;
    private float speedY;
    private float colorLerp;

    public bool Fallen { get; private set; }

    public BridgeTile(Vector2 position, Rectangle tileSize)
      : base(position, tileSize.Width, false)
    {
      this.images = new List<Monocle.Image>();
      if (tileSize.Width == 16)
      {
        int height = 24;
        int y = 0;
        while (y < tileSize.Height)
        {
          Monocle.Image image;
          this.Add((Component) (image = new Monocle.Image(GFX.Game["scenery/bridge"].GetSubtexture(tileSize.X, y, tileSize.Width, height, (MTexture) null))));
          image.Origin = new Vector2(image.Width / 2f, 0.0f);
          image.X = image.Width / 2f;
          image.Y = (float) (y - 8);
          this.images.Add(image);
          y += height;
          height = 12;
        }
      }
      else
      {
        Monocle.Image image;
        this.Add((Component) (image = new Monocle.Image(GFX.Game["scenery/bridge"].GetSubtexture(tileSize))));
        image.Origin = new Vector2(image.Width / 2f, 0.0f);
        image.X = image.Width / 2f;
        image.Y = -8f;
        this.images.Add(image);
      }
    }

    public override void Update()
    {
      base.Update();
      bool flag = (double) this.images[0].Width == 16.0;
      if (!this.Fallen)
        return;
      if ((double) this.shakeTimer > 0.0)
      {
        this.shakeTimer -= Engine.DeltaTime;
        if (this.Scene.OnInterval(0.02f))
          this.shakeOffset = Calc.Random.ShakeVector();
        if ((double) this.shakeTimer <= 0.0)
        {
          this.Collidable = false;
          this.SceneAs<Level>().Shake(0.1f);
          Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
          if (flag)
          {
            Audio.Play("event:/game/00_prologue/bridge_support_break", this.Position);
            foreach (Monocle.Image image in this.images)
            {
              if ((double) image.RenderPosition.Y > (double) this.Y + 4.0)
                Dust.Burst(image.RenderPosition, -1.570796f, 8);
            }
          }
        }
        this.images[0].Position = new Vector2(this.images[0].Width / 2f, -8f) + this.shakeOffset;
      }
      else
      {
        this.colorLerp = Calc.Approach(this.colorLerp, 1f, 10f * Engine.DeltaTime);
        this.images[0].Color = Color.Lerp(Color.White, Color.Gray, this.colorLerp);
        this.shakeOffset = Vector2.Zero;
        if (flag)
        {
          int num = 0;
          foreach (Monocle.Image image in this.images)
          {
            image.Rotation -= (float) ((num % 2 == 0 ? -1.0 : 1.0) * (double) Engine.DeltaTime * (double) num * 2.0);
            image.Y += (float) ((double) num * (double) Engine.DeltaTime * 16.0);
            ++num;
          }
          this.speedY = Calc.Approach(this.speedY, 120f, 600f * Engine.DeltaTime);
        }
        else
          this.speedY = Calc.Approach(this.speedY, 200f, 900f * Engine.DeltaTime);
        this.MoveV(this.speedY * Engine.DeltaTime);
        if ((double) this.Top > 220.0)
          this.RemoveSelf();
      }
    }

    public void Fall(float timer = 0.2f)
    {
      if (this.Fallen)
        return;
      this.Fallen = true;
      this.shakeTimer = timer;
    }
  }
}

