using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;
using Schwarzer.Windows;
using Larvend.PlotEditor.Serialization;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class NewProjectCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            if (!string.IsNullOrEmpty(ProjectManager.GUID))
            {
                Debug.LogWarning("You have already opened a project. Cleaning temp file...");
            }

            if (ProjectHelper.NewProject(out ProjectManager.GUID))
            {
                ProjectManager.ProjectFilePath = null;
                
                TypeEventSystem.Global.Send<PlotEditorUIRefreshEvent>();
            }
            else ProjectManager.GUID = null;
        }
    }

    public class OpenProjectCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            if (!string.IsNullOrEmpty(ProjectManager.GUID))
            {
                Debug.LogWarning("You have already opened a project. Cleaning temp file...");
            }

            var _path = Dialog.OpenFileDialog(Title: "Open Project", InitPath: Application.dataPath);
            ProjectManager.OpenProject(_path);
        }
    }

    public class SaveProjectCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            if (string.IsNullOrEmpty(ProjectManager.GUID)) return;

            if (string.IsNullOrEmpty(ProjectManager.ProjectFilePath))
            {
                var _targetPath = Dialog.SaveFileDialog(Title: "Save", Filename: "New Plot.lpf", InitPath: Application.dataPath, Filter: "lpf");
                if (string.IsNullOrEmpty(_targetPath)) return;

                ProjectHelper.SaveProject(_targetPath);
            }
            else
            {
                ProjectHelper.SaveProject(ProjectManager.ProjectFilePath);
            }
        }
    }

    public class SaveAsProjectCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            if (string.IsNullOrEmpty(ProjectManager.GUID)) return;

            var _targetPath = Dialog.SaveFileDialog(Title: "Save As", Filename: "New Plot.lpf", InitPath: Application.dataPath, Filter: "lpf");
            if (string.IsNullOrEmpty(_targetPath)) return;

            ProjectHelper.SaveProject(_targetPath);
        }
    }
}
