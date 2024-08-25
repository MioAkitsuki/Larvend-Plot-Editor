using System;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public class Goto : Command
    {
        private bool isFinished = false;
        private Guid gotoGuid;

        public override CommandType GetCommandType() => CommandType.Goto;
        public Goto(GotoData _data) : base(_data)
        {
            gotoGuid = _data.GotoGuid;
        }

        public override void OnEnter()
        {
            isFinished = false;
            PlotManager.Instance.JumpToCommand(gotoGuid);

            Finish();
        }

        public override void OnUpdate()
        {
            return;
        }

        public override void OnExit()
        {
            return;
        }

        public override void Skip()
        {
            return;
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