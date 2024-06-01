using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Larvend.PlotEditor.Serialization;
using Larvend.PlotEditor.DataSystem;

namespace Larvend.PlotEditor
{
    public class PlotManager : MonoSingleton<PlotManager>
    {
        public static bool HasPlot = false;

        // private AudioSource musicSource;
        // private AudioSource voiceSource;

        private Queue<Command> commands = new();

        public static CommandGroup currentGroup = new CommandGroup();
        public static CommandGroup automaticGroup = new CommandGroup();

        public bool IsCountingDown = false;
        private Coroutine CurrentCoroutine = null;

        void Awake()
        {
            // musicSource = transform.Find("MusicSource").GetComponent<AudioSource>();
            // voiceSource = transform.Find("VoiceSource").GetComponent<AudioSource>();

            TypeEventSystem.Global.Register<NextCommandEvent>(e => {
                NextCommand();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            transform.Find("Finish").gameObject.SetActive(false);
            ReadAndStart();
        }

        void Update()
        {
            currentGroup?.OnUpdate();

            if (currentGroup.IsFinished() && !automaticGroup.IsEmpty && !IsCountingDown)
            {
                currentGroup.OnExit();
                currentGroup = automaticGroup;
                currentGroup.OnEnter();

                automaticGroup = new CommandGroup();
            }
        }

        private void ReadAndStart()
        {
            commands = new();
            currentGroup = new();
            automaticGroup = new();

            IsCountingDown = false;
            CurrentCoroutine = null;
            StopAllCoroutines();

            foreach (var command in ProjectManager.Project.Commands)
            {
                commands.Enqueue(Command.Parse(command));
            }
            HasPlot = true;
            NextCommand();
        }

        private void NextCommand()
        {
            if (!currentGroup.IsEmpty && (!currentGroup.IsFinished() || IsCountingDown))
            {
                if (currentGroup.IsSkippable())
                {
                    currentGroup.Skip();

                    if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
                    IsCountingDown = false;

                    currentGroup.Clear();
                }
                else return;
            }
            else if (!IsCountingDown)
            {
                currentGroup?.OnExit();

                if (commands.Count == 0)
                {
                    transform.Find("Finish").gameObject.SetActive(true);
                    return;
                }
                
                currentGroup.Clear();
                automaticGroup.Clear();

                currentGroup.Add(commands.Dequeue());
                while (commands.Count > 0 && commands.Peek().Data.Timing == CommandTiming.WithPrevious)
                {
                    currentGroup.Add(commands.Dequeue());
                }

                if (commands.Count > 0 && commands.Peek().Data.Timing == CommandTiming.AfterPrevious)
                {
                    automaticGroup.Add(commands.Dequeue());
                    while (commands.Count > 0 && commands.Peek().Data.Timing == CommandTiming.WithPrevious)
                    {
                        automaticGroup.Add(commands.Dequeue());
                    }
                }

                currentGroup?.OnEnter();
                CurrentCoroutine = currentGroup.time == 0f ? null : StartCoroutine(Countdown(currentGroup.time));
            }
        }

        IEnumerator Countdown(float time)
        {
            IsCountingDown = true;
            yield return new WaitForSeconds(time);

            IsCountingDown = false;
            CurrentCoroutine = null;
        }
    }

}