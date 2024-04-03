using System.Collections;
using System.Collections.Generic;
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
        }

        private void RefreshUIStatus()
        {
            if (string.IsNullOrEmpty(ProjectManager.GUID))
            {
                saveProjectButton.interactable = false;
                saveAsProjectButton.interactable = false;
            }
        }

        private void Initialize()
        {
            newProjectButton = transform.Find("Menu/Files/Dropdown/New").GetComponent<Button>();
            newProjectButton.onClick.AddListener(() => this.SendCommand<NewProjectCommand>());
            openProjectButton = transform.Find("Menu/Files/Dropdown/Open").GetComponent<Button>();
            openProjectButton.onClick.AddListener(() => this.SendCommand<OpenProjectCommand>());
            saveProjectButton = transform.Find("Menu/Files/Dropdown/Save").GetComponent<Button>();
            saveProjectButton.onClick.AddListener(() => this.SendCommand<SaveProjectCommand>());
            saveAsProjectButton = transform.Find("Menu/Files/Dropdown/SaveAs").GetComponent<Button>();
            saveAsProjectButton.onClick.AddListener(() => this.SendCommand<SaveAsProjectCommand>());
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

}