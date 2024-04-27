using System.Collections;
using System.Collections.Generic;
using System.IO;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using UnityEditor;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public partial class ProjectManager : MonoSingleton<ProjectManager> , ICanSendCommand
    {
        public static string GUID;
        public static string ProjectFolderPath => GUID == null ? "" : Path.Combine(Application.temporaryCachePath, GUID);
        public static string ProjectFilePath;

        public static ProjectData Project
        {
            get => _project ??= new ProjectData();
            set => _project = value;
        }
        private static ProjectData _project;

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

        public static bool IsProjectExist() => !string.IsNullOrEmpty(GUID) & Project != null;

        static bool WantsToQuit()
        {
            if (!string.IsNullOrEmpty(GUID)) Directory.Delete(ProjectFolderPath, true);
            return true;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}