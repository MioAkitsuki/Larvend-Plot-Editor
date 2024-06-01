using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public enum SelectionType
    {
        None = 0,
        FullScreen = 1,
        Right = 2
    }

    [YamlSerializable, Serializable]
    public class SelectionData : CommandData
    {
        public override CommandType Type => CommandType.Selection;
        public SelectionType SelectionType { get; set; }
        public int Count { get; set; }
        public List<OptionData> Options { get; set; }

        public override void Update(CommandData data)
        {
            if (data is SelectionData selectionData)
            {
                SelectionType = selectionData.SelectionType;
                
                Count = selectionData.Count == 0 ? Count : selectionData.Count;
                Options = selectionData.Options.IsNullOrEmpty() ? Options : selectionData.Options;
            }
        }

        public string GetOptionString()
        {
            string res = "";
            foreach (var option in Options)
            {
                res += $" - {option.Option}\n";
            }
            return res;
        }

        [YamlIgnore] public static SelectionData Default => new SelectionData()
        {
            SelectionType = SelectionType.FullScreen,
            Timing = CommandTiming.OnClick,
            Skippable = false,
            Count = 0,
            Options = new List<OptionData>()
        };
    }
}

namespace Larvend.PlotEditor.DataSystem
{
    [YamlSerializable, Serializable]
    public class OptionData
    {
        public string Option { get; set; }
        public bool Interactable { get; set;}
        public Guid GotoGuid { get; set; }

        [YamlIgnore] public static OptionData Default => new OptionData()
        {
            Option = "",
            Interactable = true,
            GotoGuid = new Guid()
        };
    }
}