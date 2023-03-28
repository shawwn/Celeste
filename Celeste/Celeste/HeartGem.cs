// Decompiled with JetBrains decompiler
// Type: Celeste.HeartGem
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
    [Tracked(false)]
    public class HeartGem : Entity
    {
        private const string FAKE_HEART_FLAG = "fake_heart";
        public static ParticleType P_BlueShine;
        public static ParticleType P_RedShine;
        public static ParticleType P_GoldShine;
        public static ParticleType P_FakeShine;
        public bool IsGhost;
        public const float GhostAlpha = 0.8f;
        public bool IsFake;
        private Sprite sprite;
        private Sprite white;
        private ParticleType shineParticle;
        public Wiggler ScaleWiggler;
        private Wiggler moveWiggler;
        private Vector2 moveWiggleDir;
        private BloomPoint bloom;
        private VertexLight light;
        private Poem poem;
        private BirdNPC bird;
        private float timer;
        private bool collected;
        private bool autoPulse = true;
        private float bounceSfxDelay;
        private bool removeCameraTriggers;
        private SoundEmitter sfx;
        private List<InvisibleBarrier> walls = new List<InvisibleBarrier>();
        private HoldableCollider holdableCollider;
        private EntityID entityID;
        private InvisibleBarrier fakeRightWall;

        public HeartGem(Vector2 position)
            : base(position)
        {
            this.Add((Component) (this.holdableCollider = new HoldableCollider(new Action<Holdable>(this.OnHoldable))));
            this.Add((Component) new MirrorReflection());
        }

        public HeartGem(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
            this.removeCameraTriggers = data.Bool(nameof (removeCameraTriggers));
            this.IsFake = data.Bool("fake");
            this.entityID = new EntityID(data.Level.Name, data.ID);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            AreaKey area = (this.Scene as Level).Session.Area;
            this.IsGhost = !this.IsFake && SaveData.Instance.Areas[area.ID].Modes[(int) area.Mode].HeartGem;
            string id = !this.IsFake ? (!this.IsGhost ? "heartgem" + (object) (int) area.Mode : "heartGemGhost") : "heartgem3";
            this.Add((Component) (this.sprite = GFX.SpriteBank.Create(id)));
            this.sprite.Play("spin");
            this.sprite.OnLoop = (Action<string>) (anim =>
            {
                if (!this.Visible || !(anim == "spin") || !this.autoPulse)
                    return;
                if (this.IsFake)
                    Audio.Play("event:/new_content/game/10_farewell/fakeheart_pulse", this.Position);
                else
                    Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
                this.ScaleWiggler.Start();
                (this.Scene as Level).Displacement.AddBurst(this.Position, 0.35f, 8f, 48f, 0.25f);
            });
            if (this.IsGhost)
                this.sprite.Color = Color.White * 0.8f;
            this.Collider = (Collider) new Hitbox(16f, 16f, -8f, -8f);
            this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
            this.Add((Component) (this.ScaleWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.25)))));
            this.Add((Component) (this.bloom = new BloomPoint(0.75f, 16f)));
            Color color;
            if (this.IsFake)
            {
                color = Calc.HexToColor("dad8cc");
                this.shineParticle = HeartGem.P_FakeShine;
            }
            else if (area.Mode == AreaMode.Normal)
            {
                color = Color.Aqua;
                this.shineParticle = HeartGem.P_BlueShine;
            }
            else if (area.Mode == AreaMode.BSide)
            {
                color = Color.Red;
                this.shineParticle = HeartGem.P_RedShine;
            }
            else
            {
                color = Color.Gold;
                this.shineParticle = HeartGem.P_GoldShine;
            }
            this.Add((Component) (this.light = new VertexLight(Color.Lerp(color, Color.White, 0.5f), 1f, 32, 64)));
            if (this.IsFake)
            {
                this.bloom.Alpha = 0.0f;
                this.light.Alpha = 0.0f;
            }
            this.moveWiggler = Wiggler.Create(0.8f, 2f);
            this.moveWiggler.StartZero = true;
            this.Add((Component) this.moveWiggler);
            if (!this.IsFake)
                return;
            Player entity = this.Scene.Tracker.GetEntity<Player>();
            if (entity != null && (double) entity.X > (double) this.X || (scene as Level).Session.GetFlag("fake_heart"))
            {
                this.Visible = false;
                Alarm.Set((Entity) this, 0.0001f, (Action) (() =>
                {
                    this.FakeRemoveCameraTrigger();
                    this.RemoveSelf();
                }));
            }
            else
                scene.Add((Entity) (this.fakeRightWall = new InvisibleBarrier(new Vector2(this.X + 160f, this.Y - 200f), 8f, 400f)));
        }

        public override void Update()
        {
            this.bounceSfxDelay -= Engine.DeltaTime;
            this.timer += Engine.DeltaTime;
            this.sprite.Position = Vector2.UnitY * (float) Math.Sin((double) this.timer * 2.0) * 2f + this.moveWiggleDir * this.moveWiggler.Value * -8f;
            if (this.white != null)
            {
                this.white.Position = this.sprite.Position;
                this.white.Scale = this.sprite.Scale;
                if (this.white.CurrentAnimationID != this.sprite.CurrentAnimationID)
                    this.white.Play(this.sprite.CurrentAnimationID);
                this.white.SetAnimationFrame(this.sprite.CurrentAnimationFrame);
            }
            if (this.collected)
            {
                Player entity = this.Scene.Tracker.GetEntity<Player>();
                if (entity == null || entity.Dead)
                    this.EndCutscene();
            }
            base.Update();
            if (this.collected || !this.Scene.OnInterval(0.1f))
                return;
            this.SceneAs<Level>().Particles.Emit(this.shineParticle, 1, this.Center, Vector2.One * 8f);
        }

        public void OnHoldable(Holdable h)
        {
            Player entity = this.Scene.Tracker.GetEntity<Player>();
            if (this.collected || entity == null || !h.Dangerous(this.holdableCollider))
                return;
            this.Collect(entity);
        }

        public void OnPlayer(Player player)
        {
            if (this.collected || (this.Scene as Level).Frozen)
                return;
            if (player.DashAttacking)
            {
                this.Collect(player);
            }
            else
            {
                if ((double) this.bounceSfxDelay <= 0.0)
                {
                    if (this.IsFake)
                        Audio.Play("event:/new_content/game/10_farewell/fakeheart_bounce", this.Position);
                    else
                        Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
                    this.bounceSfxDelay = 0.1f;
                }
                player.PointBounce(this.Center);
                this.moveWiggler.Start();
                this.ScaleWiggler.Start();
                this.moveWiggleDir = (this.Center - player.Center).SafeNormalize(Vector2.UnitY);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
        }

        private void Collect(Player player)
        {
            this.Scene.Tracker.GetEntity<AngryOshiro>()?.StopControllingTime();
            this.Add((Component) new Coroutine(this.CollectRoutine(player))
            {
                UseRawDeltaTime = true
            });
            this.collected = true;
            if (!this.removeCameraTriggers)
                return;
            foreach (Entity entity in this.Scene.Entities.FindAll<CameraOffsetTrigger>())
                entity.RemoveSelf();
        }

        private IEnumerator CollectRoutine(Player player)
        {
            HeartGem follow = this;
            Level level = follow.Scene as Level;
            AreaKey area = level.Session.Area;
            string poemID = AreaData.Get((Scene) level).Mode[(int) area.Mode].PoemID;
            bool completeArea = !follow.IsFake && (area.Mode != AreaMode.Normal || area.ID == 9);
            if (follow.IsFake)
                level.StartCutscene(new Action<Level>(follow.SkipFakeHeartCutscene));
            else
                level.CanRetry = false;
            if (completeArea || follow.IsFake)
            {
                Audio.SetMusic((string) null);
                Audio.SetAmbience((string) null);
            }
            if (completeArea)
            {
                List<Strawberry> strawberryList = new List<Strawberry>();
                foreach (Follower follower in player.Leader.Followers)
                {
                    if (follower.Entity is Strawberry)
                        strawberryList.Add(follower.Entity as Strawberry);
                }
                foreach (Strawberry strawberry in strawberryList)
                    strawberry.OnCollect();
            }
            string sfx = "event:/game/general/crystalheart_blue_get";
            if (follow.IsFake)
                sfx = "event:/new_content/game/10_farewell/fakeheart_get";
            else if (area.Mode == AreaMode.BSide)
                sfx = "event:/game/general/crystalheart_red_get";
            else if (area.Mode == AreaMode.CSide)
                sfx = "event:/game/general/crystalheart_gold_get";
            follow.sfx = SoundEmitter.Play(sfx, (Entity) follow);
            // ISSUE: reference to a compiler-generated method
            // follow.Add((Component) new LevelEndingHook(new Action(follow.\u003CCollectRoutine\u003Eb__35_0)));
            follow.Add((Component)new LevelEndingHook((Action)(() => this.sfx.Source.Stop(true))));
            follow.walls.Add(new InvisibleBarrier(new Vector2((float) level.Bounds.Right, (float) level.Bounds.Top), 8f, (float) level.Bounds.Height));
            List<InvisibleBarrier> walls1 = follow.walls;
            Rectangle bounds = level.Bounds;
            double x = (double) (bounds.Left - 8);
            bounds = level.Bounds;
            double top = (double) bounds.Top;
            InvisibleBarrier invisibleBarrier1 = new InvisibleBarrier(new Vector2((float) x, (float) top), 8f, (float) level.Bounds.Height);
            walls1.Add(invisibleBarrier1);
            List<InvisibleBarrier> walls2 = follow.walls;
            bounds = level.Bounds;
            double left = (double) bounds.Left;
            bounds = level.Bounds;
            double y = (double) (bounds.Top - 8);
            InvisibleBarrier invisibleBarrier2 = new InvisibleBarrier(new Vector2((float) left, (float) y), (float) level.Bounds.Width, 8f);
            walls2.Add(invisibleBarrier2);
            foreach (InvisibleBarrier wall in follow.walls)
                follow.Scene.Add((Entity) wall);
            follow.Add((Component) (follow.white = GFX.SpriteBank.Create("heartGemWhite")));
            follow.Depth = -2000000;
            yield return (object) null;
            Celeste.Freeze(0.2f);
            yield return (object) null;
            Engine.TimeRate = 0.5f;
            player.Depth = -2000000;
            for (int index = 0; index < 10; ++index)
                follow.Scene.Add((Entity) new AbsorbOrb(follow.Position));
            level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            level.Flash(Color.White);
            level.FormationBackdrop.Display = true;
            level.FormationBackdrop.Alpha = 1f;
            follow.light.Alpha = follow.bloom.Alpha = 0.0f;
            follow.Visible = false;
            float t;
            for (t = 0.0f; (double) t < 2.0; t += Engine.RawDeltaTime)
            {
                Engine.TimeRate = Calc.Approach(Engine.TimeRate, 0.0f, Engine.RawDeltaTime * 0.25f);
                yield return (object) null;
            }
            yield return (object) null;
            if (player.Dead)
                yield return (object) 100f;
            Engine.TimeRate = 1f;
            follow.Tag = (int) Tags.FrozenUpdate;
            level.Frozen = true;
            if (!follow.IsFake)
            {
                follow.RegisterAsCollected(level, poemID);
                if (completeArea)
                {
                    level.TimerStopped = true;
                    level.RegisterAreaComplete();
                }
            }
            string text = (string) null;
            if (!string.IsNullOrEmpty(poemID))
                text = Dialog.Clean("poem_" + poemID);
            follow.poem = new Poem(text, follow.IsFake ? 3 : (int) area.Mode, area.Mode == AreaMode.CSide || follow.IsFake ? 1f : 0.6f);
            follow.poem.Alpha = 0.0f;
            follow.Scene.Add((Entity) follow.poem);
            for (t = 0.0f; (double) t < 1.0; t += Engine.RawDeltaTime)
            {
                follow.poem.Alpha = Ease.CubeOut(t);
                yield return (object) null;
            }
            if (follow.IsFake)
            {
                yield return (object) follow.DoFakeRoutineWithBird(player);
            }
            else
            {
                while (!Input.MenuConfirm.Pressed && !Input.MenuCancel.Pressed)
                    yield return (object) null;
                follow.sfx.Source.Param("end", 1f);
                if (!completeArea)
                {
                    level.FormationBackdrop.Display = false;
                    for (t = 0.0f; (double) t < 1.0; t += Engine.RawDeltaTime * 2f)
                    {
                        follow.poem.Alpha = Ease.CubeIn(1f - t);
                        yield return (object) null;
                    }
                    player.Depth = 0;
                    follow.EndCutscene();
                }
                else
                {
                    FadeWipe fadeWipe = new FadeWipe((Scene) level, false);
                    fadeWipe.Duration = 3.25f;
                    yield return (object) fadeWipe.Duration;
                    level.CompleteArea(false, true);
                }
            }
        }

        private void EndCutscene()
        {
            Level scene = this.Scene as Level;
            scene.Frozen = false;
            scene.CanRetry = true;
            scene.FormationBackdrop.Display = false;
            Engine.TimeRate = 1f;
            if (this.poem != null)
                this.poem.RemoveSelf();
            foreach (Entity wall in this.walls)
                wall.RemoveSelf();
            this.RemoveSelf();
        }

        private void RegisterAsCollected(Level level, string poemID)
        {
            level.Session.HeartGem = true;
            level.Session.UpdateLevelStartDashes();
            int unlockedModes = SaveData.Instance.UnlockedModes;
            SaveData.Instance.RegisterHeartGem(level.Session.Area);
            if (!string.IsNullOrEmpty(poemID))
                SaveData.Instance.RegisterPoemEntry(poemID);
            if (unlockedModes < 3 && SaveData.Instance.UnlockedModes >= 3)
                level.Session.UnlockedCSide = true;
            if (SaveData.Instance.TotalHeartGems < 24)
                return;
            Achievements.Register(Achievement.CSIDES);
        }

        private IEnumerator DoFakeRoutineWithBird(Player player)
        {
            HeartGem heartGem = this;
            Level level = heartGem.Scene as Level;
            int panAmount = 64;
            Vector2 panFrom = level.Camera.Position;
            Vector2 panTo = level.Camera.Position + new Vector2((float) -panAmount, 0.0f);
            Vector2 birdFrom = new Vector2(panTo.X - 16f, player.Y - 20f);
            Vector2 birdTo = new Vector2((float) ((double) panFrom.X + 320.0 + 16.0), player.Y - 20f);
            yield return (object) 2f;
            Glitch.Value = 0.75f;
            while ((double) Glitch.Value > 0.0)
            {
                Glitch.Value = Calc.Approach(Glitch.Value, 0.0f, Engine.RawDeltaTime * 4f);
                level.Shake();
                yield return (object) null;
            }
            yield return (object) 1.1f;
            Glitch.Value = 0.75f;
            while ((double) Glitch.Value > 0.0)
            {
                Glitch.Value = Calc.Approach(Glitch.Value, 0.0f, Engine.RawDeltaTime * 4f);
                level.Shake();
                yield return (object) null;
            }
            yield return (object) 0.4f;
            float p;
            for (p = 0.0f; (double) p < 1.0; p += Engine.RawDeltaTime / 2f)
            {
                level.Camera.Position = panFrom + (panTo - panFrom) * Ease.CubeInOut(p);
                heartGem.poem.Offset = new Vector2((float) (panAmount * 8), 0.0f) * Ease.CubeInOut(p);
                yield return (object) null;
            }
            heartGem.bird = new BirdNPC(birdFrom, BirdNPC.Modes.None);
            heartGem.bird.Sprite.Play("fly");
            heartGem.bird.Sprite.UseRawDeltaTime = true;
            heartGem.bird.Facing = Facings.Right;
            heartGem.bird.Depth = -2000100;
            heartGem.bird.Tag = (int) Tags.FrozenUpdate;
            heartGem.bird.Add((Component) new VertexLight(Color.White, 0.5f, 8, 32));
            heartGem.bird.Add((Component) new BloomPoint(0.5f, 12f));
            level.Add((Entity) heartGem.bird);
            for (p = 0.0f; (double) p < 1.0; p += Engine.RawDeltaTime / 2.6f)
            {
                level.Camera.Position = panTo + (panFrom - panTo) * Ease.CubeInOut(p);
                heartGem.poem.Offset = new Vector2((float) (panAmount * 8), 0.0f) * Ease.CubeInOut(1f - p);
                float num1 = 0.1f;
                float num2 = 0.9f;
                if ((double) p > (double) num1 && (double) p <= (double) num2)
                {
                    float num3 = (float) (((double) p - (double) num1) / ((double) num2 - (double) num1));
                    heartGem.bird.Position = birdFrom + (birdTo - birdFrom) * num3 + Vector2.UnitY * (float) Math.Sin((double) num3 * 8.0) * 8f;
                }
                if (level.OnRawInterval(0.2f))
                    TrailManager.Add((Entity) heartGem.bird, Calc.HexToColor("639bff"), frozenUpdate: true, useRawDeltaTime: true);
                yield return (object) null;
            }
            heartGem.bird.RemoveSelf();
            heartGem.bird = (BirdNPC) null;
            Engine.TimeRate = 0.0f;
            level.Frozen = false;
            player.Active = false;
            player.StateMachine.State = 11;
            while ((double) Engine.TimeRate != 1.0)
            {
                Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 0.5f * Engine.RawDeltaTime);
                yield return (object) null;
            }
            Engine.TimeRate = 1f;
            yield return (object) Textbox.Say("CH9_FAKE_HEART");
            heartGem.sfx.Source.Param("end", 1f);
            yield return (object) 0.283f;
            level.FormationBackdrop.Display = false;
            for (p = 0.0f; (double) p < 1.0; p += Engine.RawDeltaTime / 0.2f)
            {
                heartGem.poem.TextAlpha = Ease.CubeIn(1f - p);
                heartGem.poem.ParticleSpeed = heartGem.poem.TextAlpha;
                yield return (object) null;
            }
            heartGem.poem.Heart.Play("break");
            while (heartGem.poem.Heart.Animating)
            {
                heartGem.poem.Shake += Engine.DeltaTime;
                yield return (object) null;
            }
            heartGem.poem.RemoveSelf();
            heartGem.poem = (Poem) null;
            for (int index = 0; index < 10; ++index)
            {
                Vector2 position = level.Camera.Position + new Vector2(320f, 180f) * 0.5f;
                Vector2 vector2 = level.Camera.Position + new Vector2(160f, -64f);
                heartGem.Scene.Add((Entity) new AbsorbOrb(position, absorbTarget: new Vector2?(vector2)));
            }
            level.Shake();
            Glitch.Value = 0.8f;
            while ((double) Glitch.Value > 0.0)
            {
                Glitch.Value -= Engine.DeltaTime * 4f;
                yield return (object) null;
            }
            yield return (object) 0.25f;
            level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/intermission_heartgroove";
            level.Session.Audio.Apply();
            player.Active = true;
            player.Depth = 0;
            player.StateMachine.State = 11;
            while (!player.OnGround() && (double) player.Bottom < (double) level.Bounds.Bottom)
                yield return (object) null;
            player.Facing = Facings.Right;
            yield return (object) 0.5f;
            yield return (object) Textbox.Say("CH9_KEEP_GOING", new Func<IEnumerator>(heartGem.PlayerStepForward));
            heartGem.SkipFakeHeartCutscene(level);
            level.EndCutscene();
        }

        private IEnumerator PlayerStepForward()
        {
            HeartGem heartGem = this;
            yield return (object) 0.1f;
            Player entity = heartGem.Scene.Tracker.GetEntity<Player>();
            if (entity != null && entity.CollideCheck<Solid>(entity.Position + new Vector2(12f, 1f)))
                yield return (object) entity.DummyWalkToExact((int) entity.X + 10);
            yield return (object) 0.2f;
        }

        private void SkipFakeHeartCutscene(Level level)
        {
            Engine.TimeRate = 1f;
            Glitch.Value = 0.0f;
            if (this.sfx != null)
                this.sfx.Source.Stop();
            level.Session.SetFlag("fake_heart");
            level.Frozen = false;
            level.FormationBackdrop.Display = false;
            level.Session.Audio.Music.Event = "event:/new_content/music/lvl10/intermission_heartgroove";
            level.Session.Audio.Apply();
            Player entity1 = this.Scene.Tracker.GetEntity<Player>();
            if (entity1 != null)
            {
                entity1.Sprite.Play("idle");
                entity1.Active = true;
                entity1.StateMachine.State = 0;
                entity1.Dashes = 1;
                entity1.Speed = Vector2.Zero;
                entity1.MoveV(200f);
                entity1.Depth = 0;
                for (int index = 0; index < 10; ++index)
                    entity1.UpdateHair(true);
            }
            foreach (Entity entity2 in this.Scene.Entities.FindAll<AbsorbOrb>())
                entity2.RemoveSelf();
            if (this.poem != null)
                this.poem.RemoveSelf();
            if (this.bird != null)
                this.bird.RemoveSelf();
            if (this.fakeRightWall != null)
                this.fakeRightWall.RemoveSelf();
            this.FakeRemoveCameraTrigger();
            foreach (Entity wall in this.walls)
                wall.RemoveSelf();
            this.RemoveSelf();
        }

        private void FakeRemoveCameraTrigger()
        {
            CameraTargetTrigger cameraTargetTrigger = this.CollideFirst<CameraTargetTrigger>();
            if (cameraTargetTrigger == null)
                return;
            cameraTargetTrigger.LerpStrength = 0.0f;
        }
    }
}