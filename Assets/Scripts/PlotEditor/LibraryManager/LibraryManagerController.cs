using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public partial class LibraryManagerController : MonoSingleton<LibraryManagerController> , IController
    {
        public enum States
        {
            None,
            Image,
            Audio
        }

        private PlotEditorModel mModel;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();
        public List<GameObject> ResourcePrefabs;
        public List<Sprite> AudioPreviewSprites;

        private CanvasGroup mCanvasGroup;
        private Button mCloseButon;

        private CanvasGroup mImageManager;
        private Transform mImageManagerContent;
        private ImageInspectorController mImageInspector;

        private CanvasGroup mAudioManager;
        private Transform mAudioManagerContent;

        private Toggle mImageManagerToggle;
        private Toggle mAudioManagerToggle;

        public static bool IsOnSelection = false;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();
            mCloseButon = transform.Find("Background").GetComponent<Button>();
            mCloseButon.onClick.AddListener(() => {
                stateMachine.ChangeState(States.None);
            });

            mImageManager = transform.Find("ImageManager").GetComponent<CanvasGroup>();
            mImageManagerContent = mImageManager.transform.Find("Scroll View/Viewport/Content");
            mImageManagerContent.GetChild(0).GetComponent<ButtonExtension>().OnLeftClick += () => {
                ResourceManager.ImportImageResource();
                TypeEventSystem.Global.Send<OnCurrentImageResourceChangedEvent>();
            };
            mImageInspector = mImageManager.transform.Find("Inspector").GetComponent<ImageInspectorController>();

            mAudioManager = transform.Find("AudioManager").GetComponent<CanvasGroup>();
            mAudioManagerContent = mAudioManager.transform.Find("Scroll View/Viewport/Content");
            mAudioManagerContent.GetChild(0).GetComponent<ButtonExtension>().OnLeftClick += () => {
                ResourceManager.ImportAudioResource();
                TypeEventSystem.Global.Send<OnCurrentAudioResourceChangedEvent>();
            };

            mImageManagerToggle = transform.Find("Menu/Image").GetComponent<Toggle>();
            mImageManagerToggle.onValueChanged.AddListener(value => {
                if (value) stateMachine.ChangeState(States.Image);
            });
            mAudioManagerToggle = transform.Find("Menu/Audio").GetComponent<Toggle>();
            mAudioManagerToggle.onValueChanged.AddListener(value => {
                if (value) stateMachine.ChangeState(States.Audio);
            });

            TypeEventSystem.Global.Register<OnResourcesChangedEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            stateMachine.AddState(States.None, new NoneState(stateMachine, this));
            stateMachine.AddState(States.Image, new ImageState(stateMachine, this));
            stateMachine.AddState(States.Audio, new AudioState(stateMachine, this));

            stateMachine.StartState(States.None);
        }

        public static void CallUp()
        {
            if (Instance.mImageManagerToggle.isOn) StateMachine.ChangeState(States.Image);
            else if (Instance.mAudioManagerToggle.isOn) StateMachine.ChangeState(States.Audio);
        }

        public static void SelectResource<T>()
        {
            IsOnSelection = true;

            switch (typeof(T))
            {
                case var type when type == typeof(ImageResource):
                    StateMachine.ChangeState(States.Image);
                    break;
                case var type when type == typeof(AudioResource):
                    StateMachine.ChangeState(States.Audio);
                    break;
            }

            Instance.StartCoroutine(Instance.SelectResourceCoroutine());
        }

        private IEnumerator SelectResourceCoroutine()
        {
            yield return new WaitUntil(() => StateMachine.CurrentStateId == States.None);
            IsOnSelection = false;
        }

        private void Refresh()
        {
            foreach (var image in mImageManagerContent.GetComponentsInChildren<ImageResourceController>())
            {
                image.Dispose();
            }
            foreach (var audio in mAudioManagerContent.GetComponentsInChildren<AudioResourceController>())
            {
                audio.Dispose();
            }

            foreach (var image in ResourceManager.Instance.Images)
            {
                Instantiate(ResourcePrefabs[0], mImageManagerContent).GetComponent<ImageResourceController>().Initialize(image.Value);
            }
            foreach (var audio in ResourceManager.Instance.Audios)
            {
                Instantiate(ResourcePrefabs[1], mAudioManagerContent).GetComponent<AudioResourceController>().Initialize(audio.Value);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

    public partial class LibraryManagerController
    {
        public class NoneState : AbstractState<States, LibraryManagerController>
        {
            public NoneState(FSM<States> fsm, LibraryManagerController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.None;

            protected override void OnEnter()
            {
                mTarget.mImageManager.alpha = 0f;
                mTarget.mImageManager.blocksRaycasts = false;
                mTarget.mImageManager.interactable = false;

                mTarget.mAudioManager.alpha = 0f;
                mTarget.mAudioManager.blocksRaycasts = false;
                mTarget.mAudioManager.interactable = false;

                mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.mCanvasGroup, 0f, 0.1f));
            }

            protected override void OnExit()
            {
                mTarget.Refresh();
            }
        }

        public class ImageState : AbstractState<States, LibraryManagerController>
        {
            public ImageState(FSM<States> fsm, LibraryManagerController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Image;

            protected override void OnEnter()
            {
                mTarget.mImageManager.alpha = 1f;
                mTarget.mImageManager.blocksRaycasts = true;
                mTarget.mImageManager.interactable = true;

                mTarget.mAudioManager.alpha = 0f;
                mTarget.mAudioManager.blocksRaycasts = false;
                mTarget.mAudioManager.interactable = false;

                mTarget.mImageManagerToggle.SetIsOnWithoutNotify(true);

                mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.mCanvasGroup, 1f, 0.1f));
            }

            protected override void OnExit() {}
        }

        public class AudioState : AbstractState<States, LibraryManagerController>
        {
            public AudioState(FSM<States> fsm, LibraryManagerController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Audio;

            protected override void OnEnter()
            {
                mTarget.mImageManager.alpha = 0f;
                mTarget.mImageManager.blocksRaycasts = false;
                mTarget.mImageManager.interactable = false;

                mTarget.mAudioManager.alpha = 1f;
                mTarget.mAudioManager.blocksRaycasts = true;
                mTarget.mAudioManager.interactable = true;

                mTarget.mAudioManagerToggle.SetIsOnWithoutNotify(true);

                mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.mCanvasGroup, 1f, 0.1f));
            }

            protected override void OnExit() {}
        }
    }
}