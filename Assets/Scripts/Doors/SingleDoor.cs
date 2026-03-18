using UnityEngine;

public class SingleDoor : MonoBehaviour, IInteractible
{
    public int price;
    public int interactRange = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CanInteract())
        {
            if (InteractionUI.instance != null)
            {
                InteractionUI.instance.Show(GetInteractText());
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    public void Interact()
    {
        if (ScoreManager.instance.Score >= price)
        {
            if (Tutoriel.instance != null && Tutoriel.instance.CurrentStep != null && Tutoriel.instance.CurrentStep.id == "door") Tutoriel.CompleteStep("door");
            ScoreManager.instance.Score -= price;
            Destroy(gameObject);
        }
    }

    public bool CanInteract()
    {
        if (PlayerController.instance == null) return false;

        return Vector3.Distance(transform.position, PlayerController.instance.transform.position) < interactRange;
    }

    public string GetInteractText()
    {
        return "Press E to open door for " + price.ToString() + " points";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void UpdateHUD()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (PlayerController.instance != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
            Gizmos.DrawRay(transform.position, PlayerController.instance.transform.position - transform.position);
        }
    }
}
