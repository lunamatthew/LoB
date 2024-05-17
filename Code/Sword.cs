using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {
    [SerializeField]
    private BoxCollider swordHitBox;
    [SerializeField]
    private AudioClip[] audioClipsOnCollide;
    [SerializeField]
    private AudioClip[] audioClipsOnCollideNoFlesh;
    private int lastImpactSoundIndex;
    [SerializeField]
    private GameObject sparksPS;


    public override void OnEquip() {
        //throw new System.NotImplementedException();
        audioSource.clip = onUseAudioClip;
        audioSource.PlayDelayed(1.2f);
    }

    public override void OnUse() {
        //throw new System.NotImplementedException();
        swordHitBox.enabled = true;
        int soundIndex = Utility.GetRandomNonRepeatInt(audioClipsActivating.Length, lastImpactSoundIndex);
        lastImpactSoundIndex = soundIndex;
        audioSource.clip = audioClipsActivating[soundIndex];
        audioSource.Play();
        StartCoroutine(WaitTurnOffHitBox());
    }

    private IEnumerator WaitTurnOffHitBox(float secondsToWait = 0.75f) {
        yield return new WaitForSeconds(secondsToWait);
        swordHitBox.enabled = false;
    }

    public override void onStopUse() {
        //throw new System.NotImplementedException();
        audioSource.clip = onStopUseAudioClip;
        audioSource.PlayDelayed(1.2f);
        swordHitBox.enabled = false;
    }

    private void SoundActivate(bool isFlesh) {
        if (isFlesh) {
            int soundIndex = Utility.GetRandomNonRepeatInt(audioClipsOnCollide.Length, lastImpactSoundIndex);
            lastImpactSoundIndex = soundIndex;
            audioSource.clip = audioClipsOnCollide[soundIndex];
        } else {
            int soundIndex = Utility.GetRandomNonRepeatInt(audioClipsOnCollideNoFlesh.Length, lastImpactSoundIndex);
            lastImpactSoundIndex = soundIndex;
            audioSource.clip = audioClipsOnCollideNoFlesh[soundIndex];
        }
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other) {
        swordHitBox.enabled = false;
        Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position);

        if (other.CompareTag("NPC")) {
            Debug.Log("NPC detected on sword hit!");
            SoundActivate(true);
            NonPlayableCharacter npc = other.GetComponent<NonPlayableCharacter>();
            if (npc != null) {
                npc.GetStats().TakeDamage(GetWeaponDamage());
                npc.CheckDeath();
            }
        } else {
            Debug.Log(name + " detected collision with " + other.name);
            SoundActivate(false);
            GameObject particleEffect = Instantiate(sparksPS, collisionPoint, Quaternion.identity);
            Destroy(particleEffect, 3f);
        }
    }
}
