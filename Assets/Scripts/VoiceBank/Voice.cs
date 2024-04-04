using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Larvend
{
    [Serializable]
    public class Voice
    {
        public string name;
        public AudioClip audioClip;

        public Voice(AudioClip clip)
        {
            audioClip = clip;
            name = clip.name;
        }
    }
}