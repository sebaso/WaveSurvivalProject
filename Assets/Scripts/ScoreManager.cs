using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField]
    private int _score;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            UpdateScore();
        }
    }

    public TextMeshProUGUI scoreDisplay;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        UpdateScore();
    }

    public void AddScore(int amount)
    {
        this.Score += amount;
    }

    public void UpdateScore()
    {
        scoreDisplay.text = Score.ToString();
    }
}
