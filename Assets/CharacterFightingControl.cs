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
            PlayerState.MakeBusyForTime(GetAnimationLength("Parry"));
            animator.Play("Parry", 0, 0.0f);
        }
        else if (punchPressed)
        {
            PlayerState.MakeBusyForTime(GetAnimationLength("Light Punch"));
            animator.Play("Light Punch", 0, 0.0f);
        }
        else if (kickPressed)
        {
            PlayerState.MakeBusyForTime(GetAnimationLength("Roundhouse Kick"));
            animator.Play("Roundhouse Kick", 0, 0.0f);
        }
        
    }
}
