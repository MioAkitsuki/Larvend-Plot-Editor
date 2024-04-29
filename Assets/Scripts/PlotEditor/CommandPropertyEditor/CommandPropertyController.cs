using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor;
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
            Text
        }

        private PlotEditorModel mModel;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();

        private TextPropertyController mTextPropertyController;

        private Button mHideButton;

        public Coroutine CurrentCoroutine;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mTextPropertyController = transform.Find("TextProperty").GetComponent<TextPropertyController>();

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

            stateMachine.StartState(States.None);
        }

        public void Refresh()
        {
            var command = mModel.CurrentCommandController.Value;
            switch (command.Type)
            {
                case CommandType.Text:
                    mTextPropertyController.Refresh();
                    break;
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

                while (!Mathf.Approximately(rect.anchoredPosition.x, 0f))
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, 0.15f);
                    yield return new WaitForFixedUpdate();
                }
                rect.anchoredPosition = targetPos;

                mTarget.CurrentCoroutine = null;
            }
            
            IEnumerator ShowPropertyCoroutine()
            {
                var rect = mTarget.GetComponent<RectTransform>();
                var targetPos = new Vector2(400f, -30f);

                while (!Mathf.Approximately(rect.anchoredPosition.x, 400f))
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, 0.15f);
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
    }
}
