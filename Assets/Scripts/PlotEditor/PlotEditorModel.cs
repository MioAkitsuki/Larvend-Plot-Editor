using System;
using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;
using Larvend.PlotEditor.UI;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public class PlotEditorModel : AbstractModel
    {
        public LinkedList<CommandControllerBase> CommandControllers = new();
        public LinkedListNode<CommandControllerBase> CurrentCommandController = null;

        public Dictionary<Guid, LinkedListNode<CommandControllerBase>> CommandControllerDictionary = new();

        public List<ImageResourceController> ImageResourceControllers = new();
        public ImageResourceController CurrentImageResourceController = null;
        
        protected override void OnInit() {}

        public void CloseProject()
        {
            foreach(var controller in CommandControllers)
            {
                controller.gameObject.DestroySelf();
            }
            CommandControllers = new();
            CurrentCommandController = null;

            CommandControllerDictionary.Clear();

            foreach(var controller in ImageResourceControllers)
            {
                controller.gameObject.DestroySelf();
            }
            ImageResourceControllers = new();
            CurrentImageResourceController = null;
        }
    }
}
