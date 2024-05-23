using System;
using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class AvatarPropertyController : MonoSingleton<AvatarPropertyController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private AvatarData mAvatarData;

        private TMP_InputField mId;
        private TMP_Dropdown mTiming;
        private TMP_Dropdown mAvatarType;
        private TMP_InputField mSourceGuid;

        private Coroutine CurrentCoroutine = null;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();
            mCanvasGroup.alpha = 0;

            mId = transform.Find("Content/ID/InputField").GetComponent<TMP_InputField>();
            mTiming = transform.Find("Content/Timing/Dropdown").GetComponent<TMP_Dropdown>();
            mTiming.onValueChanged.AddListener(value => {
                mAvatarData.Timing = (CommandTiming) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mAvatarType = transform.Find("Content/AvatarType/Dropdown").GetComponent<TMP_Dropdown>();
            mAvatarType.onValueChanged.AddListener(value => {
                mAvatarData.AvatarType = (AvatarType) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mSourceGuid = transform.Find("Content/SourceGuid/InputField").GetComponent<TMP_InputField>();
            mSourceGuid.onSelect.AddListener(value => {
                if (CurrentCoroutine != null) return;
                CurrentCoroutine = StartCoroutine(WaitForResourceGuid());
            });
        }

        public void Refresh()
        {
            if (mModel.CurrentCommandController.Value.Type != CommandType.Avatar) return;
            mAvatarData = mModel.CurrentCommandController.Value.Data as AvatarData;

            mId.SetTextWithoutNotify(mModel.CurrentCommandController.Value.Data.Guid.ToString());
            
            mTiming.SetValueWithoutNotify((int) mAvatarData.Timing);
            mAvatarType.SetValueWithoutNotify((int) mAvatarData.AvatarType);
            mSourceGuid.SetTextWithoutNotify(mAvatarData.SourceGuid);
        }

        IEnumerator WaitForResourceGuid()
        {
            if (mModel.CurrentCommandController == null || mModel.CurrentCommandController.Value.Type != CommandType.Avatar)
            {
                CurrentCoroutine = null;
                yield break;
            }

            Action<OnImageResourceSelectedEvent> action = (e) => {
                mAvatarData.SourceGuid = e.Guid;
            };
            TypeEventSystem.Global.Register(action);

            LibraryManagerController.SelectResource<ImageResource>();
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => !LibraryManagerController.IsOnSelection);

            TypeEventSystem.Global.UnRegister(action);

            Refresh();
            TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            CurrentCoroutine = null;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
