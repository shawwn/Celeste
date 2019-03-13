// Decompiled with JetBrains decompiler
// Type: Celeste.NPC03_Oshiro_Suite
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC03_Oshiro_Suite : NPC
  {
    private const string ConversationCounter = "oshiroSuiteSadConversation";

    public NPC03_Oshiro_Suite(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.Sprite = (Sprite) new OshiroSprite(1)));
      this.Add((Component) (this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64)));
      this.Add((Component) (this.Talker = new TalkComponent(new Rectangle(-16, -8, 32, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
      this.Talker.Enabled = false;
      this.MoveAnim = "move";
      this.IdleAnim = "idle";
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.Session.GetFlag("oshiro_resort_suite"))
      {
        this.Scene.Add((Entity) new CS03_OshiroMasterSuite((NPC) this));
      }
      else
      {
        this.Sprite.Play("idle_ground", false, false);
        this.Talker.Enabled = true;
      }
    }

    private void OnTalk(Player player)
    {
      this.Add((Component) new Coroutine(this.Talk(player), true));
    }

    private IEnumerator Talk(Player player)
    {
      int conversation = this.Session.GetCounter("oshiroSuiteSadConversation");
      yield return (object) this.PlayerApproach(player, false, new float?(12f), new int?());
      yield return (object) Textbox.Say("CH3_OSHIRO_SUITE_SAD" + (object) conversation);
      yield return (object) this.PlayerLeave(player, new float?());
      ++conversation;
      conversation %= 7;
      if (conversation == 0)
        ++conversation;
      this.Session.SetCounter("oshiroSuiteSadConversation", conversation);
    }
  }
}

