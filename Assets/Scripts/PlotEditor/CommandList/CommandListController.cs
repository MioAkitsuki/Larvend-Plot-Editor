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
                switch (command)
                {
                    case TextData data:
                        mModel.CommandControllers.AddLast(Instantiate(CommandPrefabs[CommandType.Text], CommandListParent)
                            .GetComponent<CommandControllerBase>().Initialize(data));
                        break;
                    case BackgroundData data:
                        mModel.CommandControllers.AddLast(Instantiate(CommandPrefabs[CommandType.Background], CommandListParent)
                            .GetComponent<CommandControllerBase>().Initialize(data));
                        break;
                    case AvatarData data:
                        mModel.CommandControllers.AddLast(Instantiate(CommandPrefabs[CommandType.Avatar], CommandListParent)
                            .GetComponent<CommandControllerBase>().Initialize(data));
                        break;
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
