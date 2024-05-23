using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public partial class TextEditorController : MonoSingleton<TextEditorController>, IController
    {
        public enum States
        {
            None,
            FullScreen,
            Bottom
        }

        private PlotEditorModel mModel;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();

        private TextData mTextData;

        private CanvasGroup mBottomText;
        private TMP_InputField mBottomTextSpeaker;
        private TMP_InputField mBottomTextContent;

        private CanvasGroup mFullScreenText;
        private TMP_InputField mFullScreenTextSpeaker;
        private TMP_InputField mFullScreenTextContent;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mBottomText = transform.Find("BottomText").GetComponent<CanvasGroup>();
            mBottomTextSpeaker = transform.Find("BottomText/Speaker").GetComponent<TMP_InputField>();
            mBottomTextSpeaker.onEndEdit.AddListener(e => {
                mTextData.Speaker = e;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mBottomTextContent = transform.Find("BottomText/Text").GetComponent<TMP_InputField>();
            mBottomTextContent.onEndEdit.AddListener(e => {
                mTextData.Content = e;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });

            mFullScreenText = transform.Find("FullScreenText").GetComponent<CanvasGroup>();
            mFullScreenTextSpeaker = transform.Find("FullScreenText/Speaker").GetComponent<TMP_InputField>();
            mFullScreenTextSpeaker.onEndEdit.AddListener(e => {
                mTextData.Speaker = e;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mFullScreenTextContent = transform.Find("FullScreenText/Text").GetComponent<TMP_InputField>();
            mFullScreenTextContent.onEndEdit.AddListener(e => {
                mTextData.Content = e;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });

            TypeEventSystem.Global.Register<OnCommandRefreshEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCommandChangedEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            stateMachine.AddState(States.None, new NoneState(stateMachine, this));
            stateMachine.AddState(States.FullScreen, new FullScreenState(stateMachine, this));
            stateMachine.AddState(States.Bottom, new BottomState(stateMachine, this));

            stateMachine.StartState(States.None);
        }

        private void Update()
        {
            stateMachine.CurrentState.Update();
        }

        private void Refresh()
        {
            if (mModel.CurrentCommandController == null)
            {
                stateMachine.ChangeState(States.None);
                return;
            }
            if (!ProjectManager.FindNearestCommand<TextData>(mModel.CurrentCommandController.Value.Data.Guid, out var _data))
            {
                stateMachine.ChangeState(States.None);
                return;
            }
            if (mTextData != _data) mTextData = _data;

            if (_data.TextType == TextType.Bottom)
            {
                mBottomTextSpeaker.SetTextWithoutNotify(_data.Speaker);
                mBottomTextContent.SetTextWithoutNotify(_data.Content);

                stateMachine.ChangeState(States.Bottom);
            }
            else if (_data.TextType == TextType.FullScreen)
            {
                mFullScreenTextSpeaker.SetTextWithoutNotify(_data.Speaker);
                mFullScreenTextContent.SetTextWithoutNotify(_data.Content);

                stateMachine.ChangeState(States.FullScreen);
            }
            else if (_data.TextType == TextType.None)
            {
                stateMachine.ChangeState(States.None);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

    public partial class TextEditorController
    {
        public class NoneState : AbstractState<States, TextEditorController>
        {
            public NoneState(FSM<States> fsm, TextEditorController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.None;

            protected override void OnEnter()
            {
                mTarget.mBottomText.alpha = 0;
                mTarget.mBottomText.blocksRaycasts = false;
                mTarget.mBottomText.interactable = false;

                mTarget.mFullScreenText.alpha = 0;
                mTarget.mFullScreenText.blocksRaycasts = false;
                mTarget.mFullScreenText.interactable = false;
            }

            protected override void OnUpdate()
            {
                if (Input.GetKeyUp(KeyCode.DownArrow)) mTarget.SendCommand<NextCommandCommand>();
                else if (Input.GetKeyUp(KeyCode.UpArrow)) mTarget.SendCommand<PrevCommandCommand>();
            }

            protected override void OnExit() {}
        }

        public class FullScreenState : AbstractState<States, TextEditorController>
        {
            public FullScreenState(FSM<States> fsm, TextEditorController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.FullScreen;

            protected override void OnEnter()
            {
                mTarget.mBottomText.alpha = 0;
                mTarget.mBottomText.blocksRaycasts = false;
                mTarget.mBottomText.interactable = false;

                mTarget.mFullScreenText.alpha = 1;
                mTarget.mFullScreenText.blocksRaycasts = true;
                mTarget.mFullScreenText.interactable = true;
            }

            protected override void OnUpdate()
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
                {
                    if (mTarget.mFullScreenTextSpeaker.isFocused) mTarget.mFullScreenTextSpeaker.DeactivateInputField();
                    else if (mTarget.mFullScreenTextContent.isFocused) mTarget.mFullScreenTextContent.DeactivateInputField();
                }
                
                if (mTarget.mFullScreenTextContent.isFocused && Input.GetKeyUp(KeyCode.Tab))
                {
                    mTarget.mFullScreenTextContent.DeactivateInputField();
                    mTarget.mFullScreenTextSpeaker.ActivateInputField();
                }

                if (!mTarget.mFullScreenTextContent.isFocused && !mTarget.mFullScreenTextSpeaker.isFocused)
                {
                    if (Input.GetKeyUp(KeyCode.DownArrow)) mTarget.SendCommand<NextCommandCommand>();
                    else if (Input.GetKeyUp(KeyCode.UpArrow)) mTarget.SendCommand<PrevCommandCommand>();
                }
            }

            protected override void OnExit() {}
        }

        public class BottomState : AbstractState<States, TextEditorController>
        {
            public BottomState(FSM<States> fsm, TextEditorController target) : base(fsm, target) {}
            protected override bool OnCondition() => mFSM.CurrentStateId != States.Bottom;

            protected override void OnEnter()
            {
                mTarget.mBottomText.alpha = 1;
                mTarget.mBottomText.blocksRaycasts = true;
                mTarget.mBottomText.interactable = true;

                mTarget.mFullScreenText.alpha = 0;
                mTarget.mFullScreenText.blocksRaycasts = false;
                mTarget.mFullScreenText.interactable = false;
            }

            protected override void OnUpdate()
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
                {
                    if (mTarget.mBottomTextSpeaker.isFocused) mTarget.mBottomTextSpeaker.DeactivateInputField();
                    else if (mTarget.mBottomTextContent.isFocused) mTarget.mBottomTextContent.DeactivateInputField();
                }

                if (mTarget.mBottomTextSpeaker.isFocused && Input.GetKeyUp(KeyCode.Tab))
                {
                    mTarget.mBottomTextSpeaker.DeactivateInputField();
                    mTarget.mBottomTextContent.ActivateInputField();
                }

                if (!mTarget.mBottomTextSpeaker.isFocused && !mTarget.mBottomTextContent.isFocused)
                {
                    if (Input.GetKeyUp(KeyCode.DownArrow)) mTarget.SendCommand<NextCommandCommand>();
                    else if (Input.GetKeyUp(KeyCode.UpArrow)) mTarget.SendCommand<PrevCommandCommand>();
                }
            }

            protected override void OnExit() {}
        }
    }
}
