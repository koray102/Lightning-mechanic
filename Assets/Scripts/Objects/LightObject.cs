using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LightObject : BasicObjectBehaviour, IInteractable
{
    private Rigidbody rb;
    private Light lightComponent;
    private IInteractable.InteractionState state;
    private bool isColorPicked;
    internal bool isBrightEnough;
    private Coroutine volumeCoroutine;    internal List<GameObject> lightObjects = new List<GameObject>();
    private List<IInteractable> interactableList = new List<IInteractable>();
    private List<IInteractable> interactableListTotal = new List<IInteractable>();
    [SerializeField] private List<Color> colorList = new List<Color>();
    [SerializeField] private float effectRadius;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float changeDuration;
    [SerializeField] private float grabDelay = 3;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
        lightComponent.intensity = 0;
        State = IInteractable.InteractionState.NotLightning;

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if(isBrightEnough)
        {
            interactableList.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;

                if(hitObject != gameObject)
                {
                    if(hitObject.TryGetComponent(out IInteractable interactable))
                    {
                        if(interactableList.Contains(interactable))
                        {
                            //Debug.Log("interactableList already contains: " + interactable);
                        }else
                        {
                            //Debug.Log("Added interactableList: " + interactable);
                            interactableList.Add(interactable);
                        }

                        if(interactableListTotal.Contains(interactable) )
                        {
                            //Debug.Log("interactableListTotal already contains: " + interactable);

                            if (interactable.State == IInteractable.InteractionState.NotLightning)
                            {
                                interactable.OnInteract(gameObject);
                            }

                        }else
                        {
                            //Debug.Log("Added interactableListTotal: " + interactable);
                            interactableListTotal.Add(interactable);

                            interactable.OnInteract(gameObject);
                        }
                    }
                }
            }

            for (int i = interactableListTotal.Count - 1; i >= 0; i--)
            {
                IInteractable interactableSc = interactableListTotal[i];
                
                if (interactableList.Contains(interactableSc))
                {
                    //Debug.Log("Still inside: " + interactableSc);
                }
                else
                {
                    //Debug.Log("Exit: " + interactableSc);
                    interactableSc.NotInteract(gameObject);
                    interactableListTotal.RemoveAt(i);
                }
            }
        }
    }

    
    private void FixedUpdate()
    {
        MoveTo(rb, grabDelay, State);
    }

    public void OnInteract(GameObject lightener)
    {
        State = SetState(lightener, true, lightObjects);
        //SetGravity(State, rb);

        if(!isColorPicked)
        {
            int randomIndex = Random.Range(0, colorList.Count);

            lightComponent.color = colorList[randomIndex];
            isColorPicked = true;
        }
        
        if(volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }

        volumeCoroutine = StartCoroutine(IncreaseLightVolume(maxIntensity, changeDuration));
    }


    public void NotInteract(GameObject lightener)
    {
        State = SetState(lightener, false, lightObjects);
        //SetGravity(State, rb);

        if(volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }
    }

    private IEnumerator IncreaseLightVolume(float targetVolume, float duration)
    {
        float time = 0;
        float volumeModifier;
        float startVolume = lightComponent.intensity;

        float calculatedDuration = duration * Mathf.Abs(targetVolume - startVolume) / targetVolume;

        while (time < calculatedDuration)
        {
            //Debug.Log("Volume");
                
            volumeModifier = Mathf.Lerp(startVolume, targetVolume, time / calculatedDuration);
            lightComponent.intensity = volumeModifier;
            time += Time.deltaTime;

            yield return null;
        }

        isBrightEnough = true;
        lightComponent.intensity = targetVolume;
    }

    private void OnDrawGizmos()
    {
        if (isBrightEnough)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(transform.position, effectRadius);
        }
    }

    public IInteractable.InteractionState State
    {
        get { return state; }
        set { state = value; }
    }

}
