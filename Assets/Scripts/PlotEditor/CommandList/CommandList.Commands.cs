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
        public AddCommandCommand(CommandType _type)
        {
            Type = _type;
        }

        protected override void OnExecute()
        {
            switch (Type)
            {
                case CommandType.Text:
                    AddTextCommand();
                    break;
            }
        }

        private void AddTextCommand()
        {
            var model = this.GetModel<PlotEditorModel>();
            var command = GameObject.Instantiate(PlotEditorController.Instance.CommandPrefabs[CommandType.Text], CommandListController.CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(model.CommandControllers.Count);
            
            model.CommandControllers.AddLast(command);
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
        }
    }

    public class RemoveCommandCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            if (model.CurrentCommandController == null) return;

            UpdateFollowingsId();

            var pointer = model.CurrentCommandController;
            if (model.CurrentCommandController.Previous != null)
            {
                model.CurrentCommandController.Previous.Value.Select();
                model.CurrentCommandController = model.CurrentCommandController.Previous;
            }
            else if (model.CurrentCommandController.Next != null)
            {
                model.CurrentCommandController.Next.Value.Select();
                model.CurrentCommandController = model.CurrentCommandController.Next;
            }

            model.CommandControllers.Remove(pointer);
            GameObject.Destroy(pointer.Value.gameObject);
        }

        private void UpdateFollowingsId()
        {
            var model = this.GetModel<PlotEditorModel>();
            
            var pointer = model.CurrentCommandController;
            while (pointer.Next != null)
            {
                pointer.Next.Value.Id--;
                pointer = pointer.Next;
                pointer.Value.Refresh();
            }
        }
    }
}
