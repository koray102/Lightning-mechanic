using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpotlightRaycast : MonoBehaviour
{
    [SerializeField] private int rayCount = 10; // Atılacak raycast sayısı
    [SerializeField] private float notFocusedRadius = 5f; // Çemberin yarıçapı
    [SerializeField] private float focusedRadius = 2f; // Çemberin yarıçapı
    [SerializeField] private float distance = 10f; // Çemberin merkeziyle ışık kaynağı arasındaki mesafe
    [SerializeField] private GameObject denemelik;
    [SerializeField] private int segmentNumber;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private FlaslightSpecsSO flaslightSpecsSO;
    private SpotlightController spotlightControllerSc;
    private bool isFocused;
    private bool isOpened;
    private bool restartInteractables;
    private List<IInteractable> interactableListTotal = new List<IInteractable>();
    private List<IInteractable> interactableList = new List<IInteractable>();


    private void Start()
    {
        spotlightControllerSc = GetComponent<SpotlightController>();
    }

    private void Update()
    {
        isOpened = spotlightControllerSc.isOpened;
        isFocused = spotlightControllerSc.isFocused;
        restartInteractables = flaslightSpecsSO.restartInteractables;

        if(isOpened)
        {
            if(isFocused)
            {
                ConicalRaycasts(focusedRadius);
            }else
            {
                ConicalRaycasts(notFocusedRadius);
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
    }


    void ConicalRaycasts(float radius)
    {
        Vector3 position = transform.position;
        //Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Vector3 center = transform.position + transform.forward * distance; // Çemberin merkezi
        denemelik.transform.position = center;
        Transform denemelikTransform = denemelik.transform;

        for (int j = 0; j < segmentNumber; j++)
        {
            float radiusCalc = radius * ((j + 1) / (float)segmentNumber);
            float rayCountCalc = rayCount * radiusCalc / radius;


            for (int i = 0; i < rayCountCalc; i++)
            {
                float angle = i * Mathf.PI * 2f / rayCountCalc;
                Vector3 direction2 = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radiusCalc;
                Vector3 direction = (center + denemelikTransform.TransformDirection(direction2) - position).normalized;

                RaycastHit[] hits = new RaycastHit[5];
                int hitCount = Physics.RaycastNonAlloc(transform.position, direction, hits, distance, layerMask);
                //Debug.DrawRay(transform.position, direction * distance, Color.green);

                if(hitCount > 0)
                {
                    System.Array.Sort(hits, 0, hitCount, new RaycastHitComparer());
                    
                    RaycastHit hit = hits[0];
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                    if (hit.collider != null && hit.collider.TryGetComponent(out IInteractable interactable))
                    {
                        if (!interactableList.Contains(interactable))
                        {
                            interactableList.Add(interactable);
                        }

                        if (interactableListTotal.Contains(interactable))
                        {
                            if (interactable.State == IInteractable.InteractionState.NotLightning)
                            {
                                interactable.OnInteract(gameObject);
                            }
                        }
                        else
                        {
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
            flaslightSpecsSO.restartInteractables = false;
        }
    }

    // Bunun hakkında gram fikrim yok
    class RaycastHitComparer : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit x, RaycastHit y)
        {
            return x.distance.CompareTo(y.distance);
        }
    }
}
