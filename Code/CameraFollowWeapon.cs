using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSword : MonoBehaviour {
    public Transform swordTransform;
    public float followSpeed = 10.0f;

    private IEnumerator FollowSword() {
        transform.rotation = Quaternion.Lerp(transform.rotation, swordTransform.rotation, Time.deltaTime * followSpeed);
        yield return null;
    }

    private void Update() {
        if (swordTransform != null) {
            // Lerp camera position and rotation towards the sword
            //transform.position = Vector3.Lerp(transform.position, swordTransform.position, Time.deltaTime * followSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, swordTransform.rotation, Time.deltaTime * followSpeed);
        }
    }
}