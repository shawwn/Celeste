// Decompiled with JetBrains decompiler
// Type: Celeste.SummitGemManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class SummitGemManager : Entity
  {
    private List<SummitGemManager.Gem> gems = new List<SummitGemManager.Gem>();

    public SummitGemManager(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Depth = -10010;
      int index = 0;
      foreach (Vector2 position in data.NodesOffset(offset))
      {
        this.gems.Add(new SummitGemManager.Gem(index, position));
        ++index;
      }
      this.Add((Component) new Coroutine(this.Routine(), true));
    }

    public override void Awake(Scene scene)
    {
      foreach (SummitGemManager.Gem gem in this.gems)
        scene.Add((Entity) gem);
      base.Awake(scene);
    }

    private IEnumerator Routine()
    {
      SummitGemManager summitGemManager = this;
      Level level = summitGemManager.Scene as Level;
      if (level.Session.HeartGem)
      {
        foreach (SummitGemManager.Gem gem in summitGemManager.gems)
          gem.Sprite.RemoveSelf();
        summitGemManager.gems.Clear();
      }
      else
      {
        while (true)
        {
          Player entity = summitGemManager.Scene.Tracker.GetEntity<Player>();
          if (entity != null)
          {
            Vector2 vector2 = Vector2.op_Subtraction(entity.Position, summitGemManager.Position);
            if ((double) ((Vector2) ref vector2).Length() < 64.0)
              break;
          }
          yield return (object) null;
        }
        yield return (object) 0.5f;
        bool alreadyHasHeart = level.Session.OldStats.Modes[0].HeartGem;
        int broken = 0;
        int index = 0;
        List<SummitGemManager.Gem>.Enumerator enumerator = summitGemManager.gems.GetEnumerator();
        while (enumerator.MoveNext())
        {
          SummitGemManager.Gem gem = enumerator.Current;
          bool flag = level.Session.SummitGems[index];
          if (!alreadyHasHeart)
            flag = ((flag ? 1 : 0) | (SaveData.Instance.SummitGems == null ? 0 : (SaveData.Instance.SummitGems[index] ? 1 : 0))) != 0;
          if (flag)
          {
            switch (index)
            {
              case 0:
                Audio.Play("event:/game/07_summit/gem_unlock_1", gem.Position);
                break;
              case 1:
                Audio.Play("event:/game/07_summit/gem_unlock_2", gem.Position);
                break;
              case 2:
                Audio.Play("event:/game/07_summit/gem_unlock_3", gem.Position);
                break;
              case 3:
                Audio.Play("event:/game/07_summit/gem_unlock_4", gem.Position);
                break;
              case 4:
                Audio.Play("event:/game/07_summit/gem_unlock_5", gem.Position);
                break;
              case 5:
                Audio.Play("event:/game/07_summit/gem_unlock_6", gem.Position);
                break;
            }
            gem.Sprite.Play("spin", false, false);
            while (gem.Sprite.CurrentAnimationID == "spin")
            {
              gem.Bloom.Alpha = Calc.Approach(gem.Bloom.Alpha, 1f, Engine.DeltaTime * 3f);
              if ((double) gem.Bloom.Alpha > 0.5)
                gem.Shake = Calc.Random.ShakeVector();
              gem.Sprite.Y -= Engine.DeltaTime * 8f;
              gem.Sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + (double) gem.Bloom.Alpha * 0.100000001490116));
              yield return (object) null;
            }
            yield return (object) 0.2f;
            level.Shake(0.3f);
            Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
            for (int index1 = 0; index1 < 20; ++index1)
              level.ParticlesFG.Emit(SummitGem.P_Shatter, Vector2.op_Addition(gem.Position, new Vector2((float) Calc.Random.Range(-8, 8), (float) Calc.Random.Range(-8, 8))), SummitGem.GemColors[index], Calc.Random.NextFloat(6.283185f));
            ++broken;
            gem.Bloom.RemoveSelf();
            gem.Sprite.RemoveSelf();
            yield return (object) 0.25f;
          }
          ++index;
          gem = (SummitGemManager.Gem) null;
        }
        enumerator = new List<SummitGemManager.Gem>.Enumerator();
        if (broken >= 6)
        {
          HeartGem heart = summitGemManager.Scene.Entities.FindFirst<HeartGem>();
          if (heart != null)
          {
            Audio.Play("event:/game/07_summit/gem_unlock_complete", heart.Position);
            yield return (object) 0.1f;
            Vector2 from = heart.Position;
            for (float p = 0.0f; (double) p < 1.0 && heart.Scene != null; p += Engine.DeltaTime)
            {
              heart.Position = Vector2.Lerp(from, Vector2.op_Addition(summitGemManager.Position, new Vector2(0.0f, -16f)), Ease.CubeOut(p));
              yield return (object) null;
            }
            from = (Vector2) null;
          }
          heart = (HeartGem) null;
        }
      }
    }

    private class Gem : Entity
    {
      public Vector2 Shake;
      public Sprite Sprite;
      public Monocle.Image Bg;
      public BloomPoint Bloom;

      public Gem(int index, Vector2 position)
        : base(position)
      {
        this.Depth = -10010;
        this.Add((Component) (this.Bg = new Monocle.Image(GFX.Game["collectables/summitgems/" + (object) index + "/bg"])));
        this.Add((Component) (this.Sprite = new Sprite(GFX.Game, "collectables/summitgems/" + (object) index + "/gem")));
        this.Add((Component) (this.Bloom = new BloomPoint(0.0f, 20f)));
        this.Sprite.AddLoop("idle", "", 0.05f, new int[1]);
        this.Sprite.Add("spin", "", 0.05f, "idle");
        this.Sprite.Play("idle", false, false);
        this.Sprite.CenterOrigin();
        this.Bg.CenterOrigin();
      }

      public override void Update()
      {
        this.Bloom.Position = this.Sprite.Position;
        base.Update();
      }

      public override void Render()
      {
        Vector2 position = this.Sprite.Position;
        Sprite sprite = this.Sprite;
        sprite.Position = Vector2.op_Addition(sprite.Position, this.Shake);
        base.Render();
        this.Sprite.Position = position;
      }
    }
  }
}
