using System;
using Microsoft.Xna.Framework.Audio;

namespace SideScroller01
{
    static class SoundManager
    {

        static AudioEngine audioEngine;
        static WaveBank waveBank;
        static SoundBank soundBank;

        // New Sound Voice Overs
        //static AudioEngine voiceAudioEngine;
        //static WaveBank voiceWaveBank;
        //static SoundBank voiceSoundBank;

        //private static Cue voiceSound;

        public static void Initialize()
        {
            audioEngine = new AudioEngine("Content/Sounds/SoundFX.xgs");
            waveBank= new WaveBank(audioEngine, "Content/Sounds/SoundFX.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Sounds/SoundFX.xsb");

            // New Sound Voice Overs
            //voiceAudioEngine = new AudioEngine("Content/Sounds/SoundFX.xgs");
            //voiceWaveBank = new WaveBank(audioEngine, "Content/Sounds/SoundFX.xwb");
            //voiceSoundBank = new SoundBank(audioEngine, "Content/Sounds/SoundFX.xsb");

        }

        public static void Update()
        {
            audioEngine.Update();
            //voiceAudioEngine.Update();

        }

        public static void PlaySound(string soundName)
        {
            soundBank.PlayCue(soundName);
        }

        //public static void PlayVoiceSound(string soundName)
        //{
        //    voiceSound = voiceSoundBank.GetCue(soundName);
        //    voiceSound.Play();    
        //}

        //public static StopVoiceSound() {
        //   voiceSound.Stop(AudioStopOptions.Immediate);
        //}
    }
}
