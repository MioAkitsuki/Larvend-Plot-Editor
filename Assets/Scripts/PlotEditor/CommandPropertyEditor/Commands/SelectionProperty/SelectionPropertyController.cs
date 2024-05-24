using System;
using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class SelectionPropertyController : MonoSingleton<SelectionPropertyController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private SelectionData mSelectionData;
        public GameObject OptionControllerPrefab;

        private TMP_InputField mId;
        private TMP_Dropdown mTiming;
        private TMP_Dropdown mSelectionType;
        private Transform mOptions;
        private TMP_InputField mOptionsCount;

        private Coroutine CurrentCoroutine = null;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();
            mCanvasGroup.alpha = 0;

            mId = transform.Find("Content/ID/InputField").GetComponent<TMP_InputField>();
            mTiming = transform.Find("Content/Timing/Dropdown").GetComponent<TMP_Dropdown>();
            mTiming.onValueChanged.AddListener(value => {
                mSelectionData.Timing = (CommandTiming) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mSelectionType = transform.Find("Content/SelectionType/Dropdown").GetComponent<TMP_Dropdown>();
            mSelectionType.onValueChanged.AddListener(value => {
                mSelectionData.SelectionType = (SelectionType) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mOptions = transform.Find("Content/Options/Scroll View/Viewport/Content");
            mOptionsCount = transform.Find("Content/Options/InputField").GetComponent<TMP_InputField>();
            mOptionsCount.onValueChanged.AddListener(value => {
                if (!int.TryParse(value, out var res)) return;
                mSelectionData.Count = res;
                
                mSelectionData.Options = mSelectionData.Options.GetRange(0, Math.Min(mSelectionData.Count, mSelectionData.Options.Count));
                if (mSelectionData.Count > mSelectionData.Options.Count)
                {
                    for (int i = mSelectionData.Options.Count; i < mSelectionData.Count; i++)
                    {
                        mSelectionData.Options.Add(new OptionData());
                    }
                }

                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
        }

        public void Refresh()
        {
            if (mModel.CurrentCommandController.Value.Type != CommandType.Selection) return;
            mSelectionData = mModel.CurrentCommandController.Value.Data as SelectionData;

            mId.SetTextWithoutNotify(mModel.CurrentCommandController.Value.Data.Guid.ToString());
            
            mTiming.SetValueWithoutNotify((int) mSelectionData.Timing);
            mSelectionType.SetValueWithoutNotify((int) mSelectionData.SelectionType);
            mOptionsCount.SetTextWithoutNotify(mSelectionData.Count.ToString());

            mOptions.DestroyChildren();
            for (int i = 0; i < mSelectionData.Count; i++)
            {
                var option = Instantiate(OptionControllerPrefab, mOptions);
                var controller = option.GetComponent<OptionPropertyController>();

                if (i >= mSelectionData.Options.Count) mSelectionData.Options.Add(new OptionData());
                controller.Initialize(mSelectionData.Options[i]);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
