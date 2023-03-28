// Decompiled with JetBrains decompiler
// Type: Celeste.StarTrackSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    public class StarTrackSpinner : TrackSpinner
    {
        public static ParticleType[] P_Trail;
        public Sprite Sprite;
        private bool hasStarted;
        private int colorID;
        private bool trail;

        public StarTrackSpinner(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("moonBlade")));
            this.colorID = Calc.Random.Choose<int>(0, 1, 2);
            this.Sprite.Play("idle" + (object) this.colorID);
            this.Depth = -50;
            this.Add((Component) new MirrorReflection());
        }

        public override void Update()
        {
            base.Update();
            if (!this.trail || !this.Scene.OnInterval(0.03f))
                return;
            this.SceneAs<Level>().ParticlesBG.Emit(StarTrackSpinner.P_Trail[this.colorID], 1, this.Position, Vector2.One * 3f);
        }

        public override void OnTrackStart()
        {
            ++this.colorID;
            this.colorID %= 3;
            this.Sprite.Play("spin" + (object) this.colorID);
            if (this.hasStarted)
                Audio.Play("event:/game/05_mirror_temple/bladespinner_spin", this.Position);
            this.hasStarted = true;
            this.trail = true;
        }

        public override void OnTrackEnd() => this.trail = false;
    }
}