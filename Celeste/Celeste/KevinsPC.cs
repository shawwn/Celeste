// Decompiled with JetBrains decompiler
// Type: Celeste.KevinsPC
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    public class KevinsPC : Actor
    {
        private Monocle.Image image;
        private MTexture spectogram;
        private MTexture subtex;
        private SoundSource sfx;
        private float timer;

        public KevinsPC(Vector2 position)
            : base(position)
        {
            this.Add((Component) (this.image = new Monocle.Image(GFX.Game["objects/kevinspc/pc"])));
            this.image.JustifyOrigin(0.5f, 1f);
            this.Depth = 8999;
            this.spectogram = GFX.Game["objects/kevinspc/spectogram"];
            this.subtex = this.spectogram.GetSubtexture(0, 0, 32, 18, this.subtex);
            this.Add((Component) (this.sfx = new SoundSource("event:/new_content/env/local/kevinpc")));
            this.sfx.Position = new Vector2(0.0f, -16f);
            this.timer = 0.0f;
        }

        public KevinsPC(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
        }

        public override bool IsRiding(Solid solid) => this.Scene.CollideCheck(new Rectangle((int) this.X - 4, (int) this.Y, 8, 2), (Entity) solid);

        public override void Update()
        {
            base.Update();
            this.timer += Engine.DeltaTime;
            int num = this.spectogram.Width - 32;
            this.subtex = this.spectogram.GetSubtexture((int) ((double) this.timer * ((double) num / 22.0) % (double) num), 0, 32, 18, this.subtex);
        }

        public override void Render()
        {
            base.Render();
            if (this.subtex == null)
                return;
            this.subtex.Draw(this.Position + new Vector2(-16f, -39f));
            Draw.Rect(this.X - 16f, this.Y - 39f, 32f, 18f, Color.Black * 0.25f);
        }
    }
}