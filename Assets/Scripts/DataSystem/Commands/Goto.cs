using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    [YamlSerializable, Serializable]
    public class GotoData : CommandData
    {
        public override CommandType Type => CommandType.Goto;
        public Guid GotoGuid { get; set; }

        public override void Update(CommandData data)
        {
            if (data is GotoData gotoData)
            {
                GotoGuid = gotoData.GotoGuid == Guid.Empty ? GotoGuid : gotoData.GotoGuid;
            }
        }

        [YamlIgnore] public static GotoData Default => new GotoData()
        {
            Timing = CommandTiming.OnClick,
            Time = 0f
        };
    }
}