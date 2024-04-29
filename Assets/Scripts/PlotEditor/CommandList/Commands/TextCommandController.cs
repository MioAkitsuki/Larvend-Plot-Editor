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
        private TextData _ => Data as TextData;

        private TMP_Text speakerText;
        private TMP_Text contentText;

        internal override void Awake()
        {
            base.Awake();
            speakerText = transform.Find("Content/Speaker").GetComponent<TMP_Text>();
            contentText = transform.Find("Content/Content").GetComponent<TMP_Text>();
        }

        public override CommandControllerBase Initialize(CommandData _data)
        {
            Data = _data as TextData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {_.Id}");
            typeText.SetText($"Type: {_.Type}");

            if (Data != null)
            {
                speakerText.SetText($"{_.Speaker}");
                contentText.SetText($"{_.Content}");
            }
        }
    }
}
