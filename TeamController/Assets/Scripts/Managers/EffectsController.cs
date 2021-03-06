using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class EffectsController : MonoBehaviour
{
    public static EffectsController instance;
    public Volume usingAlternateVolume;

    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private List<AudioClip> combatAudioClips;

    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
    }

    public void startParticles(ParticleSystem _particles)
    {
        _particles.Play();
    }      
       
    public void stopParticles(ParticleSystem _particles)
    {
        _particles.Stop();
    }
    public void setAlternateFXvolume(float weight)
    {
        usingAlternateVolume.weight = weight;
    }
}
