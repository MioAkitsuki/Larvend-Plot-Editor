using System;
using System.Collections.Generic;

namespace Larvend
{
    [Serializable]
    public class VoiceBank
    {
        public List<Voice> Voices = new List<Voice>();
    }

    public static class VoiceBankSerializer
    {
        public static byte[] ToByte(VoiceBank voiceBank)
        {
            List<byte> byteList = new List<byte>();
            int id = 0;

            foreach (var voice in voiceBank.Voices)
            {
                byteList.AddRange(BitConverter.GetBytes(id++));
                byteList.AddRange(VoiceSerializer.ToByte(voice));
            }

            return byteList.ToArray();
        }

        public static VoiceBank Parse(byte[] bytes)
        {
            VoiceBank voiceBank = new VoiceBank();
            int startIndex = 0;

            while (startIndex < bytes.Length)
            {
                var id = BitConverter.ToInt32(bytes, startIndex);
                var voice = VoiceSerializer.Parse(bytes, startIndex + 4, out int endIndex);

                if (id == voiceBank.Voices.Count - 1) voiceBank.Voices.Add(voice);
                else throw new Exception();
                
                startIndex = endIndex;
            }

            return voiceBank;
        }
    }
}