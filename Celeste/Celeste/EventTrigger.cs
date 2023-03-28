// Decompiled with JetBrains decompiler
// Type: Celeste.EventTrigger
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
  public class EventTrigger : Trigger
  {
    public string Event;
    public bool OnSpawnHack;
    private bool triggered;
    private EventInstance snapshot;

    public EventTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Event = data.Attr("event");
      this.OnSpawnHack = data.Bool("onSpawn");
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.OnSpawnHack)
      {
        Player player = this.CollideFirst<Player>();
        if (player != null)
          this.OnEnter(player);
      }
      if (!(this.Event == "ch9_badeline_helps"))
        return;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || (double) entity.Left <= (double) this.Right)
        return;
      this.RemoveSelf();
    }

    public float Time { get; private set; }

    public override void OnEnter(Player player)
    {
      if (this.triggered)
        return;
      this.triggered = true;
      Level level = this.Scene as Level;
      switch (this.Event)
      {
        case "cancel_ch5_see_theo":
          level.Session.SetFlag("it_ch5_see_theo");
          level.Session.SetFlag("it_ch5_see_theo_b");
          level.Session.SetFlag("ignore_darkness_" + level.Session.Level);
          this.Add((Component) new Coroutine(this.Brighten()));
          break;
        case "ch5_found_theo":
          if (level.Session.GetFlag("foundTheoInCrystal"))
            break;
          this.Scene.Add((Entity) new CS05_SaveTheo(player));
          break;
        case "ch5_mirror_reflection":
          if (level.Session.GetFlag("reflection"))
            break;
          this.Scene.Add((Entity) new CS05_Reflection1(player));
          break;
        case "ch5_see_theo":
          if ((this.Scene as Level).Session.GetFlag("seeTheoInCrystal"))
            break;
          this.Scene.Add((Entity) new CS05_SeeTheo(player, 0));
          break;
        case "ch6_boss_intro":
          if (level.Session.GetFlag("boss_intro"))
            break;
          level.Add((Entity) new CS06_BossIntro(this.Center.X, player, level.Entities.FindFirst<FinalBoss>()));
          break;
        case "ch6_reflect":
          if (level.Session.GetFlag("reflection"))
            break;
          this.Scene.Add((Entity) new CS06_Reflection(player, this.Center.X - 5f));
          break;
        case "ch7_summit":
          this.Scene.Add((Entity) new CS07_Ending(player, new Vector2(this.Center.X, this.Bottom)));
          break;
        case "ch8_door":
          this.Scene.Add((Entity) new CS08_EnterDoor(player, this.Left));
          break;
        case "ch9_badeline_helps":
          if (level.Session.GetFlag("badeline_helps"))
            break;
          this.Scene.Add((Entity) new CS10_BadelineHelps(player));
          break;
        case "ch9_ding_ding_ding":
          Audio.Play("event:/new_content/game/10_farewell/pico8_flag", this.Center);
          Decal decal1 = (Decal) null;
          foreach (Decal decal2 in this.Scene.Entities.FindAll<Decal>())
          {
            if (decal2.Name.ToLower() == "decals/10-farewell/finalflag")
            {
              decal1 = decal2;
              break;
            }
          }
          decal1?.FinalFlagTrigger();
          break;
        case "ch9_end_golden":
          ScreenWipe.WipeColor = Color.White;
          new FadeWipe((Scene) level, false, (Action) (() => level.OnEndOfFrame += (Action) (() =>
          {
            level.TeleportTo(player, "end-granny", Player.IntroTypes.Transition);
            player.Speed = Vector2.Zero;
          }))).Duration = 1f;
          break;
        case "ch9_ending":
          this.Scene.Add((Entity) new CS10_Ending(player));
          break;
        case "ch9_farewell":
          this.Scene.Add((Entity) new CS10_Farewell(player));
          break;
        case "ch9_final_room":
          Session session = (this.Scene as Level).Session;
          switch (session.GetCounter("final_room_deaths"))
          {
            case 0:
              this.Scene.Add((Entity) new CS10_FinalRoom(player, true));
              break;
            case 50:
              this.Scene.Add((Entity) new CS10_FinalRoom(player, false));
              break;
          }
          session.IncrementCounter("final_room_deaths");
          break;
        case "ch9_golden_snapshot":
          this.snapshot = Audio.CreateSnapshot("snapshot:/game_10_golden_room_flavour");
          (this.Scene as Level).SnapColorGrade("golden");
          break;
        case "ch9_goto_the_future":
        case "ch9_goto_the_past":
          level.OnEndOfFrame += (Action) (() =>
          {
            Vector2 vector2_1 = new Vector2(level.LevelOffset.X + (float) level.Bounds.Width - player.X, player.Y - level.LevelOffset.Y);
            Vector2 levelOffset = level.LevelOffset;
            Vector2 vector2_2 = player.Position - level.LevelOffset;
            Vector2 vector2_3 = level.Camera.Position - level.LevelOffset;
            Facings facing = player.Facing;
            level.Remove((Entity) player);
            level.UnloadLevel();
            level.Session.Dreaming = true;
            level.Session.Level = this.Event == "ch9_goto_the_future" ? "intro-01-future" : "intro-00-past";
            level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float) level.Bounds.Left, (float) level.Bounds.Top)));
            level.Session.FirstLevel = false;
            level.LoadLevel(Player.IntroTypes.Transition);
            level.Camera.Position = level.LevelOffset + vector2_3;
            level.Session.Inventory.Dashes = 1;
            player.Dashes = Math.Min(player.Dashes, 1);
            level.Add((Entity) player);
            player.Position = level.LevelOffset + vector2_2;
            player.Facing = facing;
            player.Hair.MoveHairBy(level.LevelOffset - levelOffset);
            if (level.Wipe != null)
              level.Wipe.Cancel();
            level.Flash(Color.White);
            level.Shake();
            level.Add((Entity) new LightningStrike(new Vector2(player.X + 60f, (float) (level.Bounds.Bottom - 180)), 10, 200f));
            level.Add((Entity) new LightningStrike(new Vector2(player.X + 220f, (float) (level.Bounds.Bottom - 180)), 40, 200f, 0.25f));
            Audio.Play("event:/new_content/game/10_farewell/lightning_strike");
          });
          break;
        case "ch9_hub_intro":
          if (level.Session.GetFlag("hub_intro"))
            break;
          this.Scene.Add((Entity) new CS10_HubIntro(this.Scene, player));
          break;
        case "ch9_hub_transition_out":
          this.Add((Component) new Coroutine(this.Ch9HubTransitionBackgroundToBright(player)));
          break;
        case "ch9_moon_intro":
          if (!level.Session.GetFlag("moon_intro") && player.StateMachine.State == 13)
          {
            this.Scene.Add((Entity) new CS10_MoonIntro(player));
            break;
          }
          level.Entities.FindFirst<BirdNPC>()?.RemoveSelf();
          level.Session.Inventory.Dashes = 1;
          player.Dashes = 1;
          break;
        case "end_city":
          this.Scene.Add((Entity) new CS01_Ending(player));
          break;
        case "end_oldsite_awake":
          this.Scene.Add((Entity) new CS02_Ending(player));
          break;
        case "end_oldsite_dream":
          this.Scene.Add((Entity) new CS02_DreamingPhonecall(player));
          break;
        default:
          throw new Exception("Event '" + this.Event + "' does not exist!");
      }
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      Audio.ReleaseSnapshot(this.snapshot);
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      Audio.ReleaseSnapshot(this.snapshot);
    }

    private IEnumerator Brighten()
    {
      Level level = this.Scene as Level;
      float darkness = AreaData.Get((Scene) level).DarknessAlpha;
      while ((double) level.Lighting.Alpha != (double) darkness)
      {
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, darkness, Engine.DeltaTime * 4f);
        yield return (object) null;
      }
    }

    private IEnumerator Ch9HubTransitionBackgroundToBright(Player player)
    {
      EventTrigger eventTrigger = this;
      Level level = eventTrigger.Scene as Level;
      float start = eventTrigger.Bottom;
      float end = eventTrigger.Top;
      while (true)
      {
        float num = Calc.ClampedMap(player.Y, start, end);
        foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>("bright"))
        {
          backdrop.ForceVisible = true;
          backdrop.FadeAlphaMultiplier = num;
        }
        yield return (object) null;
      }
    }
  }
}
