using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    enum InteractionState { Focused, NotFocused, LightningObject, BothFocused, BothNotFocused, NotLightning };
    InteractionState State { get; set; }
    public void OnInteract(Vector3 movePosition, GameObject lightener);
    public void NotInteract(GameObject lightener);
}
