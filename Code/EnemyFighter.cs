using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFighter : NonPlayableCharacter {
    [SerializeField]
    private Transform player; // Reference to the player's transform
    [SerializeField]
    private float attackDistance = 2.25f; // Distance at which the enemy will start attacking
    [SerializeField]
    private float movementSpeed = 3f; // Speed at which the enemy moves towards the player
    [SerializeField]
    private bool isAIEnabled = true;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float visionDistance = 100f; // Distance at which the enemy will start walking towards the player
    private bool isAttacking = false; //isInAttackRange = false;
    private string attackString = "Attack";
    private int attackAnimFlag = 1;
    private bool isMoving = false;
    private float footStepTimer = 0f;
    [SerializeField]
    private float footstepDelay = 0.2f;
    [SerializeField]
    private AudioSource footStepAS;
    [SerializeField]
    private AudioClip[] footStepClips;
    private int lastFootStepIndex, newFootStepIndex;
    [SerializeField]
    private AudioSource weaponAS;
    [SerializeField]
    private AudioClip[] weaponAudioClips;
    private int lastWeaponSound = -1;
    private bool isJumping = false, isBlocking = false;
    [SerializeField]
    private Collider shieldCollider;
    private bool isRecovering = false;
    private Rigidbody rb;
    [SerializeField]
    private LayerMask layerMaskForObstacles;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    private float jumpHeight, jumpDistance, jumpDelay;
    [SerializeField]
    private float sprintDistance = 25.0f;
    [SerializeField]
    float sprintChance = 0.0005f;
    [SerializeField]
    float walkSpeed = 3f;
    private bool isSprinting = false;
    private float sprintingFootstepDelay, stepDelay;
    [SerializeField]
    private AudioSource voiceAudioSource;
    [SerializeField]
    private AudioClip[] voiceLineClips;

    [SerializeField]
    private Rigidbody[] ragdollLimbsRb;
    [SerializeField]
    private Collider[] ragdollColliders;

    public bool isAlive = true;
    
    /*
    [SerializeField]
    private float closeDistanceThreshold = 15.0f;
    private bool isSideStepping = false;
    private float sideStepTimer = 0.0f;
    [SerializeField]
    private float sideStepDuration = 10.0f;
    [SerializeField]
    private float sideStepSpeed = 3f;
    [SerializeField]
    private Vector3 sideStepDirection;
    */


    public override void OnDeath() {
        //throw new System.NotImplementedException();
        isAlive = false;
        isAIEnabled = false;
        isMoving = false;
        isAttacking = false;
        isSprinting = false;
        animator.enabled = false;
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;

        foreach (Rigidbody rb in ragdollLimbsRb) {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        foreach (Collider col in ragdollColliders) {
            col.enabled = true;
        }
    }

    private IEnumerator AttackBase(float time) {
        voiceAudioSource.clip = voiceLineClips[Random.Range(0, 6)];
        voiceAudioSource.Play();
        attackAnimFlag = Utility.GetRandomNonRepeatInt(5, attackAnimFlag, 1);
        animator.SetInteger(attackString, attackAnimFlag);
        PlayWeaponSound();
        yield return new WaitForSeconds(time);
        animator.SetInteger(attackString, 0);
        isAttacking = false;
    }

    private void PlayWeaponSound() {
        lastWeaponSound = Utility.GetRandomNonRepeatInt(weaponAudioClips.Length, lastWeaponSound); ;
        weaponAS.clip = weaponAudioClips[lastWeaponSound];
        weaponAS.Play();
    }

    private void Awake() {
        player = Camera.main.GetComponent<Transform>();
        if (player == null)
            isAIEnabled = false;
        rb = GetComponent<Rigidbody>();
        stepDelay = footstepDelay;
        sprintingFootstepDelay = footstepDelay / 1.5f;
    }

    private void Start() {
        npcStats = GetComponent<Stats>();
        npcStats.InitializeStats();

        if (isAIEnabled)
            StartCoroutine(RandomizeShield());
    }

    private void PlayFootStep() {
        //Debug.Log("PlayingFootStep!");
        newFootStepIndex = Utility.GetRandomNonRepeatInt(footStepClips.Length, lastFootStepIndex);
        lastFootStepIndex = newFootStepIndex;
        footStepAS.clip = footStepClips[newFootStepIndex];
        footStepAS.Play();
    }

    private IEnumerator JumpInitiate(float delayActionTime, int jumpFlag, float jumpDuration = 1.25f, float jumpHeight = 2.5f, float jumpDistance = 10.0f, bool preciseLanding = false) {
        voiceAudioSource.clip = voiceLineClips[Random.Range(7, voiceLineClips.Length)];
        voiceAudioSource.PlayDelayed(0.25f);
        animator.SetLayerWeight(1, 0f);
        animator.SetLayerWeight(2, 0f);
        animator.SetLayerWeight(3, 1.0f);
        animator.SetInteger("Jump", jumpFlag);
        yield return new WaitForSeconds(delayActionTime);
        StartCoroutine(JumpAction(jumpFlag, jumpDuration, jumpHeight, jumpDistance, preciseLanding));
    }
    /*
    private IEnumerator RandomizeShield() {
        while (true) {
            float randomDelay = Random.Range(5.0f, 35.0f); // Adjust the delay as needed
            yield return new WaitForSeconds(randomDelay);

            if (isAIEnabled && !isJumping) {
                isBlocking = true;
                animator.SetLayerWeight(4, 1.0f);
                randomDelay = Random.Range(2.0f, 6f);

                animator.SetInteger("Shield", 1);
                yield return new WaitForSeconds(randomDelay);
                animator.SetLayerWeight(4, 0.0f);
                yield return new WaitForSeconds(0.25f);
                animator.SetInteger("Shield", 0);
                isBlocking = false;
            }
        }
    }
    */

    private IEnumerator RandomizeShield() {
        while (true) {
            float randomDelay = Random.Range(5.0f, 35.0f);
            yield return new WaitForSeconds(randomDelay);

            if (isAIEnabled && !isJumping) {
                isBlocking = true;
                animator.SetLayerWeight(4, 1.0f);
                randomDelay = Random.Range(2.0f, 6f);

                animator.SetInteger("Shield", 1);
                yield return new WaitForSeconds(randomDelay);
                animator.SetInteger("Shield", 0);
                yield return new WaitForSeconds(1.0f);
                animator.SetLayerWeight(4, 0.0f);
                isBlocking = false;
            }
        } 
    }

    private IEnumerator DetermineShieldSlam() {
        float randomAction = Random.value;
        if (randomAction <= 0.25f) {
            //Debug.Log("Shield Smash!");
            voiceAudioSource.clip = voiceLineClips[Random.Range(0, voiceLineClips.Length)];
            voiceAudioSource.Play();
            animator.SetInteger("Shield", 2);
            shieldCollider.enabled = true;
            yield return new WaitForSeconds(1.5f);
            shieldCollider.enabled = false;
            animator.SetInteger("Shield", 0);
        }
    }

    private IEnumerator JumpAction(int jumpFlag, float jumpDuration, float jumpHeight, float jumpDistance, bool preciseLanding = false) {
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position;

        if (jumpFlag == 0) {
            targetPos -= transform.forward * jumpDistance;
        } else if (jumpFlag >= 1) {
            targetPos += transform.forward * jumpDistance;
        }

        for (float t = 0; t < jumpDuration; t += Time.deltaTime) {
            float normalizedTime = t / jumpDuration;
            float curveValue = Mathf.Sin(normalizedTime * Mathf.PI);

            Vector3 newPosition;

            if (preciseLanding && curveValue >= Mathf.Abs((transform.position.y - targetPos.y) / jumpHeight)) {
                newPosition = Vector3.Lerp(startPos, targetPos, normalizedTime) + Vector3.up * (curveValue * 0.8f) * jumpHeight;

            } else {
                newPosition = Vector3.Lerp(startPos, targetPos, normalizedTime) + Vector3.up * curveValue * jumpHeight;
            }

            transform.position = newPosition;
            yield return null;
        }
        voiceAudioSource.clip = voiceLineClips[Random.Range(0, 6)];
        animator.SetInteger("Jump", -1);
        animator.SetLayerWeight(1, 1.0f);
        animator.SetLayerWeight(2, 1.0f);
        animator.SetLayerWeight(3, 0f);
        transform.position = targetPos; // Ensure final position is exact
        voiceAudioSource.Play();
        isJumping = false;
        footStepAS.clip = GroundManager.Singleton.sandImpactAudioClips[Random.Range(0, GroundManager.Singleton.sandImpactAudioClips.Length)]; // eventually should account for other impact types
        footStepAS.Play();
        //Debug.Log("Ending jump!");
    }

    private void HandleFreeFall() {
        float verticalVelocity = Mathf.Abs(rb.velocity.y);
        if (verticalVelocity >= 30.0f) {
            //Debug.Log("Handling free fall");
            //rb.drag = 0f;
            isAIEnabled = false;
            isRecovering = false;
            animator.SetBool("isFalling", true);
            animator.Play("Fall", 3);
            animator.SetLayerWeight(3, 1);
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
        } else if (verticalVelocity < 5.0f && !isRecovering) {
            GroundManager.Singleton.GroundImpact(GroundType.Sand, transform.position, transform.rotation);
            //rb.drag = 50.0f;
            isRecovering = true;
            StartCoroutine(RecoverFromPain());
        }      
    }

    private IEnumerator RecoverFromPain() {
        //Debug.Log("Recovering!");
        animator.SetBool("isFalling", false);
        yield return new WaitForSeconds(11.5f);
        //Debug.Log("Turning Jump Layer off");
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 1);
        animator.SetLayerWeight(3, 0);
        isAIEnabled = true;
    }

    private bool CheckObstacleAhead() {
        Debug.Log("Checking For Obstacles!");
        Vector3 lowTransformOrigin = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        if (Physics.Raycast(lowTransformOrigin, transform.forward, out RaycastHit hit, 3f, layerMaskForObstacles)) {
            Debug.Log("Obstacles! " + hit.ToString());
            return true;
        }
        //Debug.Log(hit.transform.name);
        return false;
    }

    /*
    private void OnDrawGizmos() {
        Vector3 lowTransformOrigin = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        Gizmos.DrawLine(lowTransformOrigin, player.position);
    }
    */

    // Older fixedupdate() is found below this fixedupdate() before the implementing of side stepping
    // Currently not working as intended... needs work


    // Working version of fixedupdate() below, without implementing side stepping yet. 
    
    private void FixedUpdate() {
            HandleFreeFall();
        if (isAIEnabled) {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isMoving && Utility.IsGrounded(gameObject, 1.5f)) {
                footStepTimer += Time.fixedDeltaTime;
                if (footStepTimer >= stepDelay) {
                    PlayFootStep();
                    footStepTimer = 0f;
                }
            } else {
                footStepTimer = 0f;
            }

            if (distanceToPlayer <= visionDistance && !isJumping) {
                float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0f; // Keep the enemy's y-axis fixed
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                if (!isAttacking && distanceToPlayer > attackDistance) {
                    //Debug.Log("Starting to walk towards the player.");
                    isMoving = true;
                    if (!isSprinting) {
                        if (distanceToPlayer >= sprintDistance && Random.value <= sprintChance) {
                            isSprinting = true;
                            stepDelay = sprintingFootstepDelay;
                            animator.SetInteger("Walk", 4);
                            //Debug.Log("Sprinting!");
                            movementSpeed = walkSpeed * 4;
                        } else {
                            stepDelay = footstepDelay;
                            animator.SetInteger("Walk", 3);
                            movementSpeed = walkSpeed;
                        }
                    }
                    Vector3 direction = (player.position - transform.position).normalized;
                    transform.position += direction * movementSpeed * Time.deltaTime;
                    if (rb.velocity.magnitude <= (movementSpeed / 500f)) {
                        //Debug.Log(verticalDistance);
                        Debug.Log("CheckForObstacles?");
                        if (CheckObstacleAhead()) {
                            bool preciseLanding = false;
                            if (verticalDistance >= 4.0f) {
                                Debug.Log("SuperJump!");
                                jumpHeight = verticalDistance + 4.0f;
                                jumpDelay = 1.5f;
                                jumpDistance = verticalDistance + 1.0f;
                                preciseLanding = true;
                            } else {
                                jumpHeight = 4f;
                                jumpDelay = 1.0f;
                                jumpDistance = 7f;
                            }
                            //Debug.Log("Jumping!");
                            isJumping = true;
                            if (Random.value > 0.6f) {
                                StartCoroutine(JumpInitiate(jumpDelay, 1, 1.25f, jumpHeight, jumpDistance, preciseLanding));
                            } else {
                                StartCoroutine(JumpInitiate(jumpDelay, 2, 1.25f, jumpHeight, jumpDistance, preciseLanding));
                            }
                        }
                    }
                } else {
                    //Debug.Log("Stopping walking.");
                    isMoving = false;
                    animator.SetInteger("Walk", 0);
                }

                if (distanceToPlayer <= attackDistance) {
                    isSprinting = false;
                    if (!isAttacking) {
                        if (isBlocking) {
                            StartCoroutine(DetermineShieldSlam());
                        }
                        isAttacking = true;
                        //Debug.Log("Starting attack!");
                        StartCoroutine(AttackBase(1.25f));
                        // 25% chance per frame for jump currently
                        if (!isJumping && Random.value < 0.15f) {
                            isJumping = true;
                            StartCoroutine(JumpInitiate(1.0f, 0, 1.25f, 4f, 7.0f));
                        }
                    }
                }
            } else {
                // Player is out of vision distance, stop walking and attacking
                isSprinting = false;
                isMoving = false;
                animator.SetInteger("Walk", 0);
                isAttacking = false;
            }
        }

    }
    
}
