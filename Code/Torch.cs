using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Holdable {
    [SerializeField]
    private BoxCollider torchHitBox;
    [SerializeField]
    private GameObject torchVFX;
    private bool isLit, isPlayingSound = false;
    [SerializeField]
    private AudioSource torchFireAudioSource;

    [SerializeField]
    private BoxCollider fireCollider;

    private void Start() {
        fireCollider.enabled = false;
        type = HoldableType.Torch;
        isLit = true;
    }

    public override void OnEquip() {
        //throw new System.NotImplementedException();
        fireCollider.enabled = true;
        audioSource.clip = onUseAudioClip;
        audioSource.PlayDelayed(1.0f);
    }

    public override void onStopUse() {
        //throw new System.NotImplementedException();
        fireCollider.enabled = false;
        audioSource.clip = onStopUseAudioClip;
        audioSource.PlayDelayed(1.0f);
        torchHitBox.enabled = false;
    }

    public override void OnUse() {
        //throw new System.NotImplementedException();
        lastSoundIndex = Utility.GetRandomNonRepeatInt(audioClipsActivating.Length, lastSoundIndex);
        //audioSource.clip = audioClipsActivating[lastSoundIndex];
        //audioSource.Play();
        torchFireAudioSource.PlayOneShot(audioClipsActivating[lastSoundIndex]);
        //AudioSource.PlayClipAtPoint(audioClipsActivating[lastSoundIndex], transform.position);
        torchHitBox.enabled = true;
    }

    public void ActivateTorch() {
        isLit = true;
    }

    private IEnumerator TorchSound() {
        isPlayingSound = true;
        OnUse();
        yield return new WaitForSeconds(0.15f);
        isPlayingSound = false;
    }

    private void FixedUpdate() {
        if (mesh.activeSelf && isLit && !isPlayingSound) {
            StartCoroutine(TorchSound());
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        WebBarrier barrier = other.gameObject.GetComponent<WebBarrier>();
        if (barrier != null) {
            barrier.CatchFire();
        }
    }
}
