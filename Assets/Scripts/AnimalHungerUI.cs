using UnityEngine;

/// <summary>
/// 3D индикатор голодности животного (над головой)
/// </summary>
public class AnimalHungerUI : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Animal animal;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private Vector2 barSize = new Vector2(1f, 0.1f);
    
    [Header("3D объекты")]
    [SerializeField] private GameObject barBackground;
    [SerializeField] private GameObject barFill;
    
    [Header("Цвета")]
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color hungryColor = Color.red;
    
    private Camera mainCamera;
    private Material fillMaterial;
    private Vector3 maxFillScale;
    
    private void OnDestroy()
    {
        // Удаляем объекты при уничтожении компонента
        if (barBackground != null)
        {
            Destroy(barBackground);
        }
        if (barFill != null)
        {
            Destroy(barFill);
        }
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
        
        if (animal == null)
        {
            animal = GetComponent<Animal>();
        }
        
        if (animal == null)
        {
            Debug.LogError("Animal компонент не найден!");
            enabled = false;
            return;
        }
        
        // Создаём 3D индикатор
        CreateHungerBar();
    }
    
    private void Update()
    {
        if (animal == null || barFill == null) return;
        
        // Обновляем позицию индикатора
        UpdatePosition();
        
        // Обновляем заполнение и цвет
        UpdateFillAmount();
        
        // Поворачиваем к камере
        if (mainCamera != null && barBackground != null)
        {
            Vector3 lookPos = mainCamera.transform.position;
            lookPos.y = barBackground.transform.position.y; // Только горизонтальный поворот
            barBackground.transform.LookAt(lookPos);
            barFill.transform.rotation = barBackground.transform.rotation;
        }
    }
    
    /// <summary>
    /// Обновить позицию индикатора
    /// </summary>
    private void UpdatePosition()
    {
        Vector3 targetPos = animal.transform.position + offset;
        
        if (barBackground != null)
        {
            barBackground.transform.position = targetPos;
        }
        if (barFill != null)
        {
            barFill.transform.position = targetPos;
        }
    }
    
    /// <summary>
    /// Обновить заполнение индикатора
    /// </summary>
    private void UpdateFillAmount()
    {
        if (barFill == null) return;
        
        float percentage = animal.GetHappinessPercentage();
        
        // Изменяем scale по X для эффекта заполнения
        Vector3 newScale = maxFillScale;
        newScale.x = maxFillScale.x * percentage;
        barFill.transform.localScale = newScale;
        
        // Сдвигаем позицию влево при уменьшении
        Vector3 localOffset = Vector3.right * (maxFillScale.x * (1f - percentage) * 0.5f);
        barFill.transform.localPosition = -localOffset;
        
        // Меняем цвет в зависимости от уровня
        if (fillMaterial != null)
        {
            fillMaterial.color = Color.Lerp(hungryColor, fullColor, percentage);
        }
    }
    
    /// <summary>
    /// Создать 3D индикатор голода
    /// </summary>
    private void CreateHungerBar()
    {
        Vector3 startPos = animal.transform.position + offset;
        
        // Создаём фон (тёмный Quad)
        barBackground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        barBackground.name = $"HungerBar_BG_{animal.name}";
        barBackground.transform.position = startPos;
        barBackground.transform.localScale = new Vector3(barSize.x, barSize.y, 1f);
        
        Material bgMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        bgMaterial.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        barBackground.GetComponent<Renderer>().material = bgMaterial;
        
        // Удаляем коллайдер
        Collider bgCol = barBackground.GetComponent<Collider>();
        if (bgCol != null) Destroy(bgCol);
        
        // Создаём заполнение (цветной Quad)
        barFill = GameObject.CreatePrimitive(PrimitiveType.Quad);
        barFill.name = $"HungerBar_Fill_{animal.name}";
        barFill.transform.position = startPos + Vector3.forward * -0.01f; // Чуть впереди фона
        
        maxFillScale = new Vector3(barSize.x * 0.95f, barSize.y * 0.8f, 1f); // Чуть меньше фона
        barFill.transform.localScale = maxFillScale;
        
        fillMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        fillMaterial.color = fullColor;
        barFill.GetComponent<Renderer>().material = fillMaterial;
        
        // Удаляем коллайдер
        Collider fillCol = barFill.GetComponent<Collider>();
        if (fillCol != null) Destroy(fillCol);
        
        Debug.Log($"3D индикатор голода создан для {animal.name}!");
    }
}
