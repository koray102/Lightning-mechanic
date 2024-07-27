using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScaleObject : BasicObjectBehaviour, IInteractable
{
    private WaitForFixedUpdate waitFixedTime = new WaitForFixedUpdate();
    private Rigidbody rb;
    private Vector3 originalScale;
    private IInteractable.InteractionState state;
    private Coroutine scaleCoroutine;
    internal List<GameObject> lightObjects = new List<GameObject>();
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private float growDuration = 3f;
    [SerializeField] private float shrinkDuration = 5f;
    [SerializeField] private float grabDelay = 3;

    private void Awake()
    {
        State = IInteractable.InteractionState.NotLightning;

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        MoveTo(rb, grabDelay, State);
    }


    public void OnInteract(GameObject lightener)
    {
        //Debug.Log("OnInteract (scale)");

        State = SetState(lightener, true, lightObjects);
        //SetGravity(State, rb);

        // Büyüme küçülme ayarlama 
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        if(State == IInteractable.InteractionState.Focused)
        {
            scaleCoroutine = StartCoroutine(Scale(originalScale, shrinkDuration));
            
        }else
        {
            // Obje maksimum boyuta kadar büyüyor
            scaleCoroutine = StartCoroutine(Scale(maxScale, growDuration));
        }
    }

    public void NotInteract(GameObject lightener)
    {
        //Debug.Log("NotInteract (scale)");
        
        State = SetState(lightener, false, lightObjects);
        //SetGravity(State, rb);

        // Büyüme küçülme ayarlama
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        if(State == IInteractable.InteractionState.NotLightning || State == IInteractable.InteractionState.Focused)
        {
            scaleCoroutine = StartCoroutine(Scale(originalScale, shrinkDuration));
        }else
        {
            scaleCoroutine = StartCoroutine(Scale(maxScale, growDuration));
        }
    }

    private IEnumerator Scale(Vector3 targetScale, float duration)
    {
        float time = 0;
        Vector3 scaleModifier;
        Vector3 startScale = transform.localScale;

        float calculatedDuration = duration * Mathf.Abs(targetScale.magnitude - startScale.magnitude) / targetScale.magnitude;

        while (time < calculatedDuration)
        {
            //Debug.Log("scale");
                
            scaleModifier = Vector3.Lerp(startScale, targetScale, time / calculatedDuration);
            transform.localScale = scaleModifier;
            time += Time.fixedDeltaTime;

            yield return waitFixedTime;
        }

        transform.localScale = targetScale;
    }


    public IInteractable.InteractionState State
    {
        get { return state; }
        set { state = value; }
    }

}
