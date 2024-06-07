using System;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    public class Selection : Command
    {
        public SelectionType selectionType;

        public Vector2 positionOffset;
        public Vector2 scaleOffset;
        public float displaySpeed = 1;

        private bool isFinished = false;

        public override CommandType GetCommandType() => CommandType.Avatar;
        public Selection(SelectionData _data) : base(_data)
        {
            selectionType = _data.SelectionType;
        }

        public override void OnEnter()
        {
            isFinished = false;
            SelectionController.Execute(this);
        }

        public override void OnUpdate()
        {
            return;
        }

        public override void OnExit()
        {
            SelectionController.Exit();
        }

        public override void Skip()
        {
            SelectionController.Skip();
            Finish();
        }

        public override bool IsFinished()
        {
            return isFinished;
        }

        public void Finish()
        {
            isFinished = true;
        }
    }
}