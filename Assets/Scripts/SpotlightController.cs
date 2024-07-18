using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class SpotlightController : MonoBehaviour
{
    private Light spotlight;
    internal Vector3 lookPosition;
    internal bool isFocused;
    internal bool isOpened;
    [SerializeField] private FlaslightSpecsSO flaslightSpecsSO;
    [SerializeField] private float carryDistance;
    [SerializeField] private float focusedSpotAngle;
    [SerializeField] private float notFocusedSpotAngle;
    [SerializeField] private float focusedInnerSpotAngle;
    [SerializeField] private float notFocusedInnerSpotAngle;
    [SerializeField] private float focusedIntensity;
    [SerializeField] private float notFocusedIntensity;
    
    void Start()
    {
        spotlight = GetComponent<Light>();
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isOpened = !isOpened;
            flaslightSpecsSO.restartInteractables = true;
        }

        if(Input.GetMouseButtonDown(1))
        {
            isFocused = !isFocused;
            flaslightSpecsSO.restartInteractables = true;
        }

        if(isOpened)
        {
            lookPosition = transform.parent.position + transform.parent.forward * carryDistance;
            transform.LookAt(lookPosition);

            if(isFocused)
            {
                spotlight.spotAngle = focusedSpotAngle;
                spotlight.innerSpotAngle = focusedInnerSpotAngle;

                spotlight.intensity = focusedIntensity;
            }else
            {
                spotlight.spotAngle = notFocusedSpotAngle;
                spotlight.innerSpotAngle = notFocusedInnerSpotAngle;

                spotlight.intensity = notFocusedIntensity;
            }

        }else
        {
            spotlight.intensity = 0;
        }
    }

    /*void ColliderMethod() NOT IN USE
    {
        if(isOpened)
        {
            lookPosition = transform.parent.position + transform.parent.forward * carryDistance;
            transform.LookAt(lookPosition);

            // Işık kaynağından ileri doğru bir ışın oluştur
            Vector3 direction = transform.parent.forward;
            Vector3 startPosition = transform.position;

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 segmentPosition = Vector3.Lerp(startPosition, transform.parent.position + direction * spotlight.range, t);
                float radius = Mathf.Lerp(0, spotlight.spotAngle / 8, t);

                // Engel yoksa OverlapSphere kullanarak objeleri algıla
                Collider[] hitColliders = Physics.OverlapSphere(segmentPosition, radius);
                foreach (Collider hitCollider in hitColliders)
                {
                    float distance = (hitCollider.gameObject.transform.position - transform.position).magnitude;

                    if(distance < spotlight.range && !IsObjectBetween(spotlight.transform.position, hitCollider.gameObject.transform.position, hitCollider.gameObject))
                    {
                        GameObject hitObject = hitCollider.gameObject;
                        //Debug.Log("Işık kaynağı şu obje ile çarpıştı: " + hitObject.name);

                        if(hitObject.TryGetComponent(out IInteractable interactable))
                        {
                            Debug.Log("Interactable found: " + interactable);

                            if(interactableList.Contains(interactable))
                            {
                                //Debug.Log("interactableList already contains: " + interactable);
                            }else
                            {
                                Debug.Log("Added interactableList: " + interactable);
                                interactableList.Add(interactable);
                            }

                            if(interactableListTotal.Contains(interactable))
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
            
            interactableList.Clear();

            if(restartInteractables) // normalden focusa alınca eğer focus değmiyosa bug oluyodu ondan alta aldım elleme amk
            {
                interactableListTotal.Clear();
                restartInteractables = false;
            }

        }else
        {
            if(interactableListTotal.Count > 0)
            {
                foreach(IInteractable interactableSc in interactableListTotal)
                {
                    interactableSc.NotInteract(gameObject);
                }
                interactableListTotal.Clear();
            }
        }
    }*/


    /*bool IsObjectBetween(Vector3 start, Vector3 end, GameObject checkObject) //NOT IN USE
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, distance, ~ignoreRaycast))
        {
            // An object is between the start and end points
            if(hit.collider.gameObject != checkObject)
            {
                //Debug.Log("Arada obje var: " + hit.collider.gameObject.name);
                return true;
            }
        }

        // No object is between the start and end points
        return false;
    }*/
    

    /*private void OnDrawGizmos() //NOT IN USE
    {
        if (spotlight != null && isOpened)
        {
            Gizmos.color = Color.red;

            Vector3 direction = transform.parent.forward;
            Vector3 startPosition = transform.position;

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 segmentPosition = Vector3.Lerp(startPosition, transform.parent.position + direction * spotlight.range, t);
                float radius = Mathf.Lerp(0, spotlight.spotAngle / 8, t);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(segmentPosition, radius);
            }

            // Başlangıçtan bitişe doğru bir çizgi
            Gizmos.DrawLine(startPosition, startPosition + direction * spotlight.range);
        }
    }*/
}
