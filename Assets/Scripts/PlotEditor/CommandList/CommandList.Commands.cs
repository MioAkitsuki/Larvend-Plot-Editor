using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;
using Schwarzer.Windows;
using Larvend.PlotEditor.Serialization;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;
using System.Linq;

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
            }
            ProjectManager.AddCommand(data);

            var command = GameObject.Instantiate(CommandListController.Instance.CommandPrefabs[Type], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(data);
            model.CommandControllers.AddLast(command);

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
            }

            ProjectManager.InsertCommand(data, model.CurrentCommandController.Value.Data.Id);

            var command = GameObject.Instantiate(CommandListController.Instance.CommandPrefabs[Type], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(data);
            model.CommandControllers.AddAfter(model.CurrentCommandController, command);
            command.transform.SetSiblingIndex(data.Id);

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
            }

            ProjectManager.InsertCommand(data, model.CurrentCommandController.Value.Data.Id + 1);

            var command = GameObject.Instantiate(CommandListController.Instance.CommandPrefabs[Type], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(data);
            model.CommandControllers.AddAfter(model.CurrentCommandController, command);
            command.transform.SetSiblingIndex(data.Id);

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

            model.CommandControllers.Remove(pointer);
            ProjectManager.RemoveCommand(pointer.Value.Data);

            GameObject.Destroy(pointer.Value.gameObject);

            TypeEventSystem.Global.Send<OnCommandChangedEvent>();
            TypeEventSystem.Global.Send<OnCommandRefreshEvent>();
        }
    }
}
