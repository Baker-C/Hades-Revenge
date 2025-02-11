using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float swingSpeed = 200f;
    [SerializeField] private float maxRotation = 120f;
    private GameObject weaponObject;
    private Quaternion startRotation;


    void Start()
    {
        weaponObject = transform.Find("Sword.001").gameObject;
        startRotation = weaponObject.transform.localRotation;
        Debug.Log("Weapon object: " + startRotation);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator AttackPlayer()
    {
        if (weaponObject == null) yield break;
        Debug.Log("Attacking player");

        Debug.Log("Start rotation: " + startRotation);
        float elapsedTime = 0f;
        Quaternion startRot = startRotation;
        Quaternion endRot = Quaternion.Euler(maxRotation, 0, 0);
        Debug.Log("End rotation: " + endRot);

        // Swing forward
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * swingSpeed;
            weaponObject.transform.localRotation = Quaternion.Lerp(startRot, endRot, elapsedTime);
            yield return null;
        }

        // Return to starting position
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * swingSpeed;
            weaponObject.transform.localRotation = Quaternion.Lerp(endRot, startRot, elapsedTime);
            yield return null;
        }

        weaponObject.transform.localRotation = startRot;
    }
}
