using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supernova.Utils;


namespace Supernova.Unity
{
    public class SoundManager : SerializedMonoBehaviour
    {
        public static SoundManager _instance = null;

        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (SoundManager)FindObjectOfType(typeof(SoundManager));
                }

                return _instance;
            }
        }

        public AudioSource[] audioEffects;
        public AudioSource audioBgm;

        public Dictionary<string, AudioClip> effectSound;
        public Dictionary<string, AudioClip> bgmSound;

        public float volumeEffect = 1.0f;
        public float volumeBgm = 1.0f;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(_instance);
            else
                _instance = this;

            audioBgm = new AudioSource();
            //CreateData();
        }

        public void PlayEffect(string name)
        {
            for (int i = 0; i < audioEffects.Length; i++)
            {
                if (audioEffects[i].clip == null || audioEffects[i].isPlaying == false)
                {
                    if (effectSound.ContainsKey(name) == false)
                        return;

                    audioEffects[i].clip = effectSound[name];
                    audioEffects[i].Play();
                    return;
                }
            }

            Log.Info("audio source full");
        }

        public void PlayBgm(string name)
        {
            audioBgm.Stop();

            if (bgmSound.ContainsKey(name) == false)
                return;

            audioBgm.clip = bgmSound[name];
            audioBgm.Play();
        }

        public void StopEffect()
        {
            for (int i = 0; i < audioEffects.Length; i++)
            {
                audioEffects[i].Stop();
            }
        }

        public void StopBgm()
        {
            audioBgm.Stop();
        }

        public void SetVolumeEffect(float volume)
        {
            volumeEffect = volume;

            for (int i = 0; i < audioEffects.Length; i++)
            {
                audioEffects[i].volume = volumeEffect;
            }
        }

        public void SetVolumeBgm(float volume)
        {
            volumeBgm = volume;
            audioBgm.volume = volumeBgm;
        }



        /*[OnInspectorInit]
        private void CreateData()
        {
            effectSound = new Dictionary<string, AudioClip>();
            bgmSound = new Dictionary<string, AudioClip>();
        }*/
    }
}