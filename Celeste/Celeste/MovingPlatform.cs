// Decompiled with JetBrains decompiler
// Type: Celeste.MovingPlatform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MovingPlatform : JumpThru
  {
    private Vector2 start;
    private Vector2 end;
    private float addY;
    private float sinkTimer;
    private MTexture[] textures;
    private string lastSfx;
    private SoundSource sfx;

    public MovingPlatform(Vector2 position, int width, Vector2 node)
      : base(position, width, false)
    {
      this.start = this.Position;
      this.end = node;
      this.Add((Component) (this.sfx = new SoundSource()));
      this.SurfaceSoundIndex = 5;
      this.lastSfx = Math.Sign(this.start.X - this.end.X) > 0 || Math.Sign(this.start.Y - this.end.Y) > 0 ? "event:/game/03_resort/platform_horiz_left" : "event:/game/03_resort/platform_horiz_right";
      Tween tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.SineInOut, 2f);
      tween.OnUpdate = (Action<Tween>) (t => this.MoveTo(Vector2.Lerp(this.start, this.end, t.Eased) + Vector2.UnitY * this.addY));
      tween.OnStart = (Action<Tween>) (t =>
      {
        if (this.lastSfx == "event:/game/03_resort/platform_horiz_left")
          this.sfx.Play(this.lastSfx = "event:/game/03_resort/platform_horiz_right");
        else
          this.sfx.Play(this.lastSfx = "event:/game/03_resort/platform_horiz_left");
      });
      this.Add((Component) tween);
      tween.Start(false);
      this.Add((Component) new LightOcclude(0.2f));
    }

    public MovingPlatform(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Nodes[0] + offset)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Session session = this.SceneAs<Level>().Session;
      MTexture mtexture = session.Area.ID != 7 || !session.Level.StartsWith("e-") ? GFX.Game["objects/woodPlatform/" + AreaData.Get(scene).WoodPlatform] : GFX.Game["objects/woodPlatform/" + AreaData.Get(4).WoodPlatform];
      this.textures = new MTexture[mtexture.Width / 8];
      for (int index = 0; index < this.textures.Length; ++index)
        this.textures[index] = mtexture.GetSubtexture(index * 8, 0, 8, 8);
      Vector2 vector2 = new Vector2(this.Width, this.Height + 4f) / 2f;
      scene.Add((Entity) new MovingPlatformLine(this.start + vector2, this.end + vector2));
    }

    public override void Render()
    {
      this.textures[0].Draw(this.Position);
      for (int x = 8; (double) x < (double) this.Width - 8.0; x += 8)
        this.textures[1].Draw(this.Position + new Vector2((float) x, 0.0f));
      this.textures[3].Draw(this.Position + new Vector2(this.Width - 8f, 0.0f));
      this.textures[2].Draw(this.Position + new Vector2((float) ((double) this.Width / 2.0 - 4.0), 0.0f));
    }

    public override void OnStaticMoverTrigger(StaticMover sm) => this.sinkTimer = 0.4f;

    public override void Update()
    {
      base.Update();
      if (this.HasPlayerRider())
      {
        this.sinkTimer = 0.2f;
        this.addY = Calc.Approach(this.addY, 3f, 50f * Engine.DeltaTime);
      }
      else if ((double) this.sinkTimer > 0.0)
      {
        this.sinkTimer -= Engine.DeltaTime;
        this.addY = Calc.Approach(this.addY, 3f, 50f * Engine.DeltaTime);
      }
      else
        this.addY = Calc.Approach(this.addY, 0.0f, 20f * Engine.DeltaTime);
    }
  }
}
