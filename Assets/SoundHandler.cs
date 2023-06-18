using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip SensibleBuzzer;
    public List<AudioClip> Clips;

    private int clipIndex = 0;

    private void Start()
    {
        ShuffleClips();
    }

    public void PlayNextRandomClip()
    {
        Source.clip = Clips[clipIndex];
        Source.Play();

        clipIndex++;
        if (clipIndex >= Clips.Count)
        {
            ShuffleClips();
            clipIndex = 0;
        }
    }

    public void PlaySensibleBuzzer()
    {
        Source.clip = SensibleBuzzer;
        Source.Play();
    }

    private void ShuffleClips()
    {
        Clips.Sort((a, b) => Random.Range(-1, 2));
    }
}
