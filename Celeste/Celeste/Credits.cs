// Decompiled with JetBrains decompiler
// Type: Celeste.Credits
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class Credits
  {
    public static string[] Remixers = new string[8]
    {
      "Maxo",
      "Ben Prunty",
      "Christa Lee",
      "in love with a ghost",
      "2 Mello",
      "Jukio Kallio",
      "Kuraine",
      "Matthewせいじ"
    };
    public static Color BorderColor = Color.Black;
    public float AutoScrollSpeedMultiplier = 1f;
    private float scrollSpeed = 90f;
    private float scroll = 0.0f;
    private float height = 0.0f;
    private float scrollDelay = 0.0f;
    private float scrollbarAlpha = 0.0f;
    public float BottomTimer = 0.0f;
    public bool Enabled = true;
    public bool AllowInput = true;
    public const float CreditSpacing = 64f;
    public const float AutoScrollSpeed = 90f;
    public const float InputScrollSpeed = 600f;
    public const float ScrollResumeDelay = 1f;
    public const float ScrollAcceleration = 1800f;
    private List<Credits.CreditNode> credits;
    private float alignment;
    private float scale;
    public static PixelFont Font;
    public static float FontSize;
    public static float LineHeight;

    private static List<Credits.CreditNode> CreateCredits(bool title, bool polaroids)
    {
      List<Credits.CreditNode> creditNodeList = new List<Credits.CreditNode>();
      if (title)
        creditNodeList.Add((Credits.CreditNode) new Credits.Image(nameof (title), 320f));
      creditNodeList.AddRange((IEnumerable<Credits.CreditNode>) new List<Credits.CreditNode>()
      {
        (Credits.CreditNode) new Credits.Role("Matt Thorson", new string[4]
        {
          "Director",
          "Designer",
          "Writer",
          "Gameplay Coder"
        }),
        (Credits.CreditNode) new Credits.Role("Noel Berry", new string[3]
        {
          "Co-Creator",
          "Programmer",
          "Artist"
        }),
        (Credits.CreditNode) new Credits.Role("Amora B.", new string[2]
        {
          "Concept Artist",
          "High Res Artist"
        }),
        (Credits.CreditNode) new Credits.Role("Pedro Medeiros", new string[2]
        {
          "Pixel Artist",
          "UI Artist"
        }),
        (Credits.CreditNode) new Credits.Role("Lena Raine", new string[1]
        {
          "Composer"
        }),
        (Credits.CreditNode) new Credits.Team("Power Up Audio", new string[4]
        {
          "Kevin Regamey",
          "Jeff Tangsoc",
          "Joey Godard",
          "Cole Verderber"
        }, new string[1]{ "Sound Designers" })
      });
      if (polaroids)
        creditNodeList.Add((Credits.CreditNode) new Credits.Image(GFX.Portraits, "credits/a", 64f, -0.05f, false));
      creditNodeList.AddRange((IEnumerable<Credits.CreditNode>) new List<Credits.CreditNode>()
      {
        (Credits.CreditNode) new Credits.Role("Gabby DaRienzo", new string[1]
        {
          "3D Artist"
        }),
        (Credits.CreditNode) new Credits.Role("Sven Bergström", new string[1]
        {
          "3D Lighting Artist"
        })
      });
      creditNodeList.AddRange((IEnumerable<Credits.CreditNode>) new List<Credits.CreditNode>()
      {
        (Credits.CreditNode) new Credits.Thanks("Writing Assistance", new string[5]
        {
          "Noel Berry",
          "Amora B.",
          "Greg Lobanov",
          "Lena Raine",
          "Nick Suttner"
        }),
        (Credits.CreditNode) new Credits.Thanks("Script Editor", new string[1]
        {
          "Nick Suttner"
        }),
        (Credits.CreditNode) new Credits.Thanks("Narrative Consulting", new string[4]
        {
          "Silverstring Media",
          "Claris Cyarron",
          "with Lucas JW Johnson",
          "and Tanya Kan"
        })
      });
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Remixers", Credits.Remixers).SetLanguage(7, "japanese"));
      if (polaroids)
        creditNodeList.Add((Credits.CreditNode) new Credits.Image(GFX.Portraits, "credits/b", 64f, 0.05f, false));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Contributors & Playtesters", new string[27]
      {
        "Nels Anderson",
        "Liam & Graeme Berry",
        "Tamara Bruketta",
        "Allan Defensor",
        "Grayson Evans",
        "Jada Gibbs",
        "Em Halberstadt",
        "Justin Jaffray",
        "Chevy Ray Johnston",
        "Will Lacerda",
        "Myriame Lachapelle",
        "Greg Lobanov",
        "Rafinha Martinelli",
        "Shane Neville",
        "Kyle Pulver",
        "Murphy Pyan",
        "Garret Randell",
        "Kevin Regamey",
        "Atlas Regaudie",
        "Stefano Strapazzon",
        "Nick Suttner",
        "Ryan Thorson",
        "Greg Wohlwend",
        "Justin Yngelmo",
        "baldjared",
        "zep",
        "DevilSquirrel"
      }));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Community", new string[3]
      {
        "Tool Assisted Speedrunners",
        "Everest Modding Community",
        "The Celeste Discord"
      }));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Social Media", new string[1]
      {
        "Heidy Motta"
      }));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Porting", new string[2]
      {
        "Sickhead Games, LLC",
        "Ethan Lee"
      }));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Localization", new string[2]
      {
        "EDS Wordland Ltd.",
        "Amora B."
      }));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Special Thanks", new string[11]
      {
        "Bruce Berry & Marilyn Firth",
        "Liliane Carpinski",
        "Yvonne Hanson",
        "Greg Lobanov",
        "Rodrigo Monteiro",
        "Fernando Piovesan",
        "Paulo Szyszko Pita",
        "Fernando Proença",
        "Zoe Si",
        "Julie, Richard, & Ryan Thorson",
        "Davey Wreden"
      }));
      if (polaroids)
        creditNodeList.Add((Credits.CreditNode) new Credits.Image(GFX.Portraits, "credits/c", 64f, -0.05f, false));
      creditNodeList.Add((Credits.CreditNode) new Credits.Thanks("Production Kitties", new string[3]
      {
        "Jiji, Mr Satan, Finn",
        "Azzy, Phil, Furiosa",
        "Fred, Bastion, Meredith"
      }));
      creditNodeList.AddRange((IEnumerable<Credits.CreditNode>) new List<Credits.CreditNode>()
      {
        (Credits.CreditNode) new Credits.Image(GFX.Misc, "fmod", 0.0f, 0.0f, false),
        (Credits.CreditNode) new Credits.Image(GFX.Misc, "monogame", 0.0f, 0.0f, false),
        (Credits.CreditNode) new Credits.ImageRow(new Credits.Image[2]
        {
          new Credits.Image(GFX.Misc, "fna", 0.0f, 0.0f, false),
          new Credits.Image(GFX.Misc, "xna", 0.0f, 0.0f, false)
        })
      });
      creditNodeList.Add((Credits.CreditNode) new Credits.Break(540f));
      if (polaroids)
        creditNodeList.Add((Credits.CreditNode) new Credits.Image(GFX.Portraits, "credits/d", 0.0f, 0.05f, true));
      creditNodeList.Add((Credits.CreditNode) new Credits.Ending(Dialog.Clean("CREDITS_THANKYOU", (Language) null), !polaroids));
      return creditNodeList;
    }

    public Credits(float alignment = 0.5f, float scale = 1f, bool haveTitle = true, bool havePolaroids = false)
    {
      this.alignment = alignment;
      this.scale = scale;
      this.credits = Credits.CreateCredits(haveTitle, havePolaroids);
      Credits.Font = Dialog.Languages["english"].Font;
      Credits.FontSize = Dialog.Languages["english"].FontFaceSize;
      Credits.LineHeight = (float) Credits.Font.Get(Credits.FontSize).LineHeight;
      this.height = 0.0f;
      foreach (Credits.CreditNode credit in this.credits)
        this.height += credit.Height(scale) + 64f * scale;
      this.height += 476f;
      if (!havePolaroids)
        return;
      this.height -= 280f;
    }

    public void Update()
    {
      if (this.Enabled)
      {
        this.scroll += this.scrollSpeed * Engine.DeltaTime * this.scale;
        if ((double) this.scrollDelay <= 0.0)
          this.scrollSpeed = Calc.Approach(this.scrollSpeed, 90f * this.AutoScrollSpeedMultiplier, 1800f * Engine.DeltaTime);
        else
          this.scrollDelay -= Engine.DeltaTime;
        if (this.AllowInput)
        {
          if (Input.MenuDown.Check)
          {
            this.scrollDelay = 1f;
            this.scrollSpeed = Calc.Approach(this.scrollSpeed, 600f, 1800f * Engine.DeltaTime);
          }
          else if (Input.MenuUp.Check)
          {
            this.scrollDelay = 1f;
            this.scrollSpeed = Calc.Approach(this.scrollSpeed, -600f, 1800f * Engine.DeltaTime);
          }
          else if ((double) this.scrollDelay > 0.0)
            this.scrollSpeed = Calc.Approach(this.scrollSpeed, 0.0f, 1800f * Engine.DeltaTime);
        }
        if ((double) this.scroll < 0.0 || (double) this.scroll > (double) this.height)
          this.scrollSpeed = 0.0f;
        this.scroll = Calc.Clamp(this.scroll, 0.0f, this.height);
        if ((double) this.scroll >= (double) this.height)
          this.BottomTimer += Engine.DeltaTime;
        else
          this.BottomTimer = 0.0f;
      }
      this.scrollbarAlpha = Calc.Approach(this.scrollbarAlpha, !this.Enabled || (double) this.scrollDelay <= 0.0 ? 0.0f : 1f, Engine.DeltaTime * 2f);
    }

    public void Render(Vector2 position)
    {
      Vector2 position1 = position + new Vector2(0.0f, 1080f - this.scroll).Floor();
      foreach (Credits.CreditNode credit in this.credits)
      {
        float num = credit.Height(this.scale);
        if ((double) position1.Y > -(double) num && (double) position1.Y < 1080.0)
          credit.Render(position1, this.alignment, this.scale);
        position1.Y += num + 64f * this.scale;
      }
      if ((double) this.scrollbarAlpha <= 0.0)
        return;
      int num1 = 64;
      int num2 = 1080 - num1 * 2;
      float height = (float) num2 * ((float) num2 / this.height);
      float num3 = (float) ((double) this.scroll / (double) this.height * ((double) num2 - (double) height));
      Draw.Rect(1844f, (float) num1, 12f, (float) num2, Color.White * 0.2f * this.scrollbarAlpha);
      Draw.Rect(1844f, (float) num1 + num3, 12f, height, Color.White * 0.5f * this.scrollbarAlpha);
    }

    private abstract class CreditNode
    {
      public abstract void Render(Vector2 position, float alignment = 0.5f, float scale = 1f);

      public abstract float Height(float scale = 1f);
    }

    private class Role : Credits.CreditNode
    {
      public static readonly Color NameColor = Color.White;
      public static readonly Color RolesColor = Color.White * 0.8f;
      public const float NameScale = 2f;
      public const float RolesScale = 1f;
      public const float Spacing = 8f;
      public const float BottomSpacing = 64f;
      public string Name;
      public string Roles;

      public Role(string name, params string[] roles)
      {
        this.Name = name;
        this.Roles = string.Join(", ", roles);
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
        Credits.Font.DrawOutline(Credits.FontSize, this.Name, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 2f * scale, Credits.Role.NameColor, 2f, Credits.BorderColor);
        position.Y += (float) ((double) Credits.LineHeight * 2.0 + 8.0) * scale;
        Credits.Font.DrawOutline(Credits.FontSize, this.Roles, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1f * scale, Credits.Role.RolesColor, 2f, Credits.BorderColor);
      }

      public override float Height(float scale = 1f)
      {
        return (float) ((double) Credits.LineHeight * 3.0 + 8.0 + 64.0) * scale;
      }
    }

    private class Team : Credits.CreditNode
    {
      public static readonly Color TeamColor = Color.White;
      public const float TeamScale = 1.5f;
      public string Name;
      public string[] Members;
      public string Roles;

      public Team(string name, string[] members, params string[] roles)
      {
        this.Name = name;
        this.Members = members;
        this.Roles = string.Join(", ", roles);
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
        Credits.Font.DrawOutline(Credits.FontSize, this.Name, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 2f * scale, Credits.Role.NameColor, 2f, Credits.BorderColor);
        position.Y += (float) ((double) Credits.LineHeight * 2.0 + 8.0) * scale;
        for (int index = 0; index < this.Members.Length; ++index)
        {
          Credits.Font.DrawOutline(Credits.FontSize, this.Members[index], position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.5f * scale, Credits.Team.TeamColor, 2f, Credits.BorderColor);
          position.Y += Credits.LineHeight * 1.5f * scale;
        }
        Credits.Font.DrawOutline(Credits.FontSize, this.Roles, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1f * scale, Credits.Role.RolesColor, 2f, Credits.BorderColor);
      }

      public override float Height(float scale = 1f)
      {
        return (float) ((double) Credits.LineHeight * (2.0 + (double) this.Members.Length * 1.5 + 1.0) + 8.0 + 64.0) * scale;
      }
    }

    private class Thanks : Credits.CreditNode
    {
      public readonly Color TitleColor = Color.White;
      public readonly Color CreditsColor = Color.White * 0.8f;
      private Dictionary<int, Language> languages = new Dictionary<int, Language>();
      public const float TitleScale = 1.5f;
      public const float CreditsScale = 1.25f;
      public const float Spacing = 8f;
      public int TopPadding;
      public string Title;
      public string[] Credits;

      public Thanks(string title, params string[] to)
      {
        this.Title = title;
        this.Credits = to;
      }

      public Thanks(int topPadding, string title, params string[] to)
      {
        this.TopPadding = topPadding;
        this.Title = title;
        this.Credits = to;
      }

      public Credits.Thanks SetLanguage(int entry, string language)
      {
        this.languages.Add(entry, Dialog.Languages[language]);
        return this;
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
        position.Y += (float) this.TopPadding * scale;
        Font.DrawOutline(FontSize, this.Title, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.5f * scale, this.TitleColor, 2f, BorderColor);
        position.Y += (float) ((double) LineHeight * 1.5 + 8.0) * scale;
        for (int key = 0; key < this.Credits.Length; ++key)
        {
          PixelFont font = Font;
          Language language;
          if (this.languages.TryGetValue(key, out language))
            font = language.Font;
          font.DrawOutline(FontSize, this.Credits[key], position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.25f * scale, this.CreditsColor, 2f, BorderColor);
          position.Y += LineHeight * 1.25f * scale;
        }
      }

      public override float Height(float scale = 1f)
      {
        return ((float) ((double) LineHeight * (1.5 + (double) this.Credits.Length * 1.25) + (this.Credits.Length != 0 ? 8.0 : 0.0)) + (float) this.TopPadding) * scale;
      }
    }

    private class Ending : Credits.CreditNode
    {
      public string Text;
      public bool Spacing;

      public Ending(string text, bool spacing)
      {
        this.Text = text;
        this.Spacing = spacing;
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
        if (this.Spacing)
          position.Y += 540f;
        else
          position.Y += (float) ((double) ActiveFont.LineHeight * 1.5 * (double) scale * 0.5);
        ActiveFont.DrawOutline(this.Text, new Vector2(960f, position.Y), new Vector2(0.5f, 0.5f), Vector2.One * 1.5f * scale, Color.White, 2f, Credits.BorderColor);
      }

      public override float Height(float scale = 1f)
      {
        if (this.Spacing)
          return 540f;
        return ActiveFont.LineHeight * 1.5f * scale;
      }
    }

    private class Image : Credits.CreditNode
    {
      public Atlas Atlas;
      public string ImagePath;
      public float BottomPadding;
      public float Rotation;
      public bool ScreenCenter;

      public Image(string path, float bottomPadding = 0.0f)
        : this(GFX.Gui, path, bottomPadding, 0.0f, false)
      {
      }

      public Image(
        Atlas atlas,
        string path,
        float bottomPadding = 0.0f,
        float rotation = 0.0f,
        bool screenCenter = false)
      {
        this.Atlas = atlas;
        this.ImagePath = path;
        this.BottomPadding = bottomPadding;
        this.Rotation = rotation;
        this.ScreenCenter = screenCenter;
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
        MTexture atla = this.Atlas[this.ImagePath];
        Vector2 position1 = position + new Vector2((float) atla.Width * (0.5f - alignment), (float) atla.Height * 0.5f) * scale;
        if (this.ScreenCenter)
          position1.X = 960f;
        atla.DrawCentered(position1, Color.White, scale, this.Rotation);
      }

      public override float Height(float scale = 1f)
      {
        return ((float) this.Atlas[this.ImagePath].Height + this.BottomPadding) * scale;
      }
    }

    private class ImageRow : Credits.CreditNode
    {
      private Credits.Image[] images;

      public ImageRow(params Credits.Image[] images)
      {
        this.images = images;
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
        float num1 = this.Height(scale);
        float num2 = 0.0f;
        foreach (Credits.Image image in this.images)
          num2 += (float) (image.Atlas[image.ImagePath].Width + 32) * scale;
        float num3 = num2 - 32f * scale;
        Vector2 vector2 = position - new Vector2(alignment * num3, 0.0f);
        foreach (Credits.Image image in this.images)
        {
          image.Render(vector2 + new Vector2(0.0f, (float) (((double) num1 - (double) image.Height(scale)) / 2.0)), 0.0f, scale);
          vector2.X += (float) (image.Atlas[image.ImagePath].Width + 32) * scale;
        }
      }

      public override float Height(float scale = 1f)
      {
        float num = 0.0f;
        foreach (Credits.Image image in this.images)
        {
          if ((double) image.Height(scale) > (double) num)
            num = image.Height(scale);
        }
        return num;
      }
    }

    private class Break : Credits.CreditNode
    {
      public float Size;

      public Break(float size = 64f)
      {
        this.Size = size;
      }

      public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
      {
      }

      public override float Height(float scale = 1f)
      {
        return this.Size * scale;
      }
    }
  }
}

