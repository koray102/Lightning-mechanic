using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SpotlightController : MonoBehaviour
{
    private Light spotlight;
    private float distance;
    private Vector3 lookPosition;
    private List<IInteractable> interactableListTotal = new List<IInteractable>();
    [SerializeField] private float carryDistance;
    [SerializeField] private int segments = 10;
    [SerializeField] private LayerMask ignoreRaycast;
    [SerializeField] private bool isFocused;
    [SerializeField] private float focusedSpotAngle;
    [SerializeField] private float notFocusedSpotAngle;
    [SerializeField] private float focusedInnerSpotAngle;
    [SerializeField] private float notFocusedInnerSpotAngle;
    [SerializeField] private float focusedIntensity;
    [SerializeField] private float notFocusedIntensity;
    

    void Start()
    {
        spotlight = gameObject.GetComponent<Light>();
    }


    void Update()
    {
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
    }

    void FixedUpdate()
    {
        // Işık kaynağından ileri doğru bir ışın oluştur
        Vector3 direction = spotlight.transform.forward;
        Vector3 startPosition = spotlight.transform.position;
        List<IInteractable> interactableList = new List<IInteractable>();

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 segmentPosition = Vector3.Lerp(startPosition, startPosition + direction * spotlight.range, t);
            float radius = Mathf.Lerp(0, spotlight.spotAngle / 8, t);

            // Engel yoksa OverlapSphere kullanarak objeleri algıla
            Collider[] hitColliders = Physics.OverlapSphere(segmentPosition, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                distance = (hitCollider.gameObject.transform.position - spotlight.transform.position).magnitude;

                if(distance < spotlight.range && !IsObjectBetween(spotlight.transform.position, hitCollider.gameObject.transform.position, hitCollider.gameObject))
                {
                    GameObject hitObject = hitCollider.gameObject;
                    //Debug.Log("Işık kaynağı şu obje ile çarpıştı: " + hitObject.name);

                    if(hitObject.TryGetComponent(out IInteractable interactable))
                    {
                        if(interactableList.Contains(interactable))
                        {

                        }else
                        {
                            Debug.Log("interactableList: " + interactable);
                            interactableList.Add(interactable);
                        }


                        if(interactableListTotal.Contains(interactable))
                        {

                        }else
                        {
                            Debug.Log("interactableListTotal: " + interactable);
                            interactableListTotal.Add(interactable);
                        }


                        lookPosition = transform.position + transform.forward * carryDistance;
                        interactable.OnInteract(lookPosition, isFocused);
                    }
                }
            }
        }

        
        foreach(IInteractable interactableSc in interactableListTotal)
        {
            if(interactableList.Contains(interactableSc))
            {

            }else
            {
                Debug.Log("çıkıcam, " + interactableSc + ", " + interactableList.Count);
                interactableSc.NotInteract();
                //interactableListTotal.Remove(interactableSc);
            }
        }
    }


    // SphereCast gizmoslarını çizmek için
    private void OnDrawGizmos()
    {
        if (spotlight != null)
        {
            Gizmos.color = Color.red;

            Vector3 direction = spotlight.transform.forward;
            Vector3 startPosition = spotlight.transform.position;

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector3 segmentPosition = Vector3.Lerp(startPosition, startPosition + direction * spotlight.range, t);
                float radius = Mathf.Lerp(0, spotlight.spotAngle / 8, t);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(segmentPosition, radius);
            }

            // Başlangıçtan bitişe doğru bir çizgi
            Gizmos.DrawLine(startPosition, startPosition + direction * spotlight.range);
        }
    }


    bool IsObjectBetween(Vector3 start, Vector3 end, GameObject checkObject)
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
    }
}
