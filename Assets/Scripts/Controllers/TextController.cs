using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public class TextController : MonoSingleton<TextController>
    {
        private CanvasGroup mFullScreenCanvasGroup;
        private TMP_Text mFullScreenText;
        private TMP_Text mFullScreenSpeaker;

        private CanvasGroup mByBarCanvasGroup;
        private TMP_Text mByBarText;
        private TMP_Text mByBarSpeaker;

        public Text mCurrentText;
        public TextType mCurrentType = TextType.None;
        public CanvasGroup mCurrentCanvasGroup;
        public TMP_Text mCurrentSpeakerField;
        public TMP_Text mCurrentTextField;

        private Coroutine mCurrentCoroutine;

        void Awake()
        {
            mFullScreenCanvasGroup = transform.Find("FullScreen").GetComponent<CanvasGroup>();
            mFullScreenText = mFullScreenCanvasGroup.transform.Find("Text").GetComponent<TMP_Text>();
            mFullScreenSpeaker = mFullScreenCanvasGroup.transform.Find("Speaker").GetComponent<TMP_Text>();

            mByBarCanvasGroup = transform.Find("ByBar").GetComponent<CanvasGroup>();
            mByBarText = mByBarCanvasGroup.transform.Find("Text").GetComponent<TMP_Text>();
            mByBarSpeaker = mByBarCanvasGroup.transform.Find("Speaker").GetComponent<TMP_Text>();
        }

        private void Initialize()
        {
            mFullScreenCanvasGroup.alpha = 0;
            mByBarCanvasGroup.alpha = 0;

            mFullScreenSpeaker.text = "";
            mFullScreenText.text = "";
            mByBarSpeaker.text = "";
            mByBarText.text = "";
        }

        private void SwitchType(TextType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.1f));

            mCurrentTextField?.SetText("");
            mCurrentSpeakerField?.SetText("");

            TextConfig textConfig, speakerConfig;
            switch (mCurrentType)
            {
                case TextType.None:
                    mCurrentCanvasGroup = null;
                    mCurrentSpeakerField = null;
                    mCurrentTextField = null;
                    mCurrentText.Finish();
                    break;
                case TextType.FullScreen:
                    mCurrentCanvasGroup = mFullScreenCanvasGroup;
                    mCurrentSpeakerField = mFullScreenSpeaker;
                    mCurrentTextField = mFullScreenText;
                    textConfig = TextConfig.DefaultFullScreen;
                    speakerConfig = TextConfig.DefaultFullScreenSpeaker;
                    break;
                case TextType.Bottom:
                    mCurrentCanvasGroup = mByBarCanvasGroup;
                    mCurrentSpeakerField = mByBarSpeaker;
                    mCurrentTextField = mByBarText;
                    textConfig = TextConfig.DefaultByBar;
                    speakerConfig = TextConfig.DefaultByBarSpeaker;
                    break;
                default:
                    textConfig = mCurrentText.textConfig ?? TextConfig.DefaultFullScreen;
                    speakerConfig = mCurrentText.speakerConfig ?? TextConfig.DefaultFullScreenSpeaker;
                    break;
            }

            // SetConfig(textConfig, speakerConfig);
        }

        public void SetConfig(TextConfig textConfig, TextConfig speakerConfig)
        {
            mCurrentTextField.alignment = textConfig.TextAlignmentOption;
            mCurrentTextField.fontSize = textConfig.FontSize;
            mCurrentTextField.fontStyle = textConfig.FontStyle;
            mCurrentTextField.color = textConfig.Color;

            mCurrentSpeakerField.alignment = speakerConfig.TextAlignmentOption;
            mCurrentSpeakerField.fontSize = speakerConfig.FontSize;
            mCurrentSpeakerField.fontStyle = speakerConfig.FontStyle;
            mCurrentSpeakerField.color = speakerConfig.Color;
        }

        public void Execute(Text text)
        {
            mCurrentText = text;
            if (mCurrentType != text.textType) Instance.SwitchType(text.textType);

            if (mCurrentType == TextType.Bottom)
            {
                mCurrentSpeakerField.SetText(text.speaker);
                mCurrentTextField.SetText("");

                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.TypeText());
            }
            else if (mCurrentType == TextType.FullScreen)
            {
                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeFromBlankText());
            }

            // if (text.appearMethod == Text.AppearMethod.Fade)
            // {
            //     Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
            //     if (mCurrentCoroutine != null)
            //     {
            //         Instance.StopCoroutine(mCurrentCoroutine);
            //     }

            //     mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeText());
            // }

            
        }

        private IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha, float speed)
        {
            if (canvasGroup.alpha == targetAlpha) yield break;

            while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, speed);
                yield return new WaitForFixedUpdate();
            }
            canvasGroup.alpha = targetAlpha;
        }

        public void Skip()
        {
            if (mCurrentCoroutine != null)
                Instance.StopCoroutine(mCurrentCoroutine);
            mCurrentCoroutine = null;

            if (mCurrentType != TextType.None) mCurrentTextField.text = mCurrentText.text;
        }

        public IEnumerator FadeText()
        {
            while (!Mathf.Approximately(mCurrentTextField.alpha, 0f))
            {
                mCurrentTextField.alpha = Mathf.MoveTowards(mCurrentTextField.alpha, 0f, 0.1f);
                yield return new WaitForFixedUpdate();
            }
            mCurrentTextField.alpha = 0f;

            mCurrentTextField.SetText(mCurrentText.text);
            mCurrentSpeakerField.SetText(mCurrentText.speaker);

            var speed = mCurrentText.time == 0f ? 0.05f * mCurrentText.displaySpeed : 0.05f / mCurrentText.time;

            while (!Mathf.Approximately(mCurrentTextField.alpha, 1f))
            {
                mCurrentTextField.alpha = Mathf.MoveTowards(mCurrentTextField.alpha, 1f, speed);
                yield return new WaitForFixedUpdate();
            }
            mCurrentTextField.alpha = 1f;

            mCurrentCoroutine = null;
            mCurrentText.Finish();
        }

        public IEnumerator FadeFromBlankText()
        {
            var speed = mCurrentText.time == 0f ? 0.05f * mCurrentText.displaySpeed : 0.05f / mCurrentText.time;

            while (!Mathf.Approximately(mCurrentTextField.alpha, 0f))
            {
                mCurrentTextField.alpha = Mathf.MoveTowards(mCurrentTextField.alpha, 0f, speed * 2);
                yield return new WaitForFixedUpdate();
            }
            mCurrentTextField.alpha = 0f;

            yield return new WaitForSeconds(0.5f);

            mCurrentTextField.SetText(mCurrentText.text);
            mCurrentSpeakerField.SetText(mCurrentText.speaker);

            while (!Mathf.Approximately(mCurrentTextField.alpha, 1f))
            {
                mCurrentTextField.alpha = Mathf.MoveTowards(mCurrentTextField.alpha, 1f, speed);
                yield return new WaitForFixedUpdate();
            }
            mCurrentTextField.alpha = 1f;

            mCurrentCoroutine = null;
            mCurrentText.Finish();
        }

        public IEnumerator TypeText()
        {
            mCurrentTextField.text = "";

            var len = mCurrentText.text.Length;
            var text = mCurrentText.text;
            var speed = 1 / 12f / mCurrentText.displaySpeed;
            
            for (var i = 0; i < len; i++)
            {
                mCurrentTextField.text += text[i];
                yield return new WaitForSeconds(speed);
            }

            mCurrentCoroutine = null;
            mCurrentText.Finish();
        }
    }
}