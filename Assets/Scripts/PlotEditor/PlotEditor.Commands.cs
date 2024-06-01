using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;
using Schwarzer.Windows;
using Larvend.PlotEditor.Serialization;
using UnityEngine;
using UnityEditor;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor.UI
{
    public class NewProjectCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            if (!string.IsNullOrEmpty(ProjectManager.GUID))
            {
                Debug.LogWarning("You have already opened a project. Cleaning temp file...");

                this.GetModel<PlotEditorModel>().CloseProject();
                ResourceManager.ClearAllResources();
                ProjectManager.CloseProject();
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
# if UNITY_EDITOR
            var _path = EditorUtility.OpenFilePanelWithFilters(title: "Open Project", directory: Application.dataPath, filters: new string[] {"Larvend Plot File", "lpf"});
# else
            var _path = Dialog.OpenFileDialog(Title: "Open Project", InitPath: Application.dataPath, Filter: "Larvend Plot File (*.lpf)|*.lpf|All Files|*.*");
# endif

            if (string.IsNullOrEmpty(_path)) return;
            if (!string.IsNullOrEmpty(ProjectManager.GUID))
            {
                Debug.LogWarning("You have already opened a project. Cleaning temp file...");

                this.GetModel<PlotEditorModel>().CloseProject();
                ResourceManager.ClearAllResources();
                ProjectManager.CloseProject();
            }
            
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
# if UNITY_EDITOR
                var _targetPath = EditorUtility.SaveFilePanel(title: "Save", directory: Application.dataPath, defaultName: "New Plot", extension: "lpf");
# else
                var _targetPath = Dialog.SaveFileDialog(Title: "Save", Filename: "New Plot.lpf", InitPath: Application.dataPath, Filter: "Larvend Plot File (*.lpf)|*.lpf|All Files|*.*");
# endif
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

# if UNITY_EDITOR
                var _targetPath = EditorUtility.SaveFilePanel(title: "Save As", directory: Application.dataPath, defaultName: "New Plot", extension: "lpf");
# else
                var _targetPath = Dialog.SaveFileDialog(Title: "Save As", Filename: "New Plot.lpf", InitPath: Application.dataPath, Filter: "Larvend Plot File (*.lpf)|*.lpf|All Files|*.*");
# endif
            if (string.IsNullOrEmpty(_targetPath)) return;

            ProjectHelper.SaveProject(_targetPath);
        }
    }
}
