// Decompiled with JetBrains decompiler
// Type: Celeste.SummitGem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class SummitGem : Entity
  {
    public static readonly Color[] GemColors = new Color[6]
    {
      Calc.HexToColor("9ee9ff"),
      Calc.HexToColor("54baff"),
      Calc.HexToColor("90ff2d"),
      Calc.HexToColor("ffd300"),
      Calc.HexToColor("ff609d"),
      Calc.HexToColor("c5e1ba")
    };
    public static ParticleType P_Shatter;
    public int GemID;
    public EntityID GID;
    private Sprite sprite;
    private Wiggler scaleWiggler;
    private Vector2 moveWiggleDir;
    private Wiggler moveWiggler;
    private float bounceSfxDelay;

    public SummitGem(EntityData data, Vector2 position, EntityID gid)
      : base(Vector2.op_Addition(data.Position, position))
    {
      this.GID = gid;
      this.GemID = data.Int("gem", 0);
      this.Collider = (Collider) new Hitbox(12f, 12f, -6f, -6f);
      this.Add((Component) (this.sprite = new Sprite(GFX.Game, "collectables/summitgems/" + (object) this.GemID + "/gem")));
      this.sprite.AddLoop("idle", "", 0.08f);
      this.sprite.Play("idle", false, false);
      this.sprite.CenterOrigin();
      if (SaveData.Instance.SummitGems != null && SaveData.Instance.SummitGems[this.GemID])
        this.sprite.Color = Color.op_Multiply(Color.get_White(), 0.5f);
      this.Add((Component) (this.scaleWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + (double) f * 0.300000011920929))), false, false)));
      this.moveWiggler = Wiggler.Create(0.8f, 2f, (Action<float>) null, false, false);
      this.moveWiggler.StartZero = true;
      this.Add((Component) this.moveWiggler);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    private void OnPlayer(Player player)
    {
      Level scene = this.Scene as Level;
      if (player.DashAttacking)
      {
        this.Add((Component) new Coroutine(this.SmashRoutine(player, scene), true));
      }
      else
      {
        player.PointBounce(this.Center);
        this.moveWiggler.Start();
        this.scaleWiggler.Start();
        this.moveWiggleDir = Vector2.op_Subtraction(this.Center, player.Center).SafeNormalize(Vector2.get_UnitY());
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        if ((double) this.bounceSfxDelay > 0.0)
          return;
        Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
        this.bounceSfxDelay = 0.1f;
      }
    }

    private IEnumerator SmashRoutine(Player player, Level level)
    {
      SummitGem summitGem = this;
      summitGem.Visible = false;
      summitGem.Collidable = false;
      player.Stamina = 110f;
      SoundEmitter.Play("event:/game/07_summit/gem_get", (Entity) summitGem, new Vector2?());
      Session session = (summitGem.Scene as Level).Session;
      session.DoNotLoad.Add(summitGem.GID);
      session.SummitGems[summitGem.GemID] = true;
      SaveData.Instance.RegisterSummitGem(summitGem.GemID);
      level.Shake(0.3f);
      Celeste.Celeste.Freeze(0.1f);
      SummitGem.P_Shatter.Color = SummitGem.GemColors[summitGem.GemID];
      float direction = player.Speed.Angle();
      level.ParticlesFG.Emit(SummitGem.P_Shatter, 5, summitGem.Position, Vector2.op_Multiply(Vector2.get_One(), 4f), direction - 1.570796f);
      level.ParticlesFG.Emit(SummitGem.P_Shatter, 5, summitGem.Position, Vector2.op_Multiply(Vector2.get_One(), 4f), direction + 1.570796f);
      SlashFx.Burst(summitGem.Position, direction);
      for (int index = 0; index < 10; ++index)
        summitGem.Scene.Add((Entity) new AbsorbOrb(summitGem.Position, (Entity) player));
      level.Flash(Color.get_White(), true);
      summitGem.Scene.Add((Entity) new SummitGem.BgFlash());
      Engine.TimeRate = 0.5f;
      while ((double) Engine.TimeRate < 1.0)
      {
        Engine.TimeRate += Engine.RawDeltaTime * 0.5f;
        yield return (object) null;
      }
      summitGem.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      this.bounceSfxDelay -= Engine.DeltaTime;
      this.sprite.Position = Vector2.op_Multiply(Vector2.op_Multiply(this.moveWiggleDir, this.moveWiggler.Value), -8f);
    }

    private class BgFlash : Entity
    {
      private float alpha = 1f;

      public BgFlash()
      {
        this.Depth = 10100;
        this.Tag = (int) Tags.Persistent;
      }

      public override void Update()
      {
        base.Update();
        this.alpha = Calc.Approach(this.alpha, 0.0f, Engine.DeltaTime * 0.5f);
        if ((double) this.alpha > 0.0)
          return;
        this.RemoveSelf();
      }

      public override void Render()
      {
        Vector2 position = (this.Scene as Level).Camera.Position;
        Draw.Rect((float) (position.X - 10.0), (float) (position.Y - 10.0), 340f, 200f, Color.op_Multiply(Color.get_Black(), this.alpha));
      }
    }
  }
}
