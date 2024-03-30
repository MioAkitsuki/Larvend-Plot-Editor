using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Larvend
{
    [System.Serializable, YamlSerializable]
    public abstract class ResourceBase
    {
        [YamlIgnore] public string guid;
        public string name;
        public string description;
    }

    [System.Serializable, YamlSerializable]
    public class ImageResource : ResourceBase
    {
        [YamlIgnore] public Texture2D texture;

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
}