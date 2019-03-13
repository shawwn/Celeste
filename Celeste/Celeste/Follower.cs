// Decompiled with JetBrains decompiler
// Type: Celeste.Follower
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;

namespace Celeste
{
  public class Follower : Component
  {
    public bool PersistentFollow = true;
    public float FollowDelay = 0.5f;
    public bool MoveTowardsLeader = true;
    public EntityID ParentEntityID;
    public Leader Leader;
    public Action OnGainLeader;
    public Action OnLoseLeader;
    public float DelayTimer;

    public bool HasLeader
    {
      get
      {
        return this.Leader != null;
      }
    }

    public Follower(Action onGainLeader = null, Action onLoseLeader = null)
      : base(true, false)
    {
      this.OnGainLeader = onGainLeader;
      this.OnLoseLeader = onLoseLeader;
    }

    public Follower(EntityID entityID, Action onGainLeader = null, Action onLoseLeader = null)
      : base(true, false)
    {
      this.ParentEntityID = entityID;
      this.OnGainLeader = onGainLeader;
      this.OnLoseLeader = onLoseLeader;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.DelayTimer <= 0.0)
        return;
      this.DelayTimer -= Engine.DeltaTime;
    }

    public void OnLoseLeaderUtil()
    {
      if (this.PersistentFollow)
        this.Entity.RemoveTag((int) Tags.Persistent);
      this.Leader = (Leader) null;
      if (this.OnLoseLeader == null)
        return;
      this.OnLoseLeader();
    }

    public void OnGainLeaderUtil(Leader leader)
    {
      if (this.PersistentFollow)
        this.Entity.AddTag((int) Tags.Persistent);
      this.Leader = leader;
      this.DelayTimer = this.FollowDelay;
      if (this.OnGainLeader == null)
        return;
      this.OnGainLeader();
    }

    public int FollowIndex
    {
      get
      {
        if (this.Leader == null)
          return -1;
        return this.Leader.Followers.IndexOf(this);
      }
    }
  }
}
