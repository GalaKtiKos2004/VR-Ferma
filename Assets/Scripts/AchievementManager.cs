using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Менеджер достижений — отслеживает и показывает достижения.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Header("Спрайты достижений")]
    [SerializeField] private Sprite firstCarrotSprite;
    [SerializeField] private Sprite firstOnionSprite;
    [SerializeField] private Sprite firstPumpkinSprite;
    [SerializeField] private Sprite firstTomatoSprite;
    [SerializeField] private Sprite gardenMasterSprite;

    [Header("UI")]
    [SerializeField] private AchievementUI achievementUI;

    private HashSet<string> unlockedAchievements = new HashSet<string>();
    private HashSet<string> firstHarvests = new HashSet<string>(); // Отслеживаем первые урожаи каждого типа

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        LoadAchievements();
    }

    private void Start()
    {
        // Показываем уже разблокированные достижения при старте
        ShowUnlockedAchievements();
    }

    /// <summary>
    /// Разблокировать достижение по ID.
    /// </summary>
    public void UnlockAchievement(string achievementId, Sprite icon = null)
    {
        if (unlockedAchievements.Contains(achievementId))
            return; // Уже разблокировано

        unlockedAchievements.Add(achievementId);
        SaveAchievements();

        if (achievementUI != null)
        {
            achievementUI.ShowAchievement(achievementId, icon);
        }

        Debug.Log($"[Achievement] Разблокировано: {achievementId}");
    }

    /// <summary>
    /// Проверить, разблокировано ли достижение.
    /// </summary>
    public bool IsUnlocked(string achievementId) => unlockedAchievements.Contains(achievementId);

    /// <summary>
    /// Разблокировать достижения для первого урожая.
    /// </summary>
    public void UnlockFirstHarvest(string cropType)
    {
        string id = "";
        Sprite icon = null;
        string normalizedType = cropType.ToLower().Trim();

        switch (normalizedType)
        {
            case "морковь":
            case "морковка":
                id = "first_carrot";
                icon = firstCarrotSprite;
                break;
            case "лук":
                id = "first_onion";
                icon = firstOnionSprite;
                break;
            case "тыква":
                id = "first_pumpkin";
                icon = firstPumpkinSprite;
                break;
            case "помидор":
                id = "first_tomato";
                icon = firstTomatoSprite;
                break;
        }

        if (!string.IsNullOrEmpty(id))
        {
            // Отмечаем, что этот тип собран впервые
            if (!firstHarvests.Contains(normalizedType))
            {
                firstHarvests.Add(normalizedType);
                UnlockAchievement(id, icon);
                
                // Проверяем, собраны ли все овощи
                CheckGardenMaster();
            }
        }
    }

    /// <summary>
    /// Проверить и разблокировать "Мастер сада" если собраны все овощи.
    /// </summary>
    private void CheckGardenMaster()
    {
        bool hasCarrot = firstHarvests.Contains("морковь") || firstHarvests.Contains("морковка");
        bool hasOnion = firstHarvests.Contains("лук");
        bool hasPumpkin = firstHarvests.Contains("тыква");
        bool hasTomato = firstHarvests.Contains("помидор");

        if (hasCarrot && hasOnion && hasPumpkin && hasTomato)
        {
            if (!IsUnlocked("garden_master"))
            {
                UnlockGardenMaster();
            }
        }
    }

    /// <summary>
    /// Разблокировать достижение "Мастер сада" (завершение туториала).
    /// </summary>
    public void UnlockGardenMaster()
    {
        UnlockAchievement("garden_master", gardenMasterSprite);
    }

    private void LoadAchievements()
    {
        // Загружаем из PlayerPrefs
        string saved = PlayerPrefs.GetString("Achievements", "");
        if (!string.IsNullOrEmpty(saved))
        {
            string[] ids = saved.Split(',');
            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                    unlockedAchievements.Add(id);
            }
        }

        // Восстанавливаем информацию о первых урожаях
        if (IsUnlocked("first_carrot")) firstHarvests.Add("морковь");
        if (IsUnlocked("first_onion")) firstHarvests.Add("лук");
        if (IsUnlocked("first_pumpkin")) firstHarvests.Add("тыква");
        if (IsUnlocked("first_tomato")) firstHarvests.Add("помидор");

        // Проверяем garden_master при загрузке (если все уже собраны, но достижение не разблокировано)
        if (!IsUnlocked("garden_master"))
        {
            CheckGardenMaster();
        }
    }

    /// <summary>
    /// Показать все уже разблокированные достижения при старте.
    /// </summary>
    private void ShowUnlockedAchievements()
    {
        if (achievementUI == null) return;

        foreach (string id in unlockedAchievements)
        {
            Sprite icon = GetSpriteForAchievement(id);
            if (icon != null && !achievementUI.IsShowing(id))
            {
                achievementUI.ShowAchievement(id, icon);
            }
        }
    }

    private Sprite GetSpriteForAchievement(string achievementId)
    {
        switch (achievementId)
        {
            case "first_carrot": return firstCarrotSprite;
            case "first_onion": return firstOnionSprite;
            case "first_pumpkin": return firstPumpkinSprite;
            case "first_tomato": return firstTomatoSprite;
            case "garden_master": return gardenMasterSprite;
            default: return null;
        }
    }

    private void SaveAchievements()
    {
        // Сохраняем в PlayerPrefs
        string[] ids = new string[unlockedAchievements.Count];
        unlockedAchievements.CopyTo(ids);
        PlayerPrefs.SetString("Achievements", string.Join(",", ids));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Сбросить все достижения (очистить данные и UI).
    /// </summary>
    public void ResetAchievements()
    {
        unlockedAchievements.Clear();
        firstHarvests.Clear();
        PlayerPrefs.DeleteKey("Achievements");
        PlayerPrefs.Save();

        if (achievementUI != null)
        {
            achievementUI.ClearAll();
        }

        Debug.Log("[Achievement] Все достижения сброшены!");
    }
}
