// Decompiled with JetBrains decompiler
// Type: Celeste.MountainRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;

namespace Celeste
{
  public class MountainRenderer : Monocle.Renderer
  {
    public static readonly Vector3 RotateLookAt = new Vector3(0.0f, 7f, 0.0f);
    private bool rotateAroundCenter = false;
    private float rotateTimer = 1.570796f;
    public bool AllowUserRotation = true;
    private float percent = 1f;
    private float duration = 1f;
    private float timer = 0.0f;
    private float door = 0.0f;
    public bool ForceNearFog;
    public Action OnEaseEnd;
    private const float rotateDistance = 15f;
    private const float rotateYPosition = 3f;
    private bool rotateAroundTarget;
    private const float DurationDivisor = 3f;
    public MountainCamera UntiltedCamera;
    public MountainModel Model;
    private Vector2 userOffset;
    private bool inFreeCameraDebugMode;
    private MountainCamera easeCameraFrom;
    private MountainCamera easeCameraTo;
    private float easeCameraRotationAngleTo;

    public MountainCamera Camera
    {
      get
      {
        return this.Model.Camera;
      }
    }

    public bool Animating { get; private set; }

    public int Area { get; private set; }

    public MountainRenderer()
    {
      this.Model = new MountainModel();
      this.GotoRotationMode();
    }

    public void Dispose()
    {
      this.Model.Dispose();
    }

