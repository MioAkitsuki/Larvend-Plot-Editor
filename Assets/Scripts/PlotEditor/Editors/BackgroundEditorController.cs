using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class BackgroundEditorController : MonoSingleton<BackgroundEditorController>, IController
    {
        private PlotEditorModel mModel;

        private BackgroundData mBackgroundData;

        private CanvasGroup mBackCanvasGroup;
        private Image mBackBackground;
        private AspectRatioFitter mBackAspectRatioFitter;
        private CanvasGroup mMiddleCanvasGroup;
        private Image mMiddleBackground;
        private CanvasGroup mFrontCanvasGroup;
        private Image mFrontBackground;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mBackCanvasGroup = transform.Find("BackBackground").GetComponent<CanvasGroup>();
            mBackBackground = transform.Find("BackBackground/Image").GetComponent<Image>();
            mBackAspectRatioFitter = mBackBackground.GetComponent<AspectRatioFitter>();

            mMiddleCanvasGroup = transform.Find("MiddleBackground").GetComponent<CanvasGroup>();
            mMiddleBackground = transform.Find("MiddleBackground/Image").GetComponent<Image>();
            mFrontCanvasGroup = transform.Find("FrontBackground").GetComponent<CanvasGroup>();
            mFrontBackground = transform.Find("FrontBackground/Image").GetComponent<Image>();

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
            if (!ProjectManager.FindNearestCommand<BackgroundData>(mModel.CurrentCommandController.Value.Data.Guid, out var _data))
            {
                mBackCanvasGroup.alpha = 0;
                mMiddleCanvasGroup.alpha = 0;
                mFrontCanvasGroup.alpha = 0;

                return;
            }

            if (_data.BackgroundType == BackgroundType.None)
            {
                mBackCanvasGroup.alpha = 0;
                mMiddleCanvasGroup.alpha = 0;
                mFrontCanvasGroup.alpha = 0;

                return;
            }

            if (mBackgroundData != _data) mBackgroundData = _data;
            if (ResourceManager.TryGetResource<ImageResource>(_data.SourceGuid, out var _res))
            {
                switch (_data.BackgroundType)
                {
                    case BackgroundType.Back:
                        mBackCanvasGroup.alpha = 1;
                        mBackBackground.sprite = _res.GetSprite();
                        mBackAspectRatioFitter.aspectRatio = _res.GetSprite().rect.width / _res.GetSprite().rect.height;
                        break;
                    case BackgroundType.Middle:
                        mMiddleCanvasGroup.alpha = 1;
                        mMiddleBackground.sprite = _res.GetSprite();
                        break;
                    case BackgroundType.Front:
                        mFrontCanvasGroup.alpha = 1;
                        mFrontBackground.sprite = _res.GetSprite();
                        break;
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}