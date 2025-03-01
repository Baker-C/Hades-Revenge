using UnityEngine;

public class PlayerHealth : CharacterHealth
{
    override protected void Start()
    {
        cc = gameObject.GetComponent<PlayerControl>();
    }

}
