using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {
    [SerializeField]
    private BoxCollider swordHitBox;

    public override void OnEquip() {
        //throw new System.NotImplementedException();
        audioSource.clip = onUseAudioClip;
        audioSource.PlayDelayed(1.2f);
    }

    public override void OnUse() {
        //throw new System.NotImplementedException();
        swordHitBox.enabled = true;
        int soundIndex = Utility.GetRandomNonRepeatInt(audioClipsActivating.Length, lastSoundIndex);
        lastSoundIndex = soundIndex;
        audioSource.clip = audioClipsActivating[soundIndex];
        audioSource.Play();
    }

    public override void onStopUse() {
        //throw new System.NotImplementedException();
        audioSource.clip = onStopUseAudioClip;
        audioSource.PlayDelayed(1.2f);
        swordHitBox.enabled = false;
    }
}
