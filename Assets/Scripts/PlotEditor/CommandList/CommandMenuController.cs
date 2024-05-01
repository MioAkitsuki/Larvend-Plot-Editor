using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Kuchinashi.UI;

namespace Larvend.PlotEditor.UI
{
    public class CommandMenuController : MonoBehaviour, IController
    {
        private PlotEditorModel mModel;

        private StandaloneMenu mMenu;

        private DropdownMenu insertBeforeToggle;

        private Button addTextCommandBeforeButton;
        private Button addBackgroundCommandBeforeButton;
        private Button addAvatarCommandBeforeButton;

        private DropdownMenu insertAfterToggle;

        private Button addTextCommandAfterButton;
        private Button addBackgroundCommandAfterButton;
        private Button addAvatarCommandAfterButton;

        private Button deleteCommandButton;

        void Awake()
        {
            mModel = this.GetModel<PlotEditorModel>();

            mMenu = GetComponent<StandaloneMenu>();

            insertBeforeToggle = transform.Find("InsertBefore").GetComponent<DropdownMenu>();

            addTextCommandBeforeButton = transform.Find("InsertBefore/Dropdown/Text").GetComponent<Button>();
            addTextCommandBeforeButton.onClick.AddListener(() => {
                this.SendCommand(new InsertCommandBeforeCommand(DataSystem.CommandType.Text));

                insertBeforeToggle.toggle.onClick.Invoke();
                mMenu.Toggle();
            });
            addBackgroundCommandBeforeButton = transform.Find("InsertBefore/Dropdown/Background").GetComponent<Button>();
            addBackgroundCommandBeforeButton.onClick.AddListener(() => {
                this.SendCommand(new InsertCommandBeforeCommand(DataSystem.CommandType.Background));
                
                insertBeforeToggle.toggle.onClick.Invoke();
                mMenu.Toggle();
            });
            addAvatarCommandBeforeButton = transform.Find("InsertBefore/Dropdown/Avatar").GetComponent<Button>();
            addAvatarCommandBeforeButton.onClick.AddListener(() => {
                this.SendCommand(new InsertCommandBeforeCommand(DataSystem.CommandType.Avatar));
                
                insertBeforeToggle.toggle.onClick.Invoke();
                mMenu.Toggle();
            });

            insertAfterToggle = transform.Find("InsertAfter").GetComponent<DropdownMenu>();

            addTextCommandAfterButton = transform.Find("InsertAfter/Dropdown/Text").GetComponent<Button>();
            addTextCommandAfterButton.onClick.AddListener(() => {
                this.SendCommand(new InsertCommandAfterCommand(DataSystem.CommandType.Text));

                insertAfterToggle.toggle.onClick.Invoke();
                mMenu.Toggle();
            });
            addBackgroundCommandAfterButton = transform.Find("InsertAfter/Dropdown/Background").GetComponent<Button>();
            addBackgroundCommandAfterButton.onClick.AddListener(() => {
                this.SendCommand(new InsertCommandAfterCommand(DataSystem.CommandType.Background));

                insertAfterToggle.toggle.onClick.Invoke();
                mMenu.Toggle();
            });
            addAvatarCommandAfterButton = transform.Find("InsertAfter/Dropdown/Avatar").GetComponent<Button>();
            addAvatarCommandAfterButton.onClick.AddListener(() => {
                this.SendCommand(new InsertCommandAfterCommand(DataSystem.CommandType.Avatar));

                insertAfterToggle.toggle.onClick.Invoke();
                mMenu.Toggle();
            });

            deleteCommandButton = transform.Find("Delete").GetComponent<Button>();
            deleteCommandButton.onClick.AddListener(() => {
                this.SendCommand(new RemoveCommandCommand());
                
                mMenu.Toggle();
            });
        }

        public CommandMenuController Initialize(Vector2 _pos)
        {
            mMenu.Toggle();

            Vector2 outVec;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), _pos, Camera.main, out outVec))
            {
                if (_pos.y <= 200)
                {
                    outVec.y += 200;
                }
                GetComponent<RectTransform>().anchoredPosition = outVec;
            }

            // var targetPos = _pos.y <= 200 ? Camera.main.ScreenToWorldPoint(new Vector2(_pos.x + 150, _pos.y + 100))
            //     : Camera.main.ScreenToWorldPoint(new Vector2(_pos.x + 150, _pos.y - 100));
            
            // transform.position = new Vector2(targetPos.x, targetPos.y);
            // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            
            return this;
        }

        public IArchitecture GetArchitecture()
        {
            return PlotEditor.Interface;
        }
    }
}