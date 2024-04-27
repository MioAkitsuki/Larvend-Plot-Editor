using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class CommandListController : MonoBehaviour , IController
    {
        public static Transform CommandListParent;

        void Awake()
        {
            CommandListParent = transform.Find("Scroll View/Viewport/Content");
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
