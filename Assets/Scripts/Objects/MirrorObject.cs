using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorObject : BasicObjectBehaviour, IInteractable
{
    private IInteractable.InteractionState state;
    private Coroutine setReflectionCoroutine;
    internal List<GameObject> lightObjects = new List<GameObject>();
    
    
    public void OnInteract(GameObject lightener)
    {
        State = SetState(lightener, true, lightObjects);

        if(setReflectionCoroutine != null)
        {
            StopCoroutine(setReflectionCoroutine);
        }

        if(State != IInteractable.InteractionState.LightningObject)
        {
           setReflectionCoroutine = StartCoroutine(SetReflection());
        }

    }
    

    public void NotInteract(GameObject lightener)
    {
        State = SetState(lightener, false, lightObjects);

        if(State == IInteractable.InteractionState.LightningObject || State == IInteractable.InteractionState.NotLightning)
        {
            StopCoroutine(setReflectionCoroutine);
        }
    }


    private IEnumerator SetReflection()
    {
        GameObject spotlight = GameObject.FindGameObjectWithTag("SpotLight");

        if(spotlight.TryGetComponent(out SpotlightController spotlightControllerSc))
        {
            Debug.Log("SetReflection");

            Vector3 objectNormal = spotlight.transform.position - transform.position;
            objectNormal.y = 0;
            objectNormal.z = 0;

            float distance;
            Vector3 direction;
            Vector3 startPosition; 

            while(true)
            {
                direction = Vector3.Reflect(spotlightControllerSc.lookPosition, objectNormal.normalized);

                distance = Vector3.Distance(spotlight.transform.position, transform.position);
                startPosition = spotlight.transform.position + spotlightControllerSc.lookPosition.normalized * distance;

                Debug.DrawRay(startPosition, direction, Color.blue, 1f);

                yield return null;
            }
        }
    }


    public IInteractable.InteractionState State
    {
        get { return state; }
        set { state = value; }
    }
}
