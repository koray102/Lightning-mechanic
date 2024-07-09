using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : BasicObjectBehaviour, IInteractable
{
    private Rigidbody rb;
    private Vector3 originalScale;
    private IInteractable.InteractionState state;
    private Coroutine scaleCoroutine;
    private Coroutine moveToCoroutine;
    private List<GameObject> lightObjects = new List<GameObject>();
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private float growDuration = 3f;
    [SerializeField] private float shrinkDuration = 5f;
    [SerializeField] private float grabDelay = 3;

    private void Awake()
    {
        State = IInteractable.InteractionState.NotLightning;

        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }


    public void OnInteract(Vector3 movePosition, GameObject lightener)
    {
        //Debug.Log("OnInteract (scale)");

        State = SetState(lightener, true, lightObjects);
        SetGravity(State, rb);

        // Taşınabilirlik ayarlama
        if(moveToCoroutine != null)
        {
            StopCoroutine(moveToCoroutine);
        }
        moveToCoroutine = StartCoroutine(MoveTo(rb, grabDelay, State));

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
        Debug.Log("NotInteract (scale)");
        
        State = SetState(lightener, false, lightObjects);
        SetGravity(State, rb);

        // Taşınabilirlik ayarlama
        if(moveToCoroutine != null && State != IInteractable.InteractionState.BothFocused && State != IInteractable.InteractionState.Focused)
        {
            StopCoroutine(moveToCoroutine);
        }

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
        Debug.Log(calculatedDuration);

        while (time < calculatedDuration)
        {
            //Debug.Log("scale");
                
            scaleModifier = Vector3.Lerp(startScale, targetScale, time / calculatedDuration);
            transform.localScale = scaleModifier;
            time += Time.deltaTime;

            yield return null;
        }

        transform.localScale = targetScale;
    }


    public IInteractable.InteractionState State
    {
        get { return state; }
        set { state = value; }
    }

}
