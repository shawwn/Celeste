// Decompiled with JetBrains decompiler
// Type: Celeste.MoveBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
    private static readonly Color idleBgFill = Calc.HexToColor("474070");
    private static readonly Color pressedBgFill = Calc.HexToColor("30b335");
    private static readonly Color breakingBgFill = Calc.HexToColor("cc2541");
    private List<Monocle.Image> body = new List<Monocle.Image>();
    private List<Monocle.Image> topButton = new List<Monocle.Image>();
    private List<Monocle.Image> leftButton = new List<Monocle.Image>();
    private List<Monocle.Image> rightButton = new List<Monocle.Image>();
    private List<MTexture> arrows = new List<MTexture>();
    private Color fillColor = MoveBlock.idleBgFill;
    public static ParticleType P_Activate;
    public static ParticleType P_Break;
    public static ParticleType P_Move;
    private const float Accel = 300f;
    private const float MoveSpeed = 60f;
    private const float FastMoveSpeed = 75f;
    private const float SteerSpeed = 50.26548f;
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
    private MoveBlock.Border border;
    private float flash;
    private SoundSource moveSfx;
    private bool triggered;
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
          this.homeAngle = this.targetAngle = this.angle = 3.141593f;
          this.angleSteerSign = -1;
          break;
        case MoveBlock.Directions.Up:
          this.homeAngle = this.targetAngle = this.angle = -1.570796f;
          this.angleSteerSign = 1;
          break;
        case MoveBlock.Directions.Down:
          this.homeAngle = this.targetAngle = this.angle = 1.570796f;
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
          this.AddImage(mtexture2.GetSubtexture(num3 * 8, 0, 8, 8, (MTexture) null), new Vector2((float) (index * 8), -4f), 0.0f, new Vector2(1f, 1f), this.topButton);
        }
        mtexture1 = GFX.Game["objects/moveBlock/base_h"];
      }
      else if (canSteer && (direction == MoveBlock.Directions.Up || direction == MoveBlock.Directions.Down))
      {
        for (int index = 0; index < num2; ++index)
        {
          int num3 = index == 0 ? 0 : (index < num2 - 1 ? 1 : 2);
          this.AddImage(mtexture2.GetSubtexture(num3 * 8, 0, 8, 8, (MTexture) null), new Vector2(-4f, (float) (index * 8)), 1.570796f, new Vector2(1f, -1f), this.leftButton);
          this.AddImage(mtexture2.GetSubtexture(num3 * 8, 0, 8, 8, (MTexture) null), new Vector2((float) ((num1 - 1) * 8 + 4), (float) (index * 8)), 1.570796f, new Vector2(1f, 1f), this.rightButton);
        }
        mtexture1 = GFX.Game["objects/moveBlock/base_v"];
      }
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          int num3 = index1 == 0 ? 0 : (index1 < num1 - 1 ? 1 : 2);
          int num4 = index2 == 0 ? 0 : (index2 < num2 - 1 ? 1 : 2);
          this.AddImage(mtexture1.GetSubtexture(num3 * 8, num4 * 8, 8, 8, (MTexture) null), Vector2.op_Multiply(new Vector2((float) index1, (float) index2), 8f), 0.0f, new Vector2(1f, 1f), this.body);
        }
      }
      this.arrows = GFX.Game.GetAtlasSubtextures("objects/moveBlock/arrow");
      this.Add((Component) (this.moveSfx = new SoundSource()));
      this.Add((Component) new Coroutine(this.Controller(), true));
      this.UpdateColors();
      this.Add((Component) new LightOcclude(0.5f));
    }

    public MoveBlock(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), data.Width, data.Height, data.Enum<MoveBlock.Directions>(nameof (direction), MoveBlock.Directions.Left), data.Bool(nameof (canSteer), true), data.Bool(nameof (fast), false))
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
        moveBlock.moveSfx.Play("event:/game/04_cliffside/arrowblock_move", (string) null, 0.0f);
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
          moveBlock.angle = Calc.Approach(moveBlock.angle, moveBlock.targetAngle, 50.26548f * Engine.DeltaTime);
          Vector2 vec = Vector2.op_Multiply(Calc.AngleToVector(moveBlock.angle, moveBlock.speed), Engine.DeltaTime);
          bool flag1;
          if (moveBlock.direction == MoveBlock.Directions.Right || moveBlock.direction == MoveBlock.Directions.Left)
          {
            flag1 = moveBlock.MoveCheck(vec.XComp());
            moveBlock.noSquish = moveBlock.Scene.Tracker.GetEntity<Player>();
            moveBlock.MoveVCollideSolids((float) vec.Y, false, (Action<Vector2, Vector2, Platform>) null);
            moveBlock.noSquish = (Player) null;
            if (moveBlock.Scene.OnInterval(0.03f))
            {
              if (vec.Y > 0.0)
                moveBlock.ScrapeParticles(Vector2.get_UnitY());
              else if (vec.Y < 0.0)
                moveBlock.ScrapeParticles(Vector2.op_UnaryNegation(Vector2.get_UnitY()));
            }
          }
          else
          {
            flag1 = moveBlock.MoveCheck(vec.YComp());
            moveBlock.noSquish = moveBlock.Scene.Tracker.GetEntity<Player>();
            moveBlock.MoveHCollideSolids((float) vec.X, false, (Action<Vector2, Vector2, Platform>) null);
            moveBlock.noSquish = (Player) null;
            if (moveBlock.Scene.OnInterval(0.03f))
            {
              if (vec.X > 0.0)
                moveBlock.ScrapeParticles(Vector2.get_UnitX());
              else if (vec.X < 0.0)
                moveBlock.ScrapeParticles(Vector2.op_UnaryNegation(Vector2.get_UnitX()));
            }
            if (moveBlock.direction == MoveBlock.Directions.Down)
            {
              double top = (double) moveBlock.Top;
              Rectangle bounds = moveBlock.SceneAs<Level>().Bounds;
              double num = (double) (((Rectangle) ref bounds).get_Bottom() + 32);
              if (top > num)
                flag1 = true;
            }
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
          Rectangle bounds1 = scene.Bounds;
          double left2 = (double) ((Rectangle) ref bounds1).get_Left();
          if (left1 >= left2)
          {
            double top1 = (double) moveBlock.Top;
            bounds1 = scene.Bounds;
            double top2 = (double) ((Rectangle) ref bounds1).get_Top();
            if (top1 >= top2)
            {
              double right1 = (double) moveBlock.Right;
              bounds1 = scene.Bounds;
              double right2 = (double) ((Rectangle) ref bounds1).get_Right();
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
        moveBlock.moveSfx.Stop(true);
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
            Vector2 vector2;
            ((Vector2) ref vector2).\u002Ector((float) index1 + 4f, (float) index2 + 4f);
            MoveBlock.Debris debris1 = Engine.Pooler.Create<MoveBlock.Debris>().Init(Vector2.op_Addition(moveBlock.Position, vector2), moveBlock.Center, Vector2.op_Addition(moveBlock.startPosition, vector2));
            debris.Add(debris1);
            moveBlock.Scene.Add((Entity) debris1);
          }
        }
        moveBlock.MoveStaticMovers(Vector2.op_Subtraction(moveBlock.startPosition, moveBlock.Position));
        moveBlock.DisableStaticMovers();
        moveBlock.Position = moveBlock.startPosition;
        moveBlock.Visible = moveBlock.Collidable = false;
        yield return (object) 2.2f;
        foreach (MoveBlock.Debris debris1 in debris)
          debris1.StopMoving();
        while (moveBlock.CollideCheck<Actor>() || moveBlock.CollideCheck<Solid>())
          yield return (object) null;
        moveBlock.Collidable = true;
        EventInstance instance = Audio.Play("event:/game/04_cliffside/arrowblock_reform_begin", debris[0].Position);
        Coroutine routine;
        moveBlock.Add((Component) (routine = new Coroutine(moveBlock.SoundFollowsDebrisCenter(instance, debris), true)));
        foreach (MoveBlock.Debris debris1 in debris)
          debris1.StartShaking();
        yield return (object) 0.2f;
        foreach (MoveBlock.Debris debris1 in debris)
          debris1.ReturnHome(0.65f);
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
          Vector2 vector2 = Vector2.get_Zero();
          foreach (MoveBlock.Debris debri in debris)
            vector2 = Vector2.op_Addition(vector2, debri.Position);
          Audio.Position(instance, Vector2.op_Division(vector2, (float) debris.Count));
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
        bool flag1 = (this.direction == MoveBlock.Directions.Up || this.direction == MoveBlock.Directions.Down) && this.CollideCheck<Player>(Vector2.op_Addition(this.Position, new Vector2(-1f, 0.0f)));
        bool flag2 = (this.direction == MoveBlock.Directions.Up || this.direction == MoveBlock.Directions.Down) && this.CollideCheck<Player>(Vector2.op_Addition(this.Position, new Vector2(1f, 0.0f)));
        bool flag3 = (this.direction == MoveBlock.Directions.Left || this.direction == MoveBlock.Directions.Right) && this.CollideCheck<Player>(Vector2.op_Addition(this.Position, new Vector2(0.0f, -1f)));
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
        this.moveSfx.Param("arrow_influence", (float) ((int) Math.Floor((-(double) Vector2.op_Multiply(Calc.AngleToVector(this.angle, 1f), new Vector2(-1f, 1f)).Angle() + 6.28318548202515) % 6.28318548202515 / 6.28318548202515 * 8.0 + 0.5) + 1));
      this.border.Visible = this.Visible;
      this.flash = Calc.Approach(this.flash, 0.0f, Engine.DeltaTime * 5f);
      this.UpdateColors();
    }

    public override void OnStaticMoverTrigger()
    {
      base.OnStaticMoverTrigger();
      this.triggered = true;
    }

    public override void MoveHExact(int move)
    {
      if (this.noSquish != null && (move < 0 && (double) this.noSquish.X < (double) this.X || move > 0 && (double) this.noSquish.X > (double) this.X))
      {
        while (move != 0 && this.noSquish.CollideCheck<Solid>(Vector2.op_Addition(this.noSquish.Position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) move))))
          move -= Math.Sign(move);
      }
      base.MoveHExact(move);
    }

    public override void MoveVExact(int move)
    {
      if (this.noSquish != null && move < 0 && (double) this.noSquish.Y <= (double) this.Y)
      {
        while (move != 0 && this.noSquish.CollideCheck<Solid>(Vector2.op_Addition(this.noSquish.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) move))))
          move -= Math.Sign(move);
      }
      base.MoveVExact(move);
    }

    private bool MoveCheck(Vector2 speed)
    {
      if (speed.X != 0.0)
      {
        if (!this.MoveHCollideSolids((float) speed.X, false, (Action<Vector2, Vector2, Platform>) null))
          return false;
        for (int index1 = 1; index1 <= 3; ++index1)
        {
          for (int index2 = 1; index2 >= -1; index2 -= 2)
          {
            Vector2 vector2;
            ((Vector2) ref vector2).\u002Ector((float) Math.Sign((float) speed.X), (float) (index1 * index2));
            if (!this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, vector2)))
            {
              this.MoveVExact(index1 * index2);
              this.MoveHExact(Math.Sign((float) speed.X));
              return false;
            }
          }
        }
        return true;
      }
      if (speed.Y == 0.0 || !this.MoveVCollideSolids((float) speed.Y, false, (Action<Vector2, Vector2, Platform>) null))
        return false;
      for (int index1 = 1; index1 <= 3; ++index1)
      {
        for (int index2 = 1; index2 >= -1; index2 -= 2)
        {
          Vector2 vector2;
          ((Vector2) ref vector2).\u002Ector((float) (index1 * index2), (float) Math.Sign((float) speed.Y));
          if (!this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, vector2)))
          {
            this.MoveHExact(index1 * index2);
            this.MoveVExact(Math.Sign((float) speed.Y));
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
      image.Position = Vector2.op_Addition(position, new Vector2(4f, 4f));
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
      this.Position = Vector2.op_Addition(this.Position, this.Shake);
      foreach (Component component in this.leftButton)
        component.Render();
      foreach (Component component in this.rightButton)
        component.Render();
      foreach (Component component in this.topButton)
        component.Render();
      Draw.Rect(this.X + 3f, this.Y + 3f, this.Width - 6f, this.Height - 6f, this.fillColor);
      foreach (Component component in this.body)
        component.Render();
      Draw.Rect((float) (this.Center.X - 4.0), (float) (this.Center.Y - 4.0), 8f, 8f, this.fillColor);
      if (this.state != MoveBlock.MovementState.Breaking)
        this.arrows[Calc.Clamp((int) Math.Floor((-(double) this.angle + 6.28318548202515) % 6.28318548202515 / 6.28318548202515 * 8.0 + 0.5), 0, 7)].DrawCentered(this.Center);
      else
        GFX.Game["objects/moveBlock/x"].DrawCentered(this.Center);
      float num = this.flash * 4f;
      Draw.Rect(this.X - num, this.Y - num, this.Width + num * 2f, this.Height + num * 2f, Color.op_Multiply(Color.get_White(), this.flash));
      this.Position = position;
    }

    private void ActivateParticles()
    {
      bool flag1 = this.direction == MoveBlock.Directions.Down || this.direction == MoveBlock.Directions.Up;
      int num = !this.canSteer || !flag1 ? (!this.CollideCheck<Player>(Vector2.op_Subtraction(this.Position, Vector2.get_UnitX())) ? 1 : 0) : 0;
      bool flag2 = (!this.canSteer || !flag1) && !this.CollideCheck<Player>(Vector2.op_Addition(this.Position, Vector2.get_UnitX()));
      bool flag3 = !this.canSteer | flag1 && !this.CollideCheck<Player>(Vector2.op_Subtraction(this.Position, Vector2.get_UnitY()));
      if (num != 0)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Height / 2.0), this.CenterLeft, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 4f), 0.5f), 3.141593f);
      if (flag2)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Height / 2.0), this.CenterRight, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 4f), 0.5f), 0.0f);
      if (flag3)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Width / 2.0), this.TopCenter, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 4f), 0.5f), -1.570796f);
      this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Width / 2.0), this.BottomCenter, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 4f), 0.5f), 1.570796f);
    }

    private void BreakParticles()
    {
      Vector2 center = this.Center;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 4)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 4)
        {
          Vector2 position = Vector2.op_Addition(this.Position, new Vector2((float) (2 + index1), (float) (2 + index2)));
          this.SceneAs<Level>().Particles.Emit(MoveBlock.P_Break, 1, position, Vector2.op_Multiply(Vector2.get_One(), 2f), Vector2.op_Subtraction(position, center).Angle());
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
        position = Vector2.op_Addition(this.CenterLeft, Vector2.get_UnitX());
        vector2 = Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 4f);
        direction = 3.141593f;
        num = this.Height / 32f;
      }
      else if (this.direction == MoveBlock.Directions.Left)
      {
        position = this.CenterRight;
        vector2 = Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 4f);
        direction = 0.0f;
        num = this.Height / 32f;
      }
      else if (this.direction == MoveBlock.Directions.Down)
      {
        position = Vector2.op_Addition(this.TopCenter, Vector2.get_UnitY());
        vector2 = Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 4f);
        direction = -1.570796f;
        num = this.Width / 32f;
      }
      else
      {
        position = this.BottomCenter;
        vector2 = Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 4f);
        direction = 1.570796f;
        num = this.Width / 32f;
      }
      this.particleRemainder += num;
      int particleRemainder = (int) this.particleRemainder;
      this.particleRemainder -= (float) particleRemainder;
      Vector2 positionRange = Vector2.op_Multiply(vector2, 0.5f);
      if (particleRemainder <= 0)
        return;
      this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Move, particleRemainder, position, positionRange, direction);
    }

    private void ScrapeParticles(Vector2 dir)
    {
      int num1 = this.Collidable ? 1 : 0;
      this.Collidable = false;
      if (dir.X != 0.0)
      {
        float num2 = dir.X <= 0.0 ? this.Left - 1f : this.Right;
        for (int index = 0; (double) index < (double) this.Height; index += 8)
        {
          Vector2 vector2;
          ((Vector2) ref vector2).\u002Ector(num2, this.Top + 4f + (float) index);
          if (this.Scene.CollideCheck<Solid>(vector2))
            this.SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector2);
        }
      }
      else
      {
        float num2 = dir.Y <= 0.0 ? this.Top - 1f : this.Bottom;
        for (int index = 0; (double) index < (double) this.Width; index += 8)
        {
          Vector2 vector2;
          ((Vector2) ref vector2).\u002Ector(this.Left + 4f + (float) index, num2);
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

      public override void Render()
      {
        Draw.Rect((float) ((double) this.Parent.X + this.Parent.Shake.X - 1.0), (float) ((double) this.Parent.Y + this.Parent.Shake.Y - 1.0), this.Parent.Width + 2f, this.Parent.Height + 2f, Color.get_Black());
      }
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
        : base(Vector2.get_Zero())
      {
        this.Tag = (int) Tags.TransitionUpdate;
        this.Collider = (Collider) new Hitbox(4f, 4f, -2f, -2f);
        this.Add((Component) (this.sprite = new Monocle.Image(Calc.Random.Choose<MTexture>(GFX.Game.GetAtlasSubtextures("objects/moveblock/debris")))));
        this.sprite.CenterOrigin();
        this.sprite.FlipX = Calc.Random.Chance(0.5f);
        this.onCollideH = (Collision) (c => this.speed.X = (__Null) (-this.speed.X * 0.5));
        this.onCollideV = (Collision) (c =>
        {
          if (this.firstHit || this.speed.Y > 50.0)
            Audio.Play("event:/game/general/debris_stone", this.Position, "debris_velocity", Calc.ClampedMap((float) this.speed.Y, 0.0f, 600f, 0.0f, 1f));
          this.speed.Y = this.speed.Y <= 0.0 || this.speed.Y >= 40.0 ? (__Null) (-this.speed.Y * 0.25) : (__Null) 0.0;
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
        this.speed = Vector2.op_Subtraction(position, center).SafeNormalize(60f + Calc.Random.NextFloat(60f));
        this.home = returnTo;
        this.sprite.Position = Vector2.get_Zero();
        this.sprite.Rotation = Calc.Random.NextAngle();
        this.returning = false;
        this.shaking = false;
        this.sprite.Scale.X = (__Null) 1.0;
        this.sprite.Scale.Y = (__Null) 1.0;
        this.sprite.Color = Color.get_White();
        this.alpha = 1f;
        this.firstHit = false;
        this.spin = Calc.Random.Range(3.490659f, 10.47198f) * (float) Calc.Random.Choose<int>(1, -1);
        return this;
      }

      public override void Update()
      {
        base.Update();
        if (!this.returning)
        {
          if (this.Collidable)
          {
            this.speed.X = (__Null) (double) Calc.Approach((float) this.speed.X, 0.0f, Engine.DeltaTime * 100f);
            if (!this.OnGround(1))
            {
              ref __Null local = ref this.speed.Y;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local + 400f * Engine.DeltaTime;
            }
            this.MoveH((float) this.speed.X * Engine.DeltaTime, this.onCollideH, (Solid) null);
            this.MoveV((float) this.speed.Y * Engine.DeltaTime, this.onCollideV, (Solid) null);
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
          this.sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + (double) this.returnEase * 0.5));
        }
        if ((this.Scene as Level).Transitioning)
        {
          this.alpha = Calc.Approach(this.alpha, 0.0f, Engine.DeltaTime * 4f);
          this.sprite.Color = Color.op_Multiply(Color.get_White(), this.alpha);
        }
        this.sprite.Rotation += this.spin * Calc.ClampedMap(Math.Abs((float) this.speed.Y), 50f, 150f, 0.0f, 1f) * Engine.DeltaTime;
      }

      public void StopMoving()
      {
        this.Collidable = false;
      }

      public void StartShaking()
      {
        this.shaking = true;
      }

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
        Vector2 vector2 = Vector2.op_Subtraction(this.home, this.Position).SafeNormalize();
        this.returnCurve = new SimpleCurve(this.Position, this.home, Vector2.op_Addition(Vector2.op_Division(Vector2.op_Addition(this.Position, this.home), 2f), Vector2.op_Multiply(Vector2.op_Multiply(new Vector2((float) vector2.Y, (float) -vector2.X), Calc.Random.NextFloat(16f) + 16f), (float) Calc.Random.Facing())));
      }
    }
  }
}
