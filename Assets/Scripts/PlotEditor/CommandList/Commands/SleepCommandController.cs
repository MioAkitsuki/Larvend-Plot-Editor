using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class SleepCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Sleep;
        private SleepData _ => Data as SleepData;

        private TMP_Text timeText;

        internal override void Awake()
        {
            base.Awake();

            timeText = transform.Find("Content/Time").GetComponent<TMP_Text>();
        }

        public override CommandControllerBase Initialize(CommandData _data)
        {
            Data = _data as SleepData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {_.Id}");
            typeText.SetText($"Type: {_.Type}");

            timeText.SetText($"{_.Time} Second");
        }
    }
}
