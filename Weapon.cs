using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    protected AudioSource weaponAudioSource;
    [SerializeField]
    protected AudioClip[] weaponAudioClips;
    [SerializeField]
    private float baseDamage;
    protected int lastSoundIndex;

    public abstract void OnEquip();
    public abstract void OnUse();
    public abstract void onStopUse();

    public float GetWeaponDamage() {
        return baseDamage;
    }
}
