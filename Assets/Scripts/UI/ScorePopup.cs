using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private float disappearTimer;
    public float disappearTimerMax = 1f;
    private Vector3 moveVector;
    private Color textColor;

    [SerializeField] private float gravity = 500f; //"gravedad"
    [SerializeField] private float initialVerticalSpeed = 200f;
    [SerializeField] private float horizontalSpeedRange = 200f;

    void Update()
    {
        moveVector.y -= gravity * Time.deltaTime;
        transform.position += moveVector * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float fadeSpeed = 10f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            scoreText.color = textColor;

            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Setup(int scoreAmount)
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        scoreText.SetText("+ " + scoreAmount.ToString());

        textColor = scoreText.color;
        disappearTimer = disappearTimerMax;

        float horizontalSpeed = Random.Range(horizontalSpeedRange * 0.8f, horizontalSpeedRange * 1.2f);
        float verticalVariation = Random.Range(-initialVerticalSpeed * 0.2f, initialVerticalSpeed * 0.2f);

        moveVector = new Vector3(-horizontalSpeed, verticalVariation, 0);
    }
}
