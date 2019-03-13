// Decompiled with JetBrains decompiler
// Type: Celeste.TemplePortalTorch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class TemplePortalTorch : Entity
  {
    private Sprite sprite;
    private VertexLight light;
    private BloomPoint bloom;
    private SoundSource loopSfx;

    public TemplePortalTorch(Vector2 pos)
      : base(pos)
    {
      this.Add((Component) (this.sprite = new Sprite(GFX.Game, "objects/temple/portal/portaltorch")));
      this.sprite.AddLoop("idle", "", 0.0f, new int[1]);
      this.sprite.AddLoop("lit", "", 0.08f, 1, 2, 3, 4, 5, 6);
      this.sprite.Play("idle", false, false);
      this.sprite.Origin = new Vector2(32f, 64f);
      this.Depth = 8999;
    }

    public void Light(int count = 0)
    {
      this.sprite.Play("lit", false, false);
      this.Add((Component) (this.bloom = new BloomPoint(1f, 16f)));
      this.Add((Component) (this.light = new VertexLight(Color.LightSeaGreen, 0.0f, 32, 128)));
      Audio.Play(count == 0 ? "event:/game/05_mirror_temple/mainmirror_torch_lit_1" : "event:/game/05_mirror_temple/mainmirror_torch_lit_2", this.Position);
      this.Add((Component) (this.loopSfx = new SoundSource()));
      this.loopSfx.Play("event:/game/05_mirror_temple/mainmirror_torch_loop", (string) null, 0.0f);
    }

    public override void Update()
    {
      base.Update();
      if (this.bloom != null && (double) this.bloom.Alpha > 0.5)
        this.bloom.Alpha -= Engine.DeltaTime;
      if (this.light == null || (double) this.light.Alpha >= 1.0)
        return;
      this.light.Alpha = Calc.Approach(this.light.Alpha, 1f, Engine.DeltaTime);
    }
  }
}

