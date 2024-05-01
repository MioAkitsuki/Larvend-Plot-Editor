using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public enum AvatarType
    {
        None = 0,
        BottomLeft = 1
    }

    [YamlSerializable, Serializable]
    public class AvatarData : CommandData
    {
        public override CommandType Type => CommandType.Avatar;
        public AvatarType AvatarType { get; set; }
        public string SourceGuid { get; set; }

        public override void Update(CommandData data)
        {
            if (data is AvatarData avatarData)
            {
                AvatarType = avatarData.AvatarType;
                
                SourceGuid = string.IsNullOrEmpty(avatarData.SourceGuid) ? SourceGuid : avatarData.SourceGuid;
            }
        }

        [YamlIgnore] public static AvatarData Default => new AvatarData()
        {
            AvatarType = AvatarType.BottomLeft,
            Timing = CommandTiming.OnClick,
            SourceGuid = ""
        };
    }
}