// Decompiled with JetBrains decompiler
// Type: Celeste.EventTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class EventTrigger : Trigger
  {
    public string Event;
    private bool triggered;

    public EventTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Event = data.Attr("event", "");
    }

    public float Time { get; private set; }

    public override void OnEnter(Player player)
    {
      if (this.triggered)
        return;
      this.triggered = true;
      Level scene = this.Scene as Level;
      switch (this.Event)
      {
        case "cancel_ch5_see_theo":
          scene.Session.SetFlag("it_ch5_see_theo", true);
          scene.Session.SetFlag("it_ch5_see_theo_b", true);
          scene.Session.SetFlag("ignore_darkness_" + scene.Session.Level, true);
          this.Add((Component) new Coroutine(this.Brighten(), true));
          break;
        case "ch5_found_theo":
          if (scene.Session.GetFlag("foundTheoInCrystal"))
            break;
          this.Scene.Add((Entity) new CS05_SaveTheo(player));
          break;
        case "ch5_mirror_reflection":
          if (scene.Session.GetFlag("reflection"))
            break;
          this.Scene.Add((Entity) new CS05_Reflection1(player));
          break;
        case "ch5_see_theo":
          this.Scene.Add((Entity) new CS05_SeeTheo(player, 0));
          break;
        case "ch6_boss_intro":
          if (scene.Session.GetFlag("boss_intro"))
            break;
          scene.Add((Entity) new CS06_BossIntro((float) this.Center.X, player, scene.Entities.FindFirst<FinalBoss>()));
          break;
        case "ch6_reflect":
          if (scene.Session.GetFlag("reflection"))
            break;
          this.Scene.Add((Entity) new CS06_Reflection(player, (float) (this.Center.X - 5.0)));
          break;
        case "ch7_summit":
          this.Scene.Add((Entity) new CS07_Ending(player, new Vector2((float) this.Center.X, this.Bottom)));
          break;
        case "ch8_door":
          this.Scene.Add((Entity) new CS08_EnterDoor(player, this.Left));
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
  }
}
