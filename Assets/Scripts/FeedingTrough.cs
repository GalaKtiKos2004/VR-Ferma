using UnityEngine;

/// <summary>
/// Кормушка для животных
/// </summary>
public class FeedingTrough : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float foodAmount = 100f;
    [SerializeField] private float maxFood = 100f;
    [SerializeField] private float refillAmount = 50f;
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject foodVisual; // Визуал еды в кормушке
    [SerializeField] private Material emptyMaterial;
    [SerializeField] private Material fullMaterial;
    [SerializeField] private MeshRenderer troughRenderer;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip refillSound;
    [SerializeField] private AudioClip eatSound;
    private AudioSource audioSource;
    
    [Header("Животные")]
    [SerializeField] private float feedingRange = 2f;
    [SerializeField] private LayerMask animalLayer;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        // Автоматическое кормление животных рядом
        if (foodAmount > 0)
        {
            FeedNearbyAnimals();
        }
    }
    
    /// <summary>
    /// Наполнить кормушку
    /// </summary>
    public void Refill()
    {
        if (foodAmount >= maxFood)
        {
            Debug.Log("Кормушка уже полная!");
            return;
        }
        
        foodAmount += refillAmount;
        if (foodAmount > maxFood)
        {
            foodAmount = maxFood;
        }
        
        if (refillSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(refillSound);
        }
        
        UpdateVisuals();
        Debug.Log($"Кормушка наполнена! Корма: {foodAmount:F0}/{maxFood}");
    }
    
    /// <summary>
    /// Взять корм из кормушки
    /// </summary>
    public bool TakeFood(float amount)
    {
        if (foodAmount >= amount)
        {
            foodAmount -= amount;
            
            if (eatSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(eatSound);
            }
            
            UpdateVisuals();
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Покормить животных рядом
    /// </summary>
    private void FeedNearbyAnimals()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, feedingRange, animalLayer);
        
        foreach (Collider col in nearbyColliders)
        {
            Animal animal = col.GetComponent<Animal>();
            if (animal != null && animal.IsHungry())
            {
                if (TakeFood(10f))
                {
                    animal.Feed();
                }
            }
        }
    }
    
    /// <summary>
    /// Обновить визуал кормушки
    /// </summary>
    private void UpdateVisuals()
    {
        if (foodVisual != null)
        {
            // Показываем еду только если кормушка не пустая
            foodVisual.SetActive(foodAmount > 0);
            
            // Масштабируем в зависимости от количества
            float scale = foodAmount / maxFood;
            foodVisual.transform.localScale = Vector3.one * scale;
        }
        
        if (troughRenderer != null)
        {
            if (foodAmount > 0 && fullMaterial != null)
            {
                troughRenderer.material = fullMaterial;
            }
            else if (emptyMaterial != null)
            {
                troughRenderer.material = emptyMaterial;
            }
        }
    }
    
    public float GetFoodAmount() => foodAmount;
    public float GetFoodPercentage() => foodAmount / maxFood;
    public bool IsEmpty() => foodAmount <= 0;
    
    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса кормления
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, feedingRange);
    }
}
