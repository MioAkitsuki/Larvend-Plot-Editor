using System.Collections;
using System.Collections.Generic;
using QFramework;
using Schwarzer.Windows;
using Serialization;
using UnityEditor;
using UnityEngine;

namespace Larvend
{
    public class ResourceManager : ISingleton
    {
        public static ResourceManager Instance;
        public void OnSingletonInit()
        {

        }

        public static Dictionary<string, ImageResource> Images
        {
            get => _images ??= new Dictionary<string, ImageResource>();
            set => _images = value;
        }
        private static Dictionary<string, ImageResource> _images;

        public static void ImportImageResource()
        {
            if (string.IsNullOrEmpty(Global.CurrentGUID)) return;

# if UNITY_EDITOR
            var _path = EditorUtility.OpenFilePanelWithFilters(title: "Select File", directory: Application.dataPath, filters: new string[] {"Image files", "png,jpg,jpeg"});
# else
            var _path = Dialog.OpenFileDialog(Title: "Select File", InitPath: Application.dataPath, Filter: "png|jpg|jpeg");
#endif

            if (ResourceHelper.OpenImageResource(_path, out var _resource))
            {
                Images.Add(_resource.guid, _resource);
                ResourceHelper.SaveImageResource(_resource);
            }
        }

        public static void SaveAllResources()
        {
            foreach (var _image in Images)
            {
                ResourceHelper.SaveImageResource(_image.Value);
            }
        }

        public static void AddResource(ResourceBase _resource)
        {
            switch (_resource)
            {
                case ImageResource _image:
                    Images.Add(_image.guid, _image);
                    break;
            }
        }

        public static List<AudioClip> Musics;
        public static List<AudioClip> Sounds;
        public static List<AudioClip> Voices;
    }
}
