// Decompiled with JetBrains decompiler
// Type: Celeste.LevelEnter
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class LevelEnter : Scene
  {
    private Session session;
    private Postcard postcard;
    private bool fromSaveData;

    public static void Go(Session session, bool fromSaveData)
    {
      HiresSnow snow = (HiresSnow) null;
      if (Engine.Scene is Overworld)
        snow = (Engine.Scene as Overworld).Snow;
      bool flag = !fromSaveData && session.StartedFromBeginning;
      if (flag && session.Area.ID == 0)
        Engine.Scene = (Scene) new IntroVignette(session, snow);
      else if (flag && session.Area.ID == 7 && session.Area.Mode == AreaMode.Normal)
        Engine.Scene = (Scene) new SummitVignette(session);
      else if (flag && session.Area.ID == 9 && session.Area.Mode == AreaMode.Normal)
        Engine.Scene = (Scene) new CoreVignette(session, snow);
      else
        Engine.Scene = (Scene) new LevelEnter(session, fromSaveData);
    }

    private LevelEnter(Session session, bool fromSaveData)
    {
      this.session = session;
      this.fromSaveData = fromSaveData;
      this.Add(new Entity()
      {
        (Component) new Coroutine(this.Routine(), true)
      });
      this.Add((Monocle.Renderer) new HudRenderer());
    }

    private IEnumerator Routine()
    {
      int area = -1;
      if (this.session.StartedFromBeginning && !this.fromSaveData && this.session.Area.Mode == AreaMode.Normal && ((!SaveData.Instance.Areas[this.session.Area.ID].Modes[0].Completed || SaveData.Instance.DebugMode) && this.session.Area.ID >= 1) && this.session.Area.ID <= 6)
        area = this.session.Area.ID;
      if (area >= 0)
      {
        yield return (object) 1f;
        this.Add((Entity) (this.postcard = new Postcard(Dialog.Get("postcard_area_" + (object) area, (Language) null), area)));
        yield return (object) this.postcard.DisplayRoutine();
      }
      if (this.session.StartedFromBeginning && !this.fromSaveData && this.session.Area.Mode == AreaMode.BSide)
      {
        LevelEnter.BSideTitle title = new LevelEnter.BSideTitle(this.session);
        this.Add((Entity) title);
        Audio.Play("event:/ui/main/bside_intro_text");
        yield return (object) title.EaseIn();
        yield return (object) 0.25f;
        yield return (object) title.EaseOut();
        yield return (object) 0.25f;
        title = (LevelEnter.BSideTitle) null;
      }
      Input.SetLightbarColor(AreaData.Get(this.session.Area).TitleBaseColor);
      Engine.Scene = (Scene) new LevelLoader(this.session, new Vector2?());
    }

    public override void BeforeRender()
    {
      base.BeforeRender();
      if (this.postcard == null)
        return;
      this.postcard.BeforeRender();
    }

    private class BSideTitle : Entity
    {
      private float[] fade = new float[3];
      private float[] offsets = new float[3];
      private float offset = 0.0f;
      private string title;
      private string musicBy;
      private string artist;
      private string album;
      private float musicByWidth;
      private PixelFontSize artistFont;

      public BSideTitle(Session session)
      {
        this.Tag = (int) Tags.HUD;
        this.artistFont = Dialog.Languages["english"].FontSize;
        switch (session.Area.ID)
        {
          case 1:
            this.artist = Credits.Remixers[0];
            break;
          case 2:
            this.artist = Credits.Remixers[1];
            break;
          case 3:
            this.artist = Credits.Remixers[2];
            break;
          case 4:
            this.artist = Credits.Remixers[3];
            break;
          case 5:
            this.artist = Credits.Remixers[4];
            break;
          case 6:
            this.artist = Credits.Remixers[5];
            break;
          case 7:
            this.artist = Credits.Remixers[6];
            break;
          case 9:
            this.artist = Credits.Remixers[7];
            this.artistFont = Dialog.Languages["japanese"].FontSize;
            break;
        }
        this.title = Dialog.Get(AreaData.Get(session).Name, (Language) null) + " " + Dialog.Get(AreaData.Get(session).Name + "_remix", (Language) null);
        this.musicBy = Dialog.Get("remix_by", (Language) null) + " ";
        this.musicByWidth = ActiveFont.Measure(this.musicBy).X;
        this.album = Dialog.Get("remix_album", (Language) null);
      }

      public IEnumerator EaseIn()
      {
        this.Add((Component) new Coroutine(this.FadeTo(0, 1f, 1f), true));
        yield return (object) 0.2f;
        this.Add((Component) new Coroutine(this.FadeTo(1, 1f, 1f), true));
        yield return (object) 0.2f;
        this.Add((Component) new Coroutine(this.FadeTo(2, 1f, 1f), true));
        yield return (object) 1.8f;
      }

      public IEnumerator EaseOut()
      {
        this.Add((Component) new Coroutine(this.FadeTo(0, 0.0f, 1f), true));
        yield return (object) 0.2f;
        this.Add((Component) new Coroutine(this.FadeTo(1, 0.0f, 1f), true));
        yield return (object) 0.2f;
        this.Add((Component) new Coroutine(this.FadeTo(2, 0.0f, 1f), true));
        yield return (object) 1f;
      }

      private IEnumerator FadeTo(int index, float target, float duration)
      {
        while ((double) (this.fade[index] = Calc.Approach(this.fade[index], target, Engine.DeltaTime / duration)) != (double) target)
        {
          this.offsets[index] = (double) target != 0.0 ? (float) (-(double) Ease.CubeIn(1f - this.fade[index]) * 32.0) : Ease.CubeIn(1f - this.fade[index]) * 32f;
          yield return (object) null;
        }
      }

      public override void Update()
      {
        base.Update();
        this.offset += Engine.DeltaTime * 32f;
      }

      public override void Render()
      {
        Vector2 vector2 = new Vector2(60f + this.offset, 800f);
        ActiveFont.Draw(this.title, vector2 + new Vector2(this.offsets[0], 0.0f), Color.White * this.fade[0]);
        ActiveFont.Draw(this.musicBy, vector2 + new Vector2(this.offsets[1], 60f), Color.White * this.fade[1]);
        this.artistFont.Draw(this.artist, vector2 + new Vector2(this.musicByWidth + this.offsets[1], 60f), Color.White * this.fade[1]);
        ActiveFont.Draw(this.album, vector2 + new Vector2(this.offsets[2], 120f), Color.White * this.fade[2]);
      }
    }
  }
}

