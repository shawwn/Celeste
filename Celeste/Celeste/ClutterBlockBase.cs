// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterBlockBase
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class ClutterBlockBase : Solid
  {
    private static readonly Color enabledColor = Color.Black * 0.7f;
    private static readonly Color disabledColor = Color.Black * 0.3f;
    public ClutterBlock.Colors BlockColor;
    private Color color;
    private bool enabled;
    private LightOcclude occluder;

    public ClutterBlockBase(
      Vector2 position,
      int width,
      int height,
      bool enabled,
      ClutterBlock.Colors blockColor)
      : base(position, (float) width, (float) height, true)
    {
      this.EnableAssistModeChecks = false;
      this.BlockColor = blockColor;
      this.Depth = 8999;
      this.enabled = enabled;
      this.color = enabled ? ClutterBlockBase.enabledColor : ClutterBlockBase.disabledColor;
      if (enabled)
        this.Add((Component) (this.occluder = new LightOcclude(1f)));
      else
        this.Collidable = false;
      switch (blockColor)
      {
        case ClutterBlock.Colors.Red:
          this.SurfaceSoundIndex = 17;
          break;
        case ClutterBlock.Colors.Green:
          this.SurfaceSoundIndex = 19;
          break;
        case ClutterBlock.Colors.Yellow:
          this.SurfaceSoundIndex = 18;
          break;
      }
    }

    public void Deactivate()
    {
      this.Collidable = false;
      this.color = ClutterBlockBase.disabledColor;
      this.enabled = false;
      if (this.occluder == null)
        return;
      this.Remove((Component) this.occluder);
      this.occluder = (LightOcclude) null;
    }

    public override void Render()
    {
      Draw.Rect(this.X, this.Y, this.Width, this.Height + (this.enabled ? 2f : 0.0f), this.color);
    }
  }
}

