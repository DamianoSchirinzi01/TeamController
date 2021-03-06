﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Handles all sounds throught game.  
    public enum Sound
    {
        Footsteps,

        Jump,

        SwordSwipe_1,
        SwordSwipe_2,
        SwordSwipe_3,

        SwordImpact,
        SheildImpact,

        BowReady,
        BowRelease,
        BowImpactSoft,
        BowImpactHard,

        MagicExplosion,
        ArcLightning

    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>(); //Add sound that MUST NOT PLAY REPEATEDLY during an update
        soundTimerDictionary[Sound.Footsteps] = 0f;
    }

    public static void Play2DSound(Sound clipToPlay, float destroyDelayTime, float _volume)
    {
        if (canPlaySound(clipToPlay))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                oneShotAudioSource.volume = _volume;
            }

            oneShotAudioSource.PlayOneShot(GetAudioClip(clipToPlay));
        }
    }

    public static AudioSource Play3DSound(Sound clipToPlay, bool shouldLoop, bool shouldDestroy, float destroyDelayTime, float _volume, float _range, Vector3 position)
    {
        if (canPlaySound(clipToPlay))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.volume = _volume;
            audioSource.panStereo = 0;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
            audioSource.maxDistance = _range;
            audioSource.rolloffMode = AudioRolloffMode.Linear;

            if (shouldLoop)
            {
                audioSource.loop = true;
            }

            audioSource.clip = GetAudioClip(clipToPlay);
            audioSource.Play();

            if (shouldDestroy)
            {
                Object.Destroy(soundGameObject, audioSource.clip.length + .5f);
            }

            return audioSource;
        }

        return null;
    }

    public static void PlayStaticSound(AudioSource _mainAudioSource, Sound _clipToPlay)
    {
        _mainAudioSource.clip = GetAudioClip(_clipToPlay);
        if (_mainAudioSource.isPlaying == false)
        {
            _mainAudioSource.Play();
        }
    }


    public static void DestroySound(GameObject _soundToDestroy)
    {
        Object.Destroy(_soundToDestroy, .5f);
    }

    private static AudioClip GetAudioClip(Sound clipToFind)
    {
        foreach (GameAssets.soundAudioClip soundAudioClip in GameAssets.instance.soundClipList)
        {
            if (soundAudioClip.audioClipName == clipToFind)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + clipToFind + " not found!");
        return null;
    }

    private static bool canPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true; //For most sounds, this will return true because they will not be called during an Update Method()
            case Sound.Footsteps:
                if (soundTimerDictionary.ContainsKey(sound)) //But for some like footsteps which are
                {
                    float lastTimePlayed = soundTimerDictionary[sound]; //This will check a dictionary to see if it has a key for this sound
                    float playerMoveTimerMax = .5f; //Define a delay between playing the sound
                    if (lastTimePlayed + playerMoveTimerMax < Time.time) //Test if it is time to play
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true; //If it is, we can play
                    }
                    else
                    {
                        return false; //If it is not, we return false
                    }
                }
                else //if the dictionary does not contain the key, return true
                {
                    return true;
                }
                // break;
        }
    }
}
