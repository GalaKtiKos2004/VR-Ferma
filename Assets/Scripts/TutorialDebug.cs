using UnityEngine;

/// <summary>
/// Инструменты для отладки и тестирования системы обучения
/// </summary>
public class TutorialDebug : MonoBehaviour
{
    [Header("Настройки отладки")]
    [SerializeField] private bool enableDebugKeys = true;
    [SerializeField] private KeyCode resetTutorialKey = KeyCode.R;
    [SerializeField] private KeyCode skipStepKey = KeyCode.N;
    [SerializeField] private KeyCode showInfoKey = KeyCode.I;
    
    private void Update()
    {
        if (!enableDebugKeys) return;
        
        // R - Сбросить обучение
        if (Input.GetKeyDown(resetTutorialKey))
        {
            ResetTutorial();
        }
        
        // N - Пропустить текущий шаг (для тестирования)
        if (Input.GetKeyDown(skipStepKey))
        {
            SkipCurrentStep();
        }
        
        // I - Показать информацию
        if (Input.GetKeyDown(showInfoKey))
        {
            ShowTutorialInfo();
        }
    }
    
    /// <summary>
    /// Сбросить обучение
    /// </summary>
    [ContextMenu("Сбросить обучение")]
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialCompleted");
        PlayerPrefs.Save();
        
        Debug.Log("[Tutorial Debug] Обучение сброшено! Перезапустите сцену.");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Обучение сброшено! Перезапустите сцену (R)", 3f);
        }
    }
    
    /// <summary>
    /// Пропустить текущий шаг (для быстрого тестирования)
    /// </summary>
    [ContextMenu("Пропустить шаг")]
    public void SkipCurrentStep()
    {
        if (TutorialManager.Instance == null)
        {
            Debug.LogWarning("[Tutorial Debug] TutorialManager не найден!");
            return;
        }
        
        int currentStep = TutorialManager.Instance.GetCurrentStep();
        Debug.Log($"[Tutorial Debug] Пропуск шага {currentStep}");
        
        // Симулируем завершение текущего шага (порядок: тяпка → грабли → семена → полив → 30 сек → сбор)
        switch (currentStep)
        {
            case 1:
                TutorialManager.Instance.OnHoeTaken();
                break;
            case 2:
                TutorialManager.Instance.OnBedTilled();
                break;
            case 3:
                TutorialManager.Instance.OnRakeTaken();
                break;
            case 4:
                TutorialManager.Instance.OnBedRaked();
                break;
            case 5:
                TutorialManager.Instance.OnSeedsTaken();
                break;
            case 6:
                TutorialManager.Instance.OnSeedPlanted();
                break;
            case 7:
                TutorialManager.Instance.OnCanTaken();
                break;
            case 8:
                TutorialManager.Instance.OnCanFilled();
                break;
            case 9:
                TutorialManager.Instance.OnPlantWatered();
                break;
            case 10:
                TutorialManager.Instance.OnHarvestCollected();
                break;
            default:
                Debug.Log("[Tutorial Debug] Нет активного шага для пропуска");
                break;
        }
    }
    
    /// <summary>
    /// Показать информацию о текущем состоянии обучения
    /// </summary>
    [ContextMenu("Показать информацию")]
    public void ShowTutorialInfo()
    {
        if (TutorialManager.Instance == null)
        {
            Debug.LogWarning("[Tutorial Debug] TutorialManager не найден!");
            return;
        }
        
        int currentStep = TutorialManager.Instance.GetCurrentStep();
        bool isActive = TutorialManager.Instance.IsTutorialActive();
        bool isCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        
        string stepName = GetStepName(currentStep);
        
        Debug.Log("=== ИНФОРМАЦИЯ ОБ ОБУЧЕНИИ ===");
        Debug.Log($"Обучение активно: {isActive}");
        Debug.Log($"Обучение завершено: {isCompleted}");
        Debug.Log($"Текущий шаг: {currentStep} - {stepName}");
        Debug.Log($"");
        Debug.Log("Клавиши отладки:");
        Debug.Log($"  {resetTutorialKey} - Сбросить обучение");
        Debug.Log($"  {skipStepKey} - Пропустить текущий шаг");
        Debug.Log($"  {showInfoKey} - Показать эту информацию");
        Debug.Log("=============================");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint($"Шаг {currentStep}: {stepName}", 4f);
        }
    }
    
    /// <summary>
    /// Получить название шага
    /// </summary>
    private string GetStepName(int step)
    {
        switch (step)
        {
            case 0: return "Обучение не начато";
            case 1: return "Взять тяпку";
            case 2: return "Взрыхлить грядку тяпкой";
            case 3: return "Взять грабли";
            case 4: return "Разрыхлить грядку граблями";
            case 5: return "Взять семена";
            case 6: return "Посадить семя";
            case 7: return "Взять лейку";
            case 8: return "Наполнить лейку";
            case 9: return "Полить росток";
            case 10: return "Собрать урожай";
            default: return "Неизвестный шаг";
        }
    }
    
    /// <summary>
    /// Завершить обучение принудительно
    /// </summary>
    [ContextMenu("Завершить обучение")]
    public void CompleteTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        
        Debug.Log("[Tutorial Debug] Обучение помечено как завершённое!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Обучение завершено!", 3f);
        }
    }
    
    // Оверлей «Tutorial Step / Debug Keys» в левом верхнем углу убран.
    // Клавиши R / N / I по-прежнему работают (сброс, пропуск шага, вывод в лог).
    private void OnGUI() { }
}
