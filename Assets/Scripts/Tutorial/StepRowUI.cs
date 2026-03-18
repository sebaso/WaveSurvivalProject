using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StepRowUI : MonoBehaviour
{
    [Header("Child References")]
    public Image checkIcon;    // Visible when step.isComplete
    public Image pendingIcon;  // Visible when not complete
    public TMP_Text titleText;
    public TMP_Text keyText;      // e.g. "WASD" or "LMB"

    public void Populate(TutorialStep step, bool isActive, Color activeColor, Color completedColor, Color pendingColor)
    {
        if (titleText != null) titleText.text = step.title;
        if (keyText != null) keyText.text = step.keyHint;

        bool done = step.isComplete;

        if (checkIcon != null) checkIcon.gameObject.SetActive(done);
        if (pendingIcon != null) pendingIcon.gameObject.SetActive(!done);

        // Color the title to show state
        if (titleText != null)
        {
            titleText.color = done ? completedColor
                            : isActive ? activeColor
                            : pendingColor;
        }

        // Optionally fade out completed rows slightly
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = done ? 0.55f : 1f;
    }
}