using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class RewardManager : MonoBehaviour
{
    public Canvas effectsCanvas;
    public event Action OnTextUpdate;
    public static RewardManager Instance;
    public Image coinPrefab;
    public TMP_Text scoreText;

    [Header("Settings")]
    public Vector2 rewardCoinRange = new Vector2(5, 10);
    public float spawnRadius = 300f;
    public Vector2 delayRangeBetweenCoins = new Vector2(0, 0.1f);
    public float coinMovementSpeed;
    public float spawnTime = 1;
    public float targetMovementTime = 0.5f;

    private List<Image> coins;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        coins = new List<Image>();
    }

    public void GenerateCareReward()
    {
        StartCoroutine(ThrowCoinsCoroutine((int)Random.Range(rewardCoinRange.x, rewardCoinRange.y)));
    }
    public void GenerateCareLargeReward(int? reward = null)
    {
        if (reward == null)
        {
            reward = (int)Random.Range(rewardCoinRange.x, rewardCoinRange.y) * 2;
        }
        StartCoroutine(ThrowCoinsCoroutine(reward.Value));
    }

    private IEnumerator ThrowCoinsCoroutine(int numberOfCoins)
    {
        for (var i = 0; i < numberOfCoins; i++)
        {
            var delay = Random.Range(delayRangeBetweenCoins.x, delayRangeBetweenCoins.y);
            StartCoroutine(SpawnCoinAndMoveRoutine(delay));
        }

        yield return new WaitForSeconds(spawnTime + 0.5f);

        foreach (var coin in coins)
        {
            var delay = Random.Range(delayRangeBetweenCoins.x, delayRangeBetweenCoins.y);
            StartCoroutine(MoveToTextTarget(coin, delay));
        }

        yield return new WaitForSeconds(targetMovementTime);

        foreach(var coin in coins)
        {
            Destroy(coin.gameObject);
        }
        coins.Clear();
    }

    private IEnumerator SpawnCoinAndMoveRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        var coin = Instantiate(coinPrefab, effectsCanvas.transform);
        coin.rectTransform.anchoredPosition = Vector2.zero;
        coins.Add(coin);

        var randomDirection = Camera.main.WorldToScreenPoint(Random.insideUnitCircle * spawnRadius);
        var movementTime = spawnTime - delay;
        StartCoroutine(
            MovementHelper.MoveObjectToTargetRoutine(coin.rectTransform, randomDirection, movementTime));
    }

    private IEnumerator MoveToTextTarget(Image coin, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return MovementHelper.MoveObjectToTargetRoutine(coin.rectTransform, scoreText.rectTransform.position, targetMovementTime - delay);
        MoneyMapper.Money += 1;
        OnTextUpdate?.Invoke();
    }
}