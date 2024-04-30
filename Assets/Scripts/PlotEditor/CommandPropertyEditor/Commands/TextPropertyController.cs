using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class TextPropertyController : MonoSingleton<TextPropertyController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private TextData mTextData;

        private TMP_InputField mId;
        private TMP_Dropdown mType;
        private TMP_InputField mSpeaker;
        private TMP_InputField mContent;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();
            mCanvasGroup.alpha = 0;

            mId = transform.Find("Content/ID/InputField").GetComponent<TMP_InputField>();
            mType = transform.Find("Content/Type/Dropdown").GetComponent<TMP_Dropdown>();
            mType.onValueChanged.AddListener(value => {
                mTextData.TextType = (TextType) value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mSpeaker = transform.Find("Content/Speaker/InputField").GetComponent<TMP_InputField>();
            mSpeaker.onEndEdit.AddListener(value => {
                mTextData.Speaker = value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
            mContent = transform.Find("Content/Content/InputField").GetComponent<TMP_InputField>();
            mContent.onEndEdit.AddListener(value => {
                mTextData.Content = value;
                TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
            });
        }

        void Update()
        {
            if (mSpeaker.isFocused && Input.GetKeyUp(KeyCode.Tab))
            {
                mSpeaker.DeactivateInputField();
                mContent.ActivateInputField();
            }
        }

        public void Refresh()
        {
            if (mModel.CurrentCommandController.Value.Type != CommandType.Text) return;
            mTextData = mModel.CurrentCommandController.Value.Data as TextData;

            mId.SetTextWithoutNotify(mModel.CurrentCommandController.Value.Data.Id.ToString());
            
            mType.SetValueWithoutNotify((int) mTextData.TextType);
            mSpeaker.SetTextWithoutNotify(mTextData.Speaker);
            mContent.SetTextWithoutNotify(mTextData.Content);
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
