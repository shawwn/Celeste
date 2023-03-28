// Decompiled with JetBrains decompiler
// Type: Celeste.SlashFx
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
    [Tracked(false)]
    [Pooled]
    public class SlashFx : Entity
    {
        public Sprite Sprite;
        public Vector2 Direction;

        public SlashFx()
        {
            this.Add((Component) (this.Sprite = new Sprite(GFX.Game, "effects/slash/")));
            this.Sprite.Add("play", "", 0.1f, 0, 1, 2, 3);
            this.Sprite.CenterOrigin();
            this.Sprite.OnFinish = (Action<string>) (anim => this.RemoveSelf());
            this.Depth = -100;
        }

        public override void Update()
        {
            this.Position = this.Position + this.Direction * 8f * Engine.DeltaTime;
            base.Update();
        }

        public static SlashFx Burst(Vector2 position, float direction)
        {
            Scene scene = Engine.Scene;
            SlashFx slashFx1 = Engine.Pooler.Create<SlashFx>();
            SlashFx slashFx2 = slashFx1;
            scene.Add((Entity) slashFx2);
            slashFx1.Position = position;
            slashFx1.Direction = Calc.AngleToVector(direction, 1f);
            slashFx1.Sprite.Play("play", true);
            slashFx1.Sprite.Scale = Vector2.One;
            slashFx1.Sprite.Rotation = 0.0f;
            if ((double) Math.Abs(direction - 3.1415927f) > 0.009999999776482582)
                slashFx1.Sprite.Rotation = direction;
            slashFx1.Visible = slashFx1.Active = true;
            return slashFx1;
        }
    }
}