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

            _data.Id = Project.Commands.Count;
            Project.Commands.Add(_data);
        }

        public static void InsertCommand(CommandData _data, int _index)
        {
            if (!IsProjectExist() || _index > Project.Commands.Count) return;

            _data.Id = _index;

            if (_index == Project.Commands.Count) Project.Commands.Add(_data);
            else Project.Commands.Insert(_index, _data);

            for (int i = _index + 1; i < Project.Commands.Count; i++)
            {
                Project.Commands[i].Id = i;
            }
        }

        public static void RemoveCommand(CommandData _data)
        {
            if (!IsProjectExist()) return;

            int index = _data.Id;
            Project.Commands.Remove(_data);

            for (int i = index; i < Project.Commands.Count; i++)
            {
                Project.Commands[i].Id = i;
            }
        }

        public static CommandData GetCommand(int _id)
        {
            if (!IsProjectExist()) return null;

            return Project.Commands[_id];
        }

        public static bool FindNearestCommand<T>(int _id, out T _data) where T : CommandData
        {
            _data = null;
            if (!IsProjectExist()) return false;

            if (Project.Commands.Count == 0 || _id < 0 || _id >= Project.Commands.Count) return false;

            for (int i = _id; i >= 0; i--)
            {
                if (Project.Commands[i] is T)
                {
                    _data = Project.Commands[i] as T;
                    return true;
                }
            }

            return false;
        }
    }
}