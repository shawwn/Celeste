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
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Depth = -8500;
      this.Add((Component) (this.talk = new TalkComponent(new Rectangle(-24, -8, 48, 8), new Vector2(-0.5f, -20f), new Action<Player>(this.Interact), (TalkComponent.HoverDisplay) null)));
      this.talk.PlayerMustBeFacing = false;
      this.summit = data.Bool(nameof (summit), false);
      VertexLight vertexLight = new VertexLight(new Vector2(-1f, -11f), Color.get_White(), 0.8f, 16, 24);
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
      if (vector2Array == null || vector2Array.Length == 0)
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
      this.sprite.Active = this.interacting || entity.StateMachine.State != 11;
      if (this.sprite.Active)
        return;
      this.sprite.SetAnimationFrame(0);
    }

    private IEnumerator LookRoutine(Player player)
    {
      Lookout lookout = this;
      Level level = lookout.SceneAs<Level>();
      Audio.Play("event:/game/general/lookout_use", lookout.Position);
      lookout.interacting = true;
      SandwichLava first = lookout.Scene.Entities.FindFirst<SandwichLava>();
      if (first != null)
        first.Waiting = true;
      player.StateMachine.State = 11;
      yield return (object) player.DummyWalkToExact((int) lookout.X, false, 1f);
      if ((double) Math.Abs(lookout.X - player.X) > 4.0)
      {
        player.StateMachine.State = 0;
      }
      else
      {
        if (player.Facing == Facings.Right)
          lookout.sprite.Play("lookRight", false, false);
        else
          lookout.sprite.Play("lookLeft", false, false);
        player.Sprite.Visible = player.Hair.Visible = false;
        yield return (object) 0.2f;
        lookout.Scene.Add((Entity) (lookout.hud = new Lookout.Hud()));
        lookout.hud.TrackMode = lookout.nodes != null;
        lookout.nodePercent = 0.0f;
        lookout.node = 0;
        Audio.Play("event:/ui/game/lookout_on");
        while ((double) (lookout.hud.Easer = Calc.Approach(lookout.hud.Easer, 1f, Engine.DeltaTime * 3f)) < 1.0)
        {
          level.ScreenPadding = (float) (int) ((double) Ease.CubeInOut(lookout.hud.Easer) * 16.0);
          yield return (object) null;
        }
        float accel = 800f;
        float maxspd = 240f;
        Vector2 cam = level.Camera.Position;
        Vector2 speed = Vector2.get_Zero();
        Vector2 lastDir = Vector2.get_Zero();
        Vector2 camStart = level.Camera.Position;
        Vector2 camStartCenter = Vector2.op_Addition(camStart, new Vector2(160f, 90f));
        while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && (!Input.Dash.Pressed && !Input.Jump.Pressed) && lookout.interacting)
        {
          Vector2 vector2_1 = Input.Aim.Value;
          if (Math.Sign((float) vector2_1.X) != Math.Sign((float) lastDir.X) || Math.Sign((float) vector2_1.Y) != Math.Sign((float) lastDir.Y))
            Audio.Play("event:/game/general/lookout_move", lookout.Position);
          lastDir = vector2_1;
          if (lookout.sprite.CurrentAnimationID != "lookLeft" && lookout.sprite.CurrentAnimationID != "lookRight")
          {
            if (vector2_1.X == 0.0)
            {
              if (vector2_1.Y == 0.0)
                lookout.sprite.Play("looking", false, false);
              else if (vector2_1.Y > 0.0)
                lookout.sprite.Play("lookingDown", false, false);
              else
                lookout.sprite.Play("lookingUp", false, false);
            }
            else if (vector2_1.X > 0.0)
            {
              if (vector2_1.Y == 0.0)
                lookout.sprite.Play("lookingRight", false, false);
              else if (vector2_1.Y > 0.0)
                lookout.sprite.Play("lookingDownRight", false, false);
              else
                lookout.sprite.Play("lookingUpRight", false, false);
            }
            else if (vector2_1.X < 0.0)
            {
              if (vector2_1.Y == 0.0)
                lookout.sprite.Play("lookingLeft", false, false);
              else if (vector2_1.Y > 0.0)
                lookout.sprite.Play("lookingDownLeft", false, false);
              else
                lookout.sprite.Play("lookingUpLeft", false, false);
            }
          }
          if (lookout.nodes == null)
          {
            speed = Vector2.op_Addition(speed, Vector2.op_Multiply(Vector2.op_Multiply(accel, vector2_1), Engine.DeltaTime));
            if (vector2_1.X == 0.0)
              speed.X = (__Null) (double) Calc.Approach((float) speed.X, 0.0f, accel * 2f * Engine.DeltaTime);
            if (vector2_1.Y == 0.0)
              speed.Y = (__Null) (double) Calc.Approach((float) speed.Y, 0.0f, accel * 2f * Engine.DeltaTime);
            if ((double) ((Vector2) ref speed).Length() > (double) maxspd)
              speed = speed.SafeNormalize(maxspd);
            Vector2 vector2_2 = cam;
            List<Entity> entities = lookout.Scene.Tracker.GetEntities<LookoutBlocker>();
            ref __Null local1 = ref cam.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local1 = ^(float&) ref local1 + (float) speed.X * Engine.DeltaTime;
            // ISSUE: variable of the null type
            __Null x1 = cam.X;
            Rectangle bounds1 = level.Bounds;
            double left1 = (double) ((Rectangle) ref bounds1).get_Left();
            Rectangle bounds2;
            if (x1 >= left1)
            {
              double num = cam.X + 320.0;
              bounds2 = level.Bounds;
              double right = (double) ((Rectangle) ref bounds2).get_Right();
              if (num <= right)
                goto label_42;
            }
            speed.X = (__Null) 0.0;
label_42:
            ref Vector2 local2 = ref cam;
            // ISSUE: variable of the null type
            __Null x2 = cam.X;
            bounds2 = level.Bounds;
            double left2 = (double) ((Rectangle) ref bounds2).get_Left();
            bounds2 = level.Bounds;
            double num1 = (double) (((Rectangle) ref bounds2).get_Right() - 320);
            double num2 = (double) Calc.Clamp((float) x2, (float) left2, (float) num1);
            local2.X = (__Null) num2;
            foreach (Entity entity in entities)
            {
              if (cam.X + 320.0 > (double) entity.Left && cam.Y + 180.0 > (double) entity.Top && (cam.X < (double) entity.Right && cam.Y < (double) entity.Bottom))
              {
                cam.X = vector2_2.X;
                speed.X = (__Null) 0.0;
              }
            }
            ref __Null local3 = ref cam.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local3 = ^(float&) ref local3 + (float) speed.Y * Engine.DeltaTime;
            // ISSUE: variable of the null type
            __Null y1 = cam.Y;
            Rectangle bounds3 = level.Bounds;
            double top1 = (double) ((Rectangle) ref bounds3).get_Top();
            Rectangle bounds4;
            if (y1 >= top1)
            {
              double num3 = cam.Y + 180.0;
              bounds4 = level.Bounds;
              double bottom = (double) ((Rectangle) ref bounds4).get_Bottom();
              if (num3 <= bottom)
                goto label_51;
            }
            speed.Y = (__Null) 0.0;
label_51:
            ref Vector2 local4 = ref cam;
            // ISSUE: variable of the null type
            __Null y2 = cam.Y;
            bounds4 = level.Bounds;
            double top2 = (double) ((Rectangle) ref bounds4).get_Top();
            bounds4 = level.Bounds;
            double num4 = (double) (((Rectangle) ref bounds4).get_Bottom() - 180);
            double num5 = (double) Calc.Clamp((float) y2, (float) top2, (float) num4);
            local4.Y = (__Null) num5;
            foreach (Entity entity in entities)
            {
              if (cam.X + 320.0 > (double) entity.Left && cam.Y + 180.0 > (double) entity.Top && (cam.X < (double) entity.Right && cam.Y < (double) entity.Bottom))
              {
                cam.Y = vector2_2.Y;
                speed.Y = (__Null) 0.0;
              }
            }
            level.Camera.Position = cam;
          }
          else
          {
            Vector2 control = lookout.node <= 0 ? camStartCenter : lookout.nodes[lookout.node - 1];
            Vector2 node1 = lookout.nodes[lookout.node];
            Vector2 vector2_2 = Vector2.op_Subtraction(control, node1);
            float num1 = ((Vector2) ref vector2_2).Length();
            Vector2.op_Subtraction(node1, control).SafeNormalize();
            if ((double) lookout.nodePercent < 0.25 && lookout.node > 0)
              level.Camera.Position = new SimpleCurve(Vector2.Lerp(lookout.node <= 1 ? camStartCenter : lookout.nodes[lookout.node - 2], control, 0.75f), Vector2.Lerp(control, node1, 0.25f), control).GetPoint((float) (0.5 + (double) lookout.nodePercent / 0.25 * 0.5));
            else if ((double) lookout.nodePercent > 0.75 && lookout.node < lookout.nodes.Count - 1)
            {
              Vector2 node2 = lookout.nodes[lookout.node + 1];
              level.Camera.Position = new SimpleCurve(Vector2.Lerp(control, node1, 0.75f), Vector2.Lerp(node1, node2, 0.25f), node1).GetPoint((float) (((double) lookout.nodePercent - 0.75) / 0.25 * 0.5));
            }
            else
              level.Camera.Position = Vector2.Lerp(control, node1, lookout.nodePercent);
            Camera camera = level.Camera;
            camera.Position = Vector2.op_Addition(camera.Position, new Vector2(-160f, -90f));
            lookout.nodePercent -= (float) (vector2_1.Y * ((double) maxspd / (double) num1)) * Engine.DeltaTime;
            if ((double) lookout.nodePercent < 0.0)
            {
              if (lookout.node > 0)
              {
                --lookout.node;
                lookout.nodePercent = 1f;
              }
              else
                lookout.nodePercent = 0.0f;
            }
            else if ((double) lookout.nodePercent > 1.0)
            {
              if (lookout.node < lookout.nodes.Count - 1)
              {
                ++lookout.node;
                lookout.nodePercent = 0.0f;
              }
              else
              {
                lookout.nodePercent = 1f;
                if (lookout.summit)
                  break;
              }
            }
            float num2 = 0.0f;
            float num3 = 0.0f;
            for (int index = 0; index < lookout.nodes.Count; ++index)
            {
              Vector2 vector2_3 = Vector2.op_Subtraction(index == 0 ? camStartCenter : lookout.nodes[index - 1], lookout.nodes[index]);
              float num4 = ((Vector2) ref vector2_3).Length();
              num3 += num4;
              if (index < lookout.node)
                num2 += num4;
              else if (index == lookout.node)
                num2 += num4 * lookout.nodePercent;
            }
            lookout.hud.TrackPercent = num2 / num3;
          }
          yield return (object) null;
        }
        player.Sprite.Visible = player.Hair.Visible = true;
        lookout.sprite.Play("idle", false, false);
        Audio.Play("event:/ui/game/lookout_off");
        while ((double) (lookout.hud.Easer = Calc.Approach(lookout.hud.Easer, 0.0f, Engine.DeltaTime * 3f)) > 0.0)
        {
          level.ScreenPadding = (float) (int) ((double) Ease.CubeInOut(lookout.hud.Easer) * 16.0);
          yield return (object) null;
        }
        bool atSummitTop = lookout.summit && lookout.node >= lookout.nodes.Count - 1 && (double) lookout.nodePercent >= 0.949999988079071;
        float duration;
        float approach;
        if (atSummitTop)
        {
          yield return (object) 0.5f;
          duration = 3f;
          approach = 0.0f;
          Coroutine coroutine = new Coroutine(level.ZoomTo(new Vector2(160f, 90f), 2f, duration), true);
          lookout.Add((Component) coroutine);
          while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed && (!Input.Dash.Pressed && !Input.Jump.Pressed) && lookout.interacting)
          {
            approach = Calc.Approach(approach, 1f, Engine.DeltaTime / duration);
            Audio.SetMusicParam("escape", approach);
            yield return (object) null;
          }
        }
        Vector2 vector2 = Vector2.op_Subtraction(camStart, level.Camera.Position);
        if ((double) ((Vector2) ref vector2).Length() > 600.0)
        {
          Vector2 was = level.Camera.Position;
          Vector2 direction = Vector2.op_Subtraction(was, camStart).SafeNormalize();
          approach = atSummitTop ? 1f : 0.5f;
          new FadeWipe(lookout.Scene, false, (Action) null).Duration = approach;
          for (duration = 0.0f; (double) duration < 1.0; duration += Engine.DeltaTime / approach)
          {
            level.Camera.Position = Vector2.op_Subtraction(was, Vector2.op_Multiply(direction, MathHelper.Lerp(0.0f, 64f, Ease.CubeIn(duration))));
            yield return (object) null;
          }
          level.Camera.Position = Vector2.op_Addition(camStart, Vector2.op_Multiply(direction, 32f));
          FadeWipe fadeWipe = new FadeWipe(lookout.Scene, true, (Action) null);
          was = (Vector2) null;
          direction = (Vector2) null;
        }
        Audio.SetMusicParam("escape", 0.0f);
        level.ScreenPadding = 0.0f;
        level.ZoomSnap(Vector2.get_Zero(), 1f);
        lookout.Scene.Remove((Entity) lookout.hud);
        lookout.interacting = false;
        player.StateMachine.State = 0;
        yield return (object) null;
      }
    }

    private class Hud : Entity
    {
      private MTexture halfDot = GFX.Gui["dot"].GetSubtexture(0, 0, 64, 32, (MTexture) null);
      public bool TrackMode;
      public float TrackPercent;
      public float Easer;
      private float timerUp;
      private float timerDown;
      private float timerLeft;
      private float timerRight;
      private float multUp;
      private float multDown;
      private float multLeft;
      private float multRight;
      private float left;
      private float right;
      private float up;
      private float down;
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
        int num1 = 320;
        int num2 = 180;
        bool flag1 = this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) (position.X - 8.0), (int) position.Y, num1, num2));
        bool flag2 = this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) (position.X + 8.0), (int) position.Y, num1, num2));
        bool flag3 = this.TrackMode && (double) this.TrackPercent >= 1.0 || this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) position.X, (int) (position.Y - 8.0), num1, num2));
        bool flag4 = this.TrackMode && (double) this.TrackPercent <= 0.0 || this.Scene.CollideCheck<LookoutBlocker>(new Rectangle((int) position.X, (int) (position.Y + 8.0), num1, num2));
        this.left = Calc.Approach(this.left, flag1 || position.X <= (double) (((Rectangle) ref bounds).get_Left() + 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.right = Calc.Approach(this.right, flag2 || position.X + (double) num1 >= (double) (((Rectangle) ref bounds).get_Right() - 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.up = Calc.Approach(this.up, flag3 || position.Y <= (double) (((Rectangle) ref bounds).get_Top() + 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.down = Calc.Approach(this.down, flag4 || position.Y + (double) num2 >= (double) (((Rectangle) ref bounds).get_Bottom() - 2) ? 0.0f : 1f, Engine.DeltaTime * 8f);
        this.aim = Input.Aim.Value;
        if (this.aim.X < 0.0)
        {
          this.multLeft = Calc.Approach(this.multLeft, 0.0f, Engine.DeltaTime * 2f);
          this.timerLeft += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multLeft = Calc.Approach(this.multLeft, 1f, Engine.DeltaTime * 2f);
          this.timerLeft += Engine.DeltaTime * 6f;
        }
        if (this.aim.X > 0.0)
        {
          this.multRight = Calc.Approach(this.multRight, 0.0f, Engine.DeltaTime * 2f);
          this.timerRight += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multRight = Calc.Approach(this.multRight, 1f, Engine.DeltaTime * 2f);
          this.timerRight += Engine.DeltaTime * 6f;
        }
        if (this.aim.Y < 0.0)
        {
          this.multUp = Calc.Approach(this.multUp, 0.0f, Engine.DeltaTime * 2f);
          this.timerUp += Engine.DeltaTime * 12f;
        }
        else
        {
          this.multUp = Calc.Approach(this.multUp, 1f, Engine.DeltaTime * 2f);
          this.timerUp += Engine.DeltaTime * 6f;
        }
        if (this.aim.Y > 0.0)
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
        Color color = Color.op_Multiply(Color.get_White(), num1);
        int num2 = (int) (80.0 * (double) num1);
        int num3 = (int) (80.0 * (double) num1 * (9.0 / 16.0));
        int num4 = 8;
        if (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null)
          color = Color.op_Multiply(color, 0.25f);
        Draw.Rect((float) num2, (float) num3, (float) (1920 - num2 * 2 - num4), (float) num4, color);
        Draw.Rect((float) num2, (float) (num3 + num4), (float) (num4 + 2), (float) (1080 - num3 * 2 - num4), color);
        Draw.Rect((float) (1920 - num2 - num4 - 2), (float) num3, (float) (num4 + 2), (float) (1080 - num3 * 2 - num4), color);
        Draw.Rect((float) (num2 + num4), (float) (1080 - num3 - num4), (float) (1920 - num2 * 2 - num4), (float) num4, color);
        if (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null)
          return;
        MTexture mtexture = GFX.Gui["towerarrow"];
        float num5 = (float) ((double) num3 * (double) this.up - (double) ((float) (Math.Sin((double) this.timerUp) * 18.0) * MathHelper.Lerp(0.5f, 1f, this.multUp)) - (1.0 - (double) this.multUp) * 12.0);
        mtexture.DrawCentered(new Vector2(960f, num5), Color.op_Multiply(color, this.up), 1f, 1.570796f);
        float num6 = (float) (1080.0 - (double) num3 * (double) this.down + (double) ((float) (Math.Sin((double) this.timerDown) * 18.0) * MathHelper.Lerp(0.5f, 1f, this.multDown)) + (1.0 - (double) this.multDown) * 12.0);
        mtexture.DrawCentered(new Vector2(960f, num6), Color.op_Multiply(color, this.down), 1f, 4.712389f);
        if (!this.TrackMode)
        {
          float num7 = this.left;
          float num8 = this.multLeft;
          float num9 = this.timerLeft;
          float num10 = this.right;
          float num11 = this.multRight;
          float num12 = this.timerRight;
          if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
          {
            num7 = this.right;
            num8 = this.multRight;
            num9 = this.timerRight;
            num10 = this.left;
            num11 = this.multLeft;
            num12 = this.timerLeft;
          }
          float num13 = (float) ((double) num2 * (double) num7 - (double) ((float) (Math.Sin((double) num9) * 18.0) * MathHelper.Lerp(0.5f, 1f, num8)) - (1.0 - (double) num8) * 12.0);
          mtexture.DrawCentered(new Vector2(num13, 540f), Color.op_Multiply(color, num7));
          float num14 = (float) (1920.0 - (double) num2 * (double) num10 + (double) ((float) (Math.Sin((double) num12) * 18.0) * MathHelper.Lerp(0.5f, 1f, num11)) + (1.0 - (double) num11) * 12.0);
          mtexture.DrawCentered(new Vector2(num14, 540f), Color.op_Multiply(color, num10), 1f, 3.141593f);
        }
        else
        {
          int num7 = 1080 - num3 * 2 - 128 - 64;
          int num8 = 1920 - num2 - 64;
          float num9 = (float) ((double) (1080 - num7) / 2.0 + 32.0);
          Draw.Rect((float) (num8 - 7), num9 + 7f, 14f, (float) (num7 - 14), Color.op_Multiply(Color.get_Black(), num1));
          this.halfDot.DrawJustified(new Vector2((float) num8, num9 + 7f), new Vector2(0.5f, 1f), Color.op_Multiply(Color.get_Black(), num1));
          this.halfDot.DrawJustified(new Vector2((float) num8, (float) ((double) num9 + (double) num7 - 7.0)), new Vector2(0.5f, 1f), Color.op_Multiply(Color.get_Black(), num1), new Vector2(1f, -1f));
          GFX.Gui["lookout/cursor"].DrawCentered(new Vector2((float) num8, num9 + (1f - this.TrackPercent) * (float) num7), Color.op_Multiply(Color.get_White(), num1), 1f);
          GFX.Gui["lookout/summit"].DrawCentered(new Vector2((float) num8, num9 - 64f), Color.op_Multiply(Color.get_White(), num1), 0.65f);
        }
      }
    }
  }
}
