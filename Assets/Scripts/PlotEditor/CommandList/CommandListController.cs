using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor.UI
{
    public class CommandListController : MonoBehaviour , IController
    {
        private PlotEditorModel mModel;
        public static Transform CommandListParent;

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
                        AddTextCommand(data);
                        break;
                }
            }
        }

        private void AddTextCommand(TextData data)
        {
            var command = Instantiate(PlotEditorController.Instance.CommandPrefabs[CommandType.Text], CommandListParent)
                .GetComponent<CommandControllerBase>().Initialize(mModel.CommandControllers.Count, data);
            
            mModel.CommandControllers.AddLast(command);
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}
