// Decompiled with JetBrains decompiler
// Type: Celeste.FireBall
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class FireBall : Entity
  {
    public static ParticleType P_FireTrail;
    public static ParticleType P_IceTrail;
    public static ParticleType P_IceBreak;
    private const float FireSpeed = 60f;
    private const float IceSpeed = 30f;
    private const float IceSpeedMult = 0.5f;
    private Vector2[] nodes;
    private int amount;
    private int index;
    private float offset;
    private float[] lengths;
    private float speed;
    private float speedMult;
    private float percent;
    private bool iceMode;
    private bool broken;
    private float mult;
    private SoundSource trackSfx;
    private Sprite sprite;
    private Wiggler hitWiggler;
    private Vector2 hitDir;

    public FireBall(Vector2[] nodes, int amount, int index, float offset, float speedMult)
    {
      this.Collider = (Collider) new Monocle.Circle(6f, 0.0f, 0.0f);
      this.nodes = nodes;
      this.amount = amount;
      this.index = index;
      this.offset = offset;
      this.mult = speedMult;
      this.lengths = new float[nodes.Length];
      for (int index1 = 1; index1 < this.lengths.Length; ++index1)
        this.lengths[index1] = this.lengths[index1 - 1] + Vector2.Distance(nodes[index1 - 1], nodes[index1]);
      this.speed = 60f / this.lengths[this.lengths.Length - 1] * this.mult;
      this.percent = index != 0 ? (float) index / (float) amount : 0.0f;
      this.percent += 1f / (float) amount * offset;
      this.percent %= 1f;
      this.Position = this.GetPercentPosition(this.percent);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnBounce), (Collider) new Hitbox(16f, 6f, -8f, -3f), (Collider) null));
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
      this.sprite = GFX.SpriteBank.Create("fireball");
      this.Add((Component) this.sprite);
      if (index == 0)
        this.Add((Component) (this.trackSfx = new SoundSource("event:/env/local/09_core/fireballs_idle")));
      this.Add((Component) (this.hitWiggler = Wiggler.Create(1.2f, 2f, (Action<float>) null, false, false)));
      this.hitWiggler.StartZero = true;
    }

    public FireBall(EntityData data, Vector2 offset)
      : this(data.NodesWithPosition(offset), data.Int(nameof (amount), 1), 0, data.Float(nameof (offset), 0.0f), data.Float(nameof (speed), 1f))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.iceMode = this.SceneAs<Level>().CoreMode == Session.CoreModes.Cold;
      this.speedMult = this.iceMode ? 0.0f : 1f;
      this.sprite.Play(this.iceMode ? "ice" : "hot", false, true);
      if (this.index != 0)
        return;
      for (int index = 1; index < this.amount; ++index)
        this.Scene.Add((Entity) new FireBall(this.nodes, this.amount, index, this.offset, this.mult));
      if (this.iceMode && this.trackSfx != null)
        this.trackSfx.Pause();
      this.PositionTrackSfx();
    }

    public override void Update()
    {
      base.Update();
      this.speedMult = Calc.Approach(this.speedMult, this.iceMode ? 0.5f : 1f, 2f * Engine.DeltaTime);
      this.percent += this.speed * this.speedMult * Engine.DeltaTime;
      if ((double) this.percent >= 1.0)
      {
        this.percent %= 1f;
        if (this.broken && this.nodes[this.nodes.Length - 1] != this.nodes[0])
        {
          this.broken = false;
          this.Collidable = true;
          this.sprite.Play(this.iceMode ? "ice" : "hot", false, true);
        }
      }
      this.Position = this.GetPercentPosition(this.percent);
      this.PositionTrackSfx();
      if (this.broken || !this.Scene.OnInterval(this.iceMode ? 0.08f : 0.05f))
        return;
      this.SceneAs<Level>().ParticlesBG.Emit(this.iceMode ? FireBall.P_IceTrail : FireBall.P_FireTrail, 1, this.Center, Vector2.One * 4f);
    }

    public void PositionTrackSfx()
    {
      if (this.index != 0 || this.trackSfx == null)
        return;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        Vector2? nullable = new Vector2?();
        for (int index = 1; index < this.nodes.Length; ++index)
        {
          Vector2 vector2_1 = Calc.ClosestPointOnLine(this.nodes[index - 1], this.nodes[index], entity.Center);
          int num1;
          if (nullable.HasValue)
          {
            Vector2 vector2_2 = vector2_1 - entity.Center;
            double num2 = (double) vector2_2.Length();
            vector2_2 = nullable.Value - entity.Center;
            double num3 = (double) vector2_2.Length();
            num1 = num2 < num3 ? 1 : 0;
          }
          else
            num1 = 1;
          if (num1 != 0)
            nullable = new Vector2?(vector2_1);
        }
        if (nullable.HasValue)
          this.trackSfx.Position = nullable.Value - this.Position;
      }
    }

    public override void Render()
    {
      this.sprite.Position = this.hitDir * this.hitWiggler.Value * 8f;
      if (!this.broken)
        this.sprite.DrawOutline(Color.Black, 1);
      base.Render();
    }

    private void OnPlayer(Player player)
    {
      if (!this.iceMode && !this.broken)
      {
        this.KillPlayer(player);
      }
      else
      {
        if (!this.iceMode || this.broken || (double) player.Bottom <= (double) this.Y + 4.0)
          return;
        this.KillPlayer(player);
      }
    }

    private void KillPlayer(Player player)
    {
      Vector2 direction = (player.Center - this.Center).SafeNormalize();
      if (player.Die(direction, false, true) == null)
        return;
      this.hitDir = direction;
      this.hitWiggler.Start();
    }

    private void OnBounce(Player player)
    {
      if (!this.iceMode || this.broken || (double) player.Bottom > (double) this.Y + 4.0 || (double) player.Speed.Y < 0.0)
        return;
      Audio.Play("event:/game/09_core/iceball_break", this.Position);
      this.sprite.Play("shatter", false, false);
      this.broken = true;
      this.Collidable = false;
      player.Bounce((float) (int) ((double) this.Y - 2.0));
      this.SceneAs<Level>().Particles.Emit(FireBall.P_IceBreak, 18, this.Center, Vector2.One * 6f);
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
      this.iceMode = mode == Session.CoreModes.Cold;
      if (!this.broken)
        this.sprite.Play(this.iceMode ? "ice" : "hot", false, true);
      if (this.index != 0 || this.trackSfx == null)
        return;
      if (this.iceMode)
        this.trackSfx.Pause();
      else
        this.trackSfx.Resume();
    }

    private Vector2 GetPercentPosition(float percent)
    {
      if ((double) percent <= 0.0)
        return this.nodes[0];
      if ((double) percent >= 1.0)
        return this.nodes[this.nodes.Length - 1];
      float length = this.lengths[this.lengths.Length - 1];
      float num = length * percent;
      int index = 0;
      while (index < this.lengths.Length - 1 && (double) this.lengths[index + 1] <= (double) num)
        ++index;
      float min = this.lengths[index] / length;
      float max = this.lengths[index + 1] / length;
      float amount = Calc.ClampedMap(percent, min, max, 0.0f, 1f);
      return Vector2.Lerp(this.nodes[index], this.nodes[index + 1], amount);
    }
  }
}

