using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2 Timothy 2:21-26
// Good verse to use at intro?
// ********************* TO DO'S/IDEAS *********************
// Need video for HW turn in/presentation
// fix head bobbing
// animation speed of arms breathing based on speed
// add attack(s)
// animation for dashing
// divine power / bow?
// special attack?
// Health/Stamina/Power bar should be implemented
// potion drinking/food eating
// throwing of objects
// reading of book? (Divine Power)
// sounds for attacks? Character/sword/mace/arrow/divine power
// Shield? Torch?
// Bombs?
// Flush out sprinting
// sounds for sprinting
// Breathing?
// Godfrey?
// Decide on a Boss, and mechanics FIRST before stage build
// Build boss stage / Multiple phases / state machine
// Procedural Generation? 
// Gear?
// Enemies?
// Focus on Player mechanics/A single boss for project due date
// ********************************************************

public class FPSController : MonoBehaviour {
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;

    private CharacterController characterController;

    public float bobbingSpeed = 2f; // head bob
    public float bobbingIntensity = 5f; // head bob
    private float originalYPos; // head bob
    private float waveSlice; // head bob

    private float verticalRotation = 0;

    private float translateBobbingChange, totalAxes, bobbingAppliedIntensity; // head bob

    private float targetYPos, smoothFactor = 1f;

    private float bobbingDelayTimer = 0f;
    private float bobbingDelay = 0.5f;

    [SerializeField] 
    private AudioSource footStepAS;
    [SerializeField]
    private AudioClip[] footStepClips;
    //private int footStepCount = 0;
    private int lastFootStepIndex, newFootStepIndex;

    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashSpeed = 25.0f, dashDuration = 0.5f, dashCooldown = 3.0f, lastDashTime;

    //private Transform originalTransform = Camera.main.transform;
    //private Transform maxBobTransform = Camera.main.transform;

    [SerializeField]
    private Animator playerAnimator;
    private bool canAttack = true;
    public float attackCooldown = 1.0f;

    public float maxChargeTime = 2.0f;
    public float chargeSpeed = 1.0f;
    private bool isCharging = false;
    private float currentChargeTime = 0.0f;

    [SerializeField]
    private Weapon inHandsWeapon;


    void Start() {
        Debug.Log("Main Camera: " + Camera.main.gameObject.name + " " + Camera.main);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //maxBobTransform.localPosition += new Vector3(0, 1.0f, 0);

        characterController = GetComponent<CharacterController>();
        originalYPos = Camera.main.transform.localPosition.y; // head bob
    }
    
    private void ApplyHeadBobbing() {
        //Debug.Log("Applying head bob!");
        //Debug.Log("Orig Y Pos = " + originalYPos);
        waveSlice = Mathf.Sin(Time.time * bobbingSpeed);
        //Debug.Log(waveSlice);
        if (waveSlice != 0) {
            translateBobbingChange = waveSlice * bobbingIntensity;
            totalAxes = Mathf.Clamp(characterController.velocity.magnitude, 0, 1);
            bobbingAppliedIntensity = Mathf.Clamp(totalAxes * translateBobbingChange, -0.2f, 0.35f);

            targetYPos = originalYPos + bobbingAppliedIntensity;
            //smoothFactor = 2f;
            //Debug.Log(smoothFactor);


            //Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, originalYPos + bobbingAppliedIntensity, Camera.main.transform.localPosition.z);
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, targetYPos, Camera.main.transform.localPosition.z),
                                                               Time.deltaTime * smoothFactor);
        } else {
            //Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, originalYPos, Camera.main.transform.localPosition.z);
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, originalYPos, Camera.main.transform.localPosition.z),
                                                               Time.deltaTime * smoothFactor);
        }
    }
    

    private void PlayFootStep() {
        newFootStepIndex = Utility.GetRandomNonRepeatInt(footStepClips.Length, lastFootStepIndex);
        lastFootStepIndex = newFootStepIndex;
        footStepAS.clip = footStepClips[newFootStepIndex];
        footStepAS.Play();
    }

    private void Dash() {
        if (Time.time - lastDashTime < dashCooldown) // dash cooldown time based
            return;

        dashDirection = characterController.velocity.normalized; 

        // Apply dash speed in the dash direction
        Vector3 dashVelocity = dashDirection * dashSpeed;

        StartCoroutine(PerformDash(dashVelocity));

        // Update last dash time
        lastDashTime = Time.time;
    }

    
    private IEnumerator PerformDash(Vector3 dashVelocity) {
        isDashing = true;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration) {
            characterController.SimpleMove(dashVelocity);
            //float rollAngle = Mathf.Sin((Time.time - startTime) * 10) * 10;
            //Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, rollAngle);
            yield return null;
        }

        //Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0);

        isDashing = false;
    }

    private IEnumerator AttackBase() {
        canAttack = false;
        playerAnimator.SetTrigger("AttackBase");
        inHandsWeapon.OnUse();

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        inHandsWeapon.onStopUse();
    }

    private void StartCharging() {
        isCharging = true;
        playerAnimator.SetBool("IsCharging", true);
    }

    private void ReleaseChargedAttack() {
        if (isCharging) {
            inHandsWeapon.OnUse();
            playerAnimator.SetBool("IsCharging", false);
            playerAnimator.SetTrigger("AttackChargeRelease");
            currentChargeTime = 0.0f;
            isCharging = false;
        }
    }

    private void HandleMovement() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        float forwardSpeed = Input.GetAxis("Vertical") * speed;
        float sideSpeed = Input.GetAxis("Horizontal") * speed;

        if (Input.GetKey(KeyCode.LeftShift)) {
            //Debug.Log("Sprinting!");
            forwardSpeed *= 2f;
            sideSpeed *= 1.5f;
        } else if (Input.GetKey(KeyCode.Space)) {
            Dash();
        }
        //Debug.Log("Speed = " + forwardSpeed);

        Vector3 speedVector = new Vector3(sideSpeed, 0, forwardSpeed);

        speedVector = transform.rotation * speedVector;

        characterController.SimpleMove(speedVector);
    }

    void Update() {
        HandleMovement();
        //Debug.Log("bobbing delay = " + bobbingDelayTimer);
        if (characterController.velocity.magnitude > 0 && characterController.isGrounded) {
            //Debug.Log("Bobbing Delay = " + bobbingDelayTimer);
            bobbingDelayTimer += Time.fixedDeltaTime;

            if (bobbingDelayTimer >= bobbingDelay) {
                ApplyHeadBobbing();
                PlayFootStep();
                bobbingDelayTimer = 0f;
            }
        } else {
            bobbingDelayTimer = 0f;
        }
        if (inHandsWeapon != null) {
            if (canAttack && !isDashing && Input.GetMouseButtonDown(0)) {
                StartCoroutine(AttackBase());
            }

            if (Input.GetMouseButtonDown(1))
                StartCharging();

            if (isCharging && Input.GetMouseButtonUp(1))
                ReleaseChargedAttack();
        }

        if (isCharging) {
            currentChargeTime += Time.deltaTime * chargeSpeed;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0.0f, maxChargeTime);
        }

    }
}