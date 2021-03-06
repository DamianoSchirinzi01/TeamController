using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    [Header("BackgroundMusic")]
    public List<soundAudioClip> soundClipList;

    [System.Serializable]
    public class soundAudioClip
    {
        public SoundManager.Sound audioClipName;
        public AudioClip audioClip;
    }
}
