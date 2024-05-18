using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FocusObject : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    private Rigidbody rb;
    private Vector3 movePosLerp;
    private GameObject spotlight;
    private bool isHolding;
    private float focusTime;
    [SerializeField] private float maxFocusTime;
    private Vector3 throwVector;
    [SerializeField] private float throwMultiplier;
    [SerializeField] private float grabDelay = 3;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spotlight = GameObject.FindGameObjectWithTag("SpotLight");
        movePosLerp = transform.position;
    }


    public void OnInteract(Vector3 movePosition, bool isFocused)
    {
        if(isFocused)
        {
            //Debug.Log("Object was held. Pos: " + gameObject);
            focusTime += Time.fixedDeltaTime;

            movePosLerp = movePosition - transform.position;
            rb.velocity = movePosLerp * grabDelay;
            rb.useGravity = false;
        }else
        {
            NotInteract(true);
        }
    }

    public void NotInteract(bool isLightning)
    {
        Debug.Log("Bıraktım focus");

        if(rb.useGravity == false)
        {
            focusTime = Mathf.Clamp(focusTime, 0, maxFocusTime);
            throwVector = transform.position - spotlight.transform.position;
            rb.AddForce(throwVector.normalized * throwMultiplier * focusTime, ForceMode.Impulse);

            rb.useGravity = true;
            movePosLerp = transform.position;

            focusTime = 0;
        }
    }
}
