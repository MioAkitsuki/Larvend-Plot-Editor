using System.Collections;
using System.Collections.Generic;
using QFramework;
using Schwarzer.Windows;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using Larvend.PlotEditor.Serialization;
using Larvend.PlotEditor.UI;

namespace Larvend.PlotEditor.DataSystem
{
    [YamlSerializable]
    public partial class ResourceManager : ISingleton
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
            var _path = EditorUtility.OpenFilePanelWithFilters(title: "Select File", directory: Application.dataPath, filters: new string[] {"Supported Image Format", "png,jpg,jpeg,jfif"});
# else
            var _path = Dialog.OpenFileDialog(Title: "Select File", InitPath: Application.dataPath, Filter: "Supported Image Format (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg;*.jfif");
#endif

            if (ResourceHelper.OpenImageResource(_path, out var _resource))
            {
                foreach (var i in Instance.Images)
                {
                    if (i.Value.Md5 == _resource.Md5) return;
                }

                Instance.Images.Add(_resource.Guid, _resource);
                ResourceHelper.SaveImageResource(_resource);

                TypeEventSystem.Global.Send<OnResourcesChangedEvent>();
            }
        }

        #endregion

        #region Audio Resource

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
            var _path = Dialog.OpenFileDialog(Title: "Select File", InitPath: Application.dataPath, Filter: "Ogg Vorbis (*.ogg)|*.ogg");
#endif

            if (ResourceHelper.OpenAudioResource(_path, out var _resource))
            {
                foreach (var i in Instance.Audios)
                {
                    if (i.Value.Md5 == _resource.Md5) return;
                }

                Instance.Audios.Add(_resource.Guid, _resource);
                ResourceHelper.SaveAudioResource(_resource);

                TypeEventSystem.Global.Send<OnResourcesChangedEvent>();
            }
        }

        #endregion
    }
}
