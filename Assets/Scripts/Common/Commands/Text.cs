using System;
using TMPro;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public class Text : Command
    {
        // public enum AppearMethod
        // {
        //     Appear,
        //     Fade,
        //     FadeFromBlank,
        //     TypeWriter
        // }

        // public AppearMethod appearMethod;

        public TextType textType;
        public string speaker;
        public string text;
        public TextConfig textConfig = null;
        public TextConfig speakerConfig = null;
        public AudioClip voice;

        public Vector2 positionOffset;
        public float displaySpeed = 1;

        private bool isFinished = false;

        public override CommandType GetCommandType() => CommandType.Text;
        public Text(TextData _data)
        {
            Data = _data;

            speaker = _data.Speaker;
            text = _data.Content;
            textType = _data.TextType;
        }

        public override void OnEnter()
        {
            isFinished = false;
            TextController.Instance.Execute(this);
        }

        public override void OnUpdate()
        {
            return;
        }

        public override void OnExit()
        {
            return;
        }

        public override void Skip()
        {
            TextController.Instance.Skip();
            Finish();
        }

        public override bool IsFinished()
        {
            return isFinished;
        }

        public void Finish()
        {
            isFinished = true;
        }
    }

    [Serializable]
    public class TextConfig
    {
        public int FontSize;
        public Color Color;
        public FontStyles FontStyle;
        public TextAlignmentOptions TextAlignmentOption;

        public static TextConfig DefaultFullScreen => new TextConfig() {
            FontSize = 36,
            Color = Color.white,
            FontStyle = FontStyles.Normal,
            TextAlignmentOption = TextAlignmentOptions.Midline | TextAlignmentOptions.Center
        };

        public static TextConfig DefaultFullScreenSpeaker => new TextConfig() {
            FontSize = 32,
            Color = new Color(0.8f, 0.8f, 0.8f, 1f),
            FontStyle = FontStyles.Normal,
            TextAlignmentOption = TextAlignmentOptions.Midline | TextAlignmentOptions.Right
        };

        public static TextConfig DefaultByBar => new TextConfig() {
            FontSize = 36,
            Color = Color.white,
            FontStyle = FontStyles.Normal,
            TextAlignmentOption = TextAlignmentOptions.Top | TextAlignmentOptions.Left
        };

        public static TextConfig DefaultByBarSpeaker => new TextConfig() {
            FontSize = 32,
            Color = new Color(0.8f, 0.8f, 0.8f, 1f),
            FontStyle = FontStyles.Underline,
            TextAlignmentOption = TextAlignmentOptions.Midline | TextAlignmentOptions.Left
        };
    }
}