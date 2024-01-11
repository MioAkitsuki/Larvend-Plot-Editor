using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Larvend
{
    public class StageController : MonoBehaviour , ISingleton
    {
        public static StageController Instance
        {
            get { return MonoSingletonProperty<StageController>.Instance; }
        }

        void Awake()
        {
        }

        public void OnSingletonInit()
        {
            
        }
    }
}