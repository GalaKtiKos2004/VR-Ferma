using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Менеджер достижений — отслеживает и показывает достижения.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Header("Спрайты достижений - Овощи")]
    [SerializeField] private Sprite firstCarrotSprite;
    [SerializeField] private Sprite firstOnionSprite;
    [SerializeField] private Sprite firstPumpkinSprite;
    [SerializeField] private Sprite firstTomatoSprite;
    [SerializeField] private Sprite gardenMasterSprite;
    
    [Header("Спрайты достижений - Животные")]
    [SerializeField] private Sprite firstLoveSprite; // Первое поглаженное животное
    [SerializeField] private Sprite firstEatSprite; // Первое покормленное животное
    [SerializeField] private Sprite farmerLoveSprite; // Всех поглаженных животных
    [SerializeField] private Sprite animalHappySprite; // Всех покормленных животных
    [SerializeField] private Sprite masterAnimalSprite; // Все достижения о животных
    [SerializeField] private Sprite allAchievementsSprite; // Все достижения вообще

    [Header("UI")]
    [SerializeField] private AchievementUI achievementUI;

    private HashSet<string> unlockedAchievements = new HashSet<string>();
    private HashSet<string> firstHarvests = new HashSet<string>(); // Отслеживаем первые урожаи каждого типа
    
    // Отслеживание животных
    private HashSet<Animal> pettedAnimals = new HashSet<Animal>(); // Поглаженные животные
    private HashSet<Animal> fedAnimals = new HashSet<Animal>(); // Покормленные животные
    private bool firstPetUnlocked = false; // Первое поглаживание
    private bool firstFeedUnlocked = false; // Первое кормление

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
        {
            Debug.Log($"[Achievement] Достижение {achievementId} уже разблокировано");
            return; // Уже разблокировано
        }

        unlockedAchievements.Add(achievementId);
        SaveAchievements();

        // Если спрайт не передан, пытаемся получить его из GetSpriteForAchievement
        if (icon == null)
        {
            icon = GetSpriteForAchievement(achievementId);
            if (icon == null)
            {
                Debug.LogWarning($"[Achievement] Спрайт для достижения {achievementId} не найден! Проверьте настройки в Inspector.");
            }
        }

        if (achievementUI != null)
        {
            if (icon != null)
            {
                achievementUI.ShowAchievement(achievementId, icon);
            }
            else
            {
                Debug.LogWarning($"[Achievement] Не удалось показать достижение {achievementId}: спрайт отсутствует");
            }
        }
        else
        {
            Debug.LogWarning($"[Achievement] AchievementUI не назначен! Проверьте настройки в Inspector.");
        }

        Debug.Log($"[Achievement] Разблокировано: {achievementId}, спрайт: {(icon != null ? icon.name : "NULL")}");
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
        
        // Восстанавливаем информацию о животных
        if (IsUnlocked("first_pet")) firstPetUnlocked = true;
        if (IsUnlocked("first_feed")) firstFeedUnlocked = true;
        
        // Загружаем информацию о поглаженных/покормленных животных
        // Примечание: так как Animal объекты могут пересоздаваться, мы просто проверяем достижения
        // и восстанавливаем состояние при первом взаимодействии

        // Проверяем garden_master при загрузке (если все уже собраны, но достижение не разблокировано)
        if (!IsUnlocked("garden_master"))
        {
            CheckGardenMaster();
        }
        
        // Проверяем достижения животных при загрузке (вызывается после Start, когда все объекты готовы)
        StartCoroutine(CheckAnimalAchievementsDelayed());
    }
    
    /// <summary>
    /// Проверить достижения животных с задержкой (чтобы все Animal объекты успели инициализироваться).
    /// </summary>
    private System.Collections.IEnumerator CheckAnimalAchievementsDelayed()
    {
        yield return new WaitForSeconds(0.5f); // Ждем инициализацию всех объектов
        CheckAnimalAchievements();
    }

    /// <summary>
    /// Показать все уже разблокированные достижения при старте.
    /// </summary>
    private void ShowUnlockedAchievements()
    {
        if (achievementUI == null)
        {
            Debug.LogWarning("[Achievement] AchievementUI не назначен! Достижения не будут отображаться.");
            return;
        }

        Debug.Log($"[Achievement] Показываем {unlockedAchievements.Count} разблокированных достижений");
        
        foreach (string id in unlockedAchievements)
        {
            Sprite icon = GetSpriteForAchievement(id);
            if (icon != null)
            {
                if (!achievementUI.IsShowing(id))
                {
                    achievementUI.ShowAchievement(id, icon);
                    Debug.Log($"[Achievement] Показано достижение при старте: {id}");
                }
                else
                {
                    Debug.Log($"[Achievement] Достижение {id} уже отображается");
                }
            }
            else
            {
                Debug.LogWarning($"[Achievement] Спрайт для достижения {id} не найден! Проверьте настройки в Inspector.");
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
            case "first_pet": return firstLoveSprite;
            case "first_feed": return firstEatSprite;
            case "all_pets": return farmerLoveSprite;
            case "all_feeds": return animalHappySprite;
            case "master_animal": return masterAnimalSprite;
            case "all_achievements": return allAchievementsSprite;
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
        
        Debug.Log($"[Achievement] Сохранено достижений: {unlockedAchievements.Count}");
    }

    /// <summary>
    /// Разблокировать достижение за первое поглаживание животного.
    /// </summary>
    public void UnlockFirstPet(Animal animal)
    {
        if (firstPetUnlocked)
        {
            Debug.Log("[Achievement] Первое поглаживание уже разблокировано");
            return;
        }
        
        if (animal != null)
        {
            firstPetUnlocked = true;
            Debug.Log($"[Achievement] Разблокируем first_pet, спрайт: {(firstLoveSprite != null ? firstLoveSprite.name : "NULL")}");
            UnlockAchievement("first_pet", firstLoveSprite);
            
            // Проверяем, все ли животные поглажены
            CheckAllPets();
        }
        else
        {
            Debug.LogWarning("[Achievement] UnlockFirstPet вызван с null животным!");
        }
    }
    
    /// <summary>
    /// Разблокировать достижение за первое кормление животного.
    /// </summary>
    public void UnlockFirstFeed(Animal animal)
    {
        if (firstFeedUnlocked)
        {
            Debug.Log("[Achievement] Первое кормление уже разблокировано");
            return;
        }
        
        if (animal != null)
        {
            firstFeedUnlocked = true;
            Debug.Log($"[Achievement] Разблокируем first_feed, спрайт: {(firstEatSprite != null ? firstEatSprite.name : "NULL")}");
            UnlockAchievement("first_feed", firstEatSprite);
            
            // Проверяем, все ли животные покормлены
            CheckAllFeeds();
        }
        else
        {
            Debug.LogWarning("[Achievement] UnlockFirstFeed вызван с null животным!");
        }
    }
    
    /// <summary>
    /// Зарегистрировать поглаживание животного.
    /// </summary>
    public void RegisterPet(Animal animal)
    {
        if (animal == null)
        {
            Debug.LogWarning("[Achievement] RegisterPet вызван с null животным!");
            return;
        }
        
        Debug.Log($"[Achievement] RegisterPet вызван для {animal.name}");
        
        if (!pettedAnimals.Contains(animal))
        {
            pettedAnimals.Add(animal);
            Debug.Log($"[Achievement] Добавлено поглаженное животное: {animal.name}, всего: {pettedAnimals.Count}");
            
            // Первое поглаживание
            if (!firstPetUnlocked)
            {
                Debug.Log("[Achievement] Это первое поглаживание!");
                UnlockFirstPet(animal);
            }
            else
            {
                // Проверяем, все ли животные поглажены
                CheckAllPets();
            }
        }
        else
        {
            Debug.Log($"[Achievement] Животное {animal.name} уже было поглажено ранее");
        }
    }
    
    /// <summary>
    /// Зарегистрировать кормление животного.
    /// </summary>
    public void RegisterFeed(Animal animal)
    {
        if (animal == null)
        {
            Debug.LogWarning("[Achievement] RegisterFeed вызван с null животным!");
            return;
        }
        
        Debug.Log($"[Achievement] RegisterFeed вызван для {animal.name}");
        
        if (!fedAnimals.Contains(animal))
        {
            fedAnimals.Add(animal);
            Debug.Log($"[Achievement] Добавлено покормленное животное: {animal.name}, всего: {fedAnimals.Count}");
            
            // Первое кормление
            if (!firstFeedUnlocked)
            {
                Debug.Log("[Achievement] Это первое кормление!");
                UnlockFirstFeed(animal);
            }
            else
            {
                // Проверяем, все ли животные покормлены
                CheckAllFeeds();
            }
        }
        else
        {
            Debug.Log($"[Achievement] Животное {animal.name} уже было покормлено ранее");
        }
    }
    
    /// <summary>
    /// Проверить, все ли животные поглажены.
    /// </summary>
    private void CheckAllPets()
    {
        Animal[] allAnimals = FindObjectsOfType<Animal>();
        
        Debug.Log($"[Achievement] CheckAllPets: найдено животных на сцене: {allAnimals.Length}, поглажено: {pettedAnimals.Count}");
        
        if (allAnimals.Length == 0)
        {
            Debug.LogWarning("[Achievement] CheckAllPets: на сцене нет животных!");
            return;
        }
        
        bool allPetted = true;
        foreach (Animal animal in allAnimals)
        {
            if (!pettedAnimals.Contains(animal))
            {
                allPetted = false;
                Debug.Log($"[Achievement] CheckAllPets: животное {animal.name} еще не поглажено");
                break;
            }
        }
        
        Debug.Log($"[Achievement] CheckAllPets: все поглажены = {allPetted}, достижение разблокировано = {IsUnlocked("all_pets")}");
        
        if (allPetted && !IsUnlocked("all_pets"))
        {
            Debug.Log("[Achievement] Разблокируем all_pets!");
            UnlockAchievement("all_pets", farmerLoveSprite);
            CheckMasterAnimal();
        }
    }
    
    /// <summary>
    /// Проверить, все ли животные покормлены.
    /// </summary>
    private void CheckAllFeeds()
    {
        Animal[] allAnimals = FindObjectsOfType<Animal>();
        
        Debug.Log($"[Achievement] CheckAllFeeds: найдено животных на сцене: {allAnimals.Length}, покормлено: {fedAnimals.Count}");
        
        if (allAnimals.Length == 0)
        {
            Debug.LogWarning("[Achievement] CheckAllFeeds: на сцене нет животных!");
            return;
        }
        
        bool allFed = true;
        foreach (Animal animal in allAnimals)
        {
            if (!fedAnimals.Contains(animal))
            {
                allFed = false;
                Debug.Log($"[Achievement] CheckAllFeeds: животное {animal.name} еще не покормлено");
                break;
            }
        }
        
        Debug.Log($"[Achievement] CheckAllFeeds: все покормлены = {allFed}, достижение разблокировано = {IsUnlocked("all_feeds")}");
        
        if (allFed && !IsUnlocked("all_feeds"))
        {
            Debug.Log("[Achievement] Разблокируем all_feeds!");
            UnlockAchievement("all_feeds", animalHappySprite);
            CheckMasterAnimal();
        }
    }
    
    /// <summary>
    /// Проверить достижение "Мастер животных" (все достижения о животных выполнены).
    /// </summary>
    private void CheckMasterAnimal()
    {
        bool hasFirstPet = IsUnlocked("first_pet");
        bool hasFirstFeed = IsUnlocked("first_feed");
        bool hasAllPets = IsUnlocked("all_pets");
        bool hasAllFeeds = IsUnlocked("all_feeds");
        
        if (hasFirstPet && hasFirstFeed && hasAllPets && hasAllFeeds)
        {
            if (!IsUnlocked("master_animal"))
            {
                UnlockAchievement("master_animal", masterAnimalSprite);
                CheckAllAchievements();
            }
        }
    }
    
    /// <summary>
    /// Проверить достижение "Все достижения" (все достижения в игре выполнены).
    /// </summary>
    private void CheckAllAchievements()
    {
        // Проверяем все достижения
        bool hasGardenMaster = IsUnlocked("garden_master");
        bool hasMasterAnimal = IsUnlocked("master_animal");
        
        // Проверяем все первые урожаи
        bool hasAllHarvests = firstHarvests.Contains("морковь") || firstHarvests.Contains("морковка");
        hasAllHarvests = hasAllHarvests && firstHarvests.Contains("лук");
        hasAllHarvests = hasAllHarvests && firstHarvests.Contains("тыква");
        hasAllHarvests = hasAllHarvests && firstHarvests.Contains("помидор");
        
        if (hasGardenMaster && hasMasterAnimal && hasAllHarvests)
        {
            if (!IsUnlocked("all_achievements"))
            {
                UnlockAchievement("all_achievements", allAchievementsSprite);
            }
        }
    }
    
    /// <summary>
    /// Проверить все достижения животных (вызывается при загрузке).
    /// </summary>
    private void CheckAnimalAchievements()
    {
        // Проверяем достижения животных
        CheckAllPets();
        CheckAllFeeds();
        CheckMasterAnimal();
        CheckAllAchievements();
    }
    
    /// <summary>
    /// Сбросить все достижения (очистить данные и UI).
    /// </summary>
    public void ResetAchievements()
    {
        unlockedAchievements.Clear();
        firstHarvests.Clear();
        pettedAnimals.Clear();
        fedAnimals.Clear();
        firstPetUnlocked = false;
        firstFeedUnlocked = false;
        PlayerPrefs.DeleteKey("Achievements");
        PlayerPrefs.Save();

        if (achievementUI != null)
        {
            achievementUI.ClearAll();
        }

        Debug.Log("[Achievement] Все достижения сброшены!");
    }
    
    /// <summary>
    /// Тестовая разблокировка достижения (для отладки).
    /// </summary>
    [ContextMenu("Тест: Разблокировать first_pet")]
    public void TestUnlockFirstPet()
    {
        UnlockAchievement("first_pet", firstLoveSprite);
    }
    
    [ContextMenu("Тест: Разблокировать first_feed")]
    public void TestUnlockFirstFeed()
    {
        UnlockAchievement("first_feed", firstEatSprite);
    }
    
    [ContextMenu("Тест: Разблокировать все достижения животных")]
    public void TestUnlockAllAnimalAchievements()
    {
        UnlockAchievement("first_pet", firstLoveSprite);
        UnlockAchievement("first_feed", firstEatSprite);
        UnlockAchievement("all_pets", farmerLoveSprite);
        UnlockAchievement("all_feeds", animalHappySprite);
        UnlockAchievement("master_animal", masterAnimalSprite);
    }
    
    [ContextMenu("Проверить настройки достижений")]
    public void CheckAchievementSettings()
    {
        Debug.Log("=== ПРОВЕРКА НАСТРОЕК ДОСТИЖЕНИЙ ===");
        Debug.Log($"AchievementUI назначен: {achievementUI != null}");
        Debug.Log($"First Love Sprite: {(firstLoveSprite != null ? firstLoveSprite.name : "NULL")}");
        Debug.Log($"First Eat Sprite: {(firstEatSprite != null ? firstEatSprite.name : "NULL")}");
        Debug.Log($"Farmer Love Sprite: {(farmerLoveSprite != null ? farmerLoveSprite.name : "NULL")}");
        Debug.Log($"Animal Happy Sprite: {(animalHappySprite != null ? animalHappySprite.name : "NULL")}");
        Debug.Log($"Master Animal Sprite: {(masterAnimalSprite != null ? masterAnimalSprite.name : "NULL")}");
        Debug.Log($"All Achievements Sprite: {(allAchievementsSprite != null ? allAchievementsSprite.name : "NULL")}");
        Debug.Log($"Разблокировано достижений: {unlockedAchievements.Count}");
        foreach (string id in unlockedAchievements)
        {
            Debug.Log($"  - {id}");
        }
        Debug.Log("=====================================");
    }
}
