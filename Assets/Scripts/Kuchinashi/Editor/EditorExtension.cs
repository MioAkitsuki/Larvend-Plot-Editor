# if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kuchinashi
{
    public class EditorExtension : EditorWindow
    {
        [MenuItem ("Kuchinashi/Open Persistent Data Folder")]
        public static void OpenPersistentDataFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath, Application.productName));
        }

        [MenuItem ("Kuchinashi/Open Temporary Cache Folder")]
        public static void OpenTemporaryDataFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Application.temporaryCachePath, Application.productName));
        }
    }

}

#endif