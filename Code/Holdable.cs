using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldableType { Shield, Torch, Weapon }

public abstract class Holdable : MonoBehaviour
{
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip[] audioClipsActivating;
    [SerializeField]
    protected AudioClip onUseAudioClip;
    [SerializeField]
    protected AudioClip onStopUseAudioClip;

    public HoldableType type;

    public GameObject mesh;

    protected int lastSoundIndex;

    public abstract void OnEquip();
    public abstract void OnUse();
    public abstract void onStopUse();
}
