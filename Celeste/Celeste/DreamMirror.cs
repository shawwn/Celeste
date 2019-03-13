// Decompiled with JetBrains decompiler
// Type: Celeste.DreamMirror
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class DreamMirror : Entity
  {
    private MTexture glassbg = GFX.Game["objects/mirror/glassbg"];
    private MTexture glassfg = GFX.Game["objects/mirror/glassfg"];
    private float shineAlpha = 0.5f;
    private float shineOffset = 0.0f;
    private float reflectionAlpha = 0.7f;
    private bool autoUpdateReflection = true;
    private bool updateShine = true;
    public static ParticleType P_Shatter;
    private Monocle.Image frame;
    private Sprite breakingGlass;
    private Hitbox hitbox;
    private VirtualRenderTarget mirror;
    private Entity reflection;
    private PlayerSprite reflectionSprite;
    private PlayerHair reflectionHair;
    private BadelineDummy badeline;
    private bool smashed;
    private bool smashEnded;
    private Coroutine smashCoroutine;
    private SoundSource sfx;
    private SoundSource sfxSting;

    public DreamMirror(Vector2 position)
      : base(position)
    {
      this.Depth = 9500;
      this.Add((Component) (this.breakingGlass = GFX.SpriteBank.Create("glass")));
      this.breakingGlass.Play("idle", false, false);
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      foreach (MTexture atlasSubtexture in GFX.Game.GetAtlasSubtextures("objects/mirror/mirrormask"))
      {
        MTexture shard = atlasSubtexture;
        MirrorSurface surface = new MirrorSurface((Action) null);
        surface.OnRender = (Action) (() => shard.DrawJustified(this.Position, new Vector2(0.5f, 1f), surface.ReflectionColor * (this.smashEnded ? 1f : 0.0f)));
        surface.ReflectionOffset = new Vector2((float) (9 + Calc.Random.Range(-4, 4)), (float) (4 + Calc.Random.Range(-2, 2)));
        this.Add((Component) surface);
      }
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.smashed = this.SceneAs<Level>().Session.Inventory.DreamDash;
      if (this.smashed)
      {
        this.breakingGlass.Play("broken", false, false);
        this.smashEnded = true;
      }
      else
      {
        this.reflection = new Entity();
        this.reflectionSprite = new PlayerSprite(PlayerSpriteMode.Badeline);
        this.reflectionHair = new PlayerHair(this.reflectionSprite);
        this.reflectionHair.Color = BadelineOldsite.HairColor;
        this.reflectionHair.Border = Color.Black;
        this.reflection.Add((Component) this.reflectionHair);
        this.reflection.Add((Component) this.reflectionSprite);
        this.reflectionHair.Start();
        this.reflectionSprite.OnFrameChange = (Action<string>) (anim =>
        {
          if (this.smashed || !this.CollideCheck<Player>())
            return;
          int currentAnimationFrame = this.reflectionSprite.CurrentAnimationFrame;
          if (anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6) || anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6))
            Audio.Play("event:/char/badeline/footstep", this.Center);
        });
        this.Add((Component) (this.smashCoroutine = new Coroutine(this.InteractRoutine(), true)));
      }
      Entity entity = new Entity(this.Position)
      {
        Depth = 9000
      };
      entity.Add((Component) (this.frame = new Monocle.Image(GFX.Game["objects/mirror/frame"])));
      this.frame.JustifyOrigin(0.5f, 1f);
      this.Scene.Add(entity);
      this.Collider = (Collider) (this.hitbox = new Hitbox((float) ((int) this.frame.Width - 16), (float) ((int) this.frame.Height + 32), (float) (-(int) this.frame.Width / 2 + 8), (float) (-(int) this.frame.Height - 32)));
    }

    public override void Update()
    {
      base.Update();
      if (this.reflection == null)
        return;
      this.reflection.Update();
      this.reflectionHair.Facing = (Facings) Math.Sign(this.reflectionSprite.Scale.X);
      this.reflectionHair.AfterUpdate();
    }

    private void BeforeRender()
    {
      if (this.smashed)
        return;
      Level scene = this.Scene as Level;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        if (this.autoUpdateReflection && this.reflection != null)
        {
          this.reflection.Position = new Vector2(this.X - entity.X, entity.Y - this.Y) + this.breakingGlass.Origin;
          this.reflectionSprite.Scale.X = (float) -(int) entity.Facing * Math.Abs(entity.Sprite.Scale.X);
          this.reflectionSprite.Scale.Y = entity.Sprite.Scale.Y;
          if (this.reflectionSprite.CurrentAnimationID != entity.Sprite.CurrentAnimationID && entity.Sprite.CurrentAnimationID != null && this.reflectionSprite.Has(entity.Sprite.CurrentAnimationID))
            this.reflectionSprite.Play(entity.Sprite.CurrentAnimationID, false, false);
        }
        if (this.mirror == null)
          this.mirror = VirtualContent.CreateRenderTarget("dream-mirror", this.glassbg.Width, this.glassbg.Height, false, true, 0);
        Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.mirror);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        if (this.updateShine)
          this.shineOffset = (float) (this.glassfg.Height - (int) ((double) scene.Camera.Y * 0.800000011920929 % (double) this.glassfg.Height));
        this.glassbg.Draw(Vector2.Zero);
        if (this.reflection != null)
          this.reflection.Render();
        this.glassfg.Draw(new Vector2(0.0f, this.shineOffset), Vector2.Zero, Color.White * this.shineAlpha);
        this.glassfg.Draw(new Vector2(0.0f, this.shineOffset - (float) this.glassfg.Height), Vector2.Zero, Color.White * this.shineAlpha);
        Draw.SpriteBatch.End();
      }
    }

    private IEnumerator InteractRoutine()
    {
      Player player = (Player) null;
      while (player == null)
      {
        player = this.Scene.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      while (!this.hitbox.Collide((Entity) player))
        yield return (object) null;
      this.hitbox.Width += 32f;
      this.hitbox.Position.X -= 16f;
      Audio.SetMusic((string) null, true, true);
      while (this.hitbox.Collide((Entity) player))
        yield return (object) null;
      this.Scene.Add((Entity) new CS02_Mirror(player, this));
    }

    public IEnumerator BreakRoutine(int direction)
    {
      this.autoUpdateReflection = false;
      this.reflectionSprite.Play("runFast", false, false);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
      while ((double) Math.Abs(this.reflection.X - this.breakingGlass.Width / 2f) > 3.0)
      {
        this.reflection.X += (float) (direction * 32) * Engine.DeltaTime;
        yield return (object) null;
      }
      this.reflectionSprite.Play("idle", false, false);
      yield return (object) 0.65f;
      this.Add((Component) (this.sfx = new SoundSource()));
      this.sfx.Play("event:/game/02_old_site/sequence_mirror", (string) null, 0.0f);
      yield return (object) 0.15f;
      this.Add((Component) (this.sfxSting = new SoundSource("event:/music/lvl2/dreamblock_sting_pt2")));
      Input.Rumble(RumbleStrength.Light, RumbleLength.FullSecond);
      this.updateShine = false;
      while ((double) this.shineOffset != 33.0 || (double) this.shineAlpha < 1.0)
      {
        this.shineOffset = Calc.Approach(this.shineOffset, 33f, Engine.DeltaTime * 120f);
        this.shineAlpha = Calc.Approach(this.shineAlpha, 1f, Engine.DeltaTime * 4f);
        yield return (object) null;
      }
      this.smashed = true;
      this.breakingGlass.Play("break", false, false);
      yield return (object) 0.6f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      (this.Scene as Level).Shake(0.3f);
      for (float x = (float) (-(double) this.breakingGlass.Width / 2.0); (double) x < (double) this.breakingGlass.Width / 2.0; x += 8f)
      {
        for (float y = -this.breakingGlass.Height; (double) y < 0.0; y += 8f)
        {
          if (Calc.Random.Chance(0.5f))
            (this.Scene as Level).Particles.Emit(DreamMirror.P_Shatter, 2, this.Position + new Vector2(x + 4f, y + 4f), new Vector2(8f, 8f), new Vector2(x, y).Angle());
        }
      }
      this.smashEnded = true;
      this.badeline = new BadelineDummy(this.reflection.Position + this.Position - this.breakingGlass.Origin);
      this.badeline.Floatness = 0.0f;
      for (int i = 0; i < this.badeline.Hair.Nodes.Count; ++i)
        this.badeline.Hair.Nodes[i] = this.reflectionHair.Nodes[i];
      this.Scene.Add((Entity) this.badeline);
      this.badeline.Sprite.Play("idle", false, false);
      this.badeline.Sprite.Scale = this.reflectionSprite.Scale;
      this.reflection = (Entity) null;
      yield return (object) 1.2f;
      float speed = (float) -direction * 32f;
      this.badeline.Sprite.Scale.X = (float) -direction;
      this.badeline.Sprite.Play("runFast", false, false);
      while ((double) Math.Abs(this.badeline.X - this.X) < 60.0)
      {
        speed += (float) ((double) Engine.DeltaTime * (double) -direction * 128.0);
        this.badeline.X += speed * Engine.DeltaTime;
        yield return (object) null;
      }
      this.badeline.Sprite.Play("jumpFast", false, false);
      while ((double) Math.Abs(this.badeline.X - this.X) < 128.0)
      {
        speed += (float) ((double) Engine.DeltaTime * (double) -direction * 128.0);
        this.badeline.X += speed * Engine.DeltaTime;
        this.badeline.Y -= (float) ((double) Math.Abs(speed) * (double) Engine.DeltaTime * 0.800000011920929);
        yield return (object) null;
      }
      this.badeline.RemoveSelf();
      this.badeline = (BadelineDummy) null;
      yield return (object) 1.5f;
    }

    public void Broken(bool wasSkipped)
    {
      this.updateShine = false;
      this.smashed = true;
      this.smashEnded = true;
      this.breakingGlass.Play("broken", false, false);
      if (wasSkipped && this.badeline != null)
        this.badeline.RemoveSelf();
      if (wasSkipped && this.sfx != null)
        this.sfx.Stop(true);
      if (!wasSkipped || this.sfxSting == null)
        return;
      this.sfxSting.Stop(true);
    }

    public override void Render()
    {
      if (this.smashed)
        this.breakingGlass.Render();
      else
        Draw.SpriteBatch.Draw((Texture2D) this.mirror.Target, this.Position - this.breakingGlass.Origin, Color.White * this.reflectionAlpha);
      this.frame.Render();
    }

    public override void SceneEnd(Scene scene)
    {
      this.Dispose();
      base.SceneEnd(scene);
    }

    public override void Removed(Scene scene)
    {
      this.Dispose();
      base.Removed(scene);
    }

    private void Dispose()
    {
      if (this.mirror != null)
        this.mirror.Dispose();
      this.mirror = (VirtualRenderTarget) null;
    }
  }
}

