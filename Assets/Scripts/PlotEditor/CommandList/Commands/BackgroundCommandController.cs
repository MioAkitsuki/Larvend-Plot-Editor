using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using TMPro;

namespace Larvend.PlotEditor.UI
{
    public class BackgroundCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Background;
        private BackgroundData _ => Data as BackgroundData;

        private TMP_Text backgroundTypeText;
        private TMP_Text contentText;

        internal override void Awake()
        {
            base.Awake();

            backgroundTypeText = transform.Find("Content/BackgroundType").GetComponent<TMP_Text>();
            contentText = transform.Find("Content/Content").GetComponent<TMP_Text>();
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

            backgroundTypeText.SetText($"{_.BackgroundType}");
            contentText.SetText($"{ResourceManager.GetResourceName(_.SourceGuid)}");
        }
    }
}
