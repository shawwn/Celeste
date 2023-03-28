// Decompiled with JetBrains decompiler
// Type: Celeste.ReturnMapHint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class ReturnMapHint : Entity
  {
    private MTexture checkpoint;

    public ReturnMapHint() => this.Tag = (int) Tags.HUD;

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Session session = (this.Scene as Level).Session;
      AreaKey area = session.Area;
      HashSet<string> checkpoints = SaveData.Instance.GetCheckpoints(area);
      CheckpointData checkpointData = (CheckpointData) null;
      ModeProperties modeProperties = AreaData.Areas[area.ID].Mode[(int) area.Mode];
      if (modeProperties.Checkpoints != null)
      {
        foreach (CheckpointData checkpoint in modeProperties.Checkpoints)
        {
          if (session.LevelFlags.Contains(checkpoint.Level) && checkpoints.Contains(checkpoint.Level))
            checkpointData = checkpoint;
        }
      }
      string id = area.ToString();
      if (checkpointData != null)
        id = id + "_" + checkpointData.Level;
      if (!MTN.Checkpoints.Has(id))
        return;
      this.checkpoint = MTN.Checkpoints[id];
    }

    public static string GetCheckpointPreviewName(AreaKey area, string level) => level == null ? area.ToString() : area.ToString() + "_" + level;

    private MTexture GetCheckpointPreview(AreaKey area, string level)
    {
      string checkpointPreviewName = ReturnMapHint.GetCheckpointPreviewName(area, level);
      return MTN.Checkpoints.Has(checkpointPreviewName) ? MTN.Checkpoints[checkpointPreviewName] : (MTexture) null;
    }

    public override void Render()
    {
      MTexture mtexture = GFX.Gui["checkpoint"];
      string text = Dialog.Clean("MENU_RETURN_INFO");
      MTexture checkpoint = MTN.Checkpoints["polaroid"];
      float num1 = ActiveFont.Measure(text).X * 0.75f;
      if (this.checkpoint != null)
      {
        float num2 = (float) checkpoint.Width * 0.25f;
        Vector2 vector2 = new Vector2((float) ((1920.0 - (double) num1 - (double) num2 - 64.0) / 2.0), 730f);
        float num3 = 720f / (float) this.checkpoint.ClipRect.Width;
        ActiveFont.DrawOutline(text, vector2 + new Vector2(num1 / 2f, 0.0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
        vector2.X += num1 + 64f;
        checkpoint.DrawCentered(vector2 + new Vector2(num2 / 2f, 0.0f), Color.White, 0.25f, 0.1f);
        this.checkpoint.DrawCentered(vector2 + new Vector2(num2 / 2f, 0.0f), Color.White, 0.25f * num3, 0.1f);
        mtexture.DrawCentered(vector2 + new Vector2(num2 * 0.8f, (float) ((double) checkpoint.Height * 0.25 * 0.5 * 0.800000011920929)), Color.White, 0.75f);
      }
      else
      {
        float num4 = (float) mtexture.Width * 0.75f;
        Vector2 vector2 = new Vector2((float) ((1920.0 - (double) num1 - (double) num4 - 64.0) / 2.0), 730f);
        ActiveFont.DrawOutline(text, vector2 + new Vector2(num1 / 2f, 0.0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
        vector2.X += num1 + 64f;
        mtexture.DrawCentered(vector2 + new Vector2(num4 * 0.5f, 0.0f), Color.White, 0.75f);
      }
    }
  }
}
