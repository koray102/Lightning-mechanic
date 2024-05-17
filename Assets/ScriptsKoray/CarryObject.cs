using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryObject : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private Vector3 movePosLerp;
    [SerializeField] private float grabDelay = 3;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movePosLerp = transform.position;
    }


    public void OnInteract(Vector3 movePosition, bool isFocused)
    {
        if(isFocused)
        {
            //Debug.Log("Object was held. Pos: " + gameObject);

            movePosLerp = movePosition - transform.position;
            rb.velocity = movePosLerp * grabDelay;
            rb.useGravity = false;
        }else
        {
            NotInteract();
        }
    }

    public void NotInteract()
    {
        Debug.Log("Bıraktım carry");

        if(rb.useGravity == false)
        {
            rb.useGravity = true;
            movePosLerp = transform.position;
        }
    }
}
