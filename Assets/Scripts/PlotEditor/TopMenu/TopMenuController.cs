using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Kuchinashi.UI;
using Larvend.PlotEditor.DataSystem;
using UnityEngine.SceneManagement;

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

        #region Library

        private Button libraryManagerButton;
        private Button importImageButton;
        private Button importAudioButton;

        #endregion
        
        private Button previewButton;

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
            saveProjectButton.interactable = ProjectManager.IsProjectExist();
            saveAsProjectButton.interactable = ProjectManager.IsProjectExist();

            newCommandButton.interactable = ProjectManager.IsProjectExist();

            libraryManagerButton.interactable = ProjectManager.IsProjectExist();
            importAudioButton.interactable = ProjectManager.IsProjectExist();
            importImageButton.interactable = ProjectManager.IsProjectExist();

            previewButton.interactable = ProjectManager.IsProjectExist();
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
                openProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            saveProjectButton = transform.Find("Files/Dropdown/Save").GetComponent<Button>();
            saveProjectButton.onClick.AddListener(() => {
                this.SendCommand<SaveProjectCommand>();
                saveProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            saveAsProjectButton = transform.Find("Files/Dropdown/SaveAs").GetComponent<Button>();
            saveAsProjectButton.onClick.AddListener(() => {
                this.SendCommand<SaveAsProjectCommand>();
                saveAsProjectButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            newCommandButton = transform.Find("Edit/Dropdown/NewCommand").GetComponent<Button>();
            newCommandButton.onClick.AddListener(() => {
                // this.SendCommand<NewCommandCommand>();
                newCommandButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            libraryManagerButton = transform.Find("Library/Dropdown/LibraryManager").GetComponent<Button>();
            libraryManagerButton.onClick.AddListener(() => {
                LibraryManagerController.CallUp();
                libraryManagerButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            importImageButton = transform.Find("Library/Dropdown/ImportImage").GetComponent<Button>();
            importImageButton.onClick.AddListener(() => {
                ResourceManager.ImportImageResource();
                importImageButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            importAudioButton = transform.Find("Library/Dropdown/ImportAudio").GetComponent<Button>();
            importAudioButton.onClick.AddListener(() => {
                ResourceManager.ImportAudioResource();
                importAudioButton.GetComponentInParent<DropdownMenu>().toggle.onClick.Invoke();
            });

            previewButton = transform.Find("Preview").GetComponent<Button>();
            previewButton.onClick.AddListener(() => {
                StartCoroutine(LoadPreviewCoroutine());
            });
        }

        IEnumerator LoadPreviewCoroutine()
        {
            var mAsyncOperation = SceneManager.LoadSceneAsync("PreviewScene", LoadSceneMode.Additive);
            mAsyncOperation.allowSceneActivation = true;

            yield return mAsyncOperation;
            yield return new WaitForEndOfFrame();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("PreviewScene"));
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }

}