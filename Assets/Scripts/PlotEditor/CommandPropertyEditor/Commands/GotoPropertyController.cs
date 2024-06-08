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
    public class GotoPropertyController : MonoSingleton<GotoPropertyController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private GotoData mGotoData;

        private TMP_InputField mId;
        private TMP_Dropdown mTiming;
        private TMP_InputField mGotoGuid;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();
            mCanvasGroup.alpha = 0;

            mId = transform.Find("Content/ID/InputField").GetComponent<TMP_InputField>();
            mTiming = transform.Find("Content/Timing/Dropdown").GetComponent<TMP_Dropdown>();
            mTiming.onValueChanged.AddListener(value => {
                mGotoData.Timing = (CommandTiming) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mGotoGuid = transform.Find("Content/GotoGuid/InputField").GetComponent<TMP_InputField>();
            mGotoGuid.onEndEdit.AddListener(value => {
                mGotoData.GotoGuid = ProjectManager.GetCommand(int.Parse(value)).Guid;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
        }

        public void Refresh()
        {
            if (mModel.CurrentCommandController.Value.Type != CommandType.Goto) return;
            mGotoData = mModel.CurrentCommandController.Value.Data as GotoData;

            mId.SetTextWithoutNotify(mModel.CurrentCommandController.Value.Data.Guid.ToString());
            
            mTiming.SetValueWithoutNotify((int) mGotoData.Timing);
            mGotoGuid.SetTextWithoutNotify(ProjectManager.GetCommandIndex(mGotoData.GotoGuid).ToString());
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
