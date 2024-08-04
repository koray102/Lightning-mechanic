using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreasurePlate : MonoBehaviour
{   
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float minTriggerDistance;
    private bool isTriggered;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down),out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if(hit.distance < minTriggerDistance)
            {
                isTriggered = true;
            }else
            {
                isTriggered = false;
            }
        }
    }
}
