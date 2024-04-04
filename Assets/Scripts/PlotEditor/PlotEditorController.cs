using System.Collections;
using System.Collections.Generic;
using Kuchinashi.UI;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Larvend.PlotEditor
{
    public class PlotEditorController : MonoBehaviour , IController, ISingleton
    {
        public static PlotEditorController Instance => SingletonProperty<PlotEditorController>.Instance;
        public void OnSingletonInit() {}

        #region Menu

        private Button newProjectButton;
        private Button openProjectButton;
        private Button saveProjectButton;
        private Button saveAsProjectButton;

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
        }

        private void Initialize()
        {
            newProjectButton = transform.Find("Menu/Files/Dropdown/New").GetComponent<Button>();
            newProjectButton.onClick.AddListener(() => {
                this.SendCommand<NewProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            openProjectButton = transform.Find("Menu/Files/Dropdown/Open").GetComponent<Button>();
            openProjectButton.onClick.AddListener(() => {
                this.SendCommand<OpenProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            saveProjectButton = transform.Find("Menu/Files/Dropdown/Save").GetComponent<Button>();
            saveProjectButton.onClick.AddListener(() => {
                this.SendCommand<SaveProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            saveAsProjectButton = transform.Find("Menu/Files/Dropdown/SaveAs").GetComponent<Button>();
            saveAsProjectButton.onClick.AddListener(() => {
                this.SendCommand<SaveAsProjectCommand>();
                newProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

}