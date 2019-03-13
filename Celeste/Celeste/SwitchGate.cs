// Decompiled with JetBrains decompiler
// Type: Celeste.SwitchGate
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class SwitchGate : Solid
  {
    private Color inactiveColor = Calc.HexToColor("5fcde4");
    private Color activeColor = Color.White;
    private Color finishColor = Calc.HexToColor("f141df");
    public static ParticleType P_Behind;
    public static ParticleType P_Dust;
    private MTexture[,] nineSlice;
    private Sprite icon;
    private Vector2 iconOffset;
    private Wiggler wiggler;
    private Vector2 node;
    private SoundSource openSfx;
    private bool persistent;

    public SwitchGate(
      Vector2 position,
      float width,
      float height,
      Vector2 node,
      bool persistent,
      string spriteName)
      : base(position, width, height, false)
    {
      this.node = node;
      this.persistent = persistent;
      this.Add((Component) (this.icon = new Sprite(GFX.Game, "objects/switchgate/icon")));
      this.icon.Add("spin", "", 0.1f, "spin");
      this.icon.Play("spin", false, false);
      this.icon.Rate = 0.0f;
      this.icon.Color = this.inactiveColor;
      this.icon.Position = this.iconOffset = new Vector2(width / 2f, height / 2f);
      this.icon.CenterOrigin();
      this.Add((Component) (this.wiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.icon.Scale = Vector2.One * (1f + f)), false, false)));
      MTexture mtexture = GFX.Game["objects/switchgate/" + spriteName];
      this.nineSlice = new MTexture[3, 3];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < 3; ++index2)
          this.nineSlice[index1, index2] = mtexture.GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
      }
      this.Add((Component) (this.openSfx = new SoundSource()));
      this.Add((Component) new LightOcclude(0.5f));
    }

    public SwitchGate(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height, data.Nodes[0] + offset, data.Bool(nameof (persistent), false), data.Attr("sprite", "block"))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (Switch.CheckLevelFlag(this.SceneAs<Level>()))
      {
        this.MoveTo(this.node);
        this.icon.Rate = 0.0f;
        this.icon.SetAnimationFrame(0);
        this.icon.Color = this.finishColor;
      }
      else
        this.Add((Component) new Coroutine(this.Sequence(this.node), true));
    }

    public override void Render()
    {
      float num1 = (float) ((double) this.Collider.Width / 8.0 - 1.0);
      float num2 = (float) ((double) this.Collider.Height / 8.0 - 1.0);
      for (int val1_1 = 0; (double) val1_1 <= (double) num1; ++val1_1)
      {
        for (int val1_2 = 0; (double) val1_2 <= (double) num2; ++val1_2)
          this.nineSlice[(double) val1_1 < (double) num1 ? Math.Min(val1_1, 1) : 2, (double) val1_2 < (double) num2 ? Math.Min(val1_2, 1) : 2].Draw(this.Position + this.Shake + new Vector2((float) (val1_1 * 8), (float) (val1_2 * 8)));
      }
      this.icon.Position = this.iconOffset + this.Shake;
      this.icon.DrawOutline(1);
      base.Render();
    }

    private IEnumerator Sequence(Vector2 node)
    {
      Vector2 start = this.Position;
      while (!Switch.Check(this.Scene))
        yield return (object) null;
      if (this.persistent)
        Switch.SetLevelFlag(this.SceneAs<Level>());
      yield return (object) 0.1f;
      this.openSfx.Play("event:/game/general/touchswitch_gate_open", (string) null, 0.0f);
      this.StartShaking(0.5f);
      while ((double) this.icon.Rate < 1.0)
      {
        this.icon.Color = Color.Lerp(this.inactiveColor, this.activeColor, this.icon.Rate);
        this.icon.Rate += Engine.DeltaTime * 2f;
        yield return (object) null;
      }
      yield return (object) 0.1f;
      int particleAt = 0;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 2f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.MoveTo(Vector2.Lerp(start, node, t.Eased));
        if (!this.Scene.OnInterval(0.1f))
          return;
        ++particleAt;
        particleAt %= 2;
        for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
        {
          for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
          {
            if ((index1 + index2) % 2 == particleAt)
              this.SceneAs<Level>().ParticlesBG.Emit(SwitchGate.P_Behind, this.Position + new Vector2((float) (index1 * 8), (float) (index2 * 8)) + Calc.Random.Range(Vector2.One * 2f, Vector2.One * 6f));
          }
        }
      });
      this.Add((Component) tween);
      yield return (object) 1.8f;
      tween = (Tween) null;
      bool was1 = this.Collidable;
      this.Collidable = false;
      if ((double) node.X <= (double) start.X)
      {
        Vector2 add = new Vector2(0.0f, 2f);
        for (int i = 0; (double) i < (double) this.Height / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Left - 1f, this.Top + 4f + (float) (i * 8));
          Vector2 not = at + Vector2.UnitX;
          if (this.Scene.CollideCheck<Solid>(at) && !this.Scene.CollideCheck<Solid>(not))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at + add, 3.141593f);
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at - add, 3.141593f);
          }
          at = new Vector2();
          not = new Vector2();
        }
        add = new Vector2();
      }
      if ((double) node.X >= (double) start.X)
      {
        Vector2 add = new Vector2(0.0f, 2f);
        for (int i = 0; (double) i < (double) this.Height / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Right + 1f, this.Top + 4f + (float) (i * 8));
          Vector2 not = at - Vector2.UnitX * 2f;
          if (this.Scene.CollideCheck<Solid>(at) && !this.Scene.CollideCheck<Solid>(not))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at + add, 0.0f);
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at - add, 0.0f);
          }
          at = new Vector2();
          not = new Vector2();
        }
        add = new Vector2();
      }
      if ((double) node.Y <= (double) start.Y)
      {
        Vector2 add = new Vector2(2f, 0.0f);
        for (int i = 0; (double) i < (double) this.Width / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Left + 4f + (float) (i * 8), this.Top - 1f);
          Vector2 not = at + Vector2.UnitY;
          if (this.Scene.CollideCheck<Solid>(at) && !this.Scene.CollideCheck<Solid>(not))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at + add, -1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at - add, -1.570796f);
          }
          at = new Vector2();
          not = new Vector2();
        }
        add = new Vector2();
      }
      if ((double) node.Y >= (double) start.Y)
      {
        Vector2 add = new Vector2(2f, 0.0f);
        for (int i = 0; (double) i < (double) this.Width / 8.0; ++i)
        {
          Vector2 at = new Vector2(this.Left + 4f + (float) (i * 8), this.Bottom + 1f);
          Vector2 not = at - Vector2.UnitY * 2f;
          if (this.Scene.CollideCheck<Solid>(at) && !this.Scene.CollideCheck<Solid>(not))
          {
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at + add, 1.570796f);
            this.SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Dust, at - add, 1.570796f);
          }
          at = new Vector2();
          not = new Vector2();
        }
        add = new Vector2();
      }
      this.Collidable = was1;
      Audio.Play("event:/game/general/touchswitch_gate_finish", this.Position);
      this.StartShaking(0.2f);
      while ((double) this.icon.Rate > 0.0)
      {
        this.icon.Color = Color.Lerp(this.activeColor, this.finishColor, 1f - this.icon.Rate);
        this.icon.Rate -= Engine.DeltaTime * 4f;
        yield return (object) null;
      }
      this.icon.Rate = 0.0f;
      this.icon.SetAnimationFrame(0);
      this.wiggler.Start();
      bool was2 = this.Collidable;
      this.Collidable = false;
      if (!this.Scene.CollideCheck<Solid>(this.Center))
      {
        for (int i = 0; i < 32; ++i)
        {
          float angle = Calc.Random.NextFloat(6.283185f);
          this.SceneAs<Level>().ParticlesFG.Emit(TouchSwitch.P_Fire, this.Position + this.iconOffset + Calc.AngleToVector(angle, 4f), angle);
        }
      }
      this.Collidable = was2;
    }
  }
}

