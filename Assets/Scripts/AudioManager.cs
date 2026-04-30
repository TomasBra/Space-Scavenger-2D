using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sound[] sounds;

    [Header("Pool pro jednorázové SFX")]
    [SerializeField]
    private int sfxPoolSize = 12;

    private readonly Dictionary<string, Sound> soundMap = new();
    private readonly List<AudioSource> sfxSources = new();
    private int nextSfxSourceIndex = 0;

    private void Awake()
    {
        CreateSfxPool();
        InitializeSounds();
    }

    private void Start()
    {
        PlayClip("Theme");
    }

    private void CreateSfxPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            sfxSources.Add(source);
        }
    }

    private void InitializeSounds()
    {
        soundMap.Clear();

        foreach (Sound sound in sounds)
        {
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

            if (sound.loop)
            {
                AudioSource loopSource = gameObject.AddComponent<AudioSource>();
                loopSource.playOnAwake = sound.playOnAwake;
                loopSource.loop = sound.loop;
                loopSource.volume = sound.volume;
                loopSource.pitch = sound.pitch;

                AudioClip clip = sound.GetRandomClip();
                if (clip != null)
                    loopSource.clip = clip;

                sound.source = loopSource;
            }

            soundMap.Add(sound.name, sound);
        }
    }

    public bool PlayClip(string clipName)
    {
        return PlayClipAfterDelay(clipName, 0f);
    }

    public bool PlayClipAfterDelay(string clipName, float delay)
    {
        if (!soundMap.TryGetValue(clipName, out Sound sound))
        {
            Debug.LogWarning($"Sound '{clipName}' nebyl nalezen.");
            return false;
        }

        if (!sound.HasAnyClip())
        {
            Debug.LogWarning($"Sound '{clipName}' nemá pøiøazený žádný AudioClip.");
            return false;
        }

        if (sound.loop)
        {
            if (delay > 0f)
                StartCoroutine(PlayLoopDelayed(sound, delay));
            else
                PlayLoop(sound);

            return true;
        }

        if (delay > 0f)
            StartCoroutine(PlaySfxDelayed(sound, delay));
        else
            PlaySfx(sound);

        return true;
    }

    public bool StopClip(string clipName)
    {
        if (!soundMap.TryGetValue(clipName, out Sound sound))
        {
            Debug.LogWarning($"Sound '{clipName}' nebyl nalezen.");
            return false;
        }

        if (!sound.loop || sound.source == null)
            return false;

        sound.source.Stop();
        return true;
    }

    public bool IsPlaying(string clipName)
    {
        if (!soundMap.TryGetValue(clipName, out Sound sound))
            return false;

        if (sound.loop && sound.source != null)
            return sound.source.isPlaying;

        return false;
    }

    private void PlayLoop(Sound sound)
    {
        if (sound.source == null)
            return;

        AudioClip clip = sound.GetRandomClip();
        if (clip == null)
            return;

        sound.source.clip = clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;

        if (!sound.source.isPlaying)
            sound.source.Play();
    }

    private void PlaySfx(Sound sound)
    {
        if (sound.maxSimultaneousCount > 0 && CountPlayingSourcesFor(sound.name) >= sound.maxSimultaneousCount)
            return;

        AudioClip clip = sound.GetRandomClip();
        if (clip == null)
            return;

        AudioSource source = GetFreeSfxSource();

        source.clip = null;
        source.loop = false;
        source.outputAudioMixerGroup = sound.outputMixerGroup;

        float randomizedVolume = sound.volume * Random.Range(sound.randomVolumeMin, sound.randomVolumeMax);
        float randomizedPitch = sound.pitch * Random.Range(sound.randomPitchMin, sound.randomPitchMax);

        source.volume = Mathf.Clamp01(randomizedVolume);
        source.pitch = Mathf.Clamp(randomizedPitch, -3f, 3f);

        source.PlayOneShot(clip);

        StartCoroutine(ClearSourceTagAfterPlayback(source, clip.length / Mathf.Max(0.01f, Mathf.Abs(source.pitch))));
        source.gameObject.name = $"SFX_{sound.name}";
    }

    private AudioSource GetFreeSfxSource()
    {
        for (int i = 0; i < sfxSources.Count; i++)
        {
            if (!sfxSources[i].isPlaying)
                return sfxSources[i];
        }

        AudioSource fallback = sfxSources[nextSfxSourceIndex];
        nextSfxSourceIndex = (nextSfxSourceIndex + 1) % sfxSources.Count;
        return fallback;
    }

    private int CountPlayingSourcesFor(string soundName)
    {
        int count = 0;
        string expectedName = $"SFX_{soundName}";

        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying && source.gameObject.name == expectedName)
                count++;
        }

        return count;
    }

    private IEnumerator PlaySfxDelayed(Sound sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySfx(sound);
    }

    private IEnumerator PlayLoopDelayed(Sound sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayLoop(sound);
    }

    private IEnumerator ClearSourceTagAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (!source.isPlaying)
            source.gameObject.name = "PooledAudioSource";
    }
}