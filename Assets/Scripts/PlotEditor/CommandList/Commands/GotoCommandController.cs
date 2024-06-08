using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class GotoCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Goto;
        private GotoData _ => Data as GotoData;

        private TMP_Text gotoGuidText;

        internal override void Awake()
        {
            base.Awake();

            gotoGuidText = transform.Find("Content/GotoGuid").GetComponent<TMP_Text>();
        }

        public override CommandControllerBase Initialize(CommandData _data)
        {
            Data = _data as GotoData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {_.Id}");
            typeText.SetText($"Type: {_.Type}");

            gotoGuidText.SetText(ProjectManager.GetCommandIndex(_.GotoGuid).ToString());
        }
    }
}
