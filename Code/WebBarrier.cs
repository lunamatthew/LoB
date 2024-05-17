using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebBarrier : MonoBehaviour
{
    [SerializeField]
    private GameObject fireEffect;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private GameObject webMesh;
    [SerializeField]
    private Collider colliderBarrier;
    private bool hasCaughtFire = false;

    public void CatchFire() {
        if (!hasCaughtFire) {
            //Instantiate(fireEffect, transform.position, Quaternion.identity);
            hasCaughtFire = true;
            fireEffect.SetActive(true);
            StartCoroutine(DestroyAfterDelay(4.0f, webMesh));
            colliderBarrier.enabled = false;
            StartCoroutine(DestroyAfterDelay(8.0f, gameObject));
        }
    }

    private IEnumerator DestroyAfterDelay(float time, GameObject go) {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }
}
