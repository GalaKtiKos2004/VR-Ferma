using UnityEngine;

/// <summary>
/// Визуальный эффект при кормлении животного
/// </summary>
public class FeedingEffect : MonoBehaviour
{
    [Header("Настройки частиц")]
    [SerializeField] private ParticleSystem feedParticles;
    [SerializeField] private GameObject heartPrefab; // Сердечки над животным
    
    [Header("Цвета")]
    [SerializeField] private Color foodColor = new Color(1f, 0.8f, 0.3f); // Цвет зерна
    [SerializeField] private Color heartColor = new Color(1f, 0.3f, 0.5f); // Цвет сердечек
    
    [Header("Звук")]
    [SerializeField] private AudioClip feedSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    /// <summary>
    /// Воспроизвести эффект кормления
    /// </summary>
    public void PlayFeedEffect(Vector3 position)
    {
        // Создаем частицы зерна
        if (feedParticles != null)
        {
            ParticleSystem ps = Instantiate(feedParticles, position, Quaternion.identity);
            var main = ps.main;
            main.startColor = foodColor;
            ps.Play();
            Destroy(ps.gameObject, 2f);
        }
        else
        {
            // Создаем простые частицы если нет префаба
            CreateSimpleParticles(position, foodColor, 10);
        }
        
        // Создаем сердечки над животным
        SpawnHearts(position + Vector3.up * 1.5f);
        
        // Звук
        if (feedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(feedSound);
        }
    }
    
    /// <summary>
    /// Создать простые частицы (если нет ParticleSystem)
    /// </summary>
    private void CreateSimpleParticles(Vector3 position, Color color, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.transform.position = position;
            particle.transform.localScale = Vector3.one * 0.05f;
            
            // Цвет
            Renderer renderer = particle.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            renderer.material = mat;
            
            // Убираем коллайдер
            Collider col = particle.GetComponent<Collider>();
            if (col != null) Destroy(col);
            
            // Добавляем движение
            Rigidbody rb = particle.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.01f;
            
            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1.5f),
                Random.Range(-1f, 1f)
            );
            rb.AddForce(randomDir * 2f, ForceMode.Impulse);
            
            // Удаляем через время
            Destroy(particle, 1.5f);
        }
    }
    
    /// <summary>
    /// Создать сердечки над животным
    /// </summary>
    private void SpawnHearts(Vector3 position)
    {
        if (heartPrefab != null)
        {
            GameObject heart = Instantiate(heartPrefab, position, Quaternion.identity);
            Destroy(heart, 2f);
        }
        else
        {
            // Создаем простое сердечко из Quad
            for (int i = 0; i < 3; i++)
            {
                GameObject heart = GameObject.CreatePrimitive(PrimitiveType.Quad);
                heart.transform.position = position + Vector3.up * i * 0.3f;
                heart.transform.localScale = Vector3.one * 0.3f;
                heart.transform.rotation = Quaternion.Euler(0, 45, 0);
                
                // Цвет
                Renderer renderer = heart.GetComponent<Renderer>();
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = heartColor;
                renderer.material = mat;
                
                // Убираем коллайдер
                Collider col = heart.GetComponent<Collider>();
                if (col != null) Destroy(col);
                
                // Анимация всплытия
                StartCoroutine(AnimateHeart(heart.transform, i * 0.2f));
                
                // Удаляем через время
                Destroy(heart, 2f);
            }
        }
    }
    
    /// <summary>
    /// Анимация всплытия сердечка
    /// </summary>
    private System.Collections.IEnumerator AnimateHeart(Transform heart, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 startPos = heart.position;
        float duration = 1.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Движение вверх с замедлением
            heart.position = startPos + Vector3.up * (t * 1.5f);
            
            // Вращение
            heart.Rotate(Vector3.up, 180f * Time.deltaTime);
            
            // Fade out
            if (heart.GetComponent<Renderer>() != null)
            {
                Color color = heart.GetComponent<Renderer>().material.color;
                color.a = 1f - t;
                heart.GetComponent<Renderer>().material.color = color;
            }
            
            yield return null;
        }
    }
    
    /// <summary>
    /// Воспроизвести эффект взятия зерна из ведра
    /// </summary>
    public void PlayTakeEffect(Vector3 position)
    {
        CreateSimpleParticles(position, foodColor, 5);
    }
}
