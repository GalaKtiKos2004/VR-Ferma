using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Менеджер обучения - управляет этапами туториала.
/// Порядок: тяпка → грабли → семена → поливка → 30 сек рост → сбор.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    
    [Header("Настройки обучения")]
    [SerializeField] private bool tutorialEnabled = true;
    [SerializeField] private bool tutorialCompleted = false;
    
    [Header("Этап 1: Первое растение")]
    [SerializeField] private GameObject barn; // Сарай
    [SerializeField] private GameObject hoe; // Тяпка (сначала!)
    [SerializeField] private GameObject rake; // Грабли (потом!)
    [SerializeField] private GameObject seedBag; // Семена (устаревшее, используйте тыква/помидор)
    [SerializeField] private GameObject seedBagPumpkin; // Семена тыквы
    [SerializeField] private GameObject seedBagTomato;  // Семена помидоров
    [SerializeField] private GameObject seedBagCarrot;  // Семена моркови
    [SerializeField] private GameObject seedBagOnion;   // Семена лука
    [SerializeField] private PlantBed tutorialPlantBed; // Обучающая грядка
    [SerializeField] private GameObject wateringCan; // Лейка
    [SerializeField] private GameObject well; // Колодец
    
    [Header("Подсветка")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 0.5f);
    [SerializeField] private float pulseSpeed = 2f;
    
    private int currentStep = 0;
    private Dictionary<GameObject, TutorialHighlight> highlights = new Dictionary<GameObject, TutorialHighlight>();
    
    // Флаги: тяпка → взрыхлить тяпкой → грабли → разрыхлить граблями → семена → посадка → лейка → наполнить → полив → сбор
    private bool hoeTaken = false;
    private bool bedTilled = false;
    private bool rakeTaken = false;
    private bool bedRaked = false;
    private bool seedsTaken = false;
    private bool seedPlanted = false;
    private bool canTaken = false;
    private bool canFilled = false;
    private bool plantWatered = false;
    private bool harvestCollected = false;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        if (tutorialEnabled && !tutorialCompleted)
        {
            InitializeHighlights();
            StartCoroutine(StartTutorial());
        }
    }
    
    private void InitializeHighlights()
    {
        AddHighlight(barn);
        AddHighlight(hoe);
        AddHighlight(rake);
        AddHighlight(seedBag);
        AddHighlight(seedBagPumpkin);
        AddHighlight(seedBagTomato);
        AddHighlight(seedBagCarrot);
        AddHighlight(seedBagOnion);
        AddHighlight(tutorialPlantBed?.gameObject);
        AddHighlight(wateringCan);
        AddHighlight(well);
    }
    
    private void AddHighlight(GameObject obj)
    {
        if (obj == null) return;
        TutorialHighlight h = obj.GetComponent<TutorialHighlight>();
        if (h == null) h = obj.AddComponent<TutorialHighlight>();
        h.SetHighlightColor(highlightColor);
        h.SetPulseSpeed(pulseSpeed);
        highlights[obj] = h;
    }
    
    private IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1f);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint("Добро пожаловать на ферму! Начнём обучение.", 4f);
        yield return new WaitForSeconds(4f);
        StartStep1_TakeHoe();
    }
    
    // ========== 1. Тяпка ==========
    private void StartStep1_TakeHoe()
    {
        currentStep = 1;
        Debug.Log("[Tutorial] Этап 1: Взять тяпку");
        HighlightObject(barn);
        HighlightObject(hoe);
        UnhighlightObject(rake);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint("Возьмите тяпку из сарая", 5f);
    }
    
    public void OnHoeTaken()
    {
        if (currentStep != 1 || hoeTaken) return;
        hoeTaken = true;
        Debug.Log("[Tutorial] Тяпка взята!");
        UnhighlightObject(barn);
        UnhighlightObject(hoe);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(1, GetStepName(2));
        StartCoroutine(WaitAndStartStep2());
    }
    
    private IEnumerator WaitAndStartStep2()
    {
        yield return new WaitForSeconds(1f);
        StartStep2_TillBed();
    }
    
    // ========== 2. Взрыхлить тяпкой ==========
    private void StartStep2_TillBed()
    {
        currentStep = 2;
        Debug.Log("[Tutorial] Этап 2: Взрыхлить грядку тяпкой");
        HighlightObject(tutorialPlantBed?.gameObject);
    }
    
    public void OnBedTilled()
    {
        if (currentStep != 2 || bedTilled) return;
        bedTilled = true;
        Debug.Log("[Tutorial] Грядка взрыхлена!");
        UnhighlightObject(tutorialPlantBed?.gameObject);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(2, GetStepName(3));
        StartCoroutine(WaitAndStartStep3());
    }
    
    private IEnumerator WaitAndStartStep3()
    {
        yield return new WaitForSeconds(1f);
        StartStep3_TakeRake();
    }
    
    // ========== 3. Грабли ==========
    private void StartStep3_TakeRake()
    {
        currentStep = 3;
        Debug.Log("[Tutorial] Этап 3: Взять грабли");
        HighlightObject(barn);
        HighlightObject(rake);
        UnhighlightObject(hoe);
    }
    
    public void OnRakeTaken()
    {
        if (currentStep != 3 || rakeTaken) return;
        rakeTaken = true;
        Debug.Log("[Tutorial] Грабли взяты!");
        UnhighlightObject(barn);
        UnhighlightObject(rake);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(3, GetStepName(4));
        StartCoroutine(WaitAndStartStep4());
    }
    
    private IEnumerator WaitAndStartStep4()
    {
        yield return new WaitForSeconds(1f);
        StartStep4_RakeBed();
    }
    
    // ========== 4. Разрыхлить граблями ==========
    private void StartStep4_RakeBed()
    {
        currentStep = 4;
        Debug.Log("[Tutorial] Этап 4: Разрыхлить грядку граблями");
        HighlightObject(tutorialPlantBed?.gameObject);
    }
    
    public void OnBedRaked()
    {
        if (currentStep != 4 || bedRaked) return;
        bedRaked = true;
        Debug.Log("[Tutorial] Грядка разрыхлена граблями!");
        UnhighlightObject(tutorialPlantBed?.gameObject);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(4, GetStepName(5));
        StartCoroutine(WaitAndStartStep5());
    }
    
    private IEnumerator WaitAndStartStep5()
    {
        yield return new WaitForSeconds(1f);
        StartStep5_TakeSeeds();
    }
    
    // ========== 5. Семена ==========
    private void StartStep5_TakeSeeds()
    {
        currentStep = 5;
        Debug.Log("[Tutorial] Этап 5: Взять семена");
        HighlightObject(barn);
        HighlightObject(seedBag);
        HighlightObject(seedBagPumpkin);
        HighlightObject(seedBagTomato);
        HighlightObject(seedBagCarrot);
        HighlightObject(seedBagOnion);
    }
    
    public void OnSeedsTaken()
    {
        if (currentStep != 5 || seedsTaken) return;
        seedsTaken = true;
        Debug.Log("[Tutorial] Семена взяты!");
        UnhighlightObject(barn);
        UnhighlightObject(seedBag);
        UnhighlightObject(seedBagPumpkin);
        UnhighlightObject(seedBagTomato);
        UnhighlightObject(seedBagCarrot);
        UnhighlightObject(seedBagOnion);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(5, GetStepName(6));
        StartCoroutine(WaitAndStartStep6());
    }
    
    private IEnumerator WaitAndStartStep6()
    {
        yield return new WaitForSeconds(1f);
        StartStep6_PlantSeed();
    }
    
    // ========== 6. Посадка ==========
    private void StartStep6_PlantSeed()
    {
        currentStep = 6;
        Debug.Log("[Tutorial] Этап 6: Посадить семя");
        HighlightObject(tutorialPlantBed?.gameObject);
    }
    
    public void OnSeedPlanted()
    {
        if (currentStep != 6 || seedPlanted) return;
        seedPlanted = true;
        Debug.Log("[Tutorial] Семя посажено!");
        UnhighlightObject(tutorialPlantBed?.gameObject);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(6, GetStepName(7));
        StartCoroutine(WaitAndStartStep7());
    }
    
    private IEnumerator WaitAndStartStep7()
    {
        yield return new WaitForSeconds(1f);
        StartStep7_TakeCan();
    }
    
    // ========== 7. Лейка ==========
    private void StartStep7_TakeCan()
    {
        currentStep = 7;
        Debug.Log("[Tutorial] Этап 7: Взять лейку");
        HighlightObject(wateringCan);
    }
    
    public void OnCanTaken()
    {
        if (currentStep != 7 || canTaken) return;
        canTaken = true;
        Debug.Log("[Tutorial] Лейка взята!");
        UnhighlightObject(wateringCan);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(7, GetStepName(8));
        StartCoroutine(WaitAndStartStep8());
    }
    
    private IEnumerator WaitAndStartStep8()
    {
        yield return new WaitForSeconds(1f);
        StartStep8_FillCan();
    }
    
    // ========== 8. Наполнить лейку (поливка) ==========
    private void StartStep8_FillCan()
    {
        currentStep = 8;
        Debug.Log("[Tutorial] Этап 8: Наполнить лейку");
        HighlightObject(well);
    }
    
    public void OnCanFilled()
    {
        if (currentStep != 8 || canFilled) return;
        canFilled = true;
        Debug.Log("[Tutorial] Лейка наполнена!");
        UnhighlightObject(well);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(8, GetStepName(9));
        StartCoroutine(WaitAndStartStep9());
    }
    
    private IEnumerator WaitAndStartStep9()
    {
        yield return new WaitForSeconds(1f);
        StartStep9_WaterPlant();
    }
    
    // ========== 9. Полить ==========
    private void StartStep9_WaterPlant()
    {
        currentStep = 9;
        Debug.Log("[Tutorial] Этап 9: Полить растение");
        HighlightObject(tutorialPlantBed?.gameObject);
    }
    
    public void OnPlantWatered()
    {
        if (currentStep != 9 || plantWatered) return;
        plantWatered = true;
        Debug.Log("[Tutorial] Растение полито!");
        UnhighlightObject(tutorialPlantBed?.gameObject);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialStepCompleted(9, "Собрать урожай (через 30 сек)");
        StartCoroutine(WaitAndStartStep10());
    }
    
    private IEnumerator WaitAndStartStep10()
    {
        if (tutorialPlantBed != null)
        {
            while (!tutorialPlantBed.IsFullyGrown())
                yield return new WaitForSeconds(1f);
        }
        StartStep10_Harvest();
    }
    
    // ========== 10. Сбор ==========
    private void StartStep10_Harvest()
    {
        currentStep = 10;
        Debug.Log("[Tutorial] Этап 10: Собрать урожай");
        HighlightObject(tutorialPlantBed?.gameObject);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint("Урожай созрел! Нажмите, чтобы собрать морковку!", 10f);
    }
    
    public void OnHarvestCollected()
    {
        if (currentStep != 10 || harvestCollected) return;
        harvestCollected = true;
        Debug.Log("[Tutorial] Урожай собран!");
        UnhighlightObject(tutorialPlantBed?.gameObject);
        CompleteTutorial();
    }
    
    private void CompleteTutorial()
    {
        tutorialCompleted = true;
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowTutorialFinished("✓ Шаг 10 пройден!\n🎉 Поздравляем! Вы вырастили первую морковку! Достижение: «Первый урожай!»");
        Debug.Log("[Tutorial] Обучение завершено!");
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }
    
    private void HighlightObject(GameObject obj)
    {
        if (obj != null && highlights.ContainsKey(obj))
            highlights[obj].EnableHighlight(true);
    }
    
    private void UnhighlightObject(GameObject obj)
    {
        if (obj != null && highlights.ContainsKey(obj))
            highlights[obj].EnableHighlight(false);
    }
    
    public bool IsTutorialActive() => tutorialEnabled && !tutorialCompleted;
    public int GetCurrentStep() => currentStep;
    
    private static string GetStepName(int step)
    {
        switch (step)
        {
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
            default: return "";
        }
    }
}
