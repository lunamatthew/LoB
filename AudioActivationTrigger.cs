using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioActivationTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private string triggerTag;
    [SerializeField]
    private AudioClip clipToPlay;
    [SerializeField]
    private float activationDelay = 0f;
    [SerializeField]
    private bool destroyASAfterPlay = false;
    [SerializeField]
    private bool destroyTriggerAfterPlay = false;


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(triggerTag)) {
            audioSource.clip = clipToPlay;
            audioSource.PlayDelayed(activationDelay);
            if (destroyASAfterPlay)
                Destroy(audioSource.gameObject, clipToPlay.length + 1f);
            if (destroyTriggerAfterPlay)
                Destroy(gameObject, clipToPlay.length + 1f);
        }
    }
}
