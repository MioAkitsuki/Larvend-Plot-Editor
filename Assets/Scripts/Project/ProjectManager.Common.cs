using System.Collections;
using System.Collections.Generic;
using System.IO;
using Larvend.PlotEditor.DataSystem;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public partial class ProjectManager
    {
        public static void InitializeProject(ProjectData _data)
        {
            Project = _data;

            TypeEventSystem.Global.Send<OnProjectInitializedEvent>();
        }

        public static void AddCommand(CommandData _data)
        {
            if (!IsProjectExist()) return;

            Project.Commands.Add(_data);
        }

        public static void RemoveCommand(CommandData _data)
        {
            if (!IsProjectExist()) return;

            Project.Commands.Remove(_data);
        }
    }
}