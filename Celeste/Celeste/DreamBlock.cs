// Decompiled with JetBrains decompiler
// Type: Celeste.DreamBlock
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
  public class DreamBlock : Solid
  {
    private static readonly Color activeBackColor = Color.get_Black();
    private static readonly Color disabledBackColor = Calc.HexToColor("1f2e2d");
    private static readonly Color activeLineColor = Color.get_White();
    private static readonly Color disabledLineColor = Calc.HexToColor("6a8480");
    private float whiteHeight = 1f;
    private float wobbleFrom = Calc.Random.NextFloat(6.283185f);
    private float wobbleTo = Calc.Random.NextFloat(6.283185f);
    private bool playerHasDreamDash;
    private Vector2? node;
    private LightOcclude occlude;
    private MTexture[] particleTextures;
    private DreamBlock.DreamParticle[] particles;
    private float whiteFill;
    private Vector2 shake;
    private float animTimer;
    private Shaker shaker;
    private bool fastMoving;
    private bool oneUse;
    private float wobbleEase;

    public DreamBlock(
      Vector2 position,
      float width,
      float height,
      Vector2? node,
      bool fastMoving,
      bool oneUse)
      : base(position, width, height, true)
    {
      this.Depth = -11000;
      this.node = node;
      this.fastMoving = fastMoving;
      this.oneUse = oneUse;
      this.SurfaceSoundIndex = 11;
      this.particleTextures = new MTexture[4]
      {
        GFX.Game["objects/dreamblock/particles"].GetSubtexture(14, 0, 7, 7, (MTexture) null),
        GFX.Game["objects/dreamblock/particles"].GetSubtexture(7, 0, 7, 7, (MTexture) null),
        GFX.Game["objects/dreamblock/particles"].GetSubtexture(0, 0, 7, 7, (MTexture) null),
        GFX.Game["objects/dreamblock/particles"].GetSubtexture(7, 0, 7, 7, (MTexture) null)
      };
    }

    public DreamBlock(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset), (float) data.Width, (float) data.Height, data.FirstNodeNullable(new Vector2?(offset)), data.Bool(nameof (fastMoving), false), data.Bool(nameof (oneUse), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.playerHasDreamDash = this.SceneAs<Level>().Session.Inventory.DreamDash;
      if (this.playerHasDreamDash && this.node.HasValue)
      {
        Vector2 start = this.Position;
        Vector2 end = this.node.Value;
        float duration = Vector2.Distance(start, end) / 12f;
        if (this.fastMoving)
          duration /= 3f;
        Tween tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.SineInOut, duration, true);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          if (this.Collidable)
            this.MoveTo(Vector2.Lerp(start, end, t.Eased));
          else
            this.MoveToNaive(Vector2.Lerp(start, end, t.Eased));
        });
        this.Add((Component) tween);
      }
      if (!this.playerHasDreamDash)
        this.Add((Component) (this.occlude = new LightOcclude(1f)));
      this.Setup();
    }

    public void Setup()
    {
      this.particles = new DreamBlock.DreamParticle[(int) ((double) this.Width / 8.0 * ((double) this.Height / 8.0) * 0.699999988079071)];
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position = new Vector2(Calc.Random.NextFloat(this.Width), Calc.Random.NextFloat(this.Height));
        this.particles[index].Layer = Calc.Random.Choose<int>(0, 1, 1, 2, 2, 2);
        this.particles[index].TimeOffset = Calc.Random.NextFloat();
        this.particles[index].Color = Color.op_Multiply(Color.get_LightGray(), (float) (0.5 + (double) this.particles[index].Layer / 2.0 * 0.5));
        if (this.playerHasDreamDash)
        {
          switch (this.particles[index].Layer)
          {
            case 0:
              this.particles[index].Color = Calc.Random.Choose<Color>(Calc.HexToColor("FFEF11"), Calc.HexToColor("FF00D0"), Calc.HexToColor("08a310"));
              continue;
            case 1:
              this.particles[index].Color = Calc.Random.Choose<Color>(Calc.HexToColor("5fcde4"), Calc.HexToColor("7fb25e"), Calc.HexToColor("E0564C"));
              continue;
            case 2:
              this.particles[index].Color = Calc.Random.Choose<Color>(Calc.HexToColor("5b6ee1"), Calc.HexToColor("CC3B3B"), Calc.HexToColor("7daa64"));
              continue;
            default:
              continue;
          }
        }
      }
    }

    public void OnPlayerExit(Player player)
    {
      Dust.Burst(player.Position, player.Speed.Angle(), 16);
      Vector2 vector2 = Vector2.get_Zero();
      if (this.CollideCheck((Entity) player, Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), 4f))))
        vector2 = Vector2.get_UnitX();
      else if (this.CollideCheck((Entity) player, Vector2.op_Subtraction(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), 4f))))
        vector2 = Vector2.op_UnaryNegation(Vector2.get_UnitX());
      else if (this.CollideCheck((Entity) player, Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), 4f))))
        vector2 = Vector2.get_UnitY();
      else if (this.CollideCheck((Entity) player, Vector2.op_Subtraction(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), 4f))))
        vector2 = Vector2.op_UnaryNegation(Vector2.get_UnitY());
      Vector2.op_Inequality(vector2, Vector2.get_Zero());
      if (!this.oneUse)
        return;
      this.OneUseDestroy();
    }

    private void OneUseDestroy()
    {
      this.Collidable = this.Visible = false;
      this.DisableStaticMovers();
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      if (!this.playerHasDreamDash)
        return;
      this.animTimer += 6f * Engine.DeltaTime;
      this.wobbleEase += Engine.DeltaTime * 2f;
      if ((double) this.wobbleEase > 1.0)
      {
        this.wobbleEase = 0.0f;
        this.wobbleFrom = this.wobbleTo;
        this.wobbleTo = Calc.Random.NextFloat(6.283185f);
      }
      this.SurfaceSoundIndex = 12;
    }

    public bool BlockedCheck()
    {
      TheoCrystal theoCrystal = this.CollideFirst<TheoCrystal>();
      if (theoCrystal != null && !this.TryActorWiggleUp((Entity) theoCrystal))
        return true;
      Player player = this.CollideFirst<Player>();
      return player != null && !this.TryActorWiggleUp((Entity) player);
    }

    private bool TryActorWiggleUp(Entity actor)
    {
      bool collidable = this.Collidable;
      this.Collidable = true;
      for (int index = 1; index <= 4; ++index)
      {
        if (!actor.CollideCheck<Solid>(Vector2.op_Subtraction(actor.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) index))))
        {
          Entity entity = actor;
          entity.Position = Vector2.op_Subtraction(entity.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) index));
          this.Collidable = collidable;
          return true;
        }
      }
      this.Collidable = collidable;
      return false;
    }

    public override void Render()
    {
      Camera camera = this.SceneAs<Level>().Camera;
      if ((double) this.Right < (double) camera.Left || (double) this.Left > (double) camera.Right || ((double) this.Bottom < (double) camera.Top || (double) this.Top > (double) camera.Bottom))
        return;
      Draw.Rect((float) this.shake.X + this.X, (float) this.shake.Y + this.Y, this.Width, this.Height, this.playerHasDreamDash ? DreamBlock.activeBackColor : DreamBlock.disabledBackColor);
      Vector2 position = this.SceneAs<Level>().Camera.Position;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        int layer = this.particles[index].Layer;
        Vector2 vector2 = this.PutInside(Vector2.op_Addition(this.particles[index].Position, Vector2.op_Multiply(position, (float) (0.300000011920929 + 0.25 * (double) layer))));
        Color color = this.particles[index].Color;
        MTexture particleTexture;
        switch (layer)
        {
          case 0:
            particleTexture = this.particleTextures[3 - (int) (((double) this.particles[index].TimeOffset * 4.0 + (double) this.animTimer) % 4.0)];
            break;
          case 1:
            particleTexture = this.particleTextures[1 + (int) (((double) this.particles[index].TimeOffset * 2.0 + (double) this.animTimer) % 2.0)];
            break;
          default:
            particleTexture = this.particleTextures[2];
            break;
        }
        if (vector2.X >= (double) this.X + 2.0 && vector2.Y >= (double) this.Y + 2.0 && (vector2.X < (double) this.Right - 2.0 && vector2.Y < (double) this.Bottom - 2.0))
          particleTexture.DrawCentered(Vector2.op_Addition(vector2, this.shake), color);
      }
      if ((double) this.whiteFill > 0.0)
        Draw.Rect(this.X + (float) this.shake.X, this.Y + (float) this.shake.Y, this.Width, this.Height * this.whiteHeight, Color.op_Multiply(Color.get_White(), this.whiteFill));
      this.WobbleLine(Vector2.op_Addition(this.shake, new Vector2(this.X, this.Y)), Vector2.op_Addition(this.shake, new Vector2(this.X + this.Width, this.Y)), 0.0f);
      this.WobbleLine(Vector2.op_Addition(this.shake, new Vector2(this.X + this.Width, this.Y)), Vector2.op_Addition(this.shake, new Vector2(this.X + this.Width, this.Y + this.Height)), 0.7f);
      this.WobbleLine(Vector2.op_Addition(this.shake, new Vector2(this.X + this.Width, this.Y + this.Height)), Vector2.op_Addition(this.shake, new Vector2(this.X, this.Y + this.Height)), 1.5f);
      this.WobbleLine(Vector2.op_Addition(this.shake, new Vector2(this.X, this.Y + this.Height)), Vector2.op_Addition(this.shake, new Vector2(this.X, this.Y)), 2.5f);
      Draw.Rect(Vector2.op_Addition(this.shake, new Vector2(this.X, this.Y)), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
      Draw.Rect(Vector2.op_Addition(this.shake, new Vector2((float) ((double) this.X + (double) this.Width - 2.0), this.Y)), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
      Draw.Rect(Vector2.op_Addition(this.shake, new Vector2(this.X, (float) ((double) this.Y + (double) this.Height - 2.0))), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
      Draw.Rect(Vector2.op_Addition(this.shake, new Vector2((float) ((double) this.X + (double) this.Width - 2.0), (float) ((double) this.Y + (double) this.Height - 2.0))), 2f, 2f, this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor);
    }

    private Vector2 PutInside(Vector2 pos)
    {
      while (pos.X < (double) this.X)
      {
        ref __Null local = ref pos.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + this.Width;
      }
      while (pos.X > (double) this.X + (double) this.Width)
      {
        ref __Null local = ref pos.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - this.Width;
      }
      while (pos.Y < (double) this.Y)
      {
        ref __Null local = ref pos.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + this.Height;
      }
      while (pos.Y > (double) this.Y + (double) this.Height)
      {
        ref __Null local = ref pos.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - this.Height;
      }
      return pos;
    }

    private void WobbleLine(Vector2 from, Vector2 to, float offset)
    {
      Vector2 vector2_1 = Vector2.op_Subtraction(to, from);
      float num1 = ((Vector2) ref vector2_1).Length();
      Vector2 vector2_2 = Vector2.Normalize(Vector2.op_Subtraction(to, from));
      Vector2 vector2_3;
      ((Vector2) ref vector2_3).\u002Ector((float) vector2_2.Y, (float) -vector2_2.X);
      Color color1 = this.playerHasDreamDash ? DreamBlock.activeLineColor : DreamBlock.disabledLineColor;
      Color color2 = this.playerHasDreamDash ? DreamBlock.activeBackColor : DreamBlock.disabledBackColor;
      if ((double) this.whiteFill > 0.0)
      {
        color1 = Color.Lerp(color1, Color.get_White(), this.whiteFill);
        color2 = Color.Lerp(color2, Color.get_White(), this.whiteFill);
      }
      float num2 = 0.0f;
      int num3 = 16;
      for (int index = 2; (double) index < (double) num1 - 2.0; index += num3)
      {
        float num4 = this.Lerp(this.LineAmplitude(this.wobbleFrom + offset, (float) index), this.LineAmplitude(this.wobbleTo + offset, (float) index), this.wobbleEase);
        if ((double) (index + num3) >= (double) num1)
          num4 = 0.0f;
        float num5 = Math.Min((float) num3, num1 - 2f - (float) index);
        Vector2 start = Vector2.op_Addition(Vector2.op_Addition(from, Vector2.op_Multiply(vector2_2, (float) index)), Vector2.op_Multiply(vector2_3, num2));
        Vector2 end = Vector2.op_Addition(Vector2.op_Addition(from, Vector2.op_Multiply(vector2_2, (float) index + num5)), Vector2.op_Multiply(vector2_3, num4));
        Draw.Line(Vector2.op_Subtraction(start, vector2_3), Vector2.op_Subtraction(end, vector2_3), color2);
        Draw.Line(Vector2.op_Subtraction(start, Vector2.op_Multiply(vector2_3, 2f)), Vector2.op_Subtraction(end, Vector2.op_Multiply(vector2_3, 2f)), color2);
        Draw.Line(start, end, color1);
        num2 = num4;
      }
    }

    private float LineAmplitude(float seed, float index)
    {
      return (float) (Math.Sin((double) seed + (double) index / 16.0 + Math.Sin((double) seed * 2.0 + (double) index / 32.0) * 6.28318548202515) + 1.0) * 1.5f;
    }

    private float Lerp(float a, float b, float percent)
    {
      return a + (b - a) * percent;
    }

    public IEnumerator Activate()
    {
      DreamBlock dreamBlock = this;
      Level level = dreamBlock.SceneAs<Level>();
      yield return (object) 1f;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      // ISSUE: reference to a compiler-generated method
      dreamBlock.Add((Component) (dreamBlock.shaker = new Shaker(true, new Action<Vector2>(dreamBlock.\u003CActivate\u003Eb__34_0))));
      dreamBlock.shaker.Interval = 0.02f;
      dreamBlock.shaker.On = true;
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        dreamBlock.whiteFill = Ease.CubeIn(p);
        yield return (object) null;
      }
      dreamBlock.shaker.On = false;
      yield return (object) 0.5f;
      dreamBlock.ActivateNoRoutine();
      dreamBlock.whiteHeight = 1f;
      dreamBlock.whiteFill = 1f;
      for (p = 1f; (double) p > 0.0; p -= Engine.DeltaTime * 0.5f)
      {
        dreamBlock.whiteHeight = p;
        if (level.OnInterval(0.1f))
        {
          for (int index = 0; (double) index < (double) dreamBlock.Width; index += 4)
            level.ParticlesFG.Emit(Strawberry.P_WingsBurst, new Vector2(dreamBlock.X + (float) index, (float) ((double) dreamBlock.Y + (double) dreamBlock.Height * (double) dreamBlock.whiteHeight + 1.0)));
        }
        if (level.OnInterval(0.1f))
          level.Shake(0.3f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
        yield return (object) null;
      }
      while ((double) dreamBlock.whiteFill > 0.0)
      {
        dreamBlock.whiteFill -= Engine.DeltaTime * 3f;
        yield return (object) null;
      }
    }

    public void ActivateNoRoutine()
    {
      if (this.playerHasDreamDash)
        return;
      this.playerHasDreamDash = true;
      this.Setup();
      this.Remove((Component) this.occlude);
      this.whiteHeight = 0.0f;
      this.whiteFill = 0.0f;
      if (this.shaker == null)
        return;
      this.shaker.On = false;
    }

    public void FootstepRipple(Vector2 position)
    {
      if (!this.playerHasDreamDash)
        return;
      DisplacementRenderer.Burst burst = (this.Scene as Level).Displacement.AddBurst(position, 0.5f, 0.0f, 40f, 1f, (Ease.Easer) null, (Ease.Easer) null);
      burst.WorldClipCollider = this.Collider;
      burst.WorldClipPadding = 1;
    }

    private struct DreamParticle
    {
      public Vector2 Position;
      public int Layer;
      public Color Color;
      public float TimeOffset;
    }
  }
}
