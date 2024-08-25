using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor.Utils
{
    public class LoadingMask : MonoSingleton<LoadingMask>
    {
        private CanvasGroup canvasGroup;
        private Coroutine currentCoroutine = null;

        public static bool IsActive => Instance.canvasGroup.alpha == 1;
        public static bool IsInactive => Instance.canvasGroup.alpha == 0;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public static Coroutine FadeIn()
        {
            if (Instance.currentCoroutine != null)
            {
                Instance.StopCoroutine(Instance.currentCoroutine);
            }
            return Instance.currentCoroutine = Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.canvasGroup, 1f, 0.2f));
        }

        public static Coroutine FadeOut()
        {
            if (Instance.currentCoroutine != null)
            {
                Instance.StopCoroutine(Instance.currentCoroutine);
            }
            return Instance.currentCoroutine = Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.canvasGroup, 0f, 0.2f));
        }
    }

}