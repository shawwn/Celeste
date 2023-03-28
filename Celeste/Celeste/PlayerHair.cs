// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerHair
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class PlayerHair : Component
  {
    public const string Hair = "characters/player/hair00";
    public Color Color = Player.NormalHairColor;
    public Color Border = Color.Black;
    public float Alpha = 1f;
    public Facings Facing;
    public bool DrawPlayerSpriteOutline;
    public bool SimulateMotion = true;
    public Vector2 StepPerSegment = new Vector2(0.0f, 2f);
    public float StepInFacingPerSegment = 0.5f;
    public float StepApproach = 64f;
    public float StepYSinePerSegment;
    public PlayerSprite Sprite;
    public List<Vector2> Nodes = new List<Vector2>();
    private List<MTexture> bangs = GFX.Game.GetAtlasSubtextures("characters/player/bangs");
    private float wave;

    public PlayerHair(PlayerSprite sprite)
      : base(true, true)
    {
      this.Sprite = sprite;
      for (int index = 0; index < sprite.HairCount; ++index)
        this.Nodes.Add(Vector2.Zero);
    }

    public void Start()
    {
      Vector2 vector2 = this.Entity.Position + new Vector2((float) (-(int) this.Facing * 200), 200f);
      for (int index = 0; index < this.Nodes.Count; ++index)
        this.Nodes[index] = vector2;
    }

    public void AfterUpdate()
    {
      this.Nodes[0] = this.Sprite.RenderPosition + new Vector2(0.0f, -9f * this.Sprite.Scale.Y) + this.Sprite.HairOffset * new Vector2((float) this.Facing, 1f);
      Vector2 target = this.Nodes[0] + new Vector2((float) ((double) -(int) this.Facing * (double) this.StepInFacingPerSegment * 2.0), (float) Math.Sin((double) this.wave) * this.StepYSinePerSegment) + this.StepPerSegment;
      Vector2 node = this.Nodes[0];
      float num1 = 3f;
      for (int index = 1; index < this.Sprite.HairCount; ++index)
      {
        if (index >= this.Nodes.Count)
          this.Nodes.Add(this.Nodes[index - 1]);
        if (this.SimulateMotion)
        {
          float num2 = (float) (1.0 - (double) index / (double) this.Sprite.HairCount * 0.5) * this.StepApproach;
          this.Nodes[index] = Calc.Approach(this.Nodes[index], target, num2 * Engine.DeltaTime);
        }
        if ((double) (this.Nodes[index] - node).Length() > (double) num1)
          this.Nodes[index] = node + (this.Nodes[index] - node).SafeNormalize() * num1;
        target = this.Nodes[index] + new Vector2((float) -(int) this.Facing * this.StepInFacingPerSegment, (float) Math.Sin((double) this.wave + (double) index * 0.800000011920929) * this.StepYSinePerSegment) + this.StepPerSegment;
        node = this.Nodes[index];
      }
    }

    public override void Update()
    {
      this.wave += Engine.DeltaTime * 4f;
      base.Update();
    }

    public void MoveHairBy(Vector2 amount)
    {
      for (int index = 0; index < this.Nodes.Count; ++index)
        this.Nodes[index] += amount;
    }

    public override void Render()
    {
      if (!this.Sprite.HasHair)
        return;
      Vector2 origin = new Vector2(5f, 5f);
      Color color1 = this.Border * this.Alpha;
      Color color2 = this.Color * this.Alpha;
      if (this.DrawPlayerSpriteOutline)
      {
        Color color3 = this.Sprite.Color;
        Vector2 position = this.Sprite.Position;
        this.Sprite.Color = color1;
        this.Sprite.Position = position + new Vector2(0.0f, -1f);
        this.Sprite.Render();
        this.Sprite.Position = position + new Vector2(0.0f, 1f);
        this.Sprite.Render();
        this.Sprite.Position = position + new Vector2(-1f, 0.0f);
        this.Sprite.Render();
        this.Sprite.Position = position + new Vector2(1f, 0.0f);
        this.Sprite.Render();
        this.Sprite.Color = color3;
        this.Sprite.Position = position;
      }
      this.Nodes[0] = this.Nodes[0].Floor();
      if (color1.A > (byte) 0)
      {
        for (int index = 0; index < this.Sprite.HairCount; ++index)
        {
          int hairFrame = this.Sprite.HairFrame;
          MTexture mtexture = index == 0 ? this.bangs[hairFrame] : GFX.Game["characters/player/hair00"];
          Vector2 hairScale = this.GetHairScale(index);
          mtexture.Draw(this.Nodes[index] + new Vector2(-1f, 0.0f), origin, color1, hairScale);
          mtexture.Draw(this.Nodes[index] + new Vector2(1f, 0.0f), origin, color1, hairScale);
          mtexture.Draw(this.Nodes[index] + new Vector2(0.0f, -1f), origin, color1, hairScale);
          mtexture.Draw(this.Nodes[index] + new Vector2(0.0f, 1f), origin, color1, hairScale);
        }
      }
      for (int index = this.Sprite.HairCount - 1; index >= 0; --index)
      {
        int hairFrame = this.Sprite.HairFrame;
        (index == 0 ? this.bangs[hairFrame] : GFX.Game["characters/player/hair00"]).Draw(this.Nodes[index], origin, color2, this.GetHairScale(index));
      }
    }

    private Vector2 GetHairScale(int index)
    {
      float y = (float) (0.25 + (1.0 - (double) index / (double) this.Sprite.HairCount) * 0.75);
      return new Vector2((index == 0 ? (float) this.Facing : y) * Math.Abs(this.Sprite.Scale.X), y);
    }
  }
}
