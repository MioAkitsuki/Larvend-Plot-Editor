using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend
{
    public class BackgroundController : MonoBehaviour , ISingleton
    {
        public static BackgroundController Instance
        {
            get { return MonoSingletonProperty<BackgroundController>.Instance; }
        }

        private CanvasGroup mFrontCanvasGroup;
        private Image mFrontImage;
        private Image mFrontOldImage;

        private CanvasGroup mMiddleCanvasGroup;
        private Image mMiddleImage;
        private Image mMiddleOldImage;

        private CanvasGroup mBackCanvasGroup;
        private Image mBackImage;
        private Image mBackOldImage;

        public static Background mCurrentBackground;
        public static Background.BackgroundType mCurrentType = Background.BackgroundType.None;
        public static CanvasGroup mCurrentCanvasGroup;
        public static Image mCurrentImage;
        public static Image mCurrentOldImage;

        private static Coroutine mCurrentCoroutine;

        void Awake()
        {
            mFrontCanvasGroup = transform.Find("Front").GetComponent<CanvasGroup>();
            mFrontImage = mFrontCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mFrontOldImage = mFrontCanvasGroup.transform.Find("Old").GetComponent<Image>();

            mMiddleCanvasGroup = transform.Find("Middle").GetComponent<CanvasGroup>();
            mMiddleImage = mMiddleCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mFrontOldImage = mMiddleCanvasGroup.transform.Find("Old").GetComponent<Image>();

            mBackCanvasGroup = transform.Find("Back").GetComponent<CanvasGroup>();
            mBackImage = mBackCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mBackOldImage = mBackCanvasGroup.transform.Find("Old").GetComponent<Image>();
        }

        private void Initialize()
        {
            mFrontCanvasGroup.alpha = 0;
            mMiddleCanvasGroup.alpha = 0;
            mBackCanvasGroup.alpha = 0;
        }

        private void SwitchType(Background.BackgroundType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.2f));

            switch (mCurrentType)
            {
                case Background.BackgroundType.Front:
                    mCurrentCanvasGroup = mFrontCanvasGroup;
                    mCurrentImage = mFrontImage;
                    mCurrentOldImage = mFrontOldImage;
                    break;
                case Background.BackgroundType.Middle:
                    mCurrentCanvasGroup = mMiddleCanvasGroup;
                    mCurrentImage = mMiddleImage;
                    mCurrentOldImage = mMiddleOldImage;
                    break;
                case Background.BackgroundType.Back:
                    mCurrentCanvasGroup = mBackCanvasGroup;
                    mCurrentImage = mBackImage;
                    mCurrentOldImage = mBackOldImage;
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
            if (background.backgroundType == Background.BackgroundType.None) throw new System.Exception("BackgroundType is None");
            mCurrentBackground = background;
            if (mCurrentType != background.backgroundType) Instance.SwitchType(background.backgroundType);

            if (background.appearMethod == Background.AppearMethod.Appear)
            {
                mCurrentImage.sprite = mCurrentBackground.sprite ?? null;
                mCurrentCanvasGroup.alpha = 1;

                mCurrentBackground.Finish();
            }

            if (background.appearMethod == Background.AppearMethod.CrossFade)
            {
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.CrossFadeBackground());
                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
            }

            if (background.appearMethod == Background.AppearMethod.FadeFromBlack)
            {
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeFromBlackBackground());
                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 1f, 0.2f));
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

        private IEnumerator Fade(Image image, float targetAlpha, float speed)
        {
            if (image.color.a == targetAlpha) yield break;

            while (Mathf.Abs(image.color.a - targetAlpha) >= 1f / 255)
            {
                image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, targetAlpha), speed);
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

        public void OnSingletonInit()
        {
            
        }
    }
}