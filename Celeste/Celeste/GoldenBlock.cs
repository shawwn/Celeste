// Decompiled with JetBrains decompiler
// Type: Celeste.GoldenBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class GoldenBlock : Solid
  {
    private MTexture[,] nineSlice;
    private Monocle.Image berry;
    private float startY;
    private float yLerp;
    private float sinkTimer;
    private float renderLerp;

    public GoldenBlock(Vector2 position, float width, float height)
      : base(position, width, height, false)
    {
      this.startY = this.Y;
      this.berry = new Monocle.Image(GFX.Game["collectables/goldberry/idle00"]);
      this.berry.CenterOrigin();
      this.berry.Position = new Vector2(width / 2f, height / 2f);
      MTexture mtexture = GFX.Game["objects/goldblock"];
      this.nineSlice = new MTexture[3, 3];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < 3; ++index2)
          this.nineSlice[index1, index2] = mtexture.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
      }
      this.Depth = -10000;
      this.Add((Component) new LightOcclude());
      this.Add((Component) new MirrorSurface());
      this.SurfaceSoundIndex = 32;
    }

    public GoldenBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.Visible = false;
      this.Collidable = false;
      this.renderLerp = 1f;
      bool flag = false;
      foreach (Strawberry strawberry in scene.Entities.FindAll<Strawberry>())
      {
        if (strawberry.Golden && strawberry.Follower.Leader != null)
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      if (!this.Visible)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.X > (double) this.X - 80.0)
        {
          this.Visible = true;
          this.Collidable = true;
          this.renderLerp = 1f;
        }
      }
      if (this.Visible)
        this.renderLerp = Calc.Approach(this.renderLerp, 0.0f, Engine.DeltaTime * 3f);
      if (this.HasPlayerRider())
        this.sinkTimer = 0.1f;
      else if ((double) this.sinkTimer > 0.0)
        this.sinkTimer -= Engine.DeltaTime;
      this.yLerp = (double) this.sinkTimer <= 0.0 ? Calc.Approach(this.yLerp, 0.0f, 1f * Engine.DeltaTime) : Calc.Approach(this.yLerp, 1f, 1f * Engine.DeltaTime);
      this.MoveToY(MathHelper.Lerp(this.startY, this.startY + 12f, Ease.SineInOut(this.yLerp)));
    }

    private void DrawBlock(Vector2 offset, Color color)
    {
      float num1 = (float) ((double) this.Collider.Width / 8.0 - 1.0);
      float num2 = (float) ((double) this.Collider.Height / 8.0 - 1.0);
      for (int val1_1 = 0; (double) val1_1 <= (double) num1; ++val1_1)
      {
        for (int val1_2 = 0; (double) val1_2 <= (double) num2; ++val1_2)
          this.nineSlice[(double) val1_1 < (double) num1 ? Math.Min(val1_1, 1) : 2, (double) val1_2 < (double) num2 ? Math.Min(val1_2, 1) : 2].Draw(this.Position + offset + this.Shake + new Vector2((float) (val1_1 * 8), (float) (val1_2 * 8)), Vector2.Zero, color);
      }
    }

    public override void Render()
    {
      Vector2 vector2 = new Vector2(0.0f, (float) ((double) (this.Scene as Level).Bounds.Bottom - (double) this.startY + 32.0) * Ease.CubeIn(this.renderLerp));
      Vector2 position = this.Position;
      this.Position = this.Position + vector2;
      this.DrawBlock(new Vector2(-1f, 0.0f), Color.Black);
      this.DrawBlock(new Vector2(1f, 0.0f), Color.Black);
      this.DrawBlock(new Vector2(0.0f, -1f), Color.Black);
      this.DrawBlock(new Vector2(0.0f, 1f), Color.Black);
      this.DrawBlock(Vector2.Zero, Color.White);
      this.berry.Color = Color.White;
      this.berry.RenderPosition = this.Center;
      this.berry.Render();
      this.Position = position;
    }
  }
}
