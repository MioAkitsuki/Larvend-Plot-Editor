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
    public class SelectionOptionController : MonoBehaviour , IController
    {
        public OptionData mOptionData;

        private Button button;
        private TMP_Text text;

        private void Awake()
        {
            button = GetComponent<Button>();
            text = GetComponentInChildren<TMP_Text>();
        }

        public void Initialize(OptionData optionData)
        {
            mOptionData = optionData;
            
            text.SetText(mOptionData.Option);
            button.interactable = mOptionData.Interactable;
            button.onClick.AddListener(() => {
                if (mOptionData.GotoGuid == Guid.Empty) return;
                this.SendCommand(new JumpToCommandCommand(mOptionData.GotoGuid));
            });
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}