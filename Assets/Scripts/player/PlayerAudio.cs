using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudio : MonoBehaviour
{
    [Header("Clip")] 
    [SerializeField] private AudioClip stepping;
    [SerializeField] private AudioClip falling;

    [Header("Settings")] 
    [SerializeField] private float pitchShift;
    [SerializeField] private AudioSource source;

    public void StepSound()
    {
        PlaySound(stepping);
    }

    public void FallSound()
    {
        PlaySound(falling);
    }

    private void PlaySound(AudioClip clip)
    {
        source.pitch = 1 + Random.Range(-pitchShift, pitchShift);
        source.PlayOneShot(clip);
    }
}
