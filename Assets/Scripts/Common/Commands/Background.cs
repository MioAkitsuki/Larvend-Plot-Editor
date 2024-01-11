using System;
using UnityEngine;

namespace Larvend
{
    [Serializable]
    public class Background : CommandBase
    {
        public enum BackgroundType
        {
            Front,
            Middle,
            Back
        }

        public enum AppearMethod
        {
            Appear,
            Fade,
            FadeFromBlack,
            FadeFromWhite,
            FadeFromTransparent
        }

        public AppearMethod appearMethod;
        public BackgroundType backgroundType;
        public Sprite sprite;

        public Vector2 positionOffset;
        public Vector2 scaleOffset;
        public float displaySpeed = 1;

        private bool isFinished = false;

        public override CommandType GetCommandType()
        {
            return CommandType.Background;
        }

        public override void OnEnter()
        {
            isFinished = false;
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
            throw new NotImplementedException();
        }

        public override bool IsFinished()
        {
            return isFinished;
        }
    }
}