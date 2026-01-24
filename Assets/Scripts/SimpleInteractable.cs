using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Простой интерактивный объект - универсальный компонент для взаимодействия
/// </summary>
public class SimpleInteractable : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string interactionText = "Нажмите E для взаимодействия";
    [SerializeField] private bool canInteract = true;
    [SerializeField] private float cooldownTime = 0.5f;
    
    [Header("События")]
    [SerializeField] private UnityEvent onInteract;
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject highlightEffect;
    [SerializeField] private Color highlightColor = Color.yellow;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip interactSound;
    private AudioSource audioSource;
    
    private float lastInteractionTime = 0f;
    private Material originalMaterial;
    private Renderer objRenderer;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalMaterial = objRenderer.material;
        }
        
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
    }
    
    /// <summary>
    /// Вызывается при взаимодействии
    /// </summary>
    public void Interact()
    {
        if (!canInteract)
        {
            Debug.Log("Взаимодействие недоступно");
            return;
        }
        
        // Проверка cooldown
        if (Time.time - lastInteractionTime < cooldownTime)
        {
            return;
        }
        
        lastInteractionTime = Time.time;
        
        // Воспроизводим звук
        if (interactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactSound);
        }
        
        // Вызываем события
        onInteract?.Invoke();
        
        Debug.Log($"Взаимодействие с {gameObject.name}");
    }
    
    /// <summary>
    /// Подсветить объект
    /// </summary>
    public void Highlight(bool enable)
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(enable);
        }
        else if (objRenderer != null && enable)
        {
            // Простая подсветка через изменение цвета
            objRenderer.material.color = highlightColor;
        }
        else if (objRenderer != null && !enable && originalMaterial != null)
        {
            objRenderer.material = originalMaterial;
        }
    }
    
    /// <summary>
    /// Получить текст для подсказки
    /// </summary>
    public string GetInteractionText()
    {
        return interactionText;
    }
    
    /// <summary>
    /// Установить возможность взаимодействия
    /// </summary>
    public void SetInteractable(bool value)
    {
        canInteract = value;
    }
    
    public bool CanInteract() => canInteract;
}
