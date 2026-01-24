using UnityEngine;

/// <summary>
/// Источник воды (бочка, колодец)
/// </summary>
public class WaterSource : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private bool isInfinite = true;
    [SerializeField] private float waterAmount = 1000f;
    
    [Header("Визуальные эффекты")]
    [SerializeField] private ParticleSystem splashEffect;
    [SerializeField] private Material waterMaterial;
    [SerializeField] private Transform waterSurface;
    
    [Header("Звуки")]
    [SerializeField] private AudioClip waterScoopSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Добавляем тег если его нет
        if (!gameObject.CompareTag("Water"))
        {
            gameObject.tag = "Water";
        }
    }
    
    /// <summary>
    /// Взять воду из источника
    /// </summary>
    public bool TakeWater(float amount)
    {
        if (isInfinite)
        {
            PlayEffects();
            return true;
        }
        
        if (waterAmount >= amount)
        {
            waterAmount -= amount;
            PlayEffects();
            UpdateWaterLevel();
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Воспроизвести эффекты
    /// </summary>
    private void PlayEffects()
    {
        if (splashEffect != null)
        {
            splashEffect.Play();
        }
        
        if (waterScoopSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(waterScoopSound);
        }
    }
    
    /// <summary>
    /// Обновить уровень воды
    /// </summary>
    private void UpdateWaterLevel()
    {
        if (waterSurface != null && !isInfinite)
        {
            // Опускаем уровень воды в зависимости от количества
            float percentage = waterAmount / 1000f;
            Vector3 pos = waterSurface.localPosition;
            pos.y = percentage * 1f; // 1 метр максимум
            waterSurface.localPosition = pos;
        }
    }
    
    public float GetWaterAmount() => waterAmount;
    public bool IsInfinite() => isInfinite;
}
