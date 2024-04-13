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
    private float dashSpeed = 25.0f, dashDuration = 0.5f; //dashCooldown = 3.0f, lastDashTime;

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

    private bool isMainHandBusy = false, isOffHandBusy = false;

    // vaulting
    private bool isVaulting = false;
    [SerializeField] 
    private float vaultAnimDuration = 1.0f, vaultHeight = 1.0f;
    [SerializeField]
    private LayerMask vaultLayerMask;
    // valuting

    private Rigidbody rb;
    private bool isMovementEnabled = true;

    private Vector3 currentPos, lastPos;

    private float dashStaminaCost = 15.0f;

    //private bool isDashBobbingMotion = false;

    void Start() {
        //Debug.Log("Main Camera: " + Camera.main.gameObject.name + " " + Camera.main);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        midpoint = Camera.main.transform.localPosition.y;

        playerStats.InitializeStats();

        playerUI.InitializeBars(playerStats);

        //playerStats = GetComponent<Stats>();
        //playerUI = GetComponent<PlayerUI>();
    }

    //private void OnCollisionEnter(Collision collision) {
    //    Debug.Log(collision.gameObject.name);
    //}

    public Stats getStats() {
        return playerStats;
    }

    private void PlayFootStep() {
        //Debug.Log("Playing Footstep");
        newFootStepIndex = Utility.GetRandomNonRepeatInt(footStepClips.Length, lastFootStepIndex);
        lastFootStepIndex = newFootStepIndex;
        footStepAS.clip = footStepClips[newFootStepIndex];
        footStepAS.Play();
    }

    private void Dash() {
        //if (Time.time - lastDashTime < dashCooldown)
        //    return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //dashDirection = characterController.velocity.normalized; 
        dashDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        dashDirection.y = 0f;

        if (dashDirection.magnitude > 0) {
            dashDirection.Normalize();

            Vector3 dashVelocity = dashDirection * dashSpeed;
            //Debug.Log(dashDirection);

            playerStats.TakeStaminaDrain(dashStaminaCost);
            StartCoroutine(PerformDash(dashVelocity));

            if (dashAudioClips.Length > 0) {
                lastDashAudio = Utility.GetRandomNonRepeatInt(dashAudioClips.Length, lastDashAudio);
                footStepAS.clip = dashAudioClips[lastDashAudio];
                footStepAS.volume *= 5f;
                footStepAS.Play();
            }
        }

        //lastDashTime = Time.time;
    }

    
    private IEnumerator PerformDash(Vector3 dashVelocity) {
        isDashing = true;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration) {
            rb.velocity = dashVelocity;
            //characterController.SimpleMove(dashVelocity);
            //float rollAngle = Mathf.Sin((Time.time - startTime) * 10) * 10;
            //Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, rollAngle);
            yield return null;
        }
        rb.velocity = Vector3.zero;
        //Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0);

        isDashing = false;
    }

    private IEnumerator AttackDelay() {
        canAttack = false;
        //playerAnimator.SetTrigger("AttackBase");
        //inHandsWeapon.OnUse();

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        //inHandsWeapon.onStopUse();
    }

    private void AttackBase() {
        playerAnimator.SetTrigger("AttackBase");
        inHandsWeapon.OnUse();
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

    private void EndCharging() {
        isCharging = false;
        playerAnimator.SetBool("IsCharging", false);
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
    /*
    private void HandleLooking() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
    */

    private void HandleMovement() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        //transform.rotation = Quaternion.Lerp(transform.rotation, inHandsWeapon.transform.rotation, Time.deltaTime * 10.0f);

        if (Utility.IsGrounded(gameObject) && !isDashing) {

            float forwardSpeed = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float sideSpeed = Input.GetAxis("Horizontal") * speed * Time.deltaTime;


            footStepAS.volume = Utility.VOLUME_WALK;
            bobbingSpeed = Utility.BOBBING_SPEED;
            footstepDelay = Utility.FOOTSTEP_DELAY;

            if (Input.GetKey(KeyCode.LeftShift) && playerStats.getCurrentStamina() >= 1f) {
                bobbingSpeed *= 2f;
                forwardSpeed *= 2f;
                sideSpeed *= 1.5f;
                footStepAS.volume *= 2f;
                footstepDelay /= 2f;
                playerStats.AdjustStamina(-1f);
            } else if (Input.GetKey(KeyCode.Space) && !isVaulting) {
                RaycastHit hit; // vaulting
                if (Physics.Raycast(transform.position, transform.forward, out hit, vaultHeight, vaultLayerMask)) {
                    StartCoroutine(VaultOverLedge(hit.point));
                } else if (playerStats.getCurrentStamina() >= dashStaminaCost) {
                    Dash();
                }
            }
            Vector3 movement = new Vector3(sideSpeed, 0, forwardSpeed);
            movement = transform.TransformDirection(movement); // Transform into world space

            // Apply the movement to the rigidbody
            rb.MovePosition(rb.position + movement);
        }
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
        //Debug.Log("Dash bob motion!");
        float dashBobbingAmount = 1f;
        float dashBobbingSpeed = 5f;
        float leanFactor = 25f;

        float rollAngle = Mathf.Sin(Time.time * dashBobbingSpeed) * dashBobbingAmount;

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 dashDirection = horizontalInput != 0 ? new Vector3(horizontalInput, 0f, 0f) : Vector3.zero;

        float leanAngle = Vector3.Dot(transform.right, dashDirection.normalized) * rollAngle * leanFactor;

        float totalRollAngle = rollAngle + leanAngle;

        Quaternion rollRotation = Quaternion.Euler(0f, 0f, totalRollAngle);

        float lerpFactor = 10f;
        float interpolationFactor = lerpFactor * Time.deltaTime;
        Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, rollRotation, interpolationFactor);
        //Debug.Log(rollRotation + " " + interpolationFactor);
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

    private IEnumerator MainHandBusyDelay(float time) {
        isMainHandBusy = true;
        yield return new WaitForSeconds(time);
        isMainHandBusy = false;
    }

    private IEnumerator OffHandBusyDelay(float time) {
        isOffHandBusy = true;
        yield return new WaitForSeconds(time);
        isOffHandBusy = false;
    }

    private IEnumerator UnsheathBow() {
        isBowEquipped = true;
        isMainHandBusy = true;
        isOffHandBusy = true;
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
        isOffHandBusy = false;
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

    /*
    private IEnumerator VaultOverLedge(Vector3 ledgePosition) {
        isVaulting = true;
        float startTime = Time.time;
        //playerAnimator.SetTrigger("Vault");

        while (Time.time < startTime + vaultAnimDuration) {
            float progress = (Time.time - startTime) / vaultAnimDuration;
            float verticalOffset = Mathf.Lerp(0, vaultHeight, progress);
            transform.position = new Vector3(transform.position.x, transform.position.y + verticalOffset, transform.position.z);


            yield return null;
        }
        // Ensure the player ends up at the correct height after the vault
        transform.position = new Vector3(transform.position.x, ledgePosition.y + vaultHeight, transform.position.z);
        Debug.Log("New transform: " + transform.position);
        isVaulting = false;
    }
    */

    public IEnumerator StunLock(float time) {
        isMovementEnabled = false;
        //characterController.enabled = isMovementEnabled;
        yield return new WaitForSeconds(time);
        //characterController.enabled = true;
        isMovementEnabled = true;
    }

    public Rigidbody GetRigidBody() {
        return rb;
    }

    private IEnumerator VaultOverLedge(Vector3 ledgePosition) {
        isVaulting = true;
        float startTime = Time.time;
        float initialHeight = transform.position.y;
        bool continueClimbing = true;

        //while (Time.time < startTime + vaultAnimDuration) {
        while (continueClimbing) {
            float progress = (Time.time - startTime) / vaultAnimDuration;
            float verticalOffset = Mathf.Lerp(0, vaultHeight * 3f, progress);
            float newY = initialHeight + verticalOffset;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            //if (!Input.GetKey(KeyCode.Space) || !Physics.Raycast(transform.position, transform.forward, vaultHeight, vaultLayerMask)) {
            if (!Input.GetKey(KeyCode.Space) || !Physics.Raycast(transform.position, transform.forward, 2.0f, vaultLayerMask)) {
                continueClimbing = false;
            }

            yield return null;
        }

        // Ensure the player ends up at the correct height after the vault
        transform.position = new Vector3(transform.position.x, ledgePosition.y + vaultHeight, transform.position.z);
        Debug.Log("New transform: " + transform.position);
        isVaulting = false;
    }

    // Update (Fixed for incosistent frame rate changes?)

    private void Update() {
        if (!isDashing) {
            if (!isVaulting && Input.GetKeyDown(KeyCode.Space)) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, vaultHeight, vaultLayerMask)) {
                    // Detected a ledge, start vaulting
                    StartCoroutine(VaultOverLedge(hit.point));
                }
            }

            //transform.rotation = Quaternion.Lerp(transform.rotation, inHandsWeapon.transform.rotation, Time.deltaTime * 10.0f);


            if (!isMainHandBusy) {
                if (Input.GetKeyDown(KeyCode.Alpha1) && isWeaponSheathed) {
                    StartCoroutine(MainHandBusyDelay(2.25f));
                    inHandsWeapon = playerSword;
                    StartCoroutine(UnsheathSword());
                    inHandsWeapon.OnEquip();
                } else if (Input.GetKeyDown(KeyCode.Alpha1) && !isWeaponSheathed) {
                    StartCoroutine(MainHandBusyDelay(2.0f));
                    StartCoroutine(SheathSword());
                    inHandsWeapon.onStopUse();
                }
                if (!isWeaponSheathed && !isBlocking && canAttack) {
                    if (Input.GetMouseButtonDown(0) && !isCharging) {
                        AttackBase();
                        StartCoroutine(AttackDelay());
                        playerStats.AdjustStamina(-5.5f);
                    }

                    if (Input.GetMouseButtonDown(1) && inOffHandHoldable == null)
                        StartCharging();
                    if (isCharging) {
                        if (Input.GetMouseButtonUp(1))
                            EndCharging();
                        else if (Input.GetMouseButtonDown(0)) {
                            ReleaseChargedAttack();
                            StartCoroutine(AttackDelay());
                            playerStats.AdjustStamina(-9.5f);
                        }
                    }
                }
            }

            if (!isOffHandBusy) {
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
                        StartCoroutine(OffHandBusyDelay(3.0f));
                        OffhandEnable();
                    } else {
                        StartCoroutine(OffHandBusyDelay(3.0f));
                        OffHandDisable();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Alpha4)) {
                    if (inOffHandHoldable == null) {
                        inOffHandHoldable = playerTorch;
                        StartCoroutine(OffHandBusyDelay(3.0f));
                        OffhandEnable();
                    } else {
                        StartCoroutine(OffHandBusyDelay(3.0f));
                        OffHandDisable();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha5) && isWeaponSheathed && inOffHandHoldable == null) {
                if (!isBowEquipped && !isMainHandBusy) {
                    Debug.Log("Bow Equip");
                    isOffHandBusy = true;
                    inHandsWeapon = playerBow;
                    StartCoroutine(UnsheathBow());
                } else {
                    Debug.Log("Bow UnEquip");
                    StartCoroutine(OffHandBusyDelay(3.0f));
                    StartCoroutine(MainHandBusyDelay(3.0f));
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

            if (isCharging) {
                currentChargeTime += Time.deltaTime * chargeSpeed;
                currentChargeTime = Mathf.Clamp(currentChargeTime, 0.0f, maxChargeTime);
            }
        }
    }

    
    private void FixedUpdate() {
        if (isMovementEnabled) {
            HandleMovement();
            if (!isDashing) {
                if (Utility.IsGrounded(gameObject)) {
                    BobbingMotion();
                    if (!isBlocking && !isBowBeingReadied && !isCharging)
                        playerStats.AdjustStamina(0.1f);
                    currentPos = rb.position;
                    if (currentPos != lastPos) {
                        bobbingDelayTimer += Time.fixedDeltaTime;

                        if (bobbingDelayTimer >= footstepDelay) {
                            PlayFootStep();
                            bobbingDelayTimer = 0f;
                        }
                        lastPos = currentPos;
                    } else {
                        bobbingDelayTimer = 0f;
                    }
                }
            } else {
                DashBobbingMotion();
            }
        }
        
        playerUI.UpdateAllUIBars();
    }

}

