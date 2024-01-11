using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Larvend
{
    
    [CreateAssetMenu(fileName = "PlotData", menuName = "Larvend/PlotData", order = 0)]
    public class PlotData : ScriptableObject
    {
        public string Id;
        public PlotHeader Header;

        [SerializeReference]
        public List<CommandBase> Data = new List<CommandBase>();

        #if UNITY_EDITOR
        [CustomEditor(typeof(PlotData))]
        public class PlotDataEditor : Editor
        {
            public PlotData plotData;
            private int selectedIndex;
            private static List<Type> commandTypes = new List<Type>();

            [DidReloadScripts]
            private static void OnRecompile()
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var types = assemblies.SelectMany(assembly => assembly.GetTypes());
                var filterTypes = types.Where(type => type.IsSubclassOf(typeof(CommandBase)) && !type.ContainsGenericParameters && type.IsClass);
                commandTypes = filterTypes.ToList();
            }

            private void OnEnable()
            {
                plotData = (PlotData)target;
                selectedIndex = 0;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                serializedObject.Update();

                string[] names = new string[commandTypes.Count];
                for (int i = 0; i < commandTypes.Count; i++)
                    names[i] = commandTypes[i].Name;
                selectedIndex = EditorGUILayout.Popup("Command Type", selectedIndex, names);
                Type commandType = Type.GetType("Larvend." + names[selectedIndex]);

                if (GUILayout.Button("Add"))
                {
                    var obj = Activator.CreateInstance(commandType) as CommandBase;
                    if (obj == null) return;
                    plotData.Data.Add(obj);
                }

                serializedObject.ApplyModifiedProperties();
            }
            #endif
        }
    }
}
