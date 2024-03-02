using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float bobbingIntensity = 5f; // head bob

    private float verticalRotation = 0;

    private float bobbingDelayTimer = 0f;
    private float footstepDelay;

    [SerializeField] 
    private AudioSource footStepAS;
    [SerializeField]
    private AudioClip[] footStepClips;
    private int lastFootStepIndex, newFootStepIndex;

    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashSpeed = 25.0f, dashDuration = 0.5f, dashCooldown = 3.0f, lastDashTime;

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

    private float timer = 0.0f;
    private float bobbingSpeed;
    private float bobbingAmount = 0.2f;
    private float midpoint = 0.6f;

    [SerializeField]
    private AudioClip[] dashAudioClips;
    private int lastDashAudio = 0;

    private bool isWeaponSheathed = true;

    [SerializeField]
    private Holdable inOffHandHoldable;

    private bool isBlocking = false;

    /// Player offhands
    [SerializeField]
    private Shield playerShield;
    [SerializeField]
    private Torch playerTorch;

    /// Player Weapons
    [SerializeField]
    private Sword playerSword;
    [SerializeField]
    private Bow playerBow;

    private bool isBowEquipped = false;
    private bool isBowBeingReadied = false;

    [SerializeField]
    private Stats playerStats;
    [SerializeField]
    private PlayerUI playerUI;


    void Start() {
        Debug.Log("Main Camera: " + Camera.main.gameObject.name + " " + Camera.main);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();

        midpoint = Camera.main.transform.localPosition.y;

        //playerStats = GetComponent<Stats>();
        //playerUI = GetComponent<PlayerUI>();
    }
    
    private void PlayFootStep() {
        newFootStepIndex = Utility.GetRandomNonRepeatInt(footStepClips.Length, lastFootStepIndex);
        lastFootStepIndex = newFootStepIndex;
        footStepAS.clip = footStepClips[newFootStepIndex];
        footStepAS.Play();
    }

    private void Dash() {
        if (Time.time - lastDashTime < dashCooldown)
            return;

        dashDirection = characterController.velocity.normalized; 

        Vector3 dashVelocity = dashDirection * dashSpeed;

        playerStats.AdjustStamina(-10.0f);
        StartCoroutine(PerformDash(dashVelocity));

        if (dashAudioClips.Length > 0) {
            lastDashAudio = Utility.GetRandomNonRepeatInt(dashAudioClips.Length, lastDashAudio);
            footStepAS.clip = dashAudioClips[lastDashAudio];
            footStepAS.volume *= 5f;
            footStepAS.Play();
        }

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
        //inHandsWeapon.onStopUse();
    }

    private IEnumerator TorchAttack() {
        canAttack = false;
        playerAnimator.SetTrigger("TorchSpecial");
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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

        footStepAS.volume = Utility.VOLUME_WALK;
        bobbingSpeed = Utility.BOBBING_SPEED;
        footstepDelay = Utility.FOOTSTEP_DELAY;

        if (Input.GetKey(KeyCode.LeftShift)) {
            bobbingSpeed *= 2f;
            forwardSpeed *= 2f;
            sideSpeed *= 1.5f;
            footStepAS.volume *= 2f;
            footstepDelay /= 2f;
            playerStats.AdjustStamina(-0.5f);
        } else if (Input.GetKey(KeyCode.Space)) {
            Dash();
        }

        Vector3 speedVector = new Vector3(sideSpeed, 0, forwardSpeed);

        speedVector = transform.rotation * speedVector;

        characterController.SimpleMove(speedVector);
    }

    IEnumerator DeactivateOffHandLayer(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        playerAnimator.SetLayerWeight(1, 0);
    }

    IEnumerator ActivateGOAtTime(GameObject mesh, float waitTime, bool isActive) {
        yield return new WaitForSeconds(waitTime);
        mesh.SetActive(isActive);
    }

    void DashBobbingMotion() {
        float dashBobbingAmount = 70f;
        float dashBobbingSpeed = 5f;
        float leanFactor = 1f;

        float rollAngle = Mathf.Sin(Time.time * dashBobbingSpeed) * dashBobbingAmount;

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 dashDirection = horizontalInput != 0 ? new Vector3(horizontalInput, 0f, 0f) : Vector3.zero;

        float leanAngle = Vector3.Dot(transform.right, dashDirection.normalized) * rollAngle * leanFactor;

        float totalRollAngle = rollAngle + leanAngle;

        Quaternion rollRotation = Quaternion.Euler(0f, 0f, totalRollAngle);

        float lerpFactor = 0.1f;
        float interpolationFactor = lerpFactor * Time.deltaTime;
        Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, rollRotation, interpolationFactor);
    }

    // Almond espresso decaff, shot of sugar free vanilla, extra sweet cream foam

    void BobbingMotion() {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
            timer = 0.0f;
        } else {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if (timer > Mathf.PI * 2) {
                timer = timer - (Mathf.PI * 2);
            }
        }

        if (waveslice != 0) {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;

            // Apply bobbing motion to the camera's local position
            Vector3 localPosition = Camera.main.transform.localPosition;
            localPosition.y = midpoint + translateChange;
            Camera.main.transform.localPosition = localPosition;
        } else {
            // Reset camera position if there's no bobbing motion
            Vector3 localPosition = Camera.main.transform.localPosition;
            localPosition.y = midpoint;
            Camera.main.transform.localPosition = localPosition;
        }
    }

    private IEnumerator SheathSword() {
        isWeaponSheathed = true;
        playerAnimator.SetTrigger("SheathWeapon");
        yield return new WaitForSeconds(1.75f);
        inHandsWeapon.mesh.SetActive(false);
    }

    private IEnumerator UnsheathSword() {
        isWeaponSheathed = false;
        playerAnimator.SetTrigger("UnsheathWeapon");
        yield return new WaitForSeconds(1.5f);
        inHandsWeapon.mesh.SetActive(true);
    }

    private IEnumerator UnsheathBow() {
        isBowEquipped = true;
        playerAnimator.SetLayerWeight(2, 1.0f);
        playerAnimator.SetTrigger("EquipBow");
        inHandsWeapon.OnEquip();
        yield return new WaitForSeconds(1.25f);
        inHandsWeapon.mesh.SetActive(true);
    }

    private IEnumerator SheathBow() {
        isBowEquipped = false;
        playerBow.onStopUse();
        playerAnimator.SetTrigger("UnequipBow");
        StartCoroutine(ActivateGOAtTime(inHandsWeapon.mesh, 1.25f, false));
        yield return new WaitForSeconds(3.5f);
        playerAnimator.SetLayerWeight(2, 0.0f);
    }

    private void OffhandEnable() {
        playerAnimator.SetLayerWeight(1, 1.0f);
        playerAnimator.SetTrigger("OffHandEquip");
        StartCoroutine(ActivateGOAtTime(inOffHandHoldable.mesh, 1.75f, true));
        inOffHandHoldable.OnEquip();
    }

    private void OffHandDisable() {
        StartCoroutine(DeactivateOffHandLayer(3f));
        playerAnimator.SetTrigger("OffHandUnequip");
        StartCoroutine(ActivateGOAtTime(inOffHandHoldable.mesh, 1.75f, false));
        inOffHandHoldable.onStopUse();
        inOffHandHoldable = null;
    }

    void Update() {
        if (!isDashing) {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.Alpha2) && isWeaponSheathed) {
                inHandsWeapon = playerSword;
                StartCoroutine(UnsheathSword());
                inHandsWeapon.OnEquip();
            } else if (Input.GetKeyDown(KeyCode.Alpha1) && !isWeaponSheathed) {
                StartCoroutine(SheathSword());
                inHandsWeapon.onStopUse();
            }

            if (Utility.IsGrounded(gameObject)) {
                BobbingMotion();
                if (!isBlocking && !isBowBeingReadied && !isCharging)
                    playerStats.AdjustStamina(0.1f);
                if (characterController.velocity.magnitude > 0) {
                    bobbingDelayTimer += Time.fixedDeltaTime;

                    if (bobbingDelayTimer >= footstepDelay) {
                        PlayFootStep();
                        bobbingDelayTimer = 0f;
                    }
                } else {
                    bobbingDelayTimer = 0f;
                }
            }

            if (inOffHandHoldable != null) {
                if (inOffHandHoldable.type == HoldableType.Shield && Input.GetMouseButton(1)) {
                    //Debug.Log("isBlocking!");
                    playerAnimator.SetBool("Block", true);
                    if (!isBlocking) {
                        inOffHandHoldable.OnUse();
                        isBlocking = true;
                    }
                } else {
                    //Debug.Log("Not blocking!");
                    playerAnimator.SetBool("Block", false);
                    if (isBlocking) {
                        inOffHandHoldable.GetComponent<Shield>().retractShield();
                        isBlocking = false;
                    }
                }
                if (inOffHandHoldable.type == HoldableType.Torch && Input.GetMouseButtonDown(1)) {
                    StartCoroutine(TorchAttack());
                    playerStats.AdjustStamina(-4.0f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                if (inOffHandHoldable == null) {
                    inOffHandHoldable = playerShield;
                    OffhandEnable();
                } else {
                    OffHandDisable();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                if (inOffHandHoldable == null) {
                    inOffHandHoldable = playerTorch;
                    OffhandEnable();
                } else {
                    OffHandDisable();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha5) && isWeaponSheathed && inOffHandHoldable == null) {
                if (!isBowEquipped) {
                    Debug.Log("Bow Equip");
                    inHandsWeapon = playerBow;
                    StartCoroutine(UnsheathBow());
                } else {
                    Debug.Log("Bow UnEquip");
                    StartCoroutine(SheathBow());
                }
            }

            if (inOffHandHoldable == null && isWeaponSheathed && isBowEquipped) {
                if (Input.GetMouseButtonDown(1)) {
                    Debug.Log("Bow is being readied.");
                    if (!isBowBeingReadied) {
                        playerBow.OnUse();
                        playerStats.AdjustStamina(-5.0f);
                    }
                    isBowBeingReadied = true;
                    playerAnimator.SetTrigger("LoadBow");
                }
                if (Input.GetMouseButtonUp(1)) {
                    if (isBowBeingReadied) {
                        Debug.Log("Unloading the Bow");
                        isBowBeingReadied = false;
                        playerBow.OnUndraw();
                        playerAnimator.SetTrigger("UnloadBow");
                    }
                    //playerBow.ShootArrow();
                    //playerAnimator.SetTrigger("ReleaseBow");
                } else if (Input.GetMouseButtonDown(0)) {
                    if (isBowBeingReadied) {
                        playerAnimator.SetTrigger("ReleaseBow");
                        Debug.Log("Bow is shooting arrow.");
                        playerBow.ShootArrow(40.0f);
                        isBowBeingReadied = false;
                    }
                }
            }

            if (!isWeaponSheathed && !isBlocking && canAttack) {
                if (!isDashing && Input.GetMouseButtonDown(0)) {
                    StartCoroutine(AttackBase());
                    playerStats.AdjustStamina(-5.5f);
                }

                if (Input.GetMouseButtonDown(1))
                    StartCharging();

                if (isCharging && Input.GetMouseButtonUp(1)) {
                    ReleaseChargedAttack();
                    playerStats.AdjustStamina(-9.5f);
                }
            }

            if (isCharging) {
                currentChargeTime += Time.deltaTime * chargeSpeed;
                currentChargeTime = Mathf.Clamp(currentChargeTime, 0.0f, maxChargeTime);
            }
        } else {
            DashBobbingMotion();
        }
        playerUI.UpdateAllUIBars();
    }
}
