using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoScriptsController : MonoBehaviour
{
    [SerializeField] private Hashtable hashtable = new Hashtable();
    [SerializeField] private GameObject mainObject;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject mainCam;
    private TMP_Text text;


    void Start()
    {
        mainCam = Camera.main.gameObject;
        text = GetComponent<TMP_Text>();
        
        transform.position = mainObject.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainObject.transform.position + offset + new Vector3(0, mainObject.transform.localScale.y / 2, 0);
        
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position); // Textlerin forward vektörleri tersmiş amk
    
        if(mainObject.TryGetComponent(out ScaleObject scaleObjectSc))
        {
            text.text = "Object Type: Scale" + "\n" +
                        "State: " + scaleObjectSc.State + "\n" +
                        "Lightener Count: " + scaleObjectSc.lightObjects.Count;
        }else if(mainObject.TryGetComponent(out LightObject lightObjectSc))
        {
            text.text = "Object Type: Light" + "\n" +
                        "State: " + lightObjectSc.State + "\n" +
                        "Lightener Count: " + lightObjectSc.lightObjects.Count + "\n" +
                        "Bright Enough: " + lightObjectSc.isBrightEnough;
        }else if(mainObject.TryGetComponent(out MirrorObject mirrorObjectSc))
        {
            text.text = "Object Type: Mirror" + "\n" +
                        "State: " + mirrorObjectSc.State + "\n" +
                        "Lightener Count: " + mirrorObjectSc.lightObjects.Count;
        }
    }
}
