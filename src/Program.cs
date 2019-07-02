using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotFeather;
using Groorine;
using Groorine.DataModel;

namespace Example
{
    public class GroorineAudioSource : IAudioSource
    {
        // サンプル数は計算不可能なのでnull
        public int? Samples => null;

        // MIDI
        public int Channels => 2;

        public int Bits => 16;

        public int SampleRate { get; }

        public int Latency { get; }

        public GroorineAudioSource(Stream midi, int latency = 20, int sampleRate = 44100)
        {
            data = SmfParser.Parse(midi);
            Latency = latency;
            SampleRate = sampleRate;
        }

        public IEnumerable<(short left, short right)> EnumerateSamples(int? loopStart)
        {
            var player = new Player();
            player.Load(data);
            player.Play();
            var buf = player.CreateBuffer(Latency);
            while (player.IsPlaying)
            {
                player.GetBuffer(buf);
                for (var i = 0; i < buf.Length; i += 2)
                {
                    var t = (buf[i], buf[i + 1]);
                    yield return t;
                }
            }
            player.Stop();
        }

        MidiFile data;
    }
}
