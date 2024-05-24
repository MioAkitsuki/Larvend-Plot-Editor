using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor
{
    public class BackgroundController : MonoSingleton<BackgroundController>
    {
        private CanvasGroup mFrontCanvasGroup;
        private Image mFrontImage;
        private AspectRatioFitter mFrontRatio;
        private Image mFrontOldImage;
        private AspectRatioFitter mFrontOldRatio;

        private CanvasGroup mMiddleCanvasGroup;
        private Image mMiddleImage;
        private AspectRatioFitter mMiddleRatio;
        private Image mMiddleOldImage;
        private AspectRatioFitter mMiddleOldRatio;

        private CanvasGroup mBackCanvasGroup;
        private Image mBackImage;
        private AspectRatioFitter mBackRatio;
        private Image mBackOldImage;
        private AspectRatioFitter mBackOldRatio;

        public static Background mCurrentBackground;
        public static BackgroundType mCurrentType = BackgroundType.None;
        public static CanvasGroup mCurrentCanvasGroup;
        public static Image mCurrentImage;
        public static AspectRatioFitter mCurrentRatio;
        public static Image mCurrentOldImage;
        public static AspectRatioFitter mCurrentOldRatio;

        private static Coroutine mCurrentCoroutine;

        void Awake()
        {
            mFrontCanvasGroup = transform.Find("Front").GetComponent<CanvasGroup>();
            mFrontImage = mFrontCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mFrontRatio = mFrontImage.GetComponent<AspectRatioFitter>();
            mFrontOldImage = mFrontCanvasGroup.transform.Find("Old").GetComponent<Image>();
            mFrontOldRatio = mFrontOldImage.GetComponent<AspectRatioFitter>();

            mMiddleCanvasGroup = transform.Find("Middle").GetComponent<CanvasGroup>();
            mMiddleImage = mMiddleCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mMiddleRatio = mMiddleImage.GetComponent<AspectRatioFitter>();
            mMiddleOldImage = mMiddleCanvasGroup.transform.Find("Old").GetComponent<Image>();
            mMiddleOldRatio = mMiddleOldImage.GetComponent<AspectRatioFitter>();

            mBackCanvasGroup = transform.Find("Back").GetComponent<CanvasGroup>();
            mBackImage = mBackCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mBackRatio = mBackImage.GetComponent<AspectRatioFitter>();
            mBackOldImage = mBackCanvasGroup.transform.Find("Old").GetComponent<Image>();
            mBackOldRatio = mBackOldImage.GetComponent<AspectRatioFitter>();
        }

        private void Initialize()
        {
            mFrontCanvasGroup.alpha = 0;
            mMiddleCanvasGroup.alpha = 0;
            mBackCanvasGroup.alpha = 0;
        }

        private void SwitchType(BackgroundType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.2f));

            switch (mCurrentType)
            {
                case BackgroundType.Front:
                    mCurrentCanvasGroup = mFrontCanvasGroup;
                    mCurrentImage = mFrontImage;
                    mCurrentRatio = mFrontRatio;
                    mCurrentOldImage = mFrontOldImage;
                    mCurrentOldRatio = mFrontOldRatio;
                    break;
                case BackgroundType.Middle:
                    mCurrentCanvasGroup = mMiddleCanvasGroup;
                    mCurrentImage = mMiddleImage;
                    mCurrentRatio = mMiddleRatio;
                    mCurrentOldImage = mMiddleOldImage;
                    mCurrentOldRatio = mMiddleOldRatio;
                    break;
                case BackgroundType.Back:
                    mCurrentCanvasGroup = mBackCanvasGroup;
                    mCurrentImage = mBackImage;
                    mCurrentRatio = mBackRatio;
                    mCurrentOldImage = mBackOldImage;
                    mCurrentOldRatio = mBackOldRatio;
                    break;
                default:
                    mCurrentCanvasGroup = null;
                    mCurrentImage = null;
                    mCurrentOldImage = null;
                    break;
            }
        }

        public static void Execute(Background background)
        {
            if (background.backgroundType == BackgroundType.None) throw new System.Exception("BackgroundType is None");
            mCurrentBackground = background;
            if (mCurrentType != background.backgroundType) Instance.SwitchType(background.backgroundType);

            // if (background.appearMethod == Background.AppearMethod.Appear)
            // {
            //     mCurrentImage.sprite = mCurrentBackground.sprite ?? null;
            //     mCurrentCanvasGroup.alpha = 1;

            //     mCurrentBackground.Finish();
            //     return;
            // }

            if (mCurrentCoroutine != null)
            {
                Instance.StopCoroutine(mCurrentCoroutine);
            }
            mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeFromTransparentBackground());
            Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
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

        private IEnumerator Fade(Image image, float targetAlpha, float speed)
        {
            if (image.color.a == targetAlpha) yield break;

            while (Mathf.Abs(image.color.a - targetAlpha) >= 1f / 255)
            {
                image.color = image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.MoveTowards(image.color.a, targetAlpha, speed));
                yield return new WaitForFixedUpdate();
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
        }

        public static void Skip()
        {
            if (mCurrentCoroutine != null)
                Instance.StopCoroutine(mCurrentCoroutine);
            mCurrentCoroutine = null;

            mCurrentImage.sprite = mCurrentBackground.sprite ?? null;
        }

        public IEnumerator FadeFromBlackBackground()
        {
            mCurrentOldImage.sprite = null;
            mCurrentOldImage.color = new Color(0, 0, 0, 1);

            var time = mCurrentBackground.time == 0f ? 1f / mCurrentBackground.displaySpeed : mCurrentBackground.time;

            yield return Fade(mCurrentImage, 0, 0.1f / time);
            mCurrentImage.sprite = mCurrentBackground.sprite ?? null;
            yield return Fade(mCurrentImage, 1, 0.1f / time);

            mCurrentCoroutine = null;
            mCurrentBackground.Finish();
        }

        public IEnumerator FadeFromWhiteBackground()
        {
            mCurrentOldImage.sprite = null;
            mCurrentOldImage.color = new Color(1, 1, 1, 1);

            var time = mCurrentBackground.time == 0f ? 1f / mCurrentBackground.displaySpeed : mCurrentBackground.time;

            yield return Fade(mCurrentImage, 0, 0.1f / time);
            mCurrentImage.sprite = mCurrentBackground.sprite ?? null;
            yield return Fade(mCurrentImage, 1, 0.1f / time);

            mCurrentCoroutine = null;
            mCurrentBackground.Finish();
        }

        public IEnumerator FadeFromTransparentBackground()
        {
            mCurrentOldImage.sprite = null;
            mCurrentOldImage.color = new Color(1, 1, 1, 0);

            var time = mCurrentBackground.time == 0f ? 1f / mCurrentBackground.displaySpeed : mCurrentBackground.time;

            yield return Fade(mCurrentImage, 0, 0.1f / time);

            mCurrentImage.sprite = mCurrentBackground.sprite ?? null;
            mCurrentRatio.aspectRatio = mCurrentBackground.sprite.rect.width / mCurrentBackground.sprite.rect.height;
            
            yield return Fade(mCurrentImage, 1, 0.1f / time);

            mCurrentCoroutine = null;
            mCurrentBackground.Finish();
        }

        public IEnumerator CrossFadeBackground()
        {
            mCurrentOldImage.sprite = mCurrentImage.sprite ?? null;
            mCurrentOldImage.color = new Color(1, 1, 1, 1);
            mCurrentImage.color = new Color(1, 1, 1, 0);
            mCurrentImage.sprite = mCurrentBackground.sprite ?? null;

            var time = mCurrentBackground.time == 0f ? 1f / mCurrentBackground.displaySpeed : mCurrentBackground.time;

            StartCoroutine(Fade(mCurrentImage, 1, 0.1f / time));
            StartCoroutine(Fade(mCurrentOldImage, 0, 0.1f / time));

            yield return new WaitForSeconds(time);

            mCurrentCoroutine = null;
            mCurrentBackground.Finish();
        }
    }
}