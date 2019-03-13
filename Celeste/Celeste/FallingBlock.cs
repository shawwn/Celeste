// Decompiled with JetBrains decompiler
// Type: Celeste.FallingBlock
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
  public class FallingBlock : Solid
  {
    public static ParticleType P_FallDustA;
    public static ParticleType P_FallDustB;
    public static ParticleType P_LandDust;
    public bool Triggered;
    public float FallDelay;
    private char TileType;
    private TileGrid tiles;
    private TileGrid highlight;
    private bool finalBoss;
    private bool climbFall;

    public FallingBlock(
      Vector2 position,
      char tile,
      int width,
      int height,
      bool finalBoss,
      bool behind,
      bool climbFall)
      : base(position, (float) width, (float) height, false)
    {
      this.finalBoss = finalBoss;
      this.climbFall = climbFall;
      int newSeed = Calc.Random.Next();
      Calc.PushRandom(newSeed);
      this.Add((Component) (this.tiles = GFX.FGAutotiler.GenerateBox(tile, width / 8, height / 8).TileGrid));
      Calc.PopRandom();
      if (finalBoss)
      {
        Calc.PushRandom(newSeed);
        this.Add((Component) (this.highlight = GFX.FGAutotiler.GenerateBox('G', width / 8, height / 8).TileGrid));
        Calc.PopRandom();
        this.highlight.Alpha = 0.0f;
      }
      this.Add((Component) new Coroutine(this.Sequence(), true));
      this.Add((Component) new LightOcclude(1f));
      this.Add((Component) new TileInterceptor(this.tiles, false));
      this.TileType = tile;
      this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tile];
      if (!behind)
        return;
      this.Depth = 5000;
    }

    public FallingBlock(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), data.Char("tiletype", '3'), data.Width, data.Height, false, data.Bool("behind", false), data.Bool(nameof (climbFall), true))
    {
    }

    public static FallingBlock CreateFinalBossBlock(EntityData data, Vector2 offset)
    {
      return new FallingBlock(Vector2.op_Addition(data.Position, offset), 'g', data.Width, data.Height, true, false, false);
    }

    public override void OnShake(Vector2 amount)
    {
      base.OnShake(amount);
      TileGrid tiles = this.tiles;
      tiles.Position = Vector2.op_Addition(tiles.Position, amount);
      if (this.highlight == null)
        return;
      TileGrid highlight = this.highlight;
      highlight.Position = Vector2.op_Addition(highlight.Position, amount);
    }

    public override void OnStaticMoverTrigger()
    {
      if (this.finalBoss)
        return;
      this.Triggered = true;
    }

    public bool HasStartedFalling { get; private set; }

    private bool PlayerFallCheck()
    {
      if (this.climbFall)
        return this.HasPlayerRider();
      return this.HasPlayerOnTop();
    }

    private bool PlayerWaitCheck()
    {
      if (this.Triggered || this.PlayerFallCheck())
        return true;
      if (!this.climbFall)
        return false;
      if (!this.CollideCheck<Player>(Vector2.op_Subtraction(this.Position, Vector2.get_UnitX())))
        return this.CollideCheck<Player>(Vector2.op_Addition(this.Position, Vector2.get_UnitX()));
      return true;
    }

    private IEnumerator Sequence()
    {
      FallingBlock fallingBlock = this;
      while (!fallingBlock.Triggered && (fallingBlock.finalBoss || !fallingBlock.PlayerFallCheck()))
        yield return (object) null;
      while ((double) fallingBlock.FallDelay > 0.0)
      {
        fallingBlock.FallDelay -= Engine.DeltaTime;
        yield return (object) null;
      }
      fallingBlock.HasStartedFalling = true;
label_6:
      fallingBlock.ShakeSfx();
      fallingBlock.StartShaking(0.0f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      if (fallingBlock.finalBoss)
        fallingBlock.Add((Component) new Coroutine(fallingBlock.HighlightFade(1f), true));
      yield return (object) 0.2f;
      float timer = 0.4f;
      if (fallingBlock.finalBoss)
        timer = 0.2f;
      for (; (double) timer > 0.0 && fallingBlock.PlayerWaitCheck(); timer -= Engine.DeltaTime)
        yield return (object) null;
      fallingBlock.StopShaking();
      for (int index = 2; (double) index < (double) fallingBlock.Width; index += 4)
      {
        if (fallingBlock.Scene.CollideCheck<Solid>(Vector2.op_Addition(fallingBlock.TopLeft, new Vector2((float) index, -2f))))
          fallingBlock.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustA, 2, new Vector2(fallingBlock.X + (float) index, fallingBlock.Y), Vector2.op_Multiply(Vector2.get_One(), 4f), 1.570796f);
        fallingBlock.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustB, 2, new Vector2(fallingBlock.X + (float) index, fallingBlock.Y), Vector2.op_Multiply(Vector2.get_One(), 4f));
      }
      float speed = 0.0f;
      float maxSpeed = fallingBlock.finalBoss ? 130f : 160f;
      Level level;
      while (true)
      {
        level = fallingBlock.SceneAs<Level>();
        speed = Calc.Approach(speed, maxSpeed, 500f * Engine.DeltaTime);
        if (!fallingBlock.MoveVCollideSolids(speed * Engine.DeltaTime, true, (Action<Vector2, Vector2, Platform>) null))
        {
          double top1 = (double) fallingBlock.Top;
          Rectangle bounds1 = level.Bounds;
          double num1 = (double) (((Rectangle) ref bounds1).get_Bottom() + 16);
          if (top1 <= num1)
          {
            double top2 = (double) fallingBlock.Top;
            Rectangle bounds2 = level.Bounds;
            double num2 = (double) (((Rectangle) ref bounds2).get_Bottom() - 1);
            if (top2 <= num2 || !fallingBlock.CollideCheck<Solid>(Vector2.op_Addition(fallingBlock.Position, new Vector2(0.0f, 1f))))
            {
              yield return (object) null;
              level = (Level) null;
            }
            else
              goto label_24;
          }
          else
            goto label_24;
        }
        else
          break;
      }
      fallingBlock.ImpactSfx();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      fallingBlock.SceneAs<Level>().DirectionalShake(Vector2.get_UnitY(), fallingBlock.finalBoss ? 0.2f : 0.3f);
      if (fallingBlock.finalBoss)
        fallingBlock.Add((Component) new Coroutine(fallingBlock.HighlightFade(0.0f), true));
      fallingBlock.StartShaking(0.0f);
      fallingBlock.LandParticles();
      yield return (object) 0.2f;
      fallingBlock.StopShaking();
      if (fallingBlock.CollideCheck<SolidTiles>(Vector2.op_Addition(fallingBlock.Position, new Vector2(0.0f, 1f))))
      {
        fallingBlock.Safe = true;
        yield break;
      }
      else
      {
        while (fallingBlock.CollideCheck<Platform>(Vector2.op_Addition(fallingBlock.Position, new Vector2(0.0f, 1f))))
          yield return (object) 0.1f;
        goto label_6;
      }
