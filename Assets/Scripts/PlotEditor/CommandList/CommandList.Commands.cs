using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;
using System.Linq;
using System;

namespace Larvend.PlotEditor.UI
{
    public class AddCommandCommand : AbstractCommand
    {
        public CommandType Type;
        public CommandData Data;
        public AddCommandCommand(CommandType _type, CommandData _data = null)
        {
            Type = _type;
            Data = _data;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            var data = Data;
            switch (Type)
            {
                case CommandType.Text:
                    data = Data == null ? TextData.Default : Data as TextData;
                    break;
                case CommandType.Background:
                    data = Data == null ? BackgroundData.Default : Data as BackgroundData;
                    break;
                case CommandType.Avatar:
                    data = Data == null ? AvatarData.Default : Data as AvatarData;
                    break;
                case CommandType.Selection:
                    data = Data == null ? SelectionData.Default : Data as SelectionData;
                    break;
                case CommandType.Sleep:
                    data = Data == null ? SleepData.Default : Data as SleepData;
                    break;
                case CommandType.Goto:
                    data = Data == null ? GotoData.Default : Data as GotoData;
                    break;
                case CommandType.Music:
                    data = Data == null ? MusicData.Default : Data as MusicData;
                    break;
            }
            ProjectManager.AddCommand(data);

            var command = GameObject.Instantiate(CommandListController.Instance.CommandPrefabs[Type], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(data);
            model.CommandControllerDictionary.Add(data.Guid, model.CommandControllers.AddLast(command));

            TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
        }
    }

    public class InsertCommandBeforeCommand : AbstractCommand
    {
        public CommandType Type;
        public CommandData Data;
        public InsertCommandBeforeCommand(CommandType _type, CommandData _data = null)
        {
            Type = _type;
            Data = _data;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            var data = Data;

            if (model.CurrentCommandController == null) return;

            switch (Type)
            {
                case CommandType.Text:
                    data = Data == null ? TextData.Default : Data as TextData;
                    break;
                case CommandType.Background:
                    data = Data == null ? BackgroundData.Default : Data as BackgroundData;
                    break;
                case CommandType.Avatar:
                    data = Data == null ? AvatarData.Default : Data as AvatarData;
                    break;
                case CommandType.Selection:
                    data = Data == null ? SelectionData.Default : Data as SelectionData;
                    break;
                case CommandType.Sleep:
                    data = Data == null ? SleepData.Default : Data as SleepData;
                    break;
                case CommandType.Goto:
                    data = Data == null ? GotoData.Default : Data as GotoData;
                    break;
                case CommandType.Music:
                    data = Data == null ? MusicData.Default : Data as MusicData;
                    break;
            }

            var index = ProjectManager.InsertCommand(data, model.CurrentCommandController.Value.Data.Guid, false);

            var command = GameObject.Instantiate(CommandListController.Instance.CommandPrefabs[Type], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(data);
            model.CommandControllerDictionary.Add(data.Guid, model.CommandControllers.AddAfter(model.CurrentCommandController, command));
            command.transform.SetSiblingIndex(index);

            TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
        }
    }

    public class InsertCommandAfterCommand : AbstractCommand
    {
        public CommandType Type;
        public CommandData Data;
        public InsertCommandAfterCommand(CommandType _type, CommandData _data = null)
        {
            Type = _type;
            Data = _data;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            var data = Data;

            if (model.CurrentCommandController == null) return;

            switch (Type)
            {
                case CommandType.Text:
                    data = Data == null ? TextData.Default : Data as TextData;
                    break;
                case CommandType.Background:
                    data = Data == null ? BackgroundData.Default : Data as BackgroundData;
                    break;
                case CommandType.Avatar:
                    data = Data == null ? AvatarData.Default : Data as AvatarData;
                    break;
                case CommandType.Selection:
                    data = Data == null ? SelectionData.Default : Data as SelectionData;
                    break;
                case CommandType.Sleep:
                    data = Data == null ? SleepData.Default : Data as SleepData;
                    break;
                case CommandType.Goto:
                    data = Data == null ? GotoData.Default : Data as GotoData;
                    break;
                case CommandType.Music:
                    data = Data == null ? MusicData.Default : Data as MusicData;
                    break;
            }

            var index = ProjectManager.InsertCommand(data, model.CurrentCommandController.Value.Data.Guid);

            var command = GameObject.Instantiate(CommandListController.Instance.CommandPrefabs[Type], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(data);
            model.CommandControllerDictionary.Add(data.Guid, model.CommandControllers.AddAfter(model.CurrentCommandController, command));
            command.transform.SetSiblingIndex(index);

            TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
        }
    }

    public class SelectCommandCommand : AbstractCommand
    {
        public CommandControllerBase Command;
        public SelectCommandCommand(CommandControllerBase _command)
        {
            Command = _command;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            
            if (model.CurrentCommandController != null)
            {
                if (model.CurrentCommandController.Value == Command) return;
                model.CurrentCommandController.Value.DeSelect();
            }
            model.CurrentCommandController = model.CommandControllers.Find(Command);
            model.CurrentCommandController.Value.Select();

            TypeEventSystem.Global.Send<OnCommandChangedEvent>();
        }
    }

    public class JumpToCommandCommand : AbstractCommand
    {
        public Guid CommandGuid;
        public JumpToCommandCommand(Guid _id)
        {
            CommandGuid = _id;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();

            if (!model.CommandControllerDictionary.TryGetValue(CommandGuid, out var command)) return;

            if (model.CurrentCommandController != null)
            {
                if (model.CurrentCommandController == command) return;
                model.CurrentCommandController.Value.DeSelect();
            }
            model.CurrentCommandController = command;
            model.CurrentCommandController.Value.Select();

            TypeEventSystem.Global.Send<OnCommandChangedEvent>();
        }
    }

    public class NextCommandCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            
            if (model.CurrentCommandController == null || model.CurrentCommandController.Next == null)
            {
                return;
            }
            model.CurrentCommandController.Value.DeSelect();
            model.CurrentCommandController = model.CurrentCommandController.Next;
            model.CurrentCommandController.Value.Select();

            TypeEventSystem.Global.Send<OnCommandChangedEvent>();
        }
    }

    public class PrevCommandCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            
            if (model.CurrentCommandController == null || model.CurrentCommandController.Previous == null)
            {
                return;
            }
            model.CurrentCommandController.Value.DeSelect();
            model.CurrentCommandController = model.CurrentCommandController.Previous;
            model.CurrentCommandController.Value.Select();

            TypeEventSystem.Global.Send<OnCommandChangedEvent>();
        }
    }

    public class RemoveCommandCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            if (model.CurrentCommandController == null) return;

            var pointer = model.CurrentCommandController;
            if (model.CurrentCommandController.Next != null)
            {
                model.CurrentCommandController.Next.Value.Select();
                model.CurrentCommandController = model.CurrentCommandController.Next;
            }
            else if (model.CurrentCommandController.Previous != null)
            {
                model.CurrentCommandController.Previous.Value.Select();
                model.CurrentCommandController = model.CurrentCommandController.Previous;
            }
            else
            {
                model.CurrentCommandController = null;
            }

            model.CommandControllerDictionary.Remove(pointer.Value.Data.Guid);
            model.CommandControllers.Remove(pointer);
            ProjectManager.RemoveCommand(pointer.Value.Data);

            GameObject.Destroy(pointer.Value.gameObject);

            TypeEventSystem.Global.Send<OnCommandChangedEvent>();
            TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
        }
    }
}
