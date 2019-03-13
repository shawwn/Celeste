// Decompiled with JetBrains decompiler
// Type: Celeste.FancyText
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Celeste
{
  public class FancyText
  {
    public static Color DefaultColor = Color.get_LightGray();
    private FancyText.Text group = new FancyText.Text();
    private float currentScale = 1f;
    private float currentDelay = 0.01f;
    public const float CharacterDelay = 0.01f;
    public const float PeriodDelay = 0.3f;
    public const float CommaDelay = 0.15f;
    public const float ShakeDistance = 2f;
    private Language language;
    private string text;
    private int maxLineWidth;
    private int linesPerPage;
    private PixelFont font;
    private PixelFontSize size;
    private Color defaultColor;
    private float startFade;
    private int currentLine;
    private int currentPage;
    private float currentPosition;
    private Color currentColor;
    private bool currentShake;
    private bool currentWave;
    private bool currentImpact;
    private bool currentMessedUp;
    private int currentCharIndex;

    public static FancyText.Text Parse(
      string text,
      int maxLineWidth,
      int linesPerPage,
      float startFade = 1f,
      Color? defaultColor = null,
      Language language = null)
    {
      return new FancyText(text, maxLineWidth, linesPerPage, startFade, defaultColor.HasValue ? defaultColor.Value : FancyText.DefaultColor, language).Parse();
    }

    private FancyText(
      string text,
      int maxLineWidth,
      int linesPerPage,
      float startFade,
      Color defaultColor,
      Language language)
    {
      this.text = text;
      this.maxLineWidth = maxLineWidth;
      this.linesPerPage = linesPerPage < 0 ? int.MaxValue : linesPerPage;
      this.startFade = startFade;
      this.defaultColor = this.currentColor = defaultColor;
      if (language == null)
        language = Dialog.Language;
      this.language = language;
      this.group.Nodes = new List<FancyText.Node>();
      this.group.Font = this.font = Fonts.Faces[language.FontFace];
      this.group.BaseSize = language.FontFaceSize;
      this.size = this.font.Get(this.group.BaseSize);
    }

    private FancyText.Text Parse()
    {
      string[] strArray1 = Regex.Split(this.text, this.language.SplitRegex);
      string[] strArray2 = new string[strArray1.Length];
      int num1 = 0;
      for (int index = 0; index < strArray1.Length; ++index)
      {
        if (!string.IsNullOrEmpty(strArray1[index]))
          strArray2[num1++] = strArray1[index];
      }
      Stack<Color> colorStack = new Stack<Color>();
      FancyText.Portrait[] portraitArray = new FancyText.Portrait[2];
      for (int index1 = 0; index1 < num1; ++index1)
      {
        if (strArray2[index1] == "{")
        {
          int num2 = index1 + 1;
          string[] strArray3 = strArray2;
          int index2 = num2;
          index1 = index2 + 1;
          string s = strArray3[index2];
          List<string> stringList = new List<string>();
          for (; index1 < strArray2.Length && strArray2[index1] != "}"; ++index1)
          {
            if (!string.IsNullOrWhiteSpace(strArray2[index1]))
              stringList.Add(strArray2[index1]);
          }
          float result1 = 0.0f;
          if (float.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
            this.group.Nodes.Add((FancyText.Node) new FancyText.Wait()
            {
              Duration = result1
            });
          else if (s[0] == '#')
          {
            string hex = "";
            if (s.Length > 1)
              hex = s.Substring(1);
            else if (stringList.Count > 0)
              hex = stringList[0];
            if (string.IsNullOrEmpty(hex))
            {
              this.currentColor = colorStack.Count <= 0 ? this.defaultColor : colorStack.Pop();
            }
            else
            {
              colorStack.Push(this.currentColor);
              this.currentColor = !(hex == "red") ? (!(hex == "green") ? (!(hex == "blue") ? Calc.HexToColor(hex) : Color.get_Blue()) : Color.get_Green()) : Color.get_Red();
            }
          }
          else if (s == "break")
          {
            this.CalcLineWidth();
            ++this.currentPage;
            ++this.group.Pages;
            this.currentLine = 0;
            this.currentPosition = 0.0f;
            this.group.Nodes.Add((FancyText.Node) new FancyText.NewPage());
          }
          else if (s == "n")
            this.AddNewLine();
          else if (s == ">>")
          {
            float result2;
            this.currentDelay = stringList.Count <= 0 || !float.TryParse(stringList[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2) ? 0.01f : 0.01f / result2;
          }
          else if (s.Equals("/>>"))
            this.currentDelay = 0.01f;
          else if (s.Equals("anchor"))
          {
            FancyText.Anchors result2;
            if (Enum.TryParse<FancyText.Anchors>(stringList[0], true, out result2))
              this.group.Nodes.Add((FancyText.Node) new FancyText.Anchor()
              {
                Position = result2
              });
          }
          else if (s.Equals("portrait") || s.Equals("left") || s.Equals("right"))
          {
            if (s.Equals("portrait") && stringList.Count > 0 && stringList[0].Equals("none"))
            {
              this.group.Nodes.Add((FancyText.Node) new FancyText.Portrait());
            }
            else
            {
              FancyText.Portrait portrait;
              if (s.Equals("left"))
                portrait = portraitArray[0];
              else if (s.Equals("right"))
              {
                portrait = portraitArray[1];
              }
              else
              {
                portrait = new FancyText.Portrait();
                foreach (string str in stringList)
                {
                  if (str.Equals("upsidedown"))
                    portrait.UpsideDown = true;
                  else if (str.Equals("flip"))
                    portrait.Flipped = true;
                  else if (str.Equals("left"))
                    portrait.Side = -1;
                  else if (str.Equals("right"))
                    portrait.Side = 1;
                  else if (str.Equals("pop"))
                    portrait.Pop = true;
                  else if (portrait.Sprite == null)
                    portrait.Sprite = str;
                  else
                    portrait.Animation = str;
                }
              }
              if (GFX.PortraitsSpriteBank.Has(portrait.SpriteId))
              {
                XmlElement xml1 = GFX.PortraitsSpriteBank.SpriteData[portrait.SpriteId].Sources[0].XML;
                if (xml1 != null)
                {
                  portrait.SfxEvent = "event:/char/dialogue/" + xml1.Attr("sfx", "");
                  if (xml1.HasChild("sfxs"))
                  {
                    foreach (object obj in (XmlNode) xml1["sfxs"])
                    {
                      XmlElement xml2 = obj as XmlElement;
                      if (xml2 != null && xml2.Name.Equals(portrait.Animation, StringComparison.InvariantCultureIgnoreCase))
                      {
                        portrait.SfxExpression = xml2.AttrInt("index");
                        break;
                      }
                    }
                  }
                }
              }
              this.group.Nodes.Add((FancyText.Node) portrait);
              portraitArray[portrait.Side > 0 ? 1 : 0] = portrait;
            }
          }
          else if (s.Equals("trigger") || s.Equals("silent_trigger"))
          {
            string str = "";
            for (int index3 = 1; index3 < stringList.Count; ++index3)
              str = str + stringList[index3] + " ";
            int result2;
            if (int.TryParse(stringList[0], out result2) && result2 >= 0)
              this.group.Nodes.Add((FancyText.Node) new FancyText.Trigger()
              {
                Index = result2,
                Silent = s.StartsWith("silent"),
                Label = str
              });
          }
          else if (s.Equals("*"))
            this.currentShake = true;
          else if (s.Equals("/*"))
            this.currentShake = false;
          else if (s.Equals("~"))
            this.currentWave = true;
          else if (s.Equals("/~"))
            this.currentWave = false;
          else if (s.Equals("!"))
            this.currentImpact = true;
          else if (s.Equals("/!"))
            this.currentImpact = false;
          else if (s.Equals("%"))
            this.currentMessedUp = true;
          else if (s.Equals("/%"))
            this.currentMessedUp = false;
          else if (s.Equals("big"))
            this.currentScale = 1.5f;
          else if (s.Equals("/big"))
            this.currentScale = 1f;
          else if (s.Equals("s"))
          {
            int result2 = 1;
            if (stringList.Count > 0)
              int.TryParse(stringList[0], out result2);
            this.currentPosition += (float) (5 * result2);
          }
          else if (s.Equals("savedata"))
          {
            if (SaveData.Instance == null)
            {
              if (stringList[0].Equals("name", StringComparison.OrdinalIgnoreCase))
                this.AddWord("Madeline");
              else
                this.AddWord("[SD:" + stringList[0] + "]");
            }
            else
              this.AddWord(typeof (SaveData).GetField(stringList[0]).GetValue((object) SaveData.Instance).ToString());
          }
        }
        else
          this.AddWord(strArray2[index1]);
      }
      this.CalcLineWidth();
      return this.group;
    }

    private void CalcLineWidth()
    {
      FancyText.Char @char = (FancyText.Char) null;
      int index;
      for (index = this.group.Nodes.Count - 1; index >= 0 && @char == null; --index)
      {
        if (this.group.Nodes[index] is FancyText.Char)
          @char = this.group.Nodes[index] as FancyText.Char;
        else if (this.group.Nodes[index] is FancyText.NewLine || this.group.Nodes[index] is FancyText.NewPage)
          return;
      }
      if (@char == null)
        return;
      float num = @char.Position + (float) this.size.Get(@char.Character).XAdvance * @char.Scale;
      @char.LineWidth = num;
      for (; index >= 0 && !(this.group.Nodes[index] is FancyText.NewLine) && !(this.group.Nodes[index] is FancyText.NewPage); --index)
      {
        if (this.group.Nodes[index] is FancyText.Char)
          (this.group.Nodes[index] as FancyText.Char).LineWidth = num;
      }
    }

    private void AddNewLine()
    {
      this.CalcLineWidth();
      ++this.currentLine;
      this.currentPosition = 0.0f;
      ++this.group.Lines;
      if (this.currentLine > this.linesPerPage)
      {
        ++this.group.Pages;
        ++this.currentPage;
        this.currentLine = 0;
        this.group.Nodes.Add((FancyText.Node) new FancyText.NewPage());
      }
      else
        this.group.Nodes.Add((FancyText.Node) new FancyText.NewLine());
    }

    private void AddWord(string word)
    {
      if ((double) this.currentPosition + (double) ((float) this.size.Measure(word).X * this.currentScale) > (double) this.maxLineWidth)
        this.AddNewLine();
      for (int index = 0; index < word.Length; ++index)
      {
        if (((double) this.currentPosition != 0.0 || word[index] != ' ') && word[index] != '\\')
        {
          PixelFontCharacter pixelFontCharacter = this.size.Get((int) word[index]);
          if (pixelFontCharacter != null)
          {
            float num1 = 0.0f;
            if (index == word.Length - 1 && (index == 0 || word[index - 1] != '\\'))
            {
              if (this.Contains(this.language.CommaCharacters, word[index]))
                num1 = 0.15f;
              else if (this.Contains(this.language.PeriodCharacters, word[index]))
                num1 = 0.3f;
            }
            this.group.Nodes.Add((FancyText.Node) new FancyText.Char()
            {
              Index = this.currentCharIndex++,
              Character = (int) word[index],
              Position = this.currentPosition,
              Line = this.currentLine,
              Page = this.currentPage,
              Delay = (this.currentImpact ? 0.0035f : this.currentDelay + num1),
              Color = this.currentColor,
              Scale = this.currentScale,
              Rotation = (this.currentMessedUp ? (float) Calc.Random.Choose<int>(-1, 1) * Calc.Random.Choose<float>(0.1745329f, 0.3490658f) : 0.0f),
              YOffset = (this.currentMessedUp ? (float) Calc.Random.Choose<int>(-3, -6, 3, 6) : 0.0f),
              Fade = this.startFade,
              Shake = this.currentShake,
              Impact = this.currentImpact,
              Wave = this.currentWave,
              IsPunctuation = (this.Contains(this.language.CommaCharacters, word[index]) || this.Contains(this.language.PeriodCharacters, word[index]))
            });
            this.currentPosition += (float) pixelFontCharacter.XAdvance * this.currentScale;
            int num2;
            if (index < word.Length - 1 && pixelFontCharacter.Kerning.TryGetValue((int) word[index], out num2))
              this.currentPosition += (float) num2 * this.currentScale;
          }
        }
      }
    }

    private bool Contains(string str, char character)
    {
      for (int index = 0; index < str.Length; ++index)
      {
        if ((int) str[index] == (int) character)
          return true;
      }
      return false;
    }

    public class Node
    {
    }

    public class Char : FancyText.Node
    {
      public int Index;
      public int Character;
      public float Position;
      public int Line;
      public int Page;
      public float Delay;
      public float LineWidth;
      public Color Color;
      public float Scale;
      public float Rotation;
      public float YOffset;
      public float Fade;
      public bool Shake;
      public bool Wave;
      public bool Impact;
      public bool IsPunctuation;

      public void Draw(
        PixelFont font,
        float baseSize,
        Vector2 position,
        Vector2 scale,
        float alpha)
      {
        float num = (this.Impact ? 2f - this.Fade : 1f) * this.Scale;
        Vector2 zero = Vector2.get_Zero();
        Vector2 vector2_1 = Vector2.op_Multiply(scale, num);
        PixelFontSize pixelFontSize = font.Get(baseSize * Math.Max((float) vector2_1.X, (float) vector2_1.Y));
        PixelFontCharacter pixelFontCharacter = pixelFontSize.Get(this.Character);
        Vector2 scale1 = Vector2.op_Multiply(vector2_1, baseSize / pixelFontSize.Size);
        ref __Null local1 = ref position.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + this.Position * (float) scale.X;
        Vector2 vector2_2 = Vector2.op_Addition(Vector2.op_Addition(zero, this.Shake ? Vector2.op_Multiply(new Vector2((float) (Calc.Random.Next(3) - 1), (float) (Calc.Random.Next(3) - 1)), 2f) : Vector2.get_Zero()), this.Wave ? new Vector2(0.0f, (float) Math.Sin((double) this.Index * 0.25 + (double) Engine.Scene.RawTimeActive * 8.0) * 4f) : Vector2.get_Zero());
        ref __Null local2 = ref vector2_2.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + (float) pixelFontCharacter.XOffset;
        ref __Null local3 = ref vector2_2.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local3 = ^(float&) ref local3 + ((float) pixelFontCharacter.YOffset + (float) (-8.0 * (1.0 - (double) this.Fade) + (double) this.YOffset * (double) this.Fade));
        pixelFontCharacter.Texture.Draw(Vector2.op_Addition(position, Vector2.op_Multiply(vector2_2, scale1)), Vector2.get_Zero(), Color.op_Multiply(Color.op_Multiply(this.Color, this.Fade), alpha), scale1, this.Rotation);
      }
    }

    public class Portrait : FancyText.Node
    {
      public int SfxExpression = 1;
      public int Side;
      public string Sprite;
      public string Animation;
      public bool UpsideDown;
      public bool Flipped;
      public bool Pop;
      public string SfxEvent;

      public string SpriteId
      {
        get
        {
          return "portrait_" + this.Sprite;
        }
      }

      public string BeginAnimation
      {
        get
        {
          return "begin_" + this.Animation;
        }
      }

      public string IdleAnimation
      {
        get
        {
          return "idle_" + this.Animation;
        }
      }

      public string TalkAnimation
      {
        get
        {
          return "talk_" + this.Animation;
        }
      }
    }

    public class Wait : FancyText.Node
    {
      public float Duration;
    }

    public class Trigger : FancyText.Node
    {
      public int Index;
      public bool Silent;
      public string Label;
    }

    public class NewLine : FancyText.Node
    {
    }

    public class NewPage : FancyText.Node
    {
    }

    public enum Anchors
    {
      Top,
      Middle,
      Bottom,
    }

    public class Anchor : FancyText.Node
    {
      public FancyText.Anchors Position;
    }

    public class Text
    {
      public List<FancyText.Node> Nodes;
      public int Lines;
      public int Pages;
      public PixelFont Font;
      public float BaseSize;

      public int Count
      {
        get
        {
          return this.Nodes.Count;
        }
      }

      public FancyText.Node this[int index]
      {
        get
        {
          return this.Nodes[index];
        }
      }

      public int GetCharactersOnPage(int start)
      {
        int num = 0;
        for (int index = start; index < this.Count; ++index)
        {
          if (this.Nodes[index] is FancyText.Char)
            ++num;
          else if (this.Nodes[index] is FancyText.NewPage)
            break;
        }
        return num;
      }

      public int GetNextPageStart(int start)
      {
        for (int index = start; index < this.Count; ++index)
        {
          if (this.Nodes[index] is FancyText.NewPage)
            return index + 1;
        }
        return this.Nodes.Count;
      }

      public float WidestLine()
      {
        int val1 = 0;
        for (int index = 0; index < this.Nodes.Count; ++index)
        {
          if (this.Nodes[index] is FancyText.Char)
            val1 = Math.Max(val1, (int) (this.Nodes[index] as FancyText.Char).LineWidth);
        }
        return (float) val1;
      }

      public void Draw(
        Vector2 position,
        Vector2 justify,
        Vector2 scale,
        float alpha,
        int start = 0,
        int end = 2147483647)
      {
        int num1 = Math.Min(this.Nodes.Count, end);
        int val1_1 = 0;
        float val1_2 = 0.0f;
        float num2 = 0.0f;
        PixelFontSize pixelFontSize = this.Font.Get(this.BaseSize);
        for (int index = start; index < num1; ++index)
        {
          if (this.Nodes[index] is FancyText.NewLine)
          {
            if ((double) val1_2 == 0.0)
              val1_2 = 1f;
            num2 += val1_2;
            val1_2 = 0.0f;
          }
          else if (this.Nodes[index] is FancyText.Char)
          {
            val1_1 = Math.Max(val1_1, (int) (this.Nodes[index] as FancyText.Char).LineWidth);
            val1_2 = Math.Max(val1_2, (this.Nodes[index] as FancyText.Char).Scale);
          }
          else if (this.Nodes[index] is FancyText.NewPage)
            break;
        }
        float num3 = num2 + val1_2;
        position = Vector2.op_Subtraction(position, Vector2.op_Multiply(Vector2.op_Multiply(justify, new Vector2((float) val1_1, num3 * (float) pixelFontSize.LineHeight)), scale));
        float val1_3 = 0.0f;
        for (int index = start; index < num1 && !(this.Nodes[index] is FancyText.NewPage); ++index)
        {
          if (this.Nodes[index] is FancyText.NewLine)
          {
            if ((double) val1_3 == 0.0)
              val1_3 = 1f;
            ref __Null local = ref position.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + (float) ((double) pixelFontSize.LineHeight * (double) val1_3 * scale.Y);
            val1_3 = 0.0f;
          }
          if (this.Nodes[index] is FancyText.Char)
          {
            FancyText.Char node = this.Nodes[index] as FancyText.Char;
            node.Draw(this.Font, this.BaseSize, position, scale, alpha);
            val1_3 = Math.Max(val1_3, node.Scale);
          }
        }
      }

      public void DrawJustifyPerLine(
        Vector2 position,
        Vector2 justify,
        Vector2 scale,
        float alpha,
        int start = 0,
        int end = 2147483647)
      {
        int num1 = Math.Min(this.Nodes.Count, end);
        float val1_1 = 0.0f;
        float num2 = 0.0f;
        PixelFontSize pixelFontSize = this.Font.Get(this.BaseSize);
        for (int index = start; index < num1; ++index)
        {
          if (this.Nodes[index] is FancyText.NewLine)
          {
            if ((double) val1_1 == 0.0)
              val1_1 = 1f;
            num2 += val1_1;
            val1_1 = 0.0f;
          }
          else if (this.Nodes[index] is FancyText.Char)
            val1_1 = Math.Max(val1_1, (this.Nodes[index] as FancyText.Char).Scale);
          else if (this.Nodes[index] is FancyText.NewPage)
            break;
        }
        float num3 = num2 + val1_1;
        float val1_2 = 0.0f;
        for (int index = start; index < num1 && !(this.Nodes[index] is FancyText.NewPage); ++index)
        {
          if (this.Nodes[index] is FancyText.NewLine)
          {
            if ((double) val1_2 == 0.0)
              val1_2 = 1f;
            ref __Null local = ref position.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + (float) ((double) val1_2 * (double) pixelFontSize.LineHeight * scale.Y);
            val1_2 = 0.0f;
          }
          if (this.Nodes[index] is FancyText.Char)
          {
            FancyText.Char node = this.Nodes[index] as FancyText.Char;
            Vector2 vector2 = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_UnaryNegation(justify), new Vector2(node.LineWidth, num3 * (float) pixelFontSize.LineHeight)), scale);
            node.Draw(this.Font, this.BaseSize, Vector2.op_Addition(position, vector2), scale, alpha);
            val1_2 = Math.Max(val1_2, node.Scale);
          }
        }
      }
    }
  }
}
