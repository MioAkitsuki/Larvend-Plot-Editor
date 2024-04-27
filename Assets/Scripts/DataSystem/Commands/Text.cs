using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larvend.PlotEditor.DataSystem
{
    public enum TextType
    {
        None,
        FullScreen,
        Bottom
    }

    public class TextData : CommandData
    {
        public TextType Type;
        public string Speaker;
        public string Content;
        public string VoiceId;
    }
}