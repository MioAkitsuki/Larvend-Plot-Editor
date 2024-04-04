using System;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public class Background : CommandBase
    {
        public enum BackgroundType
        {
            None,
            Front,
            Middle,
            Back
        }

        public enum AppearMethod
        {
            Appear,
            CrossFade,
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
            BackgroundController.Execute(this);
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

        public void Finish()
        {
            isFinished = true;
        }
    }
}