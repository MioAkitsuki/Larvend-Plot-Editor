using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Larvend.PlotEditor.DataSystem;
using TMPro;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public class AvatarCommandController : CommandControllerBase
    {
        public override CommandType Type => CommandType.Avatar;
        private AvatarData _ => Data as AvatarData;

        private TMP_Text avatarTypeText;
        private TMP_Text contentText;

        internal override void Awake()
        {
            base.Awake();

            avatarTypeText = transform.Find("Content/AvatarType").GetComponent<TMP_Text>();
            contentText = transform.Find("Content/Content").GetComponent<TMP_Text>();
        }

        public override CommandControllerBase Initialize(CommandData _data)
        {
            Data = _data as AvatarData;

            Refresh();

            return this;
        }

        public override void Refresh()
        {
            idText.SetText($"ID: {_.Id}");
            typeText.SetText($"Type: {_.Type}");

            avatarTypeText.SetText($"{_.AvatarType}");
            contentText.SetText($"{ResourceManager.GetResourceName(_.SourceGuid)}");
        }
    }
}
