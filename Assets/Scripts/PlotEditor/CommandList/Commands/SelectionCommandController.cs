using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class SelectionCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Selection;
        private SelectionData _ => Data as SelectionData;

        private TMP_Text selectionTypeText;
        private TMP_Text optionsText;

        internal override void Awake()
        {
            base.Awake();
            selectionTypeText = transform.Find("Content/SelectionType").GetComponent<TMP_Text>();
            optionsText = transform.Find("Content/Options").GetComponent<TMP_Text>();
        }

        public override CommandControllerBase Initialize(CommandData _data)
        {
            Data = _data as SelectionData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {_.Id}");
            typeText.SetText($"Type: {_.Type}");

            if (Data != null)
            {
                selectionTypeText.SetText($"{_.SelectionType}");
                optionsText.SetText(_.GetOptionString());
            }
        }
    }
}
