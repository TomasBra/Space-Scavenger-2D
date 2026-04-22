using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Sound[] sounds;

    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.playOnAwake = false;

            // Pro loopované zvuky / hudbu si clip klidnì pøednastav
            if (sound.loop)
                source.clip = sound.clip;

            sound.source = source;
        }
    }

    private void Start()
    {
        PlayClip("Theme");
    }

    public bool PlayClip(string clipName)
    {
        return PlayClipAfterDelay(clipName, 0f);
    }

    public bool PlayClipAfterDelay(string clipName, float delay)
    {
        foreach (Sound sound in sounds)
        {
            if (clipName != sound.name)
                continue;

            if (sound.clip == null)
            {
                Debug.LogWarning($"Sound '{clipName}' nemá pøiøazený AudioClip.");
                return false;
            }

            // Hudba / loopované zvuky
            if (sound.loop)
            {
                sound.source.clip = sound.clip;

                if (delay > 0f)
                    sound.source.PlayDelayed(delay);
                else
                    sound.source.Play();
            }
            // Jednorázové SFX
            else
            {
                if (delay > 0f)
                    StartCoroutine(PlayOneShotDelayed(sound, delay));
                else
                    sound.source.PlayOneShot(sound.clip);
            }

            return true;
        }

        Debug.LogWarning($"Sound '{clipName}' nebyl nalezen.");
        return false;
    }

    private System.Collections.IEnumerator PlayOneShotDelayed(Sound sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        sound.source.PlayOneShot(sound.clip);
    }
}