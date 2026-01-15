using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField]
    private int _score;

    public float textLerpSpeed = 1f;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            SmoothScore();
        }
    }

    public TextMeshProUGUI scoreDisplay;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        UpdateScore();
    }

    void SmoothScore()
    {
        scoreDisplay.text = Mathf.Lerp(float.Parse(scoreDisplay.text), Score, textLerpSpeed * Time.deltaTime).ToString();
    }

    public void AddScore(int amount)
    {
        this.Score += amount;
        UpdateScore();

    }

    public void UpdateScore()
    {
        scoreDisplay.text = Score.ToString();
    }
}
