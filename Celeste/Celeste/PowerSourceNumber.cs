// Decompiled with JetBrains decompiler
// Type: Celeste.PowerSourceNumber
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    public class PowerSourceNumber : Entity
    {
        private readonly Monocle.Image image;
        private readonly Monocle.Image glow;
        private float ease;
        private float timer;
        private bool gotKey;

        public PowerSourceNumber(Vector2 position, int index, bool gotCollectables)
        {
            this.Position = position;
            this.Depth = -10010;
            this.Add((Component) (this.image = new Monocle.Image(GFX.Game["scenery/powersource_numbers/1"])));
            this.Add((Component) (this.glow = new Monocle.Image(GFX.Game["scenery/powersource_numbers/1_glow"])));
            this.glow.Color = Color.Transparent;
            this.gotKey = gotCollectables;
        }

        public override void Update()
        {
            base.Update();
            if (!(this.Scene as Level).Session.GetFlag("disable_lightning") || this.gotKey)
                return;
            this.timer += Engine.DeltaTime;
            this.ease = Calc.Approach(this.ease, 1f, Engine.DeltaTime * 4f);
            this.glow.Color = Color.White * this.ease * Calc.SineMap(this.timer * 2f, 0.5f, 0.9f);
        }
    }
}