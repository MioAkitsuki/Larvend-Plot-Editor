using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    [YamlSerializable, Serializable]
    public class ProjectData
    {
        [YamlMember] public string Name { get; set; }
        [YamlMember] public string Description { get; set; }
        [YamlMember] public List<CommandData> Commands { get; set; } = new();
    }
}