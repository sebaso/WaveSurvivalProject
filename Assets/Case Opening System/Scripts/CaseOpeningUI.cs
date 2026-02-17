using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaseOpeningUI : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [Header("Roll Settings")]
    [SerializeField] private int _cellCount = 40;
    [SerializeField] private int _winningCellOffset = 4;

    [Header("UI References")]
    [SerializeField] private GameObject _rollerPanel;
    [SerializeField] private Transform _rollerContent;
    [SerializeField] private GameObject _cellPrefab;

    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private float _rollDuration = 5f;
    [SerializeField] private float _snapDuration = 1f;

    public GameObject bandagePrefab;

    public Item keyItem;
    public CaseItem caseItem;
    public Transform spawnPoint;
    public Item chosenItem;

    private RectTransform _rollerRect;
    private Vector2 _initialRollerPosition;

    private float _startPositionX;
    private float _maxSpeed;
    private float _elapsedRollTime;
    private float _finalTargetX;
    private float _randomizedTargetX;
    private Transform player;
    public int interactionDistance = 2;

    private bool _isRolling;

    private void Start()
    {
        _rollerRect = _rollerContent.GetComponent<RectTransform>();
        _initialRollerPosition = _rollerRect.localPosition;
    }

    public bool CanInteract()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= interactionDistance)
            {
                return true;
            }
        }
        return false;
    }
    private void Update()
    {
        UpdateRolling();
        if (Input.GetKeyDown(KeyCode.Space) && !_isRolling)
        {
            if (CanInteract())
            {
                if (InteractionUI.instance != null)
                {
                    InteractionUI.instance.Show("Press Space to open case");
                }
                Item drop = CaseDropSystem.GetRandomDrop(caseItem);
                chosenItem = drop;
                BeginRoll(caseItem, drop);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _isRolling)
        {
            StopAndCloseRoller();
        }
    }

    public void BeginRoll(CaseItem caseItem, Item guaranteedDrop)
    {
        _rollerPanel.SetActive(true);

        for (int i = 0; i < _cellCount; i++)
        {
            RollerCell cell = Instantiate(_cellPrefab, _rollerContent).GetComponent<RollerCell>();

            if (i == _cellCount - _winningCellOffset - 1)
            {
                cell.Refresh(guaranteedDrop);
            }
            else
            {
                Item randomDrop = CaseDropSystem.GetRandomDrop(caseItem);
                cell.Refresh(randomDrop);
            }
        }

        float cellWidth = _cellPrefab.GetComponent<RectTransform>().rect.width;
        float spacing = _rollerContent.GetComponent<HorizontalLayoutGroup>().spacing;
        _ = _rollerContent.parent.GetComponent<RectTransform>().rect.width / 2f;

        float slotWidth = cellWidth + spacing;
        float targetSlotPosition = slotWidth * (_cellCount - _winningCellOffset);
        float offset = cellWidth / 2f + spacing;

        _finalTargetX = -(targetSlotPosition - offset);

        float randomOffset = Random.Range(-cellWidth / 2f, cellWidth / 2f);
        _randomizedTargetX = _finalTargetX + randomOffset;

        _startPositionX = _rollerContent.localPosition.x;
        float totalDistance = Mathf.Abs(_randomizedTargetX - _startPositionX);

        _maxSpeed = CalculateInitialSpeed(_speedCurve, totalDistance, _rollDuration);
        _elapsedRollTime = 0f;
        _isRolling = true;
    }

    private void UpdateRolling()
    {
        if (!_isRolling)
        {
            return;
        }

        _elapsedRollTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_elapsedRollTime / _rollDuration);

        float curveValue = _speedCurve.Evaluate(normalizedTime);
        float currentSpeed = _maxSpeed * curveValue;

        float newX = _rollerRect.localPosition.x - currentSpeed * Time.deltaTime;
        _rollerRect.localPosition = new Vector3(newX, _rollerRect.localPosition.y, 0f);

        if (_elapsedRollTime >= _rollDuration)
        {
            _isRolling = false;
            StartCoroutine(SmoothSnapToTarget());
        }
    }

    private IEnumerator SmoothSnapToTarget()
    {
        float startX = _rollerContent.localPosition.x;
        float elapsed = 0f;

        while (elapsed < _snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _snapDuration);

            float newX = Mathf.Lerp(startX, _finalTargetX, Mathf.SmoothStep(0f, 1f, t));
            _rollerRect.localPosition = new Vector3(newX, _rollerRect.localPosition.y, 0f);

            yield return null;
        }

        _rollerRect.localPosition = new Vector3(_finalTargetX, _rollerRect.localPosition.y, 0f);

        yield return _waitForSeconds0_5;
        print(chosenItem.Name);
        if (chosenItem != null && chosenItem.Name == "Bandage")
        {
            Instantiate(bandagePrefab, spawnPoint.position, spawnPoint.rotation);
        }
        if (chosenItem != null && chosenItem.Name == "Soulslop")
        {
            Instantiate(bandagePrefab, spawnPoint.position, spawnPoint.rotation);
            Instantiate(bandagePrefab, spawnPoint.position, spawnPoint.rotation);
        }
        StopAndCloseRoller();
    }

    private float CalculateInitialSpeed(AnimationCurve curve, float distance, float time, int steps = 1000)
    {
        float integral = 0f;
        float step = 1f / steps;

        for (int i = 0; i < steps; i++)
        {
            float u1 = i * step;
            float u2 = (i + 1) * step;

            float y1 = curve.Evaluate(u1);
            float y2 = curve.Evaluate(u2);

            integral += (y1 + y2) * 0.5f * step;
        }

        return distance / (time * integral);
    }

    public void StopAndCloseRoller()
    {
        _rollerPanel.SetActive(false);

        foreach (Transform child in _rollerContent)
        {
            Destroy(child.gameObject);
        }

        StopAllCoroutines();
        _isRolling = false;

        _rollerContent.localPosition = _initialRollerPosition;
    }
}