using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend
{
    public class AvatarController : MonoBehaviour , ISingleton
    {
        public static AvatarController Instance
        {
            get { return MonoSingletonProperty<AvatarController>.Instance; }
        }

        private CanvasGroup mLeftCanvasGroup;
        private Image mLeftImage;

        private CanvasGroup mRightCanvasGroup;
        private Image mRightImage;

        public static Avatar mCurrentAvatar;
        public static Avatar.AvatarType mCurrentType = Avatar.AvatarType.None;
        public static CanvasGroup mCurrentCanvasGroup;
        public static Image mCurrentImage;

        private static Coroutine mCurrentCoroutine;

        void Awake()
        {
            mLeftCanvasGroup = transform.Find("Left").GetComponent<CanvasGroup>();
            mLeftImage = mLeftCanvasGroup.transform.Find("Image").GetComponent<Image>();

            mRightCanvasGroup = transform.Find("Right").GetComponent<CanvasGroup>();
            mRightImage = mRightCanvasGroup.transform.Find("Image").GetComponent<Image>();
        }

        private void Initialize()
        {
            mLeftCanvasGroup.alpha = 0;
            mRightCanvasGroup.alpha = 0;
        }

        private void SwitchType(Avatar.AvatarType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.2f));

            switch (mCurrentType)
            {
                case Avatar.AvatarType.Left:
                    mCurrentCanvasGroup = mLeftCanvasGroup;
                    mCurrentImage = mLeftImage;
                    break;
                case Avatar.AvatarType.Right:
                    mCurrentCanvasGroup = mRightCanvasGroup;
                    mCurrentImage = mRightImage;
                    break;
                default:
                    mCurrentCanvasGroup = null;
                    mCurrentImage = null;
                    break;
            }
        }

        public static void Execute(Avatar avatar)
        {
            if (avatar.avatarType == Avatar.AvatarType.None) throw new System.Exception("AvatarType is None");
            mCurrentAvatar = avatar;
            if (mCurrentType != avatar.avatarType) Instance.SwitchType(avatar.avatarType);

            if (avatar.appearMethod == Avatar.AppearMethod.Appear)
            {
                mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;
                mCurrentCanvasGroup.alpha = 1;

                mCurrentAvatar.Finish();
            }

            if (avatar.appearMethod == Avatar.AppearMethod.Fade)
            {
                if (mCurrentCoroutine != null)
                {
                    Instance.StopCoroutine(mCurrentCoroutine);
                }

                mCurrentCoroutine = Instance.StartCoroutine(Instance.FadeAvatar());
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

            mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;
        }

        public IEnumerator FadeAvatar()
        {
            yield return Fade(mCurrentCanvasGroup, 0, 0.2f);

            mCurrentImage.sprite = mCurrentAvatar.sprite ?? null;

            var speed = mCurrentAvatar.time == 0f ? 0.05f * mCurrentAvatar.displaySpeed : 0.05f / mCurrentAvatar.time;

            yield return Fade(mCurrentCanvasGroup, 1, speed);

            mCurrentCoroutine = null;
            mCurrentAvatar.Finish();
        }

        public void OnSingletonInit()
        {
            
        }
    }
}