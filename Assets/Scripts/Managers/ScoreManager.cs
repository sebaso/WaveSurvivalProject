using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] private int _targetScore;
    [SerializeField] private int _displayedScore;
    [SerializeField] private int _scoreStep = 1; // Amount to increment per frame/step
    public GameObject scorePopupPrefab;
    public TextMeshProUGUI scoreDisplay;
    public Transform scorePopupSpawnPoint;
    public Transform canvasTransform;

    public int Score
    {
        get => _targetScore;
        set => _targetScore = value;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _displayedScore = _targetScore;
        UpdateUI();
    }

    void Update()
    {
        if (_displayedScore != _targetScore)
        {
            if (_displayedScore < _targetScore)
            {
                _displayedScore += _scoreStep;
                if (_displayedScore > _targetScore) _displayedScore = _targetScore;
            }
            else if (_displayedScore > _targetScore)
            {
                _displayedScore -= _scoreStep;
                if (_displayedScore < _targetScore) _displayedScore = _targetScore;
            }

            UpdateUI();
        }
    }

    public void AddScore(int amount)
    {
        _targetScore += amount;
        SpawnScorePopup(amount);

    }

    public void SpawnScorePopup(int scoreAmount)
    {
        GameObject scorePopup = Instantiate(scorePopupPrefab, scorePopupSpawnPoint.position, Quaternion.identity);
        scorePopup.transform.SetParent(canvasTransform);
        TextMeshProUGUI text = scorePopup.GetComponent<TextMeshProUGUI>();
        text.text = scoreAmount.ToString();
        text.color = Color.yellow;
        scorePopup.GetComponent<ScorePopup>().Setup(scoreAmount);
    }
    private void UpdateUI()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = _displayedScore.ToString();
        }
    }
}
