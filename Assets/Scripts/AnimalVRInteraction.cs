using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// VR-взаимодействие с животными: поглаживание и кормление
/// </summary>
[RequireComponent(typeof(Animal))]
public class AnimalVRInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    [SerializeField] private float petCooldown = 2f; // Задержка между поглаживаниями
    [SerializeField] private float feedCooldown = 1f; // Задержка между кормлениями
    
    [Header("Визуальная подсказка")]
    [SerializeField] private GameObject highlightEffect; // Подсветка при наведении
    
    private Animal animal;
    private float lastPetTime = 0f;
    private float lastFeedTime = 0f;
    private bool hasFood = false; // Игрок держит зерно
    
    private XRSimpleInteractable interactable;
    
    private void Start()
    {
        animal = GetComponent<Animal>();
        
        // Добавляем XRSimpleInteractable для VR-взаимодействия
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        }
        
        // Подписываемся на события
        interactable.selectEntered.AddListener(OnVRTouch);
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        
        // Добавляем коллайдер если нет
        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.height = 1.5f;
            col.radius = 0.6f;
            col.center = Vector3.up * 0.75f;
            col.isTrigger = false; // Для XRSimpleInteractable должен быть false
        }
        
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnVRTouch);
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
        }
    }
    
    /// <summary>
    /// Когда игрок касается животного контроллером
    /// </summary>
    private void OnVRTouch(SelectEnterEventArgs args)
    {
        // Проверяем, держит ли игрок зерно
        var handFood = args.interactorObject.transform.GetComponentInChildren<FoodInHand>();
        
        if (handFood != null && handFood.HasFood())
        {
            // Кормим
            FeedAnimal();
            handFood.UseFood();
        }
        else
        {
            // Гладим
            PetAnimal();
        }
    }
    
    /// <summary>
    /// Наведение на животное
    /// </summary>
    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(true);
        
        // Показываем подсказку
        if (SimpleGameManager.Instance != null)
        {
            var handFood = args.interactorObject.transform.GetComponentInChildren<FoodInHand>();
            if (handFood != null && handFood.HasFood())
            {
                SimpleGameManager.Instance.ShowHint("🌾 Покормить животное");
            }
            else
            {
                SimpleGameManager.Instance.ShowHint("✋ Погладить животное");
            }
        }
    }
    
    /// <summary>
    /// Убрали курсор с животного
    /// </summary>
    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }
    
    /// <summary>
    /// Погладить животное
    /// </summary>
    private void PetAnimal()
    {
        if (Time.time - lastPetTime < petCooldown)
        {
            Debug.Log($"Слишком часто гладите {animal.name}");
            return;
        }
        
        animal.Pet();
        lastPetTime = Time.time;
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("❤️ Животное довольно!");
        }
    }
    
    /// <summary>
    /// Покормить животное
    /// </summary>
    private void FeedAnimal()
    {
        if (Time.time - lastFeedTime < feedCooldown)
        {
            return;
        }
        
        animal.Feed();
        lastFeedTime = Time.time;
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("🌾 Животное накормлено!");
        }
    }
    
    /// <summary>
    /// Кормление через триггер (когда игрок подносит руку с зерном)
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        // Проверяем, есть ли в руке зерно
        var handFood = other.GetComponent<FoodInHand>();
        if (handFood != null && handFood.HasFood())
        {
            if (Time.time - lastFeedTime >= feedCooldown)
            {
                // Автоматически кормим при поднесении руки с зерном
                FeedAnimal();
                handFood.UseFood();
            }
        }
    }
}

/// <summary>
/// Компонент для руки игрока, показывающий наличие зерна
/// </summary>
public class FoodInHand : MonoBehaviour
{
    [SerializeField] private bool hasFood = false;
    [SerializeField] private int foodAmount = 1;
    
    [Header("Визуализация")]
    [SerializeField] private GameObject foodVisual; // Визуал зерна в руке
    
    private void Start()
    {
        UpdateVisuals();
    }
    
    /// <summary>
    /// Взять зерно из ведра
    /// </summary>
    public void TakeFood(int amount = 1)
    {
        hasFood = true;
        foodAmount = amount;
        UpdateVisuals();
        
        Debug.Log($"Взято {amount} порций зерна");
    }
    
    /// <summary>
    /// Использовать зерно
    /// </summary>
    public void UseFood()
    {
        if (!hasFood) return;
        
        foodAmount--;
        if (foodAmount <= 0)
        {
            hasFood = false;
            foodAmount = 0;
        }
        
        UpdateVisuals();
        Debug.Log($"Зерно использовано. Осталось: {foodAmount}");
    }
    
    public bool HasFood() => hasFood && foodAmount > 0;
    public int GetFoodAmount() => foodAmount;
    
    private void UpdateVisuals()
    {
        if (foodVisual != null)
        {
            foodVisual.SetActive(hasFood && foodAmount > 0);
        }
    }
}
