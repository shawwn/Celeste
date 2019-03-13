// Decompiled with JetBrains decompiler
// Type: Celeste.SinkingPlatform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class SinkingPlatform : JumpThru
  {
    private float speed;
    private float startY;
    private float riseTimer;
    private MTexture[] textures;
    private Shaker shaker;
    private SoundSource downSfx;
    private SoundSource upSfx;

    public SinkingPlatform(Vector2 position, int width)
      : base(position, width, false)
    {
      this.startY = this.Y;
      this.Depth = 1;
      this.SurfaceSoundIndex = 15;
      this.Add((Component) (this.shaker = new Shaker(false, (Action<Vector2>) null)));
      this.Add((Component) new LightOcclude(0.2f));
      this.Add((Component) (this.downSfx = new SoundSource()));
      this.Add((Component) (this.upSfx = new SoundSource()));
    }

    public SinkingPlatform(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      MTexture mtexture = GFX.Game["objects/woodPlatform/" + AreaData.Get(scene).WoodPlatform];
      this.textures = new MTexture[mtexture.Width / 8];
      for (int index = 0; index < this.textures.Length; ++index)
        this.textures[index] = mtexture.GetSubtexture(index * 8, 0, 8, 8, (MTexture) null);
      scene.Add((Entity) new SinkingPlatformLine(this.Position + new Vector2(this.Width / 2f, this.Height / 2f)));
    }

    public override void Render()
    {
      Vector2 vector2 = this.shaker.Value;
      this.textures[0].Draw(this.Position + vector2);
      for (int index = 8; (double) index < (double) this.Width - 8.0; index += 8)
        this.textures[1].Draw(this.Position + vector2 + new Vector2((float) index, 0.0f));
      this.textures[3].Draw(this.Position + vector2 + new Vector2(this.Width - 8f, 0.0f));
      this.textures[2].Draw(this.Position + vector2 + new Vector2((float) ((double) this.Width / 2.0 - 4.0), 0.0f));
    }

    public override void Update()
    {
      base.Update();
      Player playerRider = this.GetPlayerRider();
      if (playerRider != null)
      {
        if ((double) this.riseTimer <= 0.0)
        {
          if ((double) this.ExactPosition.Y <= (double) this.startY)
            Audio.Play("event:/game/03_resort/platform_vert_start", this.Position);
          this.shaker.ShakeFor(0.15f, false);
        }
        this.riseTimer = 0.1f;
        this.speed = Calc.Approach(this.speed, playerRider.Ducking ? 60f : 30f, 400f * Engine.DeltaTime);
      }
      else if ((double) this.riseTimer > 0.0)
      {
        this.riseTimer -= Engine.DeltaTime;
        this.speed = Calc.Approach(this.speed, 45f, 600f * Engine.DeltaTime);
      }
      else
        this.speed = Calc.Approach(this.speed, -50f, 400f * Engine.DeltaTime);
      if ((double) this.speed > 0.0)
      {
        if (!this.downSfx.Playing)
          this.downSfx.Play("event:/game/03_resort/platform_vert_down_loop", (string) null, 0.0f);
        this.downSfx.Param("ducking", playerRider == null || !playerRider.Ducking ? 0.0f : 1f);
        if (this.upSfx.Playing)
          this.upSfx.Stop(true);
        this.MoveV(this.speed * Engine.DeltaTime);
      }
      else
      {
        if ((double) this.speed >= 0.0 || (double) this.ExactPosition.Y <= (double) this.startY)
          return;
        if (!this.upSfx.Playing)
          this.upSfx.Play("event:/game/03_resort/platform_vert_up_loop", (string) null, 0.0f);
        if (this.downSfx.Playing)
          this.downSfx.Stop(true);
        this.MoveTowardsY(this.startY, -this.speed * Engine.DeltaTime);
        if ((double) this.ExactPosition.Y <= (double) this.startY)
        {
          this.upSfx.Stop(true);
          Audio.Play("event:/game/03_resort/platform_vert_end", this.Position);
          this.shaker.ShakeFor(0.1f, false);
        }
      }
    }
  }
}

