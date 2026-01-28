using UnityEngine;

public interface IInteractible
{
    void Interact();
    bool CanInteract();
    string GetInteractText();
    Transform GetTransform();


}
