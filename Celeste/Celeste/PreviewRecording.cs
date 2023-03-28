// Decompiled with JetBrains decompiler
// Type: Celeste.PreviewRecording
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste
{
  public class PreviewRecording : Scene
  {
    public string Filename;
    public List<Player.ChaserState> Timeline;
    public PlayerPlayback entity;
    public float ScreenWidth = 640f;
    public float ScreenHeight = 360f;
    public float Width;
    public float Height;

    private Matrix Matrix => Matrix.CreateScale(1920f / this.ScreenWidth) * Engine.ScreenMatrix;

    public PreviewRecording(string filename)
    {
      this.Filename = filename;
      this.Timeline = PlaybackData.Import(File.ReadAllBytes(filename));
      float val2_1 = float.MaxValue;
      float val2_2 = float.MinValue;
      float val2_3 = float.MinValue;
      float val2_4 = float.MaxValue;
      foreach (Player.ChaserState chaserState in this.Timeline)
      {
        val2_1 = Math.Min(chaserState.Position.X, val2_1);
        val2_2 = Math.Max(chaserState.Position.X, val2_2);
        val2_4 = Math.Min(chaserState.Position.Y, val2_4);
        val2_3 = Math.Max(chaserState.Position.Y, val2_3);
      }
      this.Width = (float) (int) ((double) val2_2 - (double) val2_1);
      this.Height = (float) (int) ((double) val2_3 - (double) val2_4);
      this.Add((Entity) (this.entity = new PlayerPlayback(new Vector2((float) (((double) this.ScreenWidth - (double) this.Width) / 2.0) - val2_1, (float) (((double) this.ScreenHeight - (double) this.Height) / 2.0) - val2_4), PlayerSpriteMode.Madeline, this.Timeline)));
    }

    public override void Update()
    {
      if (MInput.Keyboard.Check(Keys.A))
        this.entity.TrimStart = Math.Max(0.0f, this.entity.TrimStart -= Engine.DeltaTime);
      if (MInput.Keyboard.Check(Keys.D))
        this.entity.TrimStart = Math.Min(this.entity.Duration, this.entity.TrimStart += Engine.DeltaTime);
      if (MInput.Keyboard.Check(Keys.Left))
        this.entity.TrimEnd = Math.Max(0.0f, this.entity.TrimEnd -= Engine.DeltaTime);
      if (MInput.Keyboard.Check(Keys.Right))
        this.entity.TrimEnd = Math.Min(this.entity.Duration, this.entity.TrimEnd += Engine.DeltaTime);
      if (MInput.Keyboard.Check(Keys.LeftControl) && MInput.Keyboard.Pressed(Keys.S))
      {
        while ((double) this.Timeline[0].TimeStamp < (double) this.entity.TrimStart)
          this.Timeline.RemoveAt(0);
        while ((double) this.Timeline[this.Timeline.Count - 1].TimeStamp > (double) this.entity.TrimEnd)
          this.Timeline.RemoveAt(this.Timeline.Count - 1);
        PlaybackData.Export(this.Timeline, this.Filename);
        Engine.Scene = (Scene) new PreviewRecording(this.Filename);
      }
      base.Update();
      this.entity.Hair.AfterUpdate();
    }

    public override void Render()
    {
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      ActiveFont.Draw("A/D:        Move Start Trim", new Vector2(8f, 8f), new Vector2(0.0f, 0.0f), Vector2.One * 0.5f, Color.White);
      ActiveFont.Draw("Left/Right: Move End Trim", new Vector2(8f, 32f), new Vector2(0.0f, 0.0f), Vector2.One * 0.5f, Color.White);
      ActiveFont.Draw("CTRL+S: Save New Trim", new Vector2(8f, 56f), new Vector2(0.0f, 0.0f), Vector2.One * 0.5f, Color.White);
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, RasterizerState.CullNone, (Effect) null, this.Matrix);
      Draw.HollowRect((float) (((double) this.ScreenWidth - (double) this.Width) / 2.0 - 16.0), (float) (((double) this.ScreenHeight - (double) this.Height) / 2.0 - 16.0), this.Width + 32f, this.Height + 32f, Color.Red * 0.6f);
      Draw.HollowRect((float) (((double) this.ScreenWidth - 320.0) / 2.0), (float) (((double) this.ScreenHeight - 180.0) / 2.0), 320f, 180f, Color.White * 0.6f);
      if (this.entity.Visible)
        this.entity.Render();
      Draw.Rect(32f, this.ScreenHeight - 48f, this.ScreenWidth - 64f, 16f, Color.DarkGray);
      Draw.Rect(32f, this.ScreenHeight - 48f, (float) (((double) this.ScreenWidth - 64.0) * ((double) this.entity.Time / (double) this.entity.Duration)), 16f, Color.White);
      Draw.Rect((float) (32.0 + ((double) this.ScreenWidth - 64.0) * ((double) this.entity.Time / (double) this.entity.Duration) - 2.0), this.ScreenHeight - 48f, 4f, 16f, Color.LimeGreen);
      Draw.Rect((float) (32.0 + ((double) this.ScreenWidth - 64.0) * ((double) this.entity.TrimStart / (double) this.entity.Duration) - 2.0), this.ScreenHeight - 48f, 4f, 16f, Color.Red);
      Draw.Rect((float) (32.0 + ((double) this.ScreenWidth - 64.0) * ((double) this.entity.TrimEnd / (double) this.entity.Duration) - 2.0), this.ScreenHeight - 48f, 4f, 16f, Color.Red);
      Draw.SpriteBatch.End();
    }
  }
}
