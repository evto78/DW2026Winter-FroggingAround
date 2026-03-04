using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public AudioSource source;
        public List<AudioClip> clips;
    }

    public List<SoundEffect> soundEffects;

    public void PlaySoundByKey(int key)
    {
        SoundEffect sfx = soundEffects[key];
        sfx.source.clip = sfx.clips[Random.Range(0, sfx.clips.Count)];
        sfx.source.PlayOneShot(sfx.clips[Random.Range(0, sfx.clips.Count)]);
    }
}
