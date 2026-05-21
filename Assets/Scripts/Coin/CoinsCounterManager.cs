using UnityEngine;

public class CoinsCounterManager : MonoBehaviour
{
    public static CoinsCounterManager Instance { get; private set; }

    private int totalCoinsInLevel;
    private int collectedCoins;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    

    // 👉 РЕГИСТРАЦИЯ монеты (важно!)
    public void RegisterCoin()
    {
        totalCoinsInLevel++;
        UpdateCoinUI();
        Debug.Log($"Монета зарегистрирована. Всего: {totalCoinsInLevel}");
    }

    // 👉 СБОР монеты
    public void CollectCoin()
    {
        collectedCoins++;
        UpdateCoinUI();

        if (collectedCoins >= totalCoinsInLevel)
        {
            Debug.Log("Все монеты собраны!");
        }
    }

    private void UpdateCoinUI()
    {
        Debug.Log($"Собрано: {collectedCoins}/{totalCoinsInLevel}");
    }
}
