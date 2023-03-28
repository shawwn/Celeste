// Decompiled with JetBrains decompiler
// Type: Celeste.LightningStrike
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class LightningStrike : Entity
  {
    private bool on;
    private float scale;
    private Random rand;
    private float strikeHeight;
    private LightningStrike.Node strike;

    public LightningStrike(Vector2 position, int seed, float height, float delay = 0.0f)
    {
      this.Position = position;
      this.Depth = 10010;
      this.rand = new Random(seed);
      this.strikeHeight = height;
      this.Add((Component) new Coroutine(this.Routine(delay)));
    }

    private IEnumerator Routine(float delay)
    {
      LightningStrike lightningStrike = this;
      if ((double) delay > 0.0)
        yield return (object) delay;
      lightningStrike.scale = 1f;
      lightningStrike.GenerateStikeNodes(-1, 10f);
      for (int j = 0; j < 5; ++j)
      {
        lightningStrike.on = true;
        yield return (object) (float) ((1.0 - (double) j / 5.0) * 0.10000000149011612);
        lightningStrike.scale -= 0.2f;
        lightningStrike.on = false;
        lightningStrike.strike.Wiggle(lightningStrike.rand);
        yield return (object) 0.01f;
      }
      lightningStrike.RemoveSelf();
    }

    private void GenerateStikeNodes(int direction, float size, LightningStrike.Node parent = null)
    {
      if (parent == null)
        parent = this.strike = new LightningStrike.Node(0.0f, 0.0f, size);
      if ((double) parent.Position.Y >= (double) this.strikeHeight)
        return;
      float x = (float) (direction * this.rand.Range(-8, 20));
      float y = (float) this.rand.Range(8, 16);
      float size1 = (float) (0.25 + (1.0 - ((double) parent.Position.Y + (double) y) / (double) this.strikeHeight) * 0.75) * size;
      LightningStrike.Node parent1 = new LightningStrike.Node(parent.Position + new Vector2(x, y), size1);
      parent.Children.Add(parent1);
      this.GenerateStikeNodes(direction, size, parent1);
      if (!this.rand.Chance(0.1f))
        return;
      LightningStrike.Node parent2 = new LightningStrike.Node(parent.Position + new Vector2(-x, y * 1.5f), size1);
      parent.Children.Add(parent2);
      this.GenerateStikeNodes(-direction, size, parent2);
    }

    public override void Render()
    {
      if (!this.on)
        return;
      this.strike.Render(this.Position, this.scale);
    }

    private class Node
    {
      public Vector2 Position;
      public float Size;
      public List<LightningStrike.Node> Children;

      public Node(float x, float y, float size)
        : this(new Vector2(x, y), size)
      {
      }

      public Node(Vector2 position, float size)
      {
        this.Position = position;
        this.Children = new List<LightningStrike.Node>();
        this.Size = size;
      }

      public void Wiggle(Random rand)
      {
        this.Position.X += (float) rand.Range(-2, 2);
        if ((double) this.Position.Y != 0.0)
          this.Position.Y += (float) rand.Range(-1, 1);
        foreach (LightningStrike.Node child in this.Children)
          child.Wiggle(rand);
      }

      public void Render(Vector2 offset, float scale)
      {
        float thickness = this.Size * scale;
        foreach (LightningStrike.Node child in this.Children)
        {
          Vector2 vector2 = (child.Position - this.Position).SafeNormalize();
          Draw.Line(offset + this.Position, offset + child.Position + vector2 * thickness * 0.5f, Color.White, thickness);
          child.Render(offset, scale);
        }
      }
    }
  }
}
