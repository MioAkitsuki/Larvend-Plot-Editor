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
        public int id;
        public string name;
        public string description;
        public AudioClip audioClip;
    }

    public static class VoiceSerializer
    {
        public static byte[] ToByte(Voice voice)
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(voice.id));
            byteList.AddRange(AudioSerializer.ToByte(voice.audioClip));

            return byteList.ToArray();
        }

        public static Voice Parse(byte[] bytes, int startIndex, out int endIndex)
        {
            Voice voice = new Voice
            {
                id = BitConverter.ToInt32(bytes, startIndex),
                audioClip = AudioSerializer.Parse(bytes, startIndex + 4, out endIndex)
            };

            return voice;
        }

        public static class AudioSerializer
        {
            public static byte[] ToByte(AudioClip audioClip)
            {
                List<byte> byteList = new List<byte>();
        
                //编码name
                byte[] nameByte = System.Text.Encoding.UTF8.GetBytes(audioClip.name);
                byte[] nameByteLength = BitConverter.GetBytes(nameByte.Length);
                byteList.AddRange(nameByteLength);                                      //4字节nameLength
                byteList.AddRange(nameByte);                                            //nameLength字节name
        
                //编码samples
                byte[] samplesByte = BitConverter.GetBytes(audioClip.samples);          //4字节sample
                byteList.AddRange(samplesByte);
        
                //编码channels
                byte[] channelsByte = BitConverter.GetBytes(audioClip.channels);        //4字节channels
                byteList.AddRange(channelsByte);
        
                //编码frequency
                byte[] frequencyByte = BitConverter.GetBytes(audioClip.frequency);      //4字节frequency
                byteList.AddRange(frequencyByte);
        
                //编码_3D
                byte[] _3DByte = BitConverter.GetBytes(audioClip.ambisonic);            //1字节_3D
                byteList.AddRange(_3DByte);
        
                //编码data
                int dataLength = audioClip.samples * audioClip.channels;
        
                float[] data = new float[dataLength];
                audioClip.GetData(data, 0);
        
                int dataByteLength = data.Length * 2;                                   //1个float转1个short，而1个short是2字节，这里相当于压缩了空间
                byte[] dataByteLengthByte = BitConverter.GetBytes(dataByteLength);      //4字节dataLength
                byteList.AddRange(dataByteLengthByte);
        
                foreach (float f in data)                                               //dataLength字节data
                {
                    short temp = (short)(f * short.MaxValue);
        
                    byte[] byteArrTemp = BitConverter.GetBytes(temp);
        
                    byteList.AddRange(byteArrTemp);
                }
        
                return byteList.ToArray();
            }

            public static AudioClip Parse(byte[] byteArray, int startIndex, out int endIndex)
            {
                int offset = startIndex;
        
                //解码nameLength，4字节
                int nameLength = BitConverter.ToInt32(byteArray, offset);
                offset += 4;
        
                //解码name，nameLength字节
                string name = System.Text.Encoding.UTF8.GetString(byteArray, offset, nameLength);
                offset += nameLength;
        
        
                //解码samples,4字节
                int samples = BitConverter.ToInt32(byteArray, offset);
                offset += 4;
        
                //解码channels，4字节
                int channels = BitConverter.ToInt32(byteArray, offset);
                offset += 4;
        
                //解码frequency，4字节
                int frequency = BitConverter.ToInt32(byteArray, offset);
                offset += 4;
        
                //解码_3D，1字节
                bool _3D = BitConverter.ToBoolean(byteArray, offset);
                offset += 1;
        
                //解码dataLength，4字节
                int dataLength = BitConverter.ToInt32(byteArray, offset);
                offset += 4;
        
                float[] data = new float[dataLength / 2];
                //解码
                for (int i = 0; i < dataLength; i += 2)
                {
                    short tempShort = BitConverter.ToInt16(byteArray, offset + i);
        
                    data[i / 2] = (float)tempShort / short.MaxValue;
                }
        
                offset += dataLength;
                endIndex = offset;
        
                AudioClip ac = AudioClip.Create(name, samples, channels, frequency, false);
                ac.SetData(data, 0);
        
                return ac;
            }
        
            /// <summary>
            /// 多声道改单声道，并压缩为22050
            /// </summary>
            /// <param name="audioClip"></param>
            /// <returns></returns>
            public static AudioClip NormalizedClip(AudioClip audioClip)
            {
                int newFrequency = 22050;
        
                if (audioClip.channels == 1 && audioClip.frequency == newFrequency) return audioClip;
        
                int dataLength = audioClip.samples * audioClip.channels;
                float[] data = new float[dataLength];
                audioClip.GetData(data, 0);
        
                //压缩比
                float rate = (float)audioClip.frequency * audioClip.channels / newFrequency;
                int newDataLength = (int)(dataLength / rate);
        
                float[] newData = new float[newDataLength];
        
                for (int i = 0; i < newData.Length; i++)
                {
                    int sampleIndex = (int)(i * rate);
        
                    float f = data[sampleIndex];
        
                    newData[i] = f;
                }
        
                AudioClip newClip = AudioClip.Create(audioClip.name, newDataLength, 1, newFrequency, false);
                newClip.SetData(newData, 0);
        
                return newClip;
            }
        }
    }
}