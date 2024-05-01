using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Larvend.PlotEditor.UI
{
    public abstract class CommandControllerBase : MonoBehaviour, IController
    {
        public abstract CommandType Type { get; }
        public CommandData Data;

        public bool IsCurrent = false;

        private PlotEditorModel model;
        private ButtonExtension button;

        internal Image background;
        internal TMP_Text idText;
        internal TMP_Text typeText;

        internal virtual void Awake()
        {
            model = this.GetModel<PlotEditorModel>();
            button = GetComponent<ButtonExtension>();

            background = transform.Find("Background").GetComponent<Image>();
            idText = transform.Find("ID").GetComponent<TMP_Text>();
            typeText = transform.Find("Type").GetComponent<TMP_Text>();
            
            button.OnLeftClick += () => {
                this.SendCommand(new SelectCommandCommand(this));
            };
            button.OnRightClick += (e) => {
                this.SendCommand(new SelectCommandCommand(this));
                Instantiate(CommandListController.Instance.CommandMenuPrefab, transform.root.GetComponentInChildren<Canvas>().transform)
                    .GetComponent<CommandMenuController>().Initialize(e.position);
            };
            button.OnDoubleClick += () => {
                this.SendCommand(new SelectCommandCommand(this));
                CommandPropertyController.Instance.Show();
            };

            TypeEventSystem.Global.Register<OnCommandRefreshEvent>(e => Refresh()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public abstract CommandControllerBase Initialize(CommandData _data);
        public abstract void Refresh();
        public void Select()
        {
            background.color = new Color(0.8f, 0.8f, 0.8f);
            IsCurrent = true;
        }
        public void DeSelect()
        {
            background.color = Color.white;
            IsCurrent = false;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
