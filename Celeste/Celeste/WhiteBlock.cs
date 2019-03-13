// Decompiled with JetBrains decompiler
// Type: Celeste.WhiteBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class WhiteBlock : JumpThru
  {
    private bool enabled = true;
    private const float duckDuration = 3f;
    private float playerDuckTimer;
    private bool activated;
    private Monocle.Image sprite;
    private Entity bgSolidTiles;

    public WhiteBlock(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset), 48, true)
    {
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["objects/whiteblock"])));
      this.Depth = -9000;
      this.SurfaceSoundIndex = 27;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!(scene as Level).Session.HeartGem)
        return;
      this.Disable();
    }

    private void Disable()
    {
      this.enabled = false;
      this.sprite.Color = Color.op_Multiply(Color.get_White(), 0.25f);
      this.Collidable = false;
    }

    private void Activate(Player player)
    {
      Audio.Play("event:/game/04_cliffside/whiteblock_fallthru", this.Center);
      this.activated = true;
      this.Collidable = false;
      player.Depth = 10001;
      Level scene = this.Scene as Level;
      Rectangle rectangle;
      ref Rectangle local = ref rectangle;
      Rectangle bounds1 = scene.Bounds;
      int num1 = ((Rectangle) ref bounds1).get_Left() / 8;
      int num2 = scene.Bounds.Y / 8;
      int num3 = scene.Bounds.Width / 8;
      int num4 = scene.Bounds.Height / 8;
      ((Rectangle) ref local).\u002Ector(num1, num2, num3, num4);
      Rectangle tileBounds = scene.Session.MapData.TileBounds;
      bool[,] data = new bool[(int) rectangle.Width, (int) rectangle.Height];
      for (int index1 = 0; index1 < rectangle.Width; ++index1)
      {
        for (int index2 = 0; index2 < rectangle.Height; ++index2)
          data[index1, index2] = scene.BgData[index1 + ((Rectangle) ref rectangle).get_Left() - ((Rectangle) ref tileBounds).get_Left(), index2 + ((Rectangle) ref rectangle).get_Top() - ((Rectangle) ref tileBounds).get_Top()] != '0';
      }
      Rectangle bounds2 = scene.Bounds;
      double left = (double) ((Rectangle) ref bounds2).get_Left();
      bounds2 = scene.Bounds;
      double top = (double) ((Rectangle) ref bounds2).get_Top();
      this.bgSolidTiles = (Entity) new Solid(new Vector2((float) left, (float) top), 1f, 1f, true);
      this.bgSolidTiles.Collider = (Collider) new Grid(8f, 8f, data);
      this.Scene.Add(this.bgSolidTiles);
    }

    public override void Update()
    {
      base.Update();
      if (!this.enabled)
        return;
      if (!this.activated)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (this.HasPlayerRider() && entity != null && entity.Ducking)
        {
          this.playerDuckTimer += Engine.DeltaTime;
          if ((double) this.playerDuckTimer >= 3.0)
            this.Activate(entity);
        }
        else
          this.playerDuckTimer = 0.0f;
        if (!(this.Scene as Level).Session.HeartGem)
          return;
        this.Disable();
      }
      else
      {
        if (this.Scene.Tracker.GetEntity<HeartGem>() != null)
          return;
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity == null)
          return;
        this.Disable();
        entity.Depth = 0;
        this.Scene.Remove(this.bgSolidTiles);
      }
    }
  }
}
