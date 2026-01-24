using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Тяпка для взрыхления грядок
/// </summary>
public class Hoe : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float tillingRange = 2f;
    [SerializeField] private LayerMask plantBedLayer;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip tillingSound;
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
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
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
        
        // VR режим - используем при нажатии триггера
        if (isVRMode && grabInteractable != null && grabInteractable.isSelected)
        {
            if (Input.GetAxis("XRI_Right_Trigger") > 0.5f || Input.GetAxis("XRI_Left_Trigger") > 0.5f)
            {
                TryTillBed();
            }
        }
        // Non-VR режим - взрыхление обрабатывается через NonVRPlayerController
    }
    
    private void TryTillBed()
    {
        if (isTilling) return;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, tillingRange, plantBedLayer))
        {
            PlantBed bed = hit.collider.GetComponent<PlantBed>();
            if (bed != null && !bed.IsTilled())
            {
                isTilling = true;
                bed.Till();
                
                if (tillingSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(tillingSound);
                }
                
                Debug.Log("Грядка взрыхлена тяпкой!");
                
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnBedTilled();
                }
                
                Invoke(nameof(ResetTilling), 0.5f);
            }
        }
    }
    
    private void ResetTilling()
    {
        isTilling = false;
    }
    
    /// <summary>
    /// Вызывается когда тяпку берут в руки
    /// </summary>
    public void OnGrabbed()
    {
        Debug.Log("Тяпка взята!");
        
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnHoeTaken();
        }
    }
}
