using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kuchinashi.UI
{
    public class DropdownMenuGroup : MonoBehaviour
    {
        private DropdownMenu activeMenu = null;

        public void ToggleActive(DropdownMenu menu)
        {
            if (activeMenu != null) activeMenu.Toggle();
            else CreateCanvas();

            activeMenu = menu;
            activeMenu.Toggle();
        }

        public void ToggleInactive()
        {
            if (activeMenu != null)
                activeMenu.Toggle();

            activeMenu = null;

            RemoveCanvas();
        }

        private void CreateCanvas()
        {
            if (!TryGetComponent<Canvas>(out var canvas))
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10000;

            if (!TryGetComponent<GraphicRaycaster>(out var raycaster))
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
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