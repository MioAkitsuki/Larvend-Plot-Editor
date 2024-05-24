using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class SelectionEditorController : MonoSingleton<SelectionEditorController>, IController
    {
        private PlotEditorModel mModel;

        public GameObject OptionPrefab;

        private SelectionData mSelectionData;

        private CanvasGroup mFullScreenCanvasGroup;
        private CanvasGroup mRightCanvasGroup;

        private Transform mFullScreenOptions;
        private Transform mRightOptions;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mFullScreenCanvasGroup = transform.Find("FullScreen").GetComponent<CanvasGroup>();
            mRightCanvasGroup = transform.Find("Right").GetComponent<CanvasGroup>();

            mFullScreenOptions = transform.Find("FullScreen/Options");
            mRightOptions = transform.Find("Right/Options");

            TypeEventSystem.Global.Register<OnCommandRefreshEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCommandChangedEvent>(e => {
                Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Refresh()
        {
            if (mModel.CurrentCommandController == null) return;
            if (mModel.CurrentCommandController.Value.Data.Type != CommandType.Selection)
            {
                mFullScreenCanvasGroup.alpha = 0;
                mRightCanvasGroup.alpha = 0;

                return;
            }

            var _data = mModel.CurrentCommandController.Value.Data as SelectionData;

            if (_data.SelectionType == SelectionType.None)
            {
                mFullScreenCanvasGroup.alpha = 0;
                mFullScreenCanvasGroup.blocksRaycasts = false;
                mRightCanvasGroup.alpha = 0;
                mRightCanvasGroup.blocksRaycasts = false;

                return;
            }
            else if (_data.SelectionType == SelectionType.FullScreen)
            {
                mFullScreenCanvasGroup.alpha = 1;
                mRightCanvasGroup.alpha = 0;

                mFullScreenOptions.DestroyChildren();
                foreach (var option in _data.Options)
                {
                    Instantiate(OptionPrefab, mFullScreenOptions).GetComponentInChildren<SelectionOptionController>().Initialize(option);
                }

                mFullScreenCanvasGroup.interactable = _data.Options.Count > 0;
                mFullScreenCanvasGroup.blocksRaycasts = _data.Options.Count > 0;
                mRightCanvasGroup.interactable = false;
                mRightCanvasGroup.blocksRaycasts = false;
            }
            else if (_data.SelectionType == SelectionType.Right)
            {
                mFullScreenCanvasGroup.alpha = 0;
                mRightCanvasGroup.alpha = 1;

                mRightOptions.DestroyChildren();
                foreach (var option in _data.Options)
                {
                    Instantiate(OptionPrefab, mRightOptions).GetComponentInChildren<SelectionOptionController>().Initialize(option);
                }

                mFullScreenCanvasGroup.interactable = false;
                mFullScreenCanvasGroup.blocksRaycasts = false;
                mRightCanvasGroup.interactable = _data.Options.Count > 0;
                mRightCanvasGroup.blocksRaycasts = _data.Options.Count > 0;
            }

            mSelectionData = _data;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}