using System.Collections;
using System.Collections.Generic;
using QFramework;
using Schwarzer.Windows;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using Larvend.PlotEditor.Serialization;
using System.IO;

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

        public static void ClearAllResources()
        {
            Instance.Images.Clear();
            Instance.Audios.Clear();
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

        public static void RemoveResource(string _guid)
        {
            if (Instance.Images.ContainsKey(_guid))
            {
                Instance.Images.Remove(_guid);

                var path = Path.Combine(ProjectManager.ProjectFolderPath, $"resources/image/{_guid}.png");
                if (File.Exists(path)) File.Delete(path);
            }

            if (Instance.Audios.ContainsKey(_guid))
            {
                Instance.Audios.Remove(_guid);

                var path = Path.Combine(ProjectManager.ProjectFolderPath, $"resources/audio/{_guid}.png");
                if (File.Exists(path)) File.Delete(path);
            }
        }

        public static void TryLinkResource(ImageResource _resource)
        {
            if (Instance.Images.TryGetValue(_resource.Guid, out var image))
            {
                image.Guid = _resource.Guid;
                image.texture = _resource.texture;
                image.Md5 = _resource.Md5;
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

        public static string GetResourceName(string _guid)
        {
            if (Instance.Images.TryGetValue(_guid, out var image))
            {
                return image.Name;
            }

            if (Instance.Audios.TryGetValue(_guid, out var audio))
            {
                return audio.Name;
            }

            return "";
        }

        public static bool TryGetResource<T>(string _guid, out T _resource) where T : ResourceBase
        {
            if (Instance.Images.TryGetValue(_guid, out var image) && image is T)
            {
                _resource = image as T;
                return true;
            }

            if (Instance.Audios.TryGetValue(_guid, out var audio) && audio is T)
            {
                _resource = audio as T;
                return true;
            }

            _resource = null;
            return false;
        }
    }
}
