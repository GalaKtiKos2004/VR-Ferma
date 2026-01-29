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
    private AudioSource audioSource;
    
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
    /// VR-взаимодействие: взять зерно
    /// </summary>
    private void OnVRInteract(SelectEnterEventArgs args)
    {
        // Ищем FoodInHand в руке игрока
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
    
    public float GetFoodAmount() => currentFood;
    public float GetFoodPercentage() => currentFood / maxFood;
    public bool IsEmpty() => currentFood <= 0;
}
