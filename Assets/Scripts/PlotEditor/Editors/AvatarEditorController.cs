using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class AvatarEditorController : MonoSingleton<AvatarEditorController>, IController
    {
        private PlotEditorModel mModel;

        private AvatarData mAvatarData;

        private CanvasGroup mBottomLeftCanvasGroup;
        private Image mBottomLeftAvatar;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mBottomLeftCanvasGroup = transform.Find("BottomLeft").GetComponent<CanvasGroup>();
            mBottomLeftAvatar = transform.Find("BottomLeft/Image").GetComponent<Image>();

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
            if (!ProjectManager.FindNearestCommand<AvatarData>(mModel.CurrentCommandController.Value.Data.Guid, out var _data))
            {
                mBottomLeftCanvasGroup.alpha = 0;

                return;
            }

            if (_data.AvatarType == AvatarType.None)
            {
                mBottomLeftCanvasGroup.alpha = 0;

                return;
            }

            if (ResourceManager.TryGetResource<ImageResource>(_data.SourceGuid, out var _res))
            {
                switch (_data.AvatarType)
                {
                    case AvatarType.BottomLeft:
                        mBottomLeftCanvasGroup.alpha = 1;
                        if (mAvatarData == _data) return;
                        mBottomLeftAvatar.sprite = _res.GetSprite();
                        break;
                }
            }

            mAvatarData = _data;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}