using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{
    private GameObject childObj;
    private bool didExploded = false;
    [SerializeField] private float collisionMultp;
    [SerializeField] private float minVelocity;


    void OnCollisionStay(Collision other)
    {
        if(other.gameObject.TryGetComponent(out IInteractable interactable))
        {
            if(other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                if(rb.velocity.magnitude >= minVelocity)
                {
                    Explode(other.gameObject);
                }
            }
        }

    }


    public void Explode(GameObject throwedObject)
    {
        if(!didExploded)
        {
            Debug.Log("Explode object");

            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
            
            foreach (var childCollider in gameObject.GetComponentsInChildren<MeshCollider>())
            {
                childObj = childCollider.gameObject;
                if(childObj != gameObject)
                {
                    childObj.AddComponent<Rigidbody>();
                    childObj.GetComponent<Rigidbody>().isKinematic = false;

                    childObj.GetComponent<Rigidbody>().AddExplosionForce(collisionMultp, throwedObject.transform.position, 20);
                }
            }

            didExploded = true;
        }
    }
}
