using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType { Sand, Grass, Stone, Metal, Water }

public class GroundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject sandImpact, grassImpact, stoneImpact, metalImpact, waterImpact;
    public AudioClip[] sandFootStepAudioClips, grassFootStepAudioClips, stoneFootStepAudioClips, metalFootStepAudioClips, waterFootStepAudioClips;
    public AudioClip[] sandImpactAudioClips;
    [SerializeField]
    private AudioSource audioSource;

    public static GroundManager Singleton { get; private set; }

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Debug.LogWarning("Duplicate GroundManager detected, destroying duplicate!");
            Destroy(gameObject);
        }
    }

    public void GroundImpact(GroundType gt, Vector3 location, Quaternion rotation) {
        Debug.Log("Ground Impact!");
        GameObject impactGO;
        switch (gt) {
            case GroundType.Grass:
                impactGO = Instantiate(grassImpact, location, rotation);
                break;
            case GroundType.Stone:
                impactGO = Instantiate(stoneImpact, location, rotation);
                break;
            case GroundType.Metal:
                impactGO = Instantiate(metalImpact, location, rotation);
                break;
            case GroundType.Water:
                impactGO = Instantiate(waterImpact, location, rotation);
                break;
            default:
                impactGO = Instantiate(sandImpact, location, rotation);
                break;
        }
        AudioSource instancedAudioSource = Instantiate(audioSource, impactGO.transform);
        instancedAudioSource.transform.position = impactGO.transform.position;
        instancedAudioSource.clip = sandImpactAudioClips[Random.Range(0, sandImpactAudioClips.Length)];
        instancedAudioSource.Play();
        Destroy(impactGO, 7f);
    }

}
