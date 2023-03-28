// Decompiled with JetBrains decompiler
// Type: Monocle.Calc
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Monocle
{
  public static class Calc
  {
    public static Random Random = new Random();
    private static Stack<Random> randomStack = new Stack<Random>();
    private static int[] shakeVectorOffsets = new int[5]
    {
      -1,
      -1,
      0,
      1,
      1
    };
    public const float Right = 0.0f;
    public const float Up = -1.5707964f;
    public const float Left = 3.1415927f;
    public const float Down = 1.5707964f;
    public const float UpRight = -0.7853982f;
    public const float UpLeft = -2.3561945f;
    public const float DownRight = 0.7853982f;
    public const float DownLeft = 2.3561945f;
    public const float DegToRad = 0.017453292f;
    public const float RadToDeg = 57.295776f;
    public const float DtR = 0.017453292f;
    public const float RtD = 57.295776f;
    public const float Circle = 6.2831855f;
    public const float HalfCircle = 3.1415927f;
    public const float QuarterCircle = 1.5707964f;
    public const float EighthCircle = 0.7853982f;
    private const string Hex = "0123456789ABCDEF";
    private static Stopwatch stopwatch;

    public static int EnumLength(Type e) => Enum.GetNames(e).Length;

    public static T StringToEnum<T>(string str) where T : struct => Enum.IsDefined(typeof (T), (object) str) ? (T) Enum.Parse(typeof (T), str) : throw new Exception("The string cannot be converted to the enum type.");

    public static T[] StringsToEnums<T>(string[] strs) where T : struct
    {
      T[] enums = new T[strs.Length];
      for (int index = 0; index < strs.Length; ++index)
        enums[index] = Calc.StringToEnum<T>(strs[index]);
      return enums;
    }

    public static bool EnumHasString<T>(string str) where T : struct => Enum.IsDefined(typeof (T), (object) str);

    public static bool StartsWith(this string str, string match) => str.IndexOf(match) == 0;

    public static bool EndsWith(this string str, string match) => str.LastIndexOf(match) == str.Length - match.Length;

    public static bool IsIgnoreCase(this string str, params string[] matches)
    {
      if (string.IsNullOrEmpty(str))
        return false;
      foreach (string match in matches)
      {
        if (str.Equals(match, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }

    public static string ToString(this int num, int minDigits)
    {
      string str = num.ToString();
      while (str.Length < minDigits)
        str = "0" + str;
      return str;
    }

    public static string[] SplitLines(
      string text,
      SpriteFont font,
      int maxLineWidth,
      char newLine = '\n')
    {
      List<string> stringList = new List<string>();
      string str1 = text;
      char[] chArray1 = new char[1]{ newLine };
      foreach (string str2 in str1.Split(chArray1))
      {
        string str3 = "";
        char[] chArray2 = new char[1]{ ' ' };
        foreach (string str4 in str2.Split(chArray2))
        {
          if ((double) font.MeasureString(str3 + " " + str4).X > (double) maxLineWidth)
          {
            stringList.Add(str3);
            str3 = str4;
          }
          else
          {
            if (str3 != "")
              str3 += " ";
            str3 += str4;
          }
        }
        stringList.Add(str3);
      }
      return stringList.ToArray();
    }

    public static int Count<T>(T target, T a, T b)
    {
      int num = 0;
      if (a.Equals((object) target))
        ++num;
      if (b.Equals((object) target))
        ++num;
      return num;
    }

    public static int Count<T>(T target, T a, T b, T c)
    {
      int num = 0;
      if (a.Equals((object) target))
        ++num;
      if (b.Equals((object) target))
        ++num;
      if (c.Equals((object) target))
        ++num;
      return num;
    }

    public static int Count<T>(T target, T a, T b, T c, T d)
    {
      int num = 0;
      if (a.Equals((object) target))
        ++num;
      if (b.Equals((object) target))
        ++num;
      if (c.Equals((object) target))
        ++num;
      if (d.Equals((object) target))
        ++num;
      return num;
    }

    public static int Count<T>(T target, T a, T b, T c, T d, T e)
    {
      int num = 0;
      if (a.Equals((object) target))
        ++num;
      if (b.Equals((object) target))
        ++num;
      if (c.Equals((object) target))
        ++num;
      if (d.Equals((object) target))
        ++num;
      if (e.Equals((object) target))
        ++num;
      return num;
    }

    public static int Count<T>(T target, T a, T b, T c, T d, T e, T f)
    {
      int num = 0;
      if (a.Equals((object) target))
        ++num;
      if (b.Equals((object) target))
        ++num;
      if (c.Equals((object) target))
        ++num;
      if (d.Equals((object) target))
        ++num;
      if (e.Equals((object) target))
        ++num;
      if (f.Equals((object) target))
        ++num;
      return num;
    }

    public static T GiveMe<T>(int index, T a, T b)
    {
      if (index == 0)
        return a;
      if (index != 1)
        throw new Exception("Index was out of range!");
      return b;
    }

    public static T GiveMe<T>(int index, T a, T b, T c)
    {
      switch (index)
      {
        case 0:
          return a;
        case 1:
          return b;
        case 2:
          return c;
        default:
          throw new Exception("Index was out of range!");
      }
    }

    public static T GiveMe<T>(int index, T a, T b, T c, T d)
    {
      switch (index)
      {
        case 0:
          return a;
        case 1:
          return b;
        case 2:
          return c;
        case 3:
          return d;
        default:
          throw new Exception("Index was out of range!");
      }
    }

    public static T GiveMe<T>(int index, T a, T b, T c, T d, T e)
    {
      switch (index)
      {
        case 0:
          return a;
        case 1:
          return b;
        case 2:
          return c;
        case 3:
          return d;
        case 4:
          return e;
        default:
          throw new Exception("Index was out of range!");
      }
    }

    public static T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f)
    {
      switch (index)
      {
        case 0:
          return a;
        case 1:
          return b;
        case 2:
          return c;
        case 3:
          return d;
        case 4:
          return e;
        case 5:
          return f;
        default:
          throw new Exception("Index was out of range!");
      }
    }

    public static void PushRandom(int newSeed)
    {
      Calc.randomStack.Push(Calc.Random);
      Calc.Random = new Random(newSeed);
    }

    public static void PushRandom(Random random)
    {
      Calc.randomStack.Push(Calc.Random);
      Calc.Random = random;
    }

    public static void PushRandom()
    {
      Calc.randomStack.Push(Calc.Random);
      Calc.Random = new Random();
    }

    public static void PopRandom() => Calc.Random = Calc.randomStack.Pop();

    public static T Choose<T>(this Random random, T a, T b) => Calc.GiveMe<T>(random.Next(2), a, b);

    public static T Choose<T>(this Random random, T a, T b, T c) => Calc.GiveMe<T>(random.Next(3), a, b, c);

    public static T Choose<T>(this Random random, T a, T b, T c, T d) => Calc.GiveMe<T>(random.Next(4), a, b, c, d);

    public static T Choose<T>(this Random random, T a, T b, T c, T d, T e) => Calc.GiveMe<T>(random.Next(5), a, b, c, d, e);

    public static T Choose<T>(this Random random, T a, T b, T c, T d, T e, T f) => Calc.GiveMe<T>(random.Next(6), a, b, c, d, e, f);

    public static T Choose<T>(this Random random, params T[] choices) => choices[random.Next(choices.Length)];

    public static T Choose<T>(this Random random, List<T> choices) => choices[random.Next(choices.Count)];

    public static int Range(this Random random, int min, int max) => min + random.Next(max - min);

    public static float Range(this Random random, float min, float max) => min + random.NextFloat(max - min);

    public static Vector2 Range(this Random random, Vector2 min, Vector2 max) => min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));

    public static int Facing(this Random random) => (double) random.NextFloat() >= 0.5 ? 1 : -1;

    public static bool Chance(this Random random, float chance) => (double) random.NextFloat() < (double) chance;

    public static float NextFloat(this Random random) => (float) random.NextDouble();

    public static float NextFloat(this Random random, float max) => random.NextFloat() * max;

    public static float NextAngle(this Random random) => random.NextFloat() * 6.2831855f;

    public static Vector2 ShakeVector(this Random random) => new Vector2((float) random.Choose<int>(Calc.shakeVectorOffsets), (float) random.Choose<int>(Calc.shakeVectorOffsets));

    public static Vector2 ClosestTo(this List<Vector2> list, Vector2 to)
    {
      Vector2 vector2 = list[0];
      float num1 = Vector2.DistanceSquared(list[0], to);
      for (int index = 1; index < list.Count; ++index)
      {
        float num2 = Vector2.DistanceSquared(list[index], to);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          vector2 = list[index];
        }
      }
      return vector2;
    }

    public static Vector2 ClosestTo(this Vector2[] list, Vector2 to)
    {
      Vector2 vector2 = list[0];
      float num1 = Vector2.DistanceSquared(list[0], to);
      for (int index = 1; index < list.Length; ++index)
      {
        float num2 = Vector2.DistanceSquared(list[index], to);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          vector2 = list[index];
        }
      }
      return vector2;
    }

    public static Vector2 ClosestTo(this Vector2[] list, Vector2 to, out int index)
    {
      index = 0;
      Vector2 vector2 = list[0];
      float num1 = Vector2.DistanceSquared(list[0], to);
      for (int index1 = 1; index1 < list.Length; ++index1)
      {
        float num2 = Vector2.DistanceSquared(list[index1], to);
        if ((double) num2 < (double) num1)
        {
          index = index1;
          num1 = num2;
          vector2 = list[index1];
        }
      }
      return vector2;
    }

    public static void Shuffle<T>(this List<T> list, Random random)
    {
      int count = list.Count;
      while (--count > 0)
      {
        T obj = list[count];
        int index;
        list[count] = list[index = random.Next(count + 1)];
        list[index] = obj;
      }
    }

    public static void Shuffle<T>(this List<T> list) => list.Shuffle<T>(Calc.Random);

    public static void ShuffleSetFirst<T>(this List<T> list, Random random, T first)
    {
      int num = 0;
      while (list.Contains(first))
      {
        list.Remove(first);
        ++num;
      }
      list.Shuffle<T>(random);
      for (int index = 0; index < num; ++index)
        list.Insert(0, first);
    }

    public static void ShuffleSetFirst<T>(this List<T> list, T first) => list.ShuffleSetFirst<T>(Calc.Random, first);

    public static void ShuffleNotFirst<T>(this List<T> list, Random random, T notFirst)
    {
      int num = 0;
      while (list.Contains(notFirst))
      {
        list.Remove(notFirst);
        ++num;
      }
      list.Shuffle<T>(random);
      for (int index = 0; index < num; ++index)
        list.Insert(random.Next(list.Count - 1) + 1, notFirst);
    }

    public static void ShuffleNotFirst<T>(this List<T> list, T notFirst) => list.ShuffleNotFirst<T>(Calc.Random, notFirst);

    public static Color Invert(this Color color) => new Color((int) byte.MaxValue - (int) color.R, (int) byte.MaxValue - (int) color.G, (int) byte.MaxValue - (int) color.B, (int) color.A);

    public static Color HexToColor(string hex)
    {
      int num1 = 0;
      if (hex.Length >= 1 && hex[0] == '#')
        num1 = 1;
      if (hex.Length - num1 >= 6)
      {
        double r = (double) ((int) Calc.HexToByte(hex[num1]) * 16 + (int) Calc.HexToByte(hex[num1 + 1])) / (double) byte.MaxValue;
        float num2 = (float) ((int) Calc.HexToByte(hex[num1 + 2]) * 16 + (int) Calc.HexToByte(hex[num1 + 3])) / (float) byte.MaxValue;
        float num3 = (float) ((int) Calc.HexToByte(hex[num1 + 4]) * 16 + (int) Calc.HexToByte(hex[num1 + 5])) / (float) byte.MaxValue;
        double g = (double) num2;
        double b = (double) num3;
        return new Color((float) r, (float) g, (float) b);
      }
      int result;
      return int.TryParse(hex.Substring(num1), out result) ? Calc.HexToColor(result) : Color.White;
    }

    public static Color HexToColor(int hex) => new Color()
    {
      A = byte.MaxValue,
      R = (byte) (hex >> 16),
      G = (byte) (hex >> 8),
      B = (byte) hex
    };

    public static Color HsvToColor(float hue, float s, float v)
    {
      int num1 = (int) ((double) hue * 360.0);
      float num2 = s * v;
      float num3 = num2 * (1f - Math.Abs((float) ((double) num1 / 60.0 % 2.0 - 1.0)));
      float num4 = v - num2;
      if (num1 < 60)
        return new Color(num4 + num2, num4 + num3, num4);
      if (num1 < 120)
        return new Color(num4 + num3, num4 + num2, num4);
      if (num1 < 180)
        return new Color(num4, num4 + num2, num4 + num3);
      if (num1 < 240)
        return new Color(num4, num4 + num3, num4 + num2);
      return num1 < 300 ? new Color(num4 + num3, num4, num4 + num2) : new Color(num4 + num2, num4, num4 + num3);
    }

    public static string ShortGameplayFormat(this TimeSpan time) => time.TotalHours >= 1.0 ? ((int) time.TotalHours).ToString() + ":" + time.ToString("mm\\:ss\\.fff") : time.ToString("m\\:ss\\.fff");

    public static string LongGameplayFormat(this TimeSpan time)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (time.TotalDays >= 2.0)
      {
        stringBuilder.Append((int) time.TotalDays);
        stringBuilder.Append(" days, ");
      }
      else if (time.TotalDays >= 1.0)
        stringBuilder.Append("1 day, ");
      stringBuilder.Append((time.TotalHours - (double) ((int) time.TotalDays * 24)).ToString("0.0"));
      stringBuilder.Append(" hours");
      return stringBuilder.ToString();
    }

    public static int Digits(this int num)
    {
      int num1 = 1;
      for (int index = 10; num >= index; index *= 10)
        ++num1;
      return num1;
    }

    public static byte HexToByte(char c) => (byte) "0123456789ABCDEF".IndexOf(char.ToUpper(c));

    public static float Percent(float num, float zeroAt, float oneAt) => MathHelper.Clamp((float) (((double) num - (double) zeroAt) / ((double) oneAt - (double) zeroAt)), 0.0f, 1f);

    public static float SignThreshold(float value, float threshold) => (double) Math.Abs(value) >= (double) threshold ? (float) Math.Sign(value) : 0.0f;

    public static float Min(params float[] values)
    {
      float num = values[0];
      for (int index = 1; index < values.Length; ++index)
        num = MathHelper.Min(values[index], num);
      return num;
    }

    public static float Max(params float[] values)
    {
      float num = values[0];
      for (int index = 1; index < values.Length; ++index)
        num = MathHelper.Max(values[index], num);
      return num;
    }

    public static int Max(int a, int b, int c, int d) => Math.Max(Math.Max(Math.Max(a, b), c), d);

    public static float ToRad(this float f) => f * ((float) Math.PI / 180f);

    public static float ToDeg(this float f) => f * 57.295776f;

    public static int Axis(bool negative, bool positive, int both = 0) => negative ? (positive ? both : -1) : (positive ? 1 : 0);

    public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

    public static float Clamp(float value, float min, float max) => Math.Min(Math.Max(value, min), max);

    public static float YoYo(float value) => (double) value <= 0.5 ? value * 2f : (float) (1.0 - ((double) value - 0.5) * 2.0);

    public static float Map(float val, float min, float max, float newMin = 0.0f, float newMax = 1f) => (float) (((double) val - (double) min) / ((double) max - (double) min) * ((double) newMax - (double) newMin)) + newMin;

    public static float SineMap(float counter, float newMin, float newMax) => Calc.Map((float) Math.Sin((double) counter), -1f, 1f, newMin, newMax);

    public static float ClampedMap(float val, float min, float max, float newMin = 0.0f, float newMax = 1f) => MathHelper.Clamp((float) (((double) val - (double) min) / ((double) max - (double) min)), 0.0f, 1f) * (newMax - newMin) + newMin;

    public static float LerpSnap(float value1, float value2, float amount, float snapThreshold = 0.1f)
    {
      float num = MathHelper.Lerp(value1, value2, amount);
      return (double) Math.Abs(num - value2) < (double) snapThreshold ? value2 : num;
    }

    public static float LerpClamp(float value1, float value2, float lerp) => MathHelper.Lerp(value1, value2, MathHelper.Clamp(lerp, 0.0f, 1f));

    public static Vector2 LerpSnap(
      Vector2 value1,
      Vector2 value2,
      float amount,
      float snapThresholdSq = 0.1f)
    {
      Vector2 vector2 = Vector2.Lerp(value1, value2, amount);
      return (double) (vector2 - value2).LengthSquared() < (double) snapThresholdSq ? value2 : vector2;
    }

    public static Vector2 Sign(this Vector2 vec) => new Vector2((float) Math.Sign(vec.X), (float) Math.Sign(vec.Y));

    public static Vector2 SafeNormalize(this Vector2 vec) => vec.SafeNormalize(Vector2.Zero);

    public static Vector2 SafeNormalize(this Vector2 vec, float length) => vec.SafeNormalize(Vector2.Zero, length);

    public static Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero)
    {
      if (vec == Vector2.Zero)
        return ifZero;
      vec.Normalize();
      return vec;
    }

    public static Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero, float length)
    {
      if (vec == Vector2.Zero)
        return ifZero * length;
      vec.Normalize();
      return vec * length;
    }

    public static Vector2 TurnRight(this Vector2 vec) => new Vector2(-vec.Y, vec.X);

    public static float ReflectAngle(float angle, float axis = 0.0f) => (float) -((double) angle + (double) axis) - axis;

    public static float ReflectAngle(float angleRadians, Vector2 axis) => Calc.ReflectAngle(angleRadians, axis.Angle());

    public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
    {
      Vector2 vector2 = lineB - lineA;
      float num = MathHelper.Clamp(Vector2.Dot(closestTo - lineA, vector2) / Vector2.Dot(vector2, vector2), 0.0f, 1f);
      return lineA + vector2 * num;
    }

    public static Vector2 Round(this Vector2 vec) => new Vector2((float) Math.Round((double) vec.X), (float) Math.Round((double) vec.Y));

    public static float Snap(float value, float increment) => (float) Math.Round((double) value / (double) increment) * increment;

    public static float Snap(float value, float increment, float offset) => (float) Math.Round(((double) value - (double) offset) / (double) increment) * increment + offset;

    public static float WrapAngleDeg(float angleDegrees) => (float) (((double) angleDegrees * (double) Math.Sign(angleDegrees) + 180.0) % 360.0 - 180.0) * (float) Math.Sign(angleDegrees);

    public static float WrapAngle(float angleRadians) => (float) (((double) angleRadians * (double) Math.Sign(angleRadians) + 3.1415927410125732) % 6.2831854820251465 - 3.1415927410125732) * (float) Math.Sign(angleRadians);

    public static Vector2 AngleToVector(float angleRadians, float length) => new Vector2((float) Math.Cos((double) angleRadians) * length, (float) Math.Sin((double) angleRadians) * length);

    public static float AngleApproach(float val, float target, float maxMove)
    {
      float num = Calc.AngleDiff(val, target);
      return (double) Math.Abs(num) < (double) maxMove ? target : val + MathHelper.Clamp(num, -maxMove, maxMove);
    }

    public static float AngleLerp(float startAngle, float endAngle, float percent) => startAngle + Calc.AngleDiff(startAngle, endAngle) * percent;

    public static float Approach(float val, float target, float maxMove) => (double) val <= (double) target ? Math.Min(val + maxMove, target) : Math.Max(val - maxMove, target);

    public static float AngleDiff(float radiansA, float radiansB)
    {
      float num = radiansB - radiansA;
      while ((double) num > 3.1415927410125732)
        num -= 6.2831855f;
      while ((double) num <= -3.1415927410125732)
        num += 6.2831855f;
      return num;
    }

    public static float AbsAngleDiff(float radiansA, float radiansB) => Math.Abs(Calc.AngleDiff(radiansA, radiansB));

    public static int SignAngleDiff(float radiansA, float radiansB) => Math.Sign(Calc.AngleDiff(radiansA, radiansB));

    public static float Angle(Vector2 from, Vector2 to) => (float) Math.Atan2((double) to.Y - (double) from.Y, (double) to.X - (double) from.X);

    public static Color ToggleColors(Color current, Color a, Color b) => current == a ? b : a;

    public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB) => (double) Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < (double) Math.Abs(Calc.AngleDiff(currentAngle, angleB)) ? angleA : angleB;

    public static float ShorterAngleDifference(
      float currentAngle,
      float angleA,
      float angleB,
      float angleC)
    {
      return (double) Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < (double) Math.Abs(Calc.AngleDiff(currentAngle, angleB)) ? Calc.ShorterAngleDifference(currentAngle, angleA, angleC) : Calc.ShorterAngleDifference(currentAngle, angleB, angleC);
    }

    public static bool IsInRange<T>(this T[] array, int index) => index >= 0 && index < array.Length;

    public static bool IsInRange<T>(this List<T> list, int index) => index >= 0 && index < list.Count;

    public static T[] Array<T>(params T[] items) => items;

    public static T[] VerifyLength<T>(this T[] array, int length)
    {
      if (array == null)
        return new T[length];
      if (array.Length == length)
        return array;
      T[] objArray = new T[length];
      for (int index = 0; index < Math.Min(length, array.Length); ++index)
        objArray[index] = array[index];
      return objArray;
    }

    public static T[][] VerifyLength<T>(this T[][] array, int length0, int length1)
    {
      array = array.VerifyLength<T[]>(length0);
      for (int index = 0; index < array.Length; ++index)
        array[index] = array[index].VerifyLength<T>(length1);
      return array;
    }

    public static bool BetweenInterval(float val, float interval) => (double) val % ((double) interval * 2.0) > (double) interval;

    public static bool OnInterval(float val, float prevVal, float interval) => (int) ((double) prevVal / (double) interval) != (int) ((double) val / (double) interval);

    public static Vector2 Toward(Vector2 from, Vector2 to, float length) => from == to ? Vector2.Zero : (to - from).SafeNormalize(length);

    public static Vector2 Toward(Entity from, Entity to, float length) => Calc.Toward(from.Position, to.Position, length);

    public static Vector2 Perpendicular(this Vector2 vector) => new Vector2(-vector.Y, vector.X);

    public static float Angle(this Vector2 vector) => (float) Math.Atan2((double) vector.Y, (double) vector.X);

    public static Vector2 Clamp(this Vector2 val, float minX, float minY, float maxX, float maxY) => new Vector2(MathHelper.Clamp(val.X, minX, maxX), MathHelper.Clamp(val.Y, minY, maxY));

    public static Vector2 Floor(this Vector2 val) => new Vector2((float) (int) Math.Floor((double) val.X), (float) (int) Math.Floor((double) val.Y));

    public static Vector2 Ceiling(this Vector2 val) => new Vector2((float) (int) Math.Ceiling((double) val.X), (float) (int) Math.Ceiling((double) val.Y));

    public static Vector2 Abs(this Vector2 val) => new Vector2(Math.Abs(val.X), Math.Abs(val.Y));

    public static Vector2 Approach(Vector2 val, Vector2 target, float maxMove)
    {
      if ((double) maxMove == 0.0 || val == target)
        return val;
      Vector2 vector2 = target - val;
      if ((double) vector2.Length() < (double) maxMove)
        return target;
      vector2.Normalize();
      return val + vector2 * maxMove;
    }

    public static Vector2 FourWayNormal(this Vector2 vec)
    {
      if (vec == Vector2.Zero)
        return Vector2.Zero;
      vec = Calc.AngleToVector((float) Math.Floor(((double) vec.Angle() + 0.7853981852531433) / 1.5707963705062866) * 1.5707964f, 1f);
      vec.X = (double) Math.Abs(vec.X) >= 0.5 ? (float) Math.Sign(vec.X) : 0.0f;
      vec.Y = (double) Math.Abs(vec.Y) >= 0.5 ? (float) Math.Sign(vec.Y) : 0.0f;
      return vec;
    }

    public static Vector2 EightWayNormal(this Vector2 vec)
    {
      if (vec == Vector2.Zero)
        return Vector2.Zero;
      vec = Calc.AngleToVector((float) Math.Floor(((double) vec.Angle() + 0.39269909262657166) / 0.7853981852531433) * 0.7853982f, 1f);
      if ((double) Math.Abs(vec.X) < 0.5)
        vec.X = 0.0f;
      else if ((double) Math.Abs(vec.Y) < 0.5)
        vec.Y = 0.0f;
      return vec;
    }

    public static Vector2 SnappedNormal(this Vector2 vec, float slices)
    {
      float num = 6.2831855f / slices;
      return Calc.AngleToVector((float) Math.Floor(((double) vec.Angle() + (double) num / 2.0) / (double) num) * num, 1f);
    }

    public static Vector2 Snapped(this Vector2 vec, float slices)
    {
      float num = 6.2831855f / slices;
      return Calc.AngleToVector((float) Math.Floor(((double) vec.Angle() + (double) num / 2.0) / (double) num) * num, vec.Length());
    }

    public static Vector2 XComp(this Vector2 vec) => Vector2.UnitX * vec.X;

    public static Vector2 YComp(this Vector2 vec) => Vector2.UnitY * vec.Y;

    public static Vector2[] ParseVector2List(string list, char seperator = '|')
    {
      string[] strArray1 = list.Split(seperator);
      Vector2[] vector2List = new Vector2[strArray1.Length];
      for (int index = 0; index < strArray1.Length; ++index)
      {
        string[] strArray2 = strArray1[index].Split(',');
        vector2List[index] = new Vector2((float) Convert.ToInt32(strArray2[0]), (float) Convert.ToInt32(strArray2[1]));
      }
      return vector2List;
    }

    public static Vector2 Rotate(this Vector2 vec, float angleRadians) => Calc.AngleToVector(vec.Angle() + angleRadians, vec.Length());

    public static Vector2 RotateTowards(
      this Vector2 vec,
      float targetAngleRadians,
      float maxMoveRadians)
    {
      return Calc.AngleToVector(Calc.AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians), vec.Length());
    }

    public static Vector3 RotateTowards(
      this Vector3 from,
      Vector3 target,
      float maxRotationRadians)
    {
      Vector3 vector3 = Vector3.Cross(from, target);
      double num1 = (double) from.Length();
      float num2 = target.Length();
      float w = (float) Math.Sqrt(num1 * num1 * ((double) num2 * (double) num2)) + Vector3.Dot(from, target);
      Quaternion quaternion = new Quaternion(vector3.X, vector3.Y, vector3.Z, w);
      if ((double) quaternion.Length() <= (double) maxRotationRadians)
        return target;
      quaternion.Normalize();
      Quaternion rotation = quaternion * maxRotationRadians;
      return Vector3.Transform(from, rotation);
    }

    public static Vector2 XZ(this Vector3 vector) => new Vector2(vector.X, vector.Z);

    public static Vector3 Approach(this Vector3 v, Vector3 target, float amount) => (double) amount > (double) (target - v).Length() ? target : v + (target - v).SafeNormalize() * amount;

    public static Vector3 SafeNormalize(this Vector3 v)
    {
      float num = v.Length();
      return (double) num > 0.0 ? v / num : Vector3.Zero;
    }

    public static int[,] ReadCSVIntGrid(string csv, int width, int height)
    {
      int[,] numArray = new int[width, height];
      for (int index1 = 0; index1 < width; ++index1)
      {
        for (int index2 = 0; index2 < height; ++index2)
          numArray[index1, index2] = -1;
      }
      string[] strArray1 = csv.Split('\n');
      for (int index3 = 0; index3 < height && index3 < strArray1.Length; ++index3)
      {
        string[] strArray2 = strArray1[index3].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries);
        for (int index4 = 0; index4 < width && index4 < strArray2.Length; ++index4)
          numArray[index4, index3] = Convert.ToInt32(strArray2[index4]);
      }
      return numArray;
    }

    public static int[] ReadCSVInt(string csv)
    {
      if (csv == "")
        return new int[0];
      string[] strArray = csv.Split(',');
      int[] numArray = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        numArray[index] = Convert.ToInt32(strArray[index].Trim());
      return numArray;
    }

    public static int[] ReadCSVIntWithTricks(string csv)
    {
      if (csv == "")
        return new int[0];
      string[] strArray1 = csv.Split(',');
      List<int> intList = new List<int>();
      foreach (string str in strArray1)
      {
        if (str.IndexOf('-') != -1)
        {
          string[] strArray2 = str.Split('-');
          int int32_1 = Convert.ToInt32(strArray2[0]);
          int int32_2 = Convert.ToInt32(strArray2[1]);
          for (int index = int32_1; index != int32_2; index += Math.Sign(int32_2 - int32_1))
            intList.Add(index);
          intList.Add(int32_2);
        }
        else if (str.IndexOf('*') != -1)
        {
          string[] strArray3 = str.Split('*');
          int int32_3 = Convert.ToInt32(strArray3[0]);
          int int32_4 = Convert.ToInt32(strArray3[1]);
          for (int index = 0; index < int32_4; ++index)
            intList.Add(int32_3);
        }
        else
          intList.Add(Convert.ToInt32(str));
      }
      return intList.ToArray();
    }

    public static string[] ReadCSV(string csv)
    {
      if (csv == "")
        return new string[0];
      string[] strArray = csv.Split(',');
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = strArray[index].Trim();
      return strArray;
    }

    public static string IntGridToCSV(int[,] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<int> values = new List<int>();
      int num1 = 0;
      for (int index1 = 0; index1 < data.GetLength(1); ++index1)
      {
        int num2 = 0;
        for (int index2 = 0; index2 < data.GetLength(0); ++index2)
        {
          if (data[index2, index1] == -1)
          {
            ++num2;
          }
          else
          {
            for (int index3 = 0; index3 < num1; ++index3)
              stringBuilder.Append('\n');
            for (int index4 = 0; index4 < num2; ++index4)
              values.Add(-1);
            num2 = num1 = 0;
            values.Add(data[index2, index1]);
          }
        }
        if (values.Count > 0)
        {
          stringBuilder.Append(string.Join<int>(",", (IEnumerable<int>) values));
          values.Clear();
        }
        ++num1;
      }
      return stringBuilder.ToString();
    }

    public static bool[,] GetBitData(string data, char rowSep = '\n')
    {
      int length1 = 0;
      for (int index = 0; index < data.Length; ++index)
      {
        if (data[index] == '1' || data[index] == '0')
          ++length1;
        else if ((int) data[index] == (int) rowSep)
          break;
      }
      int length2 = data.Count<char>((Func<char, bool>) (c => c == '\n')) + 1;
      bool[,] bitData = new bool[length1, length2];
      int index1 = 0;
      int index2 = 0;
      for (int index3 = 0; index3 < data.Length; ++index3)
      {
        switch (data[index3])
        {
          case '\n':
            index1 = 0;
            ++index2;
            break;
          case '0':
            bitData[index1, index2] = false;
            ++index1;
            break;
          case '1':
            bitData[index1, index2] = true;
            ++index1;
            break;
        }
      }
      return bitData;
    }

    public static void CombineBitData(bool[,] combineInto, string data, char rowSep = '\n')
    {
      int index1 = 0;
      int index2 = 0;
      for (int index3 = 0; index3 < data.Length; ++index3)
      {
        switch (data[index3])
        {
          case '\n':
            index1 = 0;
            ++index2;
            break;
          case '0':
            ++index1;
            break;
          case '1':
            combineInto[index1, index2] = true;
            ++index1;
            break;
        }
      }
    }

    public static void CombineBitData(bool[,] combineInto, bool[,] data)
    {
      for (int index1 = 0; index1 < combineInto.GetLength(0); ++index1)
      {
        for (int index2 = 0; index2 < combineInto.GetLength(1); ++index2)
        {
          if (data[index1, index2])
            combineInto[index1, index2] = true;
        }
      }
    }

    public static int[] ConvertStringArrayToIntArray(string[] strings)
    {
      int[] intArray = new int[strings.Length];
      for (int index = 0; index < strings.Length; ++index)
        intArray[index] = Convert.ToInt32(strings[index]);
      return intArray;
    }

    public static float[] ConvertStringArrayToFloatArray(string[] strings)
    {
      float[] floatArray = new float[strings.Length];
      for (int index = 0; index < strings.Length; ++index)
        floatArray[index] = Convert.ToSingle(strings[index], (IFormatProvider) CultureInfo.InvariantCulture);
      return floatArray;
    }

    public static bool FileExists(string filename) => File.Exists(filename);

    public static bool SaveFile<T>(T obj, string filename) where T : new()
    {
      Stream stream = (Stream) new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
      try
      {
        new XmlSerializer(typeof (T)).Serialize(stream, (object) obj);
        stream.Close();
        return true;
      }
      catch
      {
        stream.Close();
        return false;
      }
    }

    public static bool LoadFile<T>(string filename, ref T data) where T : new()
    {
      Stream stream = (Stream) new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
      try
      {
        T obj = (T) new XmlSerializer(typeof (T)).Deserialize(stream);
        stream.Close();
        data = obj;
        return true;
      }
      catch
      {
        stream.Close();
        return false;
      }
    }

    public static XmlDocument LoadContentXML(string filename)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (Stream inStream = TitleContainer.OpenStream(Path.Combine(Engine.Instance.Content.RootDirectory, filename)))
        xmlDocument.Load(inStream);
      return xmlDocument;
    }

    public static XmlDocument LoadXML(string filename)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (FileStream inStream = File.OpenRead(filename))
        xmlDocument.Load((Stream) inStream);
      return xmlDocument;
    }

    public static bool ContentXMLExists(string filename) => File.Exists(Path.Combine(Engine.ContentDirectory, filename));

    public static bool XMLExists(string filename) => File.Exists(filename);

    public static bool HasAttr(this XmlElement xml, string attributeName) => xml.Attributes[attributeName] != null;

    public static string Attr(this XmlElement xml, string attributeName) => xml.Attributes[attributeName].InnerText;

    public static string Attr(this XmlElement xml, string attributeName, string defaultValue) => !xml.HasAttr(attributeName) ? defaultValue : xml.Attributes[attributeName].InnerText;

    public static int AttrInt(this XmlElement xml, string attributeName) => Convert.ToInt32(xml.Attributes[attributeName].InnerText);

    public static int AttrInt(this XmlElement xml, string attributeName, int defaultValue) => !xml.HasAttr(attributeName) ? defaultValue : Convert.ToInt32(xml.Attributes[attributeName].InnerText);

    public static float AttrFloat(this XmlElement xml, string attributeName) => Convert.ToSingle(xml.Attributes[attributeName].InnerText, (IFormatProvider) CultureInfo.InvariantCulture);

    public static float AttrFloat(this XmlElement xml, string attributeName, float defaultValue) => !xml.HasAttr(attributeName) ? defaultValue : Convert.ToSingle(xml.Attributes[attributeName].InnerText, (IFormatProvider) CultureInfo.InvariantCulture);

    public static Vector3 AttrVector3(this XmlElement xml, string attributeName)
    {
      string[] strArray = xml.Attr(attributeName).Split(',');
      return new Vector3(float.Parse(strArray[0].Trim(), (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1].Trim(), (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[2].Trim(), (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static Vector2 AttrVector2(
      this XmlElement xml,
      string xAttributeName,
      string yAttributeName)
    {
      return new Vector2(xml.AttrFloat(xAttributeName), xml.AttrFloat(yAttributeName));
    }

    public static Vector2 AttrVector2(
      this XmlElement xml,
      string xAttributeName,
      string yAttributeName,
      Vector2 defaultValue)
    {
      return new Vector2(xml.AttrFloat(xAttributeName, defaultValue.X), xml.AttrFloat(yAttributeName, defaultValue.Y));
    }

    public static bool AttrBool(this XmlElement xml, string attributeName) => Convert.ToBoolean(xml.Attributes[attributeName].InnerText);

    public static bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue) => !xml.HasAttr(attributeName) ? defaultValue : xml.AttrBool(attributeName);

    public static char AttrChar(this XmlElement xml, string attributeName) => Convert.ToChar(xml.Attributes[attributeName].InnerText);

    public static char AttrChar(this XmlElement xml, string attributeName, char defaultValue) => !xml.HasAttr(attributeName) ? defaultValue : xml.AttrChar(attributeName);

    public static T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
    {
      if (Enum.IsDefined(typeof (T), (object) xml.Attributes[attributeName].InnerText))
        return (T) Enum.Parse(typeof (T), xml.Attributes[attributeName].InnerText);
      throw new Exception("The attribute value cannot be converted to the enum type.");
    }

    public static T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct => !xml.HasAttr(attributeName) ? defaultValue : xml.AttrEnum<T>(attributeName);

    public static Color AttrHexColor(this XmlElement xml, string attributeName) => Calc.HexToColor(xml.Attr(attributeName));

    public static Color AttrHexColor(this XmlElement xml, string attributeName, Color defaultValue) => !xml.HasAttr(attributeName) ? defaultValue : xml.AttrHexColor(attributeName);

    public static Color AttrHexColor(
      this XmlElement xml,
      string attributeName,
      string defaultValue)
    {
      return !xml.HasAttr(attributeName) ? Calc.HexToColor(defaultValue) : xml.AttrHexColor(attributeName);
    }

    public static Vector2 Position(this XmlElement xml) => new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));

    public static Vector2 Position(this XmlElement xml, Vector2 defaultPosition) => new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));

    public static int X(this XmlElement xml) => xml.AttrInt("x");

    public static int X(this XmlElement xml, int defaultX) => xml.AttrInt("x", defaultX);

    public static int Y(this XmlElement xml) => xml.AttrInt("y");

    public static int Y(this XmlElement xml, int defaultY) => xml.AttrInt("y", defaultY);

    public static int Width(this XmlElement xml) => xml.AttrInt("width");

    public static int Width(this XmlElement xml, int defaultWidth) => xml.AttrInt("width", defaultWidth);

    public static int Height(this XmlElement xml) => xml.AttrInt("height");

    public static int Height(this XmlElement xml, int defaultHeight) => xml.AttrInt("height", defaultHeight);

    public static Rectangle Rect(this XmlElement xml) => new Rectangle(xml.X(), xml.Y(), xml.Width(), xml.Height());

    public static int ID(this XmlElement xml) => xml.AttrInt("id");

    public static int InnerInt(this XmlElement xml) => Convert.ToInt32(xml.InnerText);

    public static float InnerFloat(this XmlElement xml) => Convert.ToSingle(xml.InnerText, (IFormatProvider) CultureInfo.InvariantCulture);

    public static bool InnerBool(this XmlElement xml) => Convert.ToBoolean(xml.InnerText);

    public static T InnerEnum<T>(this XmlElement xml) where T : struct => Enum.IsDefined(typeof (T), (object) xml.InnerText) ? (T) Enum.Parse(typeof (T), xml.InnerText) : throw new Exception("The attribute value cannot be converted to the enum type.");

    public static Color InnerHexColor(this XmlElement xml) => Calc.HexToColor(xml.InnerText);

    public static bool HasChild(this XmlElement xml, string childName) => xml[childName] != null;

    public static string ChildText(this XmlElement xml, string childName) => xml[childName].InnerText;

    public static string ChildText(this XmlElement xml, string childName, string defaultValue) => xml.HasChild(childName) ? xml[childName].InnerText : defaultValue;

    public static int ChildInt(this XmlElement xml, string childName) => xml[childName].InnerInt();

    public static int ChildInt(this XmlElement xml, string childName, int defaultValue) => xml.HasChild(childName) ? xml[childName].InnerInt() : defaultValue;

    public static float ChildFloat(this XmlElement xml, string childName) => xml[childName].InnerFloat();

    public static float ChildFloat(this XmlElement xml, string childName, float defaultValue) => xml.HasChild(childName) ? xml[childName].InnerFloat() : defaultValue;

    public static bool ChildBool(this XmlElement xml, string childName) => xml[childName].InnerBool();

    public static bool ChildBool(this XmlElement xml, string childName, bool defaultValue) => xml.HasChild(childName) ? xml[childName].InnerBool() : defaultValue;

    public static T ChildEnum<T>(this XmlElement xml, string childName) where T : struct
    {
      if (Enum.IsDefined(typeof (T), (object) xml[childName].InnerText))
        return (T) Enum.Parse(typeof (T), xml[childName].InnerText);
      throw new Exception("The attribute value cannot be converted to the enum type.");
    }

    public static T ChildEnum<T>(this XmlElement xml, string childName, T defaultValue) where T : struct
    {
      if (!xml.HasChild(childName))
        return defaultValue;
      if (Enum.IsDefined(typeof (T), (object) xml[childName].InnerText))
        return (T) Enum.Parse(typeof (T), xml[childName].InnerText);
      throw new Exception("The attribute value cannot be converted to the enum type.");
    }

    public static Color ChildHexColor(this XmlElement xml, string childName) => Calc.HexToColor(xml[childName].InnerText);

    public static Color ChildHexColor(this XmlElement xml, string childName, Color defaultValue) => xml.HasChild(childName) ? Calc.HexToColor(xml[childName].InnerText) : defaultValue;

    public static Color ChildHexColor(this XmlElement xml, string childName, string defaultValue) => xml.HasChild(childName) ? Calc.HexToColor(xml[childName].InnerText) : Calc.HexToColor(defaultValue);

    public static Vector2 ChildPosition(this XmlElement xml, string childName) => xml[childName].Position();

    public static Vector2 ChildPosition(
      this XmlElement xml,
      string childName,
      Vector2 defaultValue)
    {
      return xml.HasChild(childName) ? xml[childName].Position(defaultValue) : defaultValue;
    }

    public static Vector2 FirstNode(this XmlElement xml) => xml["node"] == null ? Vector2.Zero : new Vector2((float) (int) xml["node"].AttrFloat("x"), (float) (int) xml["node"].AttrFloat("y"));

    public static Vector2? FirstNodeNullable(this XmlElement xml) => xml["node"] == null ? new Vector2?() : new Vector2?(new Vector2((float) (int) xml["node"].AttrFloat("x"), (float) (int) xml["node"].AttrFloat("y")));

    public static Vector2? FirstNodeNullable(this XmlElement xml, Vector2 offset) => xml["node"] == null ? new Vector2?() : new Vector2?(new Vector2((float) (int) xml["node"].AttrFloat("x"), (float) (int) xml["node"].AttrFloat("y")) + offset);

    public static Vector2[] Nodes(this XmlElement xml, bool includePosition = false)
    {
      XmlNodeList elementsByTagName = xml.GetElementsByTagName("node");
      if (elementsByTagName == null)
      {
        if (!includePosition)
          return new Vector2[0];
        return new Vector2[1]{ xml.Position() };
      }
      Vector2[] vector2Array;
      if (includePosition)
      {
        vector2Array = new Vector2[elementsByTagName.Count + 1];
        vector2Array[0] = xml.Position();
        for (int i = 0; i < elementsByTagName.Count; ++i)
          vector2Array[i + 1] = new Vector2((float) Convert.ToInt32(elementsByTagName[i].Attributes["x"].InnerText), (float) Convert.ToInt32(elementsByTagName[i].Attributes["y"].InnerText));
      }
      else
      {
        vector2Array = new Vector2[elementsByTagName.Count];
        for (int i = 0; i < elementsByTagName.Count; ++i)
          vector2Array[i] = new Vector2((float) Convert.ToInt32(elementsByTagName[i].Attributes["x"].InnerText), (float) Convert.ToInt32(elementsByTagName[i].Attributes["y"].InnerText));
      }
      return vector2Array;
    }

    public static Vector2[] Nodes(this XmlElement xml, Vector2 offset, bool includePosition = false)
    {
      Vector2[] vector2Array = xml.Nodes(includePosition);
      for (int index = 0; index < vector2Array.Length; ++index)
        vector2Array[index] += offset;
      return vector2Array;
    }

    public static Vector2 GetNode(this XmlElement xml, int nodeNum) => xml.Nodes()[nodeNum];

    public static Vector2? GetNodeNullable(this XmlElement xml, int nodeNum) => xml.Nodes().Length > nodeNum ? new Vector2?(xml.Nodes()[nodeNum]) : new Vector2?();

    public static void SetAttr(this XmlElement xml, string attributeName, object setTo)
    {
      XmlAttribute attribute;
      if (xml.HasAttr(attributeName))
      {
        attribute = xml.Attributes[attributeName];
      }
      else
      {
        attribute = xml.OwnerDocument.CreateAttribute(attributeName);
        xml.Attributes.Append(attribute);
      }
      attribute.Value = setTo.ToString();
    }

    public static void SetChild(this XmlElement xml, string childName, object setTo)
    {
      XmlElement element;
      if (xml.HasChild(childName))
      {
        element = xml[childName];
      }
      else
      {
        element = xml.OwnerDocument.CreateElement((string) null, childName, xml.NamespaceURI);
        xml.AppendChild((XmlNode) element);
      }
      element.InnerText = setTo.ToString();
    }

    public static XmlElement CreateChild(this XmlDocument doc, string childName)
    {
      XmlElement element = doc.CreateElement((string) null, childName, doc.NamespaceURI);
      doc.AppendChild((XmlNode) element);
      return element;
    }

    public static XmlElement CreateChild(this XmlElement xml, string childName)
    {
      XmlElement element = xml.OwnerDocument.CreateElement((string) null, childName, xml.NamespaceURI);
      xml.AppendChild((XmlNode) element);
      return element;
    }

    public static int SortLeftToRight(Entity a, Entity b) => (int) (((double) a.X - (double) b.X) * 100.0);

    public static int SortRightToLeft(Entity a, Entity b) => (int) (((double) b.X - (double) a.X) * 100.0);

    public static int SortTopToBottom(Entity a, Entity b) => (int) (((double) a.Y - (double) b.Y) * 100.0);

    public static int SortBottomToTop(Entity a, Entity b) => (int) (((double) b.Y - (double) a.Y) * 100.0);

    public static int SortByDepth(Entity a, Entity b) => a.Depth - b.Depth;

    public static int SortByDepthReversed(Entity a, Entity b) => b.Depth - a.Depth;

    public static void Log()
    {
    }

    public static void TimeLog()
    {
    }

    public static void Log(params object[] obj)
    {
      foreach (object obj1 in obj)
        ;
    }

    public static void TimeLog(object obj)
    {
    }

    public static void LogEach<T>(IEnumerable<T> collection)
    {
      foreach (T obj in collection)
        ;
    }

    public static void Dissect(object obj)
    {
      foreach (FieldInfo field in obj.GetType().GetFields())
        ;
    }

    public static void StartTimer()
    {
      Calc.stopwatch = new Stopwatch();
      Calc.stopwatch.Start();
    }

    public static void EndTimer()
    {
      if (Calc.stopwatch == null)
        return;
      Calc.stopwatch.Stop();
      // "Timer: " + (object) Calc.stopwatch.ElapsedTicks + " ticks, or " + TimeSpan.FromTicks(Calc.stopwatch.ElapsedTicks).TotalSeconds.ToString("00.0000000") + " seconds";
      Calc.stopwatch = (Stopwatch) null;
    }

    public static Delegate GetMethod<T>(object obj, string method) where T : class => obj.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) == (MethodInfo) null ? (Delegate) null : Delegate.CreateDelegate(typeof (T), obj, method);

    public static T At<T>(this T[,] arr, Pnt at) => arr[at.X, at.Y];

    public static string ConvertPath(string path) => path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

    public static string ReadNullTerminatedString(this BinaryReader stream)
    {
      string str = "";
      char ch;
      while ((ch = stream.ReadChar()) != char.MinValue)
        str += ch.ToString();
      return str;
    }

    public static IEnumerator Do(params IEnumerator[] numerators)
    {
      if (numerators.Length != 0)
      {
        if (numerators.Length == 1)
        {
          yield return (object) numerators[0];
        }
        else
        {
          List<Coroutine> routines = new List<Coroutine>();
          foreach (IEnumerator numerator in numerators)
            routines.Add(new Coroutine(numerator));
          while (true)
          {
            bool flag = false;
            foreach (Coroutine coroutine in routines)
            {
              coroutine.Update();
              if (!coroutine.Finished)
                flag = true;
            }
            if (flag)
              yield return (object) null;
            else
              break;
          }
          routines = (List<Coroutine>) null;
        }
      }
    }

    public static Rectangle ClampTo(this Rectangle rect, Rectangle clamp)
    {
      if (rect.X < clamp.X)
      {
        rect.Width -= clamp.X - rect.X;
        rect.X = clamp.X;
      }
      if (rect.Y < clamp.Y)
      {
        rect.Height -= clamp.Y - rect.Y;
        rect.Y = clamp.Y;
      }
      if (rect.Right > clamp.Right)
        rect.Width = clamp.Right - rect.X;
      if (rect.Bottom > clamp.Bottom)
        rect.Height = clamp.Bottom - rect.Y;
      return rect;
    }
  }
}
