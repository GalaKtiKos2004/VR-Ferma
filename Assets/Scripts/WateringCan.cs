using UnityEngine;


/// <summary>
/// Лейка для полива грядок
/// </summary>
public class WateringCan : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float maxWater = 100f;
    [SerializeField] private float currentWater = 0f;
    [SerializeField] private float wateringRange = 2f;
    [SerializeField] private LayerMask plantBedLayer;
    
    [Header("Визуальные эффекты")]
    [SerializeField] private ParticleSystem waterParticles;
    [SerializeField] private Transform waterSpout; // Откуда льется вода
    
    [Header("Звуки")]
    [SerializeField] private AudioClip wateringSound; // Звук running-water.mp3
    [SerializeField] private AudioClip refillSound;
    private AudioSource audioSource;
    
    [Header("VR Настройки")]
    [SerializeField] private bool isVRMode = true;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isPouring = false;
    private bool wasGrabbed = false;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (waterParticles != null)
        {
            waterParticles.Stop();
        }
        
        // Определяем режим (VR или нет)
        isVRMode = FindObjectOfType<UnityEngine.XR.Management.XRGeneralSettings>() != null;
    }
    
    private void Update()
    {
        // Проверяем, взяли ли лейку (для tutorial)
        if (!wasGrabbed && grabInteractable != null && grabInteractable.isSelected)
        {
            OnGrabbed();
            wasGrabbed = true;
        }
        
        // VR режим - поливаем при нажатии триггера
        if (isVRMode && grabInteractable != null && grabInteractable.isSelected)
        {
            // Проверяем нажатие триггера на контроллере
            if (Input.GetAxis("XRI_Right_Trigger") > 0.5f || Input.GetAxis("XRI_Left_Trigger") > 0.5f)
            {
                StartPouring();
            }
            else
            {
                StopPouring();
            }
        }
        // Non-VR режим - поливаем на ЛКМ
        else if (!isVRMode && Input.GetMouseButton(0))
        {
            StartPouring();
        }
        else
        {
            StopPouring();
        }
    }
    
    /// <summary>
    /// Начать полив
    /// </summary>
    private void StartPouring()
    {
        if (currentWater <= 0)
        {
            StopPouring();
            return;
        }
        
        if (!isPouring)
        {
            isPouring = true;
            if (waterParticles != null)
            {
                waterParticles.Play();
            }
            if (wateringSound != null && audioSource != null)
            {
                audioSource.clip = wateringSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        
        // Тратим воду
        currentWater -= Time.deltaTime * 10f;
        if (currentWater < 0) currentWater = 0;
        
        // Проверяем, попадаем ли на грядку
        TryWaterPlantBed();
    }
    
    /// <summary>
    /// Остановить полив
    /// </summary>
    private void StopPouring()
    {
        if (isPouring)
        {
            isPouring = false;
            if (waterParticles != null)
            {
                waterParticles.Stop();
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
    
    /// <summary>
    /// Попытка полить грядку
    /// </summary>
    private void TryWaterPlantBed()
    {
        Vector3 spoutPosition = waterSpout != null ? waterSpout.position : transform.position;
        
        // Луч вниз от носика лейки
        RaycastHit hit;
        if (Physics.Raycast(spoutPosition, Vector3.down, out hit, wateringRange, plantBedLayer))
        {
            PlantBed bed = hit.collider.GetComponent<PlantBed>();
            if (bed != null && !bed.IsWatered())
            {
                bed.Water();
            }
        }
    }
    
    /// <summary>
    /// Наполнить лейку водой
    /// </summary>
    public void Refill()
    {
        if (currentWater >= maxWater)
        {
            Debug.Log("Лейка уже полная!");
            return;
        }
        
        currentWater = maxWater;
        
        if (refillSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(refillSound);
        }
        
        Debug.Log("Лейка наполнена!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Лейка наполнена водой!", 2f);
        }
        
        // Уведомляем tutorial manager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnCanFilled();
        }
    }
    
    /// <summary>
    /// Проверить, пуста ли лейка
    /// </summary>
    public bool IsEmpty()
    {
        return currentWater <= 0;
    }
    
    /// <summary>
    /// Получить процент заполнения
    /// </summary>
    public float GetFillPercentage()
    {
        return currentWater / maxWater;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Автоматическое наполнение при контакте с водой
        if (other.CompareTag("Water"))
        {
            Refill();
        }
    }
    
    /// <summary>
    /// Вызывается когда лейку берут в руки
    /// </summary>
    public void OnGrabbed()
    {
        Debug.Log("Лейка взята!");
        
        // Уведомляем tutorial manager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnCanTaken();
        }
    }
}
