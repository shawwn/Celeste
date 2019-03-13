// Decompiled with JetBrains decompiler
// Type: Celeste.CrumblePlatform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class CrumblePlatform : Solid
  {
    public static ParticleType P_Crumble;
    private List<Monocle.Image> images;
    private List<Monocle.Image> outline;
    private List<Coroutine> falls;
    private List<int> fallOrder;
    private ShakerList shaker;
    private LightOcclude occluder;
    private Coroutine outlineFader;

    public CrumblePlatform(Vector2 position, float width)
      : base(position, width, 8f, false)
    {
      this.EnableAssistModeChecks = false;
    }

    public CrumblePlatform(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      MTexture mtexture1 = GFX.Game["objects/crumbleBlock/outline"];
      this.outline = new List<Monocle.Image>();
      if ((double) this.Width <= 8.0)
      {
        Monocle.Image image = new Monocle.Image(mtexture1.GetSubtexture(24, 0, 8, 8, (MTexture) null));
        image.Color = Color.White * 0.0f;
        this.Add((Component) image);
        this.outline.Add(image);
      }
      else
      {
        for (int index = 0; (double) index < (double) this.Width; index += 8)
        {
          int num = index != 0 ? (index <= 0 || (double) index >= (double) this.Width - 8.0 ? 2 : 1) : 0;
          Monocle.Image image = new Monocle.Image(mtexture1.GetSubtexture(num * 8, 0, 8, 8, (MTexture) null));
          image.Position = new Vector2((float) index, 0.0f);
          image.Color = Color.White * 0.0f;
          this.Add((Component) image);
          this.outline.Add(image);
        }
      }
      this.Add((Component) (this.outlineFader = new Coroutine(true)));
      this.outlineFader.RemoveOnComplete = false;
      this.images = new List<Monocle.Image>();
      this.falls = new List<Coroutine>();
      this.fallOrder = new List<int>();
      MTexture mtexture2 = GFX.Game["objects/crumbleBlock/" + AreaData.Get(scene).CrumbleBlock];
      for (int index = 0; (double) index < (double) this.Width; index += 8)
      {
        int num = (int) (((double) Math.Abs(this.X) + (double) index) / 8.0) % 4;
        Monocle.Image image = new Monocle.Image(mtexture2.GetSubtexture(num * 8, 0, 8, 8, (MTexture) null));
        image.Position = new Vector2((float) (4 + index), 4f);
        image.CenterOrigin();
        this.Add((Component) image);
        this.images.Add(image);
        Coroutine coroutine = new Coroutine(true);
        coroutine.RemoveOnComplete = false;
        this.falls.Add(coroutine);
        this.Add((Component) coroutine);
        this.fallOrder.Add(index / 8);
      }
      this.fallOrder.Shuffle<int>();
      this.Add((Component) new Coroutine(this.Sequence(), true));
      this.Add((Component) (this.shaker = new ShakerList(this.images.Count, false, (Action<Vector2[]>) (v =>
      {
        for (int index = 0; index < this.images.Count; ++index)
          this.images[index].Position = new Vector2((float) (4 + index * 8), 4f) + v[index];
      }))));
      this.Add((Component) (this.occluder = new LightOcclude(0.2f)));
    }

    private IEnumerator Sequence()
    {
label_43:
      bool onTop = false;
      while (true)
      {
        Player player = this.GetPlayerOnTop();
        if (player == null)
        {
          player = this.GetPlayerClimbing();
          if (player == null)
          {
            yield return (object) null;
            player = (Player) null;
          }
          else
            goto label_3;
        }
        else
          break;
      }
      onTop = true;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      goto label_6;
label_3:
      onTop = false;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
label_6:
      Audio.Play("event:/game/general/platform_disintegrate", this.Center);
      this.shaker.ShakeFor(onTop ? 0.6f : 1f, false);
      foreach (Monocle.Image image in this.images)
      {
        Monocle.Image img = image;
        this.SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 2, this.Position + img.Position + new Vector2(0.0f, 2f), Vector2.One * 3f);
        img = (Monocle.Image) null;
      }
      for (int i = 0; i < (onTop ? 1 : 3); ++i)
      {
        yield return (object) 0.2f;
        foreach (Monocle.Image image in this.images)
        {
          Monocle.Image img = image;
          this.SceneAs<Level>().Particles.Emit(CrumblePlatform.P_Crumble, 2, this.Position + img.Position + new Vector2(0.0f, 2f), Vector2.One * 3f);
          img = (Monocle.Image) null;
        }
      }
      float timer = 0.4f;
      if (onTop)
      {
        for (; (double) timer > 0.0 && this.GetPlayerOnTop() != null; timer -= Engine.DeltaTime)
          yield return (object) null;
      }
      else
      {
        for (; (double) timer > 0.0; timer -= Engine.DeltaTime)
          yield return (object) null;
      }
      this.outlineFader.Replace(this.OutlineFade(1f));
      this.occluder.Visible = false;
      this.Collidable = false;
      float delay = 0.05f;
      for (int j = 0; j < 4; ++j)
      {
        for (int i = 0; i < this.images.Count; ++i)
        {
          if (i % 4 - j == 0)
            this.falls[i].Replace(this.TileOut(this.images[this.fallOrder[i]], delay * (float) j));
        }
      }
      yield return (object) 2f;
      while (this.CollideCheck<Actor>() || this.CollideCheck<Solid>())
        yield return (object) null;
      this.outlineFader.Replace(this.OutlineFade(0.0f));
      this.occluder.Visible = true;
      this.Collidable = true;
      for (int j = 0; j < 4; ++j)
      {
        for (int i = 0; i < this.images.Count; ++i)
        {
          if (i % 4 - j == 0)
            this.falls[i].Replace(this.TileIn(i, this.images[this.fallOrder[i]], 0.05f * (float) j));
        }
      }
      goto label_43;
    }

    private IEnumerator OutlineFade(float to)
    {
      float from = 1f - to;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        Color color = Color.White * (from + (to - from) * Ease.CubeInOut(t));
        foreach (Monocle.Image image in this.outline)
        {
          Monocle.Image img = image;
          img.Color = color;
          img = (Monocle.Image) null;
        }
        yield return (object) null;
        color = new Color();
      }
    }

    private IEnumerator TileOut(Monocle.Image img, float delay)
    {
      img.Color = Color.Gray;
      yield return (object) delay;
      float distance = (float) (((double) img.X * 7.0 % 3.0 + 1.0) * 12.0);
      Vector2 from = img.Position;
      for (float time = 0.0f; (double) time < 1.0; time += Engine.DeltaTime / 0.4f)
      {
        yield return (object) null;
        img.Position = from + Vector2.UnitY * Ease.CubeIn(time) * distance;
        img.Color = Color.Gray * (1f - time);
        img.Scale = Vector2.One * (float) (1.0 - (double) time * 0.5);
      }
      img.Visible = false;
    }

    private IEnumerator TileIn(int index, Monocle.Image img, float delay)
    {
      yield return (object) delay;
      Audio.Play("event:/game/general/platform_return", this.Center);
      img.Visible = true;
      img.Color = Color.White;
      img.Position = new Vector2((float) (index * 8 + 4), 4f);
      for (float time = 0.0f; (double) time < 1.0; time += Engine.DeltaTime / 0.25f)
      {
        yield return (object) null;
        img.Scale = Vector2.One * (float) (1.0 + (double) Ease.BounceOut(1f - time) * 0.200000002980232);
      }
      img.Scale = Vector2.One;
    }
  }
}

