// Decompiled with JetBrains decompiler
// Type: Celeste.ReflectionHeartStatue
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste
{
  public class ReflectionHeartStatue : Entity
  {
    private static readonly string[] Code = new string[6]
    {
      "U",
      "L",
      "DR",
      "UR",
      "L",
      "UL"
    };
    private List<string> currentInputs = new List<string>();
    private List<ReflectionHeartStatue.Torch> torches = new List<ReflectionHeartStatue.Torch>();
    private const string FlagPrefix = "heartTorch_";
    private Vector2 offset;
    private Vector2[] nodes;
    private DashListener dashListener;
    private bool enabled;

    public ReflectionHeartStatue(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.offset = offset;
      this.nodes = data.Nodes;
      this.Depth = 8999;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Session session = (this.Scene as Level).Session;
      Monocle.Image image1 = new Monocle.Image(GFX.Game["objects/reflectionHeart/statue"]);
      image1.JustifyOrigin(0.5f, 1f);
      --image1.Origin.Y;
      this.Add((Component) image1);
      List<string[]> strArrayList = new List<string[]>();
      strArrayList.Add(ReflectionHeartStatue.Code);
      strArrayList.Add(this.FlipCode(true, false));
      strArrayList.Add(this.FlipCode(false, true));
      strArrayList.Add(this.FlipCode(true, true));
      for (int index = 0; index < 4; ++index)
      {
        ReflectionHeartStatue.Torch torch = new ReflectionHeartStatue.Torch(session, this.offset + this.nodes[index], index, strArrayList[index]);
        this.Scene.Add((Entity) torch);
        this.torches.Add(torch);
      }
      int length = ReflectionHeartStatue.Code.Length;
      Vector2 vector2 = this.nodes[4] + this.offset - this.Position;
      for (int index = 0; index < length; ++index)
      {
        Monocle.Image image2 = new Monocle.Image(GFX.Game["objects/reflectionHeart/gem"]);
        image2.CenterOrigin();
        image2.Color = ForsakenCitySatellite.Colors[ReflectionHeartStatue.Code[index]];
        image2.Position = vector2 + new Vector2((float) (((double) index - (double) (length - 1) / 2.0) * 24.0), 0.0f);
        this.Add((Component) image2);
        this.Add((Component) new BloomPoint(image2.Position, 0.3f, 12f));
      }
      this.enabled = !session.HeartGem;
      if (!this.enabled)
        return;
      this.Add((Component) (this.dashListener = new DashListener()));
      this.dashListener.OnDash = (Action<Vector2>) (dir =>
      {
        string str = "";
        if ((double) dir.Y < 0.0)
          str = "U";
        else if ((double) dir.Y > 0.0)
          str = "D";
        if ((double) dir.X < 0.0)
          str += "L";
        else if ((double) dir.X > 0.0)
          str += "R";
        int num = 0;
        if ((double) dir.X < 0.0 && (double) dir.Y == 0.0)
          num = 1;
        else if ((double) dir.X < 0.0 && (double) dir.Y < 0.0)
          num = 2;
        else if ((double) dir.X == 0.0 && (double) dir.Y < 0.0)
          num = 3;
        else if ((double) dir.X > 0.0 && (double) dir.Y < 0.0)
          num = 4;
        else if ((double) dir.X > 0.0 && (double) dir.Y == 0.0)
          num = 5;
        else if ((double) dir.X > 0.0 && (double) dir.Y > 0.0)
          num = 6;
        else if ((double) dir.X == 0.0 && (double) dir.Y > 0.0)
          num = 7;
        else if ((double) dir.X < 0.0 && (double) dir.Y > 0.0)
          num = 8;
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        Audio.Play("event:/game/06_reflection/supersecret_dashflavour", entity != null ? entity.Position : Vector2.Zero, "dash_direction", (float) num);
        this.currentInputs.Add(str);
        if (this.currentInputs.Count > ReflectionHeartStatue.Code.Length)
          this.currentInputs.RemoveAt(0);
        foreach (ReflectionHeartStatue.Torch torch in this.torches)
        {
          if (!torch.Activated && this.CheckCode(torch.Code))
            torch.Activate();
        }
        this.CheckIfAllActivated(false);
      });
      this.CheckIfAllActivated(true);
    }

    private string[] FlipCode(bool h, bool v)
    {
      string[] strArray = new string[ReflectionHeartStatue.Code.Length];
      for (int index = 0; index < ReflectionHeartStatue.Code.Length; ++index)
      {
        string source = ReflectionHeartStatue.Code[index];
        if (h)
          source = source.Contains<char>('L') ? source.Replace('L', 'R') : source.Replace('R', 'L');
        if (v)
          source = source.Contains<char>('U') ? source.Replace('U', 'D') : source.Replace('D', 'U');
        strArray[index] = source;
      }
      return strArray;
    }

    private bool CheckCode(string[] code)
    {
      if (this.currentInputs.Count < code.Length)
        return false;
      for (int index = 0; index < code.Length; ++index)
      {
        if (!this.currentInputs[index].Equals(code[index]))
          return false;
      }
      return true;
    }

    private void CheckIfAllActivated(bool skipActivateRoutine = false)
    {
      if (!this.enabled)
        return;
      bool flag = true;
      foreach (ReflectionHeartStatue.Torch torch in this.torches)
      {
        if (!torch.Activated)
          flag = false;
      }
      if (flag)
        this.Activate(skipActivateRoutine);
    }

    public void Activate(bool skipActivateRoutine)
    {
      this.enabled = false;
      if (skipActivateRoutine)
        this.Scene.Add((Entity) new HeartGem(this.Position + new Vector2(0.0f, -52f)));
      else
        this.Add((Component) new Coroutine(this.ActivateRoutine(), true));
    }

    private IEnumerator ActivateRoutine()
    {
      yield return (object) 0.533f;
      Audio.Play("event:/game/06_reflection/supersecret_heartappear");
      Entity dummy = new Entity(this.Position + new Vector2(0.0f, -52f));
      dummy.Depth = 1;
      this.Scene.Add(dummy);
      Monocle.Image white = new Monocle.Image(GFX.Game["collectables/heartgem/white00"]);
      white.CenterOrigin();
      white.Scale = Vector2.Zero;
      dummy.Add((Component) white);
      BloomPoint glow = new BloomPoint(0.0f, 16f);
      dummy.Add((Component) glow);
      List<Entity> absorbs = new List<Entity>();
      for (int i = 0; i < 20; ++i)
      {
        AbsorbOrb orb = new AbsorbOrb(this.Position + new Vector2(0.0f, -20f), dummy);
        this.Scene.Add((Entity) orb);
        absorbs.Add((Entity) orb);
        yield return (object) null;
        orb = (AbsorbOrb) null;
      }
      yield return (object) 0.8f;
      float duration = 0.6f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        white.Scale = Vector2.One * p;
        glow.Alpha = p;
        (this.Scene as Level).Shake(0.3f);
        yield return (object) null;
      }
      foreach (Entity entity in absorbs)
      {
        Entity orb = entity;
        orb.RemoveSelf();
        orb = (Entity) null;
      }
      (this.Scene as Level).Flash(Color.White, false);
      this.Scene.Remove(dummy);
      this.Scene.Add((Entity) new HeartGem(this.Position + new Vector2(0.0f, -52f)));
    }

    public override void Update()
    {
      if (this.dashListener != null && !this.enabled)
      {
        this.Remove((Component) this.dashListener);
        this.dashListener = (DashListener) null;
      }
      base.Update();
    }

    public class Torch : Entity
    {
      public string[] Code;
      private Sprite sprite;
      private Session session;

      public string Flag
      {
        get
        {
          return "heartTorch_" + (object) this.Index;
        }
      }

      public bool Activated
      {
        get
        {
          return this.session.GetFlag(this.Flag);
        }
      }

      public int Index { get; private set; }

      public Torch(Session session, Vector2 position, int index, string[] code)
        : base(position)
      {
        this.Index = index;
        this.Code = code;
        this.Depth = 8999;
        this.session = session;
        Monocle.Image image = new Monocle.Image(GFX.Game.GetAtlasSubtextures("objects/reflectionHeart/hint")[index]);
        image.CenterOrigin();
        image.Position = new Vector2(0.0f, 28f);
        this.Add((Component) image);
        this.Add((Component) (this.sprite = new Sprite(GFX.Game, "objects/reflectionHeart/torch")));
        this.sprite.AddLoop("idle", "", 0.0f, new int[1]);
        this.sprite.AddLoop("lit", "", 0.08f, 1, 2, 3, 4, 5, 6);
        this.sprite.Play("idle", false, false);
        this.sprite.Origin = new Vector2(32f, 64f);
      }

      public override void Added(Scene scene)
      {
        base.Added(scene);
        if (!this.Activated)
          return;
        this.PlayLit();
      }

      public void Activate()
      {
        this.session.SetFlag(this.Flag, true);
        Alarm.Set((Entity) this, 0.2f, (Action) (() =>
        {
          Audio.Play("event:/game/06_reflection/supersecret_torch_" + (object) (this.Index + 1), this.Position);
          this.PlayLit();
        }), Alarm.AlarmMode.Oneshot);
      }

      private void PlayLit()
      {
        this.sprite.Play("lit", false, false);
        this.sprite.SetAnimationFrame(Calc.Random.Next(this.sprite.CurrentAnimationTotalFrames));
        this.Add((Component) new VertexLight(Color.LightSeaGreen, 1f, 24, 48));
        this.Add((Component) new BloomPoint(0.6f, 16f));
      }
    }
  }
}

