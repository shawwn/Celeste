// Decompiled with JetBrains decompiler
// Type: Celeste.SwapBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class SwapBlock : Solid
  {
    private float redAlpha = 1f;
    public static ParticleType P_Move;
    private const float ReturnTime = 0.8f;
    public Vector2 Direction;
    public bool Swapping;
    private Vector2 start;
    private Vector2 end;
    private float lerp;
    private int target;
    private Rectangle moveRect;
    private float speed;
    private float maxForwardSpeed;
    private float maxBackwardSpeed;
    private float returnTimer;
    private MTexture[,] nineSliceGreen;
    private MTexture[,] nineSliceRed;
    private MTexture[,] nineSliceTarget;
    private Sprite middleGreen;
    private Sprite middleRed;
    private SwapBlock.PathRenderer path;
    private EventInstance moveSfx;
    private EventInstance returnSfx;
    private DisplacementRenderer.Burst burst;
    private float particlesRemainder;

    public SwapBlock(Vector2 position, float width, float height, Vector2 node)
      : base(position, width, height, false)
    {
      this.start = this.Position;
      this.end = node;
      this.maxForwardSpeed = 360f / Vector2.Distance(this.start, this.end);
      this.maxBackwardSpeed = this.maxForwardSpeed * 0.4f;
      this.Direction.X = (__Null) (double) Math.Sign((float) (this.end.X - this.start.X));
      this.Direction.Y = (__Null) (double) Math.Sign((float) (this.end.Y - this.start.Y));
      this.Add((Component) new DashListener()
      {
        OnDash = new Action<Vector2>(this.OnDash)
      });
      int num1 = (int) MathHelper.Min(this.X, (float) node.X);
      int num2 = (int) MathHelper.Min(this.Y, (float) node.Y);
      int num3 = (int) MathHelper.Max(this.X + this.Width, (float) node.X + this.Width);
      int num4 = (int) MathHelper.Max(this.Y + this.Height, (float) node.Y + this.Height);
      this.moveRect = new Rectangle(num1, num2, num3 - num1, num4 - num2);
      MTexture mtexture1 = GFX.Game["objects/swapblock/block"];
      MTexture mtexture2 = GFX.Game["objects/swapblock/blockRed"];
      MTexture mtexture3 = GFX.Game["objects/swapblock/target"];
      this.nineSliceGreen = new MTexture[3, 3];
      this.nineSliceRed = new MTexture[3, 3];
      this.nineSliceTarget = new MTexture[3, 3];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < 3; ++index2)
        {
          this.nineSliceGreen[index1, index2] = mtexture1.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
          this.nineSliceRed[index1, index2] = mtexture2.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
          this.nineSliceTarget[index1, index2] = mtexture3.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
        }
      }
      this.Add((Component) (this.middleGreen = GFX.SpriteBank.Create("swapBlockLight")));
      this.Add((Component) (this.middleRed = GFX.SpriteBank.Create("swapBlockLightRed")));
      this.Add((Component) new LightOcclude(0.2f));
      this.Depth = -9999;
    }

    public SwapBlock(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), (float) data.Width, (float) data.Height, Vector2.op_Addition(data.Nodes[0], offset))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      scene.Add((Entity) (this.path = new SwapBlock.PathRenderer(this)));
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      Audio.Stop(this.moveSfx, true);
      Audio.Stop(this.returnSfx, true);
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      Audio.Stop(this.moveSfx, true);
      Audio.Stop(this.returnSfx, true);
    }

    private void OnDash(Vector2 direction)
    {
      this.Swapping = (double) this.lerp < 1.0;
      this.target = 1;
      this.returnTimer = 0.8f;
      this.burst = (this.Scene as Level).Displacement.AddBurst(this.Center, 0.2f, 0.0f, 16f, 1f, (Ease.Easer) null, (Ease.Easer) null);
      this.speed = (double) this.lerp < 0.200000002980232 ? MathHelper.Lerp(this.maxForwardSpeed * 0.333f, this.maxForwardSpeed, this.lerp / 0.2f) : this.maxForwardSpeed;
      Audio.Stop(this.returnSfx, true);
      Audio.Stop(this.moveSfx, true);
      if (!this.Swapping)
        Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", this.Center);
      else
        this.moveSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", this.Center);
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.returnTimer > 0.0)
      {
        this.returnTimer -= Engine.DeltaTime;
        if ((double) this.returnTimer <= 0.0)
        {
          this.target = 0;
          this.speed = 0.0f;
          this.returnSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_return", this.Center);
        }
      }
      if (this.burst != null)
        this.burst.Position = this.Center;
      this.redAlpha = Calc.Approach(this.redAlpha, this.target == 1 ? 0.0f : 1f, Engine.DeltaTime * 32f);
      if (this.target == 0 && (double) this.lerp == 0.0)
      {
        this.middleRed.SetAnimationFrame(0);
        this.middleGreen.SetAnimationFrame(0);
      }
      this.speed = this.target != 1 ? Calc.Approach(this.speed, this.maxBackwardSpeed, this.maxBackwardSpeed / 1.5f * Engine.DeltaTime) : Calc.Approach(this.speed, this.maxForwardSpeed, this.maxForwardSpeed / 0.2f * Engine.DeltaTime);
      float lerp = this.lerp;
      this.lerp = Calc.Approach(this.lerp, (float) this.target, this.speed * Engine.DeltaTime);
      if ((double) this.lerp != (double) lerp)
      {
        Vector2 liftSpeed = Vector2.op_Multiply(Vector2.op_Subtraction(this.end, this.start), this.speed);
        Vector2 position1 = this.Position;
        if (this.target == 1)
          liftSpeed = Vector2.op_Multiply(Vector2.op_Subtraction(this.end, this.start), this.maxForwardSpeed);
        if ((double) this.lerp < (double) lerp)
          liftSpeed = Vector2.op_Multiply(liftSpeed, -1f);
        if (this.target == 1 && this.Scene.OnInterval(0.02f))
          this.MoveParticles(Vector2.op_Subtraction(this.end, this.start));
        this.MoveTo(Vector2.Lerp(this.start, this.end, this.lerp), liftSpeed);
        Vector2 position2 = this.Position;
        if (Vector2.op_Inequality(position1, position2))
        {
          Audio.Position(this.moveSfx, this.Center);
          Audio.Position(this.returnSfx, this.Center);
          if (Vector2.op_Equality(this.Position, this.start) && this.target == 0)
          {
            Audio.SetParameter(this.returnSfx, "end", 1f);
            Audio.Play("event:/game/05_mirror_temple/swapblock_return_end", this.Center);
          }
          else if (Vector2.op_Equality(this.Position, this.end) && this.target == 1)
            Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", this.Center);
        }
      }
      if (this.Swapping && (double) this.lerp >= 1.0)
        this.Swapping = false;
      this.StopPlayerRunIntoAnimation = (double) this.lerp <= 0.0 || (double) this.lerp >= 1.0;
    }

    private void MoveParticles(Vector2 normal)
    {
      Vector2 position;
      Vector2 vector2;
      float direction;
      float num;
      if (normal.X > 0.0)
      {
        position = this.CenterLeft;
        vector2 = Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 6f);
        direction = 3.141593f;
        num = Math.Max(2f, this.Height / 14f);
      }
      else if (normal.X < 0.0)
      {
        position = this.CenterRight;
        vector2 = Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 6f);
        direction = 0.0f;
        num = Math.Max(2f, this.Height / 14f);
      }
      else if (normal.Y > 0.0)
      {
        position = this.TopCenter;
        vector2 = Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 6f);
        direction = -1.570796f;
        num = Math.Max(2f, this.Width / 14f);
      }
      else
      {
        position = this.BottomCenter;
        vector2 = Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 6f);
        direction = 1.570796f;
        num = Math.Max(2f, this.Width / 14f);
      }
      this.particlesRemainder += num;
      int particlesRemainder = (int) this.particlesRemainder;
      this.particlesRemainder -= (float) particlesRemainder;
      Vector2 positionRange = Vector2.op_Multiply(vector2, 0.5f);
      this.SceneAs<Level>().Particles.Emit(SwapBlock.P_Move, particlesRemainder, position, positionRange, direction);
    }

    public override void Render()
    {
      Vector2 pos = Vector2.op_Addition(this.Position, this.Shake);
      if ((double) this.lerp != (double) this.target && (double) this.speed > 0.0)
      {
        Vector2 vector2 = Vector2.op_Subtraction(this.end, this.start).SafeNormalize();
        if (this.target == 1)
          vector2 = Vector2.op_Multiply(vector2, -1f);
        float num = 16f * (this.speed / this.maxForwardSpeed);
        for (int index = 2; (double) index < (double) num; index += 2)
          this.DrawBlockStyle(Vector2.op_Addition(pos, Vector2.op_Multiply(vector2, (float) index)), this.Width, this.Height, this.nineSliceGreen, this.middleGreen, Color.op_Multiply(Color.get_White(), (float) (1.0 - (double) index / (double) num)));
      }
      if ((double) this.redAlpha < 1.0)
        this.DrawBlockStyle(pos, this.Width, this.Height, this.nineSliceGreen, this.middleGreen, Color.get_White());
      if ((double) this.redAlpha <= 0.0)
        return;
      this.DrawBlockStyle(pos, this.Width, this.Height, this.nineSliceRed, this.middleRed, Color.op_Multiply(Color.get_White(), this.redAlpha));
    }

    private void DrawBlockStyle(
      Vector2 pos,
      float width,
      float height,
      MTexture[,] ninSlice,
      Sprite middle,
      Color color)
    {
      int num1 = (int) ((double) width / 8.0);
      int num2 = (int) ((double) height / 8.0);
      ninSlice[0, 0].Draw(Vector2.op_Addition(pos, new Vector2(0.0f, 0.0f)), Vector2.get_Zero(), color);
      ninSlice[2, 0].Draw(Vector2.op_Addition(pos, new Vector2(width - 8f, 0.0f)), Vector2.get_Zero(), color);
      ninSlice[0, 2].Draw(Vector2.op_Addition(pos, new Vector2(0.0f, height - 8f)), Vector2.get_Zero(), color);
      ninSlice[2, 2].Draw(Vector2.op_Addition(pos, new Vector2(width - 8f, height - 8f)), Vector2.get_Zero(), color);
      for (int index = 1; index < num1 - 1; ++index)
      {
        ninSlice[1, 0].Draw(Vector2.op_Addition(pos, new Vector2((float) (index * 8), 0.0f)), Vector2.get_Zero(), color);
        ninSlice[1, 2].Draw(Vector2.op_Addition(pos, new Vector2((float) (index * 8), height - 8f)), Vector2.get_Zero(), color);
      }
      for (int index = 1; index < num2 - 1; ++index)
      {
        ninSlice[0, 1].Draw(Vector2.op_Addition(pos, new Vector2(0.0f, (float) (index * 8))), Vector2.get_Zero(), color);
        ninSlice[2, 1].Draw(Vector2.op_Addition(pos, new Vector2(width - 8f, (float) (index * 8))), Vector2.get_Zero(), color);
      }
      for (int index1 = 1; index1 < num1 - 1; ++index1)
      {
        for (int index2 = 1; index2 < num2 - 1; ++index2)
          ninSlice[1, 1].Draw(Vector2.op_Addition(pos, Vector2.op_Multiply(new Vector2((float) index1, (float) index2), 8f)), Vector2.get_Zero(), color);
      }
      if (middle == null)
        return;
      middle.Color = color;
      middle.RenderPosition = Vector2.op_Addition(pos, new Vector2(width / 2f, height / 2f));
      middle.Render();
    }

    private class PathRenderer : Entity
    {
      private MTexture clipTexture = new MTexture();
      private SwapBlock block;
      private MTexture pathTexture;
      private float timer;

      public PathRenderer(SwapBlock block)
        : base(block.Position)
      {
        this.block = block;
        this.Depth = 8999;
        this.pathTexture = GFX.Game["objects/swapblock/path" + (block.start.X == block.end.X ? "V" : "H")];
        this.timer = Calc.Random.NextFloat();
      }

      public override void Update()
      {
        base.Update();
        this.timer += Engine.DeltaTime * 4f;
      }

      public override void Render()
      {
        for (int left = ((Rectangle) ref this.block.moveRect).get_Left(); left < ((Rectangle) ref this.block.moveRect).get_Right(); left += this.pathTexture.Width)
        {
          for (int top = ((Rectangle) ref this.block.moveRect).get_Top(); top < ((Rectangle) ref this.block.moveRect).get_Bottom(); top += this.pathTexture.Height)
          {
            this.pathTexture.GetSubtexture(0, 0, Math.Min(this.pathTexture.Width, ((Rectangle) ref this.block.moveRect).get_Right() - left), Math.Min(this.pathTexture.Height, ((Rectangle) ref this.block.moveRect).get_Bottom() - top), this.clipTexture);
            this.clipTexture.DrawCentered(new Vector2((float) (left + this.clipTexture.Width / 2), (float) (top + this.clipTexture.Height / 2)), Color.get_White());
          }
        }
        this.block.DrawBlockStyle(new Vector2((float) this.block.moveRect.X, (float) this.block.moveRect.Y), (float) this.block.moveRect.Width, (float) this.block.moveRect.Height, this.block.nineSliceTarget, (Sprite) null, Color.op_Multiply(Color.get_White(), (float) (0.5 * (0.5 + (Math.Sin((double) this.timer) + 1.0) * 0.25))));
      }
    }
  }
}
