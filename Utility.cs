using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float STAMINA_MAX = 100.0f;
    public static float HEALTH_MAX = 100.0f;
    public static float POWER_MAX = 100.0f;
    public static float BOBBING_SPEED = 0.18f;
    public static float FOOTSTEP_DELAY = 0.5f;
    public static float VOLUME_WALK = 0.25f;

    // returns int between bounds flr and cel, excluding lst
    public static int GetRandomNonRepeatInt(int cel, int lst, int flr = 0) {
        int freshInt = Random.Range(flr, cel);
        while (freshInt == lst)
            freshInt = Random.Range(flr, cel);
        return freshInt;
    }

    public static bool IsGrounded(GameObject go, float rayLength = 0.75f) {
        RaycastHit hit;

        if (Physics.Raycast(go.transform.position, -Vector3.up, out hit, rayLength)) {
            return true;
        }
        return false;
    }

    
    public static IEnumerator PlayAudioSourceAfterTime(float time, AudioSource audioSource) {
        yield return new WaitForSeconds(time);
        audioSource.Play();
    }

    public static IEnumerator GameObjectToggle(GameObject go, bool enabled, float time) {
        yield return new WaitForSeconds(time);
        go.SetActive(enabled);
    }
    

}
