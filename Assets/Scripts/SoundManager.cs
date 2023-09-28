using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{

    public static void PlaySound(string clip) {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (clip == "jump")
            audioSource.PlayOneShot(GameAssets.GetInstance().birdJump);
        else if (clip == "score")
            audioSource.PlayOneShot(GameAssets.GetInstance().score);
        else if (clip == "lose")
            audioSource.PlayOneShot(GameAssets.GetInstance().lose);
    }
}
