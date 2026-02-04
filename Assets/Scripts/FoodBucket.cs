using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Ведро с зерном для кормления животных
/// </summary>
public class FoodBucket : MonoBehaviour
{
    [Header("VR взаимодействие")]
    [SerializeField] private bool useVRInteraction = true;
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
    [SerializeField] private AudioClip pourSound; // Звук высыпания в кормушку
    private AudioSource audioSource;
    
    [Header("Взаимодействие с кормушкой")]
    [SerializeField] private float pourRange = 2f; // Расстояние для высыпания в кормушку
    [SerializeField] private float pourAmount = 20f; // Сколько зерна высыпаем за раз
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Настраиваем VR-взаимодействие
        if (useVRInteraction)
        {
            SetupVRInteraction();
        }
        
        UpdateVisuals();
    }
    
    /// <summary>
    /// Настроить VR-взаимодействие с ведром
    /// </summary>
    private void SetupVRInteraction()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        }
        
        interactable.selectEntered.AddListener(OnVRInteract);
        
        // Убедимся что есть коллайдер
        if (GetComponent<Collider>() == null)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(0.3f, 0.4f, 0.3f);
            col.center = Vector3.up * 0.2f;
        }
    }
    
    /// <summary>
    /// VR-взаимодействие: взять зерно или высыпать в кормушку
    /// </summary>
    private void OnVRInteract(SelectEnterEventArgs args)
    {
        // Проверяем, есть ли рядом кормушка для высыпания
        FeedingTrough nearbyTrough = FindNearbyTrough();
        
        if (nearbyTrough != null && nearbyTrough.GetFoodPercentage() < 1f) // Если кормушка не полная
        {
            // Высыпаем зерно в кормушку
            if (PourIntoTrough(nearbyTrough))
            {
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint($"🌾 Зерно высыпано в кормушку!");
                }
                return;
            }
        }
        
        // Иначе берем зерно в руку
        var hand = args.interactorObject.transform;
        var foodInHand = hand.GetComponentInChildren<FoodInHand>();
        
        if (foodInHand == null)
        {
            // Создаём компонент если его нет
            foodInHand = hand.gameObject.AddComponent<FoodInHand>();
        }
        
        if (TakeFood())
        {
            foodInHand.TakeFood(1);
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint($"🌾 Зерно взято! ({currentFood:F0}/{maxFood})");
            }
        }
    }
    
    private void OnDestroy()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnVRInteract);
        }
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
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnFoodTaken();
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
            
            // Масштабируем по количеству (от 0.1 до 0.6)
            float fillPercent = currentFood / maxFood;
            float scale = Mathf.Lerp(0.1f, 0.6f, fillPercent);
            foodVisual.transform.localScale = Vector3.one * scale;
        }
    }
    
    /// <summary>
    /// Найти ближайшую кормушку
    /// </summary>
    private FeedingTrough FindNearbyTrough()
    {
        FeedingTrough[] troughs = FindObjectsOfType<FeedingTrough>();
        
        foreach (FeedingTrough trough in troughs)
        {
            float distance = Vector3.Distance(transform.position, trough.transform.position);
            if (distance <= pourRange)
            {
                return trough;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Высыпать зерно в кормушку
    /// </summary>
    private bool PourIntoTrough(FeedingTrough trough)
    {
        if (currentFood < pourAmount)
        {
            Debug.Log("В ведре недостаточно зерна для высыпания!");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Недостаточно зерна в ведре! 🪣");
            }
            return false;
        }
        
        currentFood -= pourAmount;
        trough.AddFood(pourAmount);
        UpdateVisuals();
        
        if (pourSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pourSound);
        }
        
        Debug.Log($"Зерно высыпано в кормушку! Осталось в ведре: {currentFood}/{maxFood}");
        return true;
    }
    
    public float GetFoodAmount() => currentFood;
    public float GetFoodPercentage() => currentFood / maxFood;
    public bool IsEmpty() => currentFood <= 0;
}
