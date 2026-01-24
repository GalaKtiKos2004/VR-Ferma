using UnityEngine;


/// <summary>
/// Компонент для взаимодействия с объектами в VR
/// </summary>
public class VRInteractor : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    
    [Header("UI подсказки")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Transform promptPosition;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    private GameObject currentTarget;
    
    private void Start()
    {
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    private void Update()
    {
        CheckForInteractables();
    }
    
    /// <summary>
    /// Проверка наличия объектов для взаимодействия
    /// </summary>
    private void CheckForInteractables()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionRange, interactableLayer))
        {
            currentTarget = hit.collider.gameObject;
            ShowPrompt(hit.point);
        }
        else
        {
            currentTarget = null;
            HidePrompt();
        }
    }
    
    /// <summary>
    /// Взаимодействие с текущим объектом
    /// </summary>
    public void Interact()
    {
        if (currentTarget == null) return;
        
        // Проверяем разные типы объектов
        PlantBed bed = currentTarget.GetComponent<PlantBed>();
        if (bed != null)
        {
            if (!bed.HasPlant())
            {
                bed.PlantSeed();
            }
            else
            {
                bed.Harvest();
            }
            return;
        }
        
        Animal animal = currentTarget.GetComponent<Animal>();
        if (animal != null)
        {
            animal.Pet();
            return;
        }
        
        Plant plant = currentTarget.GetComponent<Plant>();
        if (plant != null && plant.IsFullyGrown())
        {
            // Собрать урожай
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.AddCarrot();
            }
            Destroy(plant.gameObject);
            return;
        }
    }
    
    /// <summary>
    /// Показать подсказку
    /// </summary>
    private void ShowPrompt(Vector3 position)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            if (promptPosition != null)
            {
                promptPosition.position = position + Vector3.up * 0.5f;
            }
        }
    }
    
    /// <summary>
    /// Скрыть подсказку
    /// </summary>
    private void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
}
