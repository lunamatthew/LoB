using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {
    [SerializeField]
    private BoxCollider swordHitBox;

    public override void OnEquip() {
        throw new System.NotImplementedException();
    }

    public override void OnUse() {
        //throw new System.NotImplementedException();
        swordHitBox.enabled = true;
        int soundIndex = Utility.GetRandomNonRepeatInt(weaponAudioClips.Length, lastSoundIndex);
        lastSoundIndex = soundIndex;
        weaponAudioSource.clip = weaponAudioClips[soundIndex];
        weaponAudioSource.Play();
    }

    public override void onStopUse() {
        //throw new System.NotImplementedException();
        swordHitBox.enabled = false;
    }
}
