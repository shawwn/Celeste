// Decompiled with JetBrains decompiler
// Type: Celeste.Cassette
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

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
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
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
      this.sprite.Play("idle");
      this.Add((Component) (this.scaleWiggler = Wiggler.Create(0.25f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.25)))));
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
      Audio.Stop(this.remixSfx);
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      Audio.Stop(this.remixSfx);
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
      player?.RefillStamina();
      Audio.Play("event:/game/general/cassette_get", this.Position);
      this.collected = true;
      Celeste.Freeze(0.1f);
      this.Add((Component) new Coroutine(this.CollectRoutine(player)));
    }

    private IEnumerator CollectRoutine(Player player)
    {
      Cassette cassette = this;
      cassette.collecting = true;
      Level level = cassette.Scene as Level;
      CassetteBlockManager cbm = cassette.Scene.Tracker.GetEntity<CassetteBlockManager>();
      level.PauseLock = true;
      level.Frozen = true;
      cassette.Tag = (int) Tags.FrozenUpdate;
      level.Session.Cassette = true;
      level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(cassette.nodes[1]));
      level.Session.UpdateLevelStartDashes();
      SaveData.Instance.RegisterCassette(level.Session.Area);
      cbm?.StopBlocks();
      cassette.Depth = -1000000;
      level.Shake();
      level.Flash(Color.White);
      level.Displacement.Clear();
      Vector2 camWas = level.Camera.Position;
      Vector2 camTo = (cassette.Position - new Vector2(160f, 90f)).Clamp((float) (level.Bounds.Left - 64), (float) (level.Bounds.Top - 32), (float) (level.Bounds.Right + 64 - 320), (float) (level.Bounds.Bottom + 32 - 180));
      level.Camera.Position = camTo;
      level.ZoomSnap((cassette.Position - level.Camera.Position).Clamp(60f, 60f, 260f, 120f), 2f);
      cassette.sprite.Play("spin", true);
      cassette.sprite.Rate = 2f;
      float p;
      for (p = 0.0f; (double) p < 1.5; p += Engine.DeltaTime)
      {
        cassette.sprite.Rate += Engine.DeltaTime * 4f;
        yield return (object) null;
      }
      cassette.sprite.Rate = 0.0f;
      cassette.sprite.SetAnimationFrame(0);
      cassette.scaleWiggler.Start();
      yield return (object) 0.25f;
      Vector2 from = cassette.Position;
      Vector2 to = new Vector2(cassette.X, level.Camera.Top - 16f);
      float duration = 0.4f;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        cassette.sprite.Scale.X = MathHelper.Lerp(1f, 0.1f, p);
        cassette.sprite.Scale.Y = MathHelper.Lerp(1f, 3f, p);
        cassette.Position = Vector2.Lerp(from, to, Ease.CubeIn(p));
        yield return (object) null;
      }
      cassette.Visible = false;
      from = new Vector2();
      to = new Vector2();
      cassette.remixSfx = Audio.Play("event:/game/general/cassette_preview", "remix", (float) level.Session.Area.ID);
      Cassette.UnlockedBSide message = new Cassette.UnlockedBSide();
      cassette.Scene.Add((Entity) message);
      yield return (object) message.EaseIn();
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      Audio.SetParameter(cassette.remixSfx, "end", 1f);
      yield return (object) message.EaseOut();
      message = (Cassette.UnlockedBSide) null;
      duration = 0.25f;
      cassette.Add((Component) new Coroutine(level.ZoomBack(duration - 0.05f)));
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        level.Camera.Position = Vector2.Lerp(camTo, camWas, Ease.SineInOut(p));
        yield return (object) null;
      }
      if (!player.Dead && cassette.nodes != null && cassette.nodes.Length >= 2)
      {
        Audio.Play("event:/game/general/cassette_bubblereturn", level.Camera.Position + new Vector2(160f, 90f));
        player.StartCassetteFly(cassette.nodes[1], cassette.nodes[0]);
      }
      foreach (SandwichLava sandwichLava in level.Entities.FindAll<SandwichLava>())
        sandwichLava.Leave();
      level.Frozen = false;
      yield return (object) 0.25f;
      cbm?.Finish();
      level.PauseLock = false;
      level.ResetZoom();
      cassette.RemoveSelf();
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
        this.text = ActiveFont.FontSize.AutoNewline(Dialog.Clean("UI_REMIX_UNLOCKED"), 900);
        this.Depth = -10000;
      }

      public IEnumerator EaseIn()
      {
        Cassette.UnlockedBSide unlockedBside = this;
        Scene scene = unlockedBside.Scene;
        while ((double) (unlockedBside.alpha += Engine.DeltaTime / 0.5f) < 1.0)
          yield return (object) null;
        unlockedBside.alpha = 1f;
        yield return (object) 1.5f;
        unlockedBside.waitForKeyPress = true;
      }

      public IEnumerator EaseOut()
      {
        Cassette.UnlockedBSide unlockedBside = this;
        unlockedBside.waitForKeyPress = false;
        while ((double) (unlockedBside.alpha -= Engine.DeltaTime / 0.5f) > 0.0)
          yield return (object) null;
        unlockedBside.alpha = 0.0f;
        unlockedBside.RemoveSelf();
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
