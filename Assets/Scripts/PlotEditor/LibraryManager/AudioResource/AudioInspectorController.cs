using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class AudioInspectorController : MonoSingleton<AudioInspectorController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private AudioResource mAudioResource;

        private TMP_InputField mGuid;
        private TMP_InputField mName;
        private TMP_InputField mDescription;
        private Slider mSlider;
        private TMP_Text mDuration;
        private Button mStop;
        private Button mPlay;
        private Button mForward;
        private Button mDelete;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();

            mGuid = transform.Find("Content/Guid/InputField").GetComponent<TMP_InputField>();
            mName = transform.Find("Content/Name/InputField").GetComponent<TMP_InputField>();
            mName.onEndEdit.AddListener(value => {
                if (mAudioResource == null || mModel.CurrentAudioResourceController == null) return;

                mAudioResource.Name = value;
                TypeEventSystem.Global.Send<OnResourceRefreshEvent>();
            });
            mDescription = transform.Find("Content/Description/InputField").GetComponent<TMP_InputField>();
            mDescription.onEndEdit.AddListener(value => {
                if (mAudioResource == null || mModel.CurrentAudioResourceController == null) return;

                mAudioResource.Description = value;
                TypeEventSystem.Global.Send<OnResourceRefreshEvent>();
            });
            mSlider = transform.Find("Content/Slider").GetComponent<Slider>();
            mDuration = transform.Find("Content/Duration").GetComponent<TMP_Text>();

            mStop = transform.Find("Content/Control/Stop").GetComponent<Button>();
            mPlay = transform.Find("Content/Control/Play").GetComponent<Button>();
            mForward = transform.Find("Content/Control/Forward").GetComponent<Button>();

            mDelete = transform.Find("Content/Delete").GetComponent<Button>();
            mDelete.onClick.AddListener(() => {
                if (mAudioResource == null || mModel.CurrentAudioResourceController == null) return;

                ResourceManager.RemoveResource(mAudioResource.Guid);
                mModel.CurrentAudioResourceController.Dispose();
                TypeEventSystem.Global.Send<OnResourceRefreshEvent>();
            });

            TypeEventSystem.Global.Register<OnCurrentAudioResourceChangedEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnResourceRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (mModel.CurrentPlayingAudioResource != null && mModel.CurrentAudioResourceController == mModel.CurrentPlayingAudioResource)
            {
                var length = mAudioResource.audioClip.length;
                var duration = new float[] {Mathf.Floor(length / 60f), length % 60f};
                var pointer = new float[] {Mathf.Floor(AudioKit.MusicPlayer.AudioSource.time / 60f), AudioKit.MusicPlayer.AudioSource.time % 60f};

                mDuration.SetText(string.Format("{0:00}:{1:00.000} / {2:00}:{3:00.000}", pointer[0], pointer[1], duration[0], duration[1]));
                mSlider.SetValueWithoutNotify(AudioKit.MusicPlayer.AudioSource.time / length);
            }
        }

        public void Refresh()
        {
            if (mModel.CurrentAudioResourceController == null)
            {
                mGuid.SetTextWithoutNotify("");
                mName.SetTextWithoutNotify("");
                mDescription.SetTextWithoutNotify("");
                mSlider.SetValueWithoutNotify(0);
                mDuration.SetText("0:00.000 / 0:00.000");

                return;
            }
            mAudioResource = mModel.CurrentAudioResourceController.Data;

            mGuid.SetTextWithoutNotify(mAudioResource.Guid);
            mName.SetTextWithoutNotify(mAudioResource.Name);
            mDescription.SetTextWithoutNotify(mAudioResource.Description);

            var length = mAudioResource.audioClip.length;
            var duration = new float[] {Mathf.Floor(length / 60f), length % 60f};
            mDuration.SetText($"0:00.000 / {duration[0]}:{duration[1]:N3}");

            if (mModel.CurrentPlayingAudioResource != null && mModel.CurrentAudioResourceController == mModel.CurrentPlayingAudioResource)
            {
                mPlay.GetComponent<Image>().sprite = LibraryManagerController.Instance.AudioPreviewSprites[1];
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
