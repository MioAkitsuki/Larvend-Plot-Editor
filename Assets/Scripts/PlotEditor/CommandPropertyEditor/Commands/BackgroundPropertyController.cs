using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class BackgroundPropertyController : MonoSingleton<BackgroundPropertyController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private BackgroundData mBackgroundData;

        private TMP_InputField mId;
        private TMP_Dropdown mTiming;
        private TMP_Dropdown mBackgroundType;
        private TMP_InputField mSourceGuid;
        private TMP_InputField mDuration;

        private Coroutine CurrentCoroutine = null;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();
            mCanvasGroup.alpha = 0;

            mId = transform.Find("Content/ID/InputField").GetComponent<TMP_InputField>();
            mTiming = transform.Find("Content/Timing/Dropdown").GetComponent<TMP_Dropdown>();
            mTiming.onValueChanged.AddListener(value => {
                mBackgroundData.Timing = (CommandTiming) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mBackgroundType = transform.Find("Content/BackgroundType/Dropdown").GetComponent<TMP_Dropdown>();
            mBackgroundType.onValueChanged.AddListener(value => {
                mBackgroundData.BackgroundType = (BackgroundType) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mSourceGuid = transform.Find("Content/SourceGuid/InputField").GetComponent<TMP_InputField>();
            mSourceGuid.onSelect.AddListener(value => {
                if (CurrentCoroutine != null) return;
                CurrentCoroutine = StartCoroutine(WaitForResourceGuid());
            });
            mDuration = transform.Find("Content/Duration/InputField").GetComponent<TMP_InputField>();
            mDuration.onEndEdit.AddListener(value => {
                mBackgroundData.Duration = float.Parse(value);
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
        }

        public void Refresh()
        {
            if (mModel.CurrentCommandController.Value.Type != CommandType.Background) return;
            mBackgroundData = mModel.CurrentCommandController.Value.Data as BackgroundData;

            mId.SetTextWithoutNotify(mModel.CurrentCommandController.Value.Data.Id.ToString());
            
            mTiming.SetValueWithoutNotify((int) mBackgroundData.Timing);
            mBackgroundType.SetValueWithoutNotify((int) mBackgroundData.BackgroundType);
            mSourceGuid.SetTextWithoutNotify(mBackgroundData.SourceGuid);
            mDuration.SetTextWithoutNotify(mBackgroundData.Duration.ToString("N2"));
        }

        IEnumerator WaitForResourceGuid()
        {
            if (mModel.CurrentCommandController == null || mModel.CurrentCommandController.Value.Type != CommandType.Background)
            {
                CurrentCoroutine = null;
                yield break;
            }

            TypeEventSystem.Global.Register<OnImageResourceSelectedEvent>(e => {
                mBackgroundData.SourceGuid = e.Guid;
            });

            LibraryManagerController.SelectResource<ImageResource>();
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => !LibraryManagerController.IsOnSelection);

            TypeEventSystem.Global.UnRegister<OnImageResourceSelectedEvent>(e => {});

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
