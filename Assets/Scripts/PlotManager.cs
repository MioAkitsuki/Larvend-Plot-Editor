using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Larvend
{
    public class PlotManager : MonoBehaviour , ISingleton
    {
        public static PlotManager Instance
        {
            get { return MonoSingletonProperty<PlotManager>.Instance; }
        }

        private AudioSource musicSource;
        private AudioSource voiceSource;
        public PlotData plotData;

        void Awake()
        {
            musicSource = transform.Find("MusicSource").GetComponent<AudioSource>();
            voiceSource = transform.Find("VoiceSource").GetComponent<AudioSource>();
        }

        public void OnSingletonInit()
        {
            
        }
    }

}