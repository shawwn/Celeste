// Decompiled with JetBrains decompiler
// Type: Celeste.BlackholeStrengthTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
    public class BlackholeStrengthTrigger : Trigger
    {
        private BlackholeBG.Strengths strength;

        public BlackholeStrengthTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            this.strength = data.Enum<BlackholeBG.Strengths>(nameof (strength));
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            (this.Scene as Level).Background.Get<BlackholeBG>()?.NextStrength(this.Scene as Level, this.strength);
        }
    }
}