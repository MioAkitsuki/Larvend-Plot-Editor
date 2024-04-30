using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class ImageInspectorController : MonoSingleton<ImageInspectorController>, IController
    {
        private PlotEditorModel mModel;

        [HideInInspector] public CanvasGroup mCanvasGroup;
        private ImageResource mImageResource;

        private TMP_InputField mGuid;
        private TMP_InputField mName;
        private TMP_InputField mDescription;
        private Image mImage;
        private TMP_Text mSize;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mCanvasGroup = GetComponent<CanvasGroup>();

            mGuid = transform.Find("Content/Guid/InputField").GetComponent<TMP_InputField>();
            mName = transform.Find("Content/Name/InputField").GetComponent<TMP_InputField>();
            mName.onEndEdit.AddListener(value => {
                if (mImageResource == null || mModel.CurrentImageResourceController == null) return;

                mImageResource.Name = value;
                TypeEventSystem.Global.Send<OnResourceRefreshEvent>();
            });
            mDescription = transform.Find("Content/Description/InputField").GetComponent<TMP_InputField>();
            mDescription.onEndEdit.AddListener(value => {
                if (mImageResource == null || mModel.CurrentImageResourceController == null) return;

                mImageResource.Description = value;
                TypeEventSystem.Global.Send<OnResourceRefreshEvent>();
            });
            mImage = transform.Find("Content/Image").GetComponent<Image>();
            mSize = transform.Find("Content/Size").GetComponent<TMP_Text>();

            TypeEventSystem.Global.Register<OnCurrentImageResourceChangedEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnResourceRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void Refresh()
        {
            if (mModel.CurrentImageResourceController == null)
            {
                mGuid.SetTextWithoutNotify("");
                mName.SetTextWithoutNotify("");
                mDescription.SetTextWithoutNotify("");
                mImage.sprite = null;
                mSize.SetText("");

                return;
            }
            mImageResource = mModel.CurrentImageResourceController.Data;

            mGuid.SetTextWithoutNotify(mImageResource.Guid);
            mName.SetTextWithoutNotify(mImageResource.Name);
            mDescription.SetTextWithoutNotify(mImageResource.Description);
            mImage.sprite = mImageResource.GetSprite();
            mSize.SetText($"{mImage.sprite.rect.width} * {mImage.sprite.rect.height}");
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
