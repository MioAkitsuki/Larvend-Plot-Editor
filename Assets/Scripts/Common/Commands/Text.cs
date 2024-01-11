using System;
using TMPro;
using UnityEngine;

namespace Larvend
{
    [Serializable]
    public class Text : CommandBase
    {
        public enum TextType
        {
            FullScreen,
            ByBar,
            ByLeftBox,
            ByRightBox
        }

        public AppearMethod appearMethod;

        public TextType textType;
        public string speaker;
        public string text;
        public TextConfig textConfig;
        public AudioClip voice;

        public Vector2 positionOffset;
        public float displaySpeed = 1;

        private bool isFinished = false;

        public override CommandType GetCommandType()
        {
            return CommandType.Text;
        }

        public override void OnEnter()
        {
            isFinished = false;
        }

        public override void OnUpdate()
        {
            isFinished = true;
        }

        public override void OnExit()
        {
            return;
        }

        public override bool IsFinished()
        {
            return isFinished;
        }
    }

    [Serializable]
    public class TextConfig
    {
        public int FontSize;
        public Color FontColor;
        public FontStyles FontStyle;
        public TextAlignmentOptions TextAlignmentOption;

        public static TextConfig DefaultFullScreen => new TextConfig() {
            FontSize = 36,
            FontColor = Color.white,
            FontStyle = FontStyles.Normal,
            TextAlignmentOption = TextAlignmentOptions.Midline | TextAlignmentOptions.Center
        };

        public static TextConfig DefaultFullScreenSpeaker => new TextConfig() {
            FontSize = 32,
            FontColor = new Color(0.8f, 0.8f, 0.8f, 1f),
            FontStyle = FontStyles.Normal,
            TextAlignmentOption = TextAlignmentOptions.Midline | TextAlignmentOptions.Right
        };

        public static TextConfig DefaultByBar => new TextConfig() {
            FontSize = 36,
            FontColor = Color.white,
            FontStyle = FontStyles.Normal,
            TextAlignmentOption = TextAlignmentOptions.Top | TextAlignmentOptions.Left
        };

        public static TextConfig DefaultByBarSpeaker => new TextConfig() {
            FontSize = 32,
            FontColor = new Color(0.8f, 0.8f, 0.8f, 1f),
            FontStyle = FontStyles.Underline,
            TextAlignmentOption = TextAlignmentOptions.Midline | TextAlignmentOptions.Left
        };
    }
}