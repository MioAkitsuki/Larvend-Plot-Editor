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
        public enum PlayMode
        {
            Manual,
            Auto
        }

        public PlayMode playMode;
        public float time;

        abstract public CommandType GetCommandType();

        abstract public void OnEnter();

        abstract public void OnUpdate();

        abstract public void OnExit();

        abstract public bool IsFinished();
    }
}