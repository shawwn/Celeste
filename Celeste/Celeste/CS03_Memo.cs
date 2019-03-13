// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_Memo
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
  public class CS03_Memo : CutsceneEntity
  {
    private const string ReadOnceFlag = "memo_read";
    private Player player;
    private CS03_Memo.MemoPage memo;

    public CS03_Memo(Player player)
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
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      if (!this.Level.Session.GetFlag("memo_read"))
      {
        yield return (object) Textbox.Say("ch3_memo_opening");
        yield return (object) 0.1f;
      }
      this.memo = new CS03_Memo.MemoPage();
      this.Scene.Add((Entity) this.memo);
      yield return (object) this.memo.EaseIn();
      yield return (object) this.memo.Wait();
      yield return (object) this.memo.EaseOut();
      this.memo = (CS03_Memo.MemoPage) null;
      this.EndCutscene(this.Level, true);
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      level.Session.SetFlag("memo_read", true);
      if (this.memo == null)
        return;
      this.memo.RemoveSelf();
    }

    private class MemoPage : Entity
    {
      private float textDownscale = 1f;
      private float alpha = 1f;
      private float scale = 1f;
      private float rotation = 0.0f;
      private float timer = 0.0f;
      private const float TextScale = 0.75f;
      private const float PaperScale = 1.5f;
      private MTexture paper;
      private VirtualRenderTarget target;
      private FancyText.Text text;
      private bool easingOut;

      public MemoPage()
      {
        this.Tag = (int) Tags.HUD;
        this.paper = GFX.Gui["memo"];
        float num1 = (float) ((double) this.paper.Width * 1.5 - 120.0);
        this.text = FancyText.Parse(Dialog.Get("CH3_MEMO", (Language) null), (int) ((double) num1 / 0.75), -1, 1f, new Color?(Color.Black * 0.6f), (Language) null);
        float num2 = this.text.WidestLine() * 0.75f;
        if ((double) num2 > (double) num1)
          this.textDownscale = num1 / num2;
        this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      }

      public IEnumerator EaseIn()
      {
        Audio.Play("event:/game/03_resort/memo_in");
        Vector2 from = new Vector2((float) (Engine.Width / 2), (float) (Engine.Height + 100));
        Vector2 to = new Vector2((float) (Engine.Width / 2), (float) (Engine.Height / 2 - 150));
        float rFrom = -0.1f;
        float rTo = 0.05f;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
        {
          this.Position = from + (to - from) * Ease.CubeOut(p);
          this.alpha = Ease.CubeOut(p);
          this.rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
          yield return (object) null;
        }
      }

      public IEnumerator Wait()
      {
        float start = this.Position.Y;
        int index = 0;
        while (!Input.MenuCancel.Pressed)
        {
          float target = start - (float) (index * 400);
          this.Position.Y += (float) (((double) target - (double) this.Position.Y) * (1.0 - Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
          if (Input.MenuUp.Pressed && index > 0)
            --index;
          else if (index < 2)
          {
            if (Input.MenuDown.Pressed && !Input.MenuDown.Repeating || Input.MenuConfirm.Pressed)
              ++index;
          }
          else if (Input.MenuConfirm.Pressed)
            break;
          yield return (object) null;
        }
        Audio.Play("event:/ui/main/button_lowkey");
      }

      public IEnumerator EaseOut()
      {
        Audio.Play("event:/game/03_resort/memo_out");
        this.easingOut = true;
        Vector2 from = this.Position;
        Vector2 to = new Vector2((float) (Engine.Width / 2), (float) -this.target.Height);
        float rFrom = this.rotation;
        float rTo = this.rotation + 0.1f;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 1.5f)
        {
          this.Position = from + (to - from) * Ease.CubeIn(p);
          this.alpha = 1f - Ease.CubeIn(p);
          this.rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
          yield return (object) null;
        }
        this.RemoveSelf();
      }

      public void BeforeRender()
      {
        if (this.target == null)
          this.target = VirtualContent.CreateRenderTarget("oshiro-memo", (int) ((double) this.paper.Width * 1.5), (int) ((double) this.paper.Height * 1.5), false, true, 0);
        Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.target);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        this.paper.Draw(Vector2.Zero, Vector2.Zero, Color.White, 1.5f);
        this.text.Draw(new Vector2((float) ((double) this.paper.Width * 1.5 / 2.0), 210f), new Vector2(0.5f, 0.0f), Vector2.One * 0.75f * this.textDownscale, 1f, 0, int.MaxValue);
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
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.target, this.Position, new Rectangle?(this.target.Bounds), Color.White * this.alpha, this.rotation, new Vector2((float) this.target.Width, 0.0f) / 2f, this.scale, SpriteEffects.None, 0.0f);
        if (!this.easingOut)
          GFX.Gui["textboxbutton"].DrawCentered(this.Position + new Vector2((float) (this.target.Width / 2 + 40), (float) (this.target.Height + ((double) this.timer % 1.0 < 0.25 ? 6 : 0))));
      }
    }
  }
}

