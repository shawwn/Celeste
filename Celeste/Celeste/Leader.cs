﻿// Decompiled with JetBrains decompiler
// Type: Celeste.Leader
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Leader : Component
  {
    public List<Follower> Followers = new List<Follower>();
    public List<Vector2> PastPoints = new List<Vector2>();
    public const int MaxPastPoints = 350;
    public Vector2 Position;
    private static List<Strawberry> storedBerries;
    private static List<Vector2> storedOffsets;

    public Leader()
      : base(true, false)
    {
    }

    public Leader(Vector2 position)
      : base(true, false)
    {
      this.Position = position;
    }

    public void GainFollower(Follower follower)
    {
      this.Followers.Add(follower);
      follower.OnGainLeaderUtil(this);
    }

    public void LoseFollower(Follower follower)
    {
      this.Followers.Remove(follower);
      follower.OnLoseLeaderUtil();
    }

    public void LoseFollowers()
    {
      foreach (Follower follower in this.Followers)
        follower.OnLoseLeaderUtil();
      this.Followers.Clear();
    }

    public override void Update()
    {
      Vector2 vector2_1 = Vector2.op_Addition(this.Entity.Position, this.Position);
      if (this.Scene.OnInterval(0.02f))
      {
        if (this.PastPoints.Count != 0)
        {
          Vector2 vector2_2 = Vector2.op_Subtraction(vector2_1, this.PastPoints[0]);
          if ((double) ((Vector2) ref vector2_2).Length() < 3.0)
            goto label_5;
        }
        this.PastPoints.Insert(0, vector2_1);
        if (this.PastPoints.Count > 350)
          this.PastPoints.RemoveAt(this.PastPoints.Count - 1);
      }
label_5:
      int index = 5;
      foreach (Follower follower in this.Followers)
      {
        if (index >= this.PastPoints.Count)
          break;
        Vector2 pastPoint = this.PastPoints[index];
        if ((double) follower.DelayTimer <= 0.0 && follower.MoveTowardsLeader)
          follower.Entity.Position = Vector2.op_Addition(follower.Entity.Position, Vector2.op_Multiply(Vector2.op_Subtraction(pastPoint, follower.Entity.Position), 1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
        index += 5;
      }
    }

    public bool HasFollower<T>()
    {
      foreach (Component follower in this.Followers)
      {
        if (follower.Entity is T)
          return true;
      }
      return false;
    }

    public void TransferFollowers()
    {
      for (int index = 0; index < this.Followers.Count; ++index)
      {
        Follower follower = this.Followers[index];
        if (!follower.Entity.TagCheck((int) Tags.Persistent))
        {
          this.LoseFollower(follower);
          --index;
        }
      }
    }

    public static void StoreStrawberries(Leader leader)
    {
      Leader.storedBerries = new List<Strawberry>();
      Leader.storedOffsets = new List<Vector2>();
      foreach (Follower follower in leader.Followers)
      {
        if (follower.Entity is Strawberry)
        {
          Leader.storedBerries.Add(follower.Entity as Strawberry);
          Leader.storedOffsets.Add(Vector2.op_Subtraction(follower.Entity.Position, leader.Entity.Position));
        }
      }
      foreach (Strawberry storedBerry in Leader.storedBerries)
      {
        leader.Followers.Remove(storedBerry.Follower);
        storedBerry.Follower.Leader = (Leader) null;
        storedBerry.AddTag((int) Tags.Global);
      }
    }

    public static void RestoreStrawberries(Leader leader)
    {
      for (int index = 0; index < Leader.storedBerries.Count; ++index)
      {
        Strawberry storedBerry = Leader.storedBerries[index];
        leader.GainFollower(storedBerry.Follower);
        storedBerry.Position = Vector2.op_Addition(leader.Entity.Position, Leader.storedOffsets[index]);
        storedBerry.RemoveTag((int) Tags.Global);
      }
    }
  }
}
