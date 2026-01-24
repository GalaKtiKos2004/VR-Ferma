using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// UI для отображения достижений в правом верхнем углу экрана.
/// Достижения остаются на экране постоянно.
/// </summary>
public class AchievementUI : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private Transform achievementContainer; // Контейнер для иконок
    [SerializeField] private GameObject achievementIconPrefab; // Префаб иконки достижения
    [SerializeField] private float iconSize = 80f; // Размер иконки
    [SerializeField] private float spacing = 10f; // Отступ между иконками
    [SerializeField] private Vector2 anchorPosition = new Vector2(1f, 1f); // Правый верхний угол
    [SerializeField] private Vector2 offset = new Vector2(-20f, -20f); // Смещение от угла

    private Dictionary<string, GameObject> activeIcons = new Dictionary<string, GameObject>();
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }

        // Настройка позиции в правом верхнем углу
        if (rectTransform != null)
        {
            rectTransform.anchorMin = anchorPosition;
            rectTransform.anchorMax = anchorPosition;
            rectTransform.pivot = anchorPosition;
            rectTransform.anchoredPosition = offset;
        }

        // Если контейнер не задан, используем сам объект
        if (achievementContainer == null)
            achievementContainer = transform;
    }

    /// <summary>
    /// Показать достижение и оставить его на экране.
    /// </summary>
    public void ShowAchievement(string achievementId, Sprite icon)
    {
        if (string.IsNullOrEmpty(achievementId) || icon == null)
        {
            Debug.LogWarning($"[AchievementUI] Неверные параметры: id={achievementId}, icon={icon}");
            return;
        }

        // Если уже показано, не дублируем
        if (activeIcons.ContainsKey(achievementId))
        {
            Debug.Log($"[AchievementUI] Достижение {achievementId} уже отображается");
            return;
        }

        // Создаём иконку
        GameObject iconObj;
        if (achievementIconPrefab != null)
        {
            iconObj = Instantiate(achievementIconPrefab, achievementContainer);
        }
        else
        {
            // Создаём простую иконку если нет префаба
            iconObj = new GameObject($"Achievement_{achievementId}");
            iconObj.transform.SetParent(achievementContainer, false);
            
            Image img = iconObj.AddComponent<Image>();
            img.sprite = icon;
            img.preserveAspect = true;
        }

        // Настройка RectTransform
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        if (iconRect == null)
            iconRect = iconObj.AddComponent<RectTransform>();

        iconRect.sizeDelta = new Vector2(iconSize, iconSize);
        iconRect.anchorMin = new Vector2(1f, 1f);
        iconRect.anchorMax = new Vector2(1f, 1f);
        iconRect.pivot = new Vector2(1f, 1f);

        // Устанавливаем спрайт если есть Image
        Image image = iconObj.GetComponent<Image>();
        if (image != null && icon != null)
        {
            image.sprite = icon;
        }

        activeIcons[achievementId] = iconObj;

        // Обновляем позиции всех иконок
        UpdateIconPositions();

        Debug.Log($"[AchievementUI] Показано достижение: {achievementId}");
    }

    /// <summary>
    /// Обновить позиции всех иконок (после добавления/удаления).
    /// </summary>
    private void UpdateIconPositions()
    {
        int index = 0;
        var keysToRemove = new List<string>();
        
        foreach (var kvp in activeIcons)
        {
            if (kvp.Value == null)
            {
                keysToRemove.Add(kvp.Key);
                continue;
            }

            RectTransform iconRect = kvp.Value.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                // Позиционируем в ряд слева направо: первая иконка у правого края, остальные левее (в длину)
                float xPos = offset.x - (iconSize + spacing) * index;
                iconRect.anchoredPosition = new Vector2(xPos, offset.y);
            }
            index++;
        }

        // Удаляем null-элементы
        foreach (var key in keysToRemove)
            activeIcons.Remove(key);
    }

    /// <summary>
    /// Проверить, отображается ли достижение.
    /// </summary>
    public bool IsShowing(string achievementId) => activeIcons.ContainsKey(achievementId);

    /// <summary>
    /// Удалить все иконки достижений с экрана.
    /// </summary>
    public void ClearAll()
    {
        foreach (var kvp in activeIcons)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value);
        }
        activeIcons.Clear();
        Debug.Log("[AchievementUI] Все иконки достижений удалены с экрана.");
    }
}
