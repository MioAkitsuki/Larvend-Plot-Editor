using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class AudioResourceController : MonoBehaviour, IController
    {
        public AudioResource Data;

        public bool IsCurrent = false;
        private bool IsPlaying = false;

        private PlotEditorModel model;
        private ButtonExtension button;

        private Image background;
        private Button preview;
        private Image previewImg;
        private TMP_Text nameText;

        void Awake()
        {
            model = this.GetModel<PlotEditorModel>();
            button = GetComponent<ButtonExtension>();

            background = GetComponent<Image>();
            preview = transform.Find("Image").GetComponent<Button>();
            previewImg = transform.Find("Image").GetComponent<Image>();
            nameText = transform.Find("Name").GetComponent<TMP_Text>();

            preview.onClick.AddListener(() => {
                if (IsPlaying)
                {
                    Stop();
                }
                else
                {
                    model.CurrentPlayingAudioResource?.Stop();
                    Play();
                }
            });

            button.OnLeftClick += () => {
                this.SendCommand(new SelectAudioResourceCommand(this));
            };
            button.OnDoubleClick += () => {
                if (LibraryManagerController.IsOnSelection)
                {
                    TypeEventSystem.Global.Send(new OnAudioResourceSelectedEvent() { Guid = Data.Guid });
                    LibraryManagerController.StateMachine.ChangeState(LibraryManagerController.States.None);
                }
            };

            TypeEventSystem.Global.Register<OnResourceRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public AudioResourceController Initialize(AudioResource _data)
        {
            Data = _data;
            
            nameText.text = _data.Name;
            return this;
        }

        public void Refresh()
        {
            nameText.text = Data.Name;
        }

        public void Select()
        {
            background.color = new Color(0.8f, 0.8f, 0.8f);
            IsCurrent = true;
        }
        public void DeSelect()
        {
            background.color = new Color(238f / 255f, 238f / 255f, 238f / 255f);
            IsCurrent = false;
        }

        public void Play()
        {
            IsPlaying = true;
            previewImg.sprite = LibraryManagerController.Instance.AudioPreviewSprites[1];

            if (AudioKit.MusicPlayer.AudioSource?.clip == Data.audioClip) AudioKit.ResumeMusic();
            else AudioKit.PlayMusic(Data.audioClip, loop: false, onEndCallback: Stop);

            model.CurrentPlayingAudioResource = this;
        }

        public void Stop()
        {
            IsPlaying = false;
            previewImg.sprite = LibraryManagerController.Instance.AudioPreviewSprites[0];

            AudioKit.StopMusic();
            AudioKit.MusicPlayer.AudioSource.time = 0;
            
            model.CurrentPlayingAudioResource = null;
        }

        public void Pause()
        {
            IsPlaying = false;
            previewImg.sprite = LibraryManagerController.Instance.AudioPreviewSprites[0];

            AudioKit.PauseMusic();
        }

        public void Forward(float second)
        {
            AudioKit.MusicPlayer.AudioSource.time += second;
        }

        public void Dispose()
        {
            if (IsCurrent)
            {
                model.CurrentAudioResourceController = null;
                TypeEventSystem.Global.Send<OnCurrentAudioResourceChangedEvent>();
            }
            model.AudioResourceControllers.Remove(this);

            Destroy(gameObject);
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

}