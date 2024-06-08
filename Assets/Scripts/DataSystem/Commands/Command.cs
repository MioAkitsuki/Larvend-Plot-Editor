using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public enum CommandType
    {
        Text,
        Animation,
        Avatar,
        Background,
        Music,
        Selection,
        Sleep,
        Tachie,
        Goto
    }

    public enum CommandTiming
    {
        OnClick = 0,
        WithPrevious = 1,
        AfterPrevious = 2
    }

    [YamlSerializable, Serializable]
    public abstract class CommandData
    {
        public Guid Guid { get; set; }
        [YamlIgnore] public int Id => ProjectManager.GetCommandIndex(this);
        public abstract CommandType Type { get; }
        public CommandTiming Timing { get; set; } = CommandTiming.OnClick;
        public float Time { get; set; } = 0f;
        public bool Skippable { get; set; } = true;
        public abstract void Update(CommandData data);
    }

    public abstract class CommandSettings
    {

    }
}