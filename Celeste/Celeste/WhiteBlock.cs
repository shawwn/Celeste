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
    private float playerDuckTimer = 0.0f;
    private bool enabled = true;
    private const float duckDuration = 3f;
    private bool activated;
    private Monocle.Image sprite;
    private Entity bgSolidTiles;

    public WhiteBlock(EntityData data, Vector2 offset)
      : base(data.Position + offset, 48, true)
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
      this.sprite.Color = Color.White * 0.25f;
      this.Collidable = false;
    }

    private void Activate(Player player)
    {
      Audio.Play("event:/game/04_cliffside/whiteblock_fallthru", this.Center);
      this.activated = true;
      this.Collidable = false;
      player.Depth = 10001;
      Level scene = this.Scene as Level;
      Rectangle rectangle = new Rectangle(scene.Bounds.Left / 8, scene.Bounds.Y / 8, scene.Bounds.Width / 8, scene.Bounds.Height / 8);
      Rectangle tileBounds = scene.Session.MapData.TileBounds;
      bool[,] data = new bool[rectangle.Width, rectangle.Height];
      for (int index1 = 0; index1 < rectangle.Width; ++index1)
      {
        for (int index2 = 0; index2 < rectangle.Height; ++index2)
          data[index1, index2] = scene.BgData[index1 + rectangle.Left - tileBounds.Left, index2 + rectangle.Top - tileBounds.Top] != '0';
      }
      this.bgSolidTiles = (Entity) new Solid(new Vector2((float) scene.Bounds.Left, (float) scene.Bounds.Top), 1f, 1f, true);
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
        if ((this.Scene as Level).Session.HeartGem)
          this.Disable();
      }
      else if (this.Scene.Tracker.GetEntity<HeartGem>() == null)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          this.Disable();
          entity.Depth = 0;
          this.Scene.Remove(this.bgSolidTiles);
        }
      }
    }
  }
}

