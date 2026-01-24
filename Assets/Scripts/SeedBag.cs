using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Пакет с семенами для посадки растений
/// </summary>
public class SeedBag : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string seedType = "Морковь";
    [SerializeField] private int seedCount = 10;
    [SerializeField] private bool infiniteSeeds = true;
    
    [Header("Посадка")]
    [SerializeField] private float plantingRange = 2f;
    [SerializeField] private LayerMask plantBedLayer;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip plantingSound;
    private AudioSource audioSource;
    
    [Header("VR Настройки")]
    [SerializeField] private bool isVRMode = true;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isPlanting = false;
    private bool wasGrabbed;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Определяем режим (VR или нет)
        isVRMode = FindObjectOfType<UnityEngine.XR.Management.XRGeneralSettings>() != null;
    }
    
    private void Update()
    {
        // Проверяем взятие в VR
        if (!wasGrabbed && grabInteractable != null && grabInteractable.isSelected)
        {
            OnGrabbed();
            wasGrabbed = true;
        }
        
        // VR режим - сажаем при нажатии триггера
        if (isVRMode && grabInteractable != null && grabInteractable.isSelected)
        {
            if (Input.GetAxis("XRI_Right_Trigger") > 0.5f || Input.GetAxis("XRI_Left_Trigger") > 0.5f)
            {
                TryPlantSeed();
            }
        }
        // Non-VR режим - сажаем на ЛКМ
        else if (!isVRMode && Input.GetMouseButtonDown(0))
        {
            TryPlantSeed();
        }
    }
    
    /// <summary>
    /// Попытка посадить семя
    /// </summary>
    private void TryPlantSeed()
    {
        if (isPlanting) return;
        
        if (!infiniteSeeds && seedCount <= 0)
        {
            Debug.Log("Семена закончились!");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Семена закончились!", 2f);
            }
            return;
        }
        
        // Луч вниз от пакета с семенами
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, plantingRange, plantBedLayer))
        {
            PlantBed bed = hit.collider.GetComponent<PlantBed>();
            if (bed != null && bed.IsTilled() && bed.IsRaked() && !bed.HasPlant())
            {
                isPlanting = true;
                bed.PlantSeed(seedType);
                
                // Тратим семя
                if (!infiniteSeeds)
                {
                    seedCount--;
                }
                
                // Звук посадки
                if (plantingSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(plantingSound);
                }
                
                Debug.Log($"Семя {seedType} посажено! Осталось семян: {(infiniteSeeds ? "∞" : seedCount.ToString())}");
                
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint($"Семя {seedType} посажено!", 2f);
                }
                
                // Уведомляем tutorial manager
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnSeedPlanted();
                }
                
                // Сбрасываем флаг через небольшую задержку
                Invoke("ResetPlanting", 0.5f);
            }
            else if (bed != null && !bed.IsTilled())
            {
                Debug.Log("Сначала взрыхлите грядку!");
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Сначала взрыхлите грядку!", 2f);
                }
            }
            else if (bed != null && bed.HasPlant())
            {
                Debug.Log("На грядке уже есть растение!");
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("На грядке уже есть растение!", 2f);
                }
            }
        }
    }
    
    private void ResetPlanting()
    {
        isPlanting = false;
    }
    
    /// <summary>
    /// Вызывается когда пакет берут в руки
    /// </summary>
    public void OnGrabbed()
    {
        Debug.Log($"Пакет с семенами {seedType} взят!");
        
        // Уведомляем tutorial manager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnSeedsTaken();
        }
    }
    
    public int GetSeedCount() => seedCount;
    public string GetSeedType() => seedType;
    public bool HasSeeds() => infiniteSeeds || seedCount > 0;

    /// <summary>Подсказка при подходе: «Взять семена тыквы» / «Взять семена помидоров» / «Взять семена моркови» / «Взять семена лука».</summary>
    public string GetPickupHint()
    {
        if (string.Equals(seedType, "Тыква", System.StringComparison.OrdinalIgnoreCase))
            return "Взять семена тыквы";
        if (string.Equals(seedType, "Помидор", System.StringComparison.OrdinalIgnoreCase))
            return "Взять семена помидоров";
        if (string.Equals(seedType, "Морковь", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(seedType, "Морковка", System.StringComparison.OrdinalIgnoreCase))
            return "Взять семена моркови";
        if (string.Equals(seedType, "Лук", System.StringComparison.OrdinalIgnoreCase))
            return "Взять семена лука";
        return "Взять пакет с семенами";
    }
}
