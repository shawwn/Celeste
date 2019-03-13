// Decompiled with JetBrains decompiler
// Type: Monocle.Sprite
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Monocle
{
  public class Sprite : Image
  {
    public float Rate = 1f;
    public bool UseRawDeltaTime;
    public Vector2? Justify;
    public Action<string> OnFinish;
    public Action<string> OnLoop;
    public Action<string> OnFrameChange;
    public Action<string> OnLastFrame;
    public Action<string, string> OnChange;
    private Atlas atlas;
    public string Path;
    private Dictionary<string, Sprite.Animation> animations;
    private Sprite.Animation currentAnimation;
    private float animationTimer;
    private int width;
    private int height;

    public Sprite(Atlas atlas, string path)
      : base((MTexture) null, true)
    {
      this.atlas = atlas;
      this.Path = path;
      this.animations = new Dictionary<string, Sprite.Animation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.CurrentAnimationID = "";
    }

    public void Reset(Atlas atlas, string path)
    {
      this.atlas = atlas;
      this.Path = path;
      this.animations = new Dictionary<string, Sprite.Animation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.currentAnimation = (Sprite.Animation) null;
      this.CurrentAnimationID = "";
      this.OnFinish = (Action<string>) null;
      this.OnLoop = (Action<string>) null;
      this.OnFrameChange = (Action<string>) null;
      this.OnChange = (Action<string, string>) null;
      this.Animating = false;
    }

    public MTexture GetFrame(string animation, int frame)
    {
      return this.animations[animation].Frames[frame];
    }

    public Vector2 Center
    {
      get
      {
        return new Vector2(this.Width / 2f, this.Height / 2f);
      }
    }

    public override void Update()
    {
      if (!this.Animating)
        return;
      if (this.UseRawDeltaTime)
        this.animationTimer += Engine.RawDeltaTime * this.Rate;
      else
        this.animationTimer += Engine.DeltaTime * this.Rate;
      if ((double) Math.Abs(this.animationTimer) < (double) this.currentAnimation.Delay)
        return;
      this.CurrentAnimationFrame += Math.Sign(this.animationTimer);
      this.animationTimer -= (float) Math.Sign(this.animationTimer) * this.currentAnimation.Delay;
      if (this.CurrentAnimationFrame < 0 || this.CurrentAnimationFrame >= this.currentAnimation.Frames.Length)
      {
        string currentAnimationId1 = this.CurrentAnimationID;
        if (this.OnLastFrame != null)
          this.OnLastFrame(this.CurrentAnimationID);
        string currentAnimationId2 = this.CurrentAnimationID;
        if (!(currentAnimationId1 == currentAnimationId2))
          return;
        if (this.currentAnimation.Goto != null)
        {
          this.CurrentAnimationID = this.currentAnimation.Goto.Choose();
          if (this.OnChange != null)
            this.OnChange(this.LastAnimationID, this.CurrentAnimationID);
          this.LastAnimationID = this.CurrentAnimationID;
          this.currentAnimation = this.animations[this.LastAnimationID];
          this.CurrentAnimationFrame = this.CurrentAnimationFrame >= 0 ? 0 : this.currentAnimation.Frames.Length - 1;
          this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
          if (this.OnLoop == null)
            return;
          this.OnLoop(this.CurrentAnimationID);
        }
        else
        {
          this.CurrentAnimationFrame = this.CurrentAnimationFrame >= 0 ? this.currentAnimation.Frames.Length - 1 : 0;
          this.Animating = false;
          string currentAnimationId3 = this.CurrentAnimationID;
          this.CurrentAnimationID = "";
          this.currentAnimation = (Sprite.Animation) null;
          this.animationTimer = 0.0f;
          if (this.OnFinish == null)
            return;
          this.OnFinish(currentAnimationId3);
        }
      }
      else
        this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
    }

    private void SetFrame(MTexture texture)
    {
      if (texture == this.Texture)
        return;
      this.Texture = texture;
      if (this.Justify.HasValue)
        this.Origin = new Vector2((float) this.Texture.Width * (float) this.Justify.Value.X, (float) this.Texture.Height * (float) this.Justify.Value.Y);
      if (this.OnFrameChange == null)
        return;
      this.OnFrameChange(this.CurrentAnimationID);
    }

    public void SetAnimationFrame(int frame)
    {
      this.animationTimer = 0.0f;
      this.CurrentAnimationFrame = frame % this.currentAnimation.Frames.Length;
      this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
    }

    public void AddLoop(string id, string path, float delay)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, (int[]) null),
        Goto = new Chooser<string>(id, 1f)
      };
    }

    public void AddLoop(string id, string path, float delay, params int[] frames)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, frames),
        Goto = new Chooser<string>(id, 1f)
      };
    }

    public void Add(string id, string path)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = 0.0f,
        Frames = this.GetFrames(path, (int[]) null),
        Goto = (Chooser<string>) null
      };
    }

    public void Add(string id, string path, float delay)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, (int[]) null),
        Goto = (Chooser<string>) null
      };
    }

    public void Add(string id, string path, float delay, params int[] frames)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, frames),
        Goto = (Chooser<string>) null
      };
    }

    public void Add(string id, string path, float delay, string into)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, (int[]) null),
        Goto = Chooser<string>.FromString<string>(into)
      };
    }

    public void Add(string id, string path, float delay, Chooser<string> into)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, (int[]) null),
        Goto = into
      };
    }

    public void Add(string id, string path, float delay, string into, params int[] frames)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, frames),
        Goto = Chooser<string>.FromString<string>(into)
      };
    }

    public void Add(
      string id,
      string path,
      float delay,
      Chooser<string> into,
      params int[] frames)
    {
      this.animations[id] = new Sprite.Animation()
      {
        Delay = delay,
        Frames = this.GetFrames(path, frames),
        Goto = into
      };
    }

    private MTexture[] GetFrames(string path, int[] frames = null)
    {
      MTexture[] mtextureArray1;
      if (frames == null || frames.Length == 0)
      {
        mtextureArray1 = this.atlas.GetAtlasSubtextures(this.Path + path).ToArray();
      }
      else
      {
        string key = this.Path + path;
        MTexture[] mtextureArray2 = new MTexture[frames.Length];
        for (int index = 0; index < frames.Length; ++index)
        {
          MTexture atlasSubtexturesAt = this.atlas.GetAtlasSubtexturesAt(key, frames[index]);
          if (atlasSubtexturesAt == null)
            throw new Exception("Can't find sprite " + key + " with index " + (object) frames[index]);
          mtextureArray2[index] = atlasSubtexturesAt;
        }
        mtextureArray1 = mtextureArray2;
      }
      this.width = Math.Max(mtextureArray1[0].Width, this.width);
      this.height = Math.Max(mtextureArray1[0].Height, this.height);
      return mtextureArray1;
    }

    public void ClearAnimations()
    {
      this.animations.Clear();
    }

    public void Play(string id, bool restart = false, bool randomizeFrame = false)
    {
      if (!(this.CurrentAnimationID != id | restart))
        return;
      if (this.OnChange != null)
        this.OnChange(this.LastAnimationID, id);
      this.LastAnimationID = this.CurrentAnimationID = id;
      this.currentAnimation = this.animations[id];
      this.Animating = (double) this.currentAnimation.Delay > 0.0;
      if (randomizeFrame)
      {
        this.animationTimer = Calc.Random.NextFloat(this.currentAnimation.Delay);
        this.CurrentAnimationFrame = Calc.Random.Next(this.currentAnimation.Frames.Length);
      }
      else
      {
        this.animationTimer = 0.0f;
        this.CurrentAnimationFrame = 0;
      }
      this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
    }

    public void PlayOffset(string id, float offset, bool restart = false)
    {
      if (!(this.CurrentAnimationID != id | restart))
        return;
      if (this.OnChange != null)
        this.OnChange(this.LastAnimationID, id);
      this.LastAnimationID = this.CurrentAnimationID = id;
      this.currentAnimation = this.animations[id];
      if ((double) this.currentAnimation.Delay > 0.0)
      {
        this.Animating = true;
        float num = this.currentAnimation.Delay * (float) this.currentAnimation.Frames.Length * offset;
        this.CurrentAnimationFrame = 0;
        for (; (double) num >= (double) this.currentAnimation.Delay; num -= this.currentAnimation.Delay)
          ++this.CurrentAnimationFrame;
        this.CurrentAnimationFrame %= this.currentAnimation.Frames.Length;
        this.animationTimer = num;
        this.SetFrame(this.currentAnimation.Frames[this.CurrentAnimationFrame]);
      }
      else
      {
        this.animationTimer = 0.0f;
        this.Animating = false;
        this.CurrentAnimationFrame = 0;
        this.SetFrame(this.currentAnimation.Frames[0]);
      }
    }

    public IEnumerator PlayRoutine(string id, bool restart = false)
    {
      this.Play(id, restart, false);
      return this.PlayUtil();
    }

    public IEnumerator ReverseRoutine(string id, bool restart = false)
    {
      this.Reverse(id, restart);
      return this.PlayUtil();
    }

    private IEnumerator PlayUtil()
    {
      while (this.Animating)
        yield return (object) null;
    }

    public void Reverse(string id, bool restart = false)
    {
      this.Play(id, restart, false);
      if ((double) this.Rate <= 0.0)
        return;
      this.Rate *= -1f;
    }

    public bool Has(string id)
    {
      if (id != null)
        return this.animations.ContainsKey(id);
      return false;
    }

    public void Stop()
    {
      this.Animating = false;
      this.currentAnimation = (Sprite.Animation) null;
      this.CurrentAnimationID = "";
    }

    public bool Animating { get; private set; }

    public string CurrentAnimationID { get; private set; }

    public string LastAnimationID { get; private set; }

    public int CurrentAnimationFrame { get; private set; }

    public int CurrentAnimationTotalFrames
    {
      get
      {
        if (this.currentAnimation != null)
          return this.currentAnimation.Frames.Length;
        return 0;
      }
    }

    public override float Width
    {
      get
      {
        return (float) this.width;
      }
    }

    public override float Height
    {
      get
      {
        return (float) this.height;
      }
    }

    internal Sprite()
      : base((MTexture) null, true)
    {
    }

    internal Sprite CreateClone()
    {
      return this.CloneInto(new Sprite());
    }

    internal Sprite CloneInto(Sprite clone)
    {
      clone.Texture = this.Texture;
      clone.Position = this.Position;
      clone.Justify = this.Justify;
      clone.Origin = this.Origin;
      clone.animations = new Dictionary<string, Sprite.Animation>((IDictionary<string, Sprite.Animation>) this.animations, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      clone.currentAnimation = this.currentAnimation;
      clone.animationTimer = this.animationTimer;
      clone.width = this.width;
      clone.height = this.height;
      clone.Animating = this.Animating;
      clone.CurrentAnimationID = this.CurrentAnimationID;
      clone.LastAnimationID = this.LastAnimationID;
      clone.CurrentAnimationFrame = this.CurrentAnimationFrame;
      return clone;
    }

    public void DrawSubrect(Vector2 offset, Rectangle rectangle)
    {
      if (this.Texture == null)
        return;
      Rectangle relativeRect = this.Texture.GetRelativeRect(rectangle);
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(-Math.Min((float) rectangle.X - (float) this.Texture.DrawOffset.X, 0.0f), -Math.Min((float) rectangle.Y - (float) this.Texture.DrawOffset.Y, 0.0f));
      Draw.SpriteBatch.Draw(this.Texture.Texture.Texture, Vector2.op_Addition(this.RenderPosition, offset), new Rectangle?(relativeRect), this.Color, this.Rotation, Vector2.op_Subtraction(this.Origin, vector2), this.Scale, this.Effects, 0.0f);
    }

    public void LogAnimations()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, Sprite.Animation> animation1 in this.animations)
      {
        Sprite.Animation animation2 = animation1.Value;
        stringBuilder.Append(animation1.Key);
        stringBuilder.Append("\n{\n\t");
        stringBuilder.Append(string.Join("\n\t", (object[]) animation2.Frames));
        stringBuilder.Append("\n}\n");
      }
      Calc.Log((object) stringBuilder.ToString());
    }

    private class Animation
    {
      public float Delay;
      public MTexture[] Frames;
      public Chooser<string> Goto;
    }
  }
}
