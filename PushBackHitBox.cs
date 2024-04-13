using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBackHitBox : MonoBehaviour
{
    [SerializeField]
    private float pushForceHorizontal = 10.0f; // Adjust the push force as needed
    [SerializeField]
    private float verticalPushForceMultiplier = 5f; // Multiplier for vertical push force
    [SerializeField]
    private float stunTime = 1.5f;
    [SerializeField]
    private AudioClip[] audioClipsOnCollide;
    [SerializeField]
    private AudioSource audioSource;
    private int lastSoundIndex;
    [SerializeField]
    private float damage = 0f, damageRandModifier = 1f, stamDamage = 0f;

    private void SoundActivate() {
        int soundIndex = Utility.GetRandomNonRepeatInt(audioClipsOnCollide.Length, lastSoundIndex);
        lastSoundIndex = soundIndex;
        audioSource.clip = audioClipsOnCollide[soundIndex];
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) // Assuming player has a "Player" tag
        {
            FPSController fpsController = other.GetComponent<FPSController>();
            if (fpsController != null) {
                Debug.Log("player detected");

                if (audioSource != null)
                    SoundActivate();

                StartCoroutine(fpsController.StunLock(stunTime));

                if (damage > 0) {
                    if (damageRandModifier != 1) {
                        fpsController.getStats().TakeDamage(Random.Range(damage / damageRandModifier, damage));
                    } else
                        fpsController.getStats().TakeDamage(damage);
                }
                fpsController.getStats().TakeStaminaDrain(stamDamage);
                
                Vector3 pushDirection = other.transform.position - transform.position;

                // Apply the push force to the player's Rigidbody
                Vector3 pushForce = new Vector3(pushDirection.x * pushForceHorizontal,
                                                pushDirection.y * verticalPushForceMultiplier,
                                                pushDirection.z * pushForceHorizontal);
                fpsController.GetRigidBody().AddForce(pushForce, ForceMode.Impulse);
            }
        }
    }
}
