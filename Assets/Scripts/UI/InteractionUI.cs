using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI instance;
    public TextMeshProUGUI interactionText;

    private bool requestReceived;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (interactionText == null)
        {
            GameObject textObj = GameObject.FindGameObjectWithTag("MainText");
            if (textObj != null)
            {
                interactionText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void LateUpdate()
    {
        if (interactionText == null) return;

        if (requestReceived)
        {
            if (!interactionText.enabled)
                interactionText.enabled = true;

            // Reset for next frame
            requestReceived = false;
        }
        else
        {
            if (interactionText.enabled)
                interactionText.enabled = false;
        }
    }

    public void Show(string text)
    {
        if (interactionText == null) return;
        interactionText.text = text;
        requestReceived = true;
    }
    public void Hide()
    {
        if (interactionText == null) return;
        interactionText.text = "";
        requestReceived = false;
    }
}
