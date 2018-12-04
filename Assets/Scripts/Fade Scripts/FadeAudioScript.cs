using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudioScript : MonoBehaviour {

    public float durationInSeconds;

    private AudioSource audioSource;

    private float originalVolume;

    private float targetVolume;

    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log(gameObject.ToString() + ": No Audio Source found");
        } 
    }

    public void FadeToVolume(float volume)
    {
        FadeToVolume(volume, durationInSeconds);
    }

    public void FadeToVolume(float volume, float duration)
    {
        originalVolume = audioSource.volume;
        targetVolume = volume;
        StartCoroutine("DoFade", duration);
    }

    IEnumerator DoFade(float duration)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime / duration;
            audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, progress);
            yield return null;
        }
        yield return null;
    }
}
