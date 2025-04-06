using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HitAudioManager : MonoBehaviour
{
    public static HitAudioManager Instance;
    public AudioSource source;
    public List<AudioResource> audioClips = new List<AudioResource>();

    void Awake()
    {
        Instance = this;
    }

    public void PlayAudio(float magnitude)
    {
        var id = Mathf.RoundToInt(Mathf.Min(magnitude, audioClips.Count - 1));
        source.resource = audioClips[id];
        source.volume = Mathf.Min(1f, magnitude);
        source.Play();
    }
}
