using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void OnInteract(Vector3 movePosition, bool isFocused);
    public void NotInteract();
}
