using System.Collections;
using System.Collections.Generic;
using Larvend.PlotEditor.UI;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public class PlotEditorModel : AbstractModel
    {
        public LinkedList<CommandControllerBase> CommandControllers = new();
        public LinkedListNode<CommandControllerBase> CurrentCommandController = null;

        public List<ImageResourceController> ImageResourceControllers = new();
        public ImageResourceController CurrentImageResourceController = null;
        
        protected override void OnInit() {}
    }
}
