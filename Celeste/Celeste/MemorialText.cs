// Decompiled with JetBrains decompiler
// Type: Celeste.MemorialText
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MemorialText : Entity
  {
    private float alpha = 0.0f;
    private float timer = 0.0f;
    private float widestCharacter = 0.0f;
    public bool Show;
    public bool Dreamy;
    public Memorial Memorial;
    private float index;
    private string message;
    private int firstLineLength;
    private SoundSource textSfx;
    private bool textSfxPlaying;

    public MemorialText(Memorial memorial, bool dreamy)
    {
      this.AddTag((int) Tags.HUD);
      this.AddTag((int) Tags.PauseUpdate);
      this.Add((Component) (this.textSfx = new SoundSource()));
      this.Dreamy = dreamy;
      this.Memorial = memorial;
      this.message = Dialog.Clean(nameof (memorial), (Language) null);
      this.firstLineLength = this.CountToNewline(0);
      for (int index = 0; index < this.message.Length; ++index)
      {
        float x = ActiveFont.Measure(this.message[index]).X;
        if ((double) x > (double) this.widestCharacter)
          this.widestCharacter = x;
      }
      this.widestCharacter *= 0.9f;
    }

    public override void Update()
    {
      base.Update();
      if ((this.Scene as Level).Paused)
      {
        this.textSfx.Pause();
      }
      else
      {
        this.timer += Engine.DeltaTime;
        if (!this.Show)
        {
          this.alpha = Calc.Approach(this.alpha, 0.0f, Engine.DeltaTime);
          if ((double) this.alpha <= 0.0)
            this.index = (float) this.firstLineLength;
        }
        else
        {
          this.alpha = Calc.Approach(this.alpha, 1f, Engine.DeltaTime * 2f);
          if ((double) this.alpha >= 1.0)
            this.index = Calc.Approach(this.index, (float) this.message.Length, 32f * Engine.DeltaTime);
        }
        if (this.Show && (double) this.alpha >= 1.0 && (double) this.index < (double) this.message.Length)
        {
          if (!this.textSfxPlaying)
          {
            this.textSfxPlaying = true;
            this.textSfx.Play(this.Dreamy ? "event:/ui/game/memorial_dream_text_loop" : "event:/ui/game/memorial_text_loop", (string) null, 0.0f);
            this.textSfx.Param("end", 0.0f);
          }
        }
        else if (this.textSfxPlaying)
        {
          this.textSfxPlaying = false;
          this.textSfx.Stop(true);
          this.textSfx.Param("end", 1f);
        }
        this.textSfx.Resume();
      }
    }

    private int CountToNewline(int start)
    {
      int index = start;
      while (index < this.message.Length && this.message[index] != '\n')
        ++index;
      return index - start;
    }

    public override void Render()
    {
      if ((this.Scene as Level).FrozenOrPaused || (this.Scene as Level).Completed || ((double) this.index <= 0.0 || (double) this.alpha <= 0.0))
        return;
      Camera camera = this.SceneAs<Level>().Camera;
      Vector2 vector2 = new Vector2((float) (((double) this.Memorial.X - (double) camera.X) * 6.0), (float) (((double) this.Memorial.Y - (double) camera.Y) * 6.0 - 350.0 - (double) ActiveFont.LineHeight * 3.29999995231628));
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        vector2.X = 1920f - vector2.X;
      float num1 = Ease.CubeInOut(this.alpha);
      int num2 = (int) Math.Min((float) this.message.Length, this.index);
      int num3 = 0;
      float num4 = (float) (64.0 * (1.0 - (double) num1));
      int newline = this.CountToNewline(0);
      for (int index = 0; index < num2; ++index)
      {
        char character = this.message[index];
        if (character == '\n')
        {
          num3 = 0;
          newline = this.CountToNewline(index + 1);
          num4 += ActiveFont.LineHeight * 1.1f;
        }
        else
        {
          float x1 = 1f;
          float x2 = (float) ((double) -newline * (double) this.widestCharacter / 2.0 + ((double) num3 + 0.5) * (double) this.widestCharacter);
          float num5 = 0.0f;
          if (this.Dreamy && character != ' ' && character != '-' && character != '\n')
          {
            character = this.message[(index + (int) (Math.Sin((double) this.timer * 2.0 + (double) index / 8.0) * 4.0) + this.message.Length) % this.message.Length];
            num5 = (float) Math.Sin((double) this.timer * 2.0 + (double) index / 8.0) * 8f;
            x1 = Math.Sin((double) this.timer * 4.0 + (double) index / 16.0) < 0.0 ? -1f : 1f;
          }
          ActiveFont.Draw(character, vector2 + new Vector2(x2, num4 + num5), new Vector2(0.5f, 1f), new Vector2(x1, 1f), Color.White * num1);
          ++num3;
        }
      }
    }
  }
}

