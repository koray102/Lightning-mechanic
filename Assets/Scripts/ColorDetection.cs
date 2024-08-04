using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDetection : MonoBehaviour
{
    [SerializeField] private GameObject totalColorObject;
    [SerializeField] private GameObject targetColorObject;
    [SerializeField] private float colorObjectMaxIntensity;
    private Light totalColorLight;
    private Color color;
    private Light objectLight;
    private float similarity;
    private float redMultiplier = 0;
    private float greenMultiplier = 0;
    private float blueMultiplier = 0;

    void Start()
    {
        totalColorLight = totalColorObject.GetComponent<Light>();
        color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        similarity = MeasureTheSimilarity(totalColorObject, targetColorObject);
        Debug.Log("Similarity: %" + similarity);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out IInteractable interactable))
        {
            if(other.TryGetComponent(out LightObject lightObjectSc))
            {
                objectLight = lightObjectSc.GetComponent<Light>();

                if(lightObjectSc.currentLightType == LightObject.lightType.Red)
                {
                    Debug.Log("Red");
                    redMultiplier = objectLight.intensity / colorObjectMaxIntensity;

                }else if(lightObjectSc.currentLightType == LightObject.lightType.Green)
                {
                    Debug.Log("green");
                    greenMultiplier = objectLight.intensity / colorObjectMaxIntensity;

                }else if(lightObjectSc.currentLightType == LightObject.lightType.Blue)
                {
                    Debug.Log("blue");
                    blueMultiplier = objectLight.intensity / colorObjectMaxIntensity;

                }

                SetColor(redMultiplier, greenMultiplier, blueMultiplier);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out IInteractable interactable))
        {
            if(other.TryGetComponent(out LightObject lightObjectSc))
            {
                objectLight = lightObjectSc.GetComponent<Light>();

                if(lightObjectSc.currentLightType == LightObject.lightType.Red)
                {
                    redMultiplier = 0;

                }else if(lightObjectSc.currentLightType == LightObject.lightType.Green)
                {
                    greenMultiplier = 0;

                }else if(lightObjectSc.currentLightType == LightObject.lightType.Blue)
                {
                    blueMultiplier = 0;

                }

                SetColor(redMultiplier, greenMultiplier, blueMultiplier);
            }
        }
    }

    private void SetColor(float red, float green, float blue)
    {
        color.r = red;
        color.g = green;
        color.b = blue;

        totalColorLight.color = color;
    }


    private float MeasureTheSimilarity(GameObject totalLightObj, GameObject targetLightObj)
    {
        Light totalLight = totalLightObj.GetComponent<Light>();
        Light targetLight = targetLightObj.GetComponent<Light>();
        Color totalColor = totalLight.color;
        Color targetColor = targetLight.color;
        float distance;

        distance = Mathf.Pow((totalColor.r - targetColor.r), 2) + Mathf.Pow((totalColor.g - targetColor.g), 2) + Mathf.Pow((totalColor.b - targetColor.b), 2);

        return 100 * distance / 3;
    }
}
