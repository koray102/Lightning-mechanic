using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicObjectBehaviour : MonoBehaviour
{
    public IInteractable.InteractionState SetState(GameObject lightener, bool isAdded, List<GameObject> lightObjects)
    {
        if(isAdded)
        {
            if(!lightObjects.Contains(lightener))
            {
                lightObjects.Add(lightener);
            }
        }else
        {
            lightObjects.Remove(lightener);
        }

        if(lightObjects.Count == 0)
        {
            return IInteractable.InteractionState.NotLightning;

        }else if (lightObjects.Count == 1)
        {
            if(lightObjects[0].TryGetComponent(out SpotlightController spotlightControllerSc))
            {
                return spotlightControllerSc.isFocused ? IInteractable.InteractionState.Focused : IInteractable.InteractionState.NotFocused;
            }else
            {
                return IInteractable.InteractionState.LightningObject;
            }
            
        }else
        {
            foreach (var item in lightObjects)
            {
                if(item.TryGetComponent(out SpotlightController spotlightControllerSc))
                {
                    return spotlightControllerSc.isFocused ? IInteractable.InteractionState.BothFocused : IInteractable.InteractionState.BothNotFocused;
                }
            }
        }
        
        return IInteractable.InteractionState.LightningObject;
    }


    public void SetGravity(IInteractable.InteractionState state, Rigidbody rigidbody)
    {
        if(state == IInteractable.InteractionState.Focused || state == IInteractable.InteractionState.BothFocused)
        {
            rigidbody.useGravity = false;
        }else
        {
            rigidbody.useGravity = true;
        }
    }


    public IEnumerator MoveTo(Rigidbody rigidbody, float delay, IInteractable.InteractionState state)
    {
        GameObject spotlight = GameObject.FindGameObjectWithTag("SpotLight");
        bool canMove = false;

        if(state == IInteractable.InteractionState.BothFocused || state == IInteractable.InteractionState.Focused)
        {
            canMove = true;
        }

        if(spotlight.TryGetComponent(out SpotlightController spotlightControllerSc) && canMove)
        {
            Vector3 movePosition;

            while(true)
            {
                movePosition = spotlightControllerSc.lookPosition;
                rigidbody.MovePosition(Vector3.Lerp(rigidbody.position, movePosition, Time.fixedDeltaTime / delay));

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
