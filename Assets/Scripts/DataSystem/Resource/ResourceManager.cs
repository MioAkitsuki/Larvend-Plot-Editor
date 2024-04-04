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
    [YamlSerializable]
    public class ResourceManager : ISingleton
    {
        [YamlIgnore] public static ResourceManager Instance
        {
            get => _instance ??= new ResourceManager();
            set => _instance = value;
        }
        private static ResourceManager _instance;
        public void OnSingletonInit() { }

        #region Image Resource

        [YamlMember] public Dictionary<string, ImageResource> Images
        {
            get => _images ??= new Dictionary<string, ImageResource>();
            set => _images = value;
        }
        private static Dictionary<string, ImageResource> _images;

        public static void ImportImageResource()
        {
            if (string.IsNullOrEmpty(ProjectManager.ProjectFolderPath)) return;

# if UNITY_EDITOR
            var _path = EditorUtility.OpenFilePanelWithFilters(title: "Select File", directory: Application.dataPath, filters: new string[] {"Image files", "png,jpg,jpeg"});
# else
            var _path = Dialog.OpenFileDialog(Title: "Select File", InitPath: Application.dataPath, Filter: "png|jpg|jpeg");
#endif

            if (ResourceHelper.OpenImageResource(_path, out var _resource))
            {
                Instance.Images.Add(_resource.guid, _resource);
                ResourceHelper.SaveImageResource(_resource);
            }
        }

        #endregion

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
            if (Instance.Images.TryGetValue(_resource.guid, out var image))
            {
                image.texture = _resource.texture;
            }
            else
            {
                Instance.Images.Add(_resource.guid, _resource);
            }
        }

        public static void TryLinkResource(AudioResource _resource)
        {
            if (Instance.Audios.TryGetValue(_resource.guid, out var audio))
            {
                audio.audioClip = _resource.audioClip;
            }
            else
            {
                Instance.Audios.Add(_resource.guid, _resource);
            }
        }

        [YamlMember] public Dictionary<string, AudioResource> Audios
        {
            get => _audios ??= new Dictionary<string, AudioResource>();
            set => _audios = value;
        }
        private static Dictionary<string, AudioResource> _audios;

        public static void ImportAudioResource()
        {
            if (string.IsNullOrEmpty(ProjectManager.ProjectFolderPath)) return;

# if UNITY_EDITOR
            var _path = EditorUtility.OpenFilePanelWithFilters(title: "Select File", directory: Application.dataPath, filters: new string[] {"Ogg Vorbis", "ogg"});
# else
            var _path = Dialog.OpenFileDialog(Title: "Select File", InitPath: Application.dataPath, Filter: "ogg");
#endif

            if (ResourceHelper.OpenAudioResource(_path, out var _resource))
            {
                Instance.Audios.Add(_resource.guid, _resource);
                ResourceHelper.SaveAudioResource(_resource);
            }
        }
    }
}
