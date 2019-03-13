// Decompiled with JetBrains decompiler
// Type: Celeste.Cassette
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class Cassette : Entity
  {
    public static ParticleType P_Shine;
    public static ParticleType P_Collect;
    public bool IsGhost;
    private Sprite sprite;
    private SineWave hover;
    private BloomPoint bloom;
    private VertexLight light;
    private Wiggler scaleWiggler;
    private bool collected;
    private Vector2[] nodes;
    private EventInstance remixSfx;
    private bool collecting;

    public Cassette(Vector2 position, Vector2[] nodes)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox(16f, 16f, -8f, -8f);
      this.nodes = nodes;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public Cassette(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.NodesOffset(offset))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.IsGhost = SaveData.Instance.Areas[this.SceneAs<Level>().Session.Area.ID].Cassette;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create(this.IsGhost ? "cassetteGhost" : "cassette")));
      this.sprite.Play("idle", false, false);
      this.Add((Component) (this.scaleWiggler = Wiggler.Create(0.25f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.25)), false, false)));
      this.Add((Component) (this.bloom = new BloomPoint(0.25f, 16f)));
      this.Add((Component) (this.light = new VertexLight(Color.White, 0.4f, 32, 64)));
      this.Add((Component) (this.hover = new SineWave(0.5f)));
      this.hover.OnUpdate = (Action<float>) (f => this.sprite.Y = this.light.Y = this.bloom.Y = f * 2f);
      if (!this.IsGhost)
        return;
      this.sprite.Color = Color.White * 0.8f;
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      Audio.Stop(this.remixSfx, true);
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      Audio.Stop(this.remixSfx, true);
    }

    public override void Update()
    {
      base.Update();
      if (this.collecting || !this.Scene.OnInterval(0.1f))
        return;
      this.SceneAs<Level>().Particles.Emit(Cassette.P_Shine, 1, this.Center, new Vector2(12f, 10f));
    }

    private void OnPlayer(Player player)
    {
      if (this.collected)
        return;
      if (player != null)
        player.RefillStamina();
      Audio.Play("event:/game/general/cassette_get", this.Position);
      this.collected = true;
      Celeste.Freeze(0.1f);
      this.Add((Component) new Coroutine(this.CollectRoutine(player), true));
    }

    private IEnumerator CollectRoutine(Player player)
    {
      this.collecting = true;
      Level level = this.Scene as Level;
      CassetteBlockManager cbm = this.Scene.Tracker.GetEntity<CassetteBlockManager>();
      level.PauseLock = true;
      level.Frozen = true;
      this.Tag = (int) Tags.FrozenUpdate;
      level.Session.Cassette = true;
      level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(this.nodes[1]));
      level.Session.UpdateLevelStartDashes();
      SaveData.Instance.RegisterCassette(level.Session.Area);
      if (cbm != null)
        cbm.StopBlocks();
      this.Depth = -1000000;
      level.Shake(0.3f);
      level.Flash(Color.White, false);
      level.Displacement.Clear();
      Vector2 camWas = level.Camera.Position;
      Vector2 val = this.Position - new Vector2(160f, 90f);
      double num1 = (double) (level.Bounds.Left - 64);
      double num2 = (double) (level.Bounds.Top - 32);
      Rectangle bounds = level.Bounds;
      double num3 = (double) (bounds.Right + 64 - 320);
      bounds = level.Bounds;
      double num4 = (double) (bounds.Bottom + 32 - 180);
      Vector2 camTo = val.Clamp((float) num1, (float) num2, (float) num3, (float) num4);
      level.Camera.Position = camTo;
      level.ZoomSnap((this.Position - level.Camera.Position).Clamp(60f, 60f, 260f, 120f), 2f);
      this.sprite.Play("spin", true, false);
      this.sprite.Rate = 2f;
      for (float p = 0.0f; (double) p < 1.5; p += Engine.DeltaTime)
      {
        this.sprite.Rate += Engine.DeltaTime * 4f;
        yield return (object) null;
      }
      this.sprite.Rate = 0.0f;
      this.sprite.SetAnimationFrame(0);
      this.scaleWiggler.Start();
      yield return (object) 0.25f;
      Vector2 from = this.Position;
      Vector2 to = new Vector2(this.X, level.Camera.Top - 16f);
      float duration1 = 0.4f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration1)
      {
        this.sprite.Scale.X = MathHelper.Lerp(1f, 0.1f, p);
        this.sprite.Scale.Y = MathHelper.Lerp(1f, 3f, p);
        this.Position = Vector2.Lerp(from, to, Ease.CubeIn(p));
        yield return (object) null;
      }
      this.Visible = false;
      from = new Vector2();
      to = new Vector2();
      this.remixSfx = Audio.Play("event:/game/general/cassette_preview", "remix", (float) level.Session.Area.ID);
      Cassette.UnlockedBSide message = new Cassette.UnlockedBSide();
      this.Scene.Add((Entity) message);
      yield return (object) message.EaseIn();
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      Audio.SetParameter(this.remixSfx, "end", 1f);
      yield return (object) message.EaseOut();
      message = (Cassette.UnlockedBSide) null;
      float duration2 = 0.25f;
      this.Add((Component) new Coroutine(level.ZoomBack(duration2 - 0.05f), true));
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration2)
      {
        level.Camera.Position = Vector2.Lerp(camTo, camWas, Ease.SineInOut(p));
        yield return (object) null;
      }
      if (!player.Dead && this.nodes != null && this.nodes.Length >= 2)
      {
        Audio.Play("event:/game/general/cassette_bubblereturn", level.Camera.Position + new Vector2(160f, 90f));
        player.StartCassetteFly(this.nodes[1], this.nodes[0]);
      }
      List<SandwichLava> lavas = level.Entities.FindAll<SandwichLava>();
      foreach (SandwichLava sandwichLava in lavas)
      {
        SandwichLava lava = sandwichLava;
        lava.Leave();
        lava = (SandwichLava) null;
      }
      level.Frozen = false;
      yield return (object) 0.25f;
      if (cbm != null)
        cbm.Finish();
      level.PauseLock = false;
      level.ResetZoom();
      this.RemoveSelf();
    }

    private class UnlockedBSide : Entity
    {
      private float alpha;
      private string text;
      private bool waitForKeyPress;
      private float timer;

      public override void Added(Scene scene)
      {
        base.Added(scene);
        this.Tag = (int) Tags.HUD | (int) Tags.PauseUpdate;
        this.text = ActiveFont.FontSize.AutoNewline(Dialog.Clean("UI_REMIX_UNLOCKED", (Language) null), 900);
        this.Depth = -10000;
      }

      public IEnumerator EaseIn()
      {
        Level level = this.Scene as Level;
        while ((double) (this.alpha += Engine.DeltaTime / 0.5f) < 1.0)
          yield return (object) null;
        this.alpha = 1f;
        yield return (object) 1.5f;
        this.waitForKeyPress = true;
      }

      public IEnumerator EaseOut()
      {
        this.waitForKeyPress = false;
        while ((double) (this.alpha -= Engine.DeltaTime / 0.5f) > 0.0)
          yield return (object) null;
        this.alpha = 0.0f;
        this.RemoveSelf();
      }

      public override void Update()
      {
        this.timer += Engine.DeltaTime;
        base.Update();
      }

      public override void Render()
      {
        float num = Ease.CubeOut(this.alpha);
        Vector2 vector2_1 = Celeste.TargetCenter + new Vector2(0.0f, 64f);
        Vector2 vector2_2 = Vector2.UnitY * 64f * (1f - num);
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * num * 0.8f);
        GFX.Gui["collectables/cassette"].DrawJustified(vector2_1 - vector2_2 + new Vector2(0.0f, 32f), new Vector2(0.5f, 1f), Color.White * num);
        ActiveFont.Draw(this.text, vector2_1 + vector2_2, new Vector2(0.5f, 0.0f), Vector2.One, Color.White * num);
        if (!this.waitForKeyPress)
          return;
        GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1824f, (float) (984 + ((double) this.timer % 1.0 < 0.25 ? 6 : 0))));
      }
    }
  }
}

