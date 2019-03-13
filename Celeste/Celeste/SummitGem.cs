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
      : base(data.Position + position)
    {
      this.GID = gid;
      this.GemID = data.Int("gem", 0);
      this.Collider = (Collider) new Hitbox(12f, 12f, -6f, -6f);
      this.Add((Component) (this.sprite = new Sprite(GFX.Game, "collectables/summitgems/" + (object) this.GemID + "/gem")));
      this.sprite.AddLoop("idle", "", 0.08f);
      this.sprite.Play("idle", false, false);
      this.sprite.CenterOrigin();
      if (SaveData.Instance.SummitGems != null && SaveData.Instance.SummitGems[this.GemID])
        this.sprite.Color = Color.White * 0.5f;
      this.Add((Component) (this.scaleWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.300000011920929)), false, false)));
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
        this.moveWiggleDir = (this.Center - player.Center).SafeNormalize(Vector2.UnitY);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        if ((double) this.bounceSfxDelay <= 0.0)
        {
          Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
          this.bounceSfxDelay = 0.1f;
        }
      }
    }

    private IEnumerator SmashRoutine(Player player, Level level)
    {
      this.Visible = false;
      this.Collidable = false;
      player.Stamina = 110f;
      SoundEmitter.Play("event:/game/07_summit/gem_get", (Entity) this, new Vector2?());
      Session session = (this.Scene as Level).Session;
      session.DoNotLoad.Add(this.GID);
      session.SummitGems[this.GemID] = true;
      SaveData.Instance.RegisterSummitGem(this.GemID);
      level.Shake(0.3f);
      Celeste.Freeze(0.1f);
      SummitGem.P_Shatter.Color = SummitGem.GemColors[this.GemID];
      float angle = player.Speed.Angle();
      level.ParticlesFG.Emit(SummitGem.P_Shatter, 5, this.Position, Vector2.One * 4f, angle - 1.570796f);
      level.ParticlesFG.Emit(SummitGem.P_Shatter, 5, this.Position, Vector2.One * 4f, angle + 1.570796f);
      SlashFx.Burst(this.Position, angle);
      for (int i = 0; i < 10; ++i)
        this.Scene.Add((Entity) new AbsorbOrb(this.Position, (Entity) player));
      level.Flash(Color.White, true);
      this.Scene.Add((Entity) new SummitGem.BgFlash());
      Engine.TimeRate = 0.5f;
      while ((double) Engine.TimeRate < 1.0)
      {
        Engine.TimeRate += Engine.RawDeltaTime * 0.5f;
        yield return (object) null;
      }
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      this.bounceSfxDelay -= Engine.DeltaTime;
      this.sprite.Position = this.moveWiggleDir * this.moveWiggler.Value * -8f;
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
        Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, Color.Black * this.alpha);
      }
    }
  }
}

