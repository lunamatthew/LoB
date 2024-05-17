using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType { Flesh, Metal, Concrete, Sand };

public abstract class Projectile : MonoBehaviour {
    public GameObject mesh;
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip impactAudioClip;
    [SerializeField]
    protected AudioClip passByAudioClip;
    [SerializeField]
    protected Collider projectileCollider;

    protected Rigidbody rb;
    

    public abstract void OnFire(float force);
    protected abstract void OnImpact(ImpactType t);
    protected void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    public void ChangeCollision(int collisionOption) {
        switch(collisionOption) {
            case 1:
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                break;
            case 2:
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                break;
            case 3:
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                break;
            default:
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                break;
        }
    }
}
