// Decompiled with JetBrains decompiler
// Type: Celeste.TrailManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class TrailManager : Entity
  {
    private TrailManager.Snapshot[] snapshots = new TrailManager.Snapshot[64];
    private static BlendState MaxBlendState;
    private const int size = 64;
    private const int columns = 8;
    private const int rows = 8;
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
        this.buffer = VirtualContent.CreateRenderTarget("trail-manager", 512, 512, false, true, 0);
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.buffer);
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, LightingRenderer.OccludeBlendState);
      for (int index = 0; index < this.snapshots.Length; ++index)
      {
        if (this.snapshots[index] != null && !this.snapshots[index].Drawn)
          Draw.Rect((float) (index % 8 * 64), (float) (index / 8 * 64), 64f, 64f, Color.get_Transparent());
      }
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) RasterizerState.CullNone);
      for (int index1 = 0; index1 < this.snapshots.Length; ++index1)
      {
        if (this.snapshots[index1] != null && !this.snapshots[index1].Drawn)
        {
          TrailManager.Snapshot snapshot = this.snapshots[index1];
          Vector2 vector2 = Vector2.op_Subtraction(new Vector2((float) (((double) (index1 % 8) + 0.5) * 64.0), (float) (((double) (index1 / 8) + 0.5) * 64.0)), snapshot.Position);
          if (snapshot.Hair != null)
          {
            for (int index2 = 0; index2 < snapshot.Hair.Nodes.Count; ++index2)
            {
              List<Vector2> nodes = snapshot.Hair.Nodes;
              int index3 = index2;
              nodes[index3] = Vector2.op_Addition(nodes[index3], vector2);
            }
            snapshot.Hair.Render();
            for (int index2 = 0; index2 < snapshot.Hair.Nodes.Count; ++index2)
            {
              List<Vector2> nodes = snapshot.Hair.Nodes;
              int index3 = index2;
              nodes[index3] = Vector2.op_Subtraction(nodes[index3], vector2);
            }
          }
          Vector2 scale = snapshot.Sprite.Scale;
          snapshot.Sprite.Scale = snapshot.SpriteScale;
          Monocle.Image sprite1 = snapshot.Sprite;
          sprite1.Position = Vector2.op_Addition(sprite1.Position, vector2);
          snapshot.Sprite.Render();
          snapshot.Sprite.Scale = scale;
          Monocle.Image sprite2 = snapshot.Sprite;
          sprite2.Position = Vector2.op_Subtraction(sprite2.Position, vector2);
          snapshot.Drawn = true;
        }
      }
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, TrailManager.MaxBlendState);
      Draw.Rect(0.0f, 0.0f, (float) this.buffer.Width, (float) this.buffer.Height, new Color(1f, 1f, 1f, 1f));
      Draw.SpriteBatch.End();
      this.dirty = false;
    }

    public static void Add(Entity entity, Color color, float duration = 1f)
    {
      Monocle.Image sprite = (Monocle.Image) entity.Get<PlayerSprite>() ?? (Monocle.Image) entity.Get<Sprite>();
      PlayerHair hair = entity.Get<PlayerHair>();
      TrailManager.Add(entity.Position, sprite, hair, color, entity.Depth + 1, duration);
    }

    public static void Add(Vector2 position, Monocle.Image image, Color color, int depth, float duration = 1f)
    {
      TrailManager.Add(position, image, (PlayerHair) null, color, depth, duration);
    }

    public static void Add(
      Vector2 position,
      Monocle.Image sprite,
      PlayerHair hair,
      Color color,
      int depth,
      float duration = 1f)
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
          snapshot.Init(manager, index, position, sprite, hair, color, duration, depth);
          manager.snapshots[index] = snapshot;
          manager.dirty = true;
          Engine.Scene.Add((Entity) snapshot);
          break;
        }
      }
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

    static TrailManager()
    {
      BlendState blendState = new BlendState();
      blendState.set_ColorSourceBlend((Blend) 8);
      blendState.set_AlphaSourceBlend((Blend) 8);
      TrailManager.MaxBlendState = blendState;
    }

    [Pooled]
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

      public Snapshot()
      {
        this.Tag = (int) Tags.Global;
        this.Add((Component) new MirrorReflection());
      }

      public void Init(
        TrailManager manager,
        int index,
        Vector2 position,
        Monocle.Image sprite,
        PlayerHair hair,
        Color color,
        float duration,
        int depth)
      {
        this.Manager = manager;
        this.Index = index;
        this.Position = position;
        this.Sprite = sprite;
        this.SpriteScale = sprite.Scale;
        this.Hair = hair;
        this.Color = color;
        this.Percent = 0.0f;
        this.Duration = duration;
        this.Depth = depth;
        this.Drawn = false;
      }

      public override void Update()
      {
        if ((double) this.Percent >= 1.0)
          this.RemoveSelf();
        this.Percent += Engine.DeltaTime / this.Duration;
      }

      public override void Render()
      {
        VirtualRenderTarget buffer = this.Manager.buffer;
        Rectangle rectangle;
        ((Rectangle) ref rectangle).\u002Ector(this.Index % 8 * 64, this.Index / 8 * 64, 64, 64);
        float num = (float) (0.75 * (1.0 - (double) Ease.CubeOut(this.Percent)));
        if (buffer == null)
          return;
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) buffer, this.Position, new Rectangle?(rectangle), Color.op_Multiply(this.Color, num), 0.0f, Vector2.op_Multiply(new Vector2(64f, 64f), 0.5f), Vector2.get_One(), (SpriteEffects) 0, 0.0f);
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
