using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public enum BackgroundType
    {
        None = 0,
        Front = 1,
        Middle = 2,
        Back = 3,
    }

    [YamlSerializable, Serializable]
    public class BackgroundData : CommandData
    {
        public override CommandType Type => CommandType.Background;
        public BackgroundType BackgroundType { get; set; }
        public string SourceGuid { get; set; }

        public override void Update(CommandData data)
        {
            if (data is BackgroundData backgroundData)
            {
                BackgroundType = backgroundData.BackgroundType;
                
                SourceGuid = string.IsNullOrEmpty(backgroundData.SourceGuid) ? SourceGuid : backgroundData.SourceGuid;
            }
        }

        [YamlIgnore] public static BackgroundData Default => new BackgroundData()
        {
            BackgroundType = BackgroundType.Back,
            Timing = CommandTiming.OnClick,
            SourceGuid = ""
        };
    }
}