using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Video;
using YamlDotNet.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    [System.Serializable, YamlSerializable]
    public abstract class ResourceBase
    {
        [YamlIgnore] public string Guid { get; set; }
        public string Md5 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [System.Serializable, YamlSerializable]
    public class ImageResource : ResourceBase
    {
        [YamlIgnore] public Texture2D texture;

        public ImageResource() {}
        public ImageResource(string _guid, string _name, Texture2D _texture, string _md5)
        {
            Guid = _guid;
            Name = _name;
            texture = _texture;
            Md5 = _md5;
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
        public AudioResource(string _guid, string _name, AudioClip _audioClip, string _md5)
        {
            Guid = _guid;
            Name = _name;
            audioClip = _audioClip;
            Md5 = _md5;
        }
    }

    [System.Serializable, YamlSerializable]
    public class VideoResource : ResourceBase
    {
        [YamlIgnore] public VideoClip videoClip;

        public VideoResource(string _guid, string _name, VideoClip _videoClip)
        {
            Guid = _guid;
            Name = _name;
            videoClip = _videoClip;
        }
    }
}