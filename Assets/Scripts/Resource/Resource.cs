using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larvend
{
    public abstract class ResourceBase
    {
        public string guid;
        public string name;
    }

    public class ImageResource : ResourceBase
    {
        public Texture2D texture;

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