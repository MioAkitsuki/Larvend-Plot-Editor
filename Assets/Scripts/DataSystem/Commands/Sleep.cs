using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    [YamlSerializable, Serializable]
    public class SleepData : CommandData
    {
        public override CommandType Type => CommandType.Sleep;
        public float Time { get; set; }

        public override void Update(CommandData data)
        {
            if (data is SleepData sleepData)
            {
                Time = sleepData.Time == 0f ? Time : sleepData.Time;
            }
        }

        [YamlIgnore] public static SleepData Default => new SleepData()
        {
            Timing = CommandTiming.OnClick,
            Time = 1f
        };
    }
}