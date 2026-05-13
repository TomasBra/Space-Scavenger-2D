using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : GameObject2D
{
    [SerializeField]
    private Sound[] sounds;

    private readonly Dictionary<string, Sound> soundMap = new();

    public Sound theme;
    public AudioSource themeAudiosource;

    private void Awake()
    {
        InitializeSounds();
    }

    private void Start()
    {
        base.Start();
        PlayTheme();
    }

    private void PlayTheme()
    {
        themeAudiosource.playOnAwake = false;
        themeAudiosource.volume = theme.volume;
        themeAudiosource.pitch = theme.pitch;
        themeAudiosource.clip = theme.GetRandomClip();
        themeAudiosource.loop = true;

        theme.source = themeAudiosource;
        theme.source.PlayDelayed(theme.playOffset);
    }

    private void InitializeSounds()
    {
        soundMap.Clear();

        foreach (Sound sound in sounds)
        {
            if (sound == null)
                continue;

            if (string.IsNullOrWhiteSpace(sound.name))
            {
                Debug.LogWarning("Narazil jsem na Sound bez jména.");
                continue;
            }

            if (soundMap.ContainsKey(sound.name))
            {
                Debug.LogWarning($"Duplicitní Sound name: '{sound.name}'. Pøeskakuji.");
                continue;
            }

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = sound.loop;
            source.volume = sound.volume;
            source.pitch = sound.pitch;

            sound.source = source;

            soundMap.Add(sound.name, sound);
        }
    }

    public bool PlayClip(string clipName)
    {
        if (!soundMap.TryGetValue(clipName, out Sound sound))
        {
            Debug.LogWarning($"Sound '{clipName}' nebyl nalezen.");
            return false;
        }

        if (!sound.HasAnyClip())
        {
            Debug.LogWarning($"Sound '{clipName}' nemá žádný AudioClip.");
            return false;
        }

        AudioClip clip = sound.GetRandomClip();
        if (clip == null)
            return false;

        sound.source.clip = clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;

        sound.source.Play();

        return true;
    }

    public bool StopClip(string clipName)
    {
        if (!soundMap.TryGetValue(clipName, out Sound sound))
        {
            Debug.LogWarning($"Sound '{clipName}' nebyl nalezen.");
            return false;
        }

        if (sound.source == null)
            return false;

        sound.source.Stop();
        return true;
    }

    public bool IsPlaying(string clipName)
    {
        if (!soundMap.TryGetValue(clipName, out Sound sound))
            return false;

        if (sound.source == null)
            return false;

        return sound.source.isPlaying;
    }
}