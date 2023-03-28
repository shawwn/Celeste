// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_BadelineHelps
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
  public class CS10_BadelineHelps : CutsceneEntity
  {
    public const string Flag = "badeline_helps";
    private Player player;
    private BadelineDummy badeline;
    private EventInstance entrySfx;

    public CS10_BadelineHelps(Player player)
      : base()
    {
      this.Depth = -8500;
      this.player = player;
    }

    public override void OnBegin(Level level) => this.Add((Component)new Coroutine(this.Cutscene(level)));

    private IEnumerator Cutscene(Level level)
    {
      CS10_BadelineHelps cs10BadelineHelps = this;
      Vector2 spawn = level.GetSpawnPoint(cs10BadelineHelps.player.Position);
      cs10BadelineHelps.player.Dashes = 2;
      cs10BadelineHelps.player.StateMachine.State = 11;
      cs10BadelineHelps.player.DummyGravity = false;
      cs10BadelineHelps.entrySfx = Audio.Play("event:/new_content/char/madeline/screenentry_stubborn",
        cs10BadelineHelps.player.Position);
      yield return (object)cs10BadelineHelps.player.MoonLanding(spawn);
      yield return (object)level.ZoomTo(new Vector2(spawn.X - level.Camera.X, 134f), 2f, 0.5f);
      yield return (object)1f;
      yield return (object)cs10BadelineHelps.BadelineAppears();
      yield return (object)0.3f;
      yield return (object)Textbox.Say("CH9_HELPING_HAND", new Func<IEnumerator>(cs10BadelineHelps.MadelineFacesAway),
        new Func<IEnumerator>(cs10BadelineHelps.MadelineFacesBadeline),
        new Func<IEnumerator>(cs10BadelineHelps.MadelineStepsForwards));
      if (cs10BadelineHelps.badeline != null)
        yield return (object)cs10BadelineHelps.BadelineVanishes();
      yield return (object)level.ZoomBack(0.5f);
      cs10BadelineHelps.EndCutscene(level);
    }

    private IEnumerator BadelineAppears()
    {
      CS10_BadelineHelps cs10BadelineHelps = this;
      cs10BadelineHelps.StartMusic();
      Audio.Play("event:/char/badeline/maddy_split", cs10BadelineHelps.player.Position);
      cs10BadelineHelps.Level.Add((Entity)(cs10BadelineHelps.badeline =
        new BadelineDummy(cs10BadelineHelps.player.Center)));
      cs10BadelineHelps.Level.Displacement.AddBurst(cs10BadelineHelps.badeline.Center, 0.5f, 8f, 32f, 0.5f);
      cs10BadelineHelps.player.Dashes = 1;
      cs10BadelineHelps.badeline.Sprite.Scale.X = -1f;
      yield return (object)cs10BadelineHelps.badeline.FloatTo(cs10BadelineHelps.player.Center + new Vector2(18f, -10f),
        new int?(-1), false);
      yield return (object)0.2f;
      cs10BadelineHelps.player.Facing = Facings.Right;
      yield return (object)null;
    }

    // private IEnumerator MadelineFacesAway()
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num = this.\u003C\u003E1__state;
    //   CS10_BadelineHelps cs10BadelineHelps = this;
    //   if (num != 0)
    //   {
    //     if (num != 1)
    //       return false;
    //     // ISSUE: reference to a compiler-generated field
    //     this.\u003C\u003E1__state = -1;
    //     return false;
    //   }
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = -1;
    //   cs10BadelineHelps.Level.NextColorGrade("feelingdown", 0.1f);
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E2__current = (object) cs10BadelineHelps.player.DummyWalkTo(cs10BadelineHelps.player.X - 16f);
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = 1;
    //   return true;
    // }
    private IEnumerator MadelineFacesAway()
    {
      Level.NextColorGrade("feelingdown", 0.1f);
      yield return player.DummyWalkTo(player.X - 16f);
    }

  private IEnumerator MadelineFacesBadeline()
    {
      this.player.Facing = Facings.Right;
      yield return (object) 0.2f;
    }

    private IEnumerator MadelineStepsForwards()
    {
      CS10_BadelineHelps cs10BadelineHelps = this;
      Vector2 spawnPoint = cs10BadelineHelps.Level.GetSpawnPoint(cs10BadelineHelps.player.Position);
      cs10BadelineHelps.Add((Component) new Coroutine(cs10BadelineHelps.player.DummyWalkToExact((int) spawnPoint.X)));
      yield return (object) 0.1f;
      yield return (object) cs10BadelineHelps.badeline.FloatTo(cs10BadelineHelps.badeline.Position + new Vector2(20f, 0.0f), faceDirection: false);
    }

    private IEnumerator BadelineVanishes()
    {
      yield return (object) 0.2f;
      this.badeline.Vanish();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.badeline = (BadelineDummy) null;
      yield return (object) 0.2f;
    }

    private void StartMusic()
    {
      if (!(this.Level.Session.Audio.Music.Event != "event:/new_content/music/lvl10/cassette_rooms"))
        return;
      int num = 0;
      CassetteBlockManager entity = this.Level.Tracker.GetEntity<CassetteBlockManager>();
      if (entity != null)
        num = entity.GetSixteenthNote();
      this.Level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/cassette_rooms";
      this.Level.Session.Audio.Music.Param("sixteenth_note", (float) num);
      this.Level.Session.Audio.Apply(true);
      this.Level.Session.Audio.Music.Param("sixteenth_note", 7f);
    }

    public override void OnEnd(Level level)
    {
      this.Level.Session.Inventory.Dashes = 1;
      this.player.Dashes = 1;
      this.player.Depth = 0;
      this.player.Dashes = 1;
      this.player.Speed = Vector2.Zero;
      this.player.Position = level.GetSpawnPoint(this.player.Position);
      Player player = this.player;
      player.Position = player.Position - Vector2.UnitY * 12f;
      this.player.MoveVExact(100);
      this.player.Active = true;
      this.player.Visible = true;
      this.player.StateMachine.State = 0;
      if (this.badeline != null)
        this.badeline.RemoveSelf();
      level.ResetZoom();
      level.Session.SetFlag("badeline_helps");
      if (!this.WasSkipped)
        return;
      Audio.Stop(this.entrySfx);
      this.StartMusic();
      level.SnapColorGrade("feelingdown");
    }
  }
}
