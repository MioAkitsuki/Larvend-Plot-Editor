using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Kuchinashi.UI;

namespace Larvend.PlotEditor.UI
{
    public class TopMenuController : MonoBehaviour, IController
    {
        #region Menu

        private Button newProjectButton;
        private Button openProjectButton;
        private Button saveProjectButton;
        private Button saveAsProjectButton;

        #endregion

        #region Edit

        private Button newCommandButton;

        #endregion

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
            saveProjectButton.interactable = !string.IsNullOrEmpty(ProjectManager.GUID);
            saveAsProjectButton.interactable = !string.IsNullOrEmpty(ProjectManager.GUID);

            newCommandButton.interactable = !string.IsNullOrEmpty(ProjectManager.GUID);
        }

        private void Initialize()
        {
            newProjectButton = transform.Find("Files/Dropdown/New").GetComponent<Button>();
            newProjectButton.onClick.AddListener(() => {
                this.SendCommand<NewProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            openProjectButton = transform.Find("Files/Dropdown/Open").GetComponent<Button>();
            openProjectButton.onClick.AddListener(() => {
                this.SendCommand<OpenProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            saveProjectButton = transform.Find("Files/Dropdown/Save").GetComponent<Button>();
            saveProjectButton.onClick.AddListener(() => {
                this.SendCommand<SaveProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            saveAsProjectButton = transform.Find("Files/Dropdown/SaveAs").GetComponent<Button>();
            saveAsProjectButton.onClick.AddListener(() => {
                this.SendCommand<SaveAsProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            newCommandButton = transform.Find("Edit/Dropdown/NewCommand").GetComponent<Button>();
            newCommandButton.onClick.AddListener(() => {
                // this.SendCommand<NewCommandCommand>();
                newCommandButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

}