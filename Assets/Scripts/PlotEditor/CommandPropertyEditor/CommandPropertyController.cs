using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;
using UnityEngine.UI;
using Kuchinashi;

namespace Larvend.PlotEditor.UI
{
    public partial class CommandPropertyController : MonoSingleton<CommandPropertyController> , IController
    {
        public enum States
        {
            None,
            Text,
            Background,
            Avatar
        }

        private PlotEditorModel mModel;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();

        private CanvasGroup mMaskCanvasGroup;

        private TextPropertyController mTextPropertyController;
        private BackgroundPropertyController mBackgroundPropertyController;
        private AvatarPropertyController mAvatarPropertyController;

        private Button mHideButton;

        public Coroutine CurrentCoroutine;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mMaskCanvasGroup = transform.Find("Mask").GetComponent<CanvasGroup>();
            mMaskCanvasGroup.GetComponent<Button>().onClick.AddListener(() => {
                stateMachine.ChangeState(States.None);
            });

            mTextPropertyController = transform.Find("TextProperty").GetComponent<TextPropertyController>();
            mBackgroundPropertyController = transform.Find("BackgroundProperty").GetComponent<BackgroundPropertyController>();
            mAvatarPropertyController = transform.Find("AvatarProperty").GetComponent<AvatarPropertyController>();

            mHideButton = transform.Find("HideButton").GetComponent<Button>();
            mHideButton.onClick.AddListener(() => {
                stateMachine.ChangeState(States.None);
            });

            TypeEventSystem.Global.Register<OnCommandRefreshEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnCommandChangedEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            stateMachine.AddState(States.None, new NoneState(stateMachine, this));
            stateMachine.AddState(States.Text, new TextState(stateMachine, this));
            stateMachine.AddState(States.Background, new BackgroundState(stateMachine, this));
            stateMachine.AddState(States.Avatar, new AvatarState(stateMachine, this));

            stateMachine.StartState(States.None);
        }

        public void Refresh()
        {
            if (mModel.CurrentCommandController == null)
            {
                stateMachine.ChangeState(States.None);
                return;
            }

            var command = mModel.CurrentCommandController.Value;
            switch (command.Type)
            {
                case CommandType.Text:
                    mTextPropertyController.Refresh();
                    break;
                case CommandType.Background:
                    mBackgroundPropertyController.Refresh();
                    break;
                case CommandType.Avatar:
                    mAvatarPropertyController.Refresh();
                    break;
            }

            if (stateMachine.CurrentStateId != States.None)
            {
                Show();
            }
        }

        public void Show()
        {
            var command = mModel.CurrentCommandController.Value;
            switch (command.Type)
            {
                case CommandType.Text:
                    stateMachine.ChangeState(States.Text);
                    break;
                case CommandType.Background:
                    stateMachine.ChangeState(States.Background);
                    break;
                case CommandType.Avatar:
                    stateMachine.ChangeState(States.Avatar);
                    break;
                default:
                    stateMachine.ChangeState(States.None);
                    break;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

    public partial class CommandPropertyController
    {
        public class NoneState : AbstractState<States, CommandPropertyController>
        {
            public NoneState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.None;

            protected override void OnEnter()
            {
                if (mTarget.CurrentCoroutine != null)
                {
                    mTarget.StopCoroutine(mTarget.CurrentCoroutine);
                }
                mTarget.CurrentCoroutine = mTarget.StartCoroutine(HidePropertyCoroutine());
            }

            protected override void OnExit()
            {
                if (mTarget.CurrentCoroutine != null)
                {
                    mTarget.StopCoroutine(mTarget.CurrentCoroutine);
                }
                mTarget.CurrentCoroutine = mTarget.StartCoroutine(ShowPropertyCoroutine());
            }

            IEnumerator HidePropertyCoroutine()
            {
                var rect = mTarget.GetComponent<RectTransform>();
                var targetPos = new Vector2(0f, -30f);

                mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.mMaskCanvasGroup, 0f, 0.15f));
                while (!Mathf.Approximately(rect.anchoredPosition.x, 0f))
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, 0.2f);
                    yield return new WaitForFixedUpdate();
                }
                rect.anchoredPosition = targetPos;

                mTarget.CurrentCoroutine = null;
            }
            
            IEnumerator ShowPropertyCoroutine()
            {
                var rect = mTarget.GetComponent<RectTransform>();
                var targetPos = new Vector2(400f, -30f);

                mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.mMaskCanvasGroup, 1f, 0.15f));
                while (!Mathf.Approximately(rect.anchoredPosition.x, 400f))
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, 0.2f);
                    yield return new WaitForFixedUpdate();
                }
                rect.anchoredPosition = targetPos;

                mTarget.CurrentCoroutine = null;
            }
        }

        public class TextState : AbstractState<States, CommandPropertyController>
        {
            public TextState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Text;

            protected override void OnEnter()
            {
                mTarget.mTextPropertyController.mCanvasGroup.alpha = 1f;
                mTarget.mTextPropertyController.mCanvasGroup.interactable = true;
                mTarget.mTextPropertyController.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                mTarget.mTextPropertyController.mCanvasGroup.alpha = 0f;
                mTarget.mTextPropertyController.mCanvasGroup.interactable = false;
                mTarget.mTextPropertyController.mCanvasGroup.blocksRaycasts = false;
            }
        }

        public class BackgroundState : AbstractState<States, CommandPropertyController>
        {
            public BackgroundState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Background;

            protected override void OnEnter()
            {
                mTarget.mBackgroundPropertyController.mCanvasGroup.alpha = 1f;
                mTarget.mBackgroundPropertyController.mCanvasGroup.interactable = true;
                mTarget.mBackgroundPropertyController.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                mTarget.mBackgroundPropertyController.mCanvasGroup.alpha = 0f;
                mTarget.mBackgroundPropertyController.mCanvasGroup.interactable = false;
                mTarget.mBackgroundPropertyController.mCanvasGroup.blocksRaycasts = false;
            }
        }

        public class AvatarState : AbstractState<States, CommandPropertyController>
        {
            public AvatarState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Avatar;

            protected override void OnEnter()
            {
                mTarget.mAvatarPropertyController.mCanvasGroup.alpha = 1f;
                mTarget.mAvatarPropertyController.mCanvasGroup.interactable = true;
                mTarget.mAvatarPropertyController.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                mTarget.mAvatarPropertyController.mCanvasGroup.alpha = 0f;
                mTarget.mAvatarPropertyController.mCanvasGroup.interactable = false;
                mTarget.mAvatarPropertyController.mCanvasGroup.blocksRaycasts = false;
            }
        }
    }
}
