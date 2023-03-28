// Decompiled with JetBrains decompiler
// Type: Celeste.TrailManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class TrailManager : Entity
  {
    private static BlendState MaxBlendState = new BlendState()
    {
      ColorSourceBlend = Blend.DestinationAlpha,
      AlphaSourceBlend = Blend.DestinationAlpha
    };
    private const int size = 64;
    private const int columns = 8;
    private const int rows = 8;
    private TrailManager.Snapshot[] snapshots = new TrailManager.Snapshot[64];
    private VirtualRenderTarget buffer;
    private bool dirty;

    public TrailManager()
    {
      this.Tag = (int) Tags.Global;
      this.Depth = 10;
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      this.Add((Component) new MirrorReflection());
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

    private void BeforeRender()
    {
      if (!this.dirty)
        return;
      if (this.buffer == null)
        this.buffer = VirtualContent.CreateRenderTarget("trail-manager", 512, 512);
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.buffer);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, LightingRenderer.OccludeBlendState);
      for (int index = 0; index < this.snapshots.Length; ++index)
      {
        if (this.snapshots[index] != null && !this.snapshots[index].Drawn)
          Draw.Rect((float) (index % 8 * 64), (float) (index / 8 * 64), 64f, 64f, Color.Transparent);
      }
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, RasterizerState.CullNone);
      for (int index1 = 0; index1 < this.snapshots.Length; ++index1)
      {
        if (this.snapshots[index1] != null && !this.snapshots[index1].Drawn)
        {
          TrailManager.Snapshot snapshot = this.snapshots[index1];
          Vector2 vector2 = new Vector2((float) (((double) (index1 % 8) + 0.5) * 64.0), (float) (((double) (index1 / 8) + 0.5) * 64.0)) - snapshot.Position;
          if (snapshot.Hair != null)
          {
            for (int index2 = 0; index2 < snapshot.Hair.Nodes.Count; ++index2)
              snapshot.Hair.Nodes[index2] += vector2;
            snapshot.Hair.Render();
            for (int index3 = 0; index3 < snapshot.Hair.Nodes.Count; ++index3)
              snapshot.Hair.Nodes[index3] -= vector2;
          }
          Vector2 scale = snapshot.Sprite.Scale;
          snapshot.Sprite.Scale = snapshot.SpriteScale;
          Monocle.Image sprite1 = snapshot.Sprite;
          sprite1.Position = sprite1.Position + vector2;
          snapshot.Sprite.Render();
          snapshot.Sprite.Scale = scale;
          Monocle.Image sprite2 = snapshot.Sprite;
          sprite2.Position = sprite2.Position - vector2;
          snapshot.Drawn = true;
        }
      }
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, TrailManager.MaxBlendState);
      Draw.Rect(0.0f, 0.0f, (float) this.buffer.Width, (float) this.buffer.Height, new Color(1f, 1f, 1f, 1f));
      Draw.SpriteBatch.End();
      this.dirty = false;
    }

    public static void Add(
      Entity entity,
      Color color,
      float duration = 1f,
      bool frozenUpdate = false,
      bool useRawDeltaTime = false)
    {
      Monocle.Image sprite = (Monocle.Image) entity.Get<PlayerSprite>() ?? (Monocle.Image) entity.Get<Sprite>();
      PlayerHair hair = entity.Get<PlayerHair>();
      TrailManager.Add(entity.Position, sprite, hair, sprite.Scale, color, entity.Depth + 1, duration, frozenUpdate, useRawDeltaTime);
    }

    public static void Add(Entity entity, Vector2 scale, Color color, float duration = 1f)
    {
      Monocle.Image sprite = (Monocle.Image) entity.Get<PlayerSprite>() ?? (Monocle.Image) entity.Get<Sprite>();
      PlayerHair hair = entity.Get<PlayerHair>();
      TrailManager.Add(entity.Position, sprite, hair, scale, color, entity.Depth + 1, duration);
    }

    public static void Add(Vector2 position, Monocle.Image image, Color color, int depth, float duration = 1f) => TrailManager.Add(position, image, (PlayerHair) null, image.Scale, color, depth, duration);

    public static TrailManager.Snapshot Add(
      Vector2 position,
      Monocle.Image sprite,
      PlayerHair hair,
      Vector2 scale,
      Color color,
      int depth,
      float duration = 1f,
      bool frozenUpdate = false,
      bool useRawDeltaTime = false)
    {
      TrailManager manager = Engine.Scene.Tracker.GetEntity<TrailManager>();
      if (manager == null)
      {
        manager = new TrailManager();
        Engine.Scene.Add((Entity) manager);
      }
      for (int index = 0; index < manager.snapshots.Length; ++index)
      {
        if (manager.snapshots[index] == null)
        {
          TrailManager.Snapshot snapshot = Engine.Pooler.Create<TrailManager.Snapshot>();
          snapshot.Init(manager, index, position, sprite, hair, scale, color, duration, depth, frozenUpdate, useRawDeltaTime);
          manager.snapshots[index] = snapshot;
          manager.dirty = true;
          Engine.Scene.Add((Entity) snapshot);
          return snapshot;
        }
      }
      return (TrailManager.Snapshot) null;
    }

    public static void Clear()
    {
      TrailManager entity = Engine.Scene.Tracker.GetEntity<TrailManager>();
      if (entity == null)
        return;
      for (int index = 0; index < entity.snapshots.Length; ++index)
      {
        if (entity.snapshots[index] != null)
          entity.snapshots[index].RemoveSelf();
      }
    }

    [Pooled]
    [Tracked(false)]
    public class Snapshot : Entity
    {
      public TrailManager Manager;
      public Monocle.Image Sprite;
      public Vector2 SpriteScale;
      public PlayerHair Hair;
      public int Index;
      public Color Color;
      public float Percent;
      public float Duration;
      public bool Drawn;
      public bool UseRawDeltaTime;

      public Snapshot() => this.Add((Component) new MirrorReflection());

      public void Init(
        TrailManager manager,
        int index,
        Vector2 position,
        Monocle.Image sprite,
        PlayerHair hair,
        Vector2 scale,
        Color color,
        float duration,
        int depth,
        bool frozenUpdate,
        bool useRawDeltaTime)
      {
        this.Tag = (int) Tags.Global;
        if (frozenUpdate)
          this.Tag |= (int) Tags.FrozenUpdate;
        this.Manager = manager;
        this.Index = index;
        this.Position = position;
        this.Sprite = sprite;
        this.SpriteScale = scale;
        this.Hair = hair;
        this.Color = color;
        this.Percent = 0.0f;
        this.Duration = duration;
        this.Depth = depth;
        this.Drawn = false;
        this.UseRawDeltaTime = useRawDeltaTime;
      }

      public override void Update()
      {
        if ((double) this.Duration <= 0.0)
        {
          if (!this.Drawn)
            return;
          this.RemoveSelf();
        }
        else
        {
          if ((double) this.Percent >= 1.0)
            this.RemoveSelf();
          this.Percent += (this.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) / this.Duration;
        }
      }

      public override void Render()
      {
        VirtualRenderTarget buffer = this.Manager.buffer;
        Rectangle rectangle = new Rectangle(this.Index % 8 * 64, this.Index / 8 * 64, 64, 64);
        float num = (double) this.Duration > 0.0 ? (float) (0.75 * (1.0 - (double) Ease.CubeOut(this.Percent))) : 1f;
        if (buffer == null)
          return;
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) buffer, this.Position, new Rectangle?(rectangle), this.Color * num, 0.0f, new Vector2(64f, 64f) * 0.5f, Vector2.One, SpriteEffects.None, 0.0f);
      }

      public override void Removed(Scene scene)
      {
        if (this.Manager != null)
          this.Manager.snapshots[this.Index] = (TrailManager.Snapshot) null;
        base.Removed(scene);
      }
    }
  }
}
