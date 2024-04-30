using System.Collections;
using System.Collections.Generic;
using QFramework;
using Schwarzer.Windows;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using Larvend.PlotEditor.Serialization;

namespace Larvend.PlotEditor.DataSystem
{
    public partial class ResourceManager
    {
        public static void SaveAllResources()
        {
            foreach (var _image in Instance.Images)
            {
                ResourceHelper.SaveImageResource(_image.Value);
            }
        }

        public static void AddResource(ResourceBase _resource)
        {
            switch (_resource)
            {
                case ImageResource _image:
                    TryLinkResource(_image);
                    break;
                case AudioResource _audio:
                    TryLinkResource(_audio);
                    break;
            }
        }

        public static void TryLinkResource(ImageResource _resource)
        {
            if (Instance.Images.TryGetValue(_resource.Guid, out var image))
            {
                image.Guid = _resource.Guid;
                image.texture = _resource.texture;
            }
            else
            {
                Instance.Images.Add(_resource.Guid, _resource);
            }
        }

        public static void TryLinkResource(AudioResource _resource)
        {
            if (Instance.Audios.TryGetValue(_resource.Guid, out var audio))
            {
                audio.Guid = _resource.Guid;
                audio.audioClip = _resource.audioClip;
            }
            else
            {
                Instance.Audios.Add(_resource.Guid, _resource);
            }
        }
    }
}
