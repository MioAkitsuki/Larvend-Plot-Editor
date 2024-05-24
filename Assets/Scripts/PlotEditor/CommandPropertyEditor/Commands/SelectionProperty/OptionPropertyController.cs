using System;
using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class OptionPropertyController : MonoBehaviour
    {
        private OptionData _;

        private Toggle interactable;

        private TMP_InputField gotoGuid;
        private TMP_InputField option;

        void Awake()
        {
            interactable = transform.Find("Interactable").GetComponent<Toggle>();
            interactable.onValueChanged.AddListener(b => {
                _.Interactable = b;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });

            gotoGuid = transform.Find("Content/GotoGuid").GetComponent<TMP_InputField>();
            gotoGuid.onEndEdit.AddListener(s => {
                _.GotoGuid = ProjectManager.GetCommand(int.Parse(s)).Guid;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            option = transform.Find("Content/Option").GetComponent<TMP_InputField>();
            option.onEndEdit.AddListener(s => {
                _.Option = s;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
        }

        public void Initialize(OptionData data)
        {
            _ = data;
            interactable.SetIsOnWithoutNotify(_.Interactable);
            
            gotoGuid.SetTextWithoutNotify(ProjectManager.GetCommandIndex(_.GotoGuid).ToString());
            option.SetTextWithoutNotify(_.Option);
        }
    }
}