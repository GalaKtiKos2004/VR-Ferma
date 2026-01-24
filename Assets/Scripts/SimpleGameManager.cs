using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Простой менеджер игры
/// </summary>
public class SimpleGameManager : MonoBehaviour
{
    public static SimpleGameManager Instance { get; private set; }
    
    [Header("Ресурсы игрока")]
    [SerializeField] private int coins = 0;
    [SerializeField] private int carrots = 0;
    [SerializeField] private int tomatoes = 0;
    [SerializeField] private int pumpkins = 0;
    [SerializeField] private int onions = 0;
    [SerializeField] private int eggs = 0;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI carrotsText;
    [SerializeField] private TextMeshProUGUI tomatoesText;
    [SerializeField] private TextMeshProUGUI eggsText;
    [SerializeField] private TextMeshProUGUI hintText;
    
    [Header("Настройки игры")]
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private bool gameStarted = false;
    
    private float hintTutorialLockUntil; // Блокировка перезаписи подсказки (шаг пройден)
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        gameStarted = true;
        UpdateUI();
        ShowHint("Добро пожаловать на ферму! Полейте грядку и посадите растение.");
    }
    
    private void Update()
    {
        if (gameStarted)
        {
            gameTime += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Добавить ресурсы
    /// </summary>
    public void AddCarrot(int amount = 1)
    {
        carrots += amount;
        coins += 10 * amount; // Монеты за морковь
        UpdateUI();
        ShowHint($"Собрано морковок: +{amount}! Монет: +{10 * amount}");
    }
    
    public void AddTomato(int amount = 1)
    {
        tomatoes += amount;
        coins += 15 * amount;
        UpdateUI();
        ShowHint($"Собрано помидоров: +{amount}! Монет: +{15 * amount}");
    }

    public void AddPumpkin(int amount = 1)
    {
        pumpkins += amount;
        coins += 12 * amount;
        UpdateUI();
        ShowHint($"Собрано тыкв: +{amount}! Монет: +{12 * amount}");
    }

    public void AddOnion(int amount = 1)
    {
        onions += amount;
        coins += 8 * amount;
        UpdateUI();
        ShowHint($"Собран лук: +{amount}! Монет: +{8 * amount}");
    }
    
    public void AddEgg(int amount = 1)
    {
        eggs += amount;
        coins += 5 * amount;
        UpdateUI();
        ShowHint($"Получено яиц: +{amount}! Монет: +{5 * amount}");
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }
    
    /// <summary>
    /// Показать подсказку. Не перезаписывает, если активно окно «Шаг пройден».
    /// </summary>
    public void ShowHint(string message, float duration = 3f)
    {
        if (Time.time < hintTutorialLockUntil)
            return;
        if (hintText != null)
        {
            hintText.gameObject.SetActive(true);
            hintText.text = message;
            CancelInvoke(nameof(ClearHint));
            Invoke(nameof(ClearHint), duration);
        }
        Debug.Log($"[Подсказка] {message}");
    }
    
    /// <summary>
    /// Очистить подсказку и скрыть окно
    /// </summary>
    private void ClearHint()
    {
        hintTutorialLockUntil = 0f;
        if (hintText != null)
        {
            hintText.text = "";
            hintText.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Показать «Шаг N пройден», следующий шаг; окно скрывается через 5 секунд
    /// </summary>
    public void ShowTutorialStepCompleted(int completedStep, string nextStepName)
    {
        hintTutorialLockUntil = Time.time + 5f;
        string msg = $"✓ Шаг {completedStep} пройден!\nСледующий шаг: {nextStepName}";
        if (hintText != null)
        {
            hintText.gameObject.SetActive(true);
            hintText.text = msg;
            CancelInvoke(nameof(ClearHint));
            Invoke(nameof(ClearHint), 5f);
        }
        Debug.Log($"[Подсказка] {msg}");
    }
    
    /// <summary>
    /// Показать завершение обучения; окно скрывается через 5 секунд
    /// </summary>
    public void ShowTutorialFinished(string message)
    {
        hintTutorialLockUntil = Time.time + 5f;
        if (hintText != null)
        {
            hintText.gameObject.SetActive(true);
            hintText.text = message;
            CancelInvoke(nameof(ClearHint));
            Invoke(nameof(ClearHint), 5f);
        }
        Debug.Log($"[Подсказка] {message}");
    }
    
    /// <summary>
    /// Обновить UI
    /// </summary>
    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Монеты: {coins}";
        }
        
        if (carrotsText != null)
        {
            carrotsText.text = $"Морковь: {carrots}";
        }
        
        if (tomatoesText != null)
        {
            tomatoesText.text = $"Помидоры: {tomatoes}";
        }
        
        if (eggsText != null)
        {
            eggsText.text = $"Яйца: {eggs}";
        }
    }
    
    // Геттеры
    public int GetCoins() => coins;
    public int GetCarrots() => carrots;
    public int GetTomatoes() => tomatoes;
    public int GetPumpkins() => pumpkins;
    public int GetOnions() => onions;
    public int GetEggs() => eggs;
    public float GetGameTime() => gameTime;
}