    public override void Update(Scene scene)
    {
      this.timer += Engine.DeltaTime;
      this.Model.Update();
      this.userOffset += ((this.AllowUserRotation ? -Input.MountainAim.Value * 0.8f : Vector2.Zero) - this.userOffset) * (1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime));
      if (!this.rotateAroundCenter)
      {
        if (this.Area == 8)
          this.userOffset.Y = Math.Max(0.0f, this.userOffset.Y);
        if (this.Area == 7)
          this.userOffset.X = Calc.Clamp(this.userOffset.X, -0.4f, 0.4f);
      }
      if (!this.inFreeCameraDebugMode)
      {
        if (this.rotateAroundCenter)
        {
          this.rotateTimer -= Engine.DeltaTime * 0.1f;
          this.Model.Camera.Position += (new Vector3((float) Math.Cos((double) this.rotateTimer) * 15f, 3f, (float) Math.Sin((double) this.rotateTimer) * 15f) - this.Model.Camera.Position) * (1f - (float) Math.Pow(0.100000001490116, (double) Engine.DeltaTime));
          this.Model.Camera.Target = MountainRenderer.RotateLookAt + Vector3.Up * this.userOffset.Y;
          this.Model.Camera.Rotation = Quaternion.Slerp(this.Model.Camera.Rotation, new Quaternion().LookAt(this.Model.Camera.Position, this.Model.Camera.Target, Vector3.Up), Engine.DeltaTime * 4f);
          this.UntiltedCamera = this.Camera;
        }
        else
        {
          if (this.Animating)
          {
            this.percent = Calc.Approach(this.percent, 1f, Engine.DeltaTime / this.duration);
            float num1 = Ease.SineOut(this.percent);
            this.Model.Camera.Position = this.GetBetween(this.easeCameraFrom.Position, this.easeCameraTo.Position, num1);
            this.Model.Camera.Target = this.GetBetween(this.easeCameraFrom.Target, this.easeCameraTo.Target, num1);
            Vector3 vector1 = this.easeCameraFrom.Rotation.Forward();
            Vector3 vector2 = this.easeCameraTo.Rotation.Forward();
            Vector2 vector2_1 = vector1.XZ();
            double num2 = (double) vector2_1.Length();
            vector2_1 = vector2.XZ();
            double num3 = (double) vector2_1.Length();
            double num4 = (double) num1;
            float length = Calc.LerpClamp((float) num2, (float) num3, (float) num4);
            Vector2 vector3 = Calc.AngleToVector(MathHelper.Lerp(vector1.XZ().Angle(), this.easeCameraRotationAngleTo, num1), length);
            float y = Calc.LerpClamp(vector1.Y, vector2.Y, num1);
            this.Model.Camera.Rotation = new Quaternion().LookAt(new Vector3(vector3.X, y, vector3.Y), Vector3.Up);
            if ((double) this.percent >= 1.0)
            {
              this.rotateTimer = new Vector2(this.Model.Camera.Position.X, this.Model.Camera.Position.Z).Angle();
              this.Animating = false;
              if (this.OnEaseEnd != null)
                this.OnEaseEnd();
            }
          }
          else if (this.rotateAroundTarget)
          {
            this.rotateTimer -= Engine.DeltaTime * 0.1f;
            float num = (new Vector2(this.easeCameraTo.Target.X, this.easeCameraTo.Target.Z) - new Vector2(this.easeCameraTo.Position.X, this.easeCameraTo.Position.Z)).Length();
            this.Model.Camera.Position += (new Vector3(this.easeCameraTo.Target.X + (float) Math.Cos((double) this.rotateTimer) * num, this.easeCameraTo.Position.Y, this.easeCameraTo.Target.Z + (float) Math.Sin((double) this.rotateTimer) * num) - this.Model.Camera.Position) * (1f - (float) Math.Pow(0.100000001490116, (double) Engine.DeltaTime));
            this.Model.Camera.Target = this.easeCameraTo.Target + Vector3.Up * this.userOffset.Y * 2f + Vector3.Left * this.userOffset.X * 2f;
            this.Model.Camera.Rotation = Quaternion.Slerp(this.Model.Camera.Rotation, new Quaternion().LookAt(this.Model.Camera.Position, this.Model.Camera.Target, Vector3.Up), Engine.DeltaTime * 4f);
            this.UntiltedCamera = this.Camera;
          }
          else
          {
            this.Model.Camera.Rotation = this.easeCameraTo.Rotation;
            this.Model.Camera.Target = this.easeCameraTo.Target;
          }
          this.UntiltedCamera = this.Camera;
          if (this.userOffset != Vector2.Zero && !this.rotateAroundTarget)
            this.Model.Camera.LookAt(this.Model.Camera.Position + this.Model.Camera.Rotation.Forward() + this.Model.Camera.Rotation.Left() * this.userOffset.X * 0.25f + this.Model.Camera.Rotation.Up() * this.userOffset.Y * 0.25f);
        }
      }
      else
      {
        Vector3 to = Vector3.Transform(Vector3.Forward, this.Model.Camera.Rotation.Conjugated());
        this.Model.Camera.Rotation = this.Model.Camera.Rotation.LookAt(Vector3.Zero, to, Vector3.Up);
        Vector3 axis = Vector3.Transform(Vector3.Left, this.Model.Camera.Rotation.Conjugated());
        Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 0.0f);
        if (MInput.Keyboard.Check(Keys.W))
          vector3_1 += to;
        if (MInput.Keyboard.Check(Keys.S))
          vector3_1 -= to;
        if (MInput.Keyboard.Check(Keys.D))
          vector3_1 -= axis;
        if (MInput.Keyboard.Check(Keys.A))
          vector3_1 += axis;
        if (MInput.Keyboard.Check(Keys.Q))
          vector3_1 += Vector3.Up;
        if (MInput.Keyboard.Check(Keys.Z))
          vector3_1 += Vector3.Down;
        this.Model.Camera.Position += vector3_1 * (MInput.Keyboard.Check(Keys.LeftShift) ? 0.5f : 5f) * Engine.DeltaTime;
        if (MInput.Mouse.CheckLeftButton)
        {
          MouseState state = Mouse.GetState();
          int x = Engine.Graphics.GraphicsDevice.Viewport.Width / 2;
          int y = Engine.Graphics.GraphicsDevice.Viewport.Height / 2;
          int num1 = state.X - x;
          int num2 = state.Y - y;
          this.Model.Camera.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, (float) num1 * 0.1f * Engine.DeltaTime);
          this.Model.Camera.Rotation *= Quaternion.CreateFromAxisAngle(axis, (float) -num2 * 0.1f * Engine.DeltaTime);
          Mouse.SetPosition(x, y);
        }
        if (this.Area >= 0)
        {
          Vector3 target = AreaData.Areas[this.Area].MountainIdle.Target;
          Vector3 vector3_2 = axis * 0.05f;
          Vector3 vector3_3 = Vector3.Up * 0.05f;
          this.Model.DebugPoints.Clear();
          this.Model.DebugPoints.Add(new VertexPositionColor(target - vector3_2 + vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target + vector3_2 + vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target + vector3_2 - vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target - vector3_2 + vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target + vector3_2 - vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target - vector3_2 - vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target - vector3_2 * 0.25f - vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target + vector3_2 * 0.25f - vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target + vector3_2 * 0.25f + Vector3.Down * 100f, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target - vector3_2 * 0.25f - vector3_3, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target + vector3_2 * 0.25f + Vector3.Down * 100f, Color.Red));
          this.Model.DebugPoints.Add(new VertexPositionColor(target - vector3_2 * 0.25f + Vector3.Down * 100f, Color.Red));
        }
      }
      this.door = Calc.Approach(this.door, this.Area != 9 || this.rotateAroundCenter ? 0.0f : 1f, Engine.DeltaTime * 1f);
      this.Model.CoreWallPosition = Vector3.Lerp(Vector3.Zero, -new Vector3(-1.5f, 1.5f, 1f), Ease.CubeInOut(this.door));
      this.Model.NearFogAlpha = Calc.Approach(this.Model.NearFogAlpha, this.ForceNearFog || this.rotateAroundCenter ? 1f : 0.0f, (this.rotateAroundCenter ? 1f : 4f) * Engine.DeltaTime);
      if (Celeste.PlayMode != Celeste.PlayModes.Debug)
        return;
      if (MInput.Keyboard.Pressed(Keys.P))
        Console.WriteLine(this.GetCameraString());
      if (MInput.Keyboard.Pressed(Keys.F2))
        Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.ReturnFromOptions, (HiresSnow) null);
      if (MInput.Keyboard.Pressed(Keys.Space))
        this.inFreeCameraDebugMode = !this.inFreeCameraDebugMode;
      this.Model.DrawDebugPoints = this.inFreeCameraDebugMode;
      if (MInput.Keyboard.Pressed(Keys.F1))
        AreaData.ReloadMountainViews();
    }

    private Vector3 GetBetween(Vector3 from, Vector3 to, float ease)
    {
      Vector2 from1 = new Vector2(from.X, from.Z);
      Vector2 from2 = new Vector2(to.X, to.Z);
      float angleRadians = Calc.AngleLerp(Calc.Angle(from1, Vector2.Zero), Calc.Angle(from2, Vector2.Zero), ease);
      float num1 = from1.Length();
      float num2 = from2.Length();
      float length = num1 + (num2 - num1) * ease;
      float y = from.Y + (to.Y - from.Y) * ease;
      Vector2 vector2 = -Calc.AngleToVector(angleRadians, length);
      return new Vector3(vector2.X, y, vector2.Y);
    }

    public override void BeforeRender(Scene scene)
    {
      this.Model.BeforeRender(scene);
    }

    public override void Render(Scene scene)
    {
      this.Model.Render();
      Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      GFX.Overworld["vignette"].Draw(Vector2.Zero, Vector2.Zero, Color.White * 0.2f);
      Draw.SpriteBatch.End();
      if (!this.inFreeCameraDebugMode)
        return;
      Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      ActiveFont.DrawOutline(this.GetCameraString(), new Vector2(8f, 8f), Vector2.Zero, Vector2.One * 0.75f, Color.White, 2f, Color.Black);
      Draw.SpriteBatch.End();
    }

    public void SnapCamera(int area, MountainCamera transform, bool targetRotate = false)
    {
      this.Area = area;
      this.Animating = false;
      this.rotateAroundCenter = false;
      this.rotateAroundTarget = targetRotate;
      this.Model.Camera = transform;
      this.percent = 1f;
    }

    public void SnapState(int state)
    {
      this.Model.SnapState(state);
    }

    public float EaseCamera(
      int area,
      MountainCamera transform,
      float? duration = null,
      bool nearTarget = true,
      bool targetRotate = false)
    {
      if (this.Area != area && area >= 0)
        this.PlayWhoosh(this.Area, area);
      this.Area = area;
      this.percent = 0.0f;
      this.Animating = true;
      this.rotateAroundCenter = false;
      this.rotateAroundTarget = targetRotate;
      this.userOffset = Vector2.Zero;
      this.easeCameraFrom = this.Model.Camera;
      if (nearTarget)
        this.easeCameraFrom.Target = this.easeCameraFrom.Position + (this.easeCameraFrom.Target - this.easeCameraFrom.Position).SafeNormalize() * 0.5f;
      this.easeCameraTo = transform;
      float radiansA = this.easeCameraFrom.Rotation.Forward().XZ().Angle();
      float radiansB = this.easeCameraTo.Rotation.Forward().XZ().Angle();
      float num1 = Calc.AngleDiff(radiansA, radiansB);
      float num2 = (float) -Math.Sign(num1) * (6.283185f - Math.Abs(num1));
      Vector3 between = this.GetBetween(this.easeCameraFrom.Position, this.easeCameraTo.Position, 0.5f);
      Vector2 vector1 = Calc.AngleToVector(MathHelper.Lerp(radiansA, radiansA + num1, 0.5f), 1f);
      Vector2 vector2 = Calc.AngleToVector(MathHelper.Lerp(radiansA, radiansA + num2, 0.5f), 1f);
      Vector3 vector3 = between + new Vector3(vector1.X, 0.0f, vector1.Y);
      double num3 = (double) vector3.Length();
      vector3 = between + new Vector3(vector2.X, 0.0f, vector2.Y);
      double num4 = (double) vector3.Length();
      this.easeCameraRotationAngleTo = num3 >= num4 ? radiansA + num2 : radiansA + num1;
      this.duration = duration.HasValue ? duration.Value : this.GetDuration(this.easeCameraFrom, this.easeCameraTo);
      return this.duration;
    }

    public void EaseState(int state)
    {
      this.Model.EaseState(state);
    }

    public void GotoRotationMode()
    {
      if (this.rotateAroundCenter)
        return;
      this.rotateAroundCenter = true;
      this.rotateTimer = new Vector2(this.Model.Camera.Position.X, this.Model.Camera.Position.Z).Angle();
      this.Model.EaseState(0);
    }

    private float GetDuration(MountainCamera from, MountainCamera to)
    {
      return Calc.Clamp((float) (Math.Max((double) Math.Abs(Calc.AngleDiff(Calc.Angle(new Vector2(from.Position.X, from.Position.Z), new Vector2(from.Target.X, from.Target.Z)), Calc.Angle(new Vector2(to.Position.X, to.Position.Z), new Vector2(to.Target.X, to.Target.Z)))) * 0.5, Math.Sqrt((double) (from.Position - to.Position).Length()) / 3.0) * 0.699999988079071), 0.3f, 1.1f);
    }

    private void PlayWhoosh(int from, int to)
    {
      string path = "";
      if (from == 0 && to == 1)
        path = "event:/ui/world_map/whoosh/400ms_forward";
      else if (from == 1 && to == 0)
        path = "event:/ui/world_map/whoosh/400ms_back";
      else if (from == 1 && to == 2)
        path = "event:/ui/world_map/whoosh/600ms_forward";
      else if (from == 2 && to == 1)
        path = "event:/ui/world_map/whoosh/600ms_back";
      else if (from < to && from > 1 && from < 7)
        path = "event:/ui/world_map/whoosh/700ms_forward";
      else if (from > to && from > 2 && from < 8)
        path = "event:/ui/world_map/whoosh/700ms_back";
      else if (from == 7 && to == 8)
        path = "event:/ui/world_map/whoosh/1000ms_forward";
      else if (from == 8 && to == 7)
        path = "event:/ui/world_map/whoosh/1000ms_back";
      else if (from == 8 && to == 9)
        path = "event:/ui/world_map/whoosh/900ms_forward";
      else if (from == 9 && to == 8)
        path = "event:/ui/world_map/whoosh/900ms_back";
      if (string.IsNullOrEmpty(path))
        return;
      Audio.Play(path);
    }

    private string GetCameraString()
    {
      Vector3 position = this.Model.Camera.Position;
      Vector3 vector3 = position + Vector3.Transform(Vector3.Forward, this.Model.Camera.Rotation.Conjugated()) * 2f;
      return "position=\"" + position.X.ToString("0.000") + ", " + position.Y.ToString("0.000") + ", " + position.Z.ToString("0.000") + "\" \ntarget=\"" + vector3.X.ToString("0.000") + ", " + vector3.Y.ToString("0.000") + ", " + vector3.Z.ToString("0.000") + "\"";
    }
  }
}

