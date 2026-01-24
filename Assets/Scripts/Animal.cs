using UnityEngine;

/// <summary>
/// Животное на ферме
/// </summary>
public class Animal : MonoBehaviour
{
    [Header("Настройки животного")]
    [SerializeField] private string animalName = "Курица";
    [SerializeField] private float maxHappiness = 100f;
    [SerializeField] private float currentHappiness = 50f;
    [SerializeField] private float happinessDecayRate = 5f; // Уменьшение счастья в секунду
    
    [Header("Кормление")]
    [SerializeField] private float feedAmount = 30f; // Сколько счастья дает кормление
    [SerializeField] private float petAmount = 20f; // Сколько счастья дает поглаживание
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject happyEffect; // Частицы/сердечки когда довольно
    [SerializeField] private GameObject hungryEffect; // Эффект когда голодное
    [SerializeField] private Animator animator;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip happySound;
    [SerializeField] private AudioClip hungrySound;
    [SerializeField] private AudioClip eatSound;
    private AudioSource audioSource;
    
    [Header("Продукция")]
    [SerializeField] private bool canProduce = true; // Может ли производить (яйца, молоко)
    [SerializeField] private float productionTime = 60f; // Время до следующей продукции
    private float productionTimer = 0f;
    
    private bool isHungry = false;
    private float lastInteractionTime = 0f;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Ускоряем анимацию животного
        if (animator != null)
        {
            animator.speed = 1.5f; // В 1.5 раза быстрее
        }
        
        UpdateEffects();
    }
    
    private void Update()
    {
        // Уменьшаем счастье со временем
        currentHappiness -= happinessDecayRate * Time.deltaTime;
        if (currentHappiness < 0) currentHappiness = 0;
        
        // Проверяем состояние
        bool wasHungry = isHungry;
        isHungry = currentHappiness < 30f;
        
        if (isHungry && !wasHungry)
        {
            OnBecomeHungry();
        }
        else if (!isHungry && wasHungry)
        {
            OnBecomeHappy();
        }
        
        UpdateEffects();
        
        // Производство продукции
        if (canProduce && !isHungry)
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= productionTime)
            {
                ProduceItem();
                productionTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// Покормить животное
    /// </summary>
    public void Feed()
    {
        currentHappiness += feedAmount;
        if (currentHappiness > maxHappiness)
        {
            currentHappiness = maxHappiness;
        }
        
        if (eatSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(eatSound);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Eat");
        }
        
        Debug.Log($"{animalName} покормлено! Счастье: {currentHappiness:F0}/{maxHappiness}");
        lastInteractionTime = Time.time;
    }
    
    /// <summary>
    /// Погладить животное
    /// </summary>
    public void Pet()
    {
        // Не даем спамить поглаживания
        if (Time.time - lastInteractionTime < 2f)
        {
            return;
        }
        
        currentHappiness += petAmount;
        if (currentHappiness > maxHappiness)
        {
            currentHappiness = maxHappiness;
        }
        
        if (happySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(happySound);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Happy");
        }
        
        // Показываем сердечки
        if (happyEffect != null)
        {
            GameObject effect = Instantiate(happyEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        Debug.Log($"{animalName} погладили! Счастье: {currentHappiness:F0}/{maxHappiness}");
        lastInteractionTime = Time.time;
    }
    
    /// <summary>
    /// Производство продукции (яйца, молоко)
    /// </summary>
    private void ProduceItem()
    {
        Debug.Log($"{animalName} произвело продукцию!");
        // Здесь можно создать префаб яйца/молока
    }
    
    /// <summary>
    /// Животное стало голодным
    /// </summary>
    private void OnBecomeHungry()
    {
        if (hungrySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hungrySound);
        }
        
        Debug.Log($"{animalName} голодно!");
    }
    
    /// <summary>
    /// Животное стало довольным
    /// </summary>
    private void OnBecomeHappy()
    {
        if (happySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(happySound);
        }
        
        Debug.Log($"{animalName} довольно!");
    }
    
    /// <summary>
    /// Обновить визуальные эффекты
    /// </summary>
    private void UpdateEffects()
    {
        if (happyEffect != null)
        {
            happyEffect.SetActive(!isHungry && currentHappiness > 70f);
        }
        
        if (hungryEffect != null)
        {
            hungryEffect.SetActive(isHungry);
        }
    }
    
    public bool IsHungry() => isHungry;
    public float GetHappiness() => currentHappiness;
    public float GetHappinessPercentage() => currentHappiness / maxHappiness;
}
