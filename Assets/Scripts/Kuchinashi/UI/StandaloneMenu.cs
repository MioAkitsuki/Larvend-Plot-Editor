using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Kuchinashi.UI
{
    public class StandaloneMenu : MonoBehaviour
    {
        public CanvasGroup window;

        private GameObject blocker;
        private bool isOpen = false;

        void Awake()
        {
            window.alpha = 0;
            window.interactable = false;
            window.blocksRaycasts = false;
        }

        public void Toggle()
        {
            StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(window, isOpen ? 0f : 1f, 0.15f));
            if (!isOpen)
            {
                CreateBlocker();
                CreateCanvas();
            }
            else if (blocker != null)
            {
                Destroy(blocker);
                gameObject.DestroySelfAfterDelay(0.5f);
            }

            isOpen = !isOpen;
        }

        private void CreateBlocker()
        {
            var rootCanvas = transform.GetComponentInParent<Canvas>().rootCanvas.transform;
            GameObject go = new GameObject("Standalone Menu Blocker");
            go.transform.SetParent(rootCanvas);
            go.layer = LayerMask.NameToLayer("UI");

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;

            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 9998;

            var image = go.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0f);

            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() => {
                Toggle();
            });

            go.AddComponent<GraphicRaycaster>();

            blocker = go;
        }

        private void CreateCanvas()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10000;

            gameObject.AddComponent<GraphicRaycaster>();
        }

        private void RemoveCanvas()
        {
            List<Component> components = new List<Component>() {
                gameObject.GetComponent<GraphicRaycaster>(),
                gameObject.GetComponent<Canvas>()
            };
            
            foreach (Component item in components)
            {
                Destroy(item);
            }
        }
    }

}