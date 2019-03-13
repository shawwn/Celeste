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
    private string current = (string) null;
    private List<object> elements = new List<object>();
    private Vector2 textboxScroll = new Vector2(0.0f, 0.0f);
    private float delay = 0.0f;
    private Language language;
    private const float scale = 0.6f;

    public PreviewDialog(Language language = null, float listScroll = 64f, float textboxScroll = 0.0f, string dialog = null)
    {
      this.listScroll.Y = listScroll;
      this.textboxScroll.Y = textboxScroll;
      if (language != null)
        this.SetLanguage(language);
      if (dialog != null)
        this.SetCurrent(dialog);
      this.Add((Monocle.Renderer) new PreviewDialog.Renderer(this));
    }

    public override void Update()
    {
      if (!Engine.Instance.IsActive)
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
              textbox.RenderOffset = this.textboxScroll + Vector2.UnitY * num;
              num += 300f;
              if (textbox.Scene != null)
                textbox.Update();
            }
            else
              num += (float) (this.language.FontSize.LineHeight + 50);
          }
          this.textboxScroll.Y += (float) MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
          this.textboxScroll.Y -= (float) ((double) Input.Aim.Value.Y * (double) Engine.DeltaTime * (double) ActiveFont.LineHeight * 20.0);
          this.textboxScroll.Y = Calc.Clamp(this.textboxScroll.Y, 716f - num, 64f);
          if (MInput.Keyboard.Pressed(Keys.Escape) || Input.MenuConfirm.Pressed)
            this.ClearTextboxes();
          else if (MInput.Keyboard.Pressed(Keys.Space))
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
          this.listScroll.Y += (float) MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
          this.listScroll.Y -= (float) ((double) Input.Aim.Value.Y * (double) Engine.DeltaTime * (double) ActiveFont.LineHeight * 20.0);
          this.listScroll.Y = Calc.Clamp(this.listScroll.Y, (float) (1016.0 - (double) this.list.Count * (double) ActiveFont.LineHeight * 0.600000023841858), 64f);
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
            if (MInput.Keyboard.Pressed(Keys.Escape) || Input.MenuConfirm.Pressed)
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
        if (MInput.Keyboard.Pressed(Keys.F2))
        {
          Celeste.ReloadPortraits();
          Engine.Scene = (Scene) new PreviewDialog(this.language, this.listScroll.Y, this.textboxScroll.Y, this.current);
        }
        if (!MInput.Keyboard.Pressed(Keys.F1) || this.language == null)
          return;
        Celeste.ReloadDialog();
        Engine.Scene = (Scene) new PreviewDialog(Dialog.Languages[this.language.Id], this.listScroll.Y, this.textboxScroll.Y, this.current);
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
      this.textboxScroll = Vector2.Zero;
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
          textbox2.RenderOffset = this.textboxScroll + Vector2.UnitY * (float) (1 + page * 300);
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
        return Vector2.Transform(new Vector2((float) MInput.Mouse.CurrentState.X, (float) MInput.Mouse.CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
      }
    }

    private void RenderContent()
    {
      Draw.Rect(0.0f, 0.0f, 960f, 1080f, Color.DarkSlateGray * 0.25f);
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
              this.language.Font.DrawOutline(this.language.FontFaceSize, "#" + num1.ToString(), textbox.RenderOffset + new Vector2(32f, 64f), Vector2.Zero, Vector2.One * 0.5f, Color.White, 2f, Color.Black);
              ++num1;
              num2 += 300;
            }
          }
          else
          {
            this.language.Font.DrawOutline(this.language.FontFaceSize, element.ToString(), this.textboxScroll + new Vector2(128f, (float) (num2 + 50 + this.language.FontSize.LineHeight)), new Vector2(0.0f, 0.5f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
            num2 += this.language.FontSize.LineHeight + 50;
          }
        }
        ActiveFont.DrawOutline(this.current, new Vector2(1888f, 32f), new Vector2(1f, 0.0f), Vector2.One * 0.5f, Color.Red, 2f, Color.Black);
      }
      else if (this.language != null)
      {
        int i = 0;
        foreach (string text in this.list)
        {
          if (this.language.Font.Sizes.Count > 0)
            this.language.Font.Draw(this.language.FontFaceSize, text, this.listScroll + new Vector2(0.0f, (float) ((double) i * (double) ActiveFont.LineHeight * 0.600000023841858)), Vector2.Zero, Vector2.One * 0.6f, this.MouseOverOption(i) ? Color.White : Color.Gray);
          ++i;
        }
      }
      else
      {
        int i = 0;
        foreach (KeyValuePair<string, Language> language in Dialog.Languages)
        {
          language.Value.FontSize.Draw(language.Value.Label, this.listScroll + new Vector2(0.0f, (float) ((double) i * (double) ActiveFont.LineHeight * 0.600000023841858)), Vector2.Zero, Vector2.One * 0.6f, this.MouseOverOption(i) ? Color.White : Color.Gray);
          ++i;
        }
      }
      Draw.Rect(this.Mouse.X - 12f, this.Mouse.Y - 4f, 24f, 8f, Color.Red);
      Draw.Rect(this.Mouse.X - 4f, this.Mouse.Y - 12f, 8f, 24f, Color.Red);
    }

    private bool MouseOverOption(int i)
    {
      return (double) this.Mouse.X > (double) this.listScroll.X && (double) this.Mouse.Y > (double) this.listScroll.Y + (double) i * (double) ActiveFont.LineHeight * 0.600000023841858 && (double) MInput.Mouse.X < 960.0 && (double) this.Mouse.Y < (double) this.listScroll.Y + (double) (i + 1) * (double) ActiveFont.LineHeight * 0.600000023841858;
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

