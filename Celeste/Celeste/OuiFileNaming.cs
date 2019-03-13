// Decompiled with JetBrains decompiler
// Type: Celeste.OuiFileNaming
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class OuiFileNaming : Oui
  {
    private int index = 0;
    private int line = 0;
    private bool selectingOptions = true;
    private Color unselectColor = Color.LightGray;
    private Color selectColorA = Calc.HexToColor("84FF54");
    private Color selectColorB = Calc.HexToColor("FCFF59");
    private Color disableColor = Color.DarkSlateBlue;
    public string StartingName;
    public OuiFileSelectSlot FileSlot;
    public const int MaxNameLength = 12;
    public const int MinNameLength = 1;
    private string[] letters;
    private float widestLetter;
    private float widestLine;
    private int widestLineCount;
    private int optionsIndex;
    private float lineHeight;
    private float lineSpacing;
    private float boxPadding;
    private float optionsScale;
    private string cancel;
    private string space;
    private string backspace;
    private string accept;
    private float cancelWidth;
    private float spaceWidth;
    private float backspaceWidth;
    private float beginWidth;
    private float optionsWidth;
    private float boxWidth;
    private float boxHeight;
    private float pressedTimer;
    private float timer;
    private float ease;
    private Wiggler wiggler;

    public string Name
    {
      get
      {
        return this.FileSlot.Name;
      }
      set
      {
        this.FileSlot.Name = value;
      }
    }

    private Vector2 boxtopleft
    {
      get
      {
        return this.Position + new Vector2((float) ((1920.0 - (double) this.boxWidth) / 2.0), (float) (360.0 + (680.0 - (double) this.boxHeight) / 2.0));
      }
    }

    public OuiFileNaming()
    {
      this.wiggler = Wiggler.Create(0.25f, 4f, (Action<float>) null, false, false);
      this.Position = new Vector2(0.0f, 1080f);
      this.Visible = false;
    }

    public override IEnumerator Enter(Oui from)
    {
      if (this.Name == Dialog.Clean("FILE_DEFAULT", (Language) null))
        this.Name = "";
      this.Overworld.ShowInputUI = false;
      this.selectingOptions = false;
      this.optionsIndex = 0;
      this.index = 0;
      this.line = 0;
      string chars = Dialog.Clean("name_letters", (Language) null);
      this.letters = chars.Split('\n');
      string str = chars;
      for (int index = 0; index < str.Length; ++index)
      {
        char letter = str[index];
        float size = ActiveFont.Measure(letter).X;
        if ((double) size > (double) this.widestLetter)
          this.widestLetter = size;
      }
      str = (string) null;
      this.widestLineCount = 0;
      string[] strArray = this.letters;
      for (int index = 0; index < strArray.Length; ++index)
      {
        string l = strArray[index];
        if (l.Length > this.widestLineCount)
          this.widestLineCount = l.Length;
        l = (string) null;
      }
      strArray = (string[]) null;
      this.widestLine = (float) this.widestLineCount * this.widestLetter;
      chars = (string) null;
      this.lineHeight = ActiveFont.LineHeight;
      this.lineSpacing = ActiveFont.LineHeight * 0.1f;
      this.boxPadding = this.widestLetter;
      this.optionsScale = 0.75f;
      this.cancel = Dialog.Clean("name_back", (Language) null);
      this.space = Dialog.Clean("name_space", (Language) null);
      this.backspace = Dialog.Clean("name_backspace", (Language) null);
      this.accept = Dialog.Clean("name_accept", (Language) null);
      this.cancelWidth = ActiveFont.Measure(this.cancel).X * this.optionsScale;
      this.spaceWidth = ActiveFont.Measure(this.space).X * this.optionsScale;
      this.backspaceWidth = ActiveFont.Measure(this.backspace).X * this.optionsScale;
      this.beginWidth = ActiveFont.Measure(this.accept).X * this.optionsScale;
      this.optionsWidth = (float) ((double) this.cancelWidth + (double) this.spaceWidth + (double) this.backspaceWidth + (double) this.beginWidth + (double) this.widestLetter * 3.0);
      this.boxWidth = Math.Max(this.widestLine, this.optionsWidth) + this.boxPadding * 2f;
      this.boxHeight = (float) ((double) (this.letters.Length + 1) * (double) this.lineHeight + (double) this.letters.Length * (double) this.lineSpacing + (double) this.boxPadding * 3.0);
      this.Visible = true;
      Vector2 posFrom = this.Position;
      Vector2 posTo = Vector2.Zero;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        this.ease = Ease.CubeIn(t);
        this.Position = posFrom + (posTo - posFrom) * Ease.CubeInOut(t);
        yield return (object) null;
      }
      this.ease = 1f;
      posFrom = new Vector2();
      posTo = new Vector2();
      yield return (object) 0.2f;
      this.Focused = true;
      yield return (object) 0.2f;
      this.wiggler.Start();
    }

    public override IEnumerator Leave(Oui next)
    {
      this.Overworld.ShowInputUI = true;
      this.Focused = false;
      Vector2 posFrom = this.Position;
      Vector2 posTo = new Vector2(0.0f, 1080f);
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        this.ease = 1f - Ease.CubeIn(t);
        this.Position = posFrom + (posTo - posFrom) * Ease.CubeInOut(t);
        yield return (object) null;
      }
      this.Visible = false;
    }

    public override void Update()
    {
      base.Update();
      if (this.Selected && this.Focused)
      {
        if (Input.MenuRight.Pressed && (this.optionsIndex < 3 || !this.selectingOptions) && (this.Name.Length > 0 || !this.selectingOptions))
        {
          if (this.selectingOptions)
          {
            this.optionsIndex = Math.Min(this.optionsIndex + 1, 3);
          }
          else
          {
            do
            {
              this.index = (this.index + 1) % this.letters[this.line].Length;
            }
            while (this.letters[this.line][this.index] == ' ');
          }
          this.wiggler.Start();
          Audio.Play("event:/ui/main/rename_entry_rollover");
        }
        else if (Input.MenuLeft.Pressed && (this.optionsIndex > 0 || !this.selectingOptions))
        {
          if (this.selectingOptions)
          {
            this.optionsIndex = Math.Max(this.optionsIndex - 1, 0);
          }
          else
          {
            do
            {
              this.index = (this.index + this.letters[this.line].Length - 1) % this.letters[this.line].Length;
            }
            while (this.letters[this.line][this.index] == ' ');
          }
          this.wiggler.Start();
          Audio.Play("event:/ui/main/rename_entry_rollover");
        }
        else if (Input.MenuDown.Pressed && !this.selectingOptions)
        {
          int index = this.line + 1;
          while (true)
          {
            if (index < this.letters.Length)
            {
              if (this.index >= this.letters[index].Length || this.letters[index][this.index] == ' ')
                ++index;
              else
                goto label_15;
            }
            else
              break;
          }
          this.selectingOptions = true;
          goto label_18;
label_15:
          this.line = index;
label_18:
          if (this.selectingOptions)
          {
            float num1 = (float) this.index * this.widestLetter;
            float num2 = this.boxWidth - this.boxPadding * 2f;
            this.optionsIndex = this.Name.Length != 0 && (double) num1 >= (double) this.cancelWidth + ((double) num2 - (double) this.cancelWidth - (double) this.beginWidth - (double) this.backspaceWidth - (double) this.spaceWidth - (double) this.widestLetter * 3.0) / 2.0 ? ((double) num1 >= (double) num2 - (double) this.beginWidth - (double) this.backspaceWidth - (double) this.widestLetter * 2.0 ? ((double) num1 >= (double) num2 - (double) this.beginWidth - (double) this.widestLetter ? 3 : 2) : 1) : 0;
          }
          this.wiggler.Start();
          Audio.Play("event:/ui/main/rename_entry_rollover");
        }
        else if ((Input.MenuUp.Pressed || this.selectingOptions && this.Name.Length <= 0 && this.optionsIndex > 0) && (this.line > 0 || this.selectingOptions))
        {
          if (this.selectingOptions)
          {
            this.line = this.letters.Length;
            this.selectingOptions = false;
            float num = this.boxWidth - this.boxPadding * 2f;
            if (this.optionsIndex == 0)
              this.index = (int) ((double) this.cancelWidth / 2.0 / (double) this.widestLetter);
            else if (this.optionsIndex == 1)
              this.index = (int) (((double) num - (double) this.beginWidth - (double) this.backspaceWidth - (double) this.spaceWidth / 2.0 - (double) this.widestLetter * 2.0) / (double) this.widestLetter);
            else if (this.optionsIndex == 2)
              this.index = (int) (((double) num - (double) this.beginWidth - (double) this.backspaceWidth / 2.0 - (double) this.widestLetter) / (double) this.widestLetter);
            else if (this.optionsIndex == 3)
              this.index = (int) (((double) num - (double) this.beginWidth / 2.0) / (double) this.widestLetter);
          }
          --this.line;
          while (this.line > 0 && (this.index >= this.letters[this.line].Length || this.letters[this.line][this.index] == ' '))
            --this.line;
          while (this.index >= this.letters[this.line].Length || this.letters[this.line][this.index] == ' ')
            --this.index;
          this.wiggler.Start();
          Audio.Play("event:/ui/main/rename_entry_rollover");
        }
        else if (Input.MenuConfirm.Pressed)
        {
          if (this.selectingOptions)
          {
            if (this.optionsIndex == 0)
              this.Cancel();
            else if (this.optionsIndex == 1 && this.Name.Length > 0)
              this.Space();
            else if (this.optionsIndex == 2)
              this.Backspace();
            else if (this.optionsIndex == 3)
              this.Finish();
          }
          else if (this.Name.Length < 12)
          {
            this.Name += this.letters[this.line][this.index].ToString();
            this.wiggler.Start();
            Audio.Play("event:/ui/main/rename_entry_char");
          }
          else
            Audio.Play("event:/ui/main/button_invalid");
        }
        else if (Input.MenuCancel.Pressed)
        {
          if (this.Name.Length > 0)
            this.Backspace();
          else
            this.Cancel();
        }
        else if (Input.Pause.Pressed)
          this.Finish();
      }
      this.pressedTimer -= Engine.DeltaTime;
      this.timer += Engine.DeltaTime;
      this.wiggler.Update();
    }

    private void Space()
    {
      if (this.Name.Length < 12 && this.Name.Length > 0)
      {
        this.Name += " ";
        this.wiggler.Start();
        Audio.Play("event:/ui/main/rename_entry_char");
      }
      else
        Audio.Play("event:/ui/main/button_invalid");
    }

    private void Backspace()
    {
      if (this.Name.Length > 0)
      {
        this.Name = this.Name.Substring(0, this.Name.Length - 1);
        Audio.Play("event:/ui/main/rename_entry_backspace");
      }
      else
        Audio.Play("event:/ui/main/button_invalid");
    }

    private void Finish()
    {
      if (this.Name.Length >= 1)
      {
        this.Focused = false;
        this.Overworld.Goto<OuiFileSelect>();
        Audio.Play("event:/ui/main/rename_entry_accept");
      }
      else
        Audio.Play("event:/ui/main/button_invalid");
    }

    private void Cancel()
    {
      this.FileSlot.Name = this.StartingName;
      this.Focused = false;
      this.Overworld.Goto<OuiFileSelect>();
      Audio.Play("event:/ui/main/button_back");
    }

    public override void Render()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.8f * this.ease);
      Vector2 vector2 = this.boxtopleft + new Vector2(this.boxPadding, this.boxPadding);
      int num1 = 0;
      foreach (string letter in this.letters)
      {
        for (int index = 0; index < letter.Length; ++index)
        {
          bool selected = num1 == this.line && index == this.index && !this.selectingOptions;
          Vector2 scale = Vector2.One * (selected ? 1.2f : 1f);
          Vector2 at = vector2 + new Vector2(this.widestLetter, this.lineHeight) / 2f;
          if (selected)
            at += new Vector2(0.0f, this.wiggler.Value) * 8f;
          this.DrawOptionText(letter[index].ToString(), at, new Vector2(0.5f, 0.5f), scale, selected, false);
          vector2.X += this.widestLetter;
        }
        vector2.X = this.boxtopleft.X + this.boxPadding;
        vector2.Y += this.lineHeight + this.lineSpacing;
        ++num1;
      }
      float num2 = this.wiggler.Value * 8f;
      vector2.Y = this.boxtopleft.Y + this.boxHeight - this.lineHeight - this.boxPadding;
      Draw.Rect(vector2.X, vector2.Y - this.boxPadding * 0.5f, this.boxWidth - this.boxPadding * 2f, 4f, Color.White);
      this.DrawOptionText(this.cancel, vector2 + new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 0 ? 0.0f : num2)), new Vector2(0.0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 0, false);
      vector2.X = this.boxtopleft.X + this.boxWidth - this.backspaceWidth - this.widestLetter - this.spaceWidth - this.widestLetter - this.beginWidth - this.boxPadding;
      this.DrawOptionText(this.space, vector2 + new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 1 ? 0.0f : num2)), new Vector2(0.0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 1, this.Name.Length == 0 || !this.Focused);
      vector2.X += this.spaceWidth + this.widestLetter;
      this.DrawOptionText(this.backspace, vector2 + new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 2 ? 0.0f : num2)), new Vector2(0.0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 2, this.Name.Length <= 0 || !this.Focused);
      vector2.X += this.backspaceWidth + this.widestLetter;
      this.DrawOptionText(this.accept, vector2 + new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 3 ? 0.0f : num2)), new Vector2(0.0f, 1f), Vector2.One * this.optionsScale, this.selectingOptions && this.optionsIndex == 3, this.Name.Length < 1 || !this.Focused);
    }

    private void DrawOptionText(
      string text,
      Vector2 at,
      Vector2 justify,
      Vector2 scale,
      bool selected,
      bool disabled = false)
    {
      bool flag = selected && (double) this.pressedTimer > 0.0;
      Color color = disabled ? this.disableColor : this.GetTextColor(selected);
      Color edgeColor = disabled ? Color.Lerp(this.disableColor, Color.Black, 0.7f) : Color.Gray;
      if (flag)
        ActiveFont.Draw(text, at + Vector2.UnitY, justify, scale, color);
      else
        ActiveFont.DrawEdgeOutline(text, at, justify, scale, color, 4f, edgeColor, 0.0f, new Color());
    }

    private Color GetTextColor(bool selected)
    {
      if (selected)
        return Calc.BetweenInterval(this.timer, 0.1f) ? this.selectColorA : this.selectColorB;
      return this.unselectColor;
    }
  }
}

