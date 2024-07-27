using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicObjectBehaviour : MonoBehaviour
{
    private WaitForFixedUpdate waitFixedTime = new WaitForFixedUpdate();

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


    public void MoveTo(Rigidbody rigidbody, float delay, IInteractable.InteractionState state)
    {
        GameObject spotlight = GameObject.FindGameObjectWithTag("SpotLight");
        bool canMove;

        if(state == IInteractable.InteractionState.BothFocused || state == IInteractable.InteractionState.Focused)
        {
            canMove = true;
        }else
        {
            canMove = false;
        }

        if(spotlight.TryGetComponent(out SpotlightController spotlightControllerSc) && canMove)
        {
            Vector3 movePosition;
            Vector3 direction;
            float speed;
            Vector3 randomRotation = new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f)
            );

            movePosition = spotlightControllerSc.lookPosition;
            direction = (movePosition - rigidbody.position).normalized;
            speed = Vector3.Distance(rigidbody.position, movePosition) / delay;
                
            rigidbody.velocity = direction * speed;

            if (rigidbody.velocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rigidbody.velocity);
                rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, targetRotation, 0.02f * speed));
            }

            rigidbody.rotation *= Quaternion.Euler(randomRotation * 3);
        }
    }
}
