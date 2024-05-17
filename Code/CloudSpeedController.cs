using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

public class CloudSpeedController : MonoBehaviour
{
    [SerializeField]
    private GameObject distanceModifier;
    [SerializeField]
    private GameObject distanceModifierTarget;
    [SerializeField]
    private Volume volume;
    public float speed = 75f; // changed based on distance of distance modifier to distance modifier target
    private VolumetricClouds volumetricClouds;
    private float distance = 0f;

    private void Start() {
        if (volume != null && volume.profile.TryGet(out volumetricClouds)) {
            Debug.Log("Got dynamic cloud component!");
        } else {
            Debug.Log("VolumetricClouds not found in the Volume component.");
        }
    }

    private void FixedUpdate() {
        distance = Vector3.Distance(distanceModifier.transform.position, distanceModifierTarget.transform.position);
        adjustWindSpeed(distance);
    }

    private void adjustWindSpeed(float speed) {
        speed = -speed + 2000.0f;
        volumetricClouds.globalWindSpeed = new WindSpeedParameter(speed, WindParameter.WindOverrideMode.Custom, true);
    }
}
