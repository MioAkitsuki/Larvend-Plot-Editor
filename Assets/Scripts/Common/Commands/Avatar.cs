using System;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public class Avatar : CommandBase
    {
        public enum AvatarType
        {
            None,
            Left,
            Right
        }

        public enum AppearMethod
        {
            Appear,
            CrossFade,
            FadeFromTransparent
        }

        public AppearMethod appearMethod;
        public AvatarType avatarType;
        public Sprite sprite;

        public Vector2 positionOffset;
        public Vector2 scaleOffset;
        public float displaySpeed = 1;

        private bool isFinished = false;

        public override CommandType GetCommandType()
        {
            return CommandType.Avatar;
        }

        public override void OnEnter()
        {
            isFinished = false;
            AvatarController.Execute(this);
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
            AvatarController.Skip();
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