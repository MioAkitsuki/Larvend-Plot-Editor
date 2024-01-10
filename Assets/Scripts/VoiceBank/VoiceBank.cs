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

            foreach (var voice in voiceBank.Voices)
                byteList.AddRange(VoiceSerializer.ToByte(voice));

            return byteList.ToArray();
        }

        public static VoiceBank Parse(byte[] bytes)
        {
            VoiceBank voiceBank = new VoiceBank();
            int startIndex = 0;

            while (startIndex < bytes.Length)
            {
                var voice = VoiceSerializer.Parse(bytes, startIndex, out int endIndex);
                voiceBank.Voices.Add(voice);
                startIndex = endIndex;
            }

            return voiceBank;
        }
    }
}