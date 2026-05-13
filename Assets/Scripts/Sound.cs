using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    [Header("Clips")]
    public AudioClip[] clips;

    [Header("Základní nastavení")]
    [Range(0f, 2f)]
    public float volume = 1f;

    [Range(-3f, 3f)]
    public float pitch = 1f;

    public float playOffset = 0;
    public bool loop = false;

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