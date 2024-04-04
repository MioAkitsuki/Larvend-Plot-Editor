using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor
{
    public struct NextCommandEvent {}
    public class StageController : MonoBehaviour , ISingleton
    {
        public static StageController Instance
        {
            get { return MonoSingletonProperty<StageController>.Instance; }
        }

        private Button panel;

        void Awake()
        {
            panel = transform.Find("Panel").GetComponent<Button>();
            panel.onClick.AddListener(() =>
            {
                TypeEventSystem.Global.Send<NextCommandEvent>();
            });
        }

        public void OnSingletonInit()
        {
            
        }
    }
}