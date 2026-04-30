using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    [Header("Clips")]
    public AudioClip[] clips;

    [Header("Základní nastavení")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(-3f, 3f)]
    public float pitch = 1f;

    public bool loop = false;

    public bool playOnAwake = false;

    [Header("Náhodná variace pro SFX")]
    [Range(0.5f, 1.5f)]
    public float randomPitchMin = 0.95f;

    [Range(0.5f, 1.5f)]
    public float randomPitchMax = 1.05f;

    [Range(0f, 1.5f)]
    public float randomVolumeMin = 0.95f;

    [Range(0f, 1.5f)]
    public float randomVolumeMax = 1.0f;

    [Header("Omezení souèasného pøehrávání")]
    public int maxSimultaneousCount = 3;

    [Header("Mixer")]
    public AudioMixerGroup outputMixerGroup;

    [HideInInspector]
    public AudioSource source;

    public bool HasAnyClip()
    {
        return clips != null && clips.Length > 0 && GetRandomClip() != null;
    }

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        int safety = 0;

        while (safety < clips.Length)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
                return clip;

            safety++;
        }

        return null;
    }
}