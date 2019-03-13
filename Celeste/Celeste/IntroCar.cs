// Decompiled with JetBrains decompiler
// Type: Celeste.IntroCar
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class IntroCar : JumpThru
  {
    private Monocle.Image bodySprite;
    private Entity wheels;
    private float startY;
    private bool didHaveRider;

    public IntroCar(Vector2 position)
      : base(position, 25, true)
    {
      this.startY = (float) position.Y;
      this.Depth = 1;
      this.Add((Component) (this.bodySprite = new Monocle.Image(GFX.Game["scenery/car/body"])));
      this.bodySprite.Origin = new Vector2(this.bodySprite.Width / 2f, this.bodySprite.Height);
      this.Collider = (Collider) new ColliderList(new Collider[2]
      {
        (Collider) new Hitbox(25f, 4f, -15f, -17f),
        (Collider) new Hitbox(19f, 4f, 8f, -11f)
      });
      this.SurfaceSoundIndex = 2;
    }

    public IntroCar(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Monocle.Image image = new Monocle.Image(GFX.Game["scenery/car/wheels"]);
      image.Origin = new Vector2(image.Width / 2f, image.Height);
      this.wheels = new Entity(this.Position);
      this.wheels.Add((Component) image);
      this.wheels.Depth = 3;
      scene.Add(this.wheels);
      Level level = scene as Level;
      if (level.Session.Area.ID != 0)
        return;
      Rectangle bounds1 = level.Bounds;
      Vector2 position = new Vector2((float) ((Rectangle) ref bounds1).get_Left(), this.Y);
      double x = (double) this.X;
      Rectangle bounds2 = level.Bounds;
      double left = (double) ((Rectangle) ref bounds2).get_Left();
      int width = (int) (x - left - 48.0);
      IntroPavement introPavement = new IntroPavement(position, width);
      introPavement.Depth = -10001;
      level.Add((Entity) introPavement);
      level.Add((Entity) new IntroCarBarrier(Vector2.op_Addition(this.Position, new Vector2(32f, 0.0f)), -10, Color.get_White()));
      level.Add((Entity) new IntroCarBarrier(Vector2.op_Addition(this.Position, new Vector2(41f, 0.0f)), 5, Color.get_DarkGray()));
    }

    public override void Update()
    {
      bool flag = this.HasRider();
      if ((double) this.Y > (double) this.startY && (!flag || (double) this.Y > (double) this.startY + 1.0))
        this.MoveV(-10f * Engine.DeltaTime);
      if ((((double) this.Y > (double) this.startY ? 0 : (!this.didHaveRider ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        this.MoveV(2f);
      if (this.didHaveRider && !flag)
        Audio.Play("event:/game/00_prologue/car_up", this.Position);
      this.didHaveRider = flag;
      base.Update();
    }

    public override int GetLandSoundIndex(Entity entity)
    {
      Audio.Play("event:/game/00_prologue/car_down", this.Position);
      return -1;
    }
  }
}
