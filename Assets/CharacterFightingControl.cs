using UnityEngine;


public class CharacterFightingControl : CharacterControl
{
    PlayerState ps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        animator = GetComponent<Animator>();  
        ps = GetComponent<PlayerState>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerState.IsBusy())
        {
            return;
        }
        
        bool altPressed = Input.GetKey(KeyCode.LeftAlt);
        bool mouseRightClick = Input.GetKeyDown(KeyCode.Mouse1);
        bool mouseLeftClick = Input.GetKeyDown(KeyCode.Mouse0);

        bool punchPressed = !altPressed && mouseLeftClick;
        bool kickPressed = !altPressed && mouseRightClick;
        bool parryPressed = altPressed && mouseLeftClick;

        HandleAttack(punchPressed, kickPressed, parryPressed);
    }

    void HandleAttack(bool punchPressed, bool kickPressed, bool parryPressed)
    {
        if (parryPressed)
        {
            animator.Play("Parry", 0, 0.0f);
            float animationLength = GetAnimationLength("Parry");
            PlayerState.MakeBusyForTime(animationLength);
            AttackMove(animationLength, 0.3f);
        }
        else if (punchPressed)
        {
            float animationLength = GetAnimationLength("Light Punch", 1.4f);
            animator.Play("Light Punch", 0, 0.0f);
            PlayerState.MakeBusyForTime(animationLength);
            AttackMove(animationLength, 0.3f);
        }
        else if (kickPressed)
        {
            float animationLength = GetAnimationLength("Roundhouse Kick", 1.6f);
            animator.Play("Roundhouse Kick", 0, 0.0f);
            PlayerState.MakeBusyForTime(animationLength);
            AttackMove(animationLength, 0.3f);
        }
        
    }
}
