using UnityEngine;

/// <summary>
/// Растение с системой роста
/// </summary>
public class Plant : MonoBehaviour
{
    [Header("Настройки роста")]
    [SerializeField] private float growthTime = 30f; // Время роста в секундах
    [SerializeField] private int maxGrowthStage = 3; // Максимальная стадия роста
    
    [Header("Визуальные стадии")]
    [SerializeField] private GameObject[] growthStages; // Модели для каждой стадии
    
    [Header("Настройки полива")]
    [SerializeField] private bool needsWater = true;
    [SerializeField] private float waterEffectDuration = 5f;
    
    private int currentStage = 0;
    private float growthTimer = 0f;
    private bool hasWater = false;
    private float waterTimer = 0f;
    
    private void Start()
    {
        UpdateVisuals();
    }
    
    private void Update()
    {
        // Обновляем таймер воды
        if (hasWater)
        {
            waterTimer += Time.deltaTime;
            if (waterTimer >= waterEffectDuration)
            {
                hasWater = false;
                waterTimer = 0f;
            }
        }
        
        // Рост растения
        if (currentStage < maxGrowthStage)
        {
            // Растет только если есть вода или вода не нужна
            if (hasWater || !needsWater)
            {
                growthTimer += Time.deltaTime;
                
                if (growthTimer >= growthTime)
                {
                    Grow();
                    growthTimer = 0f;
                }
            }
        }
    }
    
    /// <summary>
    /// Дать воду растению
    /// </summary>
    public void GiveWater()
    {
        hasWater = true;
        waterTimer = 0f;
        Debug.Log($"Растение полито! Будет расти {waterEffectDuration} секунд.");
    }
    
    /// <summary>
    /// Перейти на следующую стадию роста
    /// </summary>
    private void Grow()
    {
        currentStage++;
        if (currentStage > maxGrowthStage)
        {
            currentStage = maxGrowthStage;
        }
        
        UpdateVisuals();
        Debug.Log($"Растение выросло до стадии {currentStage}/{maxGrowthStage}");
    }
    
    /// <summary>
    /// Обновить визуал в зависимости от стадии роста
    /// </summary>
    private void UpdateVisuals()
    {
        if (growthStages == null || growthStages.Length == 0)
        {
            // Если нет моделей, просто меняем масштаб
            float scale = 0.3f + (currentStage * 0.35f);
            transform.localScale = Vector3.one * scale;
            return;
        }
        
        // Показываем нужную модель
        for (int i = 0; i < growthStages.Length; i++)
        {
            if (growthStages[i] != null)
            {
                growthStages[i].SetActive(i == currentStage);
            }
        }
    }
    
    /// <summary>
    /// Проверить, полностью ли выросло растение
    /// </summary>
    public bool IsFullyGrown()
    {
        return currentStage >= maxGrowthStage;
    }
    
    public int GetCurrentStage() => currentStage;
    public int GetMaxStage() => maxGrowthStage;
}
