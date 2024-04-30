using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public class ImageResourceController : MonoBehaviour, IController
    {
        public ImageResource Data;

        public bool IsCurrent = false;

        private PlotEditorModel model;
        private ButtonExtension button;

        private Image background;
        private Image thumbnail;
        private TMP_Text nameText;

        void Awake()
        {
            model = this.GetModel<PlotEditorModel>();
            button = GetComponent<ButtonExtension>();

            background = GetComponent<Image>();
            thumbnail = transform.Find("Image").GetComponent<Image>();
            nameText = transform.Find("Name").GetComponent<TMP_Text>();

            button.OnLeftClick += () => {
                this.SendCommand(new SelectImageResourceCommand(this));
            };
            button.OnDoubleClick += () => {
                if (LibraryManagerController.IsOnSelection)
                {
                    TypeEventSystem.Global.Send(new OnImageResourceSelectedEvent() { Guid = Data.Guid });
                    LibraryManagerController.StateMachine.ChangeState(LibraryManagerController.States.None);
                }
            };

            TypeEventSystem.Global.Register<OnResourceRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public ImageResourceController Initialize(ImageResource _data)
        {
            Data = _data;
            
            thumbnail.sprite = _data.GetSprite();
            nameText.text = _data.Name;
            return this;
        }

        public void Refresh()
        {
            thumbnail.sprite = Data.GetSprite();
            nameText.text = Data.Name;
        }

        public void Select()
        {
            background.color = new Color(0.8f, 0.8f, 0.8f);
            IsCurrent = true;
        }
        public void DeSelect()
        {
            background.color = new Color(238f / 255f, 238f / 255f, 238f / 255f);
            IsCurrent = false;
        }

        public void Dispose()
        {
            if (IsCurrent)
            {
                model.CurrentImageResourceController = null;
                TypeEventSystem.Global.Send<OnCurrentImageResourceChangedEvent>();
            }
            model.ImageResourceControllers.Remove(this);

            Destroy(gameObject);
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

}