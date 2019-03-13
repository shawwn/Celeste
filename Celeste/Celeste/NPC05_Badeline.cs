// Decompiled with JetBrains decompiler
// Type: Celeste.NPC05_Badeline
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC05_Badeline : NPC
  {
    public const string FirstLevel = "c-00";
    public const string SecondLevel = "c-01";
    public const string ThirdLevel = "c-01b";
    private BadelineDummy shadow;
    private Vector2[] nodes;
    private Rectangle levelBounds;
    private SoundSource moveSfx;

    public NPC05_Badeline(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.nodes = data.NodesOffset(offset);
      this.Add((Component) (this.moveSfx = new SoundSource()));
      this.Add((Component) new TransitionListener()
      {
        OnOut = (Action<float>) (f =>
        {
          if (this.shadow == null)
            return;
          this.shadow.Hair.Alpha = 1f - Math.Min(1f, f * 2f);
          this.shadow.Sprite.Color = Color.op_Multiply(Color.get_White(), this.shadow.Hair.Alpha);
          this.shadow.Light.Alpha = this.shadow.Hair.Alpha;
        })
      });
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.Session.Level.Equals("c-00"))
      {
        if (!this.Session.GetLevelFlag("c-01"))
        {
          scene.Add((Entity) (this.shadow = new BadelineDummy(this.Position)));
          this.shadow.Depth = -1000000;
          this.Add((Component) new Coroutine(this.FirstScene(), true));
        }
        else
          this.RemoveSelf();
      }
      else if (this.Session.Level.Equals("c-01"))
      {
        if (!this.Session.GetLevelFlag("c-01b"))
        {
          int num = 0;
          while (num < 4 && this.Session.GetFlag(CS05_Badeline.GetFlag(num)))
            ++num;
          if (num >= 4)
          {
            this.RemoveSelf();
          }
          else
          {
            Vector2 position = this.Position;
            if (num > 0)
              position = this.nodes[num - 1];
            scene.Add((Entity) (this.shadow = new BadelineDummy(position)));
            this.shadow.Depth = -1000000;
            this.Add((Component) new Coroutine(this.SecondScene(num), true));
          }
        }
        else
          this.RemoveSelf();
      }
      this.levelBounds = (scene as Level).Bounds;
    }

    private IEnumerator FirstScene()
    {
      NPC05_Badeline npC05Badeline = this;
      npC05Badeline.shadow.Sprite.Scale.X = (__Null) -1.0;
      npC05Badeline.shadow.FloatSpeed = 150f;
      bool playerHasFallen = false;
      bool startedMusic = false;
      Player player = (Player) null;
      Rectangle bounds;
      while (true)
      {
        player = npC05Badeline.Scene.Tracker.GetEntity<Player>();
        if (player != null)
        {
          double y = (double) player.Y;
          bounds = npC05Badeline.Level.Bounds;
          double num = (double) (((Rectangle) ref bounds).get_Top() + 180);
          if (y > num && !player.OnGround(1) && !playerHasFallen)
          {
            player.StateMachine.State = 20;
            playerHasFallen = true;
          }
        }
        if (player != null & playerHasFallen && !startedMusic && player.OnGround(1))
        {
          npC05Badeline.Level.Session.Audio.Music.Event = "event:/music/lvl5/middle_temple";
          npC05Badeline.Level.Session.Audio.Apply();
          startedMusic = true;
        }
        if (player == null || (double) player.X <= (double) npC05Badeline.X - 64.0 || (double) player.Y <= (double) npC05Badeline.Y - 32.0)
          yield return (object) null;
        else
          break;
      }
      npC05Badeline.MoveToNode(0, false);
      while (true)
      {
        do
        {
          double x = (double) npC05Badeline.shadow.X;
          bounds = npC05Badeline.Level.Bounds;
          double num = (double) (((Rectangle) ref bounds).get_Right() + 8);
          if (x < num)
            yield return (object) null;
          else
            goto label_12;
        }
        while ((double) player.X <= (double) npC05Badeline.shadow.X - 24.0);
        npC05Badeline.shadow.X = player.X + 24f;
      }
label_12:
      npC05Badeline.Scene.Remove((Entity) npC05Badeline.shadow);
      npC05Badeline.RemoveSelf();
    }

    private IEnumerator SecondScene(int startIndex)
    {
      NPC05_Badeline npc = this;
      npc.shadow.Sprite.Scale.X = (__Null) -1.0;
      npc.shadow.FloatSpeed = 300f;
      npc.shadow.FloatAccel = 400f;
      yield return (object) 0.1f;
      int index = startIndex;
      while (index < npc.nodes.Length)
      {
        Player player = npc.Scene.Tracker.GetEntity<Player>();
        while (true)
        {
          if (player != null)
            goto label_4;
label_2:
          yield return (object) null;
          continue;
label_4:
          Vector2 vector2 = Vector2.op_Subtraction(player.Position, npc.shadow.Position);
          if ((double) ((Vector2) ref vector2).Length() > 70.0)
            goto label_2;
          else
            break;
        }
        if (index < 4 && !npc.Session.GetFlag(CS05_Badeline.GetFlag(index)))
        {
          CS05_Badeline cutscene = new CS05_Badeline(player, npc, npc.shadow, index);
          npc.Scene.Add((Entity) cutscene);
          yield return (object) null;
          while (cutscene.Scene != null)
            yield return (object) null;
          ++index;
          cutscene = (CS05_Badeline) null;
        }
        player = (Player) null;
      }
      npc.Tag |= (int) Tags.TransitionUpdate;
      npc.shadow.Tag |= (int) Tags.TransitionUpdate;
      npc.Scene.Remove((Entity) npc.shadow);
      npc.RemoveSelf();
    }

    public void MoveToNode(int index, bool chatMove = true)
    {
      if (chatMove)
        this.moveSfx.Play("event:/char/badeline/temple_move_chats", (string) null, 0.0f);
      else
        SoundEmitter.Play("event:/char/badeline/temple_move_first", (Entity) this, new Vector2?());
      Vector2 start = this.shadow.Position;
      Vector2 end = this.nodes[index];
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.5f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.shadow.Position = Vector2.Lerp(start, end, t.Eased);
        if (this.Scene.OnInterval(0.03f))
          this.SceneAs<Level>().ParticlesFG.Emit(BadelineOldsite.P_Vanish, 2, Vector2.op_Addition(this.shadow.Position, new Vector2(0.0f, -6f)), Vector2.op_Multiply(Vector2.get_One(), 2f));
        if ((double) t.Eased < 0.100000001490116 || (double) t.Eased > 0.899999976158142 || !this.Scene.OnInterval(0.05f))
          return;
        TrailManager.Add((Entity) this.shadow, Color.get_Red(), 0.5f);
      });
      this.Add((Component) tween);
    }

    public void SnapToNode(int index)
    {
      this.shadow.Position = this.nodes[index];
    }
  }
}
