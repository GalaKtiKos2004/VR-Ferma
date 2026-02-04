using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Грабли для взрыхления грядок
/// </summary>
public class Rake : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float tillingRange = 2f;
    [SerializeField] private LayerMask plantBedLayer;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip tillingSound; // Звук dig.mp3
    private AudioSource audioSource;
    
    [Header("VR Настройки")]
    [SerializeField] private bool isVRMode = true;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isTilling = false;
    private bool wasGrabbed;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Настраиваем AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.5f; // 3D звук
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Определяем режим (VR или нет)
        isVRMode = FindObjectOfType<UnityEngine.XR.Management.XRGeneralSettings>() != null;
        
        // Логирование для диагностики
        Debug.Log($"[Rake] Инициализирован. Звук назначен: {tillingSound != null}, AudioSource: {audioSource != null}");
    }
    
    private void Update()
    {
        // Проверяем взятие в VR
        if (!wasGrabbed && grabInteractable != null && grabInteractable.isSelected)
        {
            OnGrabbed();
            wasGrabbed = true;
        }
        
        // VR режим - используем при нажатии триггера
        if (isVRMode && grabInteractable != null && grabInteractable.isSelected)
        {
            // Проверяем нажатие триггера на контроллере
            float triggerValue = Mathf.Max(Input.GetAxis("XRI_Right_Trigger"), Input.GetAxis("XRI_Left_Trigger"));
            if (triggerValue > 0.5f)
            {
                TryTillBed();
            }
        }
        // Non-VR режим - используем на ЛКМ (если грабли в руках через NonVRPlayerController)
        // В Non-VR взрыхление обрабатывается через NonVRPlayerController при взаимодействии с грядкой
        // Здесь оставляем для совместимости
    }
    
    /// <summary>
    /// Воспроизвести звук взрыхления (публичный метод для вызова извне)
    /// </summary>
    public void PlayTillingSound()
    {
        if (tillingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(tillingSound);
            Debug.Log($"[Rake] Воспроизведен звук: {tillingSound.name}");
        }
        else
        {
            Debug.LogWarning($"[Rake] Звук не может быть воспроизведен! tillingSound={tillingSound != null}, audioSource={audioSource != null}");
        }
    }
    
    /// <summary>
    /// Попытка взрыхлить грядку
    /// </summary>
    private void TryTillBed()
    {
        if (isTilling) return;
        
        // Луч вниз от граблей
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, tillingRange, plantBedLayer))
        {
            PlantBed bed = hit.collider.GetComponent<PlantBed>();
            if (bed != null && bed.IsTilled() && !bed.IsRaked())
            {
                isTilling = true;
                bed.Rake();
                
                // Звук взрыхления
                PlayTillingSound();
                
                Debug.Log("Грядка разрыхлена граблями!");
                
                // Уведомляем tutorial manager
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnBedRaked();
                }
                
                // Сбрасываем флаг через небольшую задержку
                Invoke("ResetTilling", 0.5f);
            }
        }
    }
    
    private void ResetTilling()
    {
        isTilling = false;
    }
    
    /// <summary>
    /// Вызывается когда грабли берут в руки
    /// </summary>
    public void OnGrabbed()
    {
        Debug.Log("Грабли взяты!");
        
        // Уведомляем tutorial manager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnRakeTaken();
        }
    }
    
    /// <summary>
    /// Тестовое воспроизведение звука (для проверки в Inspector)
    /// </summary>
    [ContextMenu("Тест: Воспроизвести звук")]
    public void TestPlaySound()
    {
        if (tillingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(tillingSound);
            Debug.Log($"[Rake] Тест: воспроизведен звук {tillingSound.name}");
        }
        else
        {
            Debug.LogWarning($"[Rake] Тест: звук не может быть воспроизведен! tillingSound={tillingSound != null}, audioSource={audioSource != null}");
        }
    }
}
