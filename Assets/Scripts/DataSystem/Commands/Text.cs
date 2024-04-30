using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public enum TextType
    {
        None = 0,
        FullScreen = 1,
        Bottom = 2
    }

    [YamlSerializable, Serializable]
    public class TextData : CommandData
    {
        public override CommandType Type => CommandType.Text;
        public TextType TextType { get; set; }
        public string Speaker { get; set; }
        public string Content { get; set; }
        public string VoiceId { get; set; }

        public override void Update(CommandData data)
        {
            if (data is TextData textData)
            {
                TextType = textData.TextType;
                
                Speaker = string.IsNullOrEmpty(textData.Speaker) ? Speaker : textData.Speaker;
                Content = string.IsNullOrEmpty(textData.Content) ? Content : textData.Content;
                VoiceId = string.IsNullOrEmpty(textData.VoiceId) ? VoiceId : textData.VoiceId;
            }
        }

        [YamlIgnore] public static TextData Default => new TextData()
        {
            TextType = TextType.Bottom,
            Timing = CommandTiming.OnClick,
            Speaker = "Sample",
            Content = "Content",
            VoiceId = ""
        };
    }
}