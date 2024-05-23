using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Larvend.PlotEditor.DataSystem;
using Larvend.PlotEditor.Serialization;
using Larvend.PlotEditor.UI;
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

        public static Dictionary<Guid, CommandData> CommandDataDictionary = new();

        public static bool IsSaved = true;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Application.wantsToQuit += WantsToQuit;
            Application.targetFrameRate = 120;

            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1) OpenProject(args[1]);
        }

        public static bool IsProjectExist() => !string.IsNullOrEmpty(GUID) & Project != null;

        static bool WantsToQuit()
        {
            if (!string.IsNullOrEmpty(GUID)) Directory.Delete(ProjectFolderPath, true);
            return true;
        }

        public static void OpenProject(string _path)
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path)) return;

            if (ProjectHelper.OpenProject(_path, out GUID))
            {
                ProjectFilePath = _path;
                
                TypeEventSystem.Global.Send<PlotEditorUIRefreshEvent>();
            }
            else GUID = null;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}