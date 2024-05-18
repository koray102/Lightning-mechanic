using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private Vector3 movePosLerp;
    private Vector3 originalScale;
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private float growSpeed = 0.05f;
    [SerializeField] private float shrinkSpeed = 0.1f;
    [SerializeField] private float grabDelay = 3;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movePosLerp = transform.position;
        originalScale = transform.localScale;
    }


    public void OnInteract(Vector3 movePosition, bool isFocused)
    {
        if(isFocused)
        {
            //Debug.Log("Object was held. Pos: " + gameObject);

            movePosLerp = movePosition - transform.position;
            rb.velocity = movePosLerp * grabDelay;
            rb.useGravity = false;
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, shrinkSpeed * Time.fixedDeltaTime);
            
        }else
        {
            // Obje maksimum boyuta kadar büyüyor
            transform.localScale = Vector3.Lerp(transform.localScale, maxScale, growSpeed * Time.fixedDeltaTime);

            NotInteract(true);
        }
    }

    public void NotInteract(bool isLightning)
    {
        Debug.Log("Bıraktım scale");

        if(rb.useGravity == false)
        {
            rb.useGravity = true;
            movePosLerp = transform.position;
        }
        
        if(!isLightning)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, shrinkSpeed * Time.fixedDeltaTime);
        }
    }
}
