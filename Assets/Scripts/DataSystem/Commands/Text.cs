using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public enum TextType
    {
        None,
        FullScreen,
        Bottom
    }

    [YamlSerializable]
    public class TextData : CommandData
    {
        public override CommandType Type => CommandType.Text;
        public TextType TextType { get; set; }
        public string Speaker { get; set; }
        public string Content { get; set; }
        public string VoiceId { get; set; }

        [YamlIgnore] public static TextData Default => new TextData()
        {
            TextType = TextType.Bottom,
            Speaker = "Sample",
            Content = "Content",
            VoiceId = ""
        };
    }
}