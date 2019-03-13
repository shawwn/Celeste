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
    private float rotateTimer = 1.570796f;
    public bool AllowUserRotation = true;
    private float percent = 1f;
    private float duration = 1f;
    public bool ForceNearFog;
    public Action OnEaseEnd;
    private const float rotateDistance = 15f;
    private const float rotateYPosition = 3f;
    private bool rotateAroundCenter;
    private bool rotateAroundTarget;
    private const float DurationDivisor = 3f;
    public MountainCamera UntiltedCamera;
    public MountainModel Model;
    private Vector2 userOffset;
    private bool inFreeCameraDebugMode;
    private MountainCamera easeCameraFrom;
    private MountainCamera easeCameraTo;
    private float easeCameraRotationAngleTo;
    private float timer;
    private float door;

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
      this.userOffset = Vector2.op_Addition(this.userOffset, Vector2.op_Multiply(Vector2.op_Subtraction(this.AllowUserRotation ? Vector2.op_Multiply(Vector2.op_UnaryNegation(Celeste.Input.MountainAim.Value), 0.8f) : Vector2.get_Zero(), this.userOffset), 1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
      if (!this.rotateAroundCenter)
      {
        if (this.Area == 8)
          this.userOffset.Y = (__Null) (double) Math.Max(0.0f, (float) this.userOffset.Y);
        if (this.Area == 7)
          this.userOffset.X = (__Null) (double) Calc.Clamp((float) this.userOffset.X, -0.4f, 0.4f);
      }
      if (!this.inFreeCameraDebugMode)
      {
        if (this.rotateAroundCenter)
        {
          this.rotateTimer -= Engine.DeltaTime * 0.1f;
          Vector3 vector3;
          ((Vector3) ref vector3).\u002Ector((float) Math.Cos((double) this.rotateTimer) * 15f, 3f, (float) Math.Sin((double) this.rotateTimer) * 15f);
          ref Vector3 local = ref this.Model.Camera.Position;
          local = Vector3.op_Addition(local, Vector3.op_Multiply(Vector3.op_Subtraction(vector3, this.Model.Camera.Position), 1f - (float) Math.Pow(0.100000001490116, (double) Engine.DeltaTime)));
          this.Model.Camera.Target = Vector3.op_Addition(MountainRenderer.RotateLookAt, Vector3.op_Multiply(Vector3.get_Up(), (float) this.userOffset.Y));
          this.Model.Camera.Rotation = Quaternion.Slerp(this.Model.Camera.Rotation, ((Quaternion) null).LookAt(this.Model.Camera.Position, this.Model.Camera.Target, Vector3.get_Up()), Engine.DeltaTime * 4f);
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
            double num2 = (double) ((Vector2) ref vector2_1).Length();
            Vector2 vector2_2 = vector2.XZ();
            double num3 = (double) ((Vector2) ref vector2_2).Length();
            double num4 = (double) num1;
            float length = Calc.LerpClamp((float) num2, (float) num3, (float) num4);
            Vector2 vector3 = Calc.AngleToVector(MathHelper.Lerp(vector1.XZ().Angle(), this.easeCameraRotationAngleTo, num1), length);
            float num5 = Calc.LerpClamp((float) vector1.Y, (float) vector2.Y, num1);
            this.Model.Camera.Rotation = ((Quaternion) null).LookAt(new Vector3((float) vector3.X, num5, (float) vector3.Y), Vector3.get_Up());
            if ((double) this.percent >= 1.0)
            {
              this.rotateTimer = new Vector2((float) this.Model.Camera.Position.X, (float) this.Model.Camera.Position.Z).Angle();
              this.Animating = false;
              if (this.OnEaseEnd != null)
                this.OnEaseEnd();
            }
          }
          else if (this.rotateAroundTarget)
          {
            this.rotateTimer -= Engine.DeltaTime * 0.1f;
            Vector2 vector2 = Vector2.op_Subtraction(new Vector2((float) this.easeCameraTo.Target.X, (float) this.easeCameraTo.Target.Z), new Vector2((float) this.easeCameraTo.Position.X, (float) this.easeCameraTo.Position.Z));
            float num = ((Vector2) ref vector2).Length();
            Vector3 vector3;
            ((Vector3) ref vector3).\u002Ector((float) (this.easeCameraTo.Target.X + Math.Cos((double) this.rotateTimer) * (double) num), (float) this.easeCameraTo.Position.Y, (float) (this.easeCameraTo.Target.Z + Math.Sin((double) this.rotateTimer) * (double) num));
            ref Vector3 local = ref this.Model.Camera.Position;
            local = Vector3.op_Addition(local, Vector3.op_Multiply(Vector3.op_Subtraction(vector3, this.Model.Camera.Position), 1f - (float) Math.Pow(0.100000001490116, (double) Engine.DeltaTime)));
            this.Model.Camera.Target = Vector3.op_Addition(Vector3.op_Addition(this.easeCameraTo.Target, Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_Up(), (float) this.userOffset.Y), 2f)), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_Left(), (float) this.userOffset.X), 2f));
            this.Model.Camera.Rotation = Quaternion.Slerp(this.Model.Camera.Rotation, ((Quaternion) null).LookAt(this.Model.Camera.Position, this.Model.Camera.Target, Vector3.get_Up()), Engine.DeltaTime * 4f);
            this.UntiltedCamera = this.Camera;
          }
          else
          {
            this.Model.Camera.Rotation = this.easeCameraTo.Rotation;
            this.Model.Camera.Target = this.easeCameraTo.Target;
          }
          this.UntiltedCamera = this.Camera;
          if (Vector2.op_Inequality(this.userOffset, Vector2.get_Zero()) && !this.rotateAroundTarget)
            this.Model.Camera.LookAt(Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(this.Model.Camera.Position, this.Model.Camera.Rotation.Forward()), Vector3.op_Multiply(Vector3.op_Multiply(this.Model.Camera.Rotation.Left(), (float) this.userOffset.X), 0.25f)), Vector3.op_Multiply(Vector3.op_Multiply(this.Model.Camera.Rotation.Up(), (float) this.userOffset.Y), 0.25f)));
        }
      }
      else
      {
        Vector3 to = Vector3.Transform(Vector3.get_Forward(), this.Model.Camera.Rotation.Conjugated());
        this.Model.Camera.Rotation = this.Model.Camera.Rotation.LookAt(Vector3.get_Zero(), to, Vector3.get_Up());
        Vector3 vector3_1 = Vector3.Transform(Vector3.get_Left(), this.Model.Camera.Rotation.Conjugated());
        Vector3 vector3_2;
        ((Vector3) ref vector3_2).\u002Ector(0.0f, 0.0f, 0.0f);
        if (MInput.Keyboard.Check((Keys) 87))
          vector3_2 = Vector3.op_Addition(vector3_2, to);
        if (MInput.Keyboard.Check((Keys) 83))
          vector3_2 = Vector3.op_Subtraction(vector3_2, to);
        if (MInput.Keyboard.Check((Keys) 68))
          vector3_2 = Vector3.op_Subtraction(vector3_2, vector3_1);
        if (MInput.Keyboard.Check((Keys) 65))
          vector3_2 = Vector3.op_Addition(vector3_2, vector3_1);
        if (MInput.Keyboard.Check((Keys) 81))
          vector3_2 = Vector3.op_Addition(vector3_2, Vector3.get_Up());
        if (MInput.Keyboard.Check((Keys) 90))
          vector3_2 = Vector3.op_Addition(vector3_2, Vector3.get_Down());
        ref Vector3 local1 = ref this.Model.Camera.Position;
        local1 = Vector3.op_Addition(local1, Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, MInput.Keyboard.Check((Keys) 160) ? 0.5f : 5f), Engine.DeltaTime));
        if (MInput.Mouse.CheckLeftButton)
        {
          MouseState state = Mouse.GetState();
          Viewport viewport1 = Engine.Graphics.get_GraphicsDevice().get_Viewport();
          int num1 = ((Viewport) ref viewport1).get_Width() / 2;
          Viewport viewport2 = Engine.Graphics.get_GraphicsDevice().get_Viewport();
          int num2 = ((Viewport) ref viewport2).get_Height() / 2;
          int num3 = ((MouseState) ref state).get_X() - num1;
          int num4 = ((MouseState) ref state).get_Y() - num2;
          ref Quaternion local2 = ref this.Model.Camera.Rotation;
          local2 = Quaternion.op_Multiply(local2, Quaternion.CreateFromAxisAngle(Vector3.get_Up(), (float) num3 * 0.1f * Engine.DeltaTime));
          ref Quaternion local3 = ref this.Model.Camera.Rotation;
          local3 = Quaternion.op_Multiply(local3, Quaternion.CreateFromAxisAngle(vector3_1, (float) -num4 * 0.1f * Engine.DeltaTime));
          Mouse.SetPosition(num1, num2);
        }
        if (this.Area >= 0)
        {
          Vector3 target = AreaData.Areas[this.Area].MountainIdle.Target;
          Vector3 vector3_3 = Vector3.op_Multiply(vector3_1, 0.05f);
          Vector3 vector3_4 = Vector3.op_Multiply(Vector3.get_Up(), 0.05f);
          this.Model.DebugPoints.Clear();
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Addition(Vector3.op_Subtraction(target, vector3_3), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Addition(Vector3.op_Addition(target, vector3_3), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Subtraction(Vector3.op_Addition(target, vector3_3), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Addition(Vector3.op_Subtraction(target, vector3_3), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Subtraction(Vector3.op_Addition(target, vector3_3), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Subtraction(Vector3.op_Subtraction(target, vector3_3), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Subtraction(Vector3.op_Subtraction(target, Vector3.op_Multiply(vector3_3, 0.25f)), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Subtraction(Vector3.op_Addition(target, Vector3.op_Multiply(vector3_3, 0.25f)), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Addition(Vector3.op_Addition(target, Vector3.op_Multiply(vector3_3, 0.25f)), Vector3.op_Multiply(Vector3.get_Down(), 100f)), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Subtraction(Vector3.op_Subtraction(target, Vector3.op_Multiply(vector3_3, 0.25f)), vector3_4), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Addition(Vector3.op_Addition(target, Vector3.op_Multiply(vector3_3, 0.25f)), Vector3.op_Multiply(Vector3.get_Down(), 100f)), Color.get_Red()));
          this.Model.DebugPoints.Add(new VertexPositionColor(Vector3.op_Addition(Vector3.op_Subtraction(target, Vector3.op_Multiply(vector3_3, 0.25f)), Vector3.op_Multiply(Vector3.get_Down(), 100f)), Color.get_Red()));
        }
      }
      this.door = Calc.Approach(this.door, this.Area != 9 || this.rotateAroundCenter ? 0.0f : 1f, Engine.DeltaTime * 1f);
      this.Model.CoreWallPosition = Vector3.Lerp(Vector3.get_Zero(), Vector3.op_UnaryNegation(new Vector3(-1.5f, 1.5f, 1f)), Ease.CubeInOut(this.door));
      this.Model.NearFogAlpha = Calc.Approach(this.Model.NearFogAlpha, this.ForceNearFog || this.rotateAroundCenter ? 1f : 0.0f, (this.rotateAroundCenter ? 1f : 4f) * Engine.DeltaTime);
      if (Celeste.Celeste.PlayMode != Celeste.Celeste.PlayModes.Debug)
        return;
      if (MInput.Keyboard.Pressed((Keys) 80))
        Console.WriteLine(this.GetCameraString());
      if (MInput.Keyboard.Pressed((Keys) 113))
        Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.ReturnFromOptions, (HiresSnow) null);
      if (MInput.Keyboard.Pressed((Keys) 32))
        this.inFreeCameraDebugMode = !this.inFreeCameraDebugMode;
      this.Model.DrawDebugPoints = this.inFreeCameraDebugMode;
      if (!MInput.Keyboard.Pressed((Keys) 112))
        return;
      AreaData.ReloadMountainViews();
    }

    private Vector3 GetBetween(Vector3 from, Vector3 to, float ease)
    {
      Vector2 from1;
      ((Vector2) ref from1).\u002Ector((float) from.X, (float) from.Z);
      Vector2 from2;
      ((Vector2) ref from2).\u002Ector((float) to.X, (float) to.Z);
      double num1 = (double) Calc.AngleLerp(Calc.Angle(from1, Vector2.get_Zero()), Calc.Angle(from2, Vector2.get_Zero()), ease);
      float num2 = ((Vector2) ref from1).Length();
      float num3 = ((Vector2) ref from2).Length();
      float num4 = num2 + (num3 - num2) * ease;
      float num5 = (float) (from.Y + (to.Y - from.Y) * (double) ease);
      double num6 = (double) num4;
      Vector2 vector2 = Vector2.op_UnaryNegation(Calc.AngleToVector((float) num1, (float) num6));
      return new Vector3((float) vector2.X, num5, (float) vector2.Y);
    }

    public override void BeforeRender(Scene scene)
    {
      this.Model.BeforeRender(scene);
    }

    public override void Render(Scene scene)
    {
      this.Model.Render();
      Draw.SpriteBatch.Begin((SpriteSortMode) 1, (BlendState) BlendState.NonPremultiplied, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      GFX.Overworld["vignette"].Draw(Vector2.get_Zero(), Vector2.get_Zero(), Color.op_Multiply(Color.get_White(), 0.2f));
      Draw.SpriteBatch.End();
      if (!this.inFreeCameraDebugMode)
        return;
      Draw.SpriteBatch.Begin((SpriteSortMode) 1, (BlendState) BlendState.NonPremultiplied, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      ActiveFont.DrawOutline(this.GetCameraString(), new Vector2(8f, 8f), Vector2.get_Zero(), Vector2.op_Multiply(Vector2.get_One(), 0.75f), Color.get_White(), 2f, Color.get_Black());
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
      this.userOffset = Vector2.get_Zero();
      this.easeCameraFrom = this.Model.Camera;
      if (nearTarget)
        this.easeCameraFrom.Target = Vector3.op_Addition(this.easeCameraFrom.Position, Vector3.op_Multiply(Vector3.op_Subtraction(this.easeCameraFrom.Target, this.easeCameraFrom.Position).SafeNormalize(), 0.5f));
      this.easeCameraTo = transform;
      float radiansA = this.easeCameraFrom.Rotation.Forward().XZ().Angle();
      float radiansB = this.easeCameraTo.Rotation.Forward().XZ().Angle();
      float num1 = Calc.AngleDiff(radiansA, radiansB);
      float num2 = (float) -Math.Sign(num1) * (6.283185f - Math.Abs(num1));
      Vector3 between = this.GetBetween(this.easeCameraFrom.Position, this.easeCameraTo.Position, 0.5f);
      Vector2 vector1 = Calc.AngleToVector(MathHelper.Lerp(radiansA, radiansA + num1, 0.5f), 1f);
      Vector2 vector2 = Calc.AngleToVector(MathHelper.Lerp(radiansA, radiansA + num2, 0.5f), 1f);
      Vector3 vector3_1 = Vector3.op_Addition(between, new Vector3((float) vector1.X, 0.0f, (float) vector1.Y));
      double num3 = (double) ((Vector3) ref vector3_1).Length();
      Vector3 vector3_2 = Vector3.op_Addition(between, new Vector3((float) vector2.X, 0.0f, (float) vector2.Y));
      double num4 = (double) ((Vector3) ref vector3_2).Length();
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
      this.rotateTimer = new Vector2((float) this.Model.Camera.Position.X, (float) this.Model.Camera.Position.Z).Angle();
      this.Model.EaseState(0);
    }

    private float GetDuration(MountainCamera from, MountainCamera to)
    {
      double num = (double) Calc.AngleDiff(Calc.Angle(new Vector2((float) from.Position.X, (float) from.Position.Z), new Vector2((float) from.Target.X, (float) from.Target.Z)), Calc.Angle(new Vector2((float) to.Position.X, (float) to.Position.Z), new Vector2((float) to.Target.X, (float) to.Target.Z)));
      Vector3 vector3 = Vector3.op_Subtraction(from.Position, to.Position);
      double val2 = Math.Sqrt((double) ((Vector3) ref vector3).Length()) / 3.0;
      return Calc.Clamp((float) (Math.Max((double) Math.Abs((float) num) * 0.5, val2) * 0.699999988079071), 0.3f, 1.1f);
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
      Vector3 vector3 = Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.Transform(Vector3.get_Forward(), this.Model.Camera.Rotation.Conjugated()), 2f));
      return "position=\"" + position.X.ToString("0.000") + ", " + position.Y.ToString("0.000") + ", " + position.Z.ToString("0.000") + "\" \ntarget=\"" + vector3.X.ToString("0.000") + ", " + vector3.Y.ToString("0.000") + ", " + vector3.Z.ToString("0.000") + "\"";
    }
  }
}
