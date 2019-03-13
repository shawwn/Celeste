// Decompiled with JetBrains decompiler
// Type: Celeste.CompleteRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Celeste
{
  public class CompleteRenderer : HiresRenderer, IDisposable
  {
    private float fadeAlpha = 1f;
    public float SlideDuration = 1.5f;
    public List<CompleteRenderer.Layer> Layers = new List<CompleteRenderer.Layer>();
    private const float ScrollRange = 200f;
    private const float ScrollSpeed = 600f;
    private Atlas atlas;
    private XmlElement xml;
    private Coroutine routine;
    private Vector2 controlScroll;
    private float controlMult;
    public Vector2 Scroll;
    public Vector2 StartScroll;
    public Vector2 CenterScroll;
    public Vector2 Offset;
    public Action<Vector2> RenderUI;
    public Action RenderPostUI;

    public bool HasUI { get; private set; }

    public CompleteRenderer(XmlElement xml, Atlas atlas, float delay, Action onDoneSlide = null)
    {
      this.atlas = atlas;
      this.xml = xml;
      if (xml != null)
      {
        if (xml["start"] != null)
          this.StartScroll = xml["start"].Position();
        if (xml["center"] != null)
          this.CenterScroll = xml["center"].Position();
        if (xml["offset"] != null)
          this.Offset = xml["offset"].Position();
        foreach (object obj in (XmlNode) xml["layers"])
        {
          if (obj is XmlElement)
          {
            XmlElement xml1 = obj as XmlElement;
            if (xml1.Name == "layer")
              this.Layers.Add((CompleteRenderer.Layer) new CompleteRenderer.ImageLayer(this.Offset, atlas, xml1));
            else if (xml1.Name == "ui")
            {
              this.HasUI = true;
              this.Layers.Add((CompleteRenderer.Layer) new CompleteRenderer.UILayer(this, xml1));
            }
          }
        }
      }
      this.Scroll = this.StartScroll;
      this.routine = new Coroutine(this.SlideRoutine(delay, onDoneSlide), true);
    }

    public void Dispose()
    {
      if (this.atlas == null)
        return;
      this.atlas.Dispose();
    }

    private IEnumerator SlideRoutine(float delay, Action onDoneSlide)
    {
      yield return (object) delay;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / this.SlideDuration)
      {
        yield return (object) null;
        this.Scroll = Vector2.Lerp(this.StartScroll, this.CenterScroll, Ease.SineOut(p));
        this.fadeAlpha = Calc.LerpClamp(1f, 0.0f, p * 2f);
      }
      this.Scroll = this.CenterScroll;
      this.fadeAlpha = 0.0f;
      yield return (object) 0.2f;
      if (onDoneSlide != null)
        onDoneSlide();
      while (true)
      {
        this.controlMult = Calc.Approach(this.controlMult, 1f, 5f * Engine.DeltaTime);
        yield return (object) null;
      }
    }

    public override void Update(Scene scene)
    {
      Vector2 target = Vector2.op_Addition(Input.Aim.Value, Input.MountainAim.Value);
      if ((double) ((Vector2) ref target).Length() > 1.0)
        ((Vector2) ref target).Normalize();
      target = Vector2.op_Multiply(target, 200f);
      this.controlScroll = Calc.Approach(this.controlScroll, target, 600f * Engine.DeltaTime);
      foreach (CompleteRenderer.Layer layer in this.Layers)
        layer.Update(scene);
      this.routine.Update();
    }

    public override void RenderContent(Scene scene)
    {
      HiresRenderer.BeginRender((BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearWrap);
      foreach (CompleteRenderer.Layer layer in this.Layers)
        layer.Render(Vector2.op_Subtraction(Vector2.op_UnaryNegation(this.Scroll), Vector2.op_Multiply(this.controlScroll, this.controlMult)));
      if (this.RenderPostUI != null)
        this.RenderPostUI();
      if ((double) this.fadeAlpha > 0.0)
        Draw.Rect(-10f, -10f, (float) (Engine.Width + 20), (float) (Engine.Height + 20), Color.op_Multiply(Color.get_Black(), this.fadeAlpha));
      HiresRenderer.EndRender();
    }

    public abstract class Layer
    {
      public Vector2 Position;
      public Vector2 ScrollFactor;

      public Layer(XmlElement xml)
      {
        this.Position = xml.Position(Vector2.get_Zero());
        if (xml.HasAttr("scroll"))
        {
          this.ScrollFactor.X = (__Null) (double) (this.ScrollFactor.Y = (__Null) xml.AttrFloat("scroll"));
        }
        else
        {
          this.ScrollFactor.X = (__Null) (double) xml.AttrFloat("scrollX", 0.0f);
          this.ScrollFactor.Y = (__Null) (double) xml.AttrFloat("scrollY", 0.0f);
        }
      }

      public virtual void Update(Scene scene)
      {
      }

      public abstract void Render(Vector2 scroll);

      public Vector2 GetScrollPosition(Vector2 scroll)
      {
        Vector2 position = this.Position;
        if (Vector2.op_Inequality(this.ScrollFactor, Vector2.get_Zero()))
        {
          position.X = (__Null) (double) MathHelper.Lerp((float) this.Position.X, (float) (this.Position.X + scroll.X), (float) this.ScrollFactor.X);
          position.Y = (__Null) (double) MathHelper.Lerp((float) this.Position.Y, (float) (this.Position.Y + scroll.Y), (float) this.ScrollFactor.Y);
        }
        return position;
      }
    }

    public class UILayer : CompleteRenderer.Layer
    {
      private CompleteRenderer renderer;

      public UILayer(CompleteRenderer renderer, XmlElement xml)
        : base(xml)
      {
        this.renderer = renderer;
      }

      public override void Render(Vector2 scroll)
      {
        if (this.renderer.RenderUI == null)
          return;
        this.renderer.RenderUI(scroll);
      }
    }

    public class ImageLayer : CompleteRenderer.Layer
    {
      public List<MTexture> Images = new List<MTexture>();
      public float Frame;
      public float FrameRate;
      public float Alpha;
      public Vector2 Offset;
      public Vector2 Speed;

      public ImageLayer(Vector2 offset, Atlas atlas, XmlElement xml)
        : base(xml)
      {
        this.Position = Vector2.op_Addition(this.Position, offset);
        string str = xml.Attr("img");
        char[] chArray = new char[1]{ ',' };
        foreach (string id in str.Split(chArray))
        {
          if (atlas.Has(id))
            this.Images.Add(atlas[id]);
          else
            this.Images.Add((MTexture) null);
        }
        this.FrameRate = xml.AttrFloat("fps", 6f);
        this.Alpha = xml.AttrFloat("alpha", 1f);
        this.Speed = new Vector2(xml.AttrFloat("speedx", 0.0f), xml.AttrFloat("speedy", 0.0f));
      }

      public override void Update(Scene scene)
      {
        this.Frame += Engine.DeltaTime * this.FrameRate;
        this.Offset = Vector2.op_Addition(this.Offset, Vector2.op_Multiply(this.Speed, Engine.DeltaTime));
      }

      public override void Render(Vector2 scroll)
      {
        Vector2 vector2 = this.GetScrollPosition(scroll).Floor();
        MTexture image = this.Images[(int) ((double) this.Frame % (double) this.Images.Count)];
        if (image == null)
          return;
        bool flag = SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode;
        if (flag)
        {
          vector2.X = (__Null) (1920.0 - vector2.X - image.DrawOffset.X - (double) image.Texture.Texture.get_Width());
          ref __Null local = ref vector2.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) image.DrawOffset.Y;
        }
        else
          vector2 = Vector2.op_Addition(vector2, image.DrawOffset);
        Draw.SpriteBatch.Draw(image.Texture.Texture, vector2, new Rectangle?(new Rectangle((int) -this.Offset.X + 1, (int) -this.Offset.Y + 1, image.ClipRect.Width - 2, image.ClipRect.Height - 2)), Color.op_Multiply(Color.get_White(), this.Alpha), 0.0f, Vector2.get_Zero(), 1f, flag ? (SpriteEffects) 1 : (SpriteEffects) 0, 0.0f);
      }
    }
  }
}
