using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Holdable {
    [SerializeField]
    private BoxCollider shieldHitBox;

    [SerializeField]
    private AudioClip shieldRetractClip;

    private void Start() {
        type = HoldableType.Shield;
    }

    public override void OnEquip() {
        //throw new System.NotImplementedException();
        audioSource.clip = onUseAudioClip;
        audioSource.PlayDelayed(1.0f);
    }

    public override void onStopUse() {
        //throw new System.NotImplementedException();
        audioSource.clip = onStopUseAudioClip;
        audioSource.PlayDelayed(1.0f);
        shieldHitBox.enabled = false;
    }

    public override void OnUse() {
        //Debug.Log("OnUse!()");
        //throw new System.NotImplementedException();
        lastSoundIndex = Utility.GetRandomNonRepeatInt(audioClipsActivating.Length, lastSoundIndex);
        audioSource.clip = audioClipsActivating[lastSoundIndex];
        audioSource.Play();
        shieldHitBox.enabled = true;
    }

    public void retractShield() {
        //Debug.Log("Retracting shield!");
        shieldHitBox.enabled = false;
        audioSource.clip = shieldRetractClip;
        audioSource.Play();
    }
}
