using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Kuchinashi.UI;

namespace Larvend.PlotEditor.UI
{
    public class FooterMenuController : MonoBehaviour, IController
    {
        #region Add Command

        private Button addCommandMenuToggle;

        private Button addTextCommandButton;
        private Button addBackgroundCommandButton;
        private Button addAvatarCommandButton;

        #endregion

        private Button removeCommandButton;

        void Awake()
        {
            Initialize();
            RefreshUIStatus();

            TypeEventSystem.Global.Register<PlotEditorUIRefreshEvent>(e => {
                RefreshUIStatus();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void RefreshUIStatus()
        {
            addCommandMenuToggle.interactable = !string.IsNullOrEmpty(ProjectManager.GUID);
        }

        private void Initialize()
        {
            addCommandMenuToggle = transform.Find("AddCommand/Toggle").GetComponent<Button>();

            addTextCommandButton = transform.Find("AddCommand/Dropdown/Text").GetComponent<Button>();
            addTextCommandButton.onClick.AddListener(() => {
                this.SendCommand(new AddCommandCommand(DataSystem.CommandType.Text));
            });
            
            addBackgroundCommandButton = transform.Find("AddCommand/Dropdown/Background").GetComponent<Button>();
            addBackgroundCommandButton.onClick.AddListener(() => {
                Debug.Log("Add Background");
            });

            addAvatarCommandButton = transform.Find("AddCommand/Dropdown/Avatar").GetComponent<Button>();
            addAvatarCommandButton.onClick.AddListener(() => {
                Debug.Log("Add Avatar");
            });

            removeCommandButton = transform.Find("RemoveCommand").GetComponent<Button>();
            removeCommandButton.onClick.AddListener(() => {
                this.SendCommand(new RemoveCommandCommand());
            });
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}