using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSFX : MonoBehaviour
{
    public string sfxName;
    [SerializeField] private AudioClip[] clips;
    
    public AudioClip GetRandomClip()
    {
        // protected against null elements
        AudioClip clip;
        do
        {
            clip = clips[Random.Range(0, clips.Length)];
        } while (clip == null);
        return clip;
    }
}
