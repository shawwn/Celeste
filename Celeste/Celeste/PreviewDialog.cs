// Decompiled with JetBrains decompiler
// Type: Celeste.PreviewDialog
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class PreviewDialog : Scene
  {
    private List<string> list = new List<string>();
    private Vector2 listScroll = new Vector2(64f, 64f);
    private List<object> elements = new List<object>();
    private Vector2 textboxScroll = new Vector2(0.0f, 0.0f);
    private Language language;
    private const float scale = 0.6f;
    private string current;
    private float delay;

    public PreviewDialog(Language language = null, float listScroll = 64f, float textboxScroll = 0.0f, string dialog = null)
    {
      this.listScroll.Y = (__Null) (double) listScroll;
      this.textboxScroll.Y = (__Null) (double) textboxScroll;
      if (language != null)
        this.SetLanguage(language);
      if (dialog != null)
        this.SetCurrent(dialog);
      this.Add((Monocle.Renderer) new PreviewDialog.Renderer(this));
    }

    public override void Update()
    {
      if (!Engine.Instance.get_IsActive())
        this.delay = 0.1f;
      else if ((double) this.delay > 0.0)
      {
        this.delay -= Engine.DeltaTime;
      }
      else
      {
        if (this.current != null)
        {
          float num = 1f;
          foreach (object element in this.elements)
          {
            Textbox textbox = element as Textbox;
            if (textbox != null)
            {
              textbox.RenderOffset = Vector2.op_Addition(this.textboxScroll, Vector2.op_Multiply(Vector2.get_UnitY(), num));
              num += 300f;
              if (textbox.Scene != null)
                textbox.Update();
            }
            else
              num += (float) (this.language.FontSize.LineHeight + 50);
          }
          ref __Null local1 = ref this.textboxScroll.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local1 = ^(float&) ref local1 + (float) MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
          ref __Null local2 = ref this.textboxScroll.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local2 = ^(float&) ref local2 - (float) (Celeste.Input.Aim.Value.Y * (double) Engine.DeltaTime * (double) ActiveFont.LineHeight * 20.0);
          this.textboxScroll.Y = (__Null) (double) Calc.Clamp((float) this.textboxScroll.Y, 716f - num, 64f);
          if (MInput.Keyboard.Pressed((Keys) 27) || Celeste.Input.MenuConfirm.Pressed)
            this.ClearTextboxes();
          else if (MInput.Keyboard.Pressed((Keys) 32))
          {
            string current = this.current;
            this.ClearTextboxes();
            int index = this.list.IndexOf(current) + 1;
            if (index < this.list.Count)
              this.SetCurrent(this.list[index]);
          }
        }
        else
        {
          ref __Null local1 = ref this.listScroll.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local1 = ^(float&) ref local1 + (float) MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
          ref __Null local2 = ref this.listScroll.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local2 = ^(float&) ref local2 - (float) (Celeste.Input.Aim.Value.Y * (double) Engine.DeltaTime * (double) ActiveFont.LineHeight * 20.0);
          this.listScroll.Y = (__Null) (double) Calc.Clamp((float) this.listScroll.Y, (float) (1016.0 - (double) this.list.Count * (double) ActiveFont.LineHeight * 0.600000023841858), 64f);
          if (this.language != null)
          {
            if (MInput.Mouse.PressedLeftButton)
            {
              for (int i = 0; i < this.list.Count; ++i)
              {
                if (this.MouseOverOption(i))
                {
                  this.SetCurrent(this.list[i]);
                  break;
                }
              }
            }
            if (MInput.Keyboard.Pressed((Keys) 27) || Celeste.Input.MenuConfirm.Pressed)
            {
              this.listScroll = new Vector2(64f, 64f);
              this.language = (Language) null;
            }
          }
          else if (MInput.Mouse.PressedLeftButton)
          {
            int i = 0;
            foreach (KeyValuePair<string, Language> language in Dialog.Languages)
            {
              if (this.MouseOverOption(i))
              {
                this.SetLanguage(language.Value);
                this.listScroll = new Vector2(64f, 64f);
                break;
              }
              ++i;
            }
          }
        }
        if (MInput.Keyboard.Pressed((Keys) 113))
        {
          Celeste.Celeste.ReloadPortraits();
          Engine.Scene = (Scene) new PreviewDialog(this.language, (float) this.listScroll.Y, (float) this.textboxScroll.Y, this.current);
        }
        if (!MInput.Keyboard.Pressed((Keys) 112) || this.language == null)
          return;
        Celeste.Celeste.ReloadDialog();
        Engine.Scene = (Scene) new PreviewDialog(Dialog.Languages[this.language.Id], (float) this.listScroll.Y, (float) this.textboxScroll.Y, this.current);
      }
    }

    private void ClearTextboxes()
    {
      foreach (object element in this.elements)
      {
        if (element is Textbox)
          this.Remove((Entity) (element as Textbox));
      }
      this.current = (string) null;
      this.textboxScroll = Vector2.get_Zero();
    }

    private void SetCurrent(string id)
    {
      this.current = id;
      this.elements.Clear();
      Textbox textbox1 = (Textbox) null;
      int page = 0;
      while (true)
      {
        Textbox textbox2 = new Textbox(id, this.language, new Func<IEnumerator>[0]);
        if (textbox2.SkipToPage(page))
        {
          if (textbox1 != null)
          {
            for (int index = textbox1.Start + 1; index <= textbox2.Start && index < textbox1.Nodes.Count; ++index)
            {
              FancyText.Trigger node = textbox1.Nodes[index] as FancyText.Trigger;
              if (node != null)
                this.elements.Add((object) ((node.Silent ? (object) "Silent " : (object) "").ToString() + "Trigger [" + (object) node.Index + "] " + node.Label));
            }
          }
          this.Add((Entity) textbox2);
          this.elements.Add((object) textbox2);
          textbox2.RenderOffset = Vector2.op_Addition(this.textboxScroll, Vector2.op_Multiply(Vector2.get_UnitY(), (float) (1 + page * 300)));
          textbox1 = textbox2;
          ++page;
        }
        else
          break;
      }
    }

    private void SetLanguage(Language lan)
    {
      this.language = lan;
      this.list.Clear();
      bool flag = false;
      foreach (KeyValuePair<string, string> keyValuePair in this.language.Dialog)
      {
        if (!flag && keyValuePair.Key.StartsWith("CH0", StringComparison.OrdinalIgnoreCase))
          flag = true;
        if (flag && !keyValuePair.Key.StartsWith("poem_", StringComparison.OrdinalIgnoreCase) && !keyValuePair.Key.StartsWith("journal_", StringComparison.OrdinalIgnoreCase))
          this.list.Add(keyValuePair.Key);
      }
    }

    public Vector2 Mouse
    {
      get
      {
        return Vector2.Transform(new Vector2((float) ((MouseState) ref MInput.Mouse.CurrentState).get_X(), (float) ((MouseState) ref MInput.Mouse.CurrentState).get_Y()), Matrix.Invert(Engine.ScreenMatrix));
      }
    }

    private void RenderContent()
    {
      Draw.Rect(0.0f, 0.0f, 960f, 1080f, Color.op_Multiply(Color.get_DarkSlateGray(), 0.25f));
      if (this.current != null)
      {
        int num1 = 1;
        int num2 = 0;
        foreach (object element in this.elements)
        {
          Textbox textbox = element as Textbox;
          if (textbox != null)
          {
            if (textbox.Opened && this.language.Font.Sizes.Count > 0)
            {
              textbox.Render();
              this.language.Font.DrawOutline(this.language.FontFaceSize, "#" + num1.ToString(), Vector2.op_Addition(textbox.RenderOffset, new Vector2(32f, 64f)), Vector2.get_Zero(), Vector2.op_Multiply(Vector2.get_One(), 0.5f), Color.get_White(), 2f, Color.get_Black());
              ++num1;
              num2 += 300;
            }
          }
          else
          {
            this.language.Font.DrawOutline(this.language.FontFaceSize, element.ToString(), Vector2.op_Addition(this.textboxScroll, new Vector2(128f, (float) (num2 + 50 + this.language.FontSize.LineHeight))), new Vector2(0.0f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), 0.5f), Color.get_White(), 2f, Color.get_Black());
            num2 += this.language.FontSize.LineHeight + 50;
          }
        }
        ActiveFont.DrawOutline(this.current, new Vector2(1888f, 32f), new Vector2(1f, 0.0f), Vector2.op_Multiply(Vector2.get_One(), 0.5f), Color.get_Red(), 2f, Color.get_Black());
      }
      else if (this.language != null)
      {
        int i = 0;
        foreach (string text in this.list)
        {
          if (this.language.Font.Sizes.Count > 0)
            this.language.Font.Draw(this.language.FontFaceSize, text, Vector2.op_Addition(this.listScroll, new Vector2(0.0f, (float) ((double) i * (double) ActiveFont.LineHeight * 0.600000023841858))), Vector2.get_Zero(), Vector2.op_Multiply(Vector2.get_One(), 0.6f), this.MouseOverOption(i) ? Color.get_White() : Color.get_Gray());
          ++i;
        }
      }
      else
      {
        int i = 0;
        foreach (KeyValuePair<string, Language> language in Dialog.Languages)
        {
          language.Value.FontSize.Draw(language.Value.Label, Vector2.op_Addition(this.listScroll, new Vector2(0.0f, (float) ((double) i * (double) ActiveFont.LineHeight * 0.600000023841858))), Vector2.get_Zero(), Vector2.op_Multiply(Vector2.get_One(), 0.6f), this.MouseOverOption(i) ? Color.get_White() : Color.get_Gray());
          ++i;
        }
      }
      Draw.Rect((float) (this.Mouse.X - 12.0), (float) (this.Mouse.Y - 4.0), 24f, 8f, Color.get_Red());
      Draw.Rect((float) (this.Mouse.X - 4.0), (float) (this.Mouse.Y - 12.0), 8f, 24f, Color.get_Red());
    }

    private bool MouseOverOption(int i)
    {
      if (this.Mouse.X > this.listScroll.X && this.Mouse.Y > this.listScroll.Y + (double) i * (double) ActiveFont.LineHeight * 0.600000023841858 && (double) MInput.Mouse.X < 960.0)
        return this.Mouse.Y < this.listScroll.Y + (double) (i + 1) * (double) ActiveFont.LineHeight * 0.600000023841858;
      return false;
    }

    private class Renderer : HiresRenderer
    {
      public PreviewDialog previewer;

      public Renderer(PreviewDialog previewer)
      {
        this.previewer = previewer;
      }

      public override void RenderContent(Scene scene)
      {
        HiresRenderer.BeginRender((BlendState) null, (SamplerState) null);
        this.previewer.RenderContent();
        HiresRenderer.EndRender();
      }
    }
  }
}
