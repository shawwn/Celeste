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
      this.face.Position = new Vector2(this.Width, this.Height) / 2f;
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
      this.Add((Component) new WaterInteraction((Func<bool>) (() => this.crushDir != Vector2.Zero)));
    }

    public CrushBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height, data.Enum<CrushBlock.Axes>("axes", CrushBlock.Axes.Both), data.Bool("chillout", false))
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
      if (this.crushDir == Vector2.Zero)
      {
        this.face.Position = new Vector2(this.Width, this.Height) / 2f;
        if (this.CollideCheck<Player>(this.Position + new Vector2(-1f, 0.0f)))
          --this.face.X;
        else if (this.CollideCheck<Player>(this.Position + new Vector2(1f, 0.0f)))
          ++this.face.X;
        else if (this.CollideCheck<Player>(this.Position + new Vector2(0.0f, -1f)))
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
      this.Position = this.Position + this.Shake;
      Draw.Rect(this.X + 2f, this.Y + 2f, this.Width - 4f, this.Height - 4f, this.fill);
      base.Render();
      this.Position = position;
    }

    private bool Submerged
    {
      get
      {
        return this.Scene.CollideCheck<Water>(new Rectangle((int) ((double) this.Center.X - 4.0), (int) this.Center.Y, 8, 4));
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
      Vector2 vector2 = new Vector2((float) (x * 8), (float) (y * 8));
      if ((uint) borderX > 0U)
      {
        Monocle.Image image = new Monocle.Image(subtexture);
        image.Color = Color.Black;
        image.Position = vector2 + new Vector2((float) borderX, 0.0f);
        this.Add((Component) image);
      }
      if ((uint) borderY > 0U)
      {
        Monocle.Image image = new Monocle.Image(subtexture);
        image.Color = Color.Black;
        image.Position = vector2 + new Vector2(0.0f, (float) borderY);
        this.Add((Component) image);
      }
      Monocle.Image image1 = new Monocle.Image(subtexture);
      image1.Position = vector2;
      this.Add((Component) image1);
      this.idleImages.Add(image1);
      if (borderX == 0 && (uint) borderY <= 0U)
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
      else if (borderY > 0)
      {
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
      if (!this.CanActivate(-direction))
        return DashCollisionResults.NormalCollision;
      this.Attack(-direction);
      return DashCollisionResults.Rebound;
    }

    private bool CanActivate(Vector2 direction)
    {
      return (!this.giant || (double) direction.X > 0.0) && (this.canActivate && this.crushDir != direction) && (((double) direction.X == 0.0 || this.canMoveHorizontally) && ((double) direction.Y == 0.0 || this.canMoveVertically));
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
      this.currentMoveLoopSfx.Position = new Vector2(this.Width, this.Height) / 2f;
      this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop", (string) null, 0.0f);
      this.face.Play("hit", false, false);
      this.crushDir = direction;
      this.canActivate = false;
      this.attackCoroutine.Replace(this.AttackSequence());
      this.ClearRemainder();
      this.TurnOffImages();
      this.ActivateParticles(this.crushDir);
      if ((double) this.crushDir.X < 0.0)
      {
        foreach (Component activeLeftImage in this.activeLeftImages)
          activeLeftImage.Visible = true;
        this.nextFaceDirection = "left";
      }
      else if ((double) this.crushDir.X > 0.0)
      {
        foreach (Component activeRightImage in this.activeRightImages)
          activeRightImage.Visible = true;
        this.nextFaceDirection = "right";
      }
      else if ((double) this.crushDir.Y < 0.0)
      {
        foreach (Component activeTopImage in this.activeTopImages)
          activeTopImage.Visible = true;
        this.nextFaceDirection = "up";
      }
      else if ((double) this.crushDir.Y > 0.0)
      {
        foreach (Component activeBottomImage in this.activeBottomImages)
          activeBottomImage.Visible = true;
        this.nextFaceDirection = "down";
      }
      bool flag = true;
      if (this.returnStack.Count > 0)
      {
        CrushBlock.MoveState moveState = this.returnStack[this.returnStack.Count - 1];
        if (moveState.Direction == direction || moveState.Direction == -direction)
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
      if (dir == Vector2.UnitX)
      {
        direction = 0.0f;
        position = this.CenterRight - Vector2.UnitX;
        positionRange = Vector2.UnitY * (this.Height - 2f) * 0.5f;
        num = (int) ((double) this.Height / 8.0) * 4;
      }
      else if (dir == -Vector2.UnitX)
      {
        direction = 3.141593f;
        position = this.CenterLeft + Vector2.UnitX;
        positionRange = Vector2.UnitY * (this.Height - 2f) * 0.5f;
        num = (int) ((double) this.Height / 8.0) * 4;
      }
      else if (dir == Vector2.UnitY)
      {
        direction = 1.570796f;
        position = this.BottomCenter - Vector2.UnitY;
        positionRange = Vector2.UnitX * (this.Width - 2f) * 0.5f;
        num = (int) ((double) this.Width / 8.0) * 4;
      }
      else
      {
        direction = -1.570796f;
        position = this.TopCenter + Vector2.UnitY;
        positionRange = Vector2.UnitX * (this.Width - 2f) * 0.5f;
        num = (int) ((double) this.Width / 8.0) * 4;
      }
      int amount = num + 2;
      this.level.Particles.Emit(CrushBlock.P_Activate, amount, position, positionRange, direction);
    }

    private IEnumerator AttackSequence()
    {
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.StartShaking(0.4f);
      yield return (object) 0.4f;
      if (!this.chillOut)
        this.canActivate = true;
      this.StopPlayerRunIntoAnimation = false;
      bool slowing = false;
      float speed1 = 0.0f;
      while (true)
      {
        if (!this.chillOut)
          speed1 = Calc.Approach(speed1, 240f, 500f * Engine.DeltaTime);
        else if (slowing || this.CollideCheck<SolidTiles>(this.Position + this.crushDir * 256f))
        {
          speed1 = Calc.Approach(speed1, 24f, (float) (500.0 * (double) Engine.DeltaTime * 0.25));
          if (!slowing)
          {
            slowing = true;
            Alarm.Set((Entity) this, 0.5f, (Action) (() =>
            {
              this.face.Play("hurt", false, false);
              this.currentMoveLoopSfx.Stop(true);
              this.TurnOffImages();
            }), Alarm.AlarmMode.Oneshot);
          }
        }
        else
          speed1 = Calc.Approach(speed1, 240f, 500f * Engine.DeltaTime);
        bool hit = (double) this.crushDir.X == 0.0 ? this.MoveVCheck(speed1 * this.crushDir.Y * Engine.DeltaTime) : this.MoveHCheck(speed1 * this.crushDir.X * Engine.DeltaTime);
        if (!hit)
        {
          if (this.Scene.OnInterval(0.02f))
          {
            Vector2 at;
            float dir;
            if (this.crushDir == Vector2.UnitX)
            {
              at = new Vector2(this.Left + 1f, Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
              dir = 3.141593f;
            }
            else if (this.crushDir == -Vector2.UnitX)
            {
              at = new Vector2(this.Right - 1f, Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
              dir = 0.0f;
            }
            else if (this.crushDir == Vector2.UnitY)
            {
              at = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f), this.Top + 1f);
              dir = -1.570796f;
            }
            else
            {
              at = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f), this.Bottom - 1f);
              dir = 1.570796f;
            }
            this.level.Particles.Emit(CrushBlock.P_Crushing, at, dir);
            at = new Vector2();
          }
          yield return (object) null;
        }
        else
          break;
      }
      FallingBlock fallingBlock = this.CollideFirst<FallingBlock>(this.Position + this.crushDir);
      if (fallingBlock != null)
        fallingBlock.Triggered = true;
      if (this.crushDir == -Vector2.UnitX)
      {
        Vector2 add = new Vector2(0.0f, 2f);
        for (int i = 0; (double) i < (double) this.Height / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Left - 1f, this.Top + 4f + (float) (i * 8));
          if (!this.Scene.CollideCheck<Water>(at) && this.Scene.CollideCheck<Solid>(at))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at + add, 0.0f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at - add, 0.0f);
          }
          at = new Vector2();
        }
        add = new Vector2();
      }
      else if (this.crushDir == Vector2.UnitX)
      {
        Vector2 add = new Vector2(0.0f, 2f);
        for (int i = 0; (double) i < (double) this.Height / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Right + 1f, this.Top + 4f + (float) (i * 8));
          if (!this.Scene.CollideCheck<Water>(at) && this.Scene.CollideCheck<Solid>(at))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at + add, 3.141593f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at - add, 3.141593f);
          }
          at = new Vector2();
        }
        add = new Vector2();
      }
      else if (this.crushDir == -Vector2.UnitY)
      {
        Vector2 add = new Vector2(2f, 0.0f);
        for (int i = 0; (double) i < (double) this.Width / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Left + 4f + (float) (i * 8), this.Top - 1f);
          if (!this.Scene.CollideCheck<Water>(at) && this.Scene.CollideCheck<Solid>(at))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at + add, 1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at - add, 1.570796f);
          }
          at = new Vector2();
        }
        add = new Vector2();
      }
      else if (this.crushDir == Vector2.UnitY)
      {
        Vector2 add = new Vector2(2f, 0.0f);
        for (int i = 0; (double) i < (double) this.Width / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Left + 4f + (float) (i * 8), this.Bottom + 1f);
          if (!this.Scene.CollideCheck<Water>(at) && this.Scene.CollideCheck<Solid>(at))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at + add, -1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at - add, -1.570796f);
          }
          at = new Vector2();
        }
        add = new Vector2();
      }
      Audio.Play("event:/game/06_reflection/crushblock_impact", this.Center);
      this.level.DirectionalShake(this.crushDir, 0.3f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.StartShaking(0.4f);
      this.StopPlayerRunIntoAnimation = true;
      SoundSource sfx = this.currentMoveLoopSfx;
      this.currentMoveLoopSfx.Param("end", 1f);
      this.currentMoveLoopSfx = (SoundSource) null;
      Alarm.Set((Entity) this, 0.5f, (Action) (() => sfx.RemoveSelf()), Alarm.AlarmMode.Oneshot);
      this.crushDir = Vector2.Zero;
      this.TurnOffImages();
      if (!this.chillOut)
      {
        this.face.Play("hurt", false, false);
        this.returnLoopSfx.Play("event:/game/06_reflection/crushblock_return_loop", (string) null, 0.0f);
        yield return (object) 0.4f;
        float speed2 = 0.0f;
        float waypointSfxDelay = 0.0f;
        while (this.returnStack.Count > 0)
        {
          yield return (object) null;
          this.StopPlayerRunIntoAnimation = false;
          CrushBlock.MoveState ret = this.returnStack[this.returnStack.Count - 1];
          speed2 = Calc.Approach(speed2, 60f, 160f * Engine.DeltaTime);
          waypointSfxDelay -= Engine.DeltaTime;
          if ((double) ret.Direction.X != 0.0)
            this.MoveTowardsX(ret.From.X, speed2 * Engine.DeltaTime);
          if ((double) ret.Direction.Y != 0.0)
            this.MoveTowardsY(ret.From.Y, speed2 * Engine.DeltaTime);
          bool atTarget = ((double) ret.Direction.X == 0.0 || (double) this.ExactPosition.X == (double) ret.From.X) && ((double) ret.Direction.Y == 0.0 || (double) this.ExactPosition.Y == (double) ret.From.Y);
          if (atTarget)
          {
            speed2 = 0.0f;
            this.returnStack.RemoveAt(this.returnStack.Count - 1);
            this.StopPlayerRunIntoAnimation = true;
            if (this.returnStack.Count <= 0)
            {
              this.face.Play("idle", false, false);
              this.returnLoopSfx.Stop(true);
              if ((double) waypointSfxDelay <= 0.0)
                Audio.Play("event:/game/06_reflection/crushblock_rest", this.Center);
            }
            else if ((double) waypointSfxDelay <= 0.0)
              Audio.Play("event:/game/06_reflection/crushblock_rest_waypoint", this.Center);
            waypointSfxDelay = 0.1f;
            this.StartShaking(0.2f);
            yield return (object) 0.2f;
          }
          ret = new CrushBlock.MoveState();
        }
      }
    }

    private bool MoveHCheck(float amount)
    {
      if (!this.MoveHCollideSolidsAndBounds(this.level, amount, true, (Action<Vector2, Vector2, Platform>) null))
        return false;
      if ((double) amount < 0.0 && (double) this.Left <= (double) this.level.Bounds.Left || (double) amount > 0.0 && (double) this.Right >= (double) this.level.Bounds.Right)
        return true;
      for (int index1 = 1; index1 <= 4; ++index1)
      {
        for (int index2 = 1; index2 >= -1; index2 -= 2)
        {
          if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) Math.Sign(amount), (float) (index1 * index2))))
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
      if ((double) amount < 0.0 && (double) this.Top <= (double) this.level.Bounds.Top || (double) amount > 0.0 && (double) this.Bottom >= (double) (this.level.Bounds.Bottom + 32))
        return true;
      for (int index1 = 1; index1 <= 4; ++index1)
      {
        for (int index2 = 1; index2 >= -1; index2 -= 2)
        {
          if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) (index1 * index2), (float) Math.Sign(amount))))
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

