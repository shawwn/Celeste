// Decompiled with JetBrains decompiler
// Type: Celeste.Editor.MapEditor
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

namespace Celeste.Editor
{
  public class MapEditor : Scene
  {
    private static readonly Color gridColor = new Color(0.1f, 0.1f, 0.1f);
    private static Camera Camera;
    private static AreaKey area = AreaKey.None;
    private static float saveFlash = 0.0f;
    private MapData mapData;
    private List<LevelTemplate> levels = new List<LevelTemplate>();
    private Vector2 mousePosition;
    private MapEditor.MouseModes mouseMode;
    private Vector2 lastMouseScreenPosition;
    private Vector2 mouseDragStart;
    private HashSet<LevelTemplate> selection = new HashSet<LevelTemplate>();
    private HashSet<LevelTemplate> hovered = new HashSet<LevelTemplate>();
    private float fade;
    private List<Vector2[]> undoStack = new List<Vector2[]>();
    private List<Vector2[]> redoStack = new List<Vector2[]>();

    public MapEditor(AreaKey area, bool reloadMapData = true)
    {
      area.ID = Calc.Clamp(area.ID, 0, AreaData.Areas.Count - 1);
      this.mapData = AreaData.Areas[area.ID].Mode[(int) area.Mode].MapData;
      if (reloadMapData)
        this.mapData.Reload();
      foreach (LevelData level in this.mapData.Levels)
        this.levels.Add(new LevelTemplate(level));
      foreach (Rectangle rectangle in this.mapData.Filler)
        this.levels.Add(new LevelTemplate(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
      if (area != MapEditor.area)
      {
        MapEditor.area = area;
        MapEditor.Camera = new Camera();
        MapEditor.Camera.Zoom = 6f;
        MapEditor.Camera.CenterOrigin();
      }
      if (SaveData.Instance != null)
        return;
      SaveData.InitializeDebugMode();
    }

    public override void GainFocus()
    {
      base.GainFocus();
      this.SaveAndReload();
    }

    private void SelectAll()
    {
      this.selection.Clear();
      foreach (LevelTemplate level in this.levels)
        this.selection.Add(level);
    }

    public void Rename(string oldName, string newName)
    {
      LevelTemplate levelTemplate1 = (LevelTemplate) null;
      LevelTemplate levelTemplate2 = (LevelTemplate) null;
      foreach (LevelTemplate level in this.levels)
      {
        if (levelTemplate1 == null && level.Name == oldName)
        {
          levelTemplate1 = level;
          if (levelTemplate2 != null)
            break;
        }
        else if (levelTemplate2 == null && level.Name == newName)
        {
          levelTemplate2 = level;
          if (levelTemplate1 != null)
            break;
        }
      }
      string path1 = Path.Combine("..", "..", "..", "Content", "Levels", this.mapData.Filename);
      if (levelTemplate2 == null)
      {
        File.Move(Path.Combine(path1, levelTemplate1.Name + ".xml"), Path.Combine(path1, newName + ".xml"));
        levelTemplate1.Name = newName;
      }
      else
      {
        string str = Path.Combine(path1, "TEMP.xml");
        File.Move(Path.Combine(path1, levelTemplate1.Name + ".xml"), str);
        File.Move(Path.Combine(path1, levelTemplate2.Name + ".xml"), Path.Combine(path1, oldName + ".xml"));
        File.Move(str, Path.Combine(path1, newName + ".xml"));
        levelTemplate1.Name = newName;
        levelTemplate2.Name = oldName;
      }
      this.Save();
    }

    private void Save()
    {
    }

    private void SaveAndReload()
    {
    }

    private void UpdateMouse() => this.mousePosition = Vector2.Transform(MInput.Mouse.Position, Matrix.Invert(MapEditor.Camera.Matrix));

    public override void Update()
    {
      Vector2 vector2;
      vector2.X = (this.lastMouseScreenPosition.X - MInput.Mouse.Position.X) / MapEditor.Camera.Zoom;
      vector2.Y = (this.lastMouseScreenPosition.Y - MInput.Mouse.Position.Y) / MapEditor.Camera.Zoom;
      if (MInput.Keyboard.Pressed(Keys.Space) && MInput.Keyboard.Check(Keys.LeftControl))
      {
        MapEditor.Camera.Zoom = 6f;
        MapEditor.Camera.Position = Vector2.Zero;
      }
      int num = Math.Sign(MInput.Mouse.WheelDelta);
      if (num > 0 && (double) MapEditor.Camera.Zoom >= 1.0 || (double) MapEditor.Camera.Zoom > 1.0)
        MapEditor.Camera.Zoom += (float) num;
      else
        MapEditor.Camera.Zoom += (float) num * 0.25f;
      MapEditor.Camera.Zoom = Math.Max(0.25f, Math.Min(24f, MapEditor.Camera.Zoom));
      MapEditor.Camera.Position += new Vector2((float) Input.MoveX.Value, (float) Input.MoveY.Value) * 300f * Engine.DeltaTime;
      this.UpdateMouse();
      this.hovered.Clear();
      if (this.mouseMode == MapEditor.MouseModes.Hover)
      {
        this.mouseDragStart = this.mousePosition;
        if (MInput.Mouse.PressedLeftButton)
        {
          bool flag1 = this.LevelCheck(this.mousePosition);
          if (MInput.Keyboard.Check(Keys.Space))
            this.mouseMode = MapEditor.MouseModes.Pan;
          else if (MInput.Keyboard.Check(Keys.LeftControl))
          {
            if (flag1)
              this.ToggleSelection(this.mousePosition);
            else
              this.mouseMode = MapEditor.MouseModes.Select;
          }
          else if (MInput.Keyboard.Check(Keys.F))
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
              if (!MInput.Keyboard.Check(Keys.F))
                return;
              this.levels.Remove(level);
              return;
            }
            this.LoadLevel(level, this.mousePosition * 8f);
            return;
          }
        }
        else if (MInput.Mouse.PressedMiddleButton)
          this.mouseMode = MapEditor.MouseModes.Pan;
        else if (!MInput.Keyboard.Check(Keys.Space))
        {
          foreach (LevelTemplate level in this.levels)
          {
            if (level.Check(this.mousePosition))
              this.hovered.Add(level);
          }
          if (MInput.Keyboard.Check(Keys.LeftControl))
          {
            if (MInput.Keyboard.Pressed(Keys.Z))
              this.Undo();
            else if (MInput.Keyboard.Pressed(Keys.Y))
              this.Redo();
            else if (MInput.Keyboard.Pressed(Keys.A))
              this.SelectAll();
          }
        }
      }
      else if (this.mouseMode == MapEditor.MouseModes.Pan)
      {
        MapEditor.Camera.Position += vector2;
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
          if (MInput.Keyboard.Check(Keys.LeftControl))
            this.ToggleSelection(mouseRect);
          else
            this.SetSelection(mouseRect);
          this.mouseMode = MapEditor.MouseModes.Hover;
        }
      }
      else if (this.mouseMode == MapEditor.MouseModes.Move)
      {
        Vector2 relativeMove = (this.mousePosition - this.mouseDragStart).Round();
        bool snap = this.selection.Count == 1 && !MInput.Keyboard.Check(Keys.LeftAlt);
        foreach (LevelTemplate levelTemplate in this.selection)
          levelTemplate.Move(relativeMove, this.levels, snap);
        if (!MInput.Mouse.CheckLeftButton)
          this.mouseMode = MapEditor.MouseModes.Hover;
      }
      else if (this.mouseMode == MapEditor.MouseModes.Resize)
      {
        Vector2 relativeMove = (this.mousePosition - this.mouseDragStart).Round();
        foreach (LevelTemplate levelTemplate in this.selection)
          levelTemplate.Resize(relativeMove);
        if (!MInput.Mouse.CheckLeftButton)
          this.mouseMode = MapEditor.MouseModes.Hover;
      }
      if (MInput.Keyboard.Pressed(Keys.D1))
        this.SetEditorColor(0);
      else if (MInput.Keyboard.Pressed(Keys.D2))
        this.SetEditorColor(1);
      else if (MInput.Keyboard.Pressed(Keys.D3))
        this.SetEditorColor(2);
      else if (MInput.Keyboard.Pressed(Keys.D4))
        this.SetEditorColor(3);
      else if (MInput.Keyboard.Pressed(Keys.D5))
        this.SetEditorColor(4);
      else if (MInput.Keyboard.Pressed(Keys.D6))
        this.SetEditorColor(5);
      else if (MInput.Keyboard.Pressed(Keys.D7))
        this.SetEditorColor(6);
      if (MInput.Keyboard.Pressed(Keys.F1) || MInput.Keyboard.Check(Keys.LeftControl) && MInput.Keyboard.Pressed(Keys.S))
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
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, MapEditor.Camera.Matrix * Engine.ScreenMatrix);
      float width = 1920f / MapEditor.Camera.Zoom;
      float height = 1080f / MapEditor.Camera.Zoom;
      int num1 = 5;
      float num2 = (float) Math.Floor((double) MapEditor.Camera.Left / (double) num1 - 1.0) * (float) num1;
      float num3 = (float) Math.Floor((double) MapEditor.Camera.Top / (double) num1 - 1.0) * (float) num1;
      for (float num4 = num2; (double) num4 <= (double) num2 + (double) width + 10.0; num4 += 5f)
        Draw.Line(num4, MapEditor.Camera.Top, num4, MapEditor.Camera.Top + height, MapEditor.gridColor);
      for (float num5 = num3; (double) num5 <= (double) num3 + (double) height + 10.0; num5 += 5f)
        Draw.Line(MapEditor.Camera.Left, num5, MapEditor.Camera.Left + width, num5, MapEditor.gridColor);
      Draw.Line(0.0f, MapEditor.Camera.Top, 0.0f, MapEditor.Camera.Top + height, Color.DarkSlateBlue, 1f / MapEditor.Camera.Zoom);
      Draw.Line(MapEditor.Camera.Left, 0.0f, MapEditor.Camera.Left + width, 0.0f, Color.DarkSlateBlue, 1f / MapEditor.Camera.Zoom);
      foreach (LevelTemplate level in this.levels)
        level.RenderContents(MapEditor.Camera, this.levels);
      foreach (LevelTemplate level in this.levels)
        level.RenderOutline(MapEditor.Camera);
      foreach (LevelTemplate level in this.levels)
        level.RenderHighlight(MapEditor.Camera, this.selection.Contains(level), this.hovered.Contains(level));
      if (this.mouseMode == MapEditor.MouseModes.Hover)
      {
        Draw.Line(this.mousePosition.X - 12f / MapEditor.Camera.Zoom, this.mousePosition.Y, this.mousePosition.X + 12f / MapEditor.Camera.Zoom, this.mousePosition.Y, Color.Yellow, 3f / MapEditor.Camera.Zoom);
        Draw.Line(this.mousePosition.X, this.mousePosition.Y - 12f / MapEditor.Camera.Zoom, this.mousePosition.X, this.mousePosition.Y + 12f / MapEditor.Camera.Zoom, Color.Yellow, 3f / MapEditor.Camera.Zoom);
      }
      else if (this.mouseMode == MapEditor.MouseModes.Select)
        Draw.Rect(this.GetMouseRect(this.mouseDragStart, this.mousePosition), Color.Lime * 0.25f);
      if ((double) MapEditor.saveFlash > 0.0)
        Draw.Rect(MapEditor.Camera.Left, MapEditor.Camera.Top, width, height, Color.White * Ease.CubeInOut(MapEditor.saveFlash));
      if ((double) this.fade > 0.0)
        Draw.Rect(0.0f, 0.0f, 320f, 180f, Color.Black * this.fade);
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      Draw.Rect(0.0f, 0.0f, 1920f, 72f, Color.Black);
      Vector2 position1 = new Vector2(16f, 4f);
      Vector2 position2 = new Vector2(1904f, 4f);
      if (MInput.Keyboard.Check(Keys.Q))
      {
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.25f);
        using (List<LevelTemplate>.Enumerator enumerator = this.levels.GetEnumerator())
        {
label_35:
          while (enumerator.MoveNext())
          {
            LevelTemplate current = enumerator.Current;
            int index = 0;
            while (true)
            {
              if (current.Strawberries != null && index < current.Strawberries.Count)
              {
                Vector2 strawberry = current.Strawberries[index];
                ActiveFont.DrawOutline(current.StrawberryMetadata[index], (new Vector2((float) current.X + strawberry.X, (float) current.Y + strawberry.Y) - MapEditor.Camera.Position) * MapEditor.Camera.Zoom + new Vector2(960f, 532f), new Vector2(0.5f, 1f), Vector2.One * 1f, Color.Red, 2f, Color.Black);
                ++index;
              }
              else
                goto label_35;
            }
          }
        }
      }
      if (this.hovered.Count == 0)
      {
        if (this.selection.Count > 0)
        {
          ActiveFont.Draw(this.selection.Count.ToString() + " levels selected", position1, Color.Red);
        }
        else
        {
          ActiveFont.Draw(Dialog.Clean(this.mapData.Data.Name), position1, Color.Aqua);
          ActiveFont.Draw(this.mapData.Area.Mode.ToString() + " MODE", position2, Vector2.UnitX, Vector2.One, Color.Red);
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
        ActiveFont.Draw(levelTemplate.Name, position1, Color.Yellow);
        Vector2 position3 = position2;
        Vector2 unitX = Vector2.UnitX;
        Vector2 one = Vector2.One;
        Color green = Color.Green;
        ActiveFont.Draw(text, position3, unitX, one, green);
      }
      else
        ActiveFont.Draw(this.hovered.Count.ToString() + " levels", position1, Color.Yellow);
      Draw.SpriteBatch.End();
    }

    private void LoadLevel(LevelTemplate level, Vector2 at)
    {
      this.Save();
      Engine.Scene = (Scene) new LevelLoader(new Session(MapEditor.area)
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
      Vector2 vector2_1 = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
      Vector2 vector2_2 = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
      return new Rectangle((int) vector2_1.X, (int) vector2_1.Y, (int) ((double) vector2_2.X - (double) vector2_1.X), (int) ((double) vector2_2.Y - (double) vector2_1.Y));
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
