using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor
{
    public class AvatarController : MonoSingleton<AvatarController>
    {
        private CanvasGroup mLeftCanvasGroup;
        private Image mLeftImage;
        private Image mLeftOldImage;

        private CanvasGroup mRightCanvasGroup;
        private Image mRightImage;
        private Image mRightOldImage;

        public static Avatar mCurrentAvatar;
        public static AvatarType mCurrentType = AvatarType.None;
        public static CanvasGroup mCurrentCanvasGroup;
        public static Image mCurrentImage;
        public static Image mCurrentOldImage;

        private static Coroutine mCurrentCoroutine;

        void Awake()
        {
            mLeftCanvasGroup = transform.Find("Left").GetComponent<CanvasGroup>();
            mLeftImage = mLeftCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mLeftOldImage = mLeftCanvasGroup.transform.Find("Old").GetComponent<Image>();

            mRightCanvasGroup = transform.Find("Right").GetComponent<CanvasGroup>();
            mRightImage = mRightCanvasGroup.transform.Find("Image").GetComponent<Image>();
            mRightOldImage = mRightCanvasGroup.transform.Find("Old").GetComponent<Image>();
        }

        private void Initialize()
        {
            mLeftCanvasGroup.alpha = 0;
            mRightCanvasGroup.alpha = 0;
        }

        private void SwitchType(AvatarType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.2f));

            switch (mCurrentType)
            {
                case AvatarType.BottomLeft:
                    mCurrentCanvasGroup = mLeftCanvasGroup;
                    mCurrentImage = mLeftImage;
                    mCurrentOldImage = mLeftOldImage;
                    break;
                default:
                    mCurrentCanvasGroup = null;
                    mCurrentImage = null;
                    mCurrentOldImage = null;
                    break;
            }
        }

        public static void Execute(Avatar avatar)
        {
            if (avatar.avatarType == AvatarType.None)
            {
                Instance.StartCoroutine(Instance.Fade(mCurrentCanvasGroup, 0f, 0.2f));
                mCurrentAvatar.Finish();
                return;
            }

            mCurrentAvatar = avatar;
            if (mCurrentType != avatar.avatarType) Instance.SwitchType(avatar.avatarType);

            // if (avatar.appearMethod == Avatar.AppearMethod.Appear)
            // {
            //     mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;
            //     mCurrentCanvasGroup.alpha = 1;

            //     mCurrentAvatar.Finish();
            //     return;
            // }

            if (mCurrentCoroutine != null)
            {
                Instance.StopCoroutine(mCurrentCoroutine);
            }
            mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeFromTransparentAvatar());
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
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.MoveTowards(image.color.a, targetAlpha, speed));
                yield return new WaitForFixedUpdate();
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
        }

        public static void Skip()
        {
            if (mCurrentCoroutine != null)
                Instance.StopCoroutine(mCurrentCoroutine);
            mCurrentCoroutine = null;

            mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;
        }

        public IEnumerator FadeFromTransparentAvatar()
        {
            mCurrentOldImage.sprite = null;
            mCurrentOldImage.color = new Color(1, 1, 1, 0);

            var time = mCurrentAvatar.time == 0f ? 1f / mCurrentAvatar.displaySpeed : mCurrentAvatar.time;

            yield return Fade(mCurrentImage, 0, 0.1f / time);
            mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;
            yield return Fade(mCurrentImage, 1, 0.1f / time);

            mCurrentCoroutine = null;
            mCurrentAvatar.Finish();
        }

        public IEnumerator CrossFadeAvatar()
        {
            mCurrentOldImage.sprite = mCurrentImage.sprite ?? null;
            mCurrentOldImage.color = mCurrentImage.color;
            mCurrentImage.color = new Color(1, 1, 1, 0);
            mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;

            var time = mCurrentAvatar.time == 0f ? 1f / mCurrentAvatar.displaySpeed : mCurrentAvatar.time;

            StartCoroutine(Fade(mCurrentImage, 1, 0.1f / time));
            StartCoroutine(Fade(mCurrentOldImage, 0, 0.1f / time));

            yield return new WaitForSeconds(time);

            mCurrentCoroutine = null;
            mCurrentAvatar.Finish();
        }
    }
}