using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : MonoBehaviour, IInteractable
{
    private Light lightComponent;
    private bool isTurnedOn;
    [SerializeField] private List<Color> colorList = new List<Color>();
    [SerializeField] private float maxIntensity;
    [SerializeField] private float changeSpeed;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
        lightComponent.intensity = 0;
    }

    public void OnInteract(Vector3 movePosition, bool isFocused)
    {
        if(!isTurnedOn)
        {
            int randomIndex = Random.Range(0, colorList.Count);

            lightComponent.color = colorList[randomIndex];
            isTurnedOn = true;
        }
        
        lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, maxIntensity, changeSpeed * Time.fixedDeltaTime);
    }


    public void NotInteract(bool isLightning)
    {
        Debug.Log("zortingen");
    }
}
