// Decompiled with JetBrains decompiler
// Type: Celeste.HeightDisplay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class HeightDisplay : Entity
  {
    private string text = "";
    private string leftText = "";
    private string rightText = "";
    private bool easingCamera = true;
    private int index;
    private float leftSize;
    private float rightSize;
    private float numberSize;
    private Vector2 size;
    private int height;
    private float approach;
    private float ease;
    private float pulse;
    private string spawnedLevel;
    private bool setAudioProgression;

    private bool drawText
    {
      get
      {
        if (this.index >= 0 && (double) this.ease > 0.0)
          return !string.IsNullOrEmpty(this.text);
        return false;
      }
    }

    public HeightDisplay(int index)
    {
      this.Tag = (int) Tags.HUD | (int) Tags.Persistent;
      this.index = index;
      string name = "CH7_HEIGHT_" + (index < 0 ? "START" : index.ToString());
      if (index >= 0 && Dialog.Has(name, (Language) null))
      {
        this.text = Dialog.Get(name, (Language) null);
        this.text = this.text.ToUpper();
        this.height = (index + 1) * 500;
        this.approach = (float) (index * 500);
        int length = this.text.IndexOf("{X}");
        this.leftText = this.text.Substring(0, length);
        this.leftSize = (float) ActiveFont.Measure(this.leftText).X;
        this.rightText = this.text.Substring(length + 3);
        this.numberSize = (float) ActiveFont.Measure(this.height.ToString()).X;
        this.rightSize = (float) ActiveFont.Measure(this.rightText).X;
        this.size = ActiveFont.Measure(this.leftText + (object) this.height + this.rightText);
      }
      this.Add((Component) new Coroutine(this.Routine(), true));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.spawnedLevel = (scene as Level).Session.Level;
    }

    private IEnumerator Routine()
    {
      HeightDisplay heightDisplay = this;
      Player player;
      while (true)
      {
        player = heightDisplay.Scene.Tracker.GetEntity<Player>();
        if (player == null || !((heightDisplay.Scene as Level).Session.Level != heightDisplay.spawnedLevel))
          yield return (object) null;
        else
          break;
      }
      heightDisplay.StepAudioProgression();
      heightDisplay.easingCamera = false;
      yield return (object) 0.1f;
      heightDisplay.Add((Component) new Coroutine(heightDisplay.CameraUp(), true));
      if (!string.IsNullOrEmpty(heightDisplay.text) && heightDisplay.index >= 0)
        Audio.Play("event:/game/07_summit/altitude_count");
      while ((double) (heightDisplay.ease += Engine.DeltaTime / 0.15f) < 1.0)
        yield return (object) null;
      while ((double) heightDisplay.approach < (double) heightDisplay.height && !player.OnGround(1))
        yield return (object) null;
      heightDisplay.approach = (float) heightDisplay.height;
      heightDisplay.pulse = 1f;
      while ((double) (heightDisplay.pulse -= Engine.DeltaTime * 4f) > 0.0)
        yield return (object) null;
      heightDisplay.pulse = 0.0f;
      yield return (object) 1f;
      while ((double) (heightDisplay.ease -= Engine.DeltaTime / 0.15f) > 0.0)
        yield return (object) null;
      heightDisplay.RemoveSelf();
    }

    private IEnumerator CameraUp()
    {
      HeightDisplay heightDisplay = this;
      heightDisplay.easingCamera = true;
      Level level = heightDisplay.Scene as Level;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 1.5f)
      {
        Camera camera = level.Camera;
        Rectangle bounds = level.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Bottom() - 180) + 64.0 * (1.0 - (double) Ease.CubeOut(p));
        camera.Y = (float) num;
        yield return (object) null;
      }
    }

    private void StepAudioProgression()
    {
      Session session = (this.Scene as Level).Session;
      if (this.setAudioProgression || this.index < 0 || session.Area.Mode != AreaMode.Normal)
        return;
      this.setAudioProgression = true;
      int num = this.index + 1;
      if (num <= 5)
        session.Audio.Music.Progress = num;
      else
        session.Audio.Music.Event = "event:/music/lvl7/final_ascent";
      session.Audio.Apply();
    }

    public override void Update()
    {
      if (this.index >= 0 && (double) this.ease > 0.0)
      {
        if ((double) this.height - (double) this.approach > 100.0)
          this.approach += 1000f * Engine.DeltaTime;
        else if ((double) this.height - (double) this.approach > 25.0)
          this.approach += 200f * Engine.DeltaTime;
        else if ((double) this.height - (double) this.approach > 5.0)
          this.approach += 50f * Engine.DeltaTime;
        else if ((double) this.height - (double) this.approach > 0.0)
          this.approach += 10f * Engine.DeltaTime;
        else
          this.approach = (float) this.height;
      }
      Level scene = this.Scene as Level;
      if (!this.easingCamera)
      {
        Camera camera = scene.Camera;
        Rectangle bounds = scene.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Bottom() - 180 + 64);
        camera.Y = (float) num;
      }
      base.Update();
    }

    public override void Render()
    {
      if (!this.drawText)
        return;
      Vector2 vector2_1 = Vector2.op_Division(new Vector2(1920f, 1080f), 2f);
      float num1 = (float) (1.20000004768372 + (double) this.pulse * 0.200000002980232);
      Vector2 vector2_2 = Vector2.op_Multiply(this.size, num1);
      float num2 = Ease.SineInOut(this.ease);
      Vector2 vector2_3;
      ((Vector2) ref vector2_3).\u002Ector(1f, num2);
      Draw.Rect((float) (vector2_1.X - (vector2_2.X + 64.0) * 0.5 * vector2_3.X), (float) (vector2_1.Y - (vector2_2.Y + 32.0) * 0.5 * vector2_3.Y), (float) ((vector2_2.X + 64.0) * vector2_3.X), (float) ((vector2_2.Y + 32.0) * vector2_3.Y), Color.get_Black());
      Vector2 position = Vector2.op_Addition(vector2_1, new Vector2((float) (-vector2_2.X * 0.5), 0.0f));
      Vector2 scale = Vector2.op_Multiply(vector2_3, num1);
      Color color = Color.op_Multiply(Color.get_White(), num2);
      ActiveFont.Draw(this.leftText, position, new Vector2(0.0f, 0.5f), scale, color);
      ActiveFont.Draw(this.rightText, Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), this.leftSize + this.numberSize), num1)), new Vector2(0.0f, 0.5f), scale, color);
      ActiveFont.Draw(((int) this.approach).ToString(), Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_UnitX(), this.leftSize + this.numberSize * 0.5f), num1)), new Vector2(0.5f, 0.5f), scale, color);
    }

    public override void Removed(Scene scene)
    {
      this.StepAudioProgression();
      base.Removed(scene);
    }
  }
}
