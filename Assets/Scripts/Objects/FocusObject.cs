using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FocusObject : BasicObjectBehaviour, IInteractable
{
    // Start is called before the first frame update
    private Rigidbody rb;
    private List<GameObject> lightObjects = new List<GameObject>();
    private Coroutine moveToCoroutine;
    private IInteractable.InteractionState state;
    private Vector3 movePosLerp;
    private GameObject spotlight;
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


    public void OnInteract(GameObject lightener)
    {
        State = SetState(lightener, true, lightObjects);
        SetGravity(State, rb);

        // Taşınabilirlik ayarlama
        if(moveToCoroutine != null)
        {
            StopCoroutine(moveToCoroutine);
        }
        moveToCoroutine = StartCoroutine(MoveTo(rb, grabDelay, State));
    }

    public void NotInteract(GameObject lightener)
    {
        State = SetState(lightener, false, lightObjects);
        SetGravity(State, rb);

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

    public IInteractable.InteractionState State
    {
        get { return state; }
        set { state = value; }
    }
}
