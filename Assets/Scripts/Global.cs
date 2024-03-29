using System.IO;
using QFramework;
using UnityEngine;

namespace Larvend
{
    public class Global : ISingleton
    {
        public static Global Instance { get; private set; }
        public void OnSingletonInit()
        {
            Instance = this;
        }

        public static string CurrentGUID;
        public static string CurrentProjectPath => CurrentGUID == null ? "" : Path.Combine(Application.temporaryCachePath, CurrentGUID);
    }
}