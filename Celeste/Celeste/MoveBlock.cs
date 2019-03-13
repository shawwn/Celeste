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
    private MoveBlock.MovementState state = MoveBlock.MovementState.Idling;
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
          this.AddImage(mtexture1.GetSubtexture(num3 * 8, num4 * 8, 8, 8, (MTexture) null), new Vector2((float) index1, (float) index2) * 8f, 0.0f, new Vector2(1f, 1f), this.body);
        }
      }
      this.arrows = GFX.Game.GetAtlasSubtextures("objects/moveBlock/arrow");
      this.Add((Component) (this.moveSfx = new SoundSource()));
      this.Add((Component) new Coroutine(this.Controller(), true));
      this.UpdateColors();
      this.Add((Component) new LightOcclude(0.5f));
    }

    public MoveBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Height, data.Enum<MoveBlock.Directions>(nameof (direction), MoveBlock.Directions.Left), data.Bool(nameof (canSteer), true), data.Bool(nameof (fast), false))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      scene.Add((Entity) (this.border = new MoveBlock.Border(this)));
    }

    private IEnumerator Controller()
    {
      while (true)
      {
        this.triggered = false;
        this.state = MoveBlock.MovementState.Idling;
        while (!this.triggered && !this.HasPlayerRider())
          yield return (object) null;
        Audio.Play("event:/game/04_cliffside/arrowblock_activate", this.Position);
        this.state = MoveBlock.MovementState.Moving;
        this.StartShaking(0.2f);
        this.ActivateParticles();
        yield return (object) 0.2f;
        this.targetSpeed = this.fast ? 75f : 60f;
        this.moveSfx.Play("event:/game/04_cliffside/arrowblock_move", (string) null, 0.0f);
        this.moveSfx.Param("arrow_stop", 0.0f);
        this.StopPlayerRunIntoAnimation = false;
        float crashTimer = 0.15f;
        float crashResetTimer = 0.1f;
        float noSteerTimer = 0.2f;
        while (true)
        {
          if (this.canSteer)
          {
            this.targetAngle = this.homeAngle;
            bool hasPlayer = this.direction != MoveBlock.Directions.Right && this.direction != MoveBlock.Directions.Left ? this.HasPlayerClimbing() : this.HasPlayerOnTop();
            if (hasPlayer && (double) noSteerTimer > 0.0)
              noSteerTimer -= Engine.DeltaTime;
            if (hasPlayer)
            {
              if ((double) noSteerTimer <= 0.0)
                this.targetAngle = this.direction != MoveBlock.Directions.Right && this.direction != MoveBlock.Directions.Left ? this.homeAngle + 0.7853982f * (float) this.angleSteerSign * (float) Input.MoveX.Value : this.homeAngle + 0.7853982f * (float) this.angleSteerSign * (float) Input.MoveY.Value;
            }
            else
              noSteerTimer = 0.2f;
          }
          if (this.Scene.OnInterval(0.02f))
            this.MoveParticles();
          this.speed = Calc.Approach(this.speed, this.targetSpeed, 300f * Engine.DeltaTime);
          this.angle = Calc.Approach(this.angle, this.targetAngle, 50.26548f * Engine.DeltaTime);
          Vector2 move = Calc.AngleToVector(this.angle, this.speed) * Engine.DeltaTime;
          bool hit = false;
          Rectangle bounds;
          if (this.direction == MoveBlock.Directions.Right || this.direction == MoveBlock.Directions.Left)
          {
            hit = this.MoveCheck(move.XComp());
            this.noSquish = this.Scene.Tracker.GetEntity<Player>();
            this.MoveVCollideSolids(move.Y, false, (Action<Vector2, Vector2, Platform>) null);
            this.noSquish = (Player) null;
            if (this.Scene.OnInterval(0.03f))
            {
              if ((double) move.Y > 0.0)
                this.ScrapeParticles(Vector2.UnitY);
              else if ((double) move.Y < 0.0)
                this.ScrapeParticles(-Vector2.UnitY);
            }
          }
          else
          {
            hit = this.MoveCheck(move.YComp());
            this.noSquish = this.Scene.Tracker.GetEntity<Player>();
            this.MoveHCollideSolids(move.X, false, (Action<Vector2, Vector2, Platform>) null);
            this.noSquish = (Player) null;
            if (this.Scene.OnInterval(0.03f))
            {
              if ((double) move.X > 0.0)
                this.ScrapeParticles(Vector2.UnitX);
              else if ((double) move.X < 0.0)
                this.ScrapeParticles(-Vector2.UnitX);
            }
            int num1;
            if (this.direction == MoveBlock.Directions.Down)
            {
              double top = (double) this.Top;
              bounds = this.SceneAs<Level>().Bounds;
              double num2 = (double) (bounds.Bottom + 32);
              num1 = top > num2 ? 1 : 0;
            }
            else
              num1 = 0;
            if (num1 != 0)
              hit = true;
          }
          if (hit)
          {
            this.moveSfx.Param("arrow_stop", 1f);
            crashResetTimer = 0.1f;
            if ((double) crashTimer > 0.0)
              crashTimer -= Engine.DeltaTime;
            else
              break;
          }
          else
          {
            this.moveSfx.Param("arrow_stop", 0.0f);
            if ((double) crashResetTimer > 0.0)
              crashResetTimer -= Engine.DeltaTime;
            else
              crashTimer = 0.15f;
          }
          Level level = this.Scene as Level;
          double left1 = (double) this.Left;
          bounds = level.Bounds;
          double left2 = (double) bounds.Left;
          int num;
          if (left1 >= left2)
          {
            double top1 = (double) this.Top;
            bounds = level.Bounds;
            double top2 = (double) bounds.Top;
            if (top1 >= top2)
            {
              double right1 = (double) this.Right;
              bounds = level.Bounds;
              double right2 = (double) bounds.Right;
              num = right1 > right2 ? 1 : 0;
              goto label_43;
            }
          }
          num = 1;
label_43:
          if (num == 0)
          {
            move = new Vector2();
            level = (Level) null;
            yield return (object) null;
          }
          else
            break;
        }
        Audio.Play("event:/game/04_cliffside/arrowblock_break", this.Position);
        this.moveSfx.Stop(true);
        this.state = MoveBlock.MovementState.Breaking;
        this.speed = this.targetSpeed = 0.0f;
        this.angle = this.targetAngle = this.homeAngle;
        this.StartShaking(0.2f);
        this.StopPlayerRunIntoAnimation = true;
        yield return (object) 0.2f;
        this.BreakParticles();
        List<MoveBlock.Debris> debris = new List<MoveBlock.Debris>();
        for (int x = 0; (double) x < (double) this.Width; x += 8)
        {
          for (int y = 0; (double) y < (double) this.Height; y += 8)
          {
            Vector2 offset = new Vector2((float) x + 4f, (float) y + 4f);
            MoveBlock.Debris d = Engine.Pooler.Create<MoveBlock.Debris>().Init(this.Position + offset, this.Center, this.startPosition + offset);
            debris.Add(d);
            this.Scene.Add((Entity) d);
            offset = new Vector2();
            d = (MoveBlock.Debris) null;
          }
        }
        this.MoveStaticMovers(this.startPosition - this.Position);
        this.DisableStaticMovers();
        this.Position = this.startPosition;
        this.Visible = this.Collidable = false;
        yield return (object) 2.2f;
        foreach (MoveBlock.Debris debris1 in debris)
        {
          MoveBlock.Debris d = debris1;
          d.StopMoving();
          d = (MoveBlock.Debris) null;
        }
        while (this.CollideCheck<Actor>() || this.CollideCheck<Solid>())
          yield return (object) null;
        this.Collidable = true;
        EventInstance sound = Audio.Play("event:/game/04_cliffside/arrowblock_reform_begin", debris[0].Position);
        Coroutine routine;
        this.Add((Component) (routine = new Coroutine(this.SoundFollowsDebrisCenter(sound, debris), true)));
        foreach (MoveBlock.Debris debris1 in debris)
        {
          MoveBlock.Debris d = debris1;
          d.StartShaking();
          d = (MoveBlock.Debris) null;
        }
        yield return (object) 0.2f;
        foreach (MoveBlock.Debris debris1 in debris)
        {
          MoveBlock.Debris d = debris1;
          d.ReturnHome(0.65f);
          d = (MoveBlock.Debris) null;
        }
        yield return (object) 0.6f;
        routine.RemoveSelf();
        foreach (MoveBlock.Debris debris1 in debris)
        {
          MoveBlock.Debris d = debris1;
          d.RemoveSelf();
          d = (MoveBlock.Debris) null;
        }
        sound = (EventInstance) null;
        routine = (Coroutine) null;
        Audio.Play("event:/game/04_cliffside/arrowblock_reappear", this.Position);
        this.Visible = true;
        this.EnableStaticMovers();
        this.speed = this.targetSpeed = 0.0f;
        this.angle = this.targetAngle = this.homeAngle;
        this.noSquish = (Player) null;
        this.fillColor = MoveBlock.idleBgFill;
        this.UpdateColors();
        this.flash = 1f;
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
          Vector2 center = Vector2.Zero;
          foreach (MoveBlock.Debris debri in debris)
          {
            MoveBlock.Debris d = debri;
            center += d.Position;
            d = (MoveBlock.Debris) null;
          }
          center /= (float) debris.Count;
          Audio.Position(instance, center);
          yield return (object) null;
          center = new Vector2();
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
        this.moveSfx.Param("arrow_influence", (float) ((int) Math.Floor((-(double) (Calc.AngleToVector(this.angle, 1f) * new Vector2(-1f, 1f)).Angle() + 6.28318548202515) % 6.28318548202515 / 6.28318548202515 * 8.0 + 0.5) + 1));
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
        while (move != 0 && this.noSquish.CollideCheck<Solid>(this.noSquish.Position + Vector2.UnitX * (float) move))
          move -= Math.Sign(move);
      }
      base.MoveHExact(move);
    }

    public override void MoveVExact(int move)
    {
      if (this.noSquish != null && (move < 0 && (double) this.noSquish.Y <= (double) this.Y))
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
        if (!this.MoveHCollideSolids(speed.X, false, (Action<Vector2, Vector2, Platform>) null))
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
      if ((double) speed.Y == 0.0 || !this.MoveVCollideSolids(speed.Y, false, (Action<Vector2, Vector2, Platform>) null))
        return false;
      for (int index1 = 1; index1 <= 3; ++index1)
      {
        for (int index2 = 1; index2 >= -1; index2 -= 2)
        {
          if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) (index1 * index2), (float) Math.Sign(speed.Y))))
          {
            this.MoveHExact(index1 * index2);
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
      if (addTo == null)
        return;
      addTo.Add(image);
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
        this.arrows[Calc.Clamp((int) Math.Floor((-(double) this.angle + 6.28318548202515) % 6.28318548202515 / 6.28318548202515 * 8.0 + 0.5), 0, 7)].DrawCentered(this.Center);
      else
        GFX.Game["objects/moveBlock/x"].DrawCentered(this.Center);
      float num = this.flash * 4f;
      Draw.Rect(this.X - num, this.Y - num, this.Width + num * 2f, this.Height + num * 2f, Color.White * this.flash);
      this.Position = position;
    }

    private void ActivateParticles()
    {
      bool flag1 = this.direction == MoveBlock.Directions.Down || this.direction == MoveBlock.Directions.Up;
      bool flag2 = (!this.canSteer || !flag1) && !this.CollideCheck<Player>(this.Position - Vector2.UnitX);
      bool flag3 = (!this.canSteer || !flag1) && !this.CollideCheck<Player>(this.Position + Vector2.UnitX);
      bool flag4 = !this.canSteer | flag1 && !this.CollideCheck<Player>(this.Position - Vector2.UnitY);
      if (flag2)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Height / 2.0), this.CenterLeft, Vector2.UnitY * (this.Height - 4f) * 0.5f, 3.141593f);
      if (flag3)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Height / 2.0), this.CenterRight, Vector2.UnitY * (this.Height - 4f) * 0.5f, 0.0f);
      if (flag4)
        this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Width / 2.0), this.TopCenter, Vector2.UnitX * (this.Width - 4f) * 0.5f, -1.570796f);
      this.SceneAs<Level>().ParticlesBG.Emit(MoveBlock.P_Activate, (int) ((double) this.Width / 2.0), this.BottomCenter, Vector2.UnitX * (this.Width - 4f) * 0.5f, 1.570796f);
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
        direction = 3.141593f;
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
        direction = -1.570796f;
        num = this.Width / 32f;
      }
      else
      {
        position = this.BottomCenter;
        vector2 = Vector2.UnitX * (this.Width - 4f);
        direction = 1.570796f;
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
      bool collidable = this.Collidable;
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

      public override void Render()
      {
        Draw.Rect((float) ((double) this.Parent.X + (double) this.Parent.Shake.X - 1.0), (float) ((double) this.Parent.Y + (double) this.Parent.Shake.Y - 1.0), this.Parent.Width + 2f, this.Parent.Height + 2f, Color.Black);
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
            Audio.Play("event:/game/general/debris_stone", this.Position, "debris_velocity", Calc.ClampedMap(this.speed.Y, 0.0f, 600f, 0.0f, 1f));
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
            this.speed.X = Calc.Approach(this.speed.X, 0.0f, Engine.DeltaTime * 100f);
            if (!this.OnGround(1))
              this.speed.Y += 400f * Engine.DeltaTime;
            this.MoveH(this.speed.X * Engine.DeltaTime, this.onCollideH, (Solid) null);
            this.MoveV(this.speed.Y * Engine.DeltaTime, this.onCollideV, (Solid) null);
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
        this.sprite.Rotation += this.spin * Calc.ClampedMap(Math.Abs(this.speed.Y), 50f, 150f, 0.0f, 1f) * Engine.DeltaTime;
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
        Vector2 vector2 = (this.home - this.Position).SafeNormalize();
        this.returnCurve = new SimpleCurve(this.Position, this.home, (this.Position + this.home) / 2f + new Vector2(vector2.Y, -vector2.X) * (Calc.Random.NextFloat(16f) + 16f) * (float) Calc.Random.Facing());
      }
    }
  }
}

