using System;
using UnityEngine;

namespace Larvend
{
    [Serializable]
    public class Text : CommandBase
    {
        public enum TextType
        {
            FullScreen,
            ByBar,
            ByLeftBox,
            ByRightBox
        }

        private bool isFinished = false;

        public TextType textType;
        public string speaker;
        public string text;
        public int voiceId = -1;

        public Vector2 positionOffset;
        public float displaySpeed;

        public override CommandType GetCommandType()
        {
            return CommandType.Text;
        }

        public override void OnEnter()
        {
            isFinished = false;
        }

        public override void OnUpdate()
        {
            isFinished = true;
        }

        public override void OnExit()
        {
            throw new NotImplementedException();
        }

        public override bool IsFinished()
        {
            return isFinished;
        }
    }
}