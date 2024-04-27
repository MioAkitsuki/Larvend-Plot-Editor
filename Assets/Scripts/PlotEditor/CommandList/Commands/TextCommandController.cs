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

        public override CommandControllerBase Initialize(int _id)
        {
            Id = _id;

            Refresh();

            return this;
        }

        public override CommandControllerBase Initialize(int _id, CommandData _data)
        {
            Id = _id;
            Data = _data as TextData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {Id}");
            typeText.SetText($"Type: {Type}");

            if (Data != null)
            {
                speakerText.SetText($"{_.Speaker}");
                contentText.SetText($"{_.Content}");
            }
        }
    }
}
