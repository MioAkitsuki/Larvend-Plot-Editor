using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class BackgroundCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Background;
        private BackgroundData _ => Data as BackgroundData;

        internal override void Awake()
        {
            base.Awake();
        }

        public override CommandControllerBase Initialize(CommandData _data)
        {
            Data = _data as BackgroundData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {_.Id}");
            typeText.SetText($"Type: {_.Type}");
        }
    }
}
