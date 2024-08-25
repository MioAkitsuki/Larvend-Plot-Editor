using System;
using System.Collections.Generic;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public abstract class Command
    {
        public bool skippable = true;
        public float time;

        public Command(CommandData _data)
        {
            Data = _data;
            
            time = _data.Time;
            skippable = _data.Skippable;
        }

        public CommandData Data { get; set; }
        abstract public CommandType GetCommandType();

        abstract public void OnEnter();
        abstract public void OnUpdate();
        abstract public void OnExit();
        abstract public void Skip();

        abstract public bool IsFinished();

        public static Command Parse(CommandData _data)
        {
            return _data switch
            {
                TextData textData => new Text(textData),
                BackgroundData backgroundData => new Background(backgroundData),
                AvatarData avatarData => new Avatar(avatarData),
                SelectionData selectionData => new Selection(selectionData),
                GotoData gotoData => new Goto(gotoData),
                _ => null,
            };
        }
    }

    public class CommandGroup
    {
        public List<Command> commandGroup = new List<Command>();
        public float time;

        public void Add(Command command)
        {
            commandGroup.Add(command);
            if (time < command.time) time = command.time;
        }

        public void Clear()
        {
            commandGroup.Clear();
            time = 0f;
        }

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