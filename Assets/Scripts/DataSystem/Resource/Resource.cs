using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    [System.Serializable, YamlSerializable]
    public abstract class ResourceBase
    {
        [YamlIgnore] public string guid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    [System.Serializable, YamlSerializable]
    public class ImageResource : ResourceBase
    {
        [YamlIgnore] public Texture2D texture;

        public ImageResource() {}
        public ImageResource(string _guid, string _name, Texture2D _texture)
        {
            guid = _guid;
            name = _name;
            texture = _texture;
        }

        public Sprite GetSprite()
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

    [System.Serializable, YamlSerializable]
    public class AudioResource : ResourceBase
    {
        [YamlIgnore] public AudioClip audioClip;

        public AudioResource() {}
        public AudioResource(string _guid, string _name, AudioClip _audioClip)
        {
            guid = _guid;
            name = _name;
            audioClip = _audioClip;
        }
    }

    [System.Serializable, YamlSerializable]
    public class VideoResource : ResourceBase
    {
        [YamlIgnore] public VideoClip videoClip;

        public VideoResource(string _guid, string _name, VideoClip _videoClip)
        {
            guid = _guid;
            name = _name;
            videoClip = _videoClip;
        }
    }
}