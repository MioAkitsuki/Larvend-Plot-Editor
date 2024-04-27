using System.Collections;
using System.Collections.Generic;
using System.IO;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public class ProjectManager : MonoBehaviour , ISingleton
    {
        public static ProjectManager Instance => SingletonProperty<ProjectManager>.Instance;
        public void OnSingletonInit() { }

        public static string GUID;
        public static string ProjectFolderPath => GUID == null ? "" : Path.Combine(Application.temporaryCachePath, GUID);
        public static string ProjectFilePath;

        public static bool IsSaved = true;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Application.wantsToQuit += WantsToQuit;
            Application.targetFrameRate = 120;
        }
        
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                ResourceManager.ImportAudioResource();
            }
        }

        static bool WantsToQuit()
        {
            if (!string.IsNullOrEmpty(GUID)) Directory.Delete(ProjectFolderPath, true);
            return true;
        }
    }
}