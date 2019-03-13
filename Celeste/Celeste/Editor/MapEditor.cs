// Decompiled with JetBrains decompiler
// Type: Celeste.Editor.MapEditor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Editor
{
  public class MapEditor : Scene
  {
    private static readonly Color gridColor = new Color(0.1f, 0.1f, 0.1f);
    private static AreaKey area = AreaKey.None;
    private static float saveFlash = 0.0f;
    private List<LevelTemplate> levels = new List<LevelTemplate>();
    private HashSet<LevelTemplate> selection = new HashSet<LevelTemplate>();
    private HashSet<LevelTemplate> hovered = new HashSet<LevelTemplate>();
    private List<Vector2[]> undoStack = new List<Vector2[]>();
    private List<Vector2[]> redoStack = new List<Vector2[]>();
    private static Camera Camera;
    private MapData mapData;
    private Vector2 mousePosition;
    private MapEditor.MouseModes mouseMode;
    private Vector2 lastMouseScreenPosition;
    private Vector2 mouseDragStart;
    private float fade;

    public MapEditor(AreaKey area, bool reloadMapData = true)
    {
      area.ID = Calc.Clamp(area.ID, 0, AreaData.Areas.Count - 1);
      this.mapData = AreaData.Areas[area.ID].Mode[(int) area.Mode].MapData;
      if (reloadMapData)
        this.mapData.Reload();
      foreach (LevelData level in this.mapData.Levels)
        this.levels.Add(new LevelTemplate(level));
      using (List<Rectangle>.Enumerator enumerator = this.mapData.Filler.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Rectangle current = enumerator.Current;
          this.levels.Add(new LevelTemplate((int) current.X, (int) current.Y, (int) current.Width, (int) current.Height));
        }
      }
      if (area != MapEditor.area)
      {
        MapEditor.area = area;
        MapEditor.Camera = new Camera();
        MapEditor.Camera.Zoom = 6f;
        MapEditor.Camera.CenterOrigin();
      }
      if (SaveData.Instance != null)
        return;
      SaveData.InitializeDebugMode(true);
    }

    public override void GainFocus()
    {
      base.GainFocus();
      this.SaveAndReload();
    }

    private void Save()
    {
    }

    private void SaveAndReload()
    {
    }

    private void UpdateMouse()
    {
      this.mousePosition = Vector2.Transform(MInput.Mouse.Position, Matrix.Invert(MapEditor.Camera.Matrix));
    }

    public override void Update()
    {
      Vector2 vector2;
      vector2.X = (__Null) ((this.lastMouseScreenPosition.X - MInput.Mouse.Position.X) / (double) MapEditor.Camera.Zoom);
      vector2.Y = (__Null) ((this.lastMouseScreenPosition.Y - MInput.Mouse.Position.Y) / (double) MapEditor.Camera.Zoom);
      if (MInput.Keyboard.Pressed((Keys) 32) && MInput.Keyboard.Check((Keys) 162))
      {
        MapEditor.Camera.Zoom = 6f;
        MapEditor.Camera.Position = Vector2.get_Zero();
      }
      MapEditor.Camera.Zoom += (float) Math.Sign(MInput.Mouse.WheelDelta) * 1f;
      MapEditor.Camera.Zoom = Math.Max(1f, Math.Min(24f, MapEditor.Camera.Zoom));
      Camera camera1 = MapEditor.Camera;
      camera1.Position = Vector2.op_Addition(camera1.Position, Vector2.op_Multiply(Vector2.op_Multiply(new Vector2((float) Celeste.Input.MoveX.Value, (float) Celeste.Input.MoveY.Value), 300f), Engine.DeltaTime));
      this.UpdateMouse();
      this.hovered.Clear();
      if (this.mouseMode == MapEditor.MouseModes.Hover)
      {
        this.mouseDragStart = this.mousePosition;
        if (MInput.Mouse.PressedLeftButton)
        {
          bool flag1 = this.LevelCheck(this.mousePosition);
          if (MInput.Keyboard.Check((Keys) 32))
            this.mouseMode = MapEditor.MouseModes.Pan;
          else if (MInput.Keyboard.Check((Keys) 162))
          {
            if (flag1)
              this.ToggleSelection(this.mousePosition);
            else
              this.mouseMode = MapEditor.MouseModes.Select;
          }
          else if (MInput.Keyboard.Check((Keys) 70))
            this.levels.Add(new LevelTemplate((int) this.mousePosition.X, (int) this.mousePosition.Y, 32, 32));
          else if (flag1)
          {
            if (!this.SelectionCheck(this.mousePosition))
              this.SetSelection(this.mousePosition);
            bool flag2 = false;
            if (this.selection.Count == 1)
            {
              foreach (LevelTemplate levelTemplate in this.selection)
              {
                if (levelTemplate.ResizePosition(this.mousePosition) && levelTemplate.Type == LevelTemplateType.Filler)
                  flag2 = true;
              }
            }
            if (flag2)
            {
              foreach (LevelTemplate levelTemplate in this.selection)
                levelTemplate.StartResizing();
              this.mouseMode = MapEditor.MouseModes.Resize;
            }
            else
            {
              this.StoreUndo();
              foreach (LevelTemplate levelTemplate in this.selection)
                levelTemplate.StartMoving();
              this.mouseMode = MapEditor.MouseModes.Move;
            }
          }
          else
            this.mouseMode = MapEditor.MouseModes.Select;
        }
        else if (MInput.Mouse.PressedRightButton)
        {
          LevelTemplate level = this.TestCheck(this.mousePosition);
          if (level != null)
          {
            if (level.Type == LevelTemplateType.Filler)
            {
              if (!MInput.Keyboard.Check((Keys) 70))
                return;
              this.levels.Remove(level);
              return;
            }
            this.LoadLevel(level, Vector2.op_Multiply(this.mousePosition, 8f));
            return;
          }
        }
        else if (MInput.Mouse.PressedMiddleButton)
          this.mouseMode = MapEditor.MouseModes.Pan;
        else if (!MInput.Keyboard.Check((Keys) 32))
        {
          foreach (LevelTemplate level in this.levels)
          {
            if (level.Check(this.mousePosition))
              this.hovered.Add(level);
          }
          if (MInput.Keyboard.Check((Keys) 162))
          {
            if (MInput.Keyboard.Pressed((Keys) 90))
              this.Undo();
            else if (MInput.Keyboard.Pressed((Keys) 89))
              this.Redo();
          }
        }
      }
      else if (this.mouseMode == MapEditor.MouseModes.Pan)
      {
        Camera camera2 = MapEditor.Camera;
        camera2.Position = Vector2.op_Addition(camera2.Position, vector2);
        if (!MInput.Mouse.CheckLeftButton && !MInput.Mouse.CheckMiddleButton)
          this.mouseMode = MapEditor.MouseModes.Hover;
      }
      else if (this.mouseMode == MapEditor.MouseModes.Select)
      {
        Rectangle mouseRect = this.GetMouseRect(this.mouseDragStart, this.mousePosition);
        foreach (LevelTemplate level in this.levels)
        {
          if (level.Check(mouseRect))
            this.hovered.Add(level);
        }
        if (!MInput.Mouse.CheckLeftButton)
        {
          if (MInput.Keyboard.Check((Keys) 162))
            this.ToggleSelection(mouseRect);
          else
            this.SetSelection(mouseRect);
          this.mouseMode = MapEditor.MouseModes.Hover;
        }
      }
      else if (this.mouseMode == MapEditor.MouseModes.Move)
      {
        Vector2 relativeMove = Vector2.op_Subtraction(this.mousePosition, this.mouseDragStart).Round();
        bool snap = this.selection.Count == 1 && !MInput.Keyboard.Check((Keys) 164);
        foreach (LevelTemplate levelTemplate in this.selection)
          levelTemplate.Move(relativeMove, this.levels, snap);
        if (!MInput.Mouse.CheckLeftButton)
          this.mouseMode = MapEditor.MouseModes.Hover;
      }
      else if (this.mouseMode == MapEditor.MouseModes.Resize)
      {
        Vector2 relativeMove = Vector2.op_Subtraction(this.mousePosition, this.mouseDragStart).Round();
        foreach (LevelTemplate levelTemplate in this.selection)
          levelTemplate.Resize(relativeMove);
        if (!MInput.Mouse.CheckLeftButton)
          this.mouseMode = MapEditor.MouseModes.Hover;
      }
      if (MInput.Keyboard.Pressed((Keys) 49))
        this.SetEditorColor(0);
      else if (MInput.Keyboard.Pressed((Keys) 50))
        this.SetEditorColor(1);
      else if (MInput.Keyboard.Pressed((Keys) 51))
        this.SetEditorColor(2);
      else if (MInput.Keyboard.Pressed((Keys) 52))
        this.SetEditorColor(3);
      else if (MInput.Keyboard.Pressed((Keys) 53))
        this.SetEditorColor(4);
      else if (MInput.Keyboard.Pressed((Keys) 54))
        this.SetEditorColor(5);
      else if (MInput.Keyboard.Pressed((Keys) 55))
        this.SetEditorColor(6);
      if (MInput.Keyboard.Pressed((Keys) 112) || MInput.Keyboard.Check((Keys) 162) && MInput.Keyboard.Pressed((Keys) 83))
      {
        this.SaveAndReload();
      }
      else
      {
        if ((double) MapEditor.saveFlash > 0.0)
          MapEditor.saveFlash -= Engine.DeltaTime * 4f;
        this.lastMouseScreenPosition = MInput.Mouse.Position;
        base.Update();
      }
    }

    private void SetEditorColor(int index)
    {
      foreach (LevelTemplate levelTemplate in this.selection)
        levelTemplate.EditorColorIndex = index;
    }

    public override void Render()
    {
      this.UpdateMouse();
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, (Effect) null, Matrix.op_Multiply(MapEditor.Camera.Matrix, Engine.ScreenMatrix));
      float width = 1920f / MapEditor.Camera.Zoom;
      float height = 1080f / MapEditor.Camera.Zoom;
      int num1 = 5;
      float num2 = (float) Math.Floor((double) MapEditor.Camera.Left / (double) num1 - 1.0) * (float) num1;
      float num3 = (float) Math.Floor((double) MapEditor.Camera.Top / (double) num1 - 1.0) * (float) num1;
      for (float num4 = num2; (double) num4 <= (double) num2 + (double) width + 10.0; num4 += 5f)
        Draw.Line(num4, MapEditor.Camera.Top, num4, MapEditor.Camera.Top + height, MapEditor.gridColor);
      for (float num4 = num3; (double) num4 <= (double) num3 + (double) height + 10.0; num4 += 5f)
        Draw.Line(MapEditor.Camera.Left, num4, MapEditor.Camera.Left + width, num4, MapEditor.gridColor);
      Draw.Line(0.0f, MapEditor.Camera.Top, 0.0f, MapEditor.Camera.Top + height, Color.get_DarkSlateBlue());
      Draw.Line(MapEditor.Camera.Left, 0.0f, MapEditor.Camera.Left + width, 0.0f, Color.get_DarkSlateBlue());
      foreach (LevelTemplate level in this.levels)
        level.Render(this.selection.Contains(level), this.hovered.Contains(level), this.levels);
      if (this.mouseMode == MapEditor.MouseModes.Hover)
      {
        Draw.Line((float) (this.mousePosition.X - 4.0), (float) this.mousePosition.Y, (float) (this.mousePosition.X + 3.0), (float) this.mousePosition.Y, Color.get_Yellow());
        Draw.Line((float) this.mousePosition.X, (float) (this.mousePosition.Y - 3.0), (float) this.mousePosition.X, (float) (this.mousePosition.Y + 4.0), Color.get_Yellow());
      }
      else if (this.mouseMode == MapEditor.MouseModes.Select)
        Draw.Rect(this.GetMouseRect(this.mouseDragStart, this.mousePosition), Color.op_Multiply(Color.get_Lime(), 0.25f));
      if ((double) MapEditor.saveFlash > 0.0)
        Draw.Rect(MapEditor.Camera.Left, MapEditor.Camera.Top, width, height, Color.op_Multiply(Color.get_White(), Ease.CubeInOut(MapEditor.saveFlash)));
      if ((double) this.fade > 0.0)
        Draw.Rect(0.0f, 0.0f, 320f, 180f, Color.op_Multiply(Color.get_Black(), this.fade));
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      Draw.Rect(0.0f, 0.0f, 1920f, 72f, Color.get_Black());
      Vector2 position1;
      ((Vector2) ref position1).\u002Ector(16f, 4f);
      Vector2 position2;
      ((Vector2) ref position2).\u002Ector(1904f, 4f);
      if (MInput.Keyboard.Check((Keys) 81))
      {
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.get_Black(), 0.25f));
        using (List<LevelTemplate>.Enumerator enumerator = this.levels.GetEnumerator())
        {
label_25:
          while (enumerator.MoveNext())
          {
            LevelTemplate current = enumerator.Current;
            int index = 0;
            while (true)
            {
              if (current.Strawberries != null && index < current.Strawberries.Count)
              {
                Vector2 strawberry = current.Strawberries[index];
                ActiveFont.DrawOutline(current.StrawberryMetadata[index], Vector2.op_Addition(Vector2.op_Multiply(Vector2.op_Subtraction(new Vector2((float) current.X + (float) strawberry.X, (float) current.Y + (float) strawberry.Y), MapEditor.Camera.Position), MapEditor.Camera.Zoom), new Vector2(960f, 532f)), new Vector2(0.5f, 1f), Vector2.op_Multiply(Vector2.get_One(), 1f), Color.get_Red(), 2f, Color.get_Black());
                ++index;
              }
              else
                goto label_25;
            }
          }
        }
      }
      if (this.hovered.Count == 0)
      {
        if (this.selection.Count > 0)
        {
          ActiveFont.Draw(this.selection.Count.ToString() + " levels selected", position1, Color.get_Red());
        }
        else
        {
          ActiveFont.Draw(Dialog.Clean(this.mapData.Data.Name, (Language) null), position1, Color.get_Aqua());
          ActiveFont.Draw(((int) this.mapData.Area.Mode).ToString() + " MODE", position2, Vector2.get_UnitX(), Vector2.get_One(), Color.get_Red());
        }
      }
      else if (this.hovered.Count == 1)
      {
        LevelTemplate levelTemplate = (LevelTemplate) null;
        using (HashSet<LevelTemplate>.Enumerator enumerator = this.hovered.GetEnumerator())
        {
          if (enumerator.MoveNext())
            levelTemplate = enumerator.Current;
        }
        string text = levelTemplate.ActualWidth.ToString() + "x" + levelTemplate.ActualHeight.ToString() + "   " + (object) levelTemplate.X + "," + (object) levelTemplate.Y + "   " + (object) (levelTemplate.X * 8) + "," + (object) (levelTemplate.Y * 8);
        ActiveFont.Draw(levelTemplate.Name, position1, Color.get_Yellow());
        Vector2 position3 = position2;
        Vector2 unitX = Vector2.get_UnitX();
        Vector2 one = Vector2.get_One();
        Color green = Color.get_Green();
        ActiveFont.Draw(text, position3, unitX, one, green);
      }
      else
        ActiveFont.Draw(this.hovered.Count.ToString() + " levels", position1, Color.get_Yellow());
      Draw.SpriteBatch.End();
    }

    private void LoadLevel(LevelTemplate level, Vector2 at)
    {
      this.Save();
      Engine.Scene = (Scene) new LevelLoader(new Session(MapEditor.area, (string) null, (AreaStats) null)
      {
        FirstLevel = false,
        Level = level.Name,
        StartedFromBeginning = false
      }, new Vector2?(at));
    }

    private void StoreUndo()
    {
      Vector2[] vector2Array = new Vector2[this.levels.Count];
      for (int index = 0; index < this.levels.Count; ++index)
        vector2Array[index] = new Vector2((float) this.levels[index].X, (float) this.levels[index].Y);
      this.undoStack.Add(vector2Array);
      while (this.undoStack.Count > 30)
        this.undoStack.RemoveAt(0);
      this.redoStack.Clear();
    }

    private void Undo()
    {
      if (this.undoStack.Count <= 0)
        return;
      Vector2[] vector2Array = new Vector2[this.levels.Count];
      for (int index = 0; index < this.levels.Count; ++index)
        vector2Array[index] = new Vector2((float) this.levels[index].X, (float) this.levels[index].Y);
      this.redoStack.Add(vector2Array);
      Vector2[] undo = this.undoStack[this.undoStack.Count - 1];
      this.undoStack.RemoveAt(this.undoStack.Count - 1);
      for (int index = 0; index < undo.Length; ++index)
      {
        this.levels[index].X = (int) undo[index].X;
        this.levels[index].Y = (int) undo[index].Y;
      }
    }

    private void Redo()
    {
      if (this.redoStack.Count <= 0)
        return;
      Vector2[] vector2Array = new Vector2[this.levels.Count];
      for (int index = 0; index < this.levels.Count; ++index)
        vector2Array[index] = new Vector2((float) this.levels[index].X, (float) this.levels[index].Y);
      this.undoStack.Add(vector2Array);
      Vector2[] redo = this.redoStack[this.undoStack.Count - 1];
      this.redoStack.RemoveAt(this.undoStack.Count - 1);
      for (int index = 0; index < redo.Length; ++index)
      {
        this.levels[index].X = (int) redo[index].X;
        this.levels[index].Y = (int) redo[index].Y;
      }
    }

    private Rectangle GetMouseRect(Vector2 a, Vector2 b)
    {
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector(Math.Min((float) a.X, (float) b.X), Math.Min((float) a.Y, (float) b.Y));
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector(Math.Max((float) a.X, (float) b.X), Math.Max((float) a.Y, (float) b.Y));
      return new Rectangle((int) vector2_1.X, (int) vector2_1.Y, (int) (vector2_2.X - vector2_1.X), (int) (vector2_2.Y - vector2_1.Y));
    }

    private LevelTemplate TestCheck(Vector2 point)
    {
      foreach (LevelTemplate level in this.levels)
      {
        if (!level.Dummy && level.Check(point))
          return level;
      }
      return (LevelTemplate) null;
    }

    private bool LevelCheck(Vector2 point)
    {
      foreach (LevelTemplate level in this.levels)
      {
        if (level.Check(point))
          return true;
      }
      return false;
    }

    private bool SelectionCheck(Vector2 point)
    {
      foreach (LevelTemplate levelTemplate in this.selection)
      {
        if (levelTemplate.Check(point))
          return true;
      }
      return false;
    }

    private bool SetSelection(Vector2 point)
    {
      this.selection.Clear();
      foreach (LevelTemplate level in this.levels)
      {
        if (level.Check(point))
          this.selection.Add(level);
      }
      return this.selection.Count > 0;
    }

    private bool ToggleSelection(Vector2 point)
    {
      bool flag = false;
      foreach (LevelTemplate level in this.levels)
      {
        if (level.Check(point))
        {
          flag = true;
          if (this.selection.Contains(level))
            this.selection.Remove(level);
          else
            this.selection.Add(level);
        }
      }
      return flag;
    }

    private void SetSelection(Rectangle rect)
    {
      this.selection.Clear();
      foreach (LevelTemplate level in this.levels)
      {
        if (level.Check(rect))
          this.selection.Add(level);
      }
    }

    private void ToggleSelection(Rectangle rect)
    {
      foreach (LevelTemplate level in this.levels)
      {
        if (level.Check(rect))
        {
          if (this.selection.Contains(level))
            this.selection.Remove(level);
          else
            this.selection.Add(level);
        }
      }
    }

    private enum MouseModes
    {
      Hover,
      Pan,
      Select,
      Move,
      Resize,
    }
  }
}
