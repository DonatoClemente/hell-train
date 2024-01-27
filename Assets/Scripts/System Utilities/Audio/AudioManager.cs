using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace HellTrain
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [SerializeField] private AudioSource _musicSource, _fxSource, _fxSource_Reversed, _stepSource;

        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySoundFX(AudioClip clip)
        {
            _fxSource.PlayOneShot(clip);
        }

        public void PlaySoundFX(AudioClip clip, float volume)
        {
            _fxSource.PlayOneShot(clip, volume);
        }

        // Will want to refactor so that it dynamically add's audiosources and destroys them per clip that is the same.
        public void PlayFootStepFX(AudioClip clip)
        {
            _stepSource.clip = clip;
            _stepSource.Play();
        }
        
        public void PlayReversedSoundFX(AudioClip clip)
        {
            _fxSource_Reversed.timeSamples = clip.samples - 1;
            _fxSource_Reversed.clip = clip;
            _fxSource_Reversed.Play();
        }

        public void PlayReversedSoundFX(AudioClip clip, float volume)
        {
            _fxSource_Reversed.timeSamples = clip.samples - 1;
            _fxSource_Reversed.clip = clip;
            _fxSource_Reversed.volume *= volume; 
            _fxSource_Reversed.Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            _musicSource.PlayOneShot(clip);
        }

        public void ChangeMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

    }
}
