using System;
using System.Collections.Generic;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public abstract class CommandBase
    {
        public enum AppearTiming
        {
            BySequence,
            Simultaneously,
            AfterPreviousFinished
        }

        public bool skippable = true;
        public AppearTiming appearTiming;
        public float time;

        abstract public CommandType GetCommandType();

        abstract public void OnEnter();
        abstract public void OnUpdate();
        abstract public void OnExit();
        abstract public void Skip();

        abstract public bool IsFinished();
    }

    public class CommandGroup
    {
        public List<CommandBase> commandGroup = new List<CommandBase>();
        public void OnEnter()
        {
            foreach (var command in commandGroup)
            {
                command.OnEnter();
            }
        }

        public void OnUpdate()
        {
            foreach (var command in commandGroup)
            {
                command.OnUpdate();
            }
        }

        public void OnExit()
        {
            foreach (var command in commandGroup)
            {
                command.OnExit();
            }
        }

        public void Skip()
        {
            foreach (var command in commandGroup)
            {
                command.Skip();
            }
        }

        public bool IsSkippable()
        {
            foreach (var command in commandGroup)
            {
                if (!command.skippable) return false;
            }
            return true;
        }

        public bool IsFinished()
        {
            foreach (var command in commandGroup)
            {
                if (!command.IsFinished()) return false;
            }
            return true;
        }

        public bool IsEmpty => commandGroup.Count == 0;
    }
}