using UnityEngine;

/// <summary>
/// Компонент для подсветки объектов во время обучения
/// </summary>
public class TutorialHighlight : MonoBehaviour
{
    [Header("Настройки подсветки")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 0.5f);
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minIntensity = 0.3f;
    [SerializeField] private float maxIntensity = 1f;
    
    private GameObject highlightObject;
    private Material highlightMaterial;
    private bool isHighlighted = false;
    private float pulseTimer = 0f;
    
    private void Start()
    {
        CreateHighlightObject();
    }
    
    private void Update()
    {
        if (isHighlighted && highlightObject != null)
        {
            // Пульсация подсветки
            pulseTimer += Time.deltaTime * pulseSpeed;
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(pulseTimer) + 1f) / 2f);
            
            if (highlightMaterial != null)
            {
                Color color = highlightColor;
                color.a = intensity;
                highlightMaterial.color = color;
            }
        }
    }
    
    /// <summary>
    /// Создать объект подсветки
    /// </summary>
    private void CreateHighlightObject()
    {
        // Получаем renderer оригинального объекта
        Renderer originalRenderer = GetComponent<Renderer>();
        if (originalRenderer == null)
        {
            // Ищем в дочерних объектах
            originalRenderer = GetComponentInChildren<Renderer>();
        }
        
        if (originalRenderer != null)
        {
            // Создаём копию объекта немного больше размером
            highlightObject = new GameObject("Highlight");
            highlightObject.transform.SetParent(transform);
            highlightObject.transform.localPosition = Vector3.zero;
            highlightObject.transform.localRotation = Quaternion.identity;
            highlightObject.transform.localScale = Vector3.one * 1.1f; // Немного больше
            
            // Копируем mesh
            MeshFilter originalMesh = originalRenderer.GetComponent<MeshFilter>();
            if (originalMesh != null)
            {
                MeshFilter highlightMesh = highlightObject.AddComponent<MeshFilter>();
                highlightMesh.mesh = originalMesh.mesh;
                
                MeshRenderer highlightRenderer = highlightObject.AddComponent<MeshRenderer>();
                
                // Создаём прозрачный материал с эмиссией
                highlightMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                highlightMaterial.SetFloat("_Surface", 1); // Transparent
                highlightMaterial.SetFloat("_Blend", 0); // Alpha
                highlightMaterial.color = highlightColor;
                
                // Включаем emission для свечения
                highlightMaterial.EnableKeyword("_EMISSION");
                highlightMaterial.SetColor("_EmissionColor", highlightColor * 2f);
                
                highlightRenderer.material = highlightMaterial;
                highlightRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            
            highlightObject.SetActive(false);
        }
        else
        {
            // Если нет renderer, создаём простую сферу подсветки
            CreateSimpleHighlight();
        }
    }
    
    /// <summary>
    /// Создать простую сферическую подсветку
    /// </summary>
    private void CreateSimpleHighlight()
    {
        highlightObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        highlightObject.name = "Highlight";
        highlightObject.transform.SetParent(transform);
        highlightObject.transform.localPosition = Vector3.zero;
        highlightObject.transform.localRotation = Quaternion.identity;
        highlightObject.transform.localScale = Vector3.one * 1.5f;
        
        // Удаляем коллайдер
        Collider col = highlightObject.GetComponent<Collider>();
        if (col != null) Destroy(col);
        
        // Создаём материал
        highlightMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        highlightMaterial.SetFloat("_Surface", 1); // Transparent
        highlightMaterial.color = highlightColor;
        
        highlightMaterial.EnableKeyword("_EMISSION");
        highlightMaterial.SetColor("_EmissionColor", highlightColor * 2f);
        
        highlightObject.GetComponent<Renderer>().material = highlightMaterial;
        highlightObject.SetActive(false);
    }
    
    /// <summary>
    /// Включить/выключить подсветку
    /// </summary>
    public void EnableHighlight(bool enable)
    {
        isHighlighted = enable;
        
        if (highlightObject != null)
        {
            highlightObject.SetActive(enable);
        }
        
        if (enable)
        {
            pulseTimer = 0f;
        }
    }
    
    /// <summary>
    /// Установить цвет подсветки
    /// </summary>
    public void SetHighlightColor(Color color)
    {
        highlightColor = color;
        
        if (highlightMaterial != null)
        {
            highlightMaterial.color = color;
            highlightMaterial.SetColor("_EmissionColor", color * 2f);
        }
    }
    
    /// <summary>
    /// Установить скорость пульсации
    /// </summary>
    public void SetPulseSpeed(float speed)
    {
        pulseSpeed = speed;
    }
    
    private void OnDestroy()
    {
        if (highlightObject != null)
        {
            Destroy(highlightObject);
        }
    }
}
