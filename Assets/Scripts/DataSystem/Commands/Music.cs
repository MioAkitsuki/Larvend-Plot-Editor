using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    [YamlSerializable, Serializable]
    public class MusicData : CommandData
    {
        public override CommandType Type => CommandType.Music;
        public bool IsLoop { get; set; }
        public string SourceGuid { get; set; }

        public override void Update(CommandData data)
        {
            if (data is MusicData musicData)
            {
                IsLoop = musicData.IsLoop;

                SourceGuid = string.IsNullOrEmpty(musicData.SourceGuid) ? SourceGuid : musicData.SourceGuid;
            }
        }

        [YamlIgnore] public static MusicData Default => new MusicData()
        {
            Timing = CommandTiming.OnClick,
            IsLoop = false,
            SourceGuid = ""
        };
    }
}