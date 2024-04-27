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
        Background
    }

    [YamlSerializable]
    public abstract class CommandData
    {
        public abstract CommandType Type { get; }
    }

    public abstract class CommandSettings
    {

    }
}