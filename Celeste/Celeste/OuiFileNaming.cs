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
    private bool selectingOptions = true;
    private Color unselectColor = Color.get_LightGray();
    private Color selectColorA = Calc.HexToColor("84FF54");
    private Color selectColorB = Calc.HexToColor("FCFF59");
    private Color disableColor = Color.get_DarkSlateBlue();
    public string StartingName;
    public OuiFileSelectSlot FileSlot;
    public const int MaxNameLength = 12;
    public const int MinNameLength = 1;
    private string[] letters;
    private int index;
    private int line;
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
        return Vector2.op_Addition(this.Position, new Vector2((float) ((1920.0 - (double) this.boxWidth) / 2.0), (float) (360.0 + (680.0 - (double) this.boxHeight) / 2.0)));
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
      OuiFileNaming ouiFileNaming = this;
      if (ouiFileNaming.Name == Dialog.Clean("FILE_DEFAULT", (Language) null))
        ouiFileNaming.Name = "";
      ouiFileNaming.Overworld.ShowInputUI = false;
      ouiFileNaming.selectingOptions = false;
      ouiFileNaming.optionsIndex = 0;
      ouiFileNaming.index = 0;
      ouiFileNaming.line = 0;
      string str = Dialog.Clean("name_letters", (Language) null);
      ouiFileNaming.letters = str.Split('\n');
      foreach (char text in str)
      {
        float x = (float) ActiveFont.Measure(text).X;
        if ((double) x > (double) ouiFileNaming.widestLetter)
          ouiFileNaming.widestLetter = x;
      }
      ouiFileNaming.widestLineCount = 0;
      foreach (string letter in ouiFileNaming.letters)
      {
        if (letter.Length > ouiFileNaming.widestLineCount)
          ouiFileNaming.widestLineCount = letter.Length;
      }
      ouiFileNaming.widestLine = (float) ouiFileNaming.widestLineCount * ouiFileNaming.widestLetter;
      ouiFileNaming.lineHeight = ActiveFont.LineHeight;
      ouiFileNaming.lineSpacing = ActiveFont.LineHeight * 0.1f;
      ouiFileNaming.boxPadding = ouiFileNaming.widestLetter;
      ouiFileNaming.optionsScale = 0.75f;
      ouiFileNaming.cancel = Dialog.Clean("name_back", (Language) null);
      ouiFileNaming.space = Dialog.Clean("name_space", (Language) null);
      ouiFileNaming.backspace = Dialog.Clean("name_backspace", (Language) null);
      ouiFileNaming.accept = Dialog.Clean("name_accept", (Language) null);
      ouiFileNaming.cancelWidth = (float) ActiveFont.Measure(ouiFileNaming.cancel).X * ouiFileNaming.optionsScale;
      ouiFileNaming.spaceWidth = (float) ActiveFont.Measure(ouiFileNaming.space).X * ouiFileNaming.optionsScale;
      ouiFileNaming.backspaceWidth = (float) ActiveFont.Measure(ouiFileNaming.backspace).X * ouiFileNaming.optionsScale;
      ouiFileNaming.beginWidth = (float) ActiveFont.Measure(ouiFileNaming.accept).X * ouiFileNaming.optionsScale;
      ouiFileNaming.optionsWidth = (float) ((double) ouiFileNaming.cancelWidth + (double) ouiFileNaming.spaceWidth + (double) ouiFileNaming.backspaceWidth + (double) ouiFileNaming.beginWidth + (double) ouiFileNaming.widestLetter * 3.0);
      ouiFileNaming.boxWidth = Math.Max(ouiFileNaming.widestLine, ouiFileNaming.optionsWidth) + ouiFileNaming.boxPadding * 2f;
      ouiFileNaming.boxHeight = (float) ((double) (ouiFileNaming.letters.Length + 1) * (double) ouiFileNaming.lineHeight + (double) ouiFileNaming.letters.Length * (double) ouiFileNaming.lineSpacing + (double) ouiFileNaming.boxPadding * 3.0);
      ouiFileNaming.Visible = true;
      Vector2 posFrom = ouiFileNaming.Position;
      Vector2 posTo = Vector2.get_Zero();
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        ouiFileNaming.ease = Ease.CubeIn(t);
        ouiFileNaming.Position = Vector2.op_Addition(posFrom, Vector2.op_Multiply(Vector2.op_Subtraction(posTo, posFrom), Ease.CubeInOut(t)));
        yield return (object) null;
      }
      ouiFileNaming.ease = 1f;
      posFrom = (Vector2) null;
      posTo = (Vector2) null;
      yield return (object) 0.2f;
      ouiFileNaming.Focused = true;
      yield return (object) 0.2f;
      ouiFileNaming.wiggler.Start();
    }

    public override IEnumerator Leave(Oui next)
    {
      OuiFileNaming ouiFileNaming = this;
      ouiFileNaming.Overworld.ShowInputUI = true;
      ouiFileNaming.Focused = false;
      Vector2 posFrom = ouiFileNaming.Position;
      Vector2 posTo = new Vector2(0.0f, 1080f);
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        ouiFileNaming.ease = 1f - Ease.CubeIn(t);
        ouiFileNaming.Position = Vector2.op_Addition(posFrom, Vector2.op_Multiply(Vector2.op_Subtraction(posTo, posFrom), Ease.CubeInOut(t)));
        yield return (object) null;
      }
      ouiFileNaming.Visible = false;
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
          for (int index = this.line + 1; index < this.letters.Length; ++index)
          {
            if (this.index < this.letters[index].Length && this.letters[index][this.index] != ' ')
            {
              this.line = index;
              goto label_18;
            }
          }
          this.selectingOptions = true;
