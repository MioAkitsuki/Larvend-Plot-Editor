using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor;
using Larvend.PlotEditor.DataSystem;
using Kuchinashi;

namespace Larvend.PlotEditor.UI
{
    public class CommandListController : MonoSingleton<CommandListController> , IController
    {
        private PlotEditorModel mModel;
        public static Transform CommandListParent;

        public SerializableDictionary<CommandType, GameObject> CommandPrefabs;
        public GameObject CommandMenuPrefab;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();
            CommandListParent = transform.Find("Scroll View/Viewport/Content");

            TypeEventSystem.Global.Register<OnProjectInitializedEvent>(e => {
                InitializeProject();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void InitializeProject()
        {
            foreach (var command in ProjectManager.Project.Commands)
            {
                mModel.CommandControllerDictionary.Add(command.Guid, mModel.CommandControllers.AddLast(
                    Instantiate(CommandPrefabs[command.Type], CommandListParent)
                    .GetComponent<CommandControllerBase>().Initialize(command)));
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
