using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kuchinashi.UI
{
    public class DropdownMenu : MonoBehaviour
    {
        public Button toggle;
        public CanvasGroup dropdown;

        public DropdownMenuGroup group;

        private GameObject blocker;
        private bool isOpen = false;

        void Awake()
        {
            toggle.onClick.AddListener(() => {
                if (group == null) Toggle();
                else if (isOpen) group.ToggleInactive();
                else group.ToggleActive(this);
            });

            dropdown.alpha = 0;
            dropdown.interactable = false;
            dropdown.blocksRaycasts = false;
        }

        public void Toggle()
        {
            StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(dropdown, isOpen ? 0f : 1f, 0.15f));
            if (!isOpen) CreateBlocker();
            else if (blocker != null) Destroy(blocker);

            isOpen = !isOpen;
        }

        private void CreateBlocker()
        {
            var rootCanvas = transform.GetComponentInParent<Canvas>().rootCanvas.transform;
            GameObject go = new GameObject("Dropdown Blocker");
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
            canvas.sortingOrder = 9999;

            var image = go.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0f);

            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() => {
                if (group == null) Toggle();
                else group.ToggleInactive();
            });

            go.AddComponent<GraphicRaycaster>();

            blocker = go;
        }
    }

}