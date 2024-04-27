using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor.Serialization;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    public class PlotManager : MonoBehaviour , ISingleton
    {
        public static PlotManager Instance
        {
            get { return MonoSingletonProperty<PlotManager>.Instance; }
        }

        public static bool HasPlot = false;

        private AudioSource musicSource;
        private AudioSource voiceSource;
        public PlotData plotData;

        private Queue<Command> commands = new Queue<Command>();

        public static CommandGroup currentGroup = new CommandGroup();
        public static CommandGroup automaticGroup = new CommandGroup();

        void Awake()
        {
            musicSource = transform.Find("MusicSource").GetComponent<AudioSource>();
            voiceSource = transform.Find("VoiceSource").GetComponent<AudioSource>();

            TypeEventSystem.Global.Register<NextCommandEvent>(e => {
                if (HasPlot) NextCommand();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                ResourceManager.ImportAudioResource();
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                SerializationHelper.SerializeProject();
            }

            currentGroup?.OnUpdate();

            if (currentGroup.IsFinished() && !automaticGroup.IsEmpty)
            {
                currentGroup.OnExit();
                currentGroup = automaticGroup;
                currentGroup.OnEnter();

                automaticGroup = new CommandGroup();
            }
        }

        private void ReadAndStart()
        {
            foreach (var command in plotData.Data)
            {
                commands.Enqueue(command);
            }
            HasPlot = true;
            NextCommand();
        }

        private void NextCommand()
        {
            if (!currentGroup.IsEmpty && !currentGroup.IsFinished())
            {
                if (currentGroup.IsSkippable()) currentGroup.Skip();
                else return;
            }
            else
            {
                currentGroup?.OnExit();
                
                currentGroup.commandGroup.Clear();
                automaticGroup.commandGroup.Clear();
                currentGroup.commandGroup.Add(commands.Dequeue());
                while (commands.Count > 0 && commands.Peek().appearTiming == Command.AppearTiming.Simultaneously)
                {
                    currentGroup.commandGroup.Add(commands.Dequeue());
                }

                if (commands.Count > 0 && commands.Peek().appearTiming == Command.AppearTiming.AfterPreviousFinished)
                {
                    automaticGroup.commandGroup.Add(commands.Dequeue());
                    while (commands.Count > 0 && commands.Peek().appearTiming == Command.AppearTiming.Simultaneously)
                    {
                        automaticGroup.commandGroup.Add(commands.Dequeue());
                    }
                }

                currentGroup?.OnEnter();
            }

            if (commands.Count == 0) HasPlot = false;
        }

        public void OnSingletonInit()
        {
            
        }
    }

}