using UnityEngine;
using System.Collections;

public enum AttackInput
{
    None,
    Light,
    Heavy,
    Special,
    Defend
}

public class PlayerFightingControl : CharacterControl
{
    [SerializeField] CapsuleCollider cCollider;

    [Header("Attack Settings")]
    [SerializeField] float attackMoveDistance = 0.3f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float jumpKickDistance = 5f;
    [SerializeField] float jumpKickHeight = 2f;
    [SerializeField] float jumpPunchDistance = 5f;
    [SerializeField] float jumpPunchHeight = 2f;

    [Header("Click Delay Settings")]
    [SerializeField] float cooldownTime = 0.5f;
    [SerializeField] float clickWindow = 1f;
    [SerializeField] int numberOfClicks = 0;

    float timeOfNextAttack = 0.0f;
    float nextClickTime = 0.0f;

    bool inputDetected;
    bool lightAttack;
    bool heavyAttack;
    bool specialAttack;
    bool defend;

    AttackInput lastClickedInput = AttackInput.None;

    int dynamicAttackHash;



    override protected void Start()
    {
        base.Start();
        dynamicAttackHash = Animator.StringToHash("DynamicAttack");
    }



    void Update()
    {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool mouseRightClick = Input.GetKeyDown(KeyCode.Mouse1);
        bool mouseLeftClick = Input.GetKeyDown(KeyCode.Mouse0);

        inputDetected = mouseLeftClick || mouseRightClick;
        if (!inputDetected) return;

        if (PlayerState.IsBusy())
        {
            if ((PlayerState.IsDashing() || PlayerState.IsJumping()) && mouseLeftClick)
            {
                HandleDynamicAttack();
            }
            return;    
        }

        lightAttack = mouseLeftClick;
        heavyAttack = false;
        specialAttack = false;
        defend = mouseRightClick;

        if (Time.time < nextClickTime)
            return;

        if (Time.time < nextClickTime + clickWindow)
        {
            IncrementOnClick();
            HandleAttack();
            return;
        }

        if (Time.time > nextClickTime + clickWindow)
        {
            numberOfClicks = 0;
            IncrementOnClick();
            HandleAttack();
            return;
        }
    }



    void HandleDynamicAttack()
    {
        if (PlayerState.IsDashing())
        {
            StartCoroutine(DynamicAttack("Flying Punch", jumpPunchDistance, jumpPunchHeight));
        }
        else if (PlayerState.IsJumping())
        {
            StartCoroutine(DynamicAttack("Flying Kick", jumpKickDistance, jumpKickHeight));
        }
    }



    void HandleAttack()
    {        
        if (lightAttack)
            HandleLight();
        else if (heavyAttack)
            HandleHeavy();
        else if (specialAttack)
            HandleSpecial();
        else if (defend)
            HandleDefend();
    }



    void CheckInputChange(AttackInput input)
    {
        if (lastClickedInput != input)
        {
            lastClickedInput = input;
            numberOfClicks = 1;
        }
    }



    void HandleLight()
    {
        CheckInputChange(AttackInput.Light);
        if (numberOfClicks == 1)
            StartCoroutine(Attack("Light 1", 1.6f, 1f));
        else if (numberOfClicks == 2)
            StartCoroutine(Attack("Light 2", 1.6f, 1f));
        else if (numberOfClicks == 3)
            StartCoroutine(Attack("Light 3", 2f, 1.5f));
        else if (numberOfClicks == 4)
            StartCoroutine(Attack("Light 4", 1.9f, 1.5f));
        else if (numberOfClicks > 4)
            numberOfClicks = 0;
    }



    void HandleHeavy()
    {
        CheckInputChange(AttackInput.Heavy);
        if (numberOfClicks == 1)
        {
            StartCoroutine(Attack("Heavy 1"));
            numberOfClicks = 0;
        }
        else if (numberOfClicks == 2)
            StartCoroutine(Attack("Heavy 2"));
        else if (numberOfClicks == 3)
            StartCoroutine(Attack("Heavy 3"));
        else if (numberOfClicks == 4)
            StartCoroutine(Attack("Heavy 4"));
        else if (numberOfClicks > 4)
            numberOfClicks = 0;
    }



    void HandleSpecial()
    {
        CheckInputChange(AttackInput.Special);
        if (numberOfClicks == 1)
            StartCoroutine(Attack("Special 1"));
        else if (numberOfClicks == 2)
            StartCoroutine(Attack("Special 2"));
        else if (numberOfClicks == 3)
            StartCoroutine(Attack("Special 3"));
        else if (numberOfClicks == 4)
            StartCoroutine(Attack("Special 4"));
        else if (numberOfClicks > 4)
            numberOfClicks = 0;
    }



    void HandleDefend()
    {
        CheckInputChange(AttackInput.Defend);
        Defend();
    }



    IEnumerator Attack(string attackName, float effectiveSpeed = 1f, float moveDistance = 0f)
    {
        if (moveDistance == 0f)
            moveDistance = attackMoveDistance;
        if (timeOfNextAttack > Time.time)
            yield return new WaitForSeconds(timeOfNextAttack - Time.time);

        float animationLength = GetAnimationLength(attackName, effectiveSpeed);
        Debug.Log("Attack: " + attackName + " for " + animationLength + " seconds");
        PlayerState.MakeBusyForTime(animationLength, ActionState.Attacking);
        timeOfNextAttack = Time.time + animationLength - 0.5f;

        animator.Play(attackName);
        AttackMove(animationLength, moveDistance);
    }



    IEnumerator DynamicAttack(string attackName, float distance = 0f, float height = 0f, float effectiveSpeed = 1f)
    {
        bool wasDashing = false;
        if (PlayerState.IsDashing())
            wasDashing = true;
            
        float animationLength = GetAnimationLength(attackName, effectiveSpeed);
        while (PlayerState.IsBusy() && wasDashing)
        {
            yield return null;
        }
        Debug.Log("Attack: " + attackName + " for " + animationLength + " seconds");
        PlayerState.MakeBusyForTime(animationLength, ActionState.Dashing);

        
        Vector3 startPos = transform.position;
        Vector3 destination = transform.position + transform.forward * distance;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward + Vector3.up, out RaycastHit hit, distance, collisionMask))
            destination = hit.point - Vector3.up;
        Vector3 totalDisplacement = destination - startPos;

        animator.SetTrigger(dynamicAttackHash);
        
        rb.angularVelocity = Vector3.zero;
        Vector3 leapForce = Vector3.up * height + transform.forward * distance;
        Debug.Log("LeapForce: " + leapForce);
        rb.AddForce(leapForce, ForceMode.Impulse);

        if (wasDashing)
            StartCoroutine(ChangeColliderHeight(animationLength));
        yield return new WaitForSeconds(animationLength);
    }



    IEnumerator ChangeColliderHeight(float animationLength)
    {
        float originalHeight = cCollider.height;
        cCollider.height = originalHeight / 2;

        yield return new WaitForSeconds(animationLength * 3 / 4);

        float elapsedTime = 0f;
        while (elapsedTime < animationLength / 4)
        {
            cCollider.height = Mathf.Lerp(originalHeight / 2, originalHeight, elapsedTime / (animationLength / 4));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cCollider.height = originalHeight;
    }



    void Defend()
    {
        animator.Play("Defend");
        PlayerState.MakeBusyForTime(1.0f);
    }



    void IncrementOnClick()
    {
        numberOfClicks++;
        nextClickTime = Time.time + cooldownTime;
    }
}