label_18:
          if (this.selectingOptions)
          {
            float num1 = (float) this.index * this.widestLetter;
            float num2 = this.boxWidth - this.boxPadding * 2f;
            this.optionsIndex = this.Name.Length == 0 || (double) num1 < (double) this.cancelWidth + ((double) num2 - (double) this.cancelWidth - (double) this.beginWidth - (double) this.backspaceWidth - (double) this.spaceWidth - (double) this.widestLetter * 3.0) / 2.0 ? 0 : ((double) num1 >= (double) num2 - (double) this.beginWidth - (double) this.backspaceWidth - (double) this.widestLetter * 2.0 ? ((double) num1 >= (double) num2 - (double) this.beginWidth - (double) this.widestLetter ? 3 : 2) : 1);
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
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.op_Multiply(Color.get_Black(), 0.8f), this.ease));
      Vector2 vector2 = Vector2.op_Addition(this.boxtopleft, new Vector2(this.boxPadding, this.boxPadding));
      int num1 = 0;
      foreach (string letter in this.letters)
      {
        for (int index = 0; index < letter.Length; ++index)
        {
          bool selected = num1 == this.line && index == this.index && !this.selectingOptions;
          Vector2 scale = Vector2.op_Multiply(Vector2.get_One(), selected ? 1.2f : 1f);
          Vector2 at = Vector2.op_Addition(vector2, Vector2.op_Division(new Vector2(this.widestLetter, this.lineHeight), 2f));
          if (selected)
            at = Vector2.op_Addition(at, Vector2.op_Multiply(new Vector2(0.0f, this.wiggler.Value), 8f));
          this.DrawOptionText(letter[index].ToString(), at, new Vector2(0.5f, 0.5f), scale, selected, false);
          ref __Null local = ref vector2.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + this.widestLetter;
        }
        vector2.X = (__Null) (this.boxtopleft.X + (double) this.boxPadding);
        ref __Null local1 = ref vector2.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + (this.lineHeight + this.lineSpacing);
        ++num1;
      }
      float num2 = this.wiggler.Value * 8f;
      vector2.Y = (__Null) (this.boxtopleft.Y + (double) this.boxHeight - (double) this.lineHeight - (double) this.boxPadding);
      Draw.Rect((float) vector2.X, (float) (vector2.Y - (double) this.boxPadding * 0.5), this.boxWidth - this.boxPadding * 2f, 4f, Color.get_White());
      this.DrawOptionText(this.cancel, Vector2.op_Addition(vector2, new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 0 ? 0.0f : num2))), new Vector2(0.0f, 1f), Vector2.op_Multiply(Vector2.get_One(), this.optionsScale), this.selectingOptions && this.optionsIndex == 0, false);
      vector2.X = (__Null) (this.boxtopleft.X + (double) this.boxWidth - (double) this.backspaceWidth - (double) this.widestLetter - (double) this.spaceWidth - (double) this.widestLetter - (double) this.beginWidth - (double) this.boxPadding);
      this.DrawOptionText(this.space, Vector2.op_Addition(vector2, new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 1 ? 0.0f : num2))), new Vector2(0.0f, 1f), Vector2.op_Multiply(Vector2.get_One(), this.optionsScale), this.selectingOptions && this.optionsIndex == 1, this.Name.Length == 0 || !this.Focused);
      ref __Null local2 = ref vector2.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 + (this.spaceWidth + this.widestLetter);
      this.DrawOptionText(this.backspace, Vector2.op_Addition(vector2, new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 2 ? 0.0f : num2))), new Vector2(0.0f, 1f), Vector2.op_Multiply(Vector2.get_One(), this.optionsScale), this.selectingOptions && this.optionsIndex == 2, this.Name.Length <= 0 || !this.Focused);
      ref __Null local3 = ref vector2.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local3 = ^(float&) ref local3 + (this.backspaceWidth + this.widestLetter);
      this.DrawOptionText(this.accept, Vector2.op_Addition(vector2, new Vector2(0.0f, this.lineHeight + (!this.selectingOptions || this.optionsIndex != 3 ? 0.0f : num2))), new Vector2(0.0f, 1f), Vector2.op_Multiply(Vector2.get_One(), this.optionsScale), this.selectingOptions && this.optionsIndex == 3, this.Name.Length < 1 || !this.Focused);
    }

    private void DrawOptionText(
      string text,
      Vector2 at,
      Vector2 justify,
      Vector2 scale,
      bool selected,
      bool disabled = false)
    {
      int num = !selected ? 0 : ((double) this.pressedTimer > 0.0 ? 1 : 0);
      Color color = disabled ? this.disableColor : this.GetTextColor(selected);
      Color edgeColor = disabled ? Color.Lerp(this.disableColor, Color.get_Black(), 0.7f) : Color.get_Gray();
      if (num != 0)
        ActiveFont.Draw(text, Vector2.op_Addition(at, Vector2.get_UnitY()), justify, scale, color);
      else
        ActiveFont.DrawEdgeOutline(text, at, justify, scale, color, 4f, edgeColor, 0.0f, (Color) null);
    }

    private Color GetTextColor(bool selected)
    {
      if (!selected)
        return this.unselectColor;
      if (!Calc.BetweenInterval(this.timer, 0.1f))
        return this.selectColorB;
      return this.selectColorA;
    }
  }
}
