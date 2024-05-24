using System;
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

            foreach (var item in Project.Commands)
            {
                ProjectManager.CommandDataDictionary.Add(item.Guid, item);
            }
        }

        public static void AddCommand(CommandData _data)
        {
            if (!IsProjectExist()) return;

            _data.Guid = Guid.NewGuid();

            Project.Commands.Add(_data);
            CommandDataDictionary.Add(_data.Guid, _data);
        }

        public static int InsertCommand(CommandData _data, Guid _pos, bool insertAfter = true)
        {
            if (!IsProjectExist() || !CommandDataDictionary.TryGetValue(_pos, out var command)) return -1;

            _data.Guid = Guid.NewGuid();
            var index = Project.Commands.IndexOf(command);

            if (insertAfter)
            {
                if (index == Project.Commands.Count - 1) Project.Commands.Add(_data);
                else Project.Commands.Insert(index + 1, _data);

                CommandDataDictionary.Add(_data.Guid, _data);

                return index + 1;
            }
            else
            {
                Project.Commands.Insert(index, _data);
                CommandDataDictionary.Add(_data.Guid, _data);

                return index;
            }
        }

        public static void RemoveCommand(CommandData _data)
        {
            if (!IsProjectExist()) return;

            Project.Commands.Remove(_data);
            CommandDataDictionary.Remove(_data.Guid);
        }

        public static CommandData GetCommand(int _index)
        {
            if (!IsProjectExist() || _index >= Project.Commands.Count) return null;

            return Project.Commands[_index];
        }

        public static CommandData GetCommand(Guid _guid)
        {
            if (!IsProjectExist()) return null;

            return CommandDataDictionary.TryGetValue(_guid, out var command) ? command : null;
        }

        public static int GetCommandIndex(CommandData _data)
        {
            if (!IsProjectExist() || _data == null) return -1;

            return Project.Commands.IndexOf(_data);
        }

        public static int GetCommandIndex(Guid _guid)
        {
            if (!IsProjectExist() || _guid == Guid.Empty) return -1;

            return Project.Commands.IndexOf(CommandDataDictionary[_guid]);
        }

        public static bool FindNearestCommand<T>(Guid _pos, out T _data) where T : CommandData
        {
            _data = null;
            if (!IsProjectExist() || !CommandDataDictionary.TryGetValue(_pos, out var command) || Project.Commands.Count == 0) return false;

            var _id = Project.Commands.IndexOf(command);

            for (int i = _id + 1; i < Project.Commands.Count; i++)
            {
                if (Project.Commands[i].Timing != CommandTiming.WithPrevious) break;
                if (Project.Commands[i] is T)
                {
                    _data = Project.Commands[i] as T;
                    return true;
                }
            }

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