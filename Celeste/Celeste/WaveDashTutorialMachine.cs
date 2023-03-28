// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashTutorialMachine
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
  public class WaveDashTutorialMachine : JumpThru
  {
    private Entity frontEntity;
    private Monocle.Image backSprite;
    private Monocle.Image frontRightSprite;
    private Monocle.Image frontLeftSprite;
    private Sprite noise;
    private Sprite neon;
    private Solid frontWall;
    private float insideEase;
    private float cameraEase;
    private bool playerInside;
    private bool inCutscene;
    private Coroutine routine;
    private WaveDashPresentation presentation;
    private float interactStartZoom;
    private EventInstance snapshot;
    private EventInstance usingSfx;
    private SoundSource signSfx;
    private TalkComponent talk;

    public WaveDashTutorialMachine(Vector2 position)
      : base(position, 88, true)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = 1000;
      this.Hitbox.Position = new Vector2(-41f, -59f);
      this.Add((Component) (this.backSprite = new Monocle.Image(GFX.Game["objects/wavedashtutorial/building_back"])));
      this.backSprite.JustifyOrigin(0.5f, 1f);
      this.Add((Component) (this.noise = new Sprite(GFX.Game, "objects/wavedashtutorial/noise")));
      this.noise.AddLoop("static", "", 0.05f);
      this.noise.Play("static");
      this.noise.CenterOrigin();
      this.noise.Position = new Vector2(0.0f, -30f);
      this.noise.Color = Color.White * 0.5f;
      this.Add((Component) (this.frontLeftSprite = new Monocle.Image(GFX.Game["objects/wavedashtutorial/building_front_left"])));
      this.frontLeftSprite.JustifyOrigin(0.5f, 1f);
      this.Add((Component) (this.talk = new TalkComponent(new Rectangle(-12, -8, 24, 8), new Vector2(0.0f, -50f), new Action<Player>(this.OnInteract))));
      this.talk.Enabled = false;
      this.SurfaceSoundIndex = 42;
    }

    public WaveDashTutorialMachine(EntityData data, Vector2 position)
      : this(data.Position + position)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add(this.frontEntity = new Entity(this.Position));
      this.frontEntity.Tag = (int) Tags.TransitionUpdate;
      this.frontEntity.Depth = -10500;
      this.frontEntity.Add((Component) (this.frontRightSprite = new Monocle.Image(GFX.Game["objects/wavedashtutorial/building_front_right"])));
      this.frontRightSprite.JustifyOrigin(0.5f, 1f);
      this.frontEntity.Add((Component) (this.neon = new Sprite(GFX.Game, "objects/wavedashtutorial/neon_")));
      this.neon.AddLoop("loop", "", 0.07f);
      this.neon.Play("loop");
      this.neon.JustifyOrigin(0.5f, 1f);
      scene.Add((Entity) (this.frontWall = new Solid(this.Position + new Vector2(-41f, -59f), 88f, 38f, true)));
      this.frontWall.SurfaceSoundIndex = 42;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.Add((Component) (this.signSfx = new SoundSource(new Vector2(8f, -16f), "event:/new_content/env/local/cafe_sign")));
    }

    public override void Update()
    {
      base.Update();
      if (!this.inCutscene)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          this.frontWall.Collidable = true;
          bool flag = (double) entity.X > (double) this.X - 37.0 && (double) entity.X < (double) this.X + 46.0 && (double) entity.Y > (double) this.Y - 58.0 || this.frontWall.CollideCheck((Entity) entity);
          if (flag != this.playerInside)
          {
            this.playerInside = flag;
            if (this.playerInside)
            {
              this.signSfx.Stop();
              this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_inside_cafe");
            }
            else
            {
              this.signSfx.Play("event:/new_content/env/local/cafe_sign");
              Audio.ReleaseSnapshot(this.snapshot);
              this.snapshot = (EventInstance) null;
            }
          }
        }
        this.SceneAs<Level>().ZoomSnap(new Vector2(160f, 90f), (float) (1.0 + (double) Ease.QuadInOut(this.cameraEase) * 0.75));
      }
      this.talk.Enabled = this.playerInside;
      this.frontWall.Collidable = !this.playerInside;
      this.insideEase = Calc.Approach(this.insideEase, this.playerInside ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.cameraEase = Calc.Approach(this.cameraEase, this.playerInside ? 1f : 0.0f, Engine.DeltaTime * 2f);
      this.frontRightSprite.Color = Color.White * (1f - this.insideEase);
      this.frontLeftSprite.Color = this.frontRightSprite.Color;
      this.neon.Color = this.frontRightSprite.Color;
      this.frontRightSprite.Visible = (double) this.insideEase < 1.0;
      this.frontLeftSprite.Visible = (double) this.insideEase < 1.0;
      this.neon.Visible = (double) this.insideEase < 1.0;
      if (!this.Scene.OnInterval(0.05f))
        return;
      this.noise.Scale = Calc.Random.Choose<Vector2>(new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f));
    }

    private void OnInteract(Player player)
    {
      if (this.inCutscene)
        return;
      Level scene = this.Scene as Level;
      if ((HandleBase) this.usingSfx != (HandleBase) null)
      {
        Audio.SetParameter(this.usingSfx, "end", 1f);
        Audio.Stop(this.usingSfx);
      }
      this.inCutscene = true;
      this.interactStartZoom = scene.ZoomTarget;
      scene.StartCutscene(new Action<Level>(this.SkipInteraction), resetZoomOnSkip: false);
      this.Add((Component) (this.routine = new Coroutine(this.InteractRoutine(player))));
    }

    private IEnumerator InteractRoutine(Player player)
    {
      WaveDashTutorialMachine dashTutorialMachine = this;
      Level level = dashTutorialMachine.Scene as Level;
      player.StateMachine.State = 11;
      player.StateMachine.Locked = true;
      yield return (object) CutsceneEntity.CameraTo(new Vector2(dashTutorialMachine.X, dashTutorialMachine.Y - 30f) - new Vector2(160f, 90f), 0.25f, Ease.CubeOut);
      yield return (object) level.ZoomTo(new Vector2(160f, 90f), 10f, 1f);
      dashTutorialMachine.usingSfx = Audio.Play("event:/state/cafe_computer_active", player.Position);
      Audio.Play("event:/new_content/game/10_farewell/cafe_computer_on", player.Position);
      Audio.Play("event:/new_content/game/10_farewell/cafe_computer_startupsfx", player.Position);
      dashTutorialMachine.presentation = new WaveDashPresentation(dashTutorialMachine.usingSfx);
      dashTutorialMachine.Scene.Add((Entity) dashTutorialMachine.presentation);
      while (dashTutorialMachine.presentation.Viewing)
        yield return (object) null;
      yield return (object) level.ZoomTo(new Vector2(160f, 90f), dashTutorialMachine.interactStartZoom, 1f);
      player.StateMachine.Locked = false;
      player.StateMachine.State = 0;
      dashTutorialMachine.inCutscene = false;
      level.EndCutscene();
      Audio.SetAltMusic((string) null);
    }

    private void SkipInteraction(Level level)
    {
      Audio.SetAltMusic((string) null);
      this.inCutscene = false;
      level.ZoomSnap(new Vector2(160f, 90f), this.interactStartZoom);
      if ((HandleBase) this.usingSfx != (HandleBase) null)
      {
        Audio.SetParameter(this.usingSfx, "end", 1f);
        int num = (int) this.usingSfx.release();
      }
      if (this.presentation != null)
        this.presentation.RemoveSelf();
      this.presentation = (WaveDashPresentation) null;
      if (this.routine != null)
        this.routine.RemoveSelf();
      this.routine = (Coroutine) null;
      Player entity = level.Tracker.GetEntity<Player>();
      if (entity == null)
        return;
      entity.StateMachine.Locked = false;
      entity.StateMachine.State = 0;
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.Dispose();
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Dispose();
    }

    private void Dispose()
    {
      if ((HandleBase) this.usingSfx != (HandleBase) null)
      {
        Audio.SetParameter(this.usingSfx, "quit", 1f);
        int num = (int) this.usingSfx.release();
        this.usingSfx = (EventInstance) null;
      }
      Audio.ReleaseSnapshot(this.snapshot);
      this.snapshot = (EventInstance) null;
    }
  }
}
