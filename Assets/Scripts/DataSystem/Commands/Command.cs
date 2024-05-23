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
        Sound,
        Avatar,
        Tachie,
        Background,
        Selection
    }

    public enum CommandTiming
    {
        OnClick = 0,
        WithPrevious = 1,
        AfterPrevious = 2
    }

    [YamlSerializable]
    public abstract class CommandData
    {
        public Guid Guid { get; set; }
        [YamlIgnore] public int Id => ProjectManager.GetCommandIndex(this);
        public abstract CommandType Type { get; }
        public CommandTiming Timing { get; set; }
        public abstract void Update(CommandData data);
    }

    public abstract class CommandSettings
    {

    }
}