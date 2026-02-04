using UnityEngine;

/// <summary>
/// Животное на ферме
/// </summary>
public class Animal : MonoBehaviour
{
    [Header("Настройки животного")]
    [SerializeField] private string animalName = "Курица";
    [SerializeField] private float maxHappiness = 100f;
    [SerializeField] private float currentHappiness = 50f;
    [SerializeField] private float happinessDecayRate = 5f; // Уменьшение счастья в секунду
    
    [Header("Кормление")]
    [SerializeField] private float feedAmount = 30f; // Сколько счастья дает кормление
    [SerializeField] private float petAmount = 20f; // Сколько счастья дает поглаживание
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject happyEffect; // Частицы/сердечки когда довольно
    [SerializeField] private GameObject hungryEffect; // Эффект когда голодное
    [SerializeField] private Animator animator;
    
    /// <summary> Параметры аниматора: IsWalking (bool), IsHungry (bool), Eat (trigger), Happy (trigger) </summary>
    private static readonly int ParamIsHungry = Animator.StringToHash("IsHungry");
    private static readonly int ParamEat = Animator.StringToHash("Eat");
    private static readonly int ParamHappy = Animator.StringToHash("Happy");
    
    [Header("Звуки")]
    [SerializeField] private AudioClip happySound;
    [SerializeField] private AudioClip hungrySound;
    [SerializeField] private AudioClip eatSound; // Звук bite_cartoon_-_big_chomp.mp3 для кормления
    [SerializeField] private AudioClip petSound; // Звук животного при поглаживании (курица.mp3, корова.mp3, коза.mp3, свинья.mp3)
    private AudioSource audioSource;
    
    [Header("Продукция")]
    [SerializeField] private bool canProduce = true; // Может ли производить (яйца, молоко)
    [SerializeField] private float productionTime = 60f; // Время до следующей продукции
    private float productionTimer = 0f;
    
    private bool isHungry = false;
    private float lastInteractionTime = 0f;
    private float debugTimer = 0f;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Ищем Animator на себе или в детях (модели из FBX часто в дочерних объектах)
        // Для Generic рига Animator ДОЛЖЕН быть на GameObject с костями
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            
            // Если не нашли, ищем на первом child с детьми (обычно это rig root)
            if (animator == null)
            {
                foreach (Transform child in transform)
                {
                    var childAnim = child.GetComponent<Animator>();
                    if (childAnim != null)
                    {
                        animator = childAnim;
                        break;
                    }
                }
            }
        }
        
        // Диагностика Animator
        if (animator == null)
        {
            Debug.LogError($"❌ {name}: Animator НЕ НАЙДЕН! Запустите VR-Ferma → Обновить Animator");
        }
        else
        {
            Debug.Log($"[Animal.Start] {name}: Animator найден на {animator.gameObject.name}");
            Debug.Log($"  - Путь: {GetGameObjectPath(animator.gameObject)}");
            Debug.Log($"  - Controller: {(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "НЕТ ❌")}");
            Debug.Log($"  - Avatar: {(animator.avatar != null ? animator.avatar.name : "НЕТ ❌")}");
            Debug.Log($"  - Enabled: {animator.enabled}");
            Debug.Log($"  - IsInitialized: {animator.isInitialized}");
            
            if (animator.runtimeAnimatorController != null)
            {
                animator.speed = 1.5f;
                animator.Rebind(); // Rebind в Start для применения контроллера
                animator.Update(0);
                
                // Проверяем параметры
                Debug.Log($"  - Rebind выполнен, speed = {animator.speed}");
                Debug.Log($"  - Параметры контроллера:");
                foreach (var param in animator.parameters)
                {
                    Debug.Log($"    • {param.name} ({param.type})");
                }
                
                // Проверяем children (кости)
                int childCount = animator.transform.childCount;
                Debug.Log($"  - Children на Animator GO: {childCount}");
                if (childCount > 0)
                {
                    string childNames = "";
                    for (int i = 0; i < Mathf.Min(5, childCount); i++)
                        childNames += animator.transform.GetChild(i).name + ", ";
                    Debug.Log($"    Первые дети: {childNames}...");
                }
            }
            else
            {
                Debug.LogError($"❌ {name}: У Animator нет контроллера!");
            }
        }
        
        UpdateEffects();
    }
    
    private void Update()
    {
        // Уменьшаем счастье со временем
        currentHappiness -= happinessDecayRate * Time.deltaTime;
        if (currentHappiness < 0) currentHappiness = 0;
        
        // Проверяем состояние
        bool wasHungry = isHungry;
        isHungry = currentHappiness < 30f;
        
        if (isHungry && !wasHungry)
        {
            OnBecomeHungry();
        }
        else if (!isHungry && wasHungry)
        {
            OnBecomeHappy();
        }
        
        // Переключаем анимацию по состоянию (голоден / доволен)
        UpdateAnimationState();
        UpdateEffects();
        
        // Отладка анимации каждые 3 секунды
        debugTimer += Time.deltaTime;
        if (debugTimer >= 3f)
        {
            debugTimer = 0f;
            if (animator != null && animator.runtimeAnimatorController != null && animator.isInitialized)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                int hash = state.shortNameHash;
                string stateName = hash == Animator.StringToHash("Idle") ? "Idle" :
                                   hash == Animator.StringToHash("Walk") ? "Walk" :
                                   hash == Animator.StringToHash("HungryIdle") ? "HungryIdle" :
                                   hash == Animator.StringToHash("Eat") ? "Eat" :
                                   hash == Animator.StringToHash("Happy") ? "Happy" : hash.ToString();
                Debug.Log($"[Anim] {name}: State={stateName}, NormalizedTime={state.normalizedTime:F2}, IsHungry={isHungry}, Speed={animator.speed}");
            }
        }
        
        // Производство продукции
        if (canProduce && !isHungry)
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= productionTime)
            {
                ProduceItem();
                productionTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// Покормить животное
    /// </summary>
    public void Feed()
    {
        currentHappiness += feedAmount;
        if (currentHappiness > maxHappiness)
        {
            currentHappiness = maxHappiness;
        }
        
        // Звук кормления (bite_cartoon_-_big_chomp.mp3)
        if (eatSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(eatSound);
        }
        
        // Анимация кормления
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Проверяем инициализацию
            if (!animator.isInitialized)
            {
                animator.Rebind();
                animator.Update(0);
                Debug.Log($"[Feed] {name}: Animator переинициализирован");
            }
            
            // Убеждаемся, что аниматор включен
            if (!animator.enabled)
            {
                animator.enabled = true;
                Debug.LogWarning($"[Feed] {name}: Animator был выключен, включен заново");
            }
            
            // Пытаемся запустить анимацию Eat двумя способами:
            // 1. Через триггер (если переход настроен правильно)
            animator.ResetTrigger(ParamEat);
            animator.SetTrigger(ParamEat);
            
            // 2. Через CrossFade напрямую (более надежный способ)
            try
            {
                animator.CrossFade("Eat", 0.1f, 0, 0f);
                Debug.Log($"[Feed] {name}: CrossFade к Eat выполнен");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[Feed] {name}: CrossFade не удался: {e.Message}, используем только триггер");
            }
            
            // Принудительно обновляем аниматор несколько раз для гарантии
            for (int i = 0; i < 5; i++)
            {
                animator.Update(0.02f);
            }
            
            Debug.Log($"[Feed] {name}: триггер Eat установлен, CrossFade выполнен, isInitialized={animator.isInitialized}, enabled={animator.enabled}");
            
            // Проверяем состояние через небольшую задержку
            if (Application.isPlaying)
            {
                StartCoroutine(CheckAnimationStateAfterDelay("Eat", 0.15f));
            }
        }
        else
        {
            Debug.LogError($"[Feed] {name}: НЕТ ANIMATOR или CONTROLLER! Animator={animator != null}, Controller={animator?.runtimeAnimatorController != null}");
        }
        
        Debug.Log($"{animalName} покормлено! Счастье: {currentHappiness:F0}/{maxHappiness}");
        lastInteractionTime = Time.time;
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnAnimalFed();
        }
        
        // Уведомляем AchievementManager
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.RegisterFeed(this);
        }
    }
    
    /// <summary>
    /// Погладить животное
    /// </summary>
    public void Pet()
    {
        // Не даем спамить поглаживания
        if (Time.time - lastInteractionTime < 2f)
        {
            Debug.Log($"[Pet] {name}: слишком рано (cooldown)");
            return;
        }
        
        currentHappiness += petAmount;
        if (currentHappiness > maxHappiness)
        {
            currentHappiness = maxHappiness;
        }
        
        // Звук животного при поглаживании (курица.mp3, корова.mp3, коза.mp3, свинья.mp3)
        if (petSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(petSound);
        }
        else if (happySound != null && audioSource != null)
        {
            // Fallback на happySound если petSound не назначен
            audioSource.PlayOneShot(happySound);
        }
        
        // Анимация поглаживания
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Проверяем инициализацию
            if (!animator.isInitialized)
            {
                animator.Rebind();
                animator.Update(0);
                Debug.Log($"[Pet] {name}: Animator переинициализирован");
            }
            
            // Устанавливаем триггер
            animator.SetTrigger(ParamHappy);
            animator.Update(0); // Принудительное обновление для применения триггера
            
            Debug.Log($"[Pet] {name}: триггер Happy установлен, isInitialized={animator.isInitialized}");
            
            // Проверяем состояние через 0.1 сек
            if (Application.isPlaying)
                StartCoroutine(CheckAnimationStateAfterDelay("Happy", 0.1f));
        }
        else
        {
            Debug.LogError($"[Pet] {name}: НЕТ ANIMATOR или CONTROLLER! Animator={animator != null}, Controller={animator?.runtimeAnimatorController != null}");
        }
        
        // Показываем сердечки
        if (happyEffect != null)
        {
            GameObject effect = Instantiate(happyEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        Debug.Log($"{animalName} погладили! Счастье: {currentHappiness:F0}/{maxHappiness}");
        lastInteractionTime = Time.time;
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnAnimalPet();
        }
        
        // Уведомляем AchievementManager
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.RegisterPet(this);
        }
    }
    
    /// <summary>
    /// Производство продукции (яйца, молоко)
    /// </summary>
    private void ProduceItem()
    {
        Debug.Log($"{animalName} произвело продукцию!");
        // Здесь можно создать префаб яйца/молока
    }
    
    /// <summary>
    /// Животное стало голодным
    /// </summary>
    private void OnBecomeHungry()
    {
        if (hungrySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hungrySound);
        }
        
        Debug.Log($"{animalName} голодно!");
    }
    
    /// <summary>
    /// Животное стало довольным
    /// </summary>
    private void OnBecomeHappy()
    {
        if (happySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(happySound);
        }
        
        Debug.Log($"{animalName} довольно!");
    }
    
    /// <summary>
    /// Обновить визуальные эффекты
    /// </summary>
    /// <summary>
    /// Обновить параметры аниматора в зависимости от состояния.
    /// </summary>
    private void UpdateAnimationState()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        animator.SetBool(ParamIsHungry, isHungry);
    }
    
    private void UpdateEffects()
    {
        if (happyEffect != null)
        {
            happyEffect.SetActive(!isHungry && currentHappiness > 70f);
        }
        
        if (hungryEffect != null)
        {
            hungryEffect.SetActive(isHungry);
        }
    }
    
    /// <summary> Установить ссылку на Animator (например, из Editor при создании животного). </summary>
    public void SetAnimator(Animator a)
    {
        animator = a;
    }
    
    /// <summary>
    /// Тестовый метод: принудительно запустить анимацию поедания (для отладки)
    /// </summary>
    [ContextMenu("Тест: Запустить анимацию поедания")]
    public void TestEatAnimation()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            if (!animator.isInitialized)
            {
                animator.Rebind();
                animator.Update(0);
            }
            
            animator.enabled = true;
            
            // Пробуем оба способа
            animator.ResetTrigger(ParamEat);
            animator.SetTrigger(ParamEat);
            
            try
            {
                animator.CrossFade("Eat", 0.1f, 0, 0f);
                Debug.Log($"[TestEat] {name}: CrossFade к Eat выполнен");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[TestEat] {name}: CrossFade не удался: {e.Message}");
            }
            
            for (int i = 0; i < 5; i++)
            {
                animator.Update(0.02f);
            }
            
            var state = animator.GetCurrentAnimatorStateInfo(0);
            int eatHash = Animator.StringToHash("Eat");
            string stateName = state.shortNameHash == Animator.StringToHash("Idle") ? "Idle" :
                              state.shortNameHash == Animator.StringToHash("Walk") ? "Walk" :
                              state.shortNameHash == Animator.StringToHash("Eat") ? "Eat" :
                              state.shortNameHash == Animator.StringToHash("Happy") ? "Happy" :
                              state.shortNameHash.ToString();
            
            Debug.Log($"[TestEat] {name}: Триггер установлен, CrossFade выполнен. Текущее состояние: {stateName} {(state.shortNameHash == eatHash ? "✓" : "❌")}, normalizedTime={state.normalizedTime:F2}, hash={state.shortNameHash}");
        }
        else
        {
            Debug.LogError($"[TestEat] {name}: Animator или Controller отсутствует! Animator={animator != null}, Controller={animator?.runtimeAnimatorController != null}");
        }
    }
    
    
    /// <summary>
    /// Проверить состояние анимации через задержку (для отладки триггеров)
    /// </summary>
    private System.Collections.IEnumerator CheckAnimationStateAfterDelay(string expectedState, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (animator != null && animator.isInitialized && animator.layerCount > 0)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            int hash = state.shortNameHash;
            int expectedHash = Animator.StringToHash(expectedState);
            string stateName = hash == Animator.StringToHash("Idle") ? "Idle" :
                               hash == Animator.StringToHash("Walk") ? "Walk" :
                               hash == Animator.StringToHash("HungryIdle") ? "HungryIdle" :
                               hash == Animator.StringToHash("Eat") ? "Eat" :
                               hash == Animator.StringToHash("Happy") ? "Happy" : hash.ToString();
            
            if (hash == expectedHash || stateName == expectedState)
            {
                Debug.Log($"[AnimCheck] {name}: ✓ Анимация {expectedState} играет! NormalizedTime={state.normalizedTime:F2}");
            }
            else
            {
                Debug.LogWarning($"[AnimCheck] {name}: ❌ Ожидали {expectedState}, но играет {stateName} (hash: {hash} vs {expectedHash})");
                
                // Если ожидали Eat, но играет что-то другое, пытаемся принудительно запустить
                if (expectedState == "Eat")
                {
                    Debug.LogWarning($"[AnimCheck] {name}: Пытаемся принудительно запустить Eat через CrossFade");
                    bool success = TryForceEatAnimation();
                    
                    if (success)
                    {
                        // Проверяем еще раз после задержки
                        yield return new WaitForSeconds(0.1f);
                        state = animator.GetCurrentAnimatorStateInfo(0);
                        hash = state.shortNameHash;
                        if (hash == expectedHash)
                        {
                            Debug.Log($"[AnimCheck] {name}: ✓ Анимация Eat успешно запущена после принудительного вызова!");
                        }
                        else
                        {
                            Debug.LogError($"[AnimCheck] {name}: ❌ Анимация Eat все еще не запущена после принудительного вызова! Текущее: {hash}");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"[AnimCheck] {name}: Animator не инициализирован! isInitialized={animator?.isInitialized}, layerCount={animator?.layerCount}");
        }
    }
    
    /// <summary>
    /// Попытаться принудительно запустить анимацию Eat
    /// </summary>
    private bool TryForceEatAnimation()
    {
        if (animator == null || !animator.isInitialized) return false;
        
        try
        {
            animator.CrossFade("Eat", 0.05f, 0, 0f);
            
            // Обновляем аниматор
            for (int i = 0; i < 3; i++)
            {
                animator.Update(0.02f);
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AnimCheck] {name}: Ошибка при принудительном запуске Eat: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Получить полный путь GameObject в иерархии
    /// </summary>
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }
    
    public bool IsHungry() => isHungry;
    public float GetHappiness() => currentHappiness;
    public float GetHappinessPercentage() => currentHappiness / maxHappiness;
}
