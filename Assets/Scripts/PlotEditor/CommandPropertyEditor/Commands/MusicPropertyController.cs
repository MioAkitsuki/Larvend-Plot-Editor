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
    public class MusicPropertyController : MonoSingleton<MusicPropertyController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private MusicData mMusicData;

        private TMP_InputField mId;
        private TMP_Dropdown mTiming;
        private TMP_InputField mDelay;
        private Toggle mLoop;
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
                mMusicData.Timing = (CommandTiming) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mDelay = transform.Find("Content/Time/InputField").GetComponent<TMP_InputField>();
            mDelay.onEndEdit.AddListener(value => {
                mMusicData.Time = float.Parse(value);
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mLoop = transform.Find("Content/Time/Loop").GetComponent<Toggle>();
            mLoop.onValueChanged.AddListener(value => {
                mMusicData.IsLoop = value;
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
            if (mModel.CurrentCommandController.Value.Type != CommandType.Music) return;
            mMusicData = mModel.CurrentCommandController.Value.Data as MusicData;

            mId.SetTextWithoutNotify(mModel.CurrentCommandController.Value.Data.Guid.ToString());
            
            mTiming.SetValueWithoutNotify((int) mMusicData.Timing);
            mDelay.SetTextWithoutNotify(mMusicData.Time.ToString("N2"));
            mLoop.SetIsOnWithoutNotify(mMusicData.Skippable);
            mSourceGuid.SetTextWithoutNotify(mMusicData.SourceGuid);
        }

        IEnumerator WaitForResourceGuid()
        {
            if (mModel.CurrentCommandController == null || mModel.CurrentCommandController.Value.Type != CommandType.Music)
            {
                CurrentCoroutine = null;
                yield break;
            }

            Action<OnAudioResourceSelectedEvent> action = (e) => {
                mMusicData.SourceGuid = e.Guid;
            };
            TypeEventSystem.Global.Register(action);

            LibraryManagerController.SelectResource<AudioResource>();
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
