// Decompiled with JetBrains decompiler
// Type: Celeste.TextMenu
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class TextMenu : Entity
  {
    public static readonly Color HighlightColorA = Calc.HexToColor("84FF54");
    public static readonly Color HighlightColorB = Calc.HexToColor("FCFF59");
    public bool Focused = true;
    public TextMenu.InnerContentMode InnerContent = TextMenu.InnerContentMode.OneColumn;
    private List<TextMenu.Item> items = new List<TextMenu.Item>();
    public int Selection = -1;
    public float ItemSpacing = 4f;
    public float MinWidth = 0.0f;
    public float Alpha = 1f;
    public Color HighlightColor = Color.White;
    public bool AutoScroll = true;
    public Vector2 Justify;
    public Action OnESC;
    public Action OnCancel;
    public Action OnUpdate;
    public Action OnPause;
    public Action OnClose;

    public TextMenu.Item Current
    {
      get
      {
        return this.items.Count <= 0 || this.Selection < 0 ? (TextMenu.Item) null : this.items[this.Selection];
      }
      set
      {
        this.Selection = this.items.IndexOf(value);
      }
    }

    public new float Width { get; private set; }

    public new float Height { get; private set; }

    public float LeftColumnWidth { get; private set; }

    public float RightColumnWidth { get; private set; }

    public float ScrollableMinSize
    {
      get
      {
        return (float) (Engine.Height - 300);
      }
    }

    public TextMenu()
    {
      this.Tag = (int) Tags.PauseUpdate | (int) Tags.HUD;
      this.Position = new Vector2((float) Engine.Width, (float) Engine.Height) / 2f;
      this.Justify = new Vector2(0.5f, 0.5f);
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.AutoScroll)
        return;
      if ((double) this.Height > (double) this.ScrollableMinSize)
        this.Position.Y = this.ScrollTargetY;
      else
        this.Position.Y = 540f;
    }

    public TextMenu Add(TextMenu.Item item)
    {
      this.items.Add(item);
      item.Container = this;
      this.Add((Component) (item.ValueWiggler = Wiggler.Create(0.25f, 3f, (Action<float>) null, false, false)));
      this.Add((Component) (item.SelectWiggler = Wiggler.Create(0.25f, 3f, (Action<float>) null, false, false)));
      item.ValueWiggler.UseRawDeltaTime = item.SelectWiggler.UseRawDeltaTime = true;
      if (this.Selection == -1)
        this.FirstSelection();
      this.RecalculateSize();
      item.Added();
      return this;
    }

    public void Clear()
    {
      this.items = new List<TextMenu.Item>();
    }

    public int IndexOf(TextMenu.Item item)
    {
      return this.items.IndexOf(item);
    }

    public void FirstSelection()
    {
      this.Selection = -1;
      this.MoveSelection(1, false);
    }

    public void MoveSelection(int direction, bool wiggle = false)
    {
      int selection = this.Selection;
      direction = Math.Sign(direction);
      int num = 0;
      foreach (TextMenu.Item obj in this.items)
      {
        if (obj.Hoverable)
          ++num;
      }
      bool flag = num > 2;
      do
      {
        this.Selection += direction;
        if (flag)
        {
          if (this.Selection < 0)
            this.Selection = this.items.Count - 1;
          else if (this.Selection >= this.items.Count)
            this.Selection = 0;
        }
        else if (this.Selection < 0 || this.Selection > this.items.Count - 1)
        {
          this.Selection = Calc.Clamp(this.Selection, 0, this.items.Count - 1);
          break;
        }
      }
      while (!this.Current.Hoverable);
      if (!this.Current.Hoverable)
        this.Selection = selection;
      if (this.Selection == selection || this.Current == null)
        return;
      if (selection >= 0 && this.items[selection] != null && this.items[selection].OnLeave != null)
        this.items[selection].OnLeave();
      if (this.Current.OnEnter != null)
        this.Current.OnEnter();
      if (wiggle)
      {
        Audio.Play(direction > 0 ? "event:/ui/main/rollover_down" : "event:/ui/main/rollover_up");
        this.Current.SelectWiggler.Start();
      }
    }

    public void RecalculateSize()
    {
      this.LeftColumnWidth = this.RightColumnWidth = this.Height = 0.0f;
      foreach (TextMenu.Item obj in this.items)
      {
        if (obj.IncludeWidthInMeasurement)
          this.LeftColumnWidth = Math.Max(this.LeftColumnWidth, obj.LeftWidth());
      }
      foreach (TextMenu.Item obj in this.items)
      {
        if (obj.IncludeWidthInMeasurement)
          this.RightColumnWidth = Math.Max(this.RightColumnWidth, obj.RightWidth());
      }
      foreach (TextMenu.Item obj in this.items)
      {
        if (obj.Visible)
          this.Height += obj.Height() + this.ItemSpacing;
      }
      this.Height -= this.ItemSpacing;
      this.Width = Math.Max(this.MinWidth, this.LeftColumnWidth + this.RightColumnWidth);
    }

    public float GetYOffsetOf(TextMenu.Item item)
    {
      if (item == null)
        return 0.0f;
      float num = 0.0f;
      foreach (TextMenu.Item obj in this.items)
      {
        if (item.Visible)
          num += obj.Height() + this.ItemSpacing;
        if (obj == item)
          break;
      }
      return num - item.Height() * 0.5f - this.ItemSpacing;
    }

    public void Close()
    {
      if (this.Current != null && this.Current.OnLeave != null)
        this.Current.OnLeave();
      if (this.OnClose != null)
        this.OnClose();
      this.RemoveSelf();
    }

    public void CloseAndRun(IEnumerator routine, Action onClose)
    {
      this.Focused = false;
      this.Visible = false;
      this.Add((Component) new Coroutine(this.CloseAndRunRoutine(routine, onClose), true));
    }

    private IEnumerator CloseAndRunRoutine(IEnumerator routine, Action onClose)
    {
      yield return (object) routine;
      if (onClose != null)
        onClose();
      this.Close();
    }

    public override void Update()
    {
      base.Update();
      if (this.OnUpdate != null)
        this.OnUpdate();
      if (this.Focused)
      {
        if (Input.MenuDown.Pressed)
          this.MoveSelection(1, true);
        else if (Input.MenuUp.Pressed)
          this.MoveSelection(-1, true);
        if (this.Current != null)
        {
          if (Input.MenuLeft.Pressed)
            this.Current.LeftPressed();
          if (Input.MenuRight.Pressed)
            this.Current.RightPressed();
          if (Input.MenuConfirm.Pressed)
          {
            this.Current.ConfirmPressed();
            if (this.Current.OnPressed != null)
              this.Current.OnPressed();
          }
          if (Input.MenuJournal.Pressed && this.Current.OnAltPressed != null)
            this.Current.OnAltPressed();
        }
        if (!Input.MenuConfirm.Pressed)
        {
          if (Input.MenuCancel.Pressed && this.OnCancel != null)
            this.OnCancel();
          else if (Input.ESC.Pressed && this.OnESC != null)
            this.OnESC();
          else if (Input.Pause.Pressed && this.OnPause != null)
            this.OnPause();
        }
      }
      foreach (TextMenu.Item obj in this.items)
      {
        if (obj.OnUpdate != null)
          obj.OnUpdate();
        obj.Update();
      }
      if (Settings.Instance.DisableFlashes)
        this.HighlightColor = TextMenu.HighlightColorA;
      else if (Engine.Scene.OnRawInterval(0.1f))
        this.HighlightColor = !(this.HighlightColor == TextMenu.HighlightColorA) ? TextMenu.HighlightColorA : TextMenu.HighlightColorB;
      if (!this.AutoScroll)
        return;
      if ((double) this.Height > (double) this.ScrollableMinSize)
        this.Position.Y += (float) (((double) this.ScrollTargetY - (double) this.Position.Y) * (1.0 - Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
      else
        this.Position.Y = 540f;
    }

    public float ScrollTargetY
    {
      get
      {
        return Calc.Clamp((float) (Engine.Height / 2) + this.Height * this.Justify.Y - this.GetYOffsetOf(this.Current), (float) (Engine.Height - 150) - this.Height * this.Justify.Y, (float) (150.0 + (double) this.Height * (double) this.Justify.Y));
      }
    }

    public override void Render()
    {
      this.RecalculateSize();
      Vector2 vector2 = this.Position - this.Justify * new Vector2(this.Width, this.Height);
      foreach (TextMenu.Item obj in this.items)
      {
        if (obj.Visible)
        {
          float num = obj.Height();
          obj.Render(vector2 + new Vector2(0.0f, (float) ((double) num * 0.5 + (double) obj.SelectWiggler.Value * 8.0)), this.Focused && this.Current == obj);
          vector2.Y += num + this.ItemSpacing;
        }
      }
    }

    public enum InnerContentMode
    {
      OneColumn,
      TwoColumn,
    }

    public abstract class Item
    {
      public bool Selectable = false;
      public bool Visible = true;
      public bool Disabled = false;
      public bool IncludeWidthInMeasurement = true;
      public TextMenu Container;
      public Wiggler SelectWiggler;
      public Wiggler ValueWiggler;
      public Action OnEnter;
      public Action OnLeave;
      public Action OnPressed;
      public Action OnAltPressed;
      public Action OnUpdate;

      public bool Hoverable
      {
        get
        {
          return this.Selectable && this.Visible && !this.Disabled;
        }
      }

      public TextMenu.Item Enter(Action onEnter)
      {
        this.OnEnter = onEnter;
        return this;
      }

      public TextMenu.Item Leave(Action onLeave)
      {
        this.OnLeave = onLeave;
        return this;
      }

      public TextMenu.Item Pressed(Action onPressed)
      {
        this.OnPressed = onPressed;
        return this;
      }

      public TextMenu.Item AltPressed(Action onPressed)
      {
        this.OnAltPressed = onPressed;
        return this;
      }

      public float Width
      {
        get
        {
          return this.LeftWidth() + this.RightWidth();
        }
      }

      public virtual void ConfirmPressed()
      {
      }

      public virtual void LeftPressed()
      {
      }

      public virtual void RightPressed()
      {
      }

      public virtual void Added()
      {
      }

      public virtual void Update()
      {
      }

      public virtual float LeftWidth()
      {
        return 0.0f;
      }

      public virtual float RightWidth()
      {
        return 0.0f;
      }

      public virtual float Height()
      {
        return 0.0f;
      }

      public virtual void Render(Vector2 position, bool highlighted)
      {
      }
    }

    public class Header : TextMenu.Item
    {
      public const float Scale = 2f;
      public string Title;

      public Header(string title)
      {
        this.Title = title;
        this.Selectable = false;
        this.IncludeWidthInMeasurement = false;
      }

      public override float LeftWidth()
      {
        return ActiveFont.Measure(this.Title).X * 2f;
      }

      public override float Height()
      {
        return ActiveFont.LineHeight * 2f;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        float alpha = this.Container.Alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        ActiveFont.DrawEdgeOutline(this.Title, position + new Vector2(this.Container.Width * 0.5f, 0.0f), new Vector2(0.5f, 0.5f), Vector2.One * 2f, Color.Gray * alpha, 4f, Color.DarkSlateBlue * alpha, 2f, strokeColor);
      }
    }

    public class SubHeader : TextMenu.Item
    {
      public const float Scale = 0.6f;
      public string Title;

      public SubHeader(string title)
      {
        this.Title = title;
        this.Selectable = false;
      }

      public override float LeftWidth()
      {
        return ActiveFont.Measure(this.Title).X * 0.6f;
      }

      public override float Height()
      {
        return (float) ((this.Title.Length > 0 ? (double) ActiveFont.LineHeight * 0.600000023841858 : 0.0) + 48.0);
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        if (this.Title.Length <= 0)
          return;
        float alpha = this.Container.Alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        ActiveFont.DrawOutline(this.Title, position + (this.Container.InnerContent == TextMenu.InnerContentMode.TwoColumn ? new Vector2(0.0f, 32f) : new Vector2(this.Container.Width * 0.5f, 32f)), new Vector2(this.Container.InnerContent == TextMenu.InnerContentMode.TwoColumn ? 0.0f : 0.5f, 0.5f), Vector2.One * 0.6f, Color.Gray * alpha, 2f, strokeColor);
      }
    }

    public class Option<T> : TextMenu.Item
    {
      public List<Tuple<string, T>> Values = new List<Tuple<string, T>>();
      private float sine = 0.0f;
      private int lastDir = 0;
      public string Label;
      public int Index;
      public Action<T> OnValueChange;
      public int PreviousIndex;

      public Option(string label)
      {
        this.Label = label;
        this.Selectable = true;
      }

      public TextMenu.Option<T> Add(string label, T value, bool selected = false)
      {
        this.Values.Add(new Tuple<string, T>(label, value));
        if (selected)
          this.PreviousIndex = this.Index = this.Values.Count - 1;
        return this;
      }

      public TextMenu.Option<T> Change(Action<T> action)
      {
        this.OnValueChange = action;
        return this;
      }

      public override void Added()
      {
        this.Container.InnerContent = TextMenu.InnerContentMode.TwoColumn;
      }

      public override void LeftPressed()
      {
        if (this.Index <= 0)
          return;
        Audio.Play("event:/ui/main/button_toggle_off");
        this.PreviousIndex = this.Index;
        --this.Index;
        this.lastDir = -1;
        this.ValueWiggler.Start();
        if (this.OnValueChange != null)
          this.OnValueChange(this.Values[this.Index].Item2);
      }

      public override void RightPressed()
      {
        if (this.Index >= this.Values.Count - 1)
          return;
        Audio.Play("event:/ui/main/button_toggle_on");
        this.PreviousIndex = this.Index;
        ++this.Index;
        this.lastDir = 1;
        this.ValueWiggler.Start();
        if (this.OnValueChange != null)
          this.OnValueChange(this.Values[this.Index].Item2);
      }

      public override void ConfirmPressed()
      {
        if (this.Values.Count != 2)
          return;
        if (this.Index == 0)
          Audio.Play("event:/ui/main/button_toggle_on");
        else
          Audio.Play("event:/ui/main/button_toggle_off");
        this.PreviousIndex = this.Index;
        this.Index = 1 - this.Index;
        this.lastDir = this.Index == 1 ? 1 : -1;
        this.ValueWiggler.Start();
        if (this.OnValueChange != null)
          this.OnValueChange(this.Values[this.Index].Item2);
      }

      public override void Update()
      {
        this.sine += Engine.RawDeltaTime;
      }

      public override float LeftWidth()
      {
        return ActiveFont.Measure(this.Label).X + 32f;
      }

      public override float RightWidth()
      {
        float val1 = 0.0f;
        foreach (Tuple<string, T> tuple in this.Values)
          val1 = Math.Max(val1, ActiveFont.Measure(tuple.Item1).X);
        return val1 + 120f;
      }

      public override float Height()
      {
        return ActiveFont.LineHeight;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        float alpha = this.Container.Alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color1 = this.Disabled ? Color.DarkSlateGray : (highlighted ? this.Container.HighlightColor : Color.White) * alpha;
        ActiveFont.DrawOutline(this.Label, position, new Vector2(0.0f, 0.5f), Vector2.One, color1, 2f, strokeColor);
        if (this.Values.Count <= 0)
          return;
        float num = this.RightWidth();
        ActiveFont.DrawOutline(this.Values[this.Index].Item1, position + new Vector2((float) ((double) this.Container.Width - (double) num * 0.5 + (double) this.lastDir * (double) this.ValueWiggler.Value * 8.0), 0.0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color1, 2f, strokeColor);
        Vector2 vector2 = Vector2.UnitX * (highlighted ? (float) (Math.Sin((double) this.sine * 4.0) * 4.0) : 0.0f);
        bool flag1 = this.Index > 0;
        Color color2 = flag1 ? color1 : Color.DarkSlateGray * alpha;
        ActiveFont.DrawOutline("<", position + new Vector2((float) ((double) this.Container.Width - (double) num + 40.0 + (this.lastDir < 0 ? -(double) this.ValueWiggler.Value * 8.0 : 0.0)), 0.0f) - (flag1 ? vector2 : Vector2.Zero), new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);
        bool flag2 = this.Index < this.Values.Count - 1;
        Color color3 = flag2 ? color1 : Color.DarkSlateGray * alpha;
        ActiveFont.DrawOutline(">", position + new Vector2((float) ((double) this.Container.Width - 40.0 + (this.lastDir > 0 ? (double) this.ValueWiggler.Value * 8.0 : 0.0)), 0.0f) + (flag2 ? vector2 : Vector2.Zero), new Vector2(0.5f, 0.5f), Vector2.One, color3, 2f, strokeColor);
      }
    }

    public class Slider : TextMenu.Option<int>
    {
      public Slider(string label, Func<int, string> values, int min, int max, int value = -1)
        : base(label)
      {
        for (int index = min; index <= max; ++index)
          this.Add(values(index), index, value == index);
      }
    }

    public class OnOff : TextMenu.Option<bool>
    {
      public OnOff(string label, bool on)
        : base(label)
      {
        this.Add(Dialog.Clean("options_off", (Language) null), false, !on);
        this.Add(Dialog.Clean("options_on", (Language) null), true, on);
      }
    }

    public class Setting : TextMenu.Item
    {
      public string ConfirmSfx = "event:/ui/main/button_select";
      public List<object> Values = new List<object>();
      public string Label;

      public Setting(string label, string value = "")
      {
        this.Label = label;
        this.Values.Add((object) value);
        this.Selectable = true;
      }

      public Setting(string label, Keys key)
        : this(label, "")
      {
        this.Set(new List<Keys>() { key });
      }

      public Setting(string label, List<Keys> keys)
        : this(label, "")
      {
        this.Set(keys);
      }

      public Setting(string label, Buttons btn)
        : this(label, "")
      {
        this.Set(new List<Buttons>() { btn });
      }

      public Setting(string label, List<Buttons> buttons)
        : this(label, "")
      {
        this.Set(buttons);
      }

      public void Set(List<Keys> keys)
      {
        this.Values.Clear();
        int index1 = 0;
        for (int index2 = Math.Min(4, keys.Count); index1 < index2; ++index1)
        {
          if (keys[index1] != Keys.None)
          {
            MTexture mtexture = Input.GuiKey(keys[index1], (string) null);
            if (mtexture != null)
            {
              this.Values.Add((object) mtexture);
            }
            else
            {
              string str1 = keys[index1].ToString();
              string str2 = "";
              for (int index3 = 0; index3 < str1.Length; ++index3)
              {
                if (index3 > 0 && char.IsUpper(str1[index3]))
                  str2 += " ";
                str2 += str1[index3].ToString();
              }
              this.Values.Add((object) str2);
            }
          }
        }
      }

      public void Set(List<Buttons> buttons)
      {
        this.Values.Clear();
        int index1 = 0;
        for (int index2 = Math.Min(4, buttons.Count); index1 < index2; ++index1)
        {
          MTexture mtexture = Input.GuiSingleButton(buttons[index1], (string) null);
          if (mtexture != null)
          {
            this.Values.Add((object) mtexture);
          }
          else
          {
            string str1 = buttons[index1].ToString();
            string str2 = "";
            for (int index3 = 0; index3 < str1.Length; ++index3)
            {
              if (index3 > 0 && char.IsUpper(str1[index3]))
                str2 += " ";
              str2 += str1[index3].ToString();
            }
            this.Values.Add((object) str2);
          }
        }
      }

      public override void Added()
      {
        this.Container.InnerContent = TextMenu.InnerContentMode.TwoColumn;
      }

      public override void ConfirmPressed()
      {
        Audio.Play(this.ConfirmSfx);
        base.ConfirmPressed();
      }

      public override float LeftWidth()
      {
        return ActiveFont.Measure(this.Label).X;
      }

      public override float RightWidth()
      {
        float num = 0.0f;
        foreach (object obj in this.Values)
        {
          if (obj is MTexture)
            num += (float) (obj as MTexture).Width;
          else if (obj is string)
            num += (float) ((double) ActiveFont.Measure(obj as string).X * 0.699999988079071 + 16.0);
        }
        return num;
      }

      public override float Height()
      {
        return ActiveFont.LineHeight * 1.2f;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        float alpha = this.Container.Alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color = this.Disabled ? Color.DarkSlateGray : (highlighted ? this.Container.HighlightColor : Color.White) * alpha;
        ActiveFont.DrawOutline(this.Label, position, new Vector2(0.0f, 0.5f), Vector2.One, color, 2f, strokeColor);
        float num1 = this.RightWidth();
        foreach (object obj in this.Values)
        {
          if (obj is MTexture)
          {
            MTexture mtexture = obj as MTexture;
            mtexture.DrawJustified(position + new Vector2(this.Container.Width - num1, 0.0f), new Vector2(0.0f, 0.5f), Color.White * alpha);
            num1 -= (float) mtexture.Width;
          }
          else if (obj is string)
          {
            string text = obj as string;
            float num2 = (float) ((double) ActiveFont.Measure(obj as string).X * 0.699999988079071 + 16.0);
            ActiveFont.DrawOutline(text, position + new Vector2((float) ((double) this.Container.Width - (double) num1 + (double) num2 * 0.5), 0.0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, Color.LightGray * alpha, 2f, strokeColor);
            num1 -= num2;
          }
        }
      }
    }

    public class Button : TextMenu.Item
    {
      public string ConfirmSfx = "event:/ui/main/button_select";
      public string Label;
      public bool AlwaysCenter;

      public Button(string label)
      {
        this.Label = label;
        this.Selectable = true;
      }

      public override void ConfirmPressed()
      {
        Audio.Play(this.ConfirmSfx);
        base.ConfirmPressed();
      }

      public override float LeftWidth()
      {
        return ActiveFont.Measure(this.Label).X;
      }

      public override float Height()
      {
        return ActiveFont.LineHeight;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        float alpha = this.Container.Alpha;
        Color color = this.Disabled ? Color.DarkSlateGray : (highlighted ? this.Container.HighlightColor : Color.White) * alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        bool flag = this.Container.InnerContent == TextMenu.InnerContentMode.TwoColumn && !this.AlwaysCenter;
        ActiveFont.DrawOutline(this.Label, position + (flag ? Vector2.Zero : new Vector2(this.Container.Width * 0.5f, 0.0f)), !flag || this.AlwaysCenter ? new Vector2(0.5f, 0.5f) : new Vector2(0.0f, 0.5f), Vector2.One, color, 2f, strokeColor);
      }
    }

    public class LanguageButton : TextMenu.Item
    {
      public string ConfirmSfx = "event:/ui/main/button_select";
      public string Label;
      public Language Language;
      public bool AlwaysCenter;

      public LanguageButton(string label, Language language)
      {
        this.Label = label;
        this.Language = language;
        this.Selectable = true;
      }

      public override void ConfirmPressed()
      {
        Audio.Play(this.ConfirmSfx);
        base.ConfirmPressed();
      }

      public override float LeftWidth()
      {
        return ActiveFont.Measure(this.Label).X;
      }

      public override float RightWidth()
      {
        return (float) ((double) this.Language.FontSize.Measure(this.Language.Label).X + 96.0 + 32.0);
      }

      public override float Height()
      {
        return ActiveFont.LineHeight;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        float alpha = this.Container.Alpha;
        Color color = this.Disabled ? Color.DarkSlateGray : (highlighted ? this.Container.HighlightColor : Color.White) * alpha;
        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        ActiveFont.DrawOutline(this.Label, position, new Vector2(0.0f, 0.5f), Vector2.One, color, 2f, strokeColor);
        this.Language.FontSize.DrawOutline(this.Language.Label, position + new Vector2(this.Container.Width - 32f, 0.0f), new Vector2(1f, 0.5f), Vector2.One, color, 2f, strokeColor);
        if (this.Language.Icon == null)
          return;
        this.Language.Icon.DrawJustified(position + new Vector2(this.Container.Width - this.RightWidth(), 0.0f), new Vector2(0.0f, 0.5f), Color.White, 64f / (float) this.Language.Icon.Width);
      }
    }
  }
}

