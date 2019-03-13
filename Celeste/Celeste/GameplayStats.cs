// Decompiled with JetBrains decompiler
// Type: Celeste.GameplayStats
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class GameplayStats : Entity
  {
    public float DrawLerp = 0.0f;

    public GameplayStats()
    {
      this.Depth = -101;
      this.Tag = (int) Tags.HUD | (int) Tags.Global | (int) Tags.PauseUpdate | (int) Tags.TransitionUpdate;
    }

    public override void Update()
    {
      base.Update();
      Level scene = this.Scene as Level;
      this.DrawLerp = Calc.Approach(this.DrawLerp, !scene.Paused || !scene.PauseMainMenuOpen || scene.Wipe != null ? 0.0f : 1f, Engine.DeltaTime * 8f);
    }

    public override void Render()
    {
      if ((double) this.DrawLerp <= 0.0)
        return;
      float num1 = Ease.CubeOut(this.DrawLerp);
      Level scene = this.Scene as Level;
      AreaKey area = scene.Session.Area;
      AreaModeStats mode = SaveData.Instance.Areas[area.ID].Modes[(int) area.Mode];
      if (!mode.Completed && !SaveData.Instance.CheatMode && !SaveData.Instance.DebugMode)
        return;
      ModeProperties modeProperties = AreaData.Get(area).Mode[(int) area.Mode];
      int totalStrawberries = modeProperties.TotalStrawberries;
      int num2 = 32;
      Vector2 position = new Vector2((float) ((1920 - (totalStrawberries - 1) * num2 - (totalStrawberries <= 0 || modeProperties.Checkpoints == null ? 0 : modeProperties.Checkpoints.Length * num2)) / 2), (float) (1016.0 + (1.0 - (double) num1) * 80.0));
      if (totalStrawberries > 0)
      {
        int num3 = modeProperties.Checkpoints == null ? 1 : modeProperties.Checkpoints.Length + 1;
        for (int index1 = 0; index1 < num3; ++index1)
        {
          int num4 = index1 == 0 ? modeProperties.StartStrawberries : modeProperties.Checkpoints[index1 - 1].Strawberries;
          for (int index2 = 0; index2 < num4; ++index2)
          {
            EntityData entityData = modeProperties.StrawberriesByCheckpoint[index1, index2];
            if (entityData != null)
            {
              bool flag1 = false;
              foreach (EntityID strawberry in scene.Session.Strawberries)
              {
                if (entityData.ID == strawberry.ID && entityData.Level.Name == strawberry.Level)
                  flag1 = true;
              }
              MTexture mtexture = GFX.Gui["dot"];
              if (flag1)
              {
                if (area.Mode == AreaMode.CSide)
                  mtexture.DrawOutlineCentered(position, Calc.HexToColor("f2ff30"), 1.5f);
                else
                  mtexture.DrawOutlineCentered(position, Calc.HexToColor("ff3040"), 1.5f);
              }
              else
              {
                bool flag2 = false;
                foreach (EntityID strawberry in mode.Strawberries)
                {
                  if (entityData.ID == strawberry.ID && entityData.Level.Name == strawberry.Level)
                    flag2 = true;
                }
                if (flag2)
                  mtexture.DrawOutlineCentered(position, Calc.HexToColor("4193ff"), 1f);
                else
                  Draw.Rect(position.X - (float) mtexture.ClipRect.Width * 0.5f, position.Y - 4f, (float) mtexture.ClipRect.Width, 8f, Color.DarkGray);
              }
              position.X += (float) num2;
            }
          }
          if (modeProperties.Checkpoints != null && index1 < modeProperties.Checkpoints.Length)
          {
            Draw.Rect(position.X - 3f, position.Y - 16f, 6f, 32f, Color.DarkGray);
            position.X += (float) num2;
          }
        }
      }
    }
  }
}

