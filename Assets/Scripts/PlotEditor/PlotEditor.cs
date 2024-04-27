using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Larvend.PlotEditor
{
    public class PlotEditor : Architecture<PlotEditor>
    {
        protected override void Init()
        {
            this.RegisterModel(new PlotEditorModel());
        }
    }
}
