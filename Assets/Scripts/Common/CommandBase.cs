using System;

namespace Larvend
{
    public enum CommandType
    {
        Text,
        Animation,
        Sound,
        Avatar,
        Tachie,
        Background
    }

    [Serializable]
    public abstract class CommandBase
    {
        abstract public CommandType GetCommandType();

        abstract public void OnEnter();

        abstract public void OnUpdate();

        abstract public void OnExit();

        abstract public bool IsFinished();
    }
}