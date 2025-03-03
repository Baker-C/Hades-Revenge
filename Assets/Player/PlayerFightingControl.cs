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
    [Header("Attack Settings")]
    [SerializeField] float attackRange = 2.0f;
    [SerializeField] float attackMoveDistance = 0.3f;

    [Header("Click Delay Settings")]
    [SerializeField] float cooldownTime = 0.5f;
    [SerializeField] float clickDelayWindow = 3f;
    [SerializeField] int numberOfClicks = 0;

    float timeOfNextAttack = 0.0f;
    float nextClickTime = 0.0f;
    float lastClickedTime = 0f;
    AttackInput lastClickedInput = AttackInput.None;

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool mouseRightClick = Input.GetKeyDown(KeyCode.Mouse1);
        bool mouseLeftClick = Input.GetKeyDown(KeyCode.Mouse0);

        bool inputDetected = mouseLeftClick || mouseRightClick;
        bool lightAttack = !shiftPressed && mouseLeftClick;
        bool heavyAttack = shiftPressed && mouseRightClick;
        bool specialAttack = shiftPressed && mouseLeftClick;
        bool defend = !shiftPressed && mouseRightClick;

        if (Time.time < nextClickTime)
            return;

        if (inputDetected && Time.time > nextClickTime + clickDelayWindow)
            numberOfClicks = 0;
        
        if (inputDetected)
        {
            Debug.Log("Input detected");
            IncrementOnClick();
            HandleAttack(lightAttack, heavyAttack, specialAttack, false);
        }
    }

    void HandleAttack(bool lightAttack, bool heavyAttack, bool specialAttack, bool defend)
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
            StartCoroutine(Attack("Light 1"));
        else if (numberOfClicks == 2)
            StartCoroutine(Attack("Light 2"));
        else if (numberOfClicks == 3)
            StartCoroutine(Attack("Light 3"));
        else if (numberOfClicks == 4)
            StartCoroutine(Attack("Light 4"));
    }

    void HandleHeavy()
    {
        CheckInputChange(AttackInput.Heavy);
        if (numberOfClicks == 1)
            StartCoroutine(Attack("Heavy 1"));
        else if (numberOfClicks == 2)
            StartCoroutine(Attack("Heavy 2"));
        else if (numberOfClicks == 3)
            StartCoroutine(Attack("Heavy 3"));
        else if (numberOfClicks == 4)
            StartCoroutine(Attack("Heavy 4"));
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
    }

    void HandleDefend()
    {
        CheckInputChange(AttackInput.Defend);
        Defend();
    }

    IEnumerator Attack(string attackName, float effectiveSpeed = 1f, float moveDistance = 0f)
    {
        float delayUntilNextAttack = timeOfNextAttack - Time.time;
        yield return new WaitForSeconds(delayUntilNextAttack);

        float animationLength = GetAnimationLength(attackName, effectiveSpeed);
        animator.Play(attackName);
        timeOfNextAttack = Time.time + animationLength;
        PlayerState.MakeBusyForTime(animationLength);
        AttackMove(animationLength, moveDistance);
    }

    void Defend()
    {
        animator.Play("Defend");
        PlayerState.MakeBusyForTime(1.0f);
    }

    void IncrementOnClick()
    {
        Debug.Log("CLicked - OnClick PlayerFightingControl");
        numberOfClicks++;
        nextClickTime = Time.time + cooldownTime;
    }
}
