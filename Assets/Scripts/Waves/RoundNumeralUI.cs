using System.Collections;
using TMPro;
using UnityEngine;

public class RoundNumeralUI : MonoBehaviour
{
    public static RoundNumeralUI instance;

    [Header("UI References")]
    public TextMeshProUGUI roundText;
    
    [Header("Flashing Settings")]
    public Color normalColor = Color.white;
    public Color flashColor = Color.red;
    public float flashSpeed = 5f;
    public float flashDuration = 3f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Updates the text to display the current wave index in Roman Numerals.
    /// </summary>
    public void UpdateRound(int waveIndex)
    {
        if (roundText != null)
        {
            roundText.text = ToRoman(waveIndex);
            roundText.color = normalColor; // Ensure it's back to normal color if it was flashing
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }
        }
    }

    /// <summary>
    /// Starts flashing the round text to signify the end of the wave.
    /// </summary>
    public void FlashWaveEnd()
    {
        if (roundText == null) return;
        
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
            
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            // Sine wave ping-pong between 0 and 1
            float lerp = (Mathf.Sin(timer * flashSpeed) + 1f) / 2f; 
            roundText.color = Color.Lerp(normalColor, flashColor, lerp);
            yield return null;
        }

        roundText.color = normalColor;
        flashCoroutine = null;
    }

    /// <summary>
    /// Converts an integer to a Roman numeral string.
    /// </summary>
    private string ToRoman(int number)
    {
        if ((number <= 0) || (number > 3999)) return string.Empty;

        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        
        return string.Empty;
    }
}
