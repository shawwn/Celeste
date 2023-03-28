// Decompiled with JetBrains decompiler
// Type: Celeste.Decal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Celeste
{
  public class Decal : Entity
  {
    public const string Root = "decals";
    public const string MirrorMaskRoot = "mirrormasks";
    public string Name;
    public float AnimationSpeed = 12f;
    private Component image;
    public bool IsCrack;
    private List<MTexture> textures;
    private Vector2 scale;
    private float frame;
    private bool animated = true;
    private bool parallax;
    private float parallaxAmount;
    private bool scaredAnimal;
    private SineWave wave;

    public Decal(string texture, Vector2 position, Vector2 scale, int depth)
      : base(position)
    {
      this.Depth = depth;
      this.scale = scale;
      string extension = Path.GetExtension(texture);
      this.Name = Regex.Replace(Path.Combine("decals", texture.Replace(extension, "")).Replace('\\', '/'), "\\d+$", string.Empty);
      this.textures = GFX.Game.GetAtlasSubtextures(this.Name);
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      string path = this.Name.ToLower().Replace("decals/", "");
      switch (path)
      {
        case "0-prologue/house":
          this.CreateSmoke(new Vector2(36f, -28f), true);
          break;
        case "1-forsakencity/rags":
        case "1-forsakencity/ragsb":
        case "3-resort/curtain_side_a":
        case "3-resort/curtain_side_d":
          this.MakeBanner(2f, 3.5f, 2, 0.05f, true);
          break;
        case "10-farewell/bed":
        case "10-farewell/car":
        case "10-farewell/cliffside":
        case "10-farewell/floating house":
        case "10-farewell/giantcassete":
        case "10-farewell/heart_a":
        case "10-farewell/heart_b":
        case "10-farewell/reflection":
        case "10-farewell/temple":
        case "10-farewell/tower":
          this.Depth = 10001;
          this.MakeParallax(-0.15f);
          this.MakeFloaty();
          break;
        case "10-farewell/clouds/cloud_a":
        case "10-farewell/clouds/cloud_b":
        case "10-farewell/clouds/cloud_bb":
        case "10-farewell/clouds/cloud_bc":
        case "10-farewell/clouds/cloud_bd":
        case "10-farewell/clouds/cloud_c":
        case "10-farewell/clouds/cloud_cb":
        case "10-farewell/clouds/cloud_cc":
        case "10-farewell/clouds/cloud_cd":
        case "10-farewell/clouds/cloud_ce":
        case "10-farewell/clouds/cloud_d":
        case "10-farewell/clouds/cloud_db":
        case "10-farewell/clouds/cloud_dc":
        case "10-farewell/clouds/cloud_dd":
        case "10-farewell/clouds/cloud_e":
        case "10-farewell/clouds/cloud_f":
        case "10-farewell/clouds/cloud_g":
        case "10-farewell/clouds/cloud_h":
        case "10-farewell/clouds/cloud_i":
        case "10-farewell/clouds/cloud_j":
          this.Depth = -13001;
          this.MakeParallax(0.1f);
          this.scale *= 1.15f;
          break;
        case "10-farewell/coral_":
        case "10-farewell/coral_a":
        case "10-farewell/coral_b":
        case "10-farewell/coral_c":
        case "10-farewell/coral_d":
          this.MakeScaredAnimation();
          break;
        case "10-farewell/creature_a":
        case "10-farewell/creature_b":
        case "10-farewell/creature_c":
        case "10-farewell/creature_d":
        case "10-farewell/creature_e":
        case "10-farewell/creature_f":
          this.Depth = 10001;
          this.MakeParallax(-0.1f);
          this.MakeFloaty();
          break;
        case "10-farewell/finalflag":
          this.AnimationSpeed = 6f;
          this.Add(this.image = (Component) new Decal.FinalFlagDecalImage());
          break;
        case "10-farewell/glitch_a_":
        case "10-farewell/glitch_b_":
        case "10-farewell/glitch_c":
          this.frame = Calc.Random.NextFloat((float) this.textures.Count);
          break;
        case "3-resort/bridgecolumn":
          this.MakeSolid(-5f, -8f, 10f, 16f, 8);
          break;
        case "3-resort/bridgecolumntop":
          this.MakeSolid(-8f, -8f, 16f, 8f, 8);
          this.MakeSolid(-5f, 0.0f, 10f, 8f, 8);
          break;
        case "3-resort/brokenelevator":
          this.MakeSolid(-16f, -20f, 32f, 48f, 22);
          break;
        case "3-resort/roofcenter":
        case "3-resort/roofcenter_b":
        case "3-resort/roofcenter_c":
        case "3-resort/roofcenter_d":
          this.MakeSolid(-8f, -4f, 16f, 8f, 14);
          break;
        case "3-resort/roofedge":
        case "3-resort/roofedge_b":
        case "3-resort/roofedge_c":
        case "3-resort/roofedge_d":
          this.MakeSolid((double) this.scale.X < 0.0 ? 0.0f : -8f, -4f, 8f, 8f, 14);
          break;
        case "3-resort/vent":
          this.CreateSmoke(Vector2.Zero, false);
          break;
        case "4-cliffside/bridge_a":
          this.MakeSolid(-24f, 0.0f, 48f, 8f, 8, this.Depth != 9000);
          break;
        case "4-cliffside/flower_a":
        case "4-cliffside/flower_b":
        case "4-cliffside/flower_c":
        case "4-cliffside/flower_d":
          this.MakeBanner(2f, 2f, 1, 0.05f, false, 2f, true);
          break;
        case "5-temple-dark/mosaic_b":
          this.Add((Component) new BloomPoint(new Vector2(0.0f, 5f), 0.75f, 16f));
          break;
        case "5-temple/bg_mirror_a":
        case "5-temple/bg_mirror_b":
        case "5-temple/bg_mirror_shard_a":
        case "5-temple/bg_mirror_shard_b":
        case "5-temple/bg_mirror_shard_c":
        case "5-temple/bg_mirror_shard_d":
        case "5-temple/bg_mirror_shard_e":
        case "5-temple/bg_mirror_shard_f":
        case "5-temple/bg_mirror_shard_g":
        case "5-temple/bg_mirror_shard_group_a":
        case "5-temple/bg_mirror_shard_group_a_b":
        case "5-temple/bg_mirror_shard_group_a_c":
        case "5-temple/bg_mirror_shard_group_b":
        case "5-temple/bg_mirror_shard_group_c":
        case "5-temple/bg_mirror_shard_group_d":
        case "5-temple/bg_mirror_shard_group_e":
        case "5-temple/bg_mirror_shard_h":
        case "5-temple/bg_mirror_shard_i":
        case "5-temple/bg_mirror_shard_j":
        case "5-temple/bg_mirror_shard_k":
          this.scale.Y = 1f;
          this.MakeMirror(path, false);
          break;
        case "5-temple/bg_mirror_c":
        case "5-temple/statue_d":
          this.MakeMirror(path, true);
          break;
        case "6-reflection/crystal_reflection":
          this.MakeMirrorSpecialCase(path, new Vector2(-12f, 2f));
          break;
        case "7-summit/cloud_a":
        case "7-summit/cloud_b":
        case "7-summit/cloud_bb":
        case "7-summit/cloud_bc":
        case "7-summit/cloud_bd":
        case "7-summit/cloud_c":
        case "7-summit/cloud_cb":
        case "7-summit/cloud_cc":
        case "7-summit/cloud_cd":
        case "7-summit/cloud_ce":
        case "7-summit/cloud_d":
        case "7-summit/cloud_db":
        case "7-summit/cloud_dc":
        case "7-summit/cloud_dd":
        case "7-summit/cloud_e":
        case "7-summit/cloud_f":
        case "7-summit/cloud_g":
        case "7-summit/cloud_h":
        case "7-summit/cloud_i":
        case "7-summit/cloud_j":
          this.Depth = -13001;
          this.MakeParallax(0.1f);
          this.scale *= 1.15f;
          break;
        case "7-summit/summitflag":
          this.Add((Component) new SoundSource("event:/env/local/07_summit/flag_flap"));
          break;
        case "9-core/ball_a":
          this.Add(this.image = (Component) new Decal.CoreSwapImage(this.textures[0], GFX.Game["decals/9-core/ball_a_ice"]));
          break;
        case "9-core/ball_a_ice":
          this.Add(this.image = (Component) new Decal.CoreSwapImage(GFX.Game["decals/9-core/ball_a"], this.textures[0]));
          break;
        case "9-core/heart_bevel_a":
        case "9-core/heart_bevel_b":
        case "9-core/heart_bevel_c":
        case "9-core/heart_bevel_d":
          this.scale.Y = 1f;
          this.scale.X = 1f;
          break;
        case "9-core/rock_e":
          this.Add(this.image = (Component) new Decal.CoreSwapImage(this.textures[0], GFX.Game["decals/9-core/rock_e_ice"]));
          break;
        case "9-core/rock_e_ice":
          this.Add(this.image = (Component) new Decal.CoreSwapImage(GFX.Game["decals/9-core/rock_e"], this.textures[0]));
          break;
        case "generic/grass_a":
        case "generic/grass_b":
        case "generic/grass_c":
        case "generic/grass_d":
          this.MakeBanner(2f, 2f, 1, 0.05f, false, -2f);
          break;
      }
      if (this.Name.Contains("crack"))
        this.IsCrack = true;
      if (this.image != null)
        return;
      this.Add(this.image = (Component) new Decal.DecalImage());
    }

    private void MakeBanner(
      float speed,
      float amplitude,
      int sliceSize,
      float sliceSinIncrement,
      bool easeDown,
      float offset = 0.0f,
      bool onlyIfWindy = false)
    {
      Decal.Banner banner = new Decal.Banner()
      {
        WaveSpeed = speed,
        WaveAmplitude = amplitude,
        SliceSize = sliceSize,
        SliceSinIncrement = sliceSinIncrement,
        Segments = new List<List<MTexture>>(),
        EaseDown = easeDown,
        Offset = offset,
        OnlyIfWindy = onlyIfWindy
      };
      foreach (MTexture texture in this.textures)
      {
        List<MTexture> mtextureList = new List<MTexture>();
        for (int y = 0; y < texture.Height; y += sliceSize)
          mtextureList.Add(texture.GetSubtexture(0, y, texture.Width, sliceSize));
        banner.Segments.Add(mtextureList);
      }
      this.Add(this.image = (Component) banner);
    }

    private void MakeFloaty() => this.Add((Component) (this.wave = new SineWave(Calc.Random.Range(0.1f, 0.4f), Calc.Random.NextFloat() * 6.2831855f)));

    private void MakeSolid(
      float x,
      float y,
      float w,
      float h,
      int surfaceSoundIndex,
      bool blockWaterfalls = true)
    {
      Solid solid = new Solid(this.Position + new Vector2(x, y), w, h, true);
      solid.BlockWaterfalls = blockWaterfalls;
      solid.SurfaceSoundIndex = surfaceSoundIndex;
      this.Scene.Add((Entity) solid);
    }

    private void CreateSmoke(Vector2 offset, bool inbg)
    {
      Level scene = this.Scene as Level;
      ParticleEmitter particleEmitter = new ParticleEmitter(inbg ? scene.ParticlesBG : scene.ParticlesFG, ParticleTypes.Chimney, offset, new Vector2(4f, 1f), -1.5707964f, 1, 0.2f);
      this.Add((Component) particleEmitter);
      particleEmitter.SimulateCycle();
    }

    private void MakeMirror(string path, bool keepOffsetsClose)
    {
      this.Depth = 9500;
      if (keepOffsetsClose)
      {
        this.MakeMirror(path, this.GetMirrorOffset());
      }
      else
      {
        foreach (MTexture atlasSubtexture in GFX.Game.GetAtlasSubtextures("mirrormasks/" + path))
        {
          MTexture mask = atlasSubtexture;
          MirrorSurface surface = new MirrorSurface()
          {
            ReflectionOffset = this.GetMirrorOffset()
          };
          surface.OnRender = (Action) (() => mask.DrawCentered(this.Position, surface.ReflectionColor, this.scale));
          this.Add((Component) surface);
        }
      }
    }

    private void MakeMirror(string path, Vector2 offset)
    {
      this.Depth = 9500;
      foreach (MTexture atlasSubtexture in GFX.Game.GetAtlasSubtextures("mirrormasks/" + path))
      {
        MTexture mask = atlasSubtexture;
        MirrorSurface surface = new MirrorSurface()
        {
          ReflectionOffset = offset + new Vector2(Calc.Random.NextFloat(4f) - 2f, Calc.Random.NextFloat(4f) - 2f)
        };
        surface.OnRender = (Action) (() => mask.DrawCentered(this.Position, surface.ReflectionColor, this.scale));
        this.Add((Component) surface);
      }
    }

    private void MakeMirrorSpecialCase(string path, Vector2 offset)
    {
      this.Depth = 9500;
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("mirrormasks/" + path);
      for (int index = 0; index < atlasSubtextures.Count; ++index)
      {
        Vector2 vector2 = new Vector2(Calc.Random.NextFloat(4f) - 2f, Calc.Random.NextFloat(4f) - 2f);
        switch (index)
        {
          case 2:
            vector2 = new Vector2(4f, 2f);
            break;
          case 6:
            vector2 = new Vector2(-2f, 0.0f);
            break;
        }
        MTexture mask = atlasSubtextures[index];
        MirrorSurface surface = new MirrorSurface()
        {
          ReflectionOffset = offset + vector2
        };
        surface.OnRender = (Action) (() => mask.DrawCentered(this.Position, surface.ReflectionColor, this.scale));
        this.Add((Component) surface);
      }
    }

    private Vector2 GetMirrorOffset() => new Vector2((float) (Calc.Random.Range(5, 14) * Calc.Random.Choose<int>(1, -1)), (float) (Calc.Random.Range(2, 6) * Calc.Random.Choose<int>(1, -1)));

    private void MakeParallax(float amount)
    {
      this.parallax = true;
      this.parallaxAmount = amount;
    }

    private void MakeScaredAnimation()
    {
      Sprite sprite = new Sprite((Atlas) null, (string) null);
      this.image = (Component) sprite;
      sprite.AddLoop("hidden", 0.1f, this.textures[0]);
      sprite.Add("return", 0.1f, "idle", this.textures[1]);
      sprite.AddLoop("idle", 0.1f, this.textures[2], this.textures[3], this.textures[4], this.textures[5], this.textures[6], this.textures[7]);
      sprite.Add("hide", 0.1f, "hidden", this.textures[8], this.textures[9], this.textures[10], this.textures[11], this.textures[12]);
      sprite.Play("idle", true);
      sprite.Scale = this.scale;
      sprite.CenterOrigin();
      this.Add((Component) sprite);
      this.scaredAnimal = true;
    }

    public override void Update()
    {
      if (this.animated && this.textures.Count > 1)
      {
        this.frame += this.AnimationSpeed * Engine.DeltaTime;
        this.frame %= (float) this.textures.Count;
      }
      if (this.scaredAnimal)
      {
        Sprite image = this.image as Sprite;
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          if (image.CurrentAnimationID == "idle" && (double) (entity.Position - this.Position).Length() < 32.0)
            image.Play("hide");
          else if (image.CurrentAnimationID == "hidden" && (double) (entity.Position - this.Position).Length() > 48.0)
            image.Play("return");
        }
      }
      base.Update();
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      if (this.parallax)
        this.Position = this.Position + (this.Position - ((this.Scene as Level).Camera.Position + new Vector2(160f, 90f))) * this.parallaxAmount;
      if (this.wave != null)
        this.Position.Y += this.wave.Value * 4f;
      base.Render();
      this.Position = position;
    }

    public void FinalFlagTrigger()
    {
      Wiggler wiggler = Wiggler.Create(1f, 4f, (Action<float>) (v => (this.image as Decal.FinalFlagDecalImage).Rotation = 0.20943952f * v), true);
      Vector2 position = this.Position;
      position.X = Calc.Snap(position.X, 8f) - 8f;
      position.Y += 6f;
      this.Scene.Add((Entity) new SummitCheckpoint.ConfettiRenderer(position));
      this.Add((Component) wiggler);
    }

    private class Banner : Component
    {
      public float WaveSpeed;
      public float WaveAmplitude;
      public int SliceSize;
      public float SliceSinIncrement;
      public bool EaseDown;
      public float Offset;
      public bool OnlyIfWindy;
      public float WindMultiplier = 1f;
      private float sineTimer = Calc.Random.NextFloat();
      public List<List<MTexture>> Segments;

      public Decal Decal => (Decal) this.Entity;

      public Banner()
        : base(true, true)
      {
      }

      public override void Update()
      {
        if (this.OnlyIfWindy)
        {
          float x = (this.Scene as Level).Wind.X;
          this.WindMultiplier = Calc.Approach(this.WindMultiplier, Math.Min(3f, Math.Abs(x) * 0.004f), Engine.DeltaTime * 4f);
          if ((double) x != 0.0)
            this.Offset = (float) Math.Sign(x) * Math.Abs(this.Offset);
        }
        this.sineTimer += Engine.DeltaTime * this.WindMultiplier;
        base.Update();
      }

      public override void Render()
      {
        MTexture texture = this.Decal.textures[(int) this.Decal.frame];
        List<MTexture> segment = this.Segments[(int) this.Decal.frame];
        for (int index = 0; index < segment.Count; ++index)
        {
          float num = (this.EaseDown ? (float) index / (float) segment.Count : (float) (1.0 - (double) index / (double) segment.Count)) * this.WindMultiplier;
          float x = (float) (Math.Sin((double) this.sineTimer * (double) this.WaveSpeed + (double) index * (double) this.SliceSinIncrement) * (double) num * (double) this.WaveAmplitude + (double) num * (double) this.Offset);
          segment[index].Draw(this.Decal.Position + new Vector2(x, 0.0f), new Vector2((float) (texture.Width / 2), (float) (texture.Height / 2 - index * this.SliceSize)), Color.White, this.Decal.scale);
        }
      }
    }

    private class DecalImage : Component
    {
      public Decal Decal => (Decal) this.Entity;

      public DecalImage()
        : base(true, true)
      {
      }

      public override void Render() => this.Decal.textures[(int) this.Decal.frame].DrawCentered(this.Decal.Position, Color.White, this.Decal.scale);
    }

    private class FinalFlagDecalImage : Component
    {
      public float Rotation;

      public Decal Decal => (Decal) this.Entity;

      public FinalFlagDecalImage()
        : base(true, true)
      {
      }

      public override void Render()
      {
        MTexture texture = this.Decal.textures[(int) this.Decal.frame];
        texture.DrawJustified(this.Decal.Position + Vector2.UnitY * (float) (texture.Height / 2), new Vector2(0.5f, 1f), Color.White, this.Decal.scale, this.Rotation);
      }
    }

    private class CoreSwapImage : Component
    {
      private MTexture hot;
      private MTexture cold;

      public Decal Decal => (Decal) this.Entity;

      public CoreSwapImage(MTexture hot, MTexture cold)
        : base(false, true)
      {
        this.hot = hot;
        this.cold = cold;
      }

      public override void Render() => ((this.Scene as Level).CoreMode == Session.CoreModes.Cold ? this.cold : this.hot).DrawCentered(this.Decal.Position, Color.White, this.Decal.scale);
    }
  }
}
