// Decompiled with JetBrains decompiler
// Type: Celeste.FallingBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
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
      this.Add((Component) new Coroutine(this.Sequence()));
      this.Add((Component) new LightOcclude());
      this.Add((Component) new TileInterceptor(this.tiles, false));
      this.TileType = tile;
      this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tile];
      if (!behind)
        return;
      this.Depth = 5000;
    }

    public FallingBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Char("tiletype", '3'), data.Width, data.Height, false, data.Bool("behind"), data.Bool(nameof (climbFall), true))
    {
    }

    public static FallingBlock CreateFinalBossBlock(EntityData data, Vector2 offset) => new FallingBlock(data.Position + offset, 'g', data.Width, data.Height, true, false, false);

    public override void OnShake(Vector2 amount)
    {
      base.OnShake(amount);
      this.tiles.Position += amount;
      if (this.highlight == null)
        return;
      this.highlight.Position += amount;
    }

    public override void OnStaticMoverTrigger(StaticMover sm)
    {
      if (this.finalBoss)
        return;
      this.Triggered = true;
    }

    public bool HasStartedFalling { get; private set; }

    private bool PlayerFallCheck() => this.climbFall ? this.HasPlayerRider() : this.HasPlayerOnTop();

    private bool PlayerWaitCheck()
    {
      if (this.Triggered || this.PlayerFallCheck())
        return true;
      if (!this.climbFall)
        return false;
      return this.CollideCheck<Player>(this.Position - Vector2.UnitX) || this.CollideCheck<Player>(this.Position + Vector2.UnitX);
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
      fallingBlock.StartShaking();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      if (fallingBlock.finalBoss)
        fallingBlock.Add((Component) new Coroutine(fallingBlock.HighlightFade(1f)));
      yield return (object) 0.2f;
      float timer = 0.4f;
      if (fallingBlock.finalBoss)
        timer = 0.2f;
      for (; (double) timer > 0.0 && fallingBlock.PlayerWaitCheck(); timer -= Engine.DeltaTime)
        yield return (object) null;
      fallingBlock.StopShaking();
      for (int x = 2; (double) x < (double) fallingBlock.Width; x += 4)
      {
        if (fallingBlock.Scene.CollideCheck<Solid>(fallingBlock.TopLeft + new Vector2((float) x, -2f)))
          fallingBlock.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustA, 2, new Vector2(fallingBlock.X + (float) x, fallingBlock.Y), Vector2.One * 4f, 1.5707964f);
        fallingBlock.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustB, 2, new Vector2(fallingBlock.X + (float) x, fallingBlock.Y), Vector2.One * 4f);
      }
      float speed = 0.0f;
      float maxSpeed = fallingBlock.finalBoss ? 130f : 160f;
      Level level;
      while (true)
      {
        level = fallingBlock.SceneAs<Level>();
        speed = Calc.Approach(speed, maxSpeed, 500f * Engine.DeltaTime);
        if (!fallingBlock.MoveVCollideSolids(speed * Engine.DeltaTime, true))
        {
          if ((double) fallingBlock.Top <= (double) (level.Bounds.Bottom + 16) && ((double) fallingBlock.Top <= (double) (level.Bounds.Bottom - 1) || !fallingBlock.CollideCheck<Solid>(fallingBlock.Position + new Vector2(0.0f, 1f))))
          {
            yield return (object) null;
            level = (Level) null;
          }
          else
            goto label_23;
        }
        else
          break;
      }
      fallingBlock.ImpactSfx();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      fallingBlock.SceneAs<Level>().DirectionalShake(Vector2.UnitY, fallingBlock.finalBoss ? 0.2f : 0.3f);
      if (fallingBlock.finalBoss)
        fallingBlock.Add((Component) new Coroutine(fallingBlock.HighlightFade(0.0f)));
      fallingBlock.StartShaking();
      fallingBlock.LandParticles();
      yield return (object) 0.2f;
      fallingBlock.StopShaking();
      if (fallingBlock.CollideCheck<SolidTiles>(fallingBlock.Position + new Vector2(0.0f, 1f)))
      {
        fallingBlock.Safe = true;
        yield break;
      }
      else
      {
        while (fallingBlock.CollideCheck<Platform>(fallingBlock.Position + new Vector2(0.0f, 1f)))
          yield return (object) 0.1f;
        goto label_6;
      }
label_23:
      fallingBlock.Collidable = fallingBlock.Visible = false;
      yield return (object) 0.2f;
      if (level.Session.MapData.CanTransitionTo(level, new Vector2(fallingBlock.Center.X, fallingBlock.Bottom + 12f)))
      {
        yield return (object) 0.2f;
        fallingBlock.SceneAs<Level>().Shake();
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
      for (int x = 2; (double) x <= (double) this.Width; x += 4)
      {
        if (this.Scene.CollideCheck<Solid>(this.BottomLeft + new Vector2((float) x, 3f)))
        {
          this.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_FallDustA, 1, new Vector2(this.X + (float) x, this.Bottom), Vector2.One * 4f, -1.5707964f);
          float direction = (double) x >= (double) this.Width / 2.0 ? 0.0f : 3.1415927f;
          this.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_LandDust, 1, new Vector2(this.X + (float) x, this.Bottom), Vector2.One * 4f, direction);
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
