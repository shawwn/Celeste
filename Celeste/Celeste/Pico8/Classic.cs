// Decompiled with JetBrains decompiler
// Type: Celeste.Pico8.Classic
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Celeste.Pico8
{
  public class Classic
  {
    private static int k_left = 0;
    private static int k_right = 1;
    private static int k_up = 2;
    private static int k_down = 3;
    private static int k_jump = 4;
    private static int k_dash = 5;
    public static Emulator E;
    private static Point room;
    private static List<Classic.ClassicObject> objects;
    public static int freeze;
    private static int shake;
    private static bool will_restart;
    private static int delay_restart;
    private static HashSet<int> got_fruit;
    private static bool has_dashed;
    private static int sfx_timer;
    private static bool has_key;
    private static bool pause_player;
    private static bool flash_bg;
    private static int music_timer;
    private static bool new_bg;
    private static int frames;
    private static int seconds;
    private static int minutes;
    private static int deaths;
    private static int max_djump;
    private static bool start_game;
    private static int start_game_flash;
    private static List<Classic.Cloud> clouds;
    private static List<Classic.Particle> particles;
    private static List<Classic.DeadParticle> dead_particles;

    public static void Init(Emulator emulator)
    {
      Classic.E = emulator;
      Classic.room = new Point(0, 0);
      Classic.objects = new List<Classic.ClassicObject>();
      Classic.freeze = 0;
      Classic.will_restart = false;
      Classic.delay_restart = 0;
      Classic.got_fruit = new HashSet<int>();
      Classic.has_dashed = false;
      Classic.sfx_timer = 0;
      Classic.has_key = false;
      Classic.pause_player = false;
      Classic.flash_bg = false;
      Classic.music_timer = 0;
      Classic.new_bg = false;
      Classic.frames = 0;
      Classic.seconds = 0;
      Classic.minutes = 0;
      Classic.deaths = 0;
      Classic.max_djump = 1;
      Classic.start_game = false;
      Classic.start_game_flash = 0;
      Classic.clouds = new List<Classic.Cloud>();
      for (int index = 0; index <= 16; ++index)
        Classic.clouds.Add(new Classic.Cloud()
        {
          x = Classic.E.rnd(128f),
          y = Classic.E.rnd(128f),
          spd = 1f + Classic.E.rnd(4f),
          w = 32f + Classic.E.rnd(32f)
        });
      Classic.particles = new List<Classic.Particle>();
      for (int index = 0; index <= 32; ++index)
        Classic.particles.Add(new Classic.Particle()
        {
          x = Classic.E.rnd(128f),
          y = Classic.E.rnd(128f),
          s = Classic.E.flr(Classic.E.rnd(5f) / 4f),
          spd = 0.25f + Classic.E.rnd(5f),
          off = Classic.E.rnd(1f),
          c = 6 + Classic.E.flr(0.5f + Classic.E.rnd(1f))
        });
      Classic.dead_particles = new List<Classic.DeadParticle>();
      Classic.title_screen();
    }

    private static void title_screen()
    {
      Classic.got_fruit = new HashSet<int>();
      Classic.frames = 0;
      Classic.deaths = 0;
      Classic.max_djump = 1;
      Classic.start_game = false;
      Classic.start_game_flash = 0;
      Classic.E.music(40, 0, 7);
      Classic.load_room(7, 3);
    }

    private static void begin_game()
    {
      Classic.frames = 0;
      Classic.seconds = 0;
      Classic.minutes = 0;
      Classic.music_timer = 0;
      Classic.start_game = false;
      Classic.E.music(0, 0, 7);
      Classic.load_room(0, 0);
    }

    private static int level_index()
    {
      return Classic.room.X % 8 + Classic.room.Y * 8;
    }

    private static bool is_title()
    {
      return Classic.level_index() == 31;
    }

    private static void psfx(int num)
    {
      if (Classic.sfx_timer > 0)
        return;
      Classic.E.sfx(num);
    }

    private static void draw_player(Classic.ClassicObject obj, int djump)
    {
      int num = 0;
      switch (djump)
      {
        case 0:
          num = 128;
          break;
        case 2:
          num = Classic.E.flr((float) (Classic.frames / 3 % 2)) != 0 ? 144 : 160;
          break;
      }
      Classic.E.spr(obj.spr + (float) num, obj.x, obj.y, 1, 1, obj.flipX, obj.flipY);
    }

    private static void break_spring(Classic.spring obj)
    {
      obj.hide_in = 15;
    }

    private static void break_fall_floor(Classic.fall_floor obj)
    {
      if (obj.state != 0)
        return;
      Classic.psfx(15);
      obj.state = 1;
      obj.delay = 15;
      Classic.init_object<Classic.smoke>(new Classic.smoke(), obj.x, obj.y, new int?());
      Classic.spring spring = obj.collide<Classic.spring>(0, -1);
      if (spring == null)
        return;
      Classic.break_spring(spring);
    }

    private static T init_object<T>(T obj, float x, float y, int? tile = null) where T : Classic.ClassicObject
    {
      Classic.objects.Add((Classic.ClassicObject) obj);
      if (tile.HasValue)
        obj.spr = (float) tile.Value;
      obj.x = (float) (int) x;
      obj.y = (float) (int) y;
      obj.init();
      return obj;
    }

    private static void destroy_object(Classic.ClassicObject obj)
    {
      int index = Classic.objects.IndexOf(obj);
      if (index < 0)
        return;
      Classic.objects[index] = (Classic.ClassicObject) null;
    }

    private static void kill_player(Classic.player obj)
    {
      Classic.sfx_timer = 12;
      Classic.E.sfx(0);
      ++Classic.deaths;
      Classic.shake = 10;
      Classic.destroy_object((Classic.ClassicObject) obj);
      Stats.Increment(Stat.PICO_DEATHS, 1);
      Classic.dead_particles.Clear();
      for (int index = 0; index <= 7; ++index)
      {
        float a = (float) index / 8f;
        Classic.dead_particles.Add(new Classic.DeadParticle()
        {
          x = obj.x + 4f,
          y = obj.y + 4f,
          t = 10,
          spd = new Vector2(Classic.E.cos(a) * 3f, Classic.E.sin(a + 0.5f) * 3f)
        });
      }
      Classic.restart_room();
    }

    private static void restart_room()
    {
      Classic.will_restart = true;
      Classic.delay_restart = 15;
    }

    private static void next_room()
    {
      if (Classic.room.X == 2 && Classic.room.Y == 1)
        Classic.E.music(30, 500, 7);
      else if (Classic.room.X == 3 && Classic.room.Y == 1)
        Classic.E.music(20, 500, 7);
      else if (Classic.room.X == 4 && Classic.room.Y == 2)
        Classic.E.music(30, 500, 7);
      else if (Classic.room.X == 5 && Classic.room.Y == 3)
        Classic.E.music(30, 500, 7);
      if (Classic.room.X == 7)
        Classic.load_room(0, Classic.room.Y + 1);
      else
        Classic.load_room(Classic.room.X + 1, (int) Classic.room.Y);
    }

    public static void load_room(int x, int y)
    {
      Classic.has_dashed = false;
      Classic.has_key = false;
      for (int index = 0; index < Classic.objects.Count; ++index)
        Classic.objects[index] = (Classic.ClassicObject) null;
      Classic.room.X = (__Null) x;
      Classic.room.Y = (__Null) y;
      for (int index1 = 0; index1 <= 15; ++index1)
      {
        for (int index2 = 0; index2 <= 15; ++index2)
        {
          int num = Classic.E.mget(Classic.room.X * 16 + index1, Classic.room.Y * 16 + index2);
          switch (num)
          {
            case 11:
              Classic.init_object<Classic.platform>(new Classic.platform(), (float) (index1 * 8), (float) (index2 * 8), new int?()).dir = -1f;
              break;
            case 12:
              Classic.init_object<Classic.platform>(new Classic.platform(), (float) (index1 * 8), (float) (index2 * 8), new int?()).dir = 1f;
              break;
            default:
              Classic.ClassicObject classicObject = (Classic.ClassicObject) null;
              if (num == 1)
                classicObject = (Classic.ClassicObject) new Classic.player_spawn();
              else if (num == 18)
                classicObject = (Classic.ClassicObject) new Classic.spring();
              else if (num == 22)
                classicObject = (Classic.ClassicObject) new Classic.balloon();
              else if (num == 23)
                classicObject = (Classic.ClassicObject) new Classic.fall_floor();
              else if (num == 86)
                classicObject = (Classic.ClassicObject) new Classic.message();
              else if (num == 96)
                classicObject = (Classic.ClassicObject) new Classic.big_chest();
              else if (num == 118)
                classicObject = (Classic.ClassicObject) new Classic.flag();
              else if (!Classic.got_fruit.Contains(1 + Classic.level_index()))
              {
                switch (num)
                {
                  case 8:
                    classicObject = (Classic.ClassicObject) new Classic.key();
                    break;
                  case 20:
                    classicObject = (Classic.ClassicObject) new Classic.chest();
                    break;
                  case 26:
                    classicObject = (Classic.ClassicObject) new Classic.fruit();
                    break;
                  case 28:
                    classicObject = (Classic.ClassicObject) new Classic.fly_fruit();
                    break;
                  case 64:
                    classicObject = (Classic.ClassicObject) new Classic.fake_wall();
                    break;
                }
              }
              if (classicObject != null)
              {
                Classic.init_object<Classic.ClassicObject>(classicObject, (float) (index1 * 8), (float) (index2 * 8), new int?(num));
                break;
              }
              break;
          }
        }
      }
      if (Classic.is_title())
        return;
      Classic.init_object<Classic.room_title>(new Classic.room_title(), 0.0f, 0.0f, new int?());
    }

    public static void Update()
    {
      Classic.frames = (Classic.frames + 1) % 30;
      if (Classic.frames == 0 && Classic.level_index() < 30)
      {
        Classic.seconds = (Classic.seconds + 1) % 60;
        if (Classic.seconds == 0)
          ++Classic.minutes;
      }
      if (Classic.music_timer > 0)
      {
        --Classic.music_timer;
        if (Classic.music_timer <= 0)
          Classic.E.music(10, 0, 7);
      }
      if (Classic.sfx_timer > 0)
        --Classic.sfx_timer;
      if (Classic.freeze > 0)
      {
        --Classic.freeze;
      }
      else
      {
        if (Classic.shake > 0 && !Settings.Instance.DisableScreenShake)
        {
          --Classic.shake;
          Classic.E.camera();
          if (Classic.shake > 0)
            Classic.E.camera(Classic.E.rnd(5f) - 2f, Classic.E.rnd(5f) - 2f);
        }
        if (Classic.will_restart && Classic.delay_restart > 0)
        {
          --Classic.delay_restart;
          if (Classic.delay_restart <= 0)
          {
            Classic.will_restart = true;
            Classic.load_room((int) Classic.room.X, (int) Classic.room.Y);
          }
        }
        int count = Classic.objects.Count;
        for (int index = 0; index < count; ++index)
        {
          Classic.ClassicObject classicObject = Classic.objects[index];
          if (classicObject != null)
          {
            classicObject.move((float) classicObject.spd.X, (float) classicObject.spd.Y);
            classicObject.update();
          }
        }
        while (Classic.objects.IndexOf((Classic.ClassicObject) null) >= 0)
          Classic.objects.Remove((Classic.ClassicObject) null);
        if (!Classic.is_title())
          return;
        if (!Classic.start_game && (Classic.E.btn(Classic.k_jump) || Classic.E.btn(Classic.k_dash)))
        {
          Classic.E.music(-1, 0, 0);
          Classic.start_game_flash = 50;
          Classic.start_game = true;
          Classic.E.sfx(38);
        }
        if (!Classic.start_game)
          return;
        --Classic.start_game_flash;
        if (Classic.start_game_flash > -30)
          return;
        Classic.begin_game();
      }
    }

    public static void Draw()
    {
      Classic.E.pal();
      if (Classic.start_game)
      {
        int b = 10;
        if (Classic.start_game_flash > 10)
        {
          if (Classic.frames % 10 < 5)
            b = 7;
        }
        else
          b = Classic.start_game_flash <= 5 ? (Classic.start_game_flash <= 0 ? 0 : 1) : 2;
        if (b < 10)
        {
          Classic.E.pal(6, b);
          Classic.E.pal(12, b);
          Classic.E.pal(13, b);
          Classic.E.pal(5, b);
          Classic.E.pal(1, b);
          Classic.E.pal(7, b);
        }
      }
      int num = 0;
      if (Classic.flash_bg)
        num = Classic.frames / 5;
      else if (Classic.new_bg)
        num = 2;
      Classic.E.rectfill(0.0f, 0.0f, 128f, 128f, (float) num);
      if (!Classic.is_title())
      {
        foreach (Classic.Cloud cloud in Classic.clouds)
        {
          cloud.x += cloud.spd;
          Classic.E.rectfill(cloud.x, cloud.y, cloud.x + cloud.w, (float) ((double) cloud.y + 4.0 + (1.0 - (double) cloud.w / 64.0) * 12.0), Classic.new_bg ? 14f : 1f);
          if ((double) cloud.x > 128.0)
          {
            cloud.x = -cloud.w;
            cloud.y = Classic.E.rnd(120f);
          }
        }
      }
      Classic.E.map(Classic.room.X * 16, Classic.room.Y * 16, 0, 0, 16, 16, 2);
      for (int index = 0; index < Classic.objects.Count; ++index)
      {
        Classic.ClassicObject classicObject = Classic.objects[index];
        if (classicObject != null && (classicObject is Classic.platform || classicObject is Classic.big_chest))
          Classic.draw_object(classicObject);
      }
      int tx = Classic.is_title() ? -4 : 0;
      Classic.E.map(Classic.room.X * 16, Classic.room.Y * 16, tx, 0, 16, 16, 1);
      for (int index = 0; index < Classic.objects.Count; ++index)
      {
        Classic.ClassicObject classicObject = Classic.objects[index];
        if (classicObject != null && !(classicObject is Classic.platform) && !(classicObject is Classic.big_chest))
          Classic.draw_object(classicObject);
      }
      Classic.E.map(Classic.room.X * 16, Classic.room.Y * 16, 0, 0, 16, 16, 3);
      foreach (Classic.Particle particle in Classic.particles)
      {
        particle.x += particle.spd;
        particle.y += Classic.E.sin(particle.off);
        particle.off += Classic.E.min(0.05f, particle.spd / 32f);
        Classic.E.rectfill(particle.x, particle.y, particle.x + (float) particle.s, particle.y + (float) particle.s, (float) particle.c);
        if ((double) particle.x > 132.0)
        {
          particle.x = -4f;
          particle.y = Classic.E.rnd(128f);
        }
      }
      for (int index = Classic.dead_particles.Count - 1; index >= 0; --index)
      {
        Classic.DeadParticle deadParticle = Classic.dead_particles[index];
        deadParticle.x += (float) deadParticle.spd.X;
        deadParticle.y += (float) deadParticle.spd.Y;
        --deadParticle.t;
        if (deadParticle.t <= 0)
          Classic.dead_particles.RemoveAt(index);
        Classic.E.rectfill(deadParticle.x - (float) (deadParticle.t / 5), deadParticle.y - (float) (deadParticle.t / 5), deadParticle.x + (float) (deadParticle.t / 5), deadParticle.y + (float) (deadParticle.t / 5), (float) (14 + deadParticle.t % 2));
      }
      Classic.E.rectfill(-5f, -5f, -1f, 133f, 0.0f);
      Classic.E.rectfill(-5f, -5f, 133f, -1f, 0.0f);
      Classic.E.rectfill(-5f, 128f, 133f, 133f, 0.0f);
      Classic.E.rectfill(128f, -5f, 133f, 133f, 0.0f);
      if (Classic.is_title())
        Classic.E.print("press button", 42f, 96f, 5f);
      if (Classic.level_index() != 30)
        return;
      Classic.ClassicObject classicObject1 = (Classic.ClassicObject) null;
      foreach (Classic.ClassicObject classicObject2 in Classic.objects)
      {
        if (classicObject2 is Classic.player)
        {
          classicObject1 = classicObject2;
          break;
        }
      }
      if (classicObject1 == null)
        return;
      float x2 = Classic.E.min(24f, 40f - Classic.E.abs((float) ((double) classicObject1.x + 4.0 - 64.0)));
      Classic.E.rectfill(0.0f, 0.0f, x2, 128f, 0.0f);
      Classic.E.rectfill(128f - x2, 0.0f, 128f, 128f, 0.0f);
    }

    private static void draw_object(Classic.ClassicObject obj)
    {
      obj.draw();
    }

    private static void draw_time(int x, int y)
    {
      int seconds = Classic.seconds;
      int num1 = Classic.minutes % 60;
      int num2 = Classic.E.flr((float) (Classic.minutes / 60));
      Classic.E.rectfill((float) x, (float) y, (float) (x + 32), (float) (y + 6), 0.0f);
      Classic.E.print((num2 < 10 ? (object) "0" : (object) "").ToString() + (object) num2 + ":" + (num1 < 10 ? (object) "0" : (object) "") + (object) num1 + ":" + (seconds < 10 ? (object) "0" : (object) "") + (object) seconds, (float) (x + 1), (float) (y + 1), 7f);
    }

    private static float clamp(float val, float a, float b)
    {
      return Classic.E.max(a, Classic.E.min(b, val));
    }

    private static float appr(float val, float target, float amount)
    {
      if ((double) val <= (double) target)
        return Classic.E.min(val + amount, target);
      return Classic.E.max(val - amount, target);
    }

    private static int sign(float v)
    {
      if ((double) v > 0.0)
        return 1;
      return (double) v >= 0.0 ? 0 : -1;
    }

    private static bool maybe()
    {
      return (double) Classic.E.rnd(1f) < 0.5;
    }

    private static bool solid_at(float x, float y, float w, float h)
    {
      return Classic.tile_flag_at(x, y, w, h, 0);
    }

    private static bool ice_at(float x, float y, float w, float h)
    {
      return Classic.tile_flag_at(x, y, w, h, 4);
    }

    private static bool tile_flag_at(float x, float y, float w, float h, int flag)
    {
      for (int x1 = (int) Classic.E.max(0.0f, (float) Classic.E.flr(x / 8f)); (double) x1 <= (double) Classic.E.min(15f, (float) (((double) x + (double) w - 1.0) / 8.0)); ++x1)
      {
        for (int y1 = (int) Classic.E.max(0.0f, (float) Classic.E.flr(y / 8f)); (double) y1 <= (double) Classic.E.min(15f, (float) (((double) y + (double) h - 1.0) / 8.0)); ++y1)
        {
          if (Classic.E.fget(Classic.tile_at(x1, y1), flag))
            return true;
        }
      }
      return false;
    }

    private static int tile_at(int x, int y)
    {
      return Classic.E.mget(Classic.room.X * 16 + x, Classic.room.Y * 16 + y);
    }

    private static bool spikes_at(float x, float y, int w, int h, float xspd, float yspd)
    {
      for (int x1 = (int) Classic.E.max(0.0f, (float) Classic.E.flr(x / 8f)); (double) x1 <= (double) Classic.E.min(15f, (float) (((double) x + (double) w - 1.0) / 8.0)); ++x1)
      {
        for (int y1 = (int) Classic.E.max(0.0f, (float) Classic.E.flr(y / 8f)); (double) y1 <= (double) Classic.E.min(15f, (float) (((double) y + (double) h - 1.0) / 8.0)); ++y1)
        {
          int num = Classic.tile_at(x1, y1);
          if (num == 17 && (((double) y + (double) h - 1.0) % 8.0 >= 6.0 || (double) y + (double) h == (double) (y1 * 8 + 8)) && (double) yspd >= 0.0 || (num == 27 && (double) y % 8.0 <= 2.0 && (double) yspd <= 0.0 || num == 43 && (double) x % 8.0 <= 2.0 && (double) xspd <= 0.0) || num == 59 && (((double) x + (double) w - 1.0) % 8.0 >= 6.0 || (double) x + (double) w == (double) (x1 * 8 + 8)) && (double) xspd >= 0.0)
            return true;
        }
      }
      return false;
    }

    private class Cloud
    {
      public float x;
      public float y;
      public float spd;
      public float w;
    }

    private class Particle
    {
      public float x;
      public float y;
      public int s;
      public float spd;
      public float off;
      public int c;
    }

    private class DeadParticle
    {
      public float x;
      public float y;
      public int t;
      public Vector2 spd;
    }

    public class player : Classic.ClassicObject
    {
      public int djump = Classic.max_djump;
      public Vector2 dash_target = new Vector2(0.0f, 0.0f);
      public Vector2 dash_accel = new Vector2(0.0f, 0.0f);
      public bool p_jump;
      public bool p_dash;
      public int grace;
      public int jbuffer;
      public int dash_time;
      public int dash_effect_time;
      public float spr_off;
      public bool was_on_ground;
      public Classic.player_hair hair;

      public override void init()
      {
        this.spr = 1f;
        this.hitbox = new Rectangle(1, 3, 6, 5);
      }

      public override void update()
      {
        if (Classic.pause_player)
          return;
        int ox = Classic.E.btn(Classic.k_right) ? 1 : (Classic.E.btn(Classic.k_left) ? -1 : 0);
        if (Classic.spikes_at(this.x + (float) this.hitbox.X, this.y + (float) this.hitbox.Y, (int) this.hitbox.Width, (int) this.hitbox.Height, (float) this.spd.X, (float) this.spd.Y))
          Classic.kill_player(this);
        if ((double) this.y > 128.0)
          Classic.kill_player(this);
        bool flag1 = this.is_solid(0, 1);
        bool flag2 = this.is_ice(0, 1);
        if (flag1 && !this.was_on_ground)
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f, new int?());
        int num1 = !Classic.E.btn(Classic.k_jump) ? 0 : (!this.p_jump ? 1 : 0);
        this.p_jump = Classic.E.btn(Classic.k_jump);
        if (num1 != 0)
          this.jbuffer = 4;
        else if (this.jbuffer > 0)
          --this.jbuffer;
        bool flag3 = Classic.E.btn(Classic.k_dash) && !this.p_dash;
        this.p_dash = Classic.E.btn(Classic.k_dash);
        if (flag1)
        {
          this.grace = 6;
          if (this.djump < Classic.max_djump)
          {
            Classic.psfx(54);
            this.djump = Classic.max_djump;
          }
        }
        else if (this.grace > 0)
          --this.grace;
        --this.dash_effect_time;
        if (this.dash_time > 0)
        {
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
          --this.dash_time;
          this.spd.X = (__Null) (double) Classic.appr((float) this.spd.X, (float) this.dash_target.X, (float) this.dash_accel.X);
          this.spd.Y = (__Null) (double) Classic.appr((float) this.spd.Y, (float) this.dash_target.Y, (float) this.dash_accel.Y);
        }
        else
        {
          int num2 = 1;
          float amount1 = 0.6f;
          float amount2 = 0.15f;
          if (!flag1)
            amount1 = 0.4f;
          else if (flag2)
          {
            amount1 = 0.05f;
            if (ox == (this.flipX ? -1 : 1))
              amount1 = 0.05f;
          }
          if ((double) Classic.E.abs((float) this.spd.X) > (double) num2)
            this.spd.X = (__Null) (double) Classic.appr((float) this.spd.X, (float) (Classic.E.sign((float) this.spd.X) * num2), amount2);
          else
            this.spd.X = (__Null) (double) Classic.appr((float) this.spd.X, (float) (ox * num2), amount1);
          if (this.spd.X != 0.0)
            this.flipX = this.spd.X < 0.0;
          float target = 2f;
          float amount3 = 0.21f;
          if ((double) Classic.E.abs((float) this.spd.Y) <= 0.150000005960464)
            amount3 *= 0.5f;
          if (ox != 0 && this.is_solid(ox, 0) && !this.is_ice(ox, 0))
          {
            target = 0.4f;
            if ((double) Classic.E.rnd(10f) < 2.0)
              Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x + (float) (ox * 6), this.y, new int?());
          }
          if (!flag1)
            this.spd.Y = (__Null) (double) Classic.appr((float) this.spd.Y, target, amount3);
          if (this.jbuffer > 0)
          {
            if (this.grace > 0)
            {
              Classic.psfx(1);
              this.jbuffer = 0;
              this.grace = 0;
              this.spd.Y = (__Null) -2.0;
              Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f, new int?());
            }
            else
            {
              int num3 = this.is_solid(-3, 0) ? -1 : (this.is_solid(3, 0) ? 1 : 0);
              if (num3 != 0)
              {
                Classic.psfx(2);
                this.jbuffer = 0;
                this.spd.Y = (__Null) -2.0;
                this.spd.X = (__Null) (double) (-num3 * (num2 + 1));
                if (this.is_ice(num3 * 3, 0))
                  Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x + (float) (num3 * 6), this.y, new int?());
              }
            }
          }
          int num4 = 5;
          float num5 = (float) num4 * 0.7071068f;
          if (this.djump > 0 & flag3)
          {
            Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
            --this.djump;
            this.dash_time = 4;
            Classic.has_dashed = true;
            this.dash_effect_time = 10;
            int num3 = Classic.E.dashDirectionX(this.flipX ? -1 : 1);
            int num6 = Classic.E.dashDirectionY(this.flipX ? -1 : 1);
            if (num3 != 0 && num6 != 0)
            {
              this.spd.X = (__Null) ((double) num3 * (double) num5);
              this.spd.Y = (__Null) ((double) num6 * (double) num5);
            }
            else if (num3 != 0)
            {
              this.spd.X = (__Null) (double) (num3 * num4);
              this.spd.Y = (__Null) 0.0;
            }
            else
            {
              this.spd.X = (__Null) 0.0;
              this.spd.Y = (__Null) (double) (num6 * num4);
            }
            Classic.psfx(3);
            Classic.freeze = 2;
            Classic.shake = 6;
            this.dash_target.X = (__Null) (double) (2 * Classic.E.sign((float) this.spd.X));
            this.dash_target.Y = (__Null) (double) (2 * Classic.E.sign((float) this.spd.Y));
            this.dash_accel.X = (__Null) 1.5;
            this.dash_accel.Y = (__Null) 1.5;
            if (this.spd.Y < 0.0)
            {
              ref __Null local = ref this.dash_target.Y;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local * 0.75f;
            }
            if (this.spd.Y != 0.0)
            {
              ref __Null local = ref this.dash_accel.X;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local * 0.7071068f;
            }
            if (this.spd.X != 0.0)
            {
              ref __Null local = ref this.dash_accel.Y;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local * 0.7071068f;
            }
          }
          else if (flag3 && this.djump <= 0)
          {
            Classic.psfx(9);
            Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
          }
        }
        this.spr_off += 0.25f;
        if (!flag1)
        {
          if (this.is_solid(ox, 0))
            this.spr = 5f;
          else
            this.spr = 3f;
        }
        else if (Classic.E.btn(Classic.k_down))
          this.spr = 6f;
        else if (Classic.E.btn(Classic.k_up))
          this.spr = 7f;
        else if (this.spd.X == 0.0 || !Classic.E.btn(Classic.k_left) && !Classic.E.btn(Classic.k_right))
          this.spr = 1f;
        else
          this.spr = (float) (1.0 + (double) this.spr_off % 4.0);
        if ((double) this.y < -4.0 && Classic.level_index() < 30)
          Classic.next_room();
        this.was_on_ground = flag1;
      }

      public override void draw()
      {
        if ((double) this.x < -1.0 || (double) this.x > 121.0)
        {
          this.x = Classic.clamp(this.x, -1f, 121f);
          this.spd.X = (__Null) 0.0;
        }
        this.hair.draw_hair((Classic.ClassicObject) this, this.flipX ? -1 : 1, this.djump);
        Classic.draw_player((Classic.ClassicObject) this, this.djump);
      }
    }

    public class player_hair
    {
      private Classic.player_hair.node[] hair = new Classic.player_hair.node[5];

      public player_hair(Classic.ClassicObject obj)
      {
        for (int index = 0; index <= 4; ++index)
          this.hair[index] = new Classic.player_hair.node()
          {
            x = obj.x,
            y = obj.y,
            size = Classic.E.max(1f, Classic.E.min(2f, (float) (3 - index)))
          };
      }

      public void draw_hair(Classic.ClassicObject obj, int facing, int djump)
      {
        int num1;
        switch (djump)
        {
          case 1:
            num1 = 8;
            break;
          case 2:
            num1 = 7 + Classic.E.flr((float) (Classic.frames / 3 % 2)) * 4;
            break;
          default:
            num1 = 12;
            break;
        }
        int num2 = num1;
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(obj.x + 4f - (float) (facing * 2), obj.y + (Classic.E.btn(Classic.k_down) ? 4f : 3f));
        foreach (Classic.player_hair.node node in this.hair)
        {
          node.x += (float) ((vector2.X - (double) node.x) / 1.5);
          node.y += (float) ((vector2.Y + 0.5 - (double) node.y) / 1.5);
          Classic.E.circfill(node.x, node.y, node.size, (float) num2);
          ((Vector2) ref vector2).\u002Ector(node.x, node.y);
        }
      }

      private class node
      {
        public float x;
        public float y;
        public float size;
      }
    }

    public class player_spawn : Classic.ClassicObject
    {
      private Vector2 target;
      private int state;
      private int delay;
      private Classic.player_hair hair;

      public override void init()
      {
        this.spr = 3f;
        this.target = new Vector2(this.x, this.y);
        this.y = 128f;
        this.spd.Y = (__Null) -4.0;
        this.state = 0;
        this.delay = 0;
        this.solids = false;
        this.hair = new Classic.player_hair((Classic.ClassicObject) this);
        Classic.E.sfx(4);
      }

      public override void update()
      {
        if (this.state == 0)
        {
          if ((double) this.y >= this.target.Y + 16.0)
            return;
          this.state = 1;
          this.delay = 3;
        }
        else if (this.state == 1)
        {
          ref __Null local = ref this.spd.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + 0.5f;
          if (this.spd.Y > 0.0 && this.delay > 0)
          {
            this.spd.Y = (__Null) 0.0;
            --this.delay;
          }
          if (this.spd.Y <= 0.0 || (double) this.y <= this.target.Y)
            return;
          this.y = (float) this.target.Y;
          this.spd = new Vector2(0.0f, 0.0f);
          this.state = 2;
          this.delay = 5;
          Classic.shake = 5;
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f, new int?());
          Classic.E.sfx(5);
        }
        else
        {
          if (this.state != 2)
            return;
          --this.delay;
          this.spr = 6f;
          if (this.delay >= 0)
            return;
          Classic.destroy_object((Classic.ClassicObject) this);
          Classic.init_object<Classic.player>(new Classic.player(), this.x, this.y, new int?()).hair = this.hair;
        }
      }

      public override void draw()
      {
        this.hair.draw_hair((Classic.ClassicObject) this, 1, Classic.max_djump);
        Classic.draw_player((Classic.ClassicObject) this, Classic.max_djump);
      }
    }

    public class spring : Classic.ClassicObject
    {
      public int hide_in;
      private int hide_for;
      private int delay;

      public override void update()
      {
        if (this.hide_for > 0)
        {
          --this.hide_for;
          if (this.hide_for <= 0)
          {
            this.spr = 18f;
            this.delay = 0;
          }
        }
        else if ((double) this.spr == 18.0)
        {
          Classic.player player = this.collide<Classic.player>(0, 0);
          if (player != null && player.spd.Y >= 0.0)
          {
            this.spr = 19f;
            player.y = this.y - 4f;
            ref __Null local = ref player.spd.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local * 0.2f;
            player.spd.Y = (__Null) -3.0;
            player.djump = Classic.max_djump;
            this.delay = 10;
            Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
            Classic.fall_floor fallFloor = this.collide<Classic.fall_floor>(0, 1);
            if (fallFloor != null)
              Classic.break_fall_floor(fallFloor);
            Classic.psfx(8);
          }
        }
        else if (this.delay > 0)
        {
          --this.delay;
          if (this.delay <= 0)
            this.spr = 18f;
        }
        if (this.hide_in <= 0)
          return;
        --this.hide_in;
        if (this.hide_in > 0)
          return;
        this.hide_for = 60;
        this.spr = 0.0f;
      }
    }

    public class balloon : Classic.ClassicObject
    {
      private float offset = Classic.E.rnd(1f);
      private float start;
      private float timer;

      public override void init()
      {
        this.start = this.y;
        this.hitbox = new Rectangle(-1, -1, 10, 10);
      }

      public override void update()
      {
        if ((double) this.spr == 22.0)
        {
          this.offset += 0.01f;
          this.y = this.start + Classic.E.sin(this.offset) * 2f;
          Classic.player player = this.collide<Classic.player>(0, 0);
          if (player == null || player.djump >= Classic.max_djump)
            return;
          Classic.psfx(6);
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
          player.djump = Classic.max_djump;
          this.spr = 0.0f;
          this.timer = 60f;
        }
        else if ((double) this.timer > 0.0)
        {
          --this.timer;
        }
        else
        {
          Classic.psfx(7);
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
          this.spr = 22f;
        }
      }

      public override void draw()
      {
        if ((double) this.spr != 22.0)
          return;
        Classic.E.spr((float) (13.0 + (double) this.offset * 8.0 % 3.0), this.x, this.y + 6f, 1, 1, false, false);
        Classic.E.spr(this.spr, this.x, this.y, 1, 1, false, false);
      }
    }

    public class fall_floor : Classic.ClassicObject
    {
      public bool solid = true;
      public int state;
      public int delay;

      public override void update()
      {
        if (this.state == 0)
        {
          if (!this.check<Classic.player>(0, -1) && !this.check<Classic.player>(-1, 0) && !this.check<Classic.player>(1, 0))
            return;
          Classic.break_fall_floor(this);
        }
        else if (this.state == 1)
        {
          --this.delay;
          if (this.delay > 0)
            return;
          this.state = 2;
          this.delay = 60;
          this.collideable = false;
        }
        else
        {
          if (this.state != 2)
            return;
          --this.delay;
          if (this.delay > 0 || this.check<Classic.player>(0, 0))
            return;
          Classic.psfx(7);
          this.state = 0;
          this.collideable = true;
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
        }
      }

      public override void draw()
      {
        if (this.state == 2)
          return;
        if (this.state != 1)
          Classic.E.spr(23f, this.x, this.y, 1, 1, false, false);
        else
          Classic.E.spr((float) (23 + (15 - this.delay) / 5), this.x, this.y, 1, 1, false, false);
      }
    }

    public class smoke : Classic.ClassicObject
    {
      public override void init()
      {
        this.spr = 29f;
        this.spd.Y = (__Null) -0.100000001490116;
        this.spd.X = (__Null) (0.300000011920929 + (double) Classic.E.rnd(0.2f));
        this.x += Classic.E.rnd(2f) - 1f;
        this.y += Classic.E.rnd(2f) - 1f;
        this.flipX = Classic.maybe();
        this.flipY = Classic.maybe();
        this.solids = false;
      }

      public override void update()
      {
        this.spr += 0.2f;
        if ((double) this.spr < 32.0)
          return;
        Classic.destroy_object((Classic.ClassicObject) this);
      }
    }

    public class fruit : Classic.ClassicObject
    {
      private float start;
      private float off;

      public override void init()
      {
        this.spr = 26f;
        this.start = this.y;
        this.off = 0.0f;
      }

      public override void update()
      {
        Classic.player player = this.collide<Classic.player>(0, 0);
        if (player != null)
        {
          player.djump = Classic.max_djump;
          Classic.sfx_timer = 20;
          Classic.E.sfx(13);
          Classic.got_fruit.Add(1 + Classic.level_index());
          Classic.init_object<Classic.lifeup>(new Classic.lifeup(), this.x, this.y, new int?());
          Classic.destroy_object((Classic.ClassicObject) this);
          Stats.Increment(Stat.PICO_BERRIES, 1);
        }
        ++this.off;
        this.y = this.start + Classic.E.sin(this.off / 40f) * 2.5f;
      }
    }

    public class fly_fruit : Classic.ClassicObject
    {
      private float step = 0.5f;
      private float sfx_delay = 8f;
      private float start;
      private bool fly;

      public override void init()
      {
        this.start = this.y;
        this.solids = false;
      }

      public override void update()
      {
        if (this.fly)
        {
          if ((double) this.sfx_delay > 0.0)
          {
            --this.sfx_delay;
            if ((double) this.sfx_delay <= 0.0)
            {
              Classic.sfx_timer = 20;
              Classic.E.sfx(14);
            }
          }
          this.spd.Y = (__Null) (double) Classic.appr((float) this.spd.Y, -3.5f, 0.25f);
          if ((double) this.y < -16.0)
            Classic.destroy_object((Classic.ClassicObject) this);
        }
        else
        {
          if (Classic.has_dashed)
            this.fly = true;
          this.step += 0.05f;
          this.spd.Y = (__Null) ((double) Classic.E.sin(this.step) * 0.5);
        }
        Classic.player player = this.collide<Classic.player>(0, 0);
        if (player == null)
          return;
        player.djump = Classic.max_djump;
        Classic.sfx_timer = 20;
        Classic.E.sfx(13);
        Classic.got_fruit.Add(1 + Classic.level_index());
        Classic.init_object<Classic.lifeup>(new Classic.lifeup(), this.x, this.y, new int?());
        Classic.destroy_object((Classic.ClassicObject) this);
        Stats.Increment(Stat.PICO_BERRIES, 1);
      }

      public override void draw()
      {
        float num = 0.0f;
        if (!this.fly)
        {
          if ((double) Classic.E.sin(this.step) < 0.0)
            num = 1f + Classic.E.max(0.0f, (float) Classic.sign(this.y - this.start));
        }
        else
          num = (float) (((double) num + 0.25) % 3.0);
        Classic.E.spr(45f + num, this.x - 6f, this.y - 2f, 1, 1, true, false);
        Classic.E.spr(this.spr, this.x, this.y, 1, 1, false, false);
        Classic.E.spr(45f + num, this.x + 6f, this.y - 2f, 1, 1, false, false);
      }
    }

    public class lifeup : Classic.ClassicObject
    {
      private int duration;
      private float flash;

      public override void init()
      {
        this.spd.Y = (__Null) -0.25;
        this.duration = 30;
        this.x -= 2f;
        this.y -= 4f;
        this.flash = 0.0f;
        this.solids = false;
      }

      public override void update()
      {
        --this.duration;
        if (this.duration > 0)
          return;
        Classic.destroy_object((Classic.ClassicObject) this);
      }

      public override void draw()
      {
        this.flash += 0.5f;
        Classic.E.print("1000", this.x - 2f, this.y, (float) (7.0 + (double) this.flash % 2.0));
      }
    }

    public class fake_wall : Classic.ClassicObject
    {
      public override void update()
      {
        this.hitbox = new Rectangle(-1, -1, 18, 18);
        Classic.player player = this.collide<Classic.player>(0, 0);
        if (player != null && player.dash_effect_time > 0)
        {
          player.spd.X = (__Null) ((double) -Classic.sign((float) player.spd.X) * 1.5);
          player.spd.Y = (__Null) -1.5;
          player.dash_time = -1;
          Classic.sfx_timer = 20;
          Classic.E.sfx(16);
          Classic.destroy_object((Classic.ClassicObject) this);
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y, new int?());
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 8f, new int?());
          Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y + 8f, new int?());
          Classic.init_object<Classic.fruit>(new Classic.fruit(), this.x + 4f, this.y + 4f, new int?());
        }
        this.hitbox = new Rectangle(0, 0, 16, 16);
      }

      public override void draw()
      {
        Classic.E.spr(64f, this.x, this.y, 1, 1, false, false);
        Classic.E.spr(65f, this.x + 8f, this.y, 1, 1, false, false);
        Classic.E.spr(80f, this.x, this.y + 8f, 1, 1, false, false);
        Classic.E.spr(81f, this.x + 8f, this.y + 8f, 1, 1, false, false);
      }
    }

    public class key : Classic.ClassicObject
    {
      public override void update()
      {
        int num1 = Classic.E.flr(this.spr);
        this.spr = (float) (9.0 + ((double) Classic.E.sin((float) Classic.frames / 30f) + 0.5) * 1.0);
        int num2 = Classic.E.flr(this.spr);
        if (num2 == 10 && num2 != num1)
          this.flipX = !this.flipX;
        if (!this.check<Classic.player>(0, 0))
          return;
        Classic.E.sfx(23);
        Classic.sfx_timer = 20;
        Classic.destroy_object((Classic.ClassicObject) this);
        Classic.has_key = true;
      }
    }

    public class chest : Classic.ClassicObject
    {
      private float start;
      private float timer;

      public override void init()
      {
        this.x -= 4f;
        this.start = this.x;
        this.timer = 20f;
      }

      public override void update()
      {
        if (!Classic.has_key)
          return;
        --this.timer;
        this.x = this.start - 1f + Classic.E.rnd(3f);
        if ((double) this.timer > 0.0)
          return;
        Classic.sfx_timer = 20;
        Classic.E.sfx(16);
        Classic.init_object<Classic.fruit>(new Classic.fruit(), this.x, this.y - 4f, new int?());
        Classic.destroy_object((Classic.ClassicObject) this);
      }
    }

    public class platform : Classic.ClassicObject
    {
      public float dir;
      private float last;

      public override void init()
      {
        this.x -= 4f;
        this.solids = false;
        this.hitbox.Width = (__Null) 16;
        this.last = this.x;
      }

      public override void update()
      {
        this.spd.X = (__Null) ((double) this.dir * 0.649999976158142);
        if ((double) this.x < -16.0)
          this.x = 128f;
        if ((double) this.x > 128.0)
          this.x = -16f;
        if (!this.check<Classic.player>(0, 0))
          this.collide<Classic.player>(0, -1)?.move_x((int) ((double) this.x - (double) this.last), 1);
        this.last = this.x;
      }

      public override void draw()
      {
        Classic.E.spr(11f, this.x, this.y - 1f, 1, 1, false, false);
        Classic.E.spr(12f, this.x + 8f, this.y - 1f, 1, 1, false, false);
      }
    }

    public class message : Classic.ClassicObject
    {
      private float last;
      private float index;

      public override void draw()
      {
        string str = "-- celeste mountain --#this memorial to those# perished on the climb";
        if (this.check<Classic.player>(4, 0))
        {
          if ((double) this.index < (double) str.Length)
          {
            this.index += 0.5f;
            if ((double) this.index >= (double) this.last + 1.0)
            {
              ++this.last;
              Classic.E.sfx(35);
            }
          }
          Vector2 vector2;
          ((Vector2) ref vector2).\u002Ector(8f, 96f);
          for (int index = 0; (double) index < (double) this.index; ++index)
          {
            if (str[index] != '#')
            {
              Classic.E.rectfill((float) (vector2.X - 2.0), (float) (vector2.Y - 2.0), (float) (vector2.X + 7.0), (float) (vector2.Y + 6.0), 7f);
              Classic.E.print(str[index].ToString() ?? "", (float) vector2.X, (float) vector2.Y, 0.0f);
              ref __Null local = ref vector2.X;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local + 5f;
            }
            else
            {
              vector2.X = (__Null) 8.0;
              ref __Null local = ref vector2.Y;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local + 7f;
            }
          }
        }
        else
        {
          this.index = 0.0f;
          this.last = 0.0f;
        }
      }
    }

    public class big_chest : Classic.ClassicObject
    {
      private int state;
      private float timer;
      private List<Classic.big_chest.particle> particles;

      public override void init()
      {
        this.hitbox.Width = (__Null) 16;
      }

      public override void draw()
      {
        if (this.state == 0)
        {
          Classic.player player = this.collide<Classic.player>(0, 8);
          if (player != null && player.is_solid(0, 1))
          {
            Classic.E.music(-1, 500, 7);
            Classic.E.sfx(37);
            Classic.pause_player = true;
            player.spd.X = (__Null) 0.0;
            player.spd.Y = (__Null) 0.0;
            this.state = 1;
            Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y, new int?());
            Classic.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y, new int?());
            this.timer = 60f;
            this.particles = new List<Classic.big_chest.particle>();
          }
          Classic.E.spr(96f, this.x, this.y, 1, 1, false, false);
          Classic.E.spr(97f, this.x + 8f, this.y, 1, 1, false, false);
        }
        else if (this.state == 1)
        {
          --this.timer;
          Classic.shake = 5;
          Classic.flash_bg = true;
          if ((double) this.timer <= 45.0 && this.particles.Count < 50)
            this.particles.Add(new Classic.big_chest.particle()
            {
              x = 1f + Classic.E.rnd(14f),
              y = 0.0f,
              h = 32f + Classic.E.rnd(32f),
              spd = 8f + Classic.E.rnd(8f)
            });
          if ((double) this.timer < 0.0)
          {
            this.state = 2;
            this.particles.Clear();
            Classic.flash_bg = false;
            Classic.new_bg = true;
            Classic.init_object<Classic.orb>(new Classic.orb(), this.x + 4f, this.y + 4f, new int?());
            Classic.pause_player = false;
          }
          foreach (Classic.big_chest.particle particle in this.particles)
          {
            particle.y += particle.spd;
            Classic.E.rectfill(this.x + particle.x, this.y + 8f - particle.y, (float) ((double) this.x + (double) particle.x + 1.0), Classic.E.min(this.y + 8f - particle.y + particle.h, this.y + 8f), 7f);
          }
        }
        Classic.E.spr(112f, this.x, this.y + 8f, 1, 1, false, false);
        Classic.E.spr(113f, this.x + 8f, this.y + 8f, 1, 1, false, false);
      }

      private class particle
      {
        public float x;
        public float y;
        public float h;
        public float spd;
      }
    }

    public class orb : Classic.ClassicObject
    {
      public override void init()
      {
        this.spd.Y = (__Null) -4.0;
        this.solids = false;
      }

      public override void draw()
      {
        this.spd.Y = (__Null) (double) Classic.appr((float) this.spd.Y, 0.0f, 0.5f);
        Classic.player player = this.collide<Classic.player>(0, 0);
        if (this.spd.Y == 0.0 && player != null)
        {
          Classic.music_timer = 45;
          Classic.E.sfx(51);
          Classic.freeze = 10;
          Classic.shake = 10;
          Classic.destroy_object((Classic.ClassicObject) this);
          Classic.max_djump = 2;
          player.djump = 2;
        }
        Classic.E.spr(102f, this.x, this.y, 1, 1, false, false);
        float num = (float) Classic.frames / 30f;
        for (int index = 0; index <= 7; ++index)
          Classic.E.circfill((float) ((double) this.x + 4.0 + (double) Classic.E.cos(num + (float) index / 8f) * 8.0), (float) ((double) this.y + 4.0 + (double) Classic.E.sin(num + (float) index / 8f) * 8.0), 1f, 7f);
      }
    }

    public class flag : Classic.ClassicObject
    {
      private float score;
      private bool show;

      public override void init()
      {
        this.x += 5f;
        this.score = (float) Classic.got_fruit.Count;
        Stats.Increment(Stat.PICO_COMPLETES, 1);
        Achievements.Register(Achievement.PICO8);
      }

      public override void draw()
      {
        this.spr = (float) (118.0 + (double) Classic.frames / 5.0 % 3.0);
        Classic.E.spr(this.spr, this.x, this.y, 1, 1, false, false);
        if (this.show)
        {
          Classic.E.rectfill(32f, 2f, 96f, 31f, 0.0f);
          Classic.E.spr(26f, 55f, 6f, 1, 1, false, false);
          Classic.E.print("x" + (object) this.score, 64f, 9f, 7f);
          Classic.draw_time(49, 16);
          Classic.E.print("deaths:" + (object) Classic.deaths, 48f, 24f, 7f);
        }
        else
        {
          if (!this.check<Classic.player>(0, 0))
            return;
          Classic.E.sfx(55);
          Classic.sfx_timer = 30;
          this.show = true;
        }
      }
    }

    public class room_title : Classic.ClassicObject
    {
      private float delay = 5f;

      public override void draw()
      {
        --this.delay;
        if ((double) this.delay < -30.0)
        {
          Classic.destroy_object((Classic.ClassicObject) this);
        }
        else
        {
          if ((double) this.delay >= 0.0)
            return;
          Classic.E.rectfill(24f, 58f, 104f, 70f, 0.0f);
          if (Classic.room.X == 3 && Classic.room.Y == 1)
            Classic.E.print("old site", 48f, 62f, 7f);
          else if (Classic.level_index() == 30)
          {
            Classic.E.print("summit", 52f, 62f, 7f);
          }
          else
          {
            int num = (1 + Classic.level_index()) * 100;
            Classic.E.print(num.ToString() + "m", (float) (52 + (num < 1000 ? 2 : 0)), 62f, 7f);
          }
          Classic.draw_time(4, 4);
        }
      }
    }

    public class ClassicObject
    {
      public bool collideable = true;
      public bool solids = true;
      public Rectangle hitbox = new Rectangle(0, 0, 8, 8);
      public Vector2 spd = new Vector2(0.0f, 0.0f);
      public Vector2 rem = new Vector2(0.0f, 0.0f);
      public int type;
      public float spr;
      public bool flipX;
      public bool flipY;
      public float x;
      public float y;

      public virtual void init()
      {
      }

      public virtual void update()
      {
      }

      public virtual void draw()
      {
        if ((double) this.spr <= 0.0)
          return;
        Classic.E.spr(this.spr, this.x, this.y, 1, 1, this.flipX, this.flipY);
      }

      public bool is_solid(int ox, int oy)
      {
        if (oy > 0 && !this.check<Classic.platform>(ox, 0) && this.check<Classic.platform>(ox, oy) || (Classic.solid_at(this.x + (float) this.hitbox.X + (float) ox, this.y + (float) this.hitbox.Y + (float) oy, (float) this.hitbox.Width, (float) this.hitbox.Height) || this.check<Classic.fall_floor>(ox, oy)))
          return true;
        return this.check<Classic.fake_wall>(ox, oy);
      }

      public bool is_ice(int ox, int oy)
      {
        return Classic.ice_at(this.x + (float) this.hitbox.X + (float) ox, this.y + (float) this.hitbox.Y + (float) oy, (float) this.hitbox.Width, (float) this.hitbox.Height);
      }

      public T collide<T>(int ox, int oy) where T : Classic.ClassicObject
      {
        Type type = typeof (T);
        foreach (Classic.ClassicObject classicObject in Classic.objects)
        {
          if (classicObject != null && classicObject.GetType() == type && (classicObject != this && classicObject.collideable) && ((double) classicObject.x + (double) (float) classicObject.hitbox.X + (double) (float) classicObject.hitbox.Width > (double) this.x + (double) (float) this.hitbox.X + (double) ox && (double) classicObject.y + (double) (float) classicObject.hitbox.Y + (double) (float) classicObject.hitbox.Height > (double) this.y + (double) (float) this.hitbox.Y + (double) oy && ((double) classicObject.x + (double) (float) classicObject.hitbox.X < (double) this.x + (double) (float) this.hitbox.X + (double) (float) this.hitbox.Width + (double) ox && (double) classicObject.y + (double) (float) classicObject.hitbox.Y < (double) this.y + (double) (float) this.hitbox.Y + (double) (float) this.hitbox.Height + (double) oy)))
            return classicObject as T;
        }
        return default (T);
      }

      public bool check<T>(int ox, int oy) where T : Classic.ClassicObject
      {
        return (object) this.collide<T>(ox, oy) != null;
      }

      public void move(float ox, float oy)
      {
        ref __Null local1 = ref this.rem.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + ox;
        int amount1 = Classic.E.flr((float) (this.rem.X + 0.5));
        ref __Null local2 = ref this.rem.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 - (float) amount1;
        this.move_x(amount1, 0);
        ref __Null local3 = ref this.rem.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local3 = ^(float&) ref local3 + oy;
        int amount2 = Classic.E.flr((float) (this.rem.Y + 0.5));
        ref __Null local4 = ref this.rem.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local4 = ^(float&) ref local4 - (float) amount2;
        this.move_y(amount2);
      }

      public void move_x(int amount, int start)
      {
        if (this.solids)
        {
          int ox = Classic.sign((float) amount);
          for (int index = start; (double) index <= (double) Classic.E.abs((float) amount); ++index)
          {
            if (!this.is_solid(ox, 0))
            {
              this.x += (float) ox;
            }
            else
            {
              this.spd.X = (__Null) 0.0;
              this.rem.X = (__Null) 0.0;
              break;
            }
          }
        }
        else
          this.x += (float) amount;
      }

      public void move_y(int amount)
      {
        if (this.solids)
        {
          int oy = Classic.sign((float) amount);
          for (int index = 0; (double) index <= (double) Classic.E.abs((float) amount); ++index)
          {
            if (!this.is_solid(0, oy))
            {
              this.y += (float) oy;
            }
            else
            {
              this.spd.Y = (__Null) 0.0;
              this.rem.Y = (__Null) 0.0;
              break;
            }
          }
        }
        else
          this.y += (float) amount;
      }
    }
  }
}
