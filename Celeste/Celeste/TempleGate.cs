// Decompiled with JetBrains decompiler
// Type: Celeste.TempleGate
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  [Tracked(false)]
  public class TempleGate : Solid
  {
    private float holdingWaitTimer = 0.2f;
    private const int OpenHeight = 0;
    private const float HoldingWaitTime = 0.2f;
    private const float HoldingOpenDistSq = 4096f;
    private const float HoldingCloseDistSq = 6400f;
    private const int MinDrawHeight = 4;
    public string LevelID;
    public TempleGate.Types Type;
    public bool ClaimedByASwitch;
    private bool theoGate;
    private int closedHeight;
    private Sprite sprite;
    private Shaker shaker;
    private float drawHeight;
    private float drawHeightMoveSpeed;
    private bool open;
    private Vector2 holdingCheckFrom;
    private bool lockState;

    public TempleGate(
      Vector2 position,
      int height,
      TempleGate.Types type,
      string spriteName,
      string levelID)
      : base(position, 8f, (float) height, true)
    {
      this.Type = type;
      this.closedHeight = height;
      this.LevelID = levelID;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("templegate_" + spriteName)));
      this.sprite.X = this.Collider.Width / 2f;
      this.sprite.Play("idle", false, false);
      this.Add((Component) (this.shaker = new Shaker(false, (Action<Vector2>) null)));
      this.Depth = -9000;
      this.theoGate = spriteName.Equals("theo", StringComparison.InvariantCultureIgnoreCase);
      this.holdingCheckFrom = this.Position + new Vector2(this.Width / 2f, (float) (height / 2));
    }

    public TempleGate(EntityData data, Vector2 offset, string levelID)
      : this(data.Position + offset, data.Height, data.Enum<TempleGate.Types>("type", TempleGate.Types.NearestSwitch), data.Attr(nameof (sprite), "default"), levelID)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.Type == TempleGate.Types.CloseBehindPlayer)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.Left < (double) this.Right && (double) entity.Bottom >= (double) this.Top && (double) entity.Top <= (double) this.Bottom)
        {
          this.StartOpen();
          this.Add((Component) new Coroutine(this.CloseBehindPlayer(), true));
        }
      }
      else if (this.Type == TempleGate.Types.CloseBehindPlayerAlways)
      {
        this.StartOpen();
        this.Add((Component) new Coroutine(this.CloseBehindPlayer(), true));
      }
      else if (this.Type == TempleGate.Types.CloseBehindPlayerAndTheo)
      {
        this.StartOpen();
        this.Add((Component) new Coroutine(this.CloseBehindPlayerAndTheo(), true));
      }
      else if (this.Type == TempleGate.Types.HoldingTheo)
      {
        if (this.TheoIsNearby())
          this.StartOpen();
        this.Hitbox.Width = 16f;
      }
      else if (this.Type == TempleGate.Types.TouchSwitches)
        this.Add((Component) new Coroutine(this.CheckTouchSwitches(), true));
      this.drawHeight = Math.Max(4f, this.Height);
    }

    public bool CloseBehindPlayerCheck()
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      return entity != null && (double) entity.X < (double) this.X;
    }

    public void SwitchOpen()
    {
      this.sprite.Play("open", false, false);
      Alarm.Set((Entity) this, 0.2f, (Action) (() =>
      {
        this.shaker.ShakeFor(0.2f, false);
        Alarm.Set((Entity) this, 0.2f, new Action(this.Open), Alarm.AlarmMode.Oneshot);
      }), Alarm.AlarmMode.Oneshot);
    }

    public void Open()
    {
      Audio.Play(this.theoGate ? "event:/game/05_mirror_temple/gate_theo_open" : "event:/game/05_mirror_temple/gate_main_open", this.Position);
      this.holdingWaitTimer = 0.2f;
      this.drawHeightMoveSpeed = 200f;
      this.drawHeight = this.Height;
      this.shaker.ShakeFor(0.2f, false);
      this.SetHeight(0);
      this.sprite.Play("open", false, false);
      this.open = true;
    }

    public void StartOpen()
    {
      this.SetHeight(0);
      this.drawHeight = 4f;
      this.open = true;
    }

    public void Close()
    {
      Audio.Play(this.theoGate ? "event:/game/05_mirror_temple/gate_theo_close" : "event:/game/05_mirror_temple/gate_main_close", this.Position);
      this.holdingWaitTimer = 0.2f;
      this.drawHeightMoveSpeed = 300f;
      this.drawHeight = Math.Max(4f, this.Height);
      this.shaker.ShakeFor(0.2f, false);
      this.SetHeight(this.closedHeight);
      this.sprite.Play("hit", false, false);
      this.open = false;
    }

    private IEnumerator CloseBehindPlayer()
    {
      while (true)
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (this.lockState || player == null || (double) player.Left <= (double) this.Right + 4.0)
        {
          yield return (object) null;
          player = (Player) null;
        }
        else
          break;
      }
      this.Close();
    }

    private IEnumerator CloseBehindPlayerAndTheo()
    {
      while (true)
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        if (player != null && (double) player.Left > (double) this.Right + 4.0)
        {
          TheoCrystal theo = this.Scene.Tracker.GetEntity<TheoCrystal>();
          if (this.lockState || theo == null || (double) theo.Left <= (double) this.Right + 4.0)
            theo = (TheoCrystal) null;
          else
            break;
        }
        yield return (object) null;
        player = (Player) null;
      }
      this.Close();
    }

    private IEnumerator CheckTouchSwitches()
    {
      while (!Switch.Check(this.Scene))
        yield return (object) null;
      this.sprite.Play("open", false, false);
      yield return (object) 0.5f;
      this.shaker.ShakeFor(0.2f, false);
      yield return (object) 0.2f;
      while (this.lockState)
        yield return (object) null;
      this.Open();
    }

    public bool TheoIsNearby()
    {
      TheoCrystal entity = this.Scene.Tracker.GetEntity<TheoCrystal>();
      return entity == null || (double) entity.X > (double) this.X + 10.0 || (double) Vector2.DistanceSquared(this.holdingCheckFrom, entity.Center) < (this.open ? 6400.0 : 4096.0);
    }

    private void SetHeight(int height)
    {
      if ((double) height < (double) this.Collider.Height)
      {
        this.Collider.Height = (float) height;
      }
      else
      {
        float y = this.Y;
        int height1 = (int) this.Collider.Height;
        if ((double) this.Collider.Height < 64.0)
        {
          this.Y -= 64f - this.Collider.Height;
          this.Collider.Height = 64f;
        }
        this.MoveVExact(height - height1);
        this.Y = y;
        this.Collider.Height = (float) height;
      }
    }

    public override void Update()
    {
      base.Update();
      if (this.Type == TempleGate.Types.HoldingTheo)
      {
        if ((double) this.holdingWaitTimer > 0.0)
          this.holdingWaitTimer -= Engine.DeltaTime;
        else if (!this.lockState)
        {
          if (this.open && !this.TheoIsNearby())
          {
            this.Close();
            Player player = this.CollideFirst<Player>(this.Position + new Vector2(8f, 0.0f));
            if (player != null)
              player.Die(Vector2.Zero, false, true);
          }
          else if (!this.open && this.TheoIsNearby())
            this.Open();
        }
      }
      float target = Math.Max(4f, this.Height);
      if ((double) this.drawHeight != (double) target)
      {
        this.lockState = true;
        this.drawHeight = Calc.Approach(this.drawHeight, target, this.drawHeightMoveSpeed * Engine.DeltaTime);
      }
      else
        this.lockState = false;
    }

    public override void Render()
    {
      Vector2 vector2 = new Vector2((float) Math.Sign(this.shaker.Value.X), 0.0f);
      Draw.Rect(this.X - 2f, this.Y - 8f, 14f, 10f, Color.Black);
      this.sprite.DrawSubrect(Vector2.Zero + vector2, new Rectangle(0, (int) ((double) this.sprite.Height - (double) this.drawHeight), (int) this.sprite.Width, (int) this.drawHeight));
    }

    public enum Types
    {
      NearestSwitch,
      CloseBehindPlayer,
      CloseBehindPlayerAlways,
      HoldingTheo,
      TouchSwitches,
      CloseBehindPlayerAndTheo,
    }
  }
}

