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

        public AudioSource audioSource;
        public PlotData plotData;

        void Awake()
        {
            audioSource = transform.GetComponent<AudioSource>();
        }

        public void OnSingletonInit()
        {
            
        }
    }

}