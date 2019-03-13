// Decompiled with JetBrains decompiler
// Type: Celeste.CS02_Journal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS02_Journal : CutsceneEntity
  {
    private const string ReadOnceFlag = "poem_read";
    private Player player;
    private CS02_Journal.PoemPage poem;

    public CS02_Journal(Player player)
      : base(true, false)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Routine(), true));
    }

    private IEnumerator Routine()
    {
      CS02_Journal cs02Journal = this;
      cs02Journal.player.StateMachine.State = 11;
      cs02Journal.player.StateMachine.Locked = true;
      if (!cs02Journal.Level.Session.GetFlag("poem_read"))
      {
        yield return (object) Textbox.Say("ch2_journal");
        yield return (object) 0.1f;
      }
      cs02Journal.poem = new CS02_Journal.PoemPage();
      cs02Journal.Scene.Add((Entity) cs02Journal.poem);
      yield return (object) cs02Journal.poem.EaseIn();
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      Audio.Play("event:/ui/main/button_lowkey");
      yield return (object) cs02Journal.poem.EaseOut();
      cs02Journal.poem = (CS02_Journal.PoemPage) null;
      cs02Journal.EndCutscene(cs02Journal.Level, true);
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      level.Session.SetFlag("poem_read", true);
      if (this.poem == null)
        return;
      this.poem.RemoveSelf();
    }

    private class PoemPage : Entity
    {
      private float alpha = 1f;
      private float scale = 1f;
      private const float TextScale = 0.7f;
      private MTexture paper;
      private VirtualRenderTarget target;
      private FancyText.Text text;
      private float rotation;
      private float timer;
      private bool easingOut;

      public PoemPage()
      {
        this.Tag = (int) Tags.HUD;
        this.paper = GFX.Gui["poempage"];
        this.text = FancyText.Parse(Dialog.Get("CH2_POEM", (Language) null), (int) ((double) (this.paper.Width - 120) / 0.699999988079071), -1, 1f, new Color?(Color.op_Multiply(Color.get_Black(), 0.6f)), (Language) null);
        this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      }

      public IEnumerator EaseIn()
      {
        CS02_Journal.PoemPage poemPage = this;
        Audio.Play("event:/game/03_resort/memo_in");
        Vector2 vector2 = Vector2.op_Division(new Vector2((float) Engine.Width, (float) Engine.Height), 2f);
        Vector2 from = Vector2.op_Addition(vector2, new Vector2(0.0f, 200f));
        Vector2 to = vector2;
        float rFrom = -0.1f;
        float rTo = 0.05f;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
        {
          poemPage.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.CubeOut(p)));
          poemPage.alpha = Ease.CubeOut(p);
          poemPage.rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
          yield return (object) null;
        }
      }

      public IEnumerator EaseOut()
      {
        CS02_Journal.PoemPage poemPage = this;
        Audio.Play("event:/game/03_resort/memo_out");
        poemPage.easingOut = true;
        Vector2 from = poemPage.Position;
        Vector2 to = Vector2.op_Addition(Vector2.op_Division(new Vector2((float) Engine.Width, (float) Engine.Height), 2f), new Vector2(0.0f, -200f));
        float rFrom = poemPage.rotation;
        float rTo = poemPage.rotation + 0.1f;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 1.5f)
        {
          poemPage.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.CubeIn(p)));
          poemPage.alpha = 1f - Ease.CubeIn(p);
          poemPage.rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
          yield return (object) null;
        }
        poemPage.RemoveSelf();
      }

      public void BeforeRender()
      {
        if (this.target == null)
          this.target = VirtualContent.CreateRenderTarget("journal-poem", this.paper.Width, this.paper.Height, false, true, 0);
        Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.target);
        Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend);
        this.paper.Draw(Vector2.get_Zero());
        this.text.DrawJustifyPerLine(Vector2.op_Division(new Vector2((float) this.paper.Width, (float) this.paper.Height), 2f), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), 0.7f), 1f, 0, int.MaxValue);
        Draw.SpriteBatch.End();
      }

      public override void Removed(Scene scene)
      {
        if (this.target != null)
          this.target.Dispose();
        this.target = (VirtualRenderTarget) null;
        base.Removed(scene);
      }

      public override void SceneEnd(Scene scene)
      {
        if (this.target != null)
          this.target.Dispose();
        this.target = (VirtualRenderTarget) null;
        base.SceneEnd(scene);
      }

      public override void Update()
      {
        this.timer += Engine.DeltaTime;
        base.Update();
      }

      public override void Render()
      {
        Level scene = this.Scene as Level;
        if (scene != null && (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null || scene.SkippingCutscene) || this.target == null)
          return;
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.target, this.Position, new Rectangle?(this.target.Bounds), Color.op_Multiply(Color.get_White(), this.alpha), this.rotation, Vector2.op_Division(new Vector2((float) this.target.Width, (float) this.target.Height), 2f), this.scale, (SpriteEffects) 0, 0.0f);
        if (this.easingOut)
          return;
        GFX.Gui["textboxbutton"].DrawCentered(Vector2.op_Addition(this.Position, new Vector2((float) (this.target.Width / 2 + 40), (float) (this.target.Height / 2 + ((double) this.timer % 1.0 < 0.25 ? 6 : 0)))));
      }
    }
  }
}
