using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kuchinashi.UI
{
    public class DropdownMenuGroup : MonoBehaviour
    {
        private DropdownMenu activeMenu = null;

        private void Awake()
        {
            CreateCanvas();
        }

        public void ToggleActive(DropdownMenu menu)
        {
            if (activeMenu != null)
                activeMenu.Toggle();

            activeMenu = menu;
            activeMenu.Toggle();
        }

        public void ToggleInactive()
        {
            if (activeMenu != null)
                activeMenu.Toggle();

            activeMenu = null;
        }

        private void CreateCanvas()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10000;

            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

}