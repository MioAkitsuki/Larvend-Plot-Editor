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
    public class SelectImageResourceCommand : AbstractCommand
    {
        public ImageResourceController Resource;
        public SelectImageResourceCommand(ImageResourceController _resource)
        {
            Resource = _resource;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            
            if (model.CurrentImageResourceController != null)
            {
                if (model.CurrentImageResourceController == Resource) return;
                model.CurrentImageResourceController.DeSelect();
            }
            model.CurrentImageResourceController = Resource;
            model.CurrentImageResourceController.Select();

            TypeEventSystem.Global.Send<OnCurrentImageResourceChangedEvent>();
        }
    }

    public class SelectAudioResourceCommand : AbstractCommand
    {
        public AudioResourceController Resource;
        public SelectAudioResourceCommand(AudioResourceController _resource)
        {
            Resource = _resource;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PlotEditorModel>();
            
            if (model.CurrentAudioResourceController != null)
            {
                if (model.CurrentAudioResourceController == Resource) return;
                model.CurrentAudioResourceController.DeSelect();
            }
            model.CurrentAudioResourceController = Resource;
            model.CurrentAudioResourceController.Select();

            TypeEventSystem.Global.Send<OnCurrentAudioResourceChangedEvent>();
        }
    }
}
