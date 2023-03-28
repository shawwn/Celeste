// Decompiled with JetBrains decompiler
// Type: Celeste.MoveBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class MoveBlock : Solid
  {
    public static ParticleType P_Activate;
    public static ParticleType P_Break;
    public static ParticleType P_Move;
    private const float Accel = 300f;
    private const float MoveSpeed = 60f;
    private const float FastMoveSpeed = 75f;
    private const float SteerSpeed = 50.265484f;
    private const float MaxAngle = 0.7853982f;
    private const float NoSteerTime = 0.2f;
    private const float CrashTime = 0.15f;
    private const float CrashResetTime = 0.1f;
    private const float RegenTime = 3f;
    private bool canSteer;
    private bool fast;
    private MoveBlock.Directions direction;
    private float homeAngle;
    private int angleSteerSign;
    private Vector2 startPosition;
    private MoveBlock.MovementState state;
    private bool leftPressed;
    private bool rightPressed;
    private bool topPressed;
    private float speed;
    private float targetSpeed;
    private float angle;
    private float targetAngle;
    private Player noSquish;
    private List<Monocle.Image> body = new List<Monocle.Image>();
    private List<Monocle.Image> topButton = new List<Monocle.Image>();
    private List<Monocle.Image> leftButton = new List<Monocle.Image>();
    private List<Monocle.Image> rightButton = new List<Monocle.Image>();
    private List<MTexture> arrows = new List<MTexture>();
    private MoveBlock.Border border;
    private Color fillColor = MoveBlock.idleBgFill;
    private float flash;
    private SoundSource moveSfx;
    private bool triggered;
    private static readonly Color idleBgFill = Calc.HexToColor("474070");
    private static readonly Color pressedBgFill = Calc.HexToColor("30b335");
    private static readonly Color breakingBgFill = Calc.HexToColor("cc2541");
    private float particleRemainder;

    public MoveBlock(
      Vector2 position,
      int width,
      int height,
      MoveBlock.Directions direction,
      bool canSteer,
      bool fast)
      : base(position, (float) width, (float) height, false)
    {
      this.Depth = -1;
      this.startPosition = position;
      this.canSteer = canSteer;
      this.direction = direction;
      this.fast = fast;
      switch (direction)
      {
        case MoveBlock.Directions.Left:
          this.homeAngle = this.targetAngle = this.angle = 3.1415927f;
          this.angleSteerSign = -1;
          break;
        case MoveBlock.Directions.Up:
          this.homeAngle = this.targetAngle = this.angle = -1.5707964f;
          this.angleSteerSign = 1;
          break;
        case MoveBlock.Directions.Down:
          this.homeAngle = this.targetAngle = this.angle = 1.5707964f;
          this.angleSteerSign = -1;
          break;
        default:
          this.homeAngle = this.targetAngle = this.angle = 0.0f;
          this.angleSteerSign = 1;
          break;
      }
      int num1 = width / 8;
      int num2 = height / 8;
      MTexture mtexture1 = GFX.Game["objects/moveBlock/base"];
      MTexture mtexture2 = GFX.Game["objects/moveBlock/button"];
      if (canSteer && (direction == MoveBlock.Directions.Left || direction == MoveBlock.Directions.Right))
      {
        for (int index = 0; index < num1; ++index)
        {
          int num3 = index == 0 ? 0 : (index < num1 - 1 ? 1 : 2);
          this.AddImage(mtexture2.GetSubtexture(num3 * 8, 0, 8, 8), new Vector2((float) (index * 8), -4f), 0.0f, new Vector2(1f, 1f), this.topButton);
        }
        mtexture1 = GFX.Game["objects/moveBlock/base_h"];
      }
      else if (canSteer && (direction == MoveBlock.Directions.Up || direction == MoveBlock.Directions.Down))
      {
        for (int index = 0; index < num2; ++index)
        {
          int num4 = index == 0 ? 0 : (index < num2 - 1 ? 1 : 2);
          this.AddImage(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8), new Vector2(-4f, (float) (index * 8)), 1.5707964f, new Vector2(1f, -1f), this.leftButton);
          this.AddImage(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8), new Vector2((float) ((num1 - 1) * 8 + 4), (float) (index * 8)), 1.5707964f, new Vector2(1f, 1f), this.rightButton);
        }
        mtexture1 = GFX.Game["objects/moveBlock/base_v"];
      }
      for (int x = 0; x < num1; ++x)
      {
        for (int y = 0; y < num2; ++y)
        {
          int num5 = x == 0 ? 0 : (x < num1 - 1 ? 1 : 2);
          int num6 = y == 0 ? 0 : (y < num2 - 1 ? 1 : 2);
          this.AddImage(mtexture1.GetSubtexture(num5 * 8, num6 * 8, 8, 8), new Vector2((float) x, (float) y) * 8f, 0.0f, new Vector2(1f, 1f), this.body);
        }
      }
      this.arrows = GFX.Game.GetAtlasSubtextures("objects/moveBlock/arrow");
      this.Add((Component) (this.moveSfx = new SoundSource()));
      this.Add((Component) new Coroutine(this.Controller()));
      this.UpdateColors();
      this.Add((Component) new LightOcclude(0.5f));
    }

    public MoveBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Height, data.Enum<MoveBlock.Directions>(nameof (direction)), data.Bool(nameof (canSteer), true), data.Bool(nameof (fast)))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      scene.Add((Entity) (this.border = new MoveBlock.Border(this)));
    }

    private IEnumerator Controller()
    {
      MoveBlock moveBlock = this;
      while (true)
      {
        moveBlock.triggered = false;
        moveBlock.state = MoveBlock.MovementState.Idling;
        while (!moveBlock.triggered && !moveBlock.HasPlayerRider())
          yield return (object) null;
        Audio.Play("event:/game/04_cliffside/arrowblock_activate", moveBlock.Position);
        moveBlock.state = MoveBlock.MovementState.Moving;
        moveBlock.StartShaking(0.2f);
        moveBlock.ActivateParticles();
        yield return (object) 0.2f;
        moveBlock.targetSpeed = moveBlock.fast ? 75f : 60f;
        moveBlock.moveSfx.Play("event:/game/04_cliffside/arrowblock_move");
        moveBlock.moveSfx.Param("arrow_stop", 0.0f);
        moveBlock.StopPlayerRunIntoAnimation = false;
        float crashTimer = 0.15f;
        float crashResetTimer = 0.1f;
        float noSteerTimer = 0.2f;
        while (true)
        {
          if (moveBlock.canSteer)
          {
            moveBlock.targetAngle = moveBlock.homeAngle;
            bool flag = moveBlock.direction == MoveBlock.Directions.Right || moveBlock.direction == MoveBlock.Directions.Left ? moveBlock.HasPlayerOnTop() : moveBlock.HasPlayerClimbing();
            if (flag && (double) noSteerTimer > 0.0)
              noSteerTimer -= Engine.DeltaTime;
            if (flag)
            {
              if ((double) noSteerTimer <= 0.0)
                moveBlock.targetAngle = moveBlock.direction == MoveBlock.Directions.Right || moveBlock.direction == MoveBlock.Directions.Left ? moveBlock.homeAngle + 0.7853982f * (float) moveBlock.angleSteerSign * (float) Input.MoveY.Value : moveBlock.homeAngle + 0.7853982f * (float) moveBlock.angleSteerSign * (float) Input.MoveX.Value;
            }
            else
              noSteerTimer = 0.2f;
          }
          if (moveBlock.Scene.OnInterval(0.02f))
            moveBlock.MoveParticles();
          moveBlock.speed = Calc.Approach(moveBlock.speed, moveBlock.targetSpeed, 300f * Engine.DeltaTime);
          moveBlock.angle = Calc.Approach(moveBlock.angle, moveBlock.targetAngle, 50.265484f * Engine.DeltaTime);
          Vector2 vector = Calc.AngleToVector(moveBlock.angle, moveBlock.speed);
          Vector2 vec = vector * Engine.DeltaTime;
          bool flag1;
          if (moveBlock.direction == MoveBlock.Directions.Right || moveBlock.direction == MoveBlock.Directions.Left)
          {
            flag1 = moveBlock.MoveCheck(vec.XComp());
            moveBlock.noSquish = moveBlock.Scene.Tracker.GetEntity<Player>();
            moveBlock.MoveVCollideSolids(vec.Y, false);
            moveBlock.noSquish = (Player) null;
            moveBlock.LiftSpeed = vector;
            if (moveBlock.Scene.OnInterval(0.03f))
            {
              if ((double) vec.Y > 0.0)
                moveBlock.ScrapeParticles(Vector2.UnitY);
              else if ((double) vec.Y < 0.0)
                moveBlock.ScrapeParticles(-Vector2.UnitY);
            }
          }
          else
          {
            flag1 = moveBlock.MoveCheck(vec.YComp());
            moveBlock.noSquish = moveBlock.Scene.Tracker.GetEntity<Player>();
            moveBlock.MoveHCollideSolids(vec.X, false);
            moveBlock.noSquish = (Player) null;
            moveBlock.LiftSpeed = vector;
            if (moveBlock.Scene.OnInterval(0.03f))
            {
              if ((double) vec.X > 0.0)
                moveBlock.ScrapeParticles(Vector2.UnitX);
              else if ((double) vec.X < 0.0)
                moveBlock.ScrapeParticles(-Vector2.UnitX);
            }
            if (moveBlock.direction == MoveBlock.Directions.Down && (double) moveBlock.Top > (double) (moveBlock.SceneAs<Level>().Bounds.Bottom + 32))
              flag1 = true;
          }
          if (flag1)
          {
            moveBlock.moveSfx.Param("arrow_stop", 1f);
            crashResetTimer = 0.1f;
            if ((double) crashTimer > 0.0)
              crashTimer -= Engine.DeltaTime;
            else
              break;
          }
          else
          {
            moveBlock.moveSfx.Param("arrow_stop", 0.0f);
            if ((double) crashResetTimer > 0.0)
              crashResetTimer -= Engine.DeltaTime;
            else
              crashTimer = 0.15f;
          }
          Level scene = moveBlock.Scene as Level;
          double left1 = (double) moveBlock.Left;
          Rectangle bounds = scene.Bounds;
          double left2 = (double) bounds.Left;
          if (left1 >= left2)
          {
            double top1 = (double) moveBlock.Top;
            bounds = scene.Bounds;
            double top2 = (double) bounds.Top;
            if (top1 >= top2)
            {
              double right1 = (double) moveBlock.Right;
              bounds = scene.Bounds;
              double right2 = (double) bounds.Right;
              if (right1 <= right2)
                yield return (object) null;
              else
                break;
            }
            else
              break;
          }
          else
            break;
        }
        Audio.Play("event:/game/04_cliffside/arrowblock_break", moveBlock.Position);
        moveBlock.moveSfx.Stop();
        moveBlock.state = MoveBlock.MovementState.Breaking;
        moveBlock.speed = moveBlock.targetSpeed = 0.0f;
        moveBlock.angle = moveBlock.targetAngle = moveBlock.homeAngle;
        moveBlock.StartShaking(0.2f);
        moveBlock.StopPlayerRunIntoAnimation = true;
        yield return (object) 0.2f;
        moveBlock.BreakParticles();
        List<MoveBlock.Debris> debris = new List<MoveBlock.Debris>();
        for (int index1 = 0; (double) index1 < (double) moveBlock.Width; index1 += 8)
        {
          for (int index2 = 0; (double) index2 < (double) moveBlock.Height; index2 += 8)
          {
            Vector2 vector2 = new Vector2((float) index1 + 4f, (float) index2 + 4f);
            MoveBlock.Debris debris1 = Engine.Pooler.Create<MoveBlock.Debris>().Init(moveBlock.Position + vector2, moveBlock.Center, moveBlock.startPosition + vector2);
            debris.Add(debris1);
            moveBlock.Scene.Add((Entity) debris1);
          }
        }
        moveBlock.MoveStaticMovers(moveBlock.startPosition - moveBlock.Position);
        moveBlock.DisableStaticMovers();
        moveBlock.Position = moveBlock.startPosition;
        moveBlock.Visible = moveBlock.Collidable = false;
        yield return (object) 2.2f;
        foreach (MoveBlock.Debris debris2 in debris)
          debris2.StopMoving();
        while (moveBlock.CollideCheck<Actor>() || moveBlock.CollideCheck<Solid>())
          yield return (object) null;
        moveBlock.Collidable = true;
        EventInstance instance = Audio.Play("event:/game/04_cliffside/arrowblock_reform_begin", debris[0].Position);
        Coroutine routine;
        moveBlock.Add((Component) (routine = new Coroutine(moveBlock.SoundFollowsDebrisCenter(instance, debris))));
        foreach (MoveBlock.Debris debris3 in debris)
          debris3.StartShaking();
        yield return (object) 0.2f;
        foreach (MoveBlock.Debris debris4 in debris)
          debris4.ReturnHome(0.65f);
        yield return (object) 0.6f;
        routine.RemoveSelf();
        foreach (Entity entity in debris)
          entity.RemoveSelf();
        routine = (Coroutine) null;
        Audio.Play("event:/game/04_cliffside/arrowblock_reappear", moveBlock.Position);
        moveBlock.Visible = true;
        moveBlock.EnableStaticMovers();
        moveBlock.speed = moveBlock.targetSpeed = 0.0f;
        moveBlock.angle = moveBlock.targetAngle = moveBlock.homeAngle;
        moveBlock.noSquish = (Player) null;
        moveBlock.fillColor = MoveBlock.idleBgFill;
        moveBlock.UpdateColors();
        moveBlock.flash = 1f;
        debris = (List<MoveBlock.Debris>) null;
      }
    }

    private IEnumerator SoundFollowsDebrisCenter(
      EventInstance instance,
      List<MoveBlock.Debris> debris)
    {
      while (true)
      {
        PLAYBACK_STATE state;
        int playbackState = (int) instance.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.STOPPED)
        {
          Vector2 zero = Vector2.Zero;
          foreach (MoveBlock.Debris debri in debris)
            zero += debri.Position;
          Audio.Position(instance, zero / (float) debris.Count);
          yield return (object) null;
        }
        else
          break;
      }
    }

    public override void Update()
    {
      base.Update();
      if (this.canSteer)
      {
        bool flag1 = (this.direction == MoveBlock.Directions.Up || this.direction == MoveBlock.Directions.Down) && this.CollideCheck<Player>(this.Position + new Vector2(-1f, 0.0f));
        bool flag2 = (this.direction == MoveBlock.Directions.Up || this.direction == MoveBlock.Directions.Down) && this.CollideCheck<Player>(this.Position + new Vector2(1f, 0.0f));
        bool flag3 = (this.direction == MoveBlock.Directions.Left || this.direction == MoveBlock.Directions.Right) && this.CollideCheck<Player>(this.Position + new Vector2(0.0f, -1f));
        foreach (GraphicsComponent graphicsComponent in this.topButton)
          graphicsComponent.Y = flag3 ? 2f : 0.0f;
        foreach (GraphicsComponent graphicsComponent in this.leftButton)
          graphicsComponent.X = flag1 ? 2f : 0.0f;
        foreach (GraphicsComponent graphicsComponent in this.rightButton)
          graphicsComponent.X = this.Width + (flag2 ? -2f : 0.0f);
        if (flag1 && !this.leftPressed || flag3 && !this.topPressed || flag2 && !this.rightPressed)
          Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", this.Position);
        if (!flag1 && this.leftPressed || !flag3 && this.topPressed || !flag2 && this.rightPressed)
          Audio.Play("event:/game/04_cliffside/arrowblock_side_release", this.Position);
        this.leftPressed = flag1;
        this.rightPressed = flag2;
        this.topPressed = flag3;
      }
      if (this.moveSfx != null && this.moveSfx.Playing)
        this.moveSfx.Param("arrow_influence", (float) ((int) Math.Floor((-(double) (Calc.AngleToVector(this.angle, 1f) * new Vector2(-1f, 1f)).Angle() + 6.2831854820251465) % 6.2831854820251465 / 6.2831854820251465 * 8.0 + 0.5) + 1));
      this.border.Visible = this.Visible;
      this.flash = Calc.Approach(this.flash, 0.0f, Engine.DeltaTime * 5f);
      this.UpdateColors();
    }

    public override void OnStaticMoverTrigger(StaticMover sm) => this.triggered = true;

    public override void MoveHExact(int move)
    {
      if (this.noSquish != null && (move < 0 && (double) this.noSquish.X < (double) this.X || move > 0 && (double) this.noSquish.X > (double) this.X))
      {
        while (move != 0 && this.noSquish.CollideCheck<Solid>(this.noSquish.Position + Vector2.UnitX * (float) move))
          move -= Math.Sign(move);
      }
      base.MoveHExact(move);
    }

    public override void MoveVExact(int move)
    {
      if (this.noSquish != null && move < 0 && (double) this.noSquish.Y <= (double) this.Y)
      {
        while (move != 0 && this.noSquish.CollideCheck<Solid>(this.noSquish.Position + Vector2.UnitY * (float) move))
          move -= Math.Sign(move);
      }
      base.MoveVExact(move);
    }

    private bool MoveCheck(Vector2 speed)
    {
      if ((double) speed.X != 0.0)
      {
        if (!this.MoveHCollideSolids(speed.X, false))
          return false;
        for (int index1 = 1; index1 <= 3; ++index1)
        {
          for (int index2 = 1; index2 >= -1; index2 -= 2)
          {
            if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) Math.Sign(speed.X), (float) (index1 * index2))))
            {
              this.MoveVExact(index1 * index2);
              this.MoveHExact(Math.Sign(speed.X));
              return false;
            }
          }
        }
        return true;
      }
      if ((double) speed.Y == 0.0 || !this.MoveVCollideSolids(speed.Y, false))
        return false;
      for (int index3 = 1; index3 <= 3; ++index3)
      {
        for (int index4 = 1; index4 >= -1; index4 -= 2)
        {
          if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) (index3 * index4), (float) Math.Sign(speed.Y))))
          {
            this.MoveHExact(index3 * index4);
            this.MoveVExact(Math.Sign(speed.Y));
            return false;
          }
        }
      }
      return true;
    }

    private void UpdateColors()
    {
      Color color = MoveBlock.idleBgFill;
      if (this.state == MoveBlock.MovementState.Moving)
        color = MoveBlock.pressedBgFill;
      else if (this.state == MoveBlock.MovementState.Breaking)
        color = MoveBlock.breakingBgFill;
      this.fillColor = Color.Lerp(this.fillColor, color, 10f * Engine.DeltaTime);
      foreach (GraphicsComponent graphicsComponent in this.topButton)
        graphicsComponent.Color = this.fillColor;
      foreach (GraphicsComponent graphicsComponent in this.leftButton)
        graphicsComponent.Color = this.fillColor;
      foreach (GraphicsComponent graphicsComponent in this.rightButton)
        graphicsComponent.Color = this.fillColor;
    }

    private void AddImage(
      MTexture tex,
      Vector2 position,
      float rotation,
      Vector2 scale,
      List<Monocle.Image> addTo)
    {
      Monocle.Image image = new Monocle.Image(tex);
      image.Position = position + new Vector2(4f, 4f);
      image.CenterOrigin();
      image.Rotation = rotation;
      image.Scale = scale;
      this.Add((Component) image);
      addTo?.Add(image);
    }

    private void SetVisible(List<Monocle.Image> images, bool visible)
    {
      foreach (Component image in images)
        image.Visible = visible;
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = this.Position + this.Shake;
      foreach (Component component in this.leftButton)
        component.Render();
      foreach (Component component in this.rightButton)
        component.Render();
      foreach (Component component in this.topButton)
        component.Render();
      Draw.Rect(this.X + 3f, this.Y + 3f, this.Width - 6f, this.Height - 6f, this.fillColor);
      foreach (Component component in this.body)
        component.Render();
      Draw.Rect(this.Center.X - 4f, this.Center.Y - 4f, 8f, 8f, this.fillColor);
      if (this.state != MoveBlock.MovementState.Breaking)
        this.arrows[Calc.Clamp((int) Math.Floor((-(double) this.angle + 6.2831854820251465) % 6.2831854820251465 / 6.2831854820251465 * 8.0 + 0.5), 0, 7)].DrawCentered(this.Center);
      else
        GFX.Game["objects/moveBlock/x"].DrawCentered(this.Center);
      float num = this.flash * 4f;
      Draw.Rect(this.X - num, this.Y - num, this.Width + num * 2f, this.Height + num * 2f, Color.White * this.flash);
      this.Position = position;
    }

    private void ActivateParticles()
    {
      bool flag1 = this.direction == MoveBlock.Directions.Down || this.direction == MoveBlock.Directions.Up;
      int num = !this.canSteer || !flag1 ? (!this.CollideCheck<Player>(this.Position - Vector2.UnitX) ? 1 : 0) : 0;
      bool flag2 = (!this.canSteer || !flag1) && !this.CollideCheck<Player>(this.Position + Vector2.UnitX);
      bool flag3 = !this.canSteer | flag1 && !this.CollideCheck<Player>(this.Position - Vector2.UnitY);
      if (num != 0)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Height / 2.0), this.CenterLeft, Vector2.UnitY * (this.Height - 4f) * 0.5f, 3.1415927f);
      if (flag2)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Height / 2.0), this.CenterRight, Vector2.UnitY * (this.Height - 4f) * 0.5f, 0.0f);
      if (flag3)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Width / 2.0), this.TopCenter, Vector2.UnitX * (this.Width - 4f) * 0.5f, -1.5707964f);
      this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Width / 2.0), this.BottomCenter, Vector2.UnitX * (this.Width - 4f) * 0.5f, 1.5707964f);
    }

    private void BreakParticles()
    {
      Vector2 center = this.Center;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 4)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 4)
        {
          Vector2 position = this.Position + new Vector2((float) (2 + index1), (float) (2 + index2));
          this.SceneAs<Level>().Particles.Emit(MoveBlock.P_Break, 1, position, Vector2.One * 2f, (position - center).Angle());
        }
      }
    }

    private void MoveParticles()
    {
      Vector2 position;
      Vector2 vector2;
      float direction;
      float num;
      if (this.direction == MoveBlock.Directions.Right)
      {
        position = this.CenterLeft + Vector2.UnitX;
        vector2 = Vector2.UnitY * (this.Height - 4f);
        direction = 3.1415927f;
        num = this.Height / 32f;
      }
      else if (this.direction == MoveBlock.Directions.Left)
      {
        position = this.CenterRight;
        vector2 = Vector2.UnitY * (this.Height - 4f);
        direction = 0.0f;
        num = this.Height / 32f;
      }
      else if (this.direction == MoveBlock.Directions.Down)
      {
        position = this.TopCenter + Vector2.UnitY;
        vector2 = Vector2.UnitX * (this.Width - 4f);
        direction = -1.5707964f;
        num = this.Width / 32f;
      }
      else
      {
        position = this.BottomCenter;
        vector2 = Vector2.UnitX * (this.Width - 4f);
        direction = 1.5707964f;
        num = this.Width / 32f;
      }
      this.particleRemainder += num;
      int particleRemainder = (int) this.particleRemainder;
      this.particleRemainder -= (float) particleRemainder;
      Vector2 positionRange = vector2 * 0.5f;
      if (particleRemainder <= 0)
        return;
      this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Move, particleRemainder, position, positionRange, direction);
    }

    private void ScrapeParticles(Vector2 dir)
    {
      int num = this.Collidable ? 1 : 0;
      this.Collidable = false;
      if ((double) dir.X != 0.0)
      {
        float x = (double) dir.X <= 0.0 ? this.Left - 1f : this.Right;
        for (int index = 0; (double) index < (double) this.Height; index += 8)
        {
          Vector2 vector2 = new Vector2(x, this.Top + 4f + (float) index);
          if (this.Scene.CollideCheck<Solid>(vector2))
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector2);
        }
      }
      else
      {
        float y = (double) dir.Y <= 0.0 ? this.Top - 1f : this.Bottom;
        for (int index = 0; (double) index < (double) this.Width; index += 8)
        {
          Vector2 vector2 = new Vector2(this.Left + 4f + (float) index, y);
          if (this.Scene.CollideCheck<Solid>(vector2))
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector2);
        }
      }
      this.Collidable = true;
    }

    public enum Directions
    {
      Left,
      Right,
      Up,
      Down,
    }

    private enum MovementState
    {
      Idling,
      Moving,
      Breaking,
    }

    private class Border : Entity
    {
      public MoveBlock Parent;

      public Border(MoveBlock parent)
      {
        this.Parent = parent;
        this.Depth = 1;
      }

      public override void Update()
      {
        if (this.Parent.Scene != this.Scene)
          this.RemoveSelf();
        base.Update();
      }

      public override void Render() => Draw.Rect((float) ((double) this.Parent.X + (double) this.Parent.Shake.X - 1.0), (float) ((double) this.Parent.Y + (double) this.Parent.Shake.Y - 1.0), this.Parent.Width + 2f, this.Parent.Height + 2f, Color.Black);
    }

    [Pooled]
    private class Debris : Actor
    {
      private Monocle.Image sprite;
      private Vector2 home;
      private Vector2 speed;
      private bool shaking;
      private bool returning;
      private float returnEase;
      private float returnDuration;
      private SimpleCurve returnCurve;
      private bool firstHit;
      private float alpha;
      private Collision onCollideH;
      private Collision onCollideV;
      private float spin;

      public Debris()
        : base(Vector2.Zero)
      {
        this.Tag = (int) Tags.TransitionUpdate;
        this.Collider = (Collider) new Hitbox(4f, 4f, -2f, -2f);
        this.Add((Component) (this.sprite = new Monocle.Image(Calc.Random.Choose<MTexture>(GFX.Game.GetAtlasSubtextures("objects/moveblock/debris")))));
        this.sprite.CenterOrigin();
        this.sprite.FlipX = Calc.Random.Chance(0.5f);
        this.onCollideH = (Collision) (c => this.speed.X = (float) (-(double) this.speed.X * 0.5));
        this.onCollideV = (Collision) (c =>
        {
          if (this.firstHit || (double) this.speed.Y > 50.0)
            Audio.Play("event:/game/general/debris_stone", this.Position, "debris_velocity", Calc.ClampedMap(this.speed.Y, 0.0f, 600f));
          this.speed.Y = (double) this.speed.Y <= 0.0 || (double) this.speed.Y >= 40.0 ? (float) (-(double) this.speed.Y * 0.25) : 0.0f;
          this.firstHit = false;
        });
      }

      protected override void OnSquish(CollisionData data)
      {
      }

      public MoveBlock.Debris Init(Vector2 position, Vector2 center, Vector2 returnTo)
      {
        this.Collidable = true;
        this.Position = position;
        this.speed = (position - center).SafeNormalize(60f + Calc.Random.NextFloat(60f));
        this.home = returnTo;
        this.sprite.Position = Vector2.Zero;
        this.sprite.Rotation = Calc.Random.NextAngle();
        this.returning = false;
        this.shaking = false;
        this.sprite.Scale.X = 1f;
        this.sprite.Scale.Y = 1f;
        this.sprite.Color = Color.White;
        this.alpha = 1f;
        this.firstHit = false;
        this.spin = Calc.Random.Range(3.4906585f, 10.471975f) * (float) Calc.Random.Choose<int>(1, -1);
        return this;
      }

      public override void Update()
      {
        base.Update();
        if (!this.returning)
        {
          if (this.Collidable)
          {
            this.speed.X = Calc.Approach(this.speed.X, 0.0f, Engine.DeltaTime * 100f);
            if (!this.OnGround())
              this.speed.Y += 400f * Engine.DeltaTime;
            this.MoveH(this.speed.X * Engine.DeltaTime, this.onCollideH);
            this.MoveV(this.speed.Y * Engine.DeltaTime, this.onCollideV);
          }
          if (this.shaking && this.Scene.OnInterval(0.05f))
          {
            this.sprite.X = (float) (Calc.Random.Next(3) - 1);
            this.sprite.Y = (float) (Calc.Random.Next(3) - 1);
          }
        }
        else
        {
          this.Position = this.returnCurve.GetPoint(Ease.CubeOut(this.returnEase));
          this.returnEase = Calc.Approach(this.returnEase, 1f, Engine.DeltaTime / this.returnDuration);
          this.sprite.Scale = Vector2.One * (float) (1.0 + (double) this.returnEase * 0.5);
        }
        if ((this.Scene as Level).Transitioning)
        {
          this.alpha = Calc.Approach(this.alpha, 0.0f, Engine.DeltaTime * 4f);
          this.sprite.Color = Color.White * this.alpha;
        }
        this.sprite.Rotation += this.spin * Calc.ClampedMap(Math.Abs(this.speed.Y), 50f, 150f) * Engine.DeltaTime;
      }

      public void StopMoving() => this.Collidable = false;

      public void StartShaking() => this.shaking = true;

      public void ReturnHome(float duration)
      {
        if (this.Scene != null)
        {
          Camera camera = (this.Scene as Level).Camera;
          if ((double) this.X < (double) camera.X)
            this.X = camera.X - 8f;
          if ((double) this.Y < (double) camera.Y)
            this.Y = camera.Y - 8f;
          if ((double) this.X > (double) camera.X + 320.0)
            this.X = (float) ((double) camera.X + 320.0 + 8.0);
          if ((double) this.Y > (double) camera.Y + 180.0)
            this.Y = (float) ((double) camera.Y + 180.0 + 8.0);
        }
        this.returning = true;
        this.returnEase = 0.0f;
        this.returnDuration = duration;
        Vector2 vector2 = (this.home - this.Position).SafeNormalize();
        this.returnCurve = new SimpleCurve(this.Position, this.home, (this.Position + this.home) / 2f + new Vector2(vector2.Y, -vector2.X) * (Calc.Random.NextFloat(16f) + 16f) * (float) Calc.Random.Facing());
      }
    }
  }
}
