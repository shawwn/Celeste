// Decompiled with JetBrains decompiler
// Type: Celeste.CheckpointData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using System.Collections.Generic;

namespace Celeste
{
    public class CheckpointData
    {
        public string Level;
        public string Name;
        public bool Dreaming;
        public int Strawberries;
        public string ColorGrade;
        public PlayerInventory? Inventory;
        public AudioState AudioState;
        public HashSet<string> Flags;
        public Session.CoreModes? CoreMode;

        public CheckpointData(
            string level,
            string name,
            PlayerInventory? inventory = null,
            bool dreaming = false,
            AudioState audioState = null)
        {
            this.Level = level;
            this.Name = name;
            this.Inventory = inventory;
            this.Dreaming = dreaming;
            this.AudioState = audioState;
            this.CoreMode = new Session.CoreModes?();
            this.ColorGrade = (string) null;
        }
    }
}