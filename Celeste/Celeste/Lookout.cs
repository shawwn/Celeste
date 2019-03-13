// Decompiled with JetBrains decompiler
// Type: Celeste.Lookout
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
  [Tracked(false)]
  public class Lookout : Entity
  {
    private TalkComponent talk;
    private Lookout.Hud hud;
    private Sprite sprite;
    private Tween lightTween;
    private bool interacting;
    private List<Vector2> nodes;
    private int node;
    private float nodePercent;
    private bool summit;

    public Lookout(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Depth = -8500;
      this.Add((Component) (this.talk = new TalkComponent(new Rectangle(-24, -8, 48, 8), new Vector2(-0.5f, -20f), new Action<Player>(this.Interact), (TalkComponent.HoverDisplay) null)));
      this.talk.PlayerMustBeFacing = false;
      this.summit = data.Bool(nameof (summit), false);
      VertexLight vertexLight = new VertexLight(new Vector2(-1f, -11f), Color.White, 0.8f, 16, 24);
      this.Add((Component) vertexLight);
      this.lightTween = vertexLight.CreatePulseTween();
      this.Add((Component) this.lightTween);
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("lookout")));
      this.sprite.OnFrameChange = (Action<string>) (s =>
      {
        if (!(s == "idle") || this.sprite.CurrentAnimationFrame != this.sprite.CurrentAnimationTotalFrames - 1)
          return;
        this.lightTween.Start();
      });
      Vector2[] vector2Array = data.NodesOffset(offset);
      if (vector2Array == null || (uint) vector2Array.Length <= 0U)
        return;
      this.nodes = new List<Vector2>((IEnumerable<Vector2>) vector2Array);
    }

    private void Interact(Player player)
    {
      this.Add((Component) new Coroutine(this.LookRoutine(player), true)
      {
        RemoveOnComplete = true
      });
    }

    public void StopInteracting()
    {
      this.interacting = false;
      this.sprite.Play("idle", false, false);
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return;
      this.sprite.Active = this.interacting || entity.StateMachine.State != Player.StDummy;
      if (!this.sprite.Active)
        this.sprite.SetAnimationFrame(0);
    }

    private IEnumerator LookRoutine(Player player)
    {
      Level level = this.SceneAs<Level>();
      Audio.Play("event:/game/general/lookout_use", this.Position);
      this.interacting = true;
      SandwichLava lava = this.Scene.Entities.FindFirst<SandwichLava>();
      if (lava != null)
        lava.Waiting = true;
      player.StateMachine.State = Player.StDummy;
      yield return (object) player.DummyWalkToExact((int) this.X, false, 1f);
      if ((double) Math.Abs(this.X - player.X) > 4.0)
      {
        player.StateMachine.State = Player.StNormal;
      }
      else
      {
        if (player.Facing == Facings.Right)
          this.sprite.Play("lookRight", false, false);
        else
          this.sprite.Play("lookLeft", false, false);
        player.Sprite.Visible = player.Hair.Visible = false;
        yield return (object) 0.2f;
        this.Scene.Add((Entity) (this.hud = new Lookout.Hud()));
        this.hud.TrackMode = this.nodes != null;
        this.nodePercent = 0.0f;
        this.node = 0;
        Audio.Play("event:/ui/game/lookout_on");
        while ((double) (this.hud.Easer = Calc.Approach(this.hud.Easer, 1f, Engine.DeltaTime * 3f)) < 1.0)
        {
          level.ScreenPadding = (float) (int) ((double) Ease.CubeInOut(this.hud.Easer) * 16.0);
          yield return (object) null;
        }
        float accel = 800f;
        float maxspd = 240f;
        Vector2 cam = level.Camera.Position;
        Vector2 speed = Vector2.Zero;
        Vector2 lastDir = Vector2.Zero;
        Vector2 camStart = level.Camera.Position;
        Vector2 camStartCenter = camStart + new Vector2(160f, 90f);
        Vector2 vector2;
        while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && (!Input.Dash.Pressed && !Input.Jump.Pressed) && this.interacting)
        {
          Vector2 dir = Input.Aim.Value;
          if (Math.Sign(dir.X) != Math.Sign(lastDir.X) || Math.Sign(dir.Y) != Math.Sign(lastDir.Y))
            Audio.Play("event:/game/general/lookout_move", this.Position);
          lastDir = dir;
          if (this.sprite.CurrentAnimationID != "lookLeft" && this.sprite.CurrentAnimationID != "lookRight")
          {
            if ((double) dir.X == 0.0)
            {
              if ((double) dir.Y == 0.0)
                this.sprite.Play("looking", false, false);
              else if ((double) dir.Y > 0.0)
                this.sprite.Play("lookingDown", false, false);
              else
                this.sprite.Play("lookingUp", false, false);
            }
            else if ((double) dir.X > 0.0)
            {
              if ((double) dir.Y == 0.0)
                this.sprite.Play("lookingRight", false, false);
              else if ((double) dir.Y > 0.0)
                this.sprite.Play("lookingDownRight", false, false);
              else
                this.sprite.Play("lookingUpRight", false, false);
            }
            else if ((double) dir.X < 0.0)
            {
              if ((double) dir.Y == 0.0)
                this.sprite.Play("lookingLeft", false, false);
              else if ((double) dir.Y > 0.0)
                this.sprite.Play("lookingDownLeft", false, false);
              else
                this.sprite.Play("lookingUpLeft", false, false);
            }
          }
          if (this.nodes == null)
          {
            speed += accel * dir * Engine.DeltaTime;
            if ((double) dir.X == 0.0)
              speed.X = Calc.Approach(speed.X, 0.0f, accel * 2f * Engine.DeltaTime);
            if ((double) dir.Y == 0.0)
              speed.Y = Calc.Approach(speed.Y, 0.0f, accel * 2f * Engine.DeltaTime);
            if ((double) speed.Length() > (double) maxspd)
              speed = speed.SafeNormalize(maxspd);
            Vector2 last = cam;
            List<Entity> blockers = this.Scene.Tracker.GetEntities<LookoutBlocker>();
            cam.X += speed.X * Engine.DeltaTime;
            double x1 = (double) cam.X;
            Rectangle bounds1 = level.Bounds;
            double left1 = (double) bounds1.Left;
            int num1;
            if (x1 >= left1)
            {
              double num2 = (double) cam.X + 320.0;
              bounds1 = level.Bounds;
              double right = (double) bounds1.Right;
              num1 = num2 > right ? 1 : 0;
            }
            else
              num1 = 1;
            if (num1 != 0)
              speed.X = 0.0f;
            double x2 = (double) cam.X;
            bounds1 = level.Bounds;
            double left2 = (double) bounds1.Left;
            bounds1 = level.Bounds;
            double num3 = (double) (bounds1.Right - 320);
            double num4 = (double) Calc.Clamp((float) x2, (float) left2, (float) num3);
            cam.X = (float) num4;
            foreach (Entity entity in blockers)
            {
              Entity blocker = entity;
              if ((double) cam.X + 320.0 > (double) blocker.Left && (double) cam.Y + 180.0 > (double) blocker.Top && (double) cam.X < (double) blocker.Right && (double) cam.Y < (double) blocker.Bottom)
              {
                cam.X = last.X;
                speed.X = 0.0f;
              }
              blocker = (Entity) null;
            }
            cam.Y += speed.Y * Engine.DeltaTime;
            Rectangle bounds2;
            int num5;
            if ((double) cam.Y >= (double) level.Bounds.Top)
            {
              double num2 = (double) cam.Y + 180.0;
              bounds2 = level.Bounds;
              double bottom = (double) bounds2.Bottom;
              num5 = num2 > bottom ? 1 : 0;
            }
            else
              num5 = 1;
            if (num5 != 0)
              speed.Y = 0.0f;
            double y = (double) cam.Y;
            bounds2 = level.Bounds;
            double top = (double) bounds2.Top;
            bounds2 = level.Bounds;
            double num6 = (double) (bounds2.Bottom - 180);
            double num7 = (double) Calc.Clamp((float) y, (float) top, (float) num6);
            cam.Y = (float) num7;
            foreach (Entity entity in blockers)
            {
              Entity blocker = entity;
              if ((double) cam.X + 320.0 > (double) blocker.Left && (double) cam.Y + 180.0 > (double) blocker.Top && (double) cam.X < (double) blocker.Right && (double) cam.Y < (double) blocker.Bottom)
              {
                cam.Y = last.Y;
                speed.Y = 0.0f;
              }
              blocker = (Entity) null;
            }
            level.Camera.Position = cam;
            last = new Vector2();
            blockers = (List<Entity>) null;
          }
          else
          {
            Vector2 from = this.node <= 0 ? camStartCenter : this.nodes[this.node - 1];
            Vector2 to = this.nodes[this.node];
            vector2 = from - to;
            float dist = vector2.Length();
            Vector2 towards = (to - from).SafeNormalize();
            if ((double) this.nodePercent < 0.25 && this.node > 0)
            {
              Vector2 last = this.node <= 1 ? camStartCenter : this.nodes[this.node - 2];
              Vector2 curveStart = Vector2.Lerp(last, from, 0.75f);
              Vector2 curveEnd = Vector2.Lerp(from, to, 0.25f);
              SimpleCurve curve = new SimpleCurve(curveStart, curveEnd, from);
              level.Camera.Position = curve.GetPoint((float) (0.5 + (double) this.nodePercent / 0.25 * 0.5));
              last = new Vector2();
              curveStart = new Vector2();
              curveEnd = new Vector2();
              curve = new SimpleCurve();
            }
            else if ((double) this.nodePercent > 0.75 && this.node < this.nodes.Count - 1)
            {
              Vector2 next = this.nodes[this.node + 1];
              Vector2 curveStart = Vector2.Lerp(from, to, 0.75f);
              Vector2 curveEnd = Vector2.Lerp(to, next, 0.25f);
              SimpleCurve curve = new SimpleCurve(curveStart, curveEnd, to);
              level.Camera.Position = curve.GetPoint((float) (((double) this.nodePercent - 0.75) / 0.25 * 0.5));
              next = new Vector2();
              curveStart = new Vector2();
              curveEnd = new Vector2();
              curve = new SimpleCurve();
            }
            else
              level.Camera.Position = Vector2.Lerp(from, to, this.nodePercent);
            level.Camera.Position += new Vector2(-160f, -90f);
            this.nodePercent -= dir.Y * (maxspd / dist) * Engine.DeltaTime;
            if ((double) this.nodePercent < 0.0)
            {
              if (this.node > 0)
              {
                --this.node;
                this.nodePercent = 1f;
              }
              else
                this.nodePercent = 0.0f;
            }
            else if ((double) this.nodePercent > 1.0)
            {
              if (this.node < this.nodes.Count - 1)
              {
                ++this.node;
                this.nodePercent = 0.0f;
              }
              else
              {
                this.nodePercent = 1f;
                if (this.summit)
                  break;
              }
            }
            float currentDist = 0.0f;
            float totalDist = 0.0f;
            for (int i = 0; i < this.nodes.Count; ++i)
            {
              vector2 = (i == 0 ? camStartCenter : this.nodes[i - 1]) - this.nodes[i];
              float d = vector2.Length();
              totalDist += d;
              if (i < this.node)
                currentDist += d;
              else if (i == this.node)
                currentDist += d * this.nodePercent;
            }
            this.hud.TrackPercent = currentDist / totalDist;
            from = new Vector2();
            to = new Vector2();
            towards = new Vector2();
          }
          yield return (object) null;
          dir = new Vector2();
        }
        player.Sprite.Visible = player.Hair.Visible = true;
        this.sprite.Play("idle", false, false);
        Audio.Play("event:/ui/game/lookout_off");
        while ((double) (this.hud.Easer = Calc.Approach(this.hud.Easer, 0.0f, Engine.DeltaTime * 3f)) > 0.0)
        {
          level.ScreenPadding = (float) (int) ((double) Ease.CubeInOut(this.hud.Easer) * 16.0);
          yield return (object) null;
        }
        bool atSummitTop = this.summit && this.node >= this.nodes.Count - 1 && (double) this.nodePercent >= 0.949999988079071;
        if (atSummitTop)
        {
          yield return (object) 0.5f;
          float duration = 3f;
          float approach = 0.0f;
          Coroutine routine = new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 2f, duration), true);
          this.Add((Component) routine);
          while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && (!Input.Dash.Pressed && !Input.Jump.Pressed) && this.interacting)
          {
            approach = Calc.Approach(approach, 1f, Engine.DeltaTime / duration);
            Audio.SetMusicParam("escape", approach);
            yield return (object) null;
          }
          routine = (Coroutine) null;
        }
        vector2 = camStart - level.Camera.Position;
        if ((double) vector2.Length() > 600.0)
        {
          Vector2 was = level.Camera.Position;
          Vector2 direction = (was - camStart).SafeNormalize();
          float duration = atSummitTop ? 1f : 0.5f;
          new FadeWipe(this.Scene, false, (Action) null).Duration = duration;
          for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
          {
            level.Camera.Position = was - direction * MathHelper.Lerp(0.0f, 64f, Ease.CubeIn(p));
            yield return (object) null;
          }
          level.Camera.Position = camStart + direction * 32f;
          FadeWipe fadeWipe = new FadeWipe(this.Scene, true, (Action) null);
          was = new Vector2();
          direction = new Vector2();
        }
        Audio.SetMusicParam("escape", 0.0f);
        level.ScreenPadding = 0.0f;
        level.ZoomSnap(Vector2.Zero, 1f);
        this.Scene.Remove((Entity) this.hud);
        this.interacting = false;
        player.StateMachine.State = Player.StNormal;
        yield return (object) null;
      }
    }

    private class Hud : Entity
    {
      public float Easer = 0.0f;
      private float left = 0.0f;
      private float right = 0.0f;
      private float up = 0.0f;
      private float down = 0.0f;
      private MTexture halfDot = GFX.Gui["dot"].GetSubtexture(0, 0, 64, 32, (MTexture) null);
      public bool TrackMode;
      public float TrackPercent;
      private float timerUp;
      private float timerDown;
      private float timerLeft;
      private float timerRight;
      private float multUp;
      private float multDown;
      private float multLeft;
      private float multRight;
      private Vector2 aim;

      public Hud()
      {
        this.AddTag((int) Tags.HUD);
      }

      public override void Update()
      {
        Level level = this.SceneAs<Level>();
        Vector2 position = level.Camera.Position;
        Rectangle bounds = level.Bounds;
        int width = 320;
        int height = 180;
        bool flag1 = this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) ((double) position.X - 8.0), (int) position.Y, width, height));
        bool flag2 = this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) ((double) position.X + 8.0), (int) position.Y, width, height));
        bool flag3 = this.TrackMode && (double) this.TrackPercent >= 1.0 || this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) position.X, (int) ((double) position.Y - 8.0), width, height));
        bool flag4 = this.TrackMode && (double) this.TrackPercent <= 0.0 || this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) position.X, (int) ((double) position.Y + 8.0), width, height));
        this.left = Calc.Approach(this.left, flag1 || (double) position.X <= (double) (bounds.Left + 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.right = Calc.Approach(this.right, flag2 || (double) position.X + (double) width >= (double) (bounds.Right - 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.up = Calc.Approach(this.up, flag3 || (double) position.Y <= (double) (bounds.Top + 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.down = Calc.Approach(this.down, flag4 || (double) position.Y + (double) height >= (double) (bounds.Bottom - 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.aim = Input.Aim.Value;
        if ((double) this.aim.X < 0.0)
        {
          this.multLeft = Calc.Approach(this.multLeft, 0.0f, Engine.DeltaTime * 2f);
          this.timerLeft += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multLeft = Calc.Approach(this.multLeft, 1f, Engine.DeltaTime * 2f);
          this.timerLeft += Engine.DeltaTime * 6f;
        }
        if ((double) this.aim.X > 0.0)
        {
          this.multRight = Calc.Approach(this.multRight, 0.0f, Engine.DeltaTime * 2f);
          this.timerRight += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multRight = Calc.Approach(this.multRight, 1f, Engine.DeltaTime * 2f);
          this.timerRight += Engine.DeltaTime * 6f;
        }
        if ((double) this.aim.Y < 0.0)
        {
          this.multUp = Calc.Approach(this.multUp, 0.0f, Engine.DeltaTime * 2f);
          this.timerUp += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multUp = Calc.Approach(this.multUp, 1f, Engine.DeltaTime * 2f);
          this.timerUp += Engine.DeltaTime * 6f;
        }
        if ((double) this.aim.Y > 0.0)
        {
          this.multDown = Calc.Approach(this.multDown, 0.0f, Engine.DeltaTime * 2f);
          this.timerDown += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multDown = Calc.Approach(this.multDown, 1f, Engine.DeltaTime * 2f);
          this.timerDown += Engine.DeltaTime * 6f;
        }
        base.Update();
      }

      public override void Render()
      {
        Level scene = this.Scene as Level;
        float num1 = Ease.CubeInOut(this.Easer);
        Color color = Color.White * num1;
        int num2 = (int) (80.0 * (double) num1);
        int num3 = (int) (80.0 * (double) num1 * (9.0 / 16.0));
        int num4 = 8;
        if (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null)
          color *= 0.25f;
        Draw.Rect((float) num2, (float) num3, (float) (1920 - num2 * 2 - num4), (float) num4, color);
        Draw.Rect((float) num2, (float) (num3 + num4), (float) (num4 + 2), (float) (1080 - num3 * 2 - num4), color);
        Draw.Rect((float) (1920 - num2 - num4 - 2), (float) num3, (float) (num4 + 2), (float) (1080 - num3 * 2 - num4), color);
        Draw.Rect((float) (num2 + num4), (float) (1080 - num3 - num4), (float) (1920 - num2 * 2 - num4), (float) num4, color);
        if (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null)
          return;
        MTexture mtexture = GFX.Gui["towerarrow"];
        float y1 = (float) ((double) num3 * (double) this.up - (double) ((float) (Math.Sin((double) this.timerUp) * 18.0) * MathHelper.Lerp(0.5f, 1f, this.multUp)) - (1.0 - (double) this.multUp) * 12.0);
        mtexture.DrawCentered(new Vector2(960f, y1), color * this.up, 1f, 1.570796f);
        float y2 = (float) (1080.0 - (double) num3 * (double) this.down + (double) ((float) (Math.Sin((double) this.timerDown) * 18.0) * MathHelper.Lerp(0.5f, 1f, this.multDown)) + (1.0 - (double) this.multDown) * 12.0);
        mtexture.DrawCentered(new Vector2(960f, y2), color * this.down, 1f, 4.712389f);
        if (!this.TrackMode)
        {
          float num5 = this.left;
          float amount1 = this.multLeft;
          float num6 = this.timerLeft;
          float num7 = this.right;
          float amount2 = this.multRight;
          float num8 = this.timerRight;
          if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
          {
            num5 = this.right;
            amount1 = this.multRight;
            num6 = this.timerRight;
            num7 = this.left;
            amount2 = this.multLeft;
            num8 = this.timerLeft;
          }
          float x1 = (float) ((double) num2 * (double) num5 - (double) ((float) (Math.Sin((double) num6) * 18.0) * MathHelper.Lerp(0.5f, 1f, amount1)) - (1.0 - (double) amount1) * 12.0);
          mtexture.DrawCentered(new Vector2(x1, 540f), color * num5);
          float x2 = (float) (1920.0 - (double) num2 * (double) num7 + (double) ((float) (Math.Sin((double) num8) * 18.0) * MathHelper.Lerp(0.5f, 1f, amount2)) + (1.0 - (double) amount2) * 12.0);
          mtexture.DrawCentered(new Vector2(x2, 540f), color * num7, 1f, 3.141593f);
        }
        else
        {
          int num5 = 1080 - num3 * 2 - 128 - 64;
          int num6 = 1920 - num2 - 64;
          float num7 = (float) ((double) (1080 - num5) / 2.0 + 32.0);
          Draw.Rect((float) (num6 - 7), num7 + 7f, 14f, (float) (num5 - 14), Color.Black * num1);
          this.halfDot.DrawJustified(new Vector2((float) num6, num7 + 7f), new Vector2(0.5f, 1f), Color.Black * num1);
          this.halfDot.DrawJustified(new Vector2((float) num6, (float) ((double) num7 + (double) num5 - 7.0)), new Vector2(0.5f, 1f), Color.Black * num1, new Vector2(1f, -1f));
          GFX.Gui["lookout/cursor"].DrawCentered(new Vector2((float) num6, num7 + (1f - this.TrackPercent) * (float) num5), Color.White * num1, 1f);
          GFX.Gui["lookout/summit"].DrawCentered(new Vector2((float) num6, num7 - 64f), Color.White * num1, 0.65f);
        }
      }
    }
  }
}

