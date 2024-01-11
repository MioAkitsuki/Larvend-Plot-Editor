using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend
{
    public class TextController : MonoBehaviour , ISingleton
    {
        public static TextController Instance
        {
            get { return MonoSingletonProperty<TextController>.Instance; }
        }

        private CanvasGroup mFullScreenCanvasGroup;
        private TMP_Text mFullScreenText;
        private TMP_Text mFullScreenSpeaker;

        private CanvasGroup mByBarCanvasGroup;
        private TMP_Text mByBarText;
        private TMP_Text mByBarSpeaker;

        public static Text mCurrentText;
        public static Text.TextType mCurrentType = Text.TextType.None;
        public static CanvasGroup mCurrentCanvasGroup;
        public static TMP_Text mCurrentSpeakerField;
        public static TMP_Text mCurrentTextField;

        private static Coroutine mCurrentCoroutine;

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

        private void SwitchType(Text.TextType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.2f));

            TextConfig textConfig, speakerConfig;
            switch (mCurrentType)
            {
                case Text.TextType.FullScreen:
                    mCurrentCanvasGroup = mFullScreenCanvasGroup;
                    mCurrentSpeakerField = mFullScreenSpeaker;
                    mCurrentTextField = mFullScreenText;
                    textConfig = TextConfig.DefaultFullScreen;
                    speakerConfig = TextConfig.DefaultFullScreenSpeaker;
                    break;
                case Text.TextType.ByBar:
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

        public static void SetConfig(TextConfig textConfig, TextConfig speakerConfig)
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

        public static void Execute(Text text)
        {
            if (text.textType == Text.TextType.None) throw new System.Exception("TextType is None");
            mCurrentText = text;
            if (mCurrentType != text.textType) Instance.SwitchType(text.textType);

            if (text.appearMethod == Text.AppearMethod.TypeWriter)
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

            if (text.appearMethod == Text.AppearMethod.Fade)
            {
                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeText());
            }

            if (text.appearMethod == Text.AppearMethod.FadeFromBlank)
            {
                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeFromBlankText());
            }
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

        public static void Skip()
        {
            if (mCurrentCoroutine != null)
                Instance.StopCoroutine(mCurrentCoroutine);
            mCurrentCoroutine = null;

            mCurrentTextField.text = mCurrentText.text;
        }

        public IEnumerator FadeText()
        {
            while (!Mathf.Approximately(mCurrentTextField.alpha, 0f))
            {
                mCurrentTextField.alpha = Mathf.MoveTowards(mCurrentTextField.alpha, 0f, 0.2f);
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
                mCurrentTextField.alpha = Mathf.MoveTowards(mCurrentTextField.alpha, 0f, speed);
                yield return new WaitForFixedUpdate();
            }
            mCurrentTextField.alpha = 0f;

            yield return new WaitForSeconds(mCurrentText.time == 0f ? 1f : mCurrentText.time);

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

        public void OnSingletonInit()
        {
            
        }
    }
}