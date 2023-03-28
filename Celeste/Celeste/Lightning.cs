// Decompiled with JetBrains decompiler
// Type: Celeste.Lightning
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class Lightning : Entity
  {
    public static ParticleType P_Shatter;
    public const string Flag = "disable_lightning";
    public float Fade;
    private bool disappearing;
    private float toggleOffset;
    public int VisualWidth;
    public int VisualHeight;

    public Lightning(Vector2 position, int width, int height, Vector2? node, float moveTime)
      : base(position)
    {
      this.VisualWidth = width;
      this.VisualHeight = height;
      this.Collider = (Collider) new Hitbox((float) (width - 2), (float) (height - 2), 1f, 1f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
      if (node.HasValue)
        this.Add((Component) new Coroutine(this.MoveRoutine(position, node.Value, moveTime)));
      this.toggleOffset = Calc.Random.NextFloat();
    }

    public Lightning(EntityData data, Vector2 levelOffset)
      : this(data.Position + levelOffset, data.Width, data.Height, data.FirstNodeNullable(new Vector2?(levelOffset)), data.Float("moveTime"))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Tracker.GetEntity<LightningRenderer>().Track(this);
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      scene.Tracker.GetEntity<LightningRenderer>().Untrack(this);
    }

    public override void Update()
    {
      if (this.Collidable && this.Scene.OnInterval(0.25f, this.toggleOffset))
        this.ToggleCheck();
      if (!this.Collidable && this.Scene.OnInterval(0.05f, this.toggleOffset))
        this.ToggleCheck();
      base.Update();
    }

    public void ToggleCheck() => this.Collidable = this.Visible = this.InView();

    private bool InView()
    {
      Camera camera = (this.Scene as Level).Camera;
      return (double) this.X + (double) this.Width > (double) camera.X - 16.0 && (double) this.Y + (double) this.Height > (double) camera.Y - 16.0 && (double) this.X < (double) camera.X + 320.0 + 16.0 && (double) this.Y < (double) camera.Y + 180.0 + 16.0;
    }

    private void OnPlayer(Player player)
    {
      if (this.disappearing || SaveData.Instance.Assists.Invincible)
        return;
      int num = Math.Sign(player.X - this.X);
      if (num == 0)
        num = -1;
      player.Die(Vector2.UnitX * (float) num);
    }

    private IEnumerator MoveRoutine(Vector2 start, Vector2 end, float moveTime)
    {
      while (true)
      {
        yield return (object) this.Move(start, end, moveTime);
        yield return (object) this.Move(end, start, moveTime);
      }
    }

    private IEnumerator Move(Vector2 start, Vector2 end, float moveTime)
    {
      Lightning lightning = this;
      float at = 0.0f;
      while (true)
      {
        lightning.Position = Vector2.Lerp(start, end, Ease.SineInOut(at));
        if ((double) at < 1.0)
        {
          yield return (object) null;
          at = MathHelper.Clamp(at + Engine.DeltaTime / moveTime, 0.0f, 1f);
        }
        else
          break;
      }
    }

    private void Shatter()
    {
      if (this.Scene == null)
        return;
      for (int x = 4; (double) x < (double) this.Width; x += 8)
      {
        for (int y = 4; (double) y < (double) this.Height; y += 8)
          this.SceneAs<Level>().ParticlesFG.Emit(Lightning.P_Shatter, 1, this.TopLeft + new Vector2((float) x, (float) y), Vector2.One * 3f);
      }
    }

    public static IEnumerator PulseRoutine(Level level)
    {
      float t;
      for (t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 8f)
      {
        Lightning.SetPulseValue(level, t);
        yield return (object) null;
      }
      for (t = 1f; (double) t > 0.0; t -= Engine.DeltaTime * 8f)
      {
        Lightning.SetPulseValue(level, t);
        yield return (object) null;
      }
      Lightning.SetPulseValue(level, 0.0f);
    }

    private static void SetPulseValue(Level level, float t)
    {
      BloomRenderer bloom = level.Bloom;
      LightningRenderer entity = level.Tracker.GetEntity<LightningRenderer>();
      Glitch.Value = MathHelper.Lerp(0.0f, 0.075f, t);
      double num = (double) MathHelper.Lerp(1f, 1.2f, t);
      bloom.Strength = (float) num;
      entity.Fade = t * 0.2f;
    }

    private static void SetBreakValue(Level level, float t)
    {
      BloomRenderer bloom = level.Bloom;
      LightningRenderer entity = level.Tracker.GetEntity<LightningRenderer>();
      Glitch.Value = MathHelper.Lerp(0.0f, 0.15f, t);
      double num = (double) MathHelper.Lerp(1f, 1.5f, t);
      bloom.Strength = (float) num;
      entity.Fade = t * 0.6f;
    }

    public static IEnumerator RemoveRoutine(Level level, Action onComplete = null)
    {
      List<Lightning> blocks = level.Entities.FindAll<Lightning>();
      foreach (Lightning lightning in new List<Lightning>((IEnumerable<Lightning>) blocks))
      {
        lightning.disappearing = true;
        if ((double) lightning.Right < (double) level.Camera.Left || (double) lightning.Bottom < (double) level.Camera.Top || (double) lightning.Left > (double) level.Camera.Right || (double) lightning.Top > (double) level.Camera.Bottom)
        {
          blocks.Remove(lightning);
          lightning.RemoveSelf();
        }
      }
      LightningRenderer entity1 = level.Tracker.GetEntity<LightningRenderer>();
      entity1.StopAmbience();
      entity1.UpdateSeeds = false;
      float t;
      for (t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 4f)
      {
        Lightning.SetBreakValue(level, t);
        yield return (object) null;
      }
      Lightning.SetBreakValue(level, 1f);
      level.Shake();
      for (int index = blocks.Count - 1; index >= 0; --index)
        blocks[index].Shatter();
      for (t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 8f)
      {
        Lightning.SetBreakValue(level, 1f - t);
        yield return (object) null;
      }
      Lightning.SetBreakValue(level, 0.0f);
      foreach (Entity entity2 in blocks)
        entity2.RemoveSelf();
      FlingBird first = level.Entities.FindFirst<FlingBird>();
      if (first != null)
        first.LightningRemoved = true;
      if (onComplete != null)
        onComplete();
    }
  }
}
