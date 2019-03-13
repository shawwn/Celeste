// Decompiled with JetBrains decompiler
// Type: Celeste.ResortMirror
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
  public class ResortMirror : Entity
  {
    private MTexture glassfg = GFX.Game["objects/mirror/glassfg"];
    private float shineAlpha = 0.7f;
    private float mirrorAlpha = 0.7f;
    private bool smashed;
    private Monocle.Image bg;
    private Monocle.Image frame;
    private Sprite breakingGlass;
    private VirtualRenderTarget mirror;
    private BadelineDummy evil;
    private bool shardReflection;

    public ResortMirror(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      this.Depth = 9500;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.smashed = this.SceneAs<Level>().Session.GetFlag("oshiro_resort_suite");
      Entity entity = new Entity(this.Position)
      {
        Depth = 9000
      };
      entity.Add((Component) (this.frame = new Monocle.Image(GFX.Game["objects/mirror/resortframe"])));
      this.frame.JustifyOrigin(0.5f, 1f);
      this.Scene.Add(entity);
      MTexture glassbg = GFX.Game["objects/mirror/glassbg"];
      int w = (int) this.frame.Width - 2;
      int h = (int) this.frame.Height - 12;
      if (!this.smashed)
        this.mirror = VirtualContent.CreateRenderTarget("resort-mirror", w, h, false, true, 0);
      else
        glassbg = GFX.Game["objects/mirror/glassbreak09"];
      this.Add((Component) (this.bg = new Monocle.Image(glassbg.GetSubtexture((glassbg.Width - w) / 2, glassbg.Height - h, w, h, (MTexture) null))));
      this.bg.JustifyOrigin(0.5f, 1f);
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/mirror/mirrormask");
      MTexture temp = new MTexture();
      foreach (MTexture mtexture in atlasSubtextures)
      {
        MTexture shard = mtexture;
        MirrorSurface surface = new MirrorSurface((Action) null);
        surface.OnRender = (Action) (() => shard.GetSubtexture((glassbg.Width - w) / 2, glassbg.Height - h, w, h, temp).DrawJustified(this.Position, new Vector2(0.5f, 1f), Color.op_Multiply(surface.ReflectionColor, this.shardReflection ? 1f : 0.0f)));
        surface.ReflectionOffset = new Vector2((float) (9 + Calc.Random.Range(-4, 4)), (float) (4 + Calc.Random.Range(-2, 2)));
        this.Add((Component) surface);
      }
    }

    public void EvilAppear()
    {
      this.Add((Component) new Coroutine(this.EvilAppearRoutine(), true));
      this.Add((Component) new Coroutine(this.FadeLights(), true));
    }

    private IEnumerator EvilAppearRoutine()
    {
      this.evil = new BadelineDummy(new Vector2((float) (this.mirror.Width + 8), (float) this.mirror.Height));
      yield return (object) this.evil.WalkTo((float) (this.mirror.Width / 2));
    }

    private IEnumerator FadeLights()
    {
      Level level = this.SceneAs<Level>();
      while ((double) level.Lighting.Alpha != 0.349999994039536)
      {
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, 0.35f, Engine.DeltaTime * 0.1f);
        yield return (object) null;
      }
    }

    public IEnumerator SmashRoutine()
    {
      ResortMirror resortMirror = this;
      yield return (object) resortMirror.evil.FloatTo(new Vector2((float) (resortMirror.mirror.Width / 2), (float) (resortMirror.mirror.Height - 8)), new int?(), true, false);
      resortMirror.breakingGlass = GFX.SpriteBank.Create("glass");
      resortMirror.breakingGlass.Position = new Vector2((float) (resortMirror.mirror.Width / 2), (float) resortMirror.mirror.Height);
      resortMirror.breakingGlass.Play("break", false, false);
      resortMirror.breakingGlass.Color = Color.op_Multiply(Color.get_White(), resortMirror.shineAlpha);
      Input.Rumble(RumbleStrength.Light, RumbleLength.FullSecond);
      while (resortMirror.breakingGlass.CurrentAnimationID == "break")
      {
        if (resortMirror.breakingGlass.CurrentAnimationFrame == 7)
          resortMirror.SceneAs<Level>().Shake(0.3f);
        resortMirror.shineAlpha = Calc.Approach(resortMirror.shineAlpha, 1f, Engine.DeltaTime * 2f);
        resortMirror.mirrorAlpha = Calc.Approach(resortMirror.mirrorAlpha, 1f, Engine.DeltaTime * 2f);
        yield return (object) null;
      }
      resortMirror.SceneAs<Level>().Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      for (float num1 = (float) (-(double) resortMirror.breakingGlass.Width / 2.0); (double) num1 < (double) resortMirror.breakingGlass.Width / 2.0; num1 += 8f)
      {
        for (float num2 = -resortMirror.breakingGlass.Height; (double) num2 < 0.0; num2 += 8f)
        {
          if (Calc.Random.Chance(0.5f))
            (resortMirror.Scene as Level).Particles.Emit(DreamMirror.P_Shatter, 2, Vector2.op_Addition(resortMirror.Position, new Vector2(num1 + 4f, num2 + 4f)), new Vector2(8f, 8f), new Vector2(num1, num2).Angle());
        }
      }
      resortMirror.shardReflection = true;
      resortMirror.evil = (BadelineDummy) null;
    }

    public void Broken()
    {
      this.evil = (BadelineDummy) null;
      this.smashed = true;
      this.shardReflection = true;
      MTexture mtexture = GFX.Game["objects/mirror/glassbreak09"];
      this.bg.Texture = mtexture.GetSubtexture((int) ((double) mtexture.Width - (double) this.bg.Width) / 2, mtexture.Height - (int) this.bg.Height, (int) this.bg.Width, (int) this.bg.Height, (MTexture) null);
    }

    private void BeforeRender()
    {
      if (this.smashed || this.mirror == null)
        return;
      Level level = this.SceneAs<Level>();
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.mirror);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone);
      NPC first = this.Scene.Entities.FindFirst<NPC>();
      if (first != null)
      {
        Vector2 renderPosition = first.Sprite.RenderPosition;
        first.Sprite.RenderPosition = Vector2.op_Addition(Vector2.op_Addition(Vector2.op_Subtraction(renderPosition, this.Position), new Vector2((float) (this.mirror.Width / 2), (float) this.mirror.Height)), new Vector2(8f, -4f));
        first.Sprite.Render();
        first.Sprite.RenderPosition = renderPosition;
      }
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        Vector2 position = entity.Position;
        entity.Position = Vector2.op_Addition(Vector2.op_Addition(Vector2.op_Subtraction(position, this.Position), new Vector2((float) (this.mirror.Width / 2), (float) this.mirror.Height)), new Vector2(8f, 0.0f));
        Vector2 vector2 = Vector2.op_Subtraction(entity.Position, position);
        for (int index1 = 0; index1 < entity.Hair.Nodes.Count; ++index1)
        {
          List<Vector2> nodes = entity.Hair.Nodes;
          int index2 = index1;
          nodes[index2] = Vector2.op_Addition(nodes[index2], vector2);
        }
        entity.Render();
        for (int index1 = 0; index1 < entity.Hair.Nodes.Count; ++index1)
        {
          List<Vector2> nodes = entity.Hair.Nodes;
          int index2 = index1;
          nodes[index2] = Vector2.op_Subtraction(nodes[index2], vector2);
        }
        entity.Position = position;
      }
      if (this.evil != null)
      {
        this.evil.Update();
        this.evil.Hair.Facing = (Facings) Math.Sign((float) this.evil.Sprite.Scale.X);
        this.evil.Hair.AfterUpdate();
        this.evil.Render();
      }
      if (this.breakingGlass != null)
      {
        this.breakingGlass.Color = Color.op_Multiply(Color.get_White(), this.shineAlpha);
        this.breakingGlass.Update();
        this.breakingGlass.Render();
      }
      else
      {
        int num = -(int) ((double) level.Camera.Y * 0.800000011920929 % (double) this.glassfg.Height);
        this.glassfg.DrawJustified(new Vector2((float) (this.mirror.Width / 2), (float) num), new Vector2(0.5f, 1f), Color.op_Multiply(Color.get_White(), this.shineAlpha));
        this.glassfg.DrawJustified(new Vector2((float) (this.mirror.Height / 2), (float) (num - this.glassfg.Height)), new Vector2(0.5f, 1f), Color.op_Multiply(Color.get_White(), this.shineAlpha));
      }
      Draw.SpriteBatch.End();
    }

    public override void Render()
    {
      this.bg.Render();
      if (!this.smashed)
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.mirror, Vector2.op_Addition(this.Position, new Vector2((float) (-this.mirror.Width / 2), (float) -this.mirror.Height)), Color.op_Multiply(Color.get_White(), this.mirrorAlpha));
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
