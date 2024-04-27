using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class TextCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Text;

        public override CommandControllerBase Initialize(int _id)
        {
            Id = _id;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {Id}");
            typeText.SetText($"Type: {Type}");
        }
    }
}
