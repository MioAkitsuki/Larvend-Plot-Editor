using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using Larvend.PlotEditor.UI;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor
{
    public class SelectionController : MonoSingleton<SelectionController>
    {
        public GameObject OptionPrefab;

        private CanvasGroup mFullScreenCanvasGroup;
        private Transform mFullScreenContent;

        private Selection mCurrentSelection;
        private SelectionType mCurrentType = SelectionType.None;
        private CanvasGroup mCurrentCanvasGroup;
        private Transform mCurrentContent;

        private Coroutine mCurrentCoroutine;

        void Awake()
        {
            mFullScreenCanvasGroup = transform.Find("FullScreen").GetComponent<CanvasGroup>();
            mFullScreenContent = transform.Find("FullScreen/Options");
        }

        private void Initialize()
        {
            mFullScreenCanvasGroup.alpha = 0;
        }

        private void SwitchType(SelectionType type)
        {
            mCurrentType = type;
            if (mCurrentCanvasGroup) StartCoroutine(Fade(mCurrentCanvasGroup, 0, 0.2f));

            switch (mCurrentType)
            {
                case SelectionType.FullScreen:
                    mCurrentCanvasGroup = mFullScreenCanvasGroup;
                    mCurrentContent = mFullScreenContent;
                    break;
                default:
                    mCurrentCanvasGroup = null;
                    mCurrentContent = null;
                    break;
            }
        }

        public static void Execute(Selection selection)
        {
            if (selection.selectionType == SelectionType.None)
            {
                Instance.StartCoroutine(Instance.Fade(Instance.mCurrentCanvasGroup, 0f, 0.2f));
                Instance.mCurrentSelection.Finish();
                return;
            }

            Instance.mCurrentSelection = selection;
            if (Instance.mCurrentType != selection.selectionType) Instance.SwitchType(selection.selectionType);

            Instance.mCurrentContent.DestroyChildren();
            foreach (var option in (selection.Data as SelectionData).Options)
            {
                Instantiate(Instance.OptionPrefab, Instance.mCurrentContent).GetComponent<OptionController>().Initialize(option);
            }

            if (Instance.mCurrentCoroutine != null)
            {
                Instance.StopCoroutine(Instance.mCurrentCoroutine);
            }
            Instance.mCurrentCoroutine = Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCurrentCanvasGroup, 1f));
        }

        public static void Exit()
        {
            if (Instance.mCurrentCoroutine != null)
            {
                Skip();
            }
            Instance.mCurrentCoroutine = Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCurrentCanvasGroup, 0f));
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
            if (Instance.mCurrentCoroutine != null)
                Instance.StopCoroutine(Instance.mCurrentCoroutine);
            Instance.mCurrentCoroutine = null;
        }
    }
}