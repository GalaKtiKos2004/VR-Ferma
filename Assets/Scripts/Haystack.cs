using UnityEngine;

/// <summary>
/// Стог сена для кормления животных (коров, овец и т.д.)
/// </summary>
public class Haystack : MonoBehaviour
{
    [Header("Настройки сена")]
    [SerializeField] private float hayAmount = 100f; // Текущее количество сена
    [SerializeField] private float maxHay = 100f; // Максимальное количество сена
    [SerializeField] private float hayPerPortion = 10f; // Сколько сена съедает животное за раз
    [SerializeField] private float feedingCooldown = 3f; // Задержка между кормлениями одного животного
    
    [Header("Настройки кормления")]
    [SerializeField] private float feedingRange = 2f; // Радиус, в котором животные могут есть
    [SerializeField] private LayerMask animalLayer; // Слой для животных
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject hayVisual; // 3D модель стога сена
    [SerializeField] private Material fullHayMaterial; // Материал полного стога
    [SerializeField] private Material emptyHayMaterial; // Материал пустого стога
    [SerializeField] private MeshRenderer haystackRenderer;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip eatSound; // Звук поедания сена
    [SerializeField] private AudioClip refillSound; // Звук пополнения
    private AudioSource audioSource;
    
    [Header("Эффекты частиц")]
    [SerializeField] private ParticleSystem eatParticles; // Частицы при поедании
    
    // Словарь для отслеживания времени последнего кормления каждого животного
    private System.Collections.Generic.Dictionary<Animal, float> lastFeedTime = new System.Collections.Generic.Dictionary<Animal, float>();
    
    private Vector3 initialScale; // Начальный размер стога
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Сохраняем начальный размер
        if (hayVisual != null)
        {
            initialScale = hayVisual.transform.localScale;
        }
        else
        {
            initialScale = transform.localScale;
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        // Автоматическое кормление животных рядом
        if (hayAmount > 0)
        {
            FeedNearbyAnimals();
        }
    }
    
    /// <summary>
    /// Покормить животных рядом со стогом
    /// </summary>
    private void FeedNearbyAnimals()
    {
        // Ищем всех животных в радиусе
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, feedingRange);
        
        foreach (Collider col in nearbyColliders)
        {
            Animal animal = col.GetComponent<Animal>();
            
            // Проверяем, что это животное и оно голодное
            if (animal != null && animal.IsHungry())
            {
                // Проверяем cooldown для этого животного
                if (!lastFeedTime.ContainsKey(animal) || Time.time - lastFeedTime[animal] >= feedingCooldown)
                {
                    // Кормим животное
                    if (TakeHay(hayPerPortion))
                    {
                        animal.Feed();
                        lastFeedTime[animal] = Time.time;
                        
                        // Воспроизводим звук и эффекты
                        PlayEatEffects();
                        
                        Debug.Log($"Животное покормлено из стога! Осталось сена: {hayAmount:F0}/{maxHay}");
                    }
                }
            }
        }
        
        // Очищаем словарь от уничтоженных животных
        CleanupDestroyedAnimals();
    }
    
    /// <summary>
    /// Взять сено из стога
    /// </summary>
    private bool TakeHay(float amount)
    {
        if (hayAmount >= amount)
        {
            hayAmount -= amount;
            UpdateVisuals();
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Пополнить стог сена (вручную или автоматически)
    /// </summary>
    public void Refill()
    {
        if (hayAmount >= maxHay)
        {
            Debug.Log("Стог уже полный!");
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Стог уже полный! 🌾", 2f);
            }
            
            return;
        }
        
        hayAmount = maxHay;
        UpdateVisuals();
        
        if (refillSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(refillSound);
        }
        
        Debug.Log("Стог сена пополнен!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Стог сена пополнен! 🌾", 2f);
        }
    }
    
    /// <summary>
    /// Воспроизвести звуки и эффекты поедания
    /// </summary>
    private void PlayEatEffects()
    {
        // Звук
        if (eatSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(eatSound);
        }
        
        // Частицы
        if (eatParticles != null)
        {
            eatParticles.Play();
        }
    }
    
    /// <summary>
    /// Обновить визуал стога в зависимости от количества сена
    /// </summary>
    private void UpdateVisuals()
    {
        float hayPercentage = hayAmount / maxHay;
        
        // Изменяем размер стога в зависимости от количества сена
        if (hayVisual != null)
        {
            // Плавно уменьшаем высоту стога (только по Y)
            Vector3 newScale = initialScale;
            newScale.y = initialScale.y * Mathf.Max(0.2f, hayPercentage); // Минимум 20% высоты
            hayVisual.transform.localScale = newScale;
        }
        else
        {
            // Если нет отдельного визуала, масштабируем сам объект
            Vector3 newScale = initialScale;
            newScale.y = initialScale.y * Mathf.Max(0.2f, hayPercentage);
            transform.localScale = newScale;
        }
        
        // Меняем материал в зависимости от заполненности
        if (haystackRenderer != null)
        {
            if (hayAmount > maxHay * 0.3f && fullHayMaterial != null)
            {
                haystackRenderer.material = fullHayMaterial;
            }
            else if (emptyHayMaterial != null)
            {
                haystackRenderer.material = emptyHayMaterial;
            }
        }
        
        // Меняем цвет стога в зависимости от количества
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (hayAmount > maxHay * 0.5f)
            {
                // Полный стог - ярко-жёлтый
                renderer.material.color = new Color(0.9f, 0.8f, 0.3f);
            }
            else if (hayAmount > maxHay * 0.2f)
            {
                // Средний - тёмно-жёлтый
                renderer.material.color = new Color(0.7f, 0.6f, 0.2f);
            }
            else
            {
                // Почти пустой - коричневатый
                renderer.material.color = new Color(0.5f, 0.4f, 0.15f);
            }
        }
    }
    
    /// <summary>
    /// Очистить словарь от уничтоженных животных
    /// </summary>
    private void CleanupDestroyedAnimals()
    {
        System.Collections.Generic.List<Animal> toRemove = new System.Collections.Generic.List<Animal>();
        
        foreach (var kvp in lastFeedTime)
        {
            if (kvp.Key == null)
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (Animal animal in toRemove)
        {
            lastFeedTime.Remove(animal);
        }
    }
    
    /// <summary>
    /// Отрисовка радиуса кормления в редакторе
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса кормления
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, feedingRange);
    }
    
    // Публичные методы для получения информации
    public float GetHayAmount() => hayAmount;
    public float GetHayPercentage() => hayAmount / maxHay;
    public bool IsEmpty() => hayAmount <= 0;
    public bool IsFull() => hayAmount >= maxHay;
}
