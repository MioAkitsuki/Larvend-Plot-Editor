using System;
using UnityEngine;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    [Serializable]
    public class Background : Command
    {
        public BackgroundType backgroundType;
        public Sprite sprite;

        public Vector2 positionOffset;
        public Vector2 scaleOffset;
        public float displaySpeed = 1;

        private bool isFinished = false;

        public override CommandType GetCommandType() => CommandType.Background;
        public Background(BackgroundData _data) : base(_data)
        {
            sprite = ResourceManager.TryGetResource<ImageResource>(_data.SourceGuid, out var res) ? res.GetSprite() : null;
            backgroundType = _data.BackgroundType;
        }

        public override void OnEnter()
        {
            isFinished = false;
            BackgroundController.Instance.Execute(this);
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
            BackgroundController.Instance.Skip();
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