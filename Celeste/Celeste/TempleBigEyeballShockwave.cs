// Decompiled with JetBrains decompiler
// Type: Celeste.TempleBigEyeballShockwave
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Pooled]
  public class TempleBigEyeballShockwave : Entity
  {
    private MTexture distortionTexture;
    private float distortionAlpha;
    private bool hasHitPlayer;

    public TempleBigEyeballShockwave()
    {
      this.Depth = -1000000;
      this.Collider = (Collider) new Hitbox(48f, 200f, -30f, -100f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      MTexture mtexture = GFX.Game["util/displacementcirclehollow"];
      this.distortionTexture = mtexture.GetSubtexture(0, 0, mtexture.Width / 2, mtexture.Height, (MTexture) null);
      this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
    }

    public TempleBigEyeballShockwave Init(Vector2 position)
    {
      this.Position = position;
      this.Collidable = true;
      this.distortionAlpha = 0.0f;
      this.hasHitPlayer = false;
      return this;
    }

    public override void Update()
    {
      base.Update();
      this.X -= 300f * Engine.DeltaTime;
      this.distortionAlpha = Calc.Approach(this.distortionAlpha, 1f, Engine.DeltaTime * 4f);
      double x = (double) this.X;
      Rectangle bounds = this.SceneAs<Level>().Bounds;
      double num = (double) (((Rectangle) ref bounds).get_Left() - 20);
      if (x >= num)
        return;
      this.RemoveSelf();
    }

    private void RenderDisplacement()
    {
      this.distortionTexture.DrawCentered(this.Position, Color.op_Multiply(Color.op_Multiply(Color.get_White(), 0.8f), this.distortionAlpha), new Vector2(0.9f, 1.5f));
    }

    private void OnPlayer(Player player)
    {
      if (player.StateMachine.State == 2)
        return;
      player.Speed.X = (__Null) -100.0;
      if (player.Speed.Y > 30.0)
        player.Speed.Y = (__Null) 30.0;
      if (this.hasHitPlayer)
        return;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/game/05_mirror_temple/eye_pulse", player.Position);
      this.hasHitPlayer = true;
    }
  }
}
