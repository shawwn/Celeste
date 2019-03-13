// Decompiled with JetBrains decompiler
// Type: Celeste.TempleMirrorPortal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class TempleMirrorPortal : Entity
  {
    public float DistortionFade = 1f;
    private int switchCounter = 0;
    private float bufferAlpha = 0.0f;
    private float bufferTimer = 0.0f;
    private TempleMirrorPortal.Debris[] debris = new TempleMirrorPortal.Debris[50];
    private Color debrisColorFrom = Calc.HexToColor("f442d4");
    private Color debrisColorTo = Calc.HexToColor("000000");
    private MTexture debrisTexture = GFX.Game["particles/blob"];
    public static ParticleType P_CurtainDrop;
    private bool canTrigger;
    private VirtualRenderTarget buffer;
    private TempleMirrorPortal.Curtain curtain;
    private TemplePortalTorch leftTorch;
    private TemplePortalTorch rightTorch;

    public TempleMirrorPortal(Vector2 position)
      : base(position)
    {
      this.Depth = 2000;
      this.Collider = (Collider) new Hitbox(120f, 64f, -60f, -32f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public TempleMirrorPortal(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add((Entity) (this.curtain = new TempleMirrorPortal.Curtain(this.Position)));
      scene.Add((Entity) new TempleMirrorPortal.Bg(this.Position));
      scene.Add((Entity) (this.leftTorch = new TemplePortalTorch(this.Position + new Vector2(-90f, 0.0f))));
      scene.Add((Entity) (this.rightTorch = new TemplePortalTorch(this.Position + new Vector2(90f, 0.0f))));
    }

    public void OnSwitchHit(int side)
    {
      this.Add((Component) new Coroutine(this.OnSwitchRoutine(side), true));
    }

    private IEnumerator OnSwitchRoutine(int side)
    {
      yield return (object) 0.4f;
      if (side < 0)
        this.leftTorch.Light(this.switchCounter);
      else
        this.rightTorch.Light(this.switchCounter);
      ++this.switchCounter;
      Session session = (this.Scene as Level).Session;
      if (session.Area.Mode == AreaMode.Normal)
      {
        LightingRenderer lighting = (this.Scene as Level).Lighting;
        float lightTarget = Math.Max(0.0f, lighting.Alpha - 0.2f);
        while ((double) (lighting.Alpha -= Engine.DeltaTime) > (double) lightTarget)
          yield return (object) null;
        lighting = (LightingRenderer) null;
      }
      yield return (object) 0.15f;
      if (this.switchCounter >= 2)
      {
        yield return (object) 0.1f;
        Audio.Play("event:/game/05_mirror_temple/mainmirror_reveal", this.Position);
        this.curtain.Drop();
        this.canTrigger = true;
        yield return (object) 0.1f;
        Level level = this.SceneAs<Level>();
        for (int i = 0; i < 120; i += 12)
        {
          for (int j = 0; j < 60; j += 6)
            level.Particles.Emit(TempleMirrorPortal.P_CurtainDrop, 1, this.curtain.Position + new Vector2((float) (i - 57), (float) (j - 27)), new Vector2(6f, 3f));
        }
        level = (Level) null;
      }
    }

    public void Activate()
    {
      this.Add((Component) new Coroutine(this.ActivateRoutine(), true));
    }

    private IEnumerator ActivateRoutine()
    {
      Level level = this.Scene as Level;
      LightingRenderer light = level.Lighting;
      float debrisStart = 0.0f;
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
      while (true)
      {
        this.bufferAlpha = Calc.Approach(this.bufferAlpha, 1f, Engine.DeltaTime);
        this.bufferTimer += 4f * Engine.DeltaTime;
        light.Alpha = Calc.Approach(light.Alpha, 0.2f, Engine.DeltaTime * 0.25f);
        if ((double) debrisStart < (double) this.debris.Length)
        {
          int index = (int) debrisStart;
          this.debris[index].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 1f);
          this.debris[index].Enabled = true;
          this.debris[index].Duration = 0.5f + Calc.Random.NextFloat(0.7f);
        }
        debrisStart += Engine.DeltaTime * 10f;
        for (int i = 0; i < this.debris.Length; ++i)
        {
          if (this.debris[i].Enabled)
          {
            this.debris[i].Percent %= 1f;
            this.debris[i].Percent += Engine.DeltaTime / this.debris[i].Duration;
          }
        }
        yield return (object) null;
      }
    }

    private void BeforeRender()
    {
      if (this.buffer == null)
        this.buffer = VirtualContent.CreateRenderTarget("temple-portal", 120, 64, false, true, 0);
      Vector2 position = new Vector2((float) this.buffer.Width, (float) this.buffer.Height) / 2f;
      MTexture mtexture = GFX.Game["objects/temple/portal/portal"];
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.buffer);
      Engine.Graphics.GraphicsDevice.Clear(Color.Black);
      Draw.SpriteBatch.Begin();
      for (int index = 0; (double) index < 10.0; ++index)
      {
        float amount = (float) ((double) this.bufferTimer % 1.0 * 0.100000001490116 + (double) index / 10.0);
        Color color = Color.Lerp(Color.Black, Color.Purple, amount);
        float scale = amount;
        float rotation = 6.283185f * amount;
        mtexture.DrawCentered(position, color, scale, rotation);
      }
      Draw.SpriteBatch.End();
    }

    private void RenderDisplacement()
    {
      Draw.Rect(this.X - 60f, this.Y - 32f, 120f, 64f, new Color(0.5f, 0.5f, 0.25f * this.DistortionFade * this.bufferAlpha, 1f));
    }

    public override void Render()
    {
      base.Render();
      if (this.buffer != null)
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.buffer, this.Position + new Vector2((float) (-(double) this.Collider.Width / 2.0), (float) (-(double) this.Collider.Height / 2.0)), Color.White * this.bufferAlpha);
      GFX.Game["objects/temple/portal/portalframe"].DrawCentered(this.Position);
      Level scene = this.Scene as Level;
      for (int index = 0; index < this.debris.Length; ++index)
      {
        TempleMirrorPortal.Debris debri = this.debris[index];
        if (debri.Enabled)
        {
          float num = Ease.SineOut(debri.Percent);
          this.debrisTexture.DrawCentered(this.Position + debri.Direction * (1f - num) * (float) (190.0 - (double) scene.Zoom * 30.0), Color.Lerp(this.debrisColorFrom, this.debrisColorTo, num), Calc.LerpClamp(1f, 0.2f, num), (float) index * 0.05f);
        }
      }
    }

    private void OnPlayer(Player player)
    {
      if (!this.canTrigger)
        return;
      this.canTrigger = false;
      this.Scene.Add((Entity) new CS04_MirrorPortal(player, this));
    }

    public override void Removed(Scene scene)
    {
      this.Dispose();
      base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      this.Dispose();
      base.SceneEnd(scene);
    }

    private void Dispose()
    {
      if (this.buffer != null)
        this.buffer.Dispose();
      this.buffer = (VirtualRenderTarget) null;
    }

    private struct Debris
    {
      public Vector2 Direction;
      public float Percent;
      public float Duration;
      public bool Enabled;
    }

    private class Bg : Entity
    {
      private MirrorSurface surface;
      private Vector2[] offsets;
      private List<MTexture> textures;

      public Bg(Vector2 position)
        : base(position)
      {
        this.Depth = 9500;
        this.textures = GFX.Game.GetAtlasSubtextures("objects/temple/portal/reflection");
        Vector2 vector2 = new Vector2(10f, 4f);
        this.offsets = new Vector2[this.textures.Count];
        for (int index = 0; index < this.offsets.Length; ++index)
          this.offsets[index] = vector2 + new Vector2((float) Calc.Random.Range(-4, 4), (float) Calc.Random.Range(-4, 4));
        this.Add((Component) (this.surface = new MirrorSurface((Action) null)));
        this.surface.OnRender = (Action) (() =>
        {
          for (int index = 0; index < this.textures.Count; ++index)
          {
            this.surface.ReflectionOffset = this.offsets[index];
            this.textures[index].DrawCentered(this.Position, this.surface.ReflectionColor);
          }
        });
      }

      public override void Render()
      {
        GFX.Game["objects/temple/portal/surface"].DrawCentered(this.Position);
      }
    }

    private class Curtain : Solid
    {
      public Sprite Sprite;

      public Curtain(Vector2 position)
        : base(position, 140f, 12f, true)
      {
        this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("temple_portal_curtain")));
        this.Depth = 1999;
        this.Collider.Position.X = -70f;
        this.Collider.Position.Y = 33f;
        this.Collidable = false;
        this.SurfaceSoundIndex = 17;
      }

      public override void Update()
      {
        base.Update();
        if (!this.Collidable)
          return;
        Player player1;
        if ((player1 = this.CollideFirst<Player>(this.Position + new Vector2(-1f, 0.0f))) != null && player1.OnGround(1) && (double) Input.Aim.Value.X > 0.0)
        {
          player1.MoveV(this.Top - player1.Bottom, (Collision) null, (Solid) null);
          player1.MoveH(1f, (Collision) null, (Solid) null);
        }
        else
        {
          Player player2;
          if ((player2 = this.CollideFirst<Player>(this.Position + new Vector2(1f, 0.0f))) != null && player2.OnGround(1) && (double) Input.Aim.Value.X < 0.0)
          {
            player2.MoveV(this.Top - player2.Bottom, (Collision) null, (Solid) null);
            player2.MoveH(-1f, (Collision) null, (Solid) null);
          }
        }
      }

      public void Drop()
      {
        this.Sprite.Play("fall", false, false);
        this.Depth = -8999;
        this.Collidable = true;
        bool flag = false;
        Player player;
        while ((player = this.CollideFirst<Player>(this.Position)) != null && !flag)
        {
          this.Collidable = false;
          flag = player.MoveV(-1f, (Collision) null, (Solid) null);
          this.Collidable = true;
        }
      }
    }
  }
}

