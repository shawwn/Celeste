// Decompiled with JetBrains decompiler
// Type: Celeste.CrushBlock
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
  public class CrushBlock : Solid
  {
    private Color fill = Calc.HexToColor("62222b");
    private List<Monocle.Image> idleImages = new List<Monocle.Image>();
    private List<Monocle.Image> activeTopImages = new List<Monocle.Image>();
    private List<Monocle.Image> activeRightImages = new List<Monocle.Image>();
    private List<Monocle.Image> activeLeftImages = new List<Monocle.Image>();
    private List<Monocle.Image> activeBottomImages = new List<Monocle.Image>();
    public static ParticleType P_Impact;
    public static ParticleType P_Crushing;
    public static ParticleType P_Activate;
    private const float CrushSpeed = 240f;
    private const float CrushAccel = 500f;
    private const float ReturnSpeed = 60f;
    private const float ReturnAccel = 160f;
    private Level level;
    private bool canActivate;
    private Vector2 crushDir;
    private List<CrushBlock.MoveState> returnStack;
    private Coroutine attackCoroutine;
    private bool canMoveVertically;
    private bool canMoveHorizontally;
    private bool chillOut;
    private bool giant;
    private Sprite face;
    private string nextFaceDirection;
    private SoundSource currentMoveLoopSfx;
    private SoundSource returnLoopSfx;

    public CrushBlock(
      Vector2 position,
      float width,
      float height,
      CrushBlock.Axes axes,
      bool chillOut = false)
      : base(position, width, height, false)
    {
      this.OnDashCollide = new DashCollision(this.OnDashed);
      this.returnStack = new List<CrushBlock.MoveState>();
      this.chillOut = chillOut;
      this.giant = (((double) this.Width < 48.0 ? 0 : ((double) this.Height >= 48.0 ? 1 : 0)) & (chillOut ? 1 : 0)) != 0;
      this.canActivate = true;
      this.attackCoroutine = new Coroutine(true);
      this.attackCoroutine.RemoveOnComplete = false;
      this.Add((Component) this.attackCoroutine);
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/crushblock/block");
      MTexture idle;
      switch (axes)
      {
        case CrushBlock.Axes.Horizontal:
          idle = atlasSubtextures[1];
          this.canMoveHorizontally = true;
          this.canMoveVertically = false;
          break;
        case CrushBlock.Axes.Vertical:
          idle = atlasSubtextures[2];
          this.canMoveHorizontally = false;
          this.canMoveVertically = true;
          break;
        default:
          idle = atlasSubtextures[3];
          this.canMoveHorizontally = this.canMoveVertically = true;
          break;
      }
      this.Add((Component) (this.face = GFX.SpriteBank.Create(this.giant ? "giant_crushblock_face" : "crushblock_face")));
      this.face.Position = Vector2.op_Division(new Vector2(this.Width, this.Height), 2f);
      this.face.Play("idle", false, false);
      this.face.OnLastFrame = (Action<string>) (f =>
      {
        if (!(f == "hit"))
          return;
        this.face.Play(this.nextFaceDirection, false, false);
      });
      int x1 = (int) ((double) this.Width / 8.0) - 1;
      int y1 = (int) ((double) this.Height / 8.0) - 1;
      this.AddImage(idle, 0, 0, 0, 0, -1, -1);
      this.AddImage(idle, x1, 0, 3, 0, 1, -1);
      this.AddImage(idle, 0, y1, 0, 3, -1, 1);
      this.AddImage(idle, x1, y1, 3, 3, 1, 1);
      for (int x2 = 1; x2 < x1; ++x2)
      {
        this.AddImage(idle, x2, 0, Calc.Random.Choose<int>(1, 2), 0, 0, -1);
        this.AddImage(idle, x2, y1, Calc.Random.Choose<int>(1, 2), 3, 0, 1);
      }
      for (int y2 = 1; y2 < y1; ++y2)
      {
        this.AddImage(idle, 0, y2, 0, Calc.Random.Choose<int>(1, 2), -1, 0);
        this.AddImage(idle, x1, y2, 3, Calc.Random.Choose<int>(1, 2), 1, 0);
      }
      this.Add((Component) new LightOcclude(0.2f));
      this.Add((Component) (this.returnLoopSfx = new SoundSource()));
      this.Add((Component) new WaterInteraction((Func<bool>) (() => Vector2.op_Inequality(this.crushDir, Vector2.get_Zero()))));
    }

    public CrushBlock(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), (float) data.Width, (float) data.Height, data.Enum<CrushBlock.Axes>("axes", CrushBlock.Axes.Both), data.Bool("chillout", false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
    }

    public override void Update()
    {
      base.Update();
      if (Vector2.op_Equality(this.crushDir, Vector2.get_Zero()))
      {
        this.face.Position = Vector2.op_Division(new Vector2(this.Width, this.Height), 2f);
        if (this.CollideCheck<Player>(Vector2.op_Addition(this.Position, new Vector2(-1f, 0.0f))))
          --this.face.X;
        else if (this.CollideCheck<Player>(Vector2.op_Addition(this.Position, new Vector2(1f, 0.0f))))
          ++this.face.X;
        else if (this.CollideCheck<Player>(Vector2.op_Addition(this.Position, new Vector2(0.0f, -1f))))
          --this.face.Y;
      }
      if (this.currentMoveLoopSfx != null)
        this.currentMoveLoopSfx.Param("submerged", this.Submerged ? 1f : 0.0f);
      if (this.returnLoopSfx == null)
        return;
      this.returnLoopSfx.Param("submerged", this.Submerged ? 1f : 0.0f);
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = Vector2.op_Addition(this.Position, this.Shake);
      Draw.Rect(this.X + 2f, this.Y + 2f, this.Width - 4f, this.Height - 4f, this.fill);
      base.Render();
      this.Position = position;
    }

    private bool Submerged
    {
      get
      {
        return this.Scene.CollideCheck<Water>(new Rectangle((int) (this.Center.X - 4.0), (int) this.Center.Y, 8, 4));
      }
    }

    private void AddImage(
      MTexture idle,
      int x,
      int y,
      int tx,
      int ty,
      int borderX = 0,
      int borderY = 0)
    {
      MTexture subtexture = idle.GetSubtexture(tx * 8, ty * 8, 8, 8, (MTexture) null);
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector((float) (x * 8), (float) (y * 8));
      if (borderX != 0)
      {
        Monocle.Image image = new Monocle.Image(subtexture);
        image.Color = Color.get_Black();
        image.Position = Vector2.op_Addition(vector2, new Vector2((float) borderX, 0.0f));
        this.Add((Component) image);
      }
      if (borderY != 0)
      {
        Monocle.Image image = new Monocle.Image(subtexture);
        image.Color = Color.get_Black();
        image.Position = Vector2.op_Addition(vector2, new Vector2(0.0f, (float) borderY));
        this.Add((Component) image);
      }
      Monocle.Image image1 = new Monocle.Image(subtexture);
      image1.Position = vector2;
      this.Add((Component) image1);
      this.idleImages.Add(image1);
      if (borderX == 0 && borderY == 0)
        return;
      if (borderX < 0)
      {
        Monocle.Image image2 = new Monocle.Image(GFX.Game["objects/crushblock/lit_left"].GetSubtexture(0, ty * 8, 8, 8, (MTexture) null));
        this.activeLeftImages.Add(image2);
        image2.Position = vector2;
        image2.Visible = false;
        this.Add((Component) image2);
      }
      else if (borderX > 0)
      {
        Monocle.Image image2 = new Monocle.Image(GFX.Game["objects/crushblock/lit_right"].GetSubtexture(0, ty * 8, 8, 8, (MTexture) null));
        this.activeRightImages.Add(image2);
        image2.Position = vector2;
        image2.Visible = false;
        this.Add((Component) image2);
      }
      if (borderY < 0)
      {
        Monocle.Image image2 = new Monocle.Image(GFX.Game["objects/crushblock/lit_top"].GetSubtexture(tx * 8, 0, 8, 8, (MTexture) null));
        this.activeTopImages.Add(image2);
        image2.Position = vector2;
        image2.Visible = false;
        this.Add((Component) image2);
      }
      else
      {
        if (borderY <= 0)
          return;
        Monocle.Image image2 = new Monocle.Image(GFX.Game["objects/crushblock/lit_bottom"].GetSubtexture(tx * 8, 0, 8, 8, (MTexture) null));
        this.activeBottomImages.Add(image2);
        image2.Position = vector2;
        image2.Visible = false;
        this.Add((Component) image2);
      }
    }

    private void TurnOffImages()
    {
      foreach (Component activeLeftImage in this.activeLeftImages)
        activeLeftImage.Visible = false;
      foreach (Component activeRightImage in this.activeRightImages)
        activeRightImage.Visible = false;
      foreach (Component activeTopImage in this.activeTopImages)
        activeTopImage.Visible = false;
      foreach (Component activeBottomImage in this.activeBottomImages)
        activeBottomImage.Visible = false;
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
      if (!this.CanActivate(Vector2.op_UnaryNegation(direction)))
        return DashCollisionResults.NormalCollision;
      this.Attack(Vector2.op_UnaryNegation(direction));
      return DashCollisionResults.Rebound;
    }

    private bool CanActivate(Vector2 direction)
    {
      return (!this.giant || direction.X > 0.0) && (this.canActivate && Vector2.op_Inequality(this.crushDir, direction)) && ((direction.X == 0.0 || this.canMoveHorizontally) && (direction.Y == 0.0 || this.canMoveVertically));
    }

    private void Attack(Vector2 direction)
    {
      Audio.Play("event:/game/06_reflection/crushblock_activate", this.Center);
      if (this.currentMoveLoopSfx != null)
      {
        this.currentMoveLoopSfx.Param("end", 1f);
        SoundSource sfx = this.currentMoveLoopSfx;
        Alarm.Set((Entity) this, 0.5f, (Action) (() => sfx.RemoveSelf()), Alarm.AlarmMode.Oneshot);
      }
      this.Add((Component) (this.currentMoveLoopSfx = new SoundSource()));
      this.currentMoveLoopSfx.Position = Vector2.op_Division(new Vector2(this.Width, this.Height), 2f);
      this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop", (string) null, 0.0f);
      this.face.Play("hit", false, false);
      this.crushDir = direction;
      this.canActivate = false;
      this.attackCoroutine.Replace(this.AttackSequence());
      this.ClearRemainder();
      this.TurnOffImages();
      this.ActivateParticles(this.crushDir);
      if (this.crushDir.X < 0.0)
      {
        foreach (Component activeLeftImage in this.activeLeftImages)
          activeLeftImage.Visible = true;
        this.nextFaceDirection = "left";
      }
      else if (this.crushDir.X > 0.0)
      {
        foreach (Component activeRightImage in this.activeRightImages)
          activeRightImage.Visible = true;
        this.nextFaceDirection = "right";
      }
      else if (this.crushDir.Y < 0.0)
      {
        foreach (Component activeTopImage in this.activeTopImages)
          activeTopImage.Visible = true;
        this.nextFaceDirection = "up";
      }
      else if (this.crushDir.Y > 0.0)
      {
        foreach (Component activeBottomImage in this.activeBottomImages)
          activeBottomImage.Visible = true;
        this.nextFaceDirection = "down";
      }
      bool flag = true;
      if (this.returnStack.Count > 0)
      {
        CrushBlock.MoveState moveState = this.returnStack[this.returnStack.Count - 1];
        if (Vector2.op_Equality(moveState.Direction, direction) || Vector2.op_Equality(moveState.Direction, Vector2.op_UnaryNegation(direction)))
          flag = false;
      }
      if (!flag)
        return;
      this.returnStack.Add(new CrushBlock.MoveState(this.Position, this.crushDir));
    }

    private void ActivateParticles(Vector2 dir)
    {
      float direction;
      Vector2 position;
      Vector2 positionRange;
      int num;
      if (Vector2.op_Equality(dir, Vector2.get_UnitX()))
      {
        direction = 0.0f;
        position = Vector2.op_Subtraction(this.CenterRight, Vector2.get_UnitX());
        positionRange = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 2f), 0.5f);
        num = (int) ((double) this.Height / 8.0) * 4;
      }
      else if (Vector2.op_Equality(dir, Vector2.op_UnaryNegation(Vector2.get_UnitX())))
      {
        direction = 3.141593f;
        position = Vector2.op_Addition(this.CenterLeft, Vector2.get_UnitX());
        positionRange = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitY(), this.Height - 2f), 0.5f);
        num = (int) ((double) this.Height / 8.0) * 4;
      }
      else if (Vector2.op_Equality(dir, Vector2.get_UnitY()))
      {
        direction = 1.570796f;
        position = Vector2.op_Subtraction(this.BottomCenter, Vector2.get_UnitY());
        positionRange = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 2f), 0.5f);
        num = (int) ((double) this.Width / 8.0) * 4;
      }
      else
      {
        direction = -1.570796f;
        position = Vector2.op_Addition(this.TopCenter, Vector2.get_UnitY());
        positionRange = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), this.Width - 2f), 0.5f);
        num = (int) ((double) this.Width / 8.0) * 4;
      }
      int amount = num + 2;
      this.level.Particles.Emit(CrushBlock.P_Activate, amount, position, positionRange, direction);
    }

    private IEnumerator AttackSequence()
    {
      CrushBlock crushBlock = this;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      crushBlock.StartShaking(0.4f);
      yield return (object) 0.4f;
      if (!crushBlock.chillOut)
        crushBlock.canActivate = true;
      crushBlock.StopPlayerRunIntoAnimation = false;
      bool slowing = false;
      float speed = 0.0f;
      while (true)
      {
        if (!crushBlock.chillOut)
          speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
        else if (slowing || crushBlock.CollideCheck<SolidTiles>(Vector2.op_Addition(crushBlock.Position, Vector2.op_Multiply(crushBlock.crushDir, 256f))))
        {
          speed = Calc.Approach(speed, 24f, (float) (500.0 * (double) Engine.DeltaTime * 0.25));
          if (!slowing)
          {
            slowing = true;
            Alarm.Set((Entity) crushBlock, 0.5f, (Action) (() =>
            {
              this.face.Play("hurt", false, false);
              this.currentMoveLoopSfx.Stop(true);
              this.TurnOffImages();
            }), Alarm.AlarmMode.Oneshot);
          }
        }
        else
          speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
        if (!(crushBlock.crushDir.X == 0.0 ? crushBlock.MoveVCheck(speed * (float) crushBlock.crushDir.Y * Engine.DeltaTime) : crushBlock.MoveHCheck(speed * (float) crushBlock.crushDir.X * Engine.DeltaTime)))
        {
          if (crushBlock.Scene.OnInterval(0.02f))
          {
            Vector2 position;
            float direction;
            if (Vector2.op_Equality(crushBlock.crushDir, Vector2.get_UnitX()))
            {
              ((Vector2) ref position).\u002Ector(crushBlock.Left + 1f, Calc.Random.Range(crushBlock.Top + 3f, crushBlock.Bottom - 3f));
              direction = 3.141593f;
            }
            else if (Vector2.op_Equality(crushBlock.crushDir, Vector2.op_UnaryNegation(Vector2.get_UnitX())))
            {
              ((Vector2) ref position).\u002Ector(crushBlock.Right - 1f, Calc.Random.Range(crushBlock.Top + 3f, crushBlock.Bottom - 3f));
              direction = 0.0f;
            }
            else if (Vector2.op_Equality(crushBlock.crushDir, Vector2.get_UnitY()))
            {
              ((Vector2) ref position).\u002Ector(Calc.Random.Range(crushBlock.Left + 3f, crushBlock.Right - 3f), crushBlock.Top + 1f);
              direction = -1.570796f;
            }
            else
            {
              ((Vector2) ref position).\u002Ector(Calc.Random.Range(crushBlock.Left + 3f, crushBlock.Right - 3f), crushBlock.Bottom - 1f);
              direction = 1.570796f;
            }
            crushBlock.level.Particles.Emit(CrushBlock.P_Crushing, position, direction);
          }
          yield return (object) null;
        }
        else
          break;
      }
      FallingBlock fallingBlock = crushBlock.CollideFirst<FallingBlock>(Vector2.op_Addition(crushBlock.Position, crushBlock.crushDir));
      if (fallingBlock != null)
        fallingBlock.Triggered = true;
      if (Vector2.op_Equality(crushBlock.crushDir, Vector2.op_UnaryNegation(Vector2.get_UnitX())))
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(0.0f, 2f);
        for (int index = 0; (double) index < (double) crushBlock.Height / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(crushBlock.Left - 1f, crushBlock.Top + 4f + (float) (index * 8));
          if (!crushBlock.Scene.CollideCheck<Water>(point) && crushBlock.Scene.CollideCheck<Solid>(point))
          {
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), 0.0f);
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), 0.0f);
          }
        }
      }
      else if (Vector2.op_Equality(crushBlock.crushDir, Vector2.get_UnitX()))
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(0.0f, 2f);
        for (int index = 0; (double) index < (double) crushBlock.Height / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(crushBlock.Right + 1f, crushBlock.Top + 4f + (float) (index * 8));
          if (!crushBlock.Scene.CollideCheck<Water>(point) && crushBlock.Scene.CollideCheck<Solid>(point))
          {
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), 3.141593f);
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), 3.141593f);
          }
        }
      }
      else if (Vector2.op_Equality(crushBlock.crushDir, Vector2.op_UnaryNegation(Vector2.get_UnitY())))
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(2f, 0.0f);
        for (int index = 0; (double) index < (double) crushBlock.Width / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(crushBlock.Left + 4f + (float) (index * 8), crushBlock.Top - 1f);
          if (!crushBlock.Scene.CollideCheck<Water>(point) && crushBlock.Scene.CollideCheck<Solid>(point))
          {
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), 1.570796f);
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), 1.570796f);
          }
        }
      }
      else if (Vector2.op_Equality(crushBlock.crushDir, Vector2.get_UnitY()))
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(2f, 0.0f);
        for (int index = 0; (double) index < (double) crushBlock.Width / 8.0; ++index)
        {
          Vector2 point;
          ((Vector2) ref point).\u002Ector(crushBlock.Left + 4f + (float) (index * 8), crushBlock.Bottom + 1f);
          if (!crushBlock.Scene.CollideCheck<Water>(point) && crushBlock.Scene.CollideCheck<Solid>(point))
          {
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Addition(point, vector2), -1.570796f);
            crushBlock.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, Vector2.op_Subtraction(point, vector2), -1.570796f);
          }
        }
      }
      Audio.Play("event:/game/06_reflection/crushblock_impact", crushBlock.Center);
      crushBlock.level.DirectionalShake(crushBlock.crushDir, 0.3f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      crushBlock.StartShaking(0.4f);
      crushBlock.StopPlayerRunIntoAnimation = true;
      SoundSource sfx = crushBlock.currentMoveLoopSfx;
      crushBlock.currentMoveLoopSfx.Param("end", 1f);
      crushBlock.currentMoveLoopSfx = (SoundSource) null;
      Alarm.Set((Entity) crushBlock, 0.5f, (Action) (() => sfx.RemoveSelf()), Alarm.AlarmMode.Oneshot);
      crushBlock.crushDir = Vector2.get_Zero();
      crushBlock.TurnOffImages();
      if (!crushBlock.chillOut)
      {
        crushBlock.face.Play("hurt", false, false);
        crushBlock.returnLoopSfx.Play("event:/game/06_reflection/crushblock_return_loop", (string) null, 0.0f);
        yield return (object) 0.4f;
        speed = 0.0f;
        float waypointSfxDelay = 0.0f;
        while (crushBlock.returnStack.Count > 0)
        {
          yield return (object) null;
          crushBlock.StopPlayerRunIntoAnimation = false;
          CrushBlock.MoveState moveState = crushBlock.returnStack[crushBlock.returnStack.Count - 1];
          speed = Calc.Approach(speed, 60f, 160f * Engine.DeltaTime);
          waypointSfxDelay -= Engine.DeltaTime;
          if (moveState.Direction.X != 0.0)
            crushBlock.MoveTowardsX((float) moveState.From.X, speed * Engine.DeltaTime);
          if (moveState.Direction.Y != 0.0)
            crushBlock.MoveTowardsY((float) moveState.From.Y, speed * Engine.DeltaTime);
          if ((moveState.Direction.X == 0.0 || crushBlock.ExactPosition.X == moveState.From.X ? (moveState.Direction.Y == 0.0 ? 1 : (crushBlock.ExactPosition.Y == moveState.From.Y ? 1 : 0)) : 0) != 0)
          {
            speed = 0.0f;
            crushBlock.returnStack.RemoveAt(crushBlock.returnStack.Count - 1);
            crushBlock.StopPlayerRunIntoAnimation = true;
            if (crushBlock.returnStack.Count <= 0)
            {
              crushBlock.face.Play("idle", false, false);
              crushBlock.returnLoopSfx.Stop(true);
              if ((double) waypointSfxDelay <= 0.0)
                Audio.Play("event:/game/06_reflection/crushblock_rest", crushBlock.Center);
            }
            else if ((double) waypointSfxDelay <= 0.0)
              Audio.Play("event:/game/06_reflection/crushblock_rest_waypoint", crushBlock.Center);
            waypointSfxDelay = 0.1f;
            crushBlock.StartShaking(0.2f);
            yield return (object) 0.2f;
          }
        }
      }
    }

    private bool MoveHCheck(float amount)
    {
      if (!this.MoveHCollideSolidsAndBounds(this.level, amount, true, (Action<Vector2, Vector2, Platform>) null))
        return false;
      if ((double) amount < 0.0)
      {
        double left1 = (double) this.Left;
        Rectangle bounds = this.level.Bounds;
        double left2 = (double) ((Rectangle) ref bounds).get_Left();
        if (left1 <= left2)
          return true;
      }
      if ((double) amount > 0.0)
      {
        double right1 = (double) this.Right;
        Rectangle bounds = this.level.Bounds;
        double right2 = (double) ((Rectangle) ref bounds).get_Right();
        if (right1 >= right2)
          return true;
      }
      for (int index1 = 1; index1 <= 4; ++index1)
      {
        for (int index2 = 1; index2 >= -1; index2 -= 2)
        {
          Vector2 vector2;
          ((Vector2) ref vector2).\u002Ector((float) Math.Sign(amount), (float) (index1 * index2));
          if (!this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, vector2)))
          {
            this.MoveVExact(index1 * index2);
            this.MoveHExact(Math.Sign(amount));
            return false;
          }
        }
      }
      return true;
    }

    private bool MoveVCheck(float amount)
    {
      if (!this.MoveVCollideSolidsAndBounds(this.level, amount, true, (Action<Vector2, Vector2, Platform>) null))
        return false;
      if ((double) amount < 0.0)
      {
        double top1 = (double) this.Top;
        Rectangle bounds = this.level.Bounds;
        double top2 = (double) ((Rectangle) ref bounds).get_Top();
        if (top1 <= top2)
          return true;
      }
      if ((double) amount > 0.0)
      {
        double bottom = (double) this.Bottom;
        Rectangle bounds = this.level.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Bottom() + 32);
        if (bottom >= num)
          return true;
      }
      for (int index1 = 1; index1 <= 4; ++index1)
      {
        for (int index2 = 1; index2 >= -1; index2 -= 2)
        {
          Vector2 vector2;
          ((Vector2) ref vector2).\u002Ector((float) (index1 * index2), (float) Math.Sign(amount));
          if (!this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, vector2)))
          {
            this.MoveHExact(index1 * index2);
            this.MoveVExact(Math.Sign(amount));
            return false;
          }
        }
      }
      return true;
    }

    public enum Axes
    {
      Both,
      Horizontal,
      Vertical,
    }

    private struct MoveState
    {
      public Vector2 From;
      public Vector2 Direction;

      public MoveState(Vector2 from, Vector2 direction)
      {
        this.From = from;
        this.Direction = direction;
      }
    }
  }
}
