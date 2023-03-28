// Decompiled with JetBrains decompiler
// Type: Celeste.Pico8.Classic
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Celeste.Pico8
{
  public class Classic
  {
    public Emulator E;
    private Point room;
    private List<Classic.ClassicObject> objects;
    public int freeze;
    private int shake;
    private bool will_restart;
    private int delay_restart;
    private HashSet<int> got_fruit;
    private bool has_dashed;
    private int sfx_timer;
    private bool has_key;
    private bool pause_player;
    private bool flash_bg;
    private int music_timer;
    private bool new_bg;
    private int k_left;
    private int k_right = 1;
    private int k_up = 2;
    private int k_down = 3;
    private int k_jump = 4;
    private int k_dash = 5;
    private int frames;
    private int seconds;
    private int minutes;
    private int deaths;
    private int max_djump;
    private bool start_game;
    private int start_game_flash;
    private bool room_just_loaded;
    private List<Classic.Cloud> clouds;
    private List<Classic.Particle> particles;
    private List<Classic.DeadParticle> dead_particles;

    public void Init(Emulator emulator)
    {
      this.E = emulator;
      this.room = new Point(0, 0);
      this.objects = new List<Classic.ClassicObject>();
      this.freeze = 0;
      this.will_restart = false;
      this.delay_restart = 0;
      this.got_fruit = new HashSet<int>();
      this.has_dashed = false;
      this.sfx_timer = 0;
      this.has_key = false;
      this.pause_player = false;
      this.flash_bg = false;
      this.music_timer = 0;
      this.new_bg = false;
      this.room_just_loaded = false;
      this.frames = 0;
      this.seconds = 0;
      this.minutes = 0;
      this.deaths = 0;
      this.max_djump = 1;
      this.start_game = false;
      this.start_game_flash = 0;
      this.clouds = new List<Classic.Cloud>();
      for (int index = 0; index <= 16; ++index)
        this.clouds.Add(new Classic.Cloud()
        {
          x = this.E.rnd(128f),
          y = this.E.rnd(128f),
          spd = 1f + this.E.rnd(4f),
          w = 32f + this.E.rnd(32f)
        });
      this.particles = new List<Classic.Particle>();
      for (int index = 0; index <= 32; ++index)
        this.particles.Add(new Classic.Particle()
        {
          x = this.E.rnd(128f),
          y = this.E.rnd(128f),
          s = this.E.flr(this.E.rnd(5f) / 4f),
          spd = 0.25f + this.E.rnd(5f),
          off = this.E.rnd(1f),
          c = 6 + this.E.flr(0.5f + this.E.rnd(1f))
        });
      this.dead_particles = new List<Classic.DeadParticle>();
      this.title_screen();
    }

    private void title_screen()
    {
      this.got_fruit = new HashSet<int>();
      this.frames = 0;
      this.deaths = 0;
      this.max_djump = 1;
      this.start_game = false;
      this.start_game_flash = 0;
      this.E.music(40, 0, 7);
      this.load_room(7, 3);
    }

    private void begin_game()
    {
      this.frames = 0;
      this.seconds = 0;
      this.minutes = 0;
      this.music_timer = 0;
      this.start_game = false;
      this.E.music(0, 0, 7);
      this.load_room(0, 0);
    }

    private int level_index() => this.room.X % 8 + this.room.Y * 8;

    private bool is_title() => this.level_index() == 31;

    private void psfx(int num)
    {
      if (this.sfx_timer > 0)
        return;
      this.E.sfx(num);
    }

    private void draw_player(Classic.ClassicObject obj, int djump)
    {
      int num = 0;
      switch (djump)
      {
        case 0:
          num = 128;
          break;
        case 2:
          num = this.E.flr((float) (this.frames / 3 % 2)) != 0 ? 144 : 160;
          break;
      }
      this.E.spr(obj.spr + (float) num, obj.x, obj.y, flipX: obj.flipX, flipY: obj.flipY);
    }

    private void break_spring(Classic.spring obj) => obj.hide_in = 15;

    private void break_fall_floor(Classic.fall_floor obj)
    {
      if (obj.state != 0)
        return;
      this.psfx(15);
      obj.state = 1;
      obj.delay = 15;
      this.init_object<Classic.smoke>(new Classic.smoke(), obj.x, obj.y);
      Classic.spring spring = obj.collide<Classic.spring>(0, -1);
      if (spring == null)
        return;
      this.break_spring(spring);
    }

    private T init_object<T>(T obj, float x, float y, int? tile = null) where T : Classic.ClassicObject
    {
      this.objects.Add((Classic.ClassicObject) obj);
      if (tile.HasValue)
        obj.spr = (float) tile.Value;
      obj.x = (float) (int) x;
      obj.y = (float) (int) y;
      obj.init(this, this.E);
      return obj;
    }

    private void destroy_object(Classic.ClassicObject obj)
    {
      int index = this.objects.IndexOf(obj);
      if (index < 0)
        return;
      this.objects[index] = (Classic.ClassicObject) null;
    }

    private void kill_player(Classic.player obj)
    {
      this.sfx_timer = 12;
      this.E.sfx(0);
      ++this.deaths;
      this.shake = 10;
      this.destroy_object((Classic.ClassicObject) obj);
      Stats.Increment(Stat.PICO_DEATHS);
      this.dead_particles.Clear();
      for (int index = 0; index <= 7; ++index)
      {
        float a = (float) index / 8f;
        this.dead_particles.Add(new Classic.DeadParticle()
        {
          x = obj.x + 4f,
          y = obj.y + 4f,
          t = 10,
          spd = new Vector2(this.E.cos(a) * 3f, this.E.sin(a + 0.5f) * 3f)
        });
      }
      this.restart_room();
    }

    private void restart_room()
    {
      this.will_restart = true;
      this.delay_restart = 15;
    }

    private void next_room()
    {
      if (this.room.X == 2 && this.room.Y == 1)
        this.E.music(30, 500, 7);
      else if (this.room.X == 3 && this.room.Y == 1)
        this.E.music(20, 500, 7);
      else if (this.room.X == 4 && this.room.Y == 2)
        this.E.music(30, 500, 7);
      else if (this.room.X == 5 && this.room.Y == 3)
        this.E.music(30, 500, 7);
      if (this.room.X == 7)
        this.load_room(0, this.room.Y + 1);
      else
        this.load_room(this.room.X + 1, this.room.Y);
    }

    public void load_room(int x, int y)
    {
      this.room_just_loaded = true;
      this.has_dashed = false;
      this.has_key = false;
      for (int index = 0; index < this.objects.Count; ++index)
        this.objects[index] = (Classic.ClassicObject) null;
      this.room.X = x;
      this.room.Y = y;
      for (int index1 = 0; index1 <= 15; ++index1)
      {
        for (int index2 = 0; index2 <= 15; ++index2)
        {
          int num = this.E.mget(this.room.X * 16 + index1, this.room.Y * 16 + index2);
          switch (num)
          {
            case 11:
              this.init_object<Classic.platform>(new Classic.platform(), (float) (index1 * 8), (float) (index2 * 8)).dir = -1f;
              break;
            case 12:
              this.init_object<Classic.platform>(new Classic.platform(), (float) (index1 * 8), (float) (index2 * 8)).dir = 1f;
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
              else if (!this.got_fruit.Contains(1 + this.level_index()))
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
                this.init_object<Classic.ClassicObject>(classicObject, (float) (index1 * 8), (float) (index2 * 8), new int?(num));
                break;
              }
              break;
          }
        }
      }
      if (this.is_title())
        return;
      this.init_object<Classic.room_title>(new Classic.room_title(), 0.0f, 0.0f);
    }

    public void Update()
    {
      this.frames = (this.frames + 1) % 30;
      if (this.frames == 0 && this.level_index() < 30)
      {
        this.seconds = (this.seconds + 1) % 60;
        if (this.seconds == 0)
          ++this.minutes;
      }
      if (this.music_timer > 0)
      {
        --this.music_timer;
        if (this.music_timer <= 0)
          this.E.music(10, 0, 7);
      }
      if (this.sfx_timer > 0)
        --this.sfx_timer;
      if (this.freeze > 0)
      {
        --this.freeze;
      }
      else
      {
        if (this.shake > 0 && Settings.Instance.ScreenShake != ScreenshakeAmount.Off)
        {
          --this.shake;
          this.E.camera();
          if (this.shake > 0)
          {
            if (Settings.Instance.ScreenShake == ScreenshakeAmount.On)
              this.E.camera(this.E.rnd(5f) - 2f, this.E.rnd(5f) - 2f);
            else
              this.E.camera(this.E.rnd(3f) - 1f, this.E.rnd(3f) - 1f);
          }
        }
        if (this.will_restart && this.delay_restart > 0)
        {
          --this.delay_restart;
          if (this.delay_restart <= 0)
          {
            this.will_restart = true;
            this.load_room(this.room.X, this.room.Y);
          }
        }
        this.room_just_loaded = false;
        int num = 0;
        while (num != -1)
        {
          int index = num;
          num = -1;
          for (; index < this.objects.Count; ++index)
          {
            Classic.ClassicObject classicObject = this.objects[index];
            if (classicObject != null)
            {
              classicObject.move(classicObject.spd.X, classicObject.spd.Y);
              classicObject.update();
              if (this.room_just_loaded)
              {
                this.room_just_loaded = false;
                num = index;
                break;
              }
            }
          }
          while (this.objects.IndexOf((Classic.ClassicObject) null) >= 0)
            this.objects.Remove((Classic.ClassicObject) null);
        }
        if (!this.is_title())
          return;
        if (!this.start_game && (this.E.btn(this.k_jump) || this.E.btn(this.k_dash)))
        {
          this.E.music(-1, 0, 0);
          this.start_game_flash = 50;
          this.start_game = true;
          this.E.sfx(38);
        }
        if (!this.start_game)
          return;
        --this.start_game_flash;
        if (this.start_game_flash > -30)
          return;
        this.begin_game();
      }
    }

    public void Draw()
    {
      this.E.pal();
      if (this.start_game)
      {
        int b = 10;
        if (this.start_game_flash > 10)
        {
          if (this.frames % 10 < 5)
            b = 7;
        }
        else
          b = this.start_game_flash <= 5 ? (this.start_game_flash <= 0 ? 0 : 1) : 2;
        if (b < 10)
        {
          this.E.pal(6, b);
          this.E.pal(12, b);
          this.E.pal(13, b);
          this.E.pal(5, b);
          this.E.pal(1, b);
          this.E.pal(7, b);
        }
      }
      int c = 0;
      if (this.flash_bg)
        c = this.frames / 5;
      else if (this.new_bg)
        c = 2;
      this.E.rectfill(0.0f, 0.0f, 128f, 128f, (float) c);
      if (!this.is_title())
      {
        foreach (Classic.Cloud cloud in this.clouds)
        {
          cloud.x += cloud.spd;
          this.E.rectfill(cloud.x, cloud.y, cloud.x + cloud.w, (float) ((double) cloud.y + 4.0 + (1.0 - (double) cloud.w / 64.0) * 12.0), this.new_bg ? 14f : 1f);
          if ((double) cloud.x > 128.0)
          {
            cloud.x = -cloud.w;
            cloud.y = this.E.rnd(120f);
          }
        }
      }
      this.E.map(this.room.X * 16, this.room.Y * 16, 0, 0, 16, 16, 2);
      for (int index = 0; index < this.objects.Count; ++index)
      {
        Classic.ClassicObject classicObject = this.objects[index];
        switch (classicObject)
        {
          case Classic.platform _:
          case Classic.big_chest _:
            this.draw_object(classicObject);
            break;
        }
      }
      this.E.map(this.room.X * 16, this.room.Y * 16, this.is_title() ? -4 : 0, 0, 16, 16, 1);
      for (int index = 0; index < this.objects.Count; ++index)
      {
        Classic.ClassicObject classicObject = this.objects[index];
        switch (classicObject)
        {
          case null:
          case Classic.platform _:
          case Classic.big_chest _:
            continue;
          default:
            this.draw_object(classicObject);
            continue;
        }
      }
      this.E.map(this.room.X * 16, this.room.Y * 16, 0, 0, 16, 16, 3);
      foreach (Classic.Particle particle in this.particles)
      {
        particle.x += particle.spd;
        particle.y += this.E.sin(particle.off);
        particle.off += this.E.min(0.05f, particle.spd / 32f);
        this.E.rectfill(particle.x, particle.y, particle.x + (float) particle.s, particle.y + (float) particle.s, (float) particle.c);
        if ((double) particle.x > 132.0)
        {
          particle.x = -4f;
          particle.y = this.E.rnd(128f);
        }
      }
      for (int index = this.dead_particles.Count - 1; index >= 0; --index)
      {
        Classic.DeadParticle deadParticle = this.dead_particles[index];
        deadParticle.x += deadParticle.spd.X;
        deadParticle.y += deadParticle.spd.Y;
        --deadParticle.t;
        if (deadParticle.t <= 0)
          this.dead_particles.RemoveAt(index);
        this.E.rectfill(deadParticle.x - (float) (deadParticle.t / 5), deadParticle.y - (float) (deadParticle.t / 5), deadParticle.x + (float) (deadParticle.t / 5), deadParticle.y + (float) (deadParticle.t / 5), (float) (14 + deadParticle.t % 2));
      }
      this.E.rectfill(-5f, -5f, -1f, 133f, 0.0f);
      this.E.rectfill(-5f, -5f, 133f, -1f, 0.0f);
      this.E.rectfill(-5f, 128f, 133f, 133f, 0.0f);
      this.E.rectfill(128f, -5f, 133f, 133f, 0.0f);
      if (this.is_title())
        this.E.print("press button", 42f, 96f, 5f);
      if (this.level_index() != 30)
        return;
      Classic.ClassicObject classicObject1 = (Classic.ClassicObject) null;
      foreach (Classic.ClassicObject classicObject2 in this.objects)
      {
        if (classicObject2 is Classic.player)
        {
          classicObject1 = classicObject2;
          break;
        }
      }
      if (classicObject1 == null)
        return;
      float x2 = this.E.min(24f, 40f - this.E.abs((float) ((double) classicObject1.x + 4.0 - 64.0)));
      this.E.rectfill(0.0f, 0.0f, x2, 128f, 0.0f);
      this.E.rectfill(128f - x2, 0.0f, 128f, 128f, 0.0f);
    }

    private void draw_object(Classic.ClassicObject obj) => obj.draw();

    private void draw_time(int x, int y)
    {
      int seconds = this.seconds;
      int num1 = this.minutes % 60;
      int num2 = this.E.flr((float) (this.minutes / 60));
      this.E.rectfill((float) x, (float) y, (float) (x + 32), (float) (y + 6), 0.0f);
      this.E.print((num2 < 10 ? (object) "0" : (object) "").ToString() + (object) num2 + ":" + (num1 < 10 ? (object) "0" : (object) "") + (object) num1 + ":" + (seconds < 10 ? (object) "0" : (object) "") + (object) seconds, (float) (x + 1), (float) (y + 1), 7f);
    }

    private float clamp(float val, float a, float b) => this.E.max(a, this.E.min(b, val));

    private float appr(float val, float target, float amount) => (double) val <= (double) target ? this.E.min(val + amount, target) : this.E.max(val - amount, target);

    private int sign(float v)
    {
      if ((double) v > 0.0)
        return 1;
      return (double) v >= 0.0 ? 0 : -1;
    }

    private bool maybe() => (double) this.E.rnd(1f) < 0.5;

    private bool solid_at(float x, float y, float w, float h) => this.tile_flag_at(x, y, w, h, 0);

    private bool ice_at(float x, float y, float w, float h) => this.tile_flag_at(x, y, w, h, 4);

    private bool tile_flag_at(float x, float y, float w, float h, int flag)
    {
      for (int x1 = (int) this.E.max(0.0f, (float) this.E.flr(x / 8f)); (double) x1 <= (double) this.E.min(15f, (float) (((double) x + (double) w - 1.0) / 8.0)); ++x1)
      {
        for (int y1 = (int) this.E.max(0.0f, (float) this.E.flr(y / 8f)); (double) y1 <= (double) this.E.min(15f, (float) (((double) y + (double) h - 1.0) / 8.0)); ++y1)
        {
          if (this.E.fget(this.tile_at(x1, y1), flag))
            return true;
        }
      }
      return false;
    }

    private int tile_at(int x, int y) => this.E.mget(this.room.X * 16 + x, this.room.Y * 16 + y);

    private bool spikes_at(float x, float y, int w, int h, float xspd, float yspd)
    {
      for (int x1 = (int) this.E.max(0.0f, (float) this.E.flr(x / 8f)); (double) x1 <= (double) this.E.min(15f, (float) (((double) x + (double) w - 1.0) / 8.0)); ++x1)
      {
        for (int y1 = (int) this.E.max(0.0f, (float) this.E.flr(y / 8f)); (double) y1 <= (double) this.E.min(15f, (float) (((double) y + (double) h - 1.0) / 8.0)); ++y1)
        {
          int num = this.tile_at(x1, y1);
          if (num == 17 && ((double) this.E.mod((float) ((double) y + (double) h - 1.0), 8f) >= 6.0 || (double) y + (double) h == (double) (y1 * 8 + 8)) && (double) yspd >= 0.0 || num == 27 && (double) this.E.mod(y, 8f) <= 2.0 && (double) yspd <= 0.0 || num == 43 && (double) this.E.mod(x, 8f) <= 2.0 && (double) xspd <= 0.0 || num == 59 && (((double) x + (double) w - 1.0) % 8.0 >= 6.0 || (double) x + (double) w == (double) (x1 * 8 + 8)) && (double) xspd >= 0.0)
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
      public bool p_jump;
      public bool p_dash;
      public int grace;
      public int jbuffer;
      public int djump;
      public int dash_time;
      public int dash_effect_time;
      public Vector2 dash_target = new Vector2(0.0f, 0.0f);
      public Vector2 dash_accel = new Vector2(0.0f, 0.0f);
      public float spr_off;
      public bool was_on_ground;
      public Classic.player_hair hair;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.spr = 1f;
        this.djump = g.max_djump;
        this.hitbox = new Rectangle(1, 3, 6, 5);
      }

      public override void update()
      {
        if (this.G.pause_player)
          return;
        int ox = this.E.btn(this.G.k_right) ? 1 : (this.E.btn(this.G.k_left) ? -1 : 0);
        if (this.G.spikes_at(this.x + (float) this.hitbox.X, this.y + (float) this.hitbox.Y, this.hitbox.Width, this.hitbox.Height, this.spd.X, this.spd.Y))
          this.G.kill_player(this);
        if ((double) this.y > 128.0)
          this.G.kill_player(this);
        bool flag1 = this.is_solid(0, 1);
        bool flag2 = this.is_ice(0, 1);
        if (flag1 && !this.was_on_ground)
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f);
        int num1 = !this.E.btn(this.G.k_jump) ? 0 : (!this.p_jump ? 1 : 0);
        this.p_jump = this.E.btn(this.G.k_jump);
        if (num1 != 0)
          this.jbuffer = 4;
        else if (this.jbuffer > 0)
          --this.jbuffer;
        bool flag3 = this.E.btn(this.G.k_dash) && !this.p_dash;
        this.p_dash = this.E.btn(this.G.k_dash);
        if (flag1)
        {
          this.grace = 6;
          if (this.djump < this.G.max_djump)
          {
            this.G.psfx(54);
            this.djump = this.G.max_djump;
          }
        }
        else if (this.grace > 0)
          --this.grace;
        --this.dash_effect_time;
        if (this.dash_time > 0)
        {
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
          --this.dash_time;
          this.spd.X = this.G.appr(this.spd.X, this.dash_target.X, this.dash_accel.X);
          this.spd.Y = this.G.appr(this.spd.Y, this.dash_target.Y, this.dash_accel.Y);
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
          if ((double) this.E.abs(this.spd.X) > (double) num2)
            this.spd.X = this.G.appr(this.spd.X, (float) (this.E.sign(this.spd.X) * num2), amount2);
          else
            this.spd.X = this.G.appr(this.spd.X, (float) (ox * num2), amount1);
          if ((double) this.spd.X != 0.0)
            this.flipX = (double) this.spd.X < 0.0;
          float target = 2f;
          float amount3 = 0.21f;
          if ((double) this.E.abs(this.spd.Y) <= 0.15000000596046448)
            amount3 *= 0.5f;
          if (ox != 0 && this.is_solid(ox, 0) && !this.is_ice(ox, 0))
          {
            target = 0.4f;
            if ((double) this.E.rnd(10f) < 2.0)
              this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + (float) (ox * 6), this.y);
          }
          if (!flag1)
            this.spd.Y = this.G.appr(this.spd.Y, target, amount3);
          if (this.jbuffer > 0)
          {
            if (this.grace > 0)
            {
              this.G.psfx(1);
              this.jbuffer = 0;
              this.grace = 0;
              this.spd.Y = -2f;
              this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f);
            }
            else
            {
              int num3 = this.is_solid(-3, 0) ? -1 : (this.is_solid(3, 0) ? 1 : 0);
              if (num3 != 0)
              {
                this.G.psfx(2);
                this.jbuffer = 0;
                this.spd.Y = -2f;
                this.spd.X = (float) (-num3 * (num2 + 1));
                if (!this.is_ice(num3 * 3, 0))
                  this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + (float) (num3 * 6), this.y);
              }
            }
          }
          int num4 = 5;
          float num5 = (float) num4 * 0.70710677f;
          if (this.djump > 0 & flag3)
          {
            this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
            --this.djump;
            this.dash_time = 4;
            this.G.has_dashed = true;
            this.dash_effect_time = 10;
            int num6 = this.E.dashDirectionX(this.flipX ? -1 : 1);
            int num7 = this.E.dashDirectionY(this.flipX ? -1 : 1);
            if (num6 != 0 && num7 != 0)
            {
              this.spd.X = (float) num6 * num5;
              this.spd.Y = (float) num7 * num5;
            }
            else if (num6 != 0)
            {
              this.spd.X = (float) (num6 * num4);
              this.spd.Y = 0.0f;
            }
            else if (num7 != 0)
            {
              this.spd.X = 0.0f;
              this.spd.Y = (float) (num7 * num4);
            }
            else
            {
              this.spd.X = this.flipX ? -1f : 1f;
              this.spd.Y = 0.0f;
            }
            this.G.psfx(3);
            this.G.freeze = 2;
            this.G.shake = 6;
            this.dash_target.X = (float) (2 * this.E.sign(this.spd.X));
            this.dash_target.Y = (float) (2 * this.E.sign(this.spd.Y));
            this.dash_accel.X = 1.5f;
            this.dash_accel.Y = 1.5f;
            if ((double) this.spd.Y < 0.0)
              this.dash_target.Y *= 0.75f;
            if ((double) this.spd.Y != 0.0)
              this.dash_accel.X *= 0.70710677f;
            if ((double) this.spd.X != 0.0)
              this.dash_accel.Y *= 0.70710677f;
          }
          else if (flag3 && this.djump <= 0)
          {
            this.G.psfx(9);
            this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
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
        else if (this.E.btn(this.G.k_down))
          this.spr = 6f;
        else if (this.E.btn(this.G.k_up))
          this.spr = 7f;
        else if ((double) this.spd.X == 0.0 || !this.E.btn(this.G.k_left) && !this.E.btn(this.G.k_right))
          this.spr = 1f;
        else
          this.spr = (float) (1.0 + (double) this.spr_off % 4.0);
        if ((double) this.y < -4.0 && this.G.level_index() < 30)
          this.G.next_room();
        this.was_on_ground = flag1;
      }

      public override void draw()
      {
        if ((double) this.x < -1.0 || (double) this.x > 121.0)
        {
          this.x = this.G.clamp(this.x, -1f, 121f);
          this.spd.X = 0.0f;
        }
        this.hair.draw_hair((Classic.ClassicObject) this, this.flipX ? -1 : 1, this.djump);
        this.G.draw_player((Classic.ClassicObject) this, this.djump);
      }
    }

    public class player_hair
    {
      private Classic.player_hair.node[] hair = new Classic.player_hair.node[5];
      private Emulator E;
      private Classic G;

      public player_hair(Classic.ClassicObject obj)
      {
        this.E = obj.E;
        this.G = obj.G;
        for (int index = 0; index <= 4; ++index)
          this.hair[index] = new Classic.player_hair.node()
          {
            x = obj.x,
            y = obj.y,
            size = this.E.max(1f, this.E.min(2f, (float) (3 - index)))
          };
      }

      public void draw_hair(Classic.ClassicObject obj, int facing, int djump)
      {
        int num;
        switch (djump)
        {
          case 1:
            num = 8;
            break;
          case 2:
            num = 7 + this.E.flr((float) (this.G.frames / 3 % 2)) * 4;
            break;
          default:
            num = 12;
            break;
        }
        int c = num;
        Vector2 vector2 = new Vector2(obj.x + 4f - (float) (facing * 2), obj.y + (this.E.btn(this.G.k_down) ? 4f : 3f));
        foreach (Classic.player_hair.node node in this.hair)
        {
          node.x += (float) (((double) vector2.X - (double) node.x) / 1.5);
          node.y += (float) (((double) vector2.Y + 0.5 - (double) node.y) / 1.5);
          this.E.circfill(node.x, node.y, node.size, (float) c);
          vector2 = new Vector2(node.x, node.y);
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

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.spr = 3f;
        this.target = new Vector2(this.x, this.y);
        this.y = 128f;
        this.spd.Y = -4f;
        this.state = 0;
        this.delay = 0;
        this.solids = false;
        this.hair = new Classic.player_hair((Classic.ClassicObject) this);
        this.E.sfx(4);
      }

      public override void update()
      {
        if (this.state == 0)
        {
          if ((double) this.y >= (double) this.target.Y + 16.0)
            return;
          this.state = 1;
          this.delay = 3;
        }
        else if (this.state == 1)
        {
          this.spd.Y += 0.5f;
          if ((double) this.spd.Y > 0.0 && this.delay > 0)
          {
            this.spd.Y = 0.0f;
            --this.delay;
          }
          if ((double) this.spd.Y <= 0.0 || (double) this.y <= (double) this.target.Y)
            return;
          this.y = this.target.Y;
          this.spd = new Vector2(0.0f, 0.0f);
          this.state = 2;
          this.delay = 5;
          this.G.shake = 5;
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 4f);
          this.E.sfx(5);
        }
        else
        {
          if (this.state != 2)
            return;
          --this.delay;
          this.spr = 6f;
          if (this.delay >= 0)
            return;
          this.G.destroy_object((Classic.ClassicObject) this);
          this.G.init_object<Classic.player>(new Classic.player(), this.x, this.y).hair = this.hair;
        }
      }

      public override void draw()
      {
        this.hair.draw_hair((Classic.ClassicObject) this, 1, this.G.max_djump);
        this.G.draw_player((Classic.ClassicObject) this, this.G.max_djump);
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
          if (player != null && (double) player.spd.Y >= 0.0)
          {
            this.spr = 19f;
            player.y = this.y - 4f;
            player.spd.X *= 0.2f;
            player.spd.Y = -3f;
            player.djump = this.G.max_djump;
            this.delay = 10;
            this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
            Classic.fall_floor fallFloor = this.collide<Classic.fall_floor>(0, 1);
            if (fallFloor != null)
              this.G.break_fall_floor(fallFloor);
            this.G.psfx(8);
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
      private float offset;
      private float start;
      private float timer;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.offset = this.E.rnd(1f);
        this.start = this.y;
        this.hitbox = new Rectangle(-1, -1, 10, 10);
      }

      public override void update()
      {
        if ((double) this.spr == 22.0)
        {
          this.offset += 0.01f;
          this.y = this.start + this.E.sin(this.offset) * 2f;
          Classic.player player = this.collide<Classic.player>(0, 0);
          if (player == null || player.djump >= this.G.max_djump)
            return;
          this.G.psfx(6);
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
          player.djump = this.G.max_djump;
          this.spr = 0.0f;
          this.timer = 60f;
        }
        else if ((double) this.timer > 0.0)
        {
          --this.timer;
        }
        else
        {
          this.G.psfx(7);
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
          this.spr = 22f;
        }
      }

      public override void draw()
      {
        if ((double) this.spr != 22.0)
          return;
        this.E.spr((float) (13.0 + (double) this.offset * 8.0 % 3.0), this.x, this.y + 6f);
        this.E.spr(this.spr, this.x, this.y);
      }
    }

    public class fall_floor : Classic.ClassicObject
    {
      public int state;
      public bool solid = true;
      public int delay;

      public override void update()
      {
        if (this.state == 0)
        {
          if (!this.check<Classic.player>(0, -1) && !this.check<Classic.player>(-1, 0) && !this.check<Classic.player>(1, 0))
            return;
          this.G.break_fall_floor(this);
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
          this.G.psfx(7);
          this.state = 0;
          this.collideable = true;
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
        }
      }

      public override void draw()
      {
        if (this.state == 2)
          return;
        if (this.state != 1)
          this.E.spr(23f, this.x, this.y);
        else
          this.E.spr((float) (23 + (15 - this.delay) / 5), this.x, this.y);
      }
    }

    public class smoke : Classic.ClassicObject
    {
      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.spr = 29f;
        this.spd.Y = -0.1f;
        this.spd.X = 0.3f + this.E.rnd(0.2f);
        this.x += this.E.rnd(2f) - 1f;
        this.y += this.E.rnd(2f) - 1f;
        this.flipX = this.G.maybe();
        this.flipY = this.G.maybe();
        this.solids = false;
      }

      public override void update()
      {
        this.spr += 0.2f;
        if ((double) this.spr < 32.0)
          return;
        this.G.destroy_object((Classic.ClassicObject) this);
      }
    }

    public class fruit : Classic.ClassicObject
    {
      private float start;
      private float off;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.spr = 26f;
        this.start = this.y;
        this.off = 0.0f;
      }

      public override void update()
      {
        Classic.player player = this.collide<Classic.player>(0, 0);
        if (player != null)
        {
          player.djump = this.G.max_djump;
          this.G.sfx_timer = 20;
          this.E.sfx(13);
          this.G.got_fruit.Add(1 + this.G.level_index());
          this.G.init_object<Classic.lifeup>(new Classic.lifeup(), this.x, this.y);
          this.G.destroy_object((Classic.ClassicObject) this);
          Stats.Increment(Stat.PICO_BERRIES);
        }
        ++this.off;
        this.y = this.start + this.E.sin(this.off / 40f) * 2.5f;
      }
    }

    public class fly_fruit : Classic.ClassicObject
    {
      private float start;
      private bool fly;
      private float step = 0.5f;
      private float sfx_delay = 8f;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
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
              this.G.sfx_timer = 20;
              this.E.sfx(14);
            }
          }
          this.spd.Y = this.G.appr(this.spd.Y, -3.5f, 0.25f);
          if ((double) this.y < -16.0)
            this.G.destroy_object((Classic.ClassicObject) this);
        }
        else
        {
          if (this.G.has_dashed)
            this.fly = true;
          this.step += 0.05f;
          this.spd.Y = this.E.sin(this.step) * 0.5f;
        }
        Classic.player player = this.collide<Classic.player>(0, 0);
        if (player == null)
          return;
        player.djump = this.G.max_djump;
        this.G.sfx_timer = 20;
        this.E.sfx(13);
        this.G.got_fruit.Add(1 + this.G.level_index());
        this.G.init_object<Classic.lifeup>(new Classic.lifeup(), this.x, this.y);
        this.G.destroy_object((Classic.ClassicObject) this);
        Stats.Increment(Stat.PICO_BERRIES);
      }

      public override void draw()
      {
        float num = 0.0f;
        if (!this.fly)
        {
          if ((double) this.E.sin(this.step) < 0.0)
            num = 1f + this.E.max(0.0f, (float) this.G.sign(this.y - this.start));
        }
        else
          num = (float) (((double) num + 0.25) % 3.0);
        this.E.spr(45f + num, this.x - 6f, this.y - 2f, flipX: true);
        this.E.spr(this.spr, this.x, this.y);
        this.E.spr(45f + num, this.x + 6f, this.y - 2f);
      }
    }

    public class lifeup : Classic.ClassicObject
    {
      private int duration;
      private float flash;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.spd.Y = -0.25f;
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
        this.G.destroy_object((Classic.ClassicObject) this);
      }

      public override void draw()
      {
        this.flash += 0.5f;
        this.E.print("1000", this.x - 2f, this.y, (float) (7.0 + (double) this.flash % 2.0));
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
          player.spd.X = (float) -this.G.sign(player.spd.X) * 1.5f;
          player.spd.Y = -1.5f;
          player.dash_time = -1;
          this.G.sfx_timer = 20;
          this.E.sfx(16);
          this.G.destroy_object((Classic.ClassicObject) this);
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y);
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y + 8f);
          this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y + 8f);
          this.G.init_object<Classic.fruit>(new Classic.fruit(), this.x + 4f, this.y + 4f);
        }
        this.hitbox = new Rectangle(0, 0, 16, 16);
      }

      public override void draw()
      {
        this.E.spr(64f, this.x, this.y);
        this.E.spr(65f, this.x + 8f, this.y);
        this.E.spr(80f, this.x, this.y + 8f);
        this.E.spr(81f, this.x + 8f, this.y + 8f);
      }
    }

    public class key : Classic.ClassicObject
    {
      public override void update()
      {
        int num1 = this.E.flr(this.spr);
        this.spr = (float) (9.0 + ((double) this.E.sin((float) this.G.frames / 30f) + 0.5) * 1.0);
        int num2 = this.E.flr(this.spr);
        if (num2 == 10 && num2 != num1)
          this.flipX = !this.flipX;
        if (!this.check<Classic.player>(0, 0))
          return;
        this.E.sfx(23);
        this.G.sfx_timer = 20;
        this.G.destroy_object((Classic.ClassicObject) this);
        this.G.has_key = true;
      }
    }

    public class chest : Classic.ClassicObject
    {
      private float start;
      private float timer;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.x -= 4f;
        this.start = this.x;
        this.timer = 20f;
      }

      public override void update()
      {
        if (!this.G.has_key)
          return;
        --this.timer;
        this.x = this.start - 1f + this.E.rnd(3f);
        if ((double) this.timer > 0.0)
          return;
        this.G.sfx_timer = 20;
        this.E.sfx(16);
        this.G.init_object<Classic.fruit>(new Classic.fruit(), this.x, this.y - 4f);
        this.G.destroy_object((Classic.ClassicObject) this);
      }
    }

    public class platform : Classic.ClassicObject
    {
      public float dir;
      private float last;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.x -= 4f;
        this.solids = false;
        this.hitbox.Width = 16;
        this.last = this.x;
      }

      public override void update()
      {
        this.spd.X = this.dir * 0.65f;
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
        this.E.spr(11f, this.x, this.y - 1f);
        this.E.spr(12f, this.x + 8f, this.y - 1f);
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
              this.E.sfx(35);
            }
          }
          Vector2 vector2 = new Vector2(8f, 96f);
          for (int index = 0; (double) index < (double) this.index; ++index)
          {
            if (str[index] != '#')
            {
              this.E.rectfill(vector2.X - 2f, vector2.Y - 2f, vector2.X + 7f, vector2.Y + 6f, 7f);
              this.E.print(str[index].ToString() ?? "", vector2.X, vector2.Y, 0.0f);
              vector2.X += 5f;
            }
            else
            {
              vector2.X = 8f;
              vector2.Y += 7f;
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

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.hitbox.Width = 16;
      }

      public override void draw()
      {
        if (this.state == 0)
        {
          Classic.player player = this.collide<Classic.player>(0, 8);
          if (player != null && player.is_solid(0, 1))
          {
            this.E.music(-1, 500, 7);
            this.E.sfx(37);
            this.G.pause_player = true;
            player.spd.X = 0.0f;
            player.spd.Y = 0.0f;
            this.state = 1;
            this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x, this.y);
            this.G.init_object<Classic.smoke>(new Classic.smoke(), this.x + 8f, this.y);
            this.timer = 60f;
            this.particles = new List<Classic.big_chest.particle>();
          }
          this.E.spr(96f, this.x, this.y);
          this.E.spr(97f, this.x + 8f, this.y);
        }
        else if (this.state == 1)
        {
          --this.timer;
          this.G.shake = 5;
          this.G.flash_bg = true;
          if ((double) this.timer <= 45.0 && this.particles.Count < 50)
            this.particles.Add(new Classic.big_chest.particle()
            {
              x = 1f + this.E.rnd(14f),
              y = 0.0f,
              h = 32f + this.E.rnd(32f),
              spd = 8f + this.E.rnd(8f)
            });
          if ((double) this.timer < 0.0)
          {
            this.state = 2;
            this.particles.Clear();
            this.G.flash_bg = false;
            this.G.new_bg = true;
            this.G.init_object<Classic.orb>(new Classic.orb(), this.x + 4f, this.y + 4f);
            this.G.pause_player = false;
          }
          foreach (Classic.big_chest.particle particle in this.particles)
          {
            particle.y += particle.spd;
            this.E.rectfill(this.x + particle.x, this.y + 8f - particle.y, this.x + particle.x, this.E.min(this.y + 8f - particle.y + particle.h, this.y + 8f), 7f);
          }
        }
        this.E.spr(112f, this.x, this.y + 8f);
        this.E.spr(113f, this.x + 8f, this.y + 8f);
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
      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.spd.Y = -4f;
        this.solids = false;
      }

      public override void draw()
      {
        this.spd.Y = this.G.appr(this.spd.Y, 0.0f, 0.5f);
        Classic.player player = this.collide<Classic.player>(0, 0);
        if ((double) this.spd.Y == 0.0 && player != null)
        {
          this.G.music_timer = 45;
          this.E.sfx(51);
          this.G.freeze = 10;
          this.G.shake = 10;
          this.G.destroy_object((Classic.ClassicObject) this);
          this.G.max_djump = 2;
          player.djump = 2;
        }
        this.E.spr(102f, this.x, this.y);
        float num = (float) this.G.frames / 30f;
        for (int index = 0; index <= 7; ++index)
          this.E.circfill((float) ((double) this.x + 4.0 + (double) this.E.cos(num + (float) index / 8f) * 8.0), (float) ((double) this.y + 4.0 + (double) this.E.sin(num + (float) index / 8f) * 8.0), 1f, 7f);
      }
    }

    public class flag : Classic.ClassicObject
    {
      private float score;
      private bool show;

      public override void init(Classic g, Emulator e)
      {
        base.init(g, e);
        this.x += 5f;
        this.score = (float) this.G.got_fruit.Count;
        Stats.Increment(Stat.PICO_COMPLETES);
        Achievements.Register(Achievement.PICO8);
      }

      public override void draw()
      {
        this.spr = (float) (118.0 + (double) this.G.frames / 5.0 % 3.0);
        this.E.spr(this.spr, this.x, this.y);
        if (this.show)
        {
          this.E.rectfill(32f, 2f, 96f, 31f, 0.0f);
          this.E.spr(26f, 55f, 6f);
          this.E.print("x" + (object) this.score, 64f, 9f, 7f);
          this.G.draw_time(49, 16);
          this.E.print("deaths:" + (object) this.G.deaths, 48f, 24f, 7f);
        }
        else
        {
          if (!this.check<Classic.player>(0, 0))
            return;
          this.E.sfx(55);
          this.G.sfx_timer = 30;
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
          this.G.destroy_object((Classic.ClassicObject) this);
        }
        else
        {
          if ((double) this.delay >= 0.0)
            return;
          this.E.rectfill(24f, 58f, 104f, 70f, 0.0f);
          if (this.G.room.X == 3 && this.G.room.Y == 1)
            this.E.print("old site", 48f, 62f, 7f);
          else if (this.G.level_index() == 30)
          {
            this.E.print("summit", 52f, 62f, 7f);
          }
          else
          {
            int num = (1 + this.G.level_index()) * 100;
            this.E.print(num.ToString() + "m", (float) (52 + (num < 1000 ? 2 : 0)), 62f, 7f);
          }
          this.G.draw_time(4, 4);
        }
      }
    }

    public class ClassicObject
    {
      public Classic G;
      public Emulator E;
      public int type;
      public bool collideable = true;
      public bool solids = true;
      public float spr;
      public bool flipX;
      public bool flipY;
      public float x;
      public float y;
      public Rectangle hitbox = new Rectangle(0, 0, 8, 8);
      public Vector2 spd = new Vector2(0.0f, 0.0f);
      public Vector2 rem = new Vector2(0.0f, 0.0f);

      public virtual void init(Classic g, Emulator e)
      {
        this.G = g;
        this.E = e;
      }

      public virtual void update()
      {
      }

      public virtual void draw()
      {
        if ((double) this.spr <= 0.0)
          return;
        this.E.spr(this.spr, this.x, this.y, flipX: this.flipX, flipY: this.flipY);
      }

      public bool is_solid(int ox, int oy) => oy > 0 && !this.check<Classic.platform>(ox, 0) && this.check<Classic.platform>(ox, oy) || this.G.solid_at(this.x + (float) this.hitbox.X + (float) ox, this.y + (float) this.hitbox.Y + (float) oy, (float) this.hitbox.Width, (float) this.hitbox.Height) || this.check<Classic.fall_floor>(ox, oy) || this.check<Classic.fake_wall>(ox, oy);

      public bool is_ice(int ox, int oy) => this.G.ice_at(this.x + (float) this.hitbox.X + (float) ox, this.y + (float) this.hitbox.Y + (float) oy, (float) this.hitbox.Width, (float) this.hitbox.Height);

      public T collide<T>(int ox, int oy) where T : Classic.ClassicObject
      {
        Type type = typeof (T);
        foreach (Classic.ClassicObject classicObject in this.G.objects)
        {
          if (classicObject != null && classicObject.GetType() == type && classicObject != this && classicObject.collideable && (double) classicObject.x + (double) classicObject.hitbox.X + (double) classicObject.hitbox.Width > (double) this.x + (double) this.hitbox.X + (double) ox && (double) classicObject.y + (double) classicObject.hitbox.Y + (double) classicObject.hitbox.Height > (double) this.y + (double) this.hitbox.Y + (double) oy && (double) classicObject.x + (double) classicObject.hitbox.X < (double) this.x + (double) this.hitbox.X + (double) this.hitbox.Width + (double) ox && (double) classicObject.y + (double) classicObject.hitbox.Y < (double) this.y + (double) this.hitbox.Y + (double) this.hitbox.Height + (double) oy)
            return classicObject as T;
        }
        return default (T);
      }

      public bool check<T>(int ox, int oy) where T : Classic.ClassicObject => (object) this.collide<T>(ox, oy) != null;

      public void move(float ox, float oy)
      {
        this.rem.X += ox;
        int amount1 = this.E.flr(this.rem.X + 0.5f);
        this.rem.X -= (float) amount1;
        this.move_x(amount1, 0);
        this.rem.Y += oy;
        int amount2 = this.E.flr(this.rem.Y + 0.5f);
        this.rem.Y -= (float) amount2;
        this.move_y(amount2);
      }

      public void move_x(int amount, int start)
      {
        if (this.solids)
        {
          int ox = this.G.sign((float) amount);
          for (int index = start; (double) index <= (double) this.E.abs((float) amount); ++index)
          {
            if (!this.is_solid(ox, 0))
            {
              this.x += (float) ox;
            }
            else
            {
              this.spd.X = 0.0f;
              this.rem.X = 0.0f;
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
          int oy = this.G.sign((float) amount);
          for (int index = 0; (double) index <= (double) this.E.abs((float) amount); ++index)
          {
            if (!this.is_solid(0, oy))
            {
              this.y += (float) oy;
            }
            else
            {
              this.spd.Y = 0.0f;
              this.rem.Y = 0.0f;
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