label_24:
      fallingBlock.Collidable = fallingBlock.Visible = false;
      yield return (object) 0.2f;
      if (level.Session.MapData.CanTransitionTo(level, new Vector2((float) fallingBlock.Center.X, fallingBlock.Bottom + 12f)))
      {
        yield return (object) 0.2f;
        fallingBlock.SceneAs<Level>().Shake(0.3f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      }
      fallingBlock.RemoveSelf();
      fallingBlock.DestroyStaticMovers();
    }

    private IEnumerator HighlightFade(float to)
    {
      float from = this.highlight.Alpha;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        this.highlight.Alpha = MathHelper.Lerp(from, to, Ease.CubeInOut(p));
        this.tiles.Alpha = 1f - this.highlight.Alpha;
        yield return (object) null;
      }
      this.highlight.Alpha = to;
      this.tiles.Alpha = 1f - to;
    }

    private void LandParticles()
    {
      for (int index = 2; (double) index <= (double) this.Width; index += 4)
      {
        if (this.Scene.CollideCheck<Solid>(Vector2.op_Addition(this.BottomLeft, new Vector2((float) index, 3f))))
        {
          this.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_FallDustA, 1, new Vector2(this.X + (float) index, this.Bottom), Vector2.op_Multiply(Vector2.get_One(), 4f), -1.570796f);
          float direction = (double) index >= (double) this.Width / 2.0 ? 0.0f : 3.141593f;
          this.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_LandDust, 1, new Vector2(this.X + (float) index, this.Bottom), Vector2.op_Multiply(Vector2.get_One(), 4f), direction);
        }
      }
    }

    private void ShakeSfx()
    {
      if (this.TileType == '3')
        Audio.Play("event:/game/01_forsaken_city/fallblock_ice_shake", this.Center);
      else if (this.TileType == '9')
        Audio.Play("event:/game/03_resort/fallblock_wood_shake", this.Center);
      else if (this.TileType == 'g')
        Audio.Play("event:/game/06_reflection/fallblock_boss_shake", this.Center);
      else
        Audio.Play("event:/game/general/fallblock_shake", this.Center);
    }

    private void ImpactSfx()
    {
      if (this.TileType == '3')
        Audio.Play("event:/game/01_forsaken_city/fallblock_ice_impact", this.BottomCenter);
      else if (this.TileType == '9')
        Audio.Play("event:/game/03_resort/fallblock_wood_impact", this.BottomCenter);
      else if (this.TileType == 'g')
        Audio.Play("event:/game/06_reflection/fallblock_boss_impact", this.BottomCenter);
      else
        Audio.Play("event:/game/general/fallblock_impact", this.BottomCenter);
    }
  }
}
