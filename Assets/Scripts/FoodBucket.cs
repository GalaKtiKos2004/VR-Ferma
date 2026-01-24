using UnityEngine;

/// <summary>
/// Ведро с зерном для кормления животных
/// </summary>
public class FoodBucket : MonoBehaviour
{
    [Header("Настройки зерна")]
    [SerializeField] private float maxFood = 100f;
    [SerializeField] private float currentFood = 100f;
    [SerializeField] private float foodPerPortion = 10f; // Сколько зерна берём за раз
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject foodVisual; // Визуал зерна в ведре
    [SerializeField] private Material fullMaterial;
    [SerializeField] private Material emptyMaterial;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip takeSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        UpdateVisuals();
    }
    
    /// <summary>
    /// Взять порцию зерна
    /// </summary>
    public bool TakeFood()
    {
        if (currentFood < foodPerPortion)
        {
            Debug.Log("Ведро пустое! Нужно наполнить.");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Ведро пустое! 🪣");
            }
            return false;
        }
        
        currentFood -= foodPerPortion;
        UpdateVisuals();
        
        if (takeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(takeSound);
        }
        
        Debug.Log($"Взято зерно! Осталось: {currentFood}/{maxFood}");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Зерно взято! 🌾 Покормите животное");
        }
        
        return true;
    }
    
    /// <summary>
    /// Наполнить ведро
    /// </summary>
    public void Refill()
    {
        currentFood = maxFood;
        UpdateVisuals();
        
        Debug.Log("Ведро наполнено зерном!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Ведро наполнено зерном! 🌾");
        }
    }
    
    /// <summary>
    /// Обновить визуал ведра
    /// </summary>
    private void UpdateVisuals()
    {
        if (foodVisual != null)
        {
            // Показываем зерно только если есть
            foodVisual.SetActive(currentFood > 0);
            
            // Масштабируем по количеству
            float scale = currentFood / maxFood;
            foodVisual.transform.localScale = Vector3.one * Mathf.Max(0.1f, scale);
        }
    }
    
    public float GetFoodAmount() => currentFood;
    public float GetFoodPercentage() => currentFood / maxFood;
    public bool IsEmpty() => currentFood <= 0;
}
