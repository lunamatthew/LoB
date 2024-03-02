using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    //private float baseDamage = 25.0f;
    [SerializeField]
    private TrailRenderer tr;

    private new void Awake() {
        base.Awake();
        if (tr == null)
            tr = GetComponent<TrailRenderer>();
    }

    public override void OnFire(float force) {
        //throw new System.NotImplementedException();
        if (!mesh.activeSelf)
            mesh.SetActive(true);
        if (tr != null)
            tr.emitting = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        projectileCollider.enabled = true;
        ChangeCollision(1);
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    protected override void OnImpact(ImpactType t) {
        //throw new System.NotImplementedException();
        switch (t) {
            case ImpactType.Flesh:
                break;
            case ImpactType.Concrete:
                break;
            case ImpactType.Metal:
                break;
            case ImpactType.Sand:
                break;
            default:
                break;
        }
        if (tr != null)
            tr.emitting = false;
    }

    protected void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("HitBox")) {
            Debug.Log("Arrow hit a hitbox belonging to: " + collision.gameObject.name);
            OnImpact(ImpactType.Flesh);
        } else {
            Debug.Log("Arrow hit " + collision.gameObject.name);
            OnImpact(ImpactType.Sand);
        }
        ChangeCollision(-1);
    }
}
