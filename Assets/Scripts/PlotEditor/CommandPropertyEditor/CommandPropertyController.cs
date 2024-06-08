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
            Avatar,
            Selection,
            Goto
        }

        private PlotEditorModel mModel;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();

        private CanvasGroup mMaskCanvasGroup;
        private Button mHideButton;

        public Coroutine CurrentCoroutine;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mMaskCanvasGroup = transform.Find("Mask").GetComponent<CanvasGroup>();
            mMaskCanvasGroup.GetComponent<Button>().onClick.AddListener(() => {
                stateMachine.ChangeState(States.None);
            });

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
            stateMachine.AddState(States.Selection, new SelectionState(stateMachine, this));
            stateMachine.AddState(States.Goto, new GotoState(stateMachine, this));

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
                    TextPropertyController.Instance.Refresh();
                    break;
                case CommandType.Background:
                    BackgroundPropertyController.Instance.Refresh();
                    break;
                case CommandType.Avatar:
                    AvatarPropertyController.Instance.Refresh();
                    break;
                case CommandType.Selection:
                    SelectionPropertyController.Instance.Refresh();
                    break;
                case CommandType.Goto:
                    GotoPropertyController.Instance.Refresh();
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
                case CommandType.Selection:
                    stateMachine.ChangeState(States.Selection);
                    break;
                case CommandType.Goto:
                    stateMachine.ChangeState(States.Goto);
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
                TextPropertyController.Instance.mCanvasGroup.alpha = 1f;
                TextPropertyController.Instance.mCanvasGroup.interactable = true;
                TextPropertyController.Instance.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                TextPropertyController.Instance.mCanvasGroup.alpha = 0f;
                TextPropertyController.Instance.mCanvasGroup.interactable = false;
                TextPropertyController.Instance.mCanvasGroup.blocksRaycasts = false;
            }
        }

        public class BackgroundState : AbstractState<States, CommandPropertyController>
        {
            public BackgroundState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Background;

            protected override void OnEnter()
            {
                BackgroundPropertyController.Instance.mCanvasGroup.alpha = 1f;
                BackgroundPropertyController.Instance.mCanvasGroup.interactable = true;
                BackgroundPropertyController.Instance.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                BackgroundPropertyController.Instance.mCanvasGroup.alpha = 0f;
                BackgroundPropertyController.Instance.mCanvasGroup.interactable = false;
                BackgroundPropertyController.Instance.mCanvasGroup.blocksRaycasts = false;
            }
        }

        public class AvatarState : AbstractState<States, CommandPropertyController>
        {
            public AvatarState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Avatar;

            protected override void OnEnter()
            {
                AvatarPropertyController.Instance.mCanvasGroup.alpha = 1f;
                AvatarPropertyController.Instance.mCanvasGroup.interactable = true;
                AvatarPropertyController.Instance.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                AvatarPropertyController.Instance.mCanvasGroup.alpha = 0f;
                AvatarPropertyController.Instance.mCanvasGroup.interactable = false;
                AvatarPropertyController.Instance.mCanvasGroup.blocksRaycasts = false;
            }
        }

        public class SelectionState : AbstractState<States, CommandPropertyController>
        {
            public SelectionState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Selection;

            protected override void OnEnter()
            {
                SelectionPropertyController.Instance.mCanvasGroup.alpha = 1f;
                SelectionPropertyController.Instance.mCanvasGroup.interactable = true;
                SelectionPropertyController.Instance.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                SelectionPropertyController.Instance.mCanvasGroup.alpha = 0f;
                SelectionPropertyController.Instance.mCanvasGroup.interactable = false;
                SelectionPropertyController.Instance.mCanvasGroup.blocksRaycasts = false;
            }
        }

        public class GotoState : AbstractState<States, CommandPropertyController>
        {
            public GotoState(FSM<States> fsm, CommandPropertyController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Goto;

            protected override void OnEnter()
            {
                GotoPropertyController.Instance.mCanvasGroup.alpha = 1f;
                GotoPropertyController.Instance.mCanvasGroup.interactable = true;
                GotoPropertyController.Instance.mCanvasGroup.blocksRaycasts = true;
            }

            protected override void OnExit()
            {
                GotoPropertyController.Instance.mCanvasGroup.alpha = 0f;
                GotoPropertyController.Instance.mCanvasGroup.interactable = false;
                GotoPropertyController.Instance.mCanvasGroup.blocksRaycasts = false;
            }
        }
    }
}
