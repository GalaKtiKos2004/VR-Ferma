using UnityEngine;

/// <summary>
/// Управление игроком в режиме без VR (клавиатура + мышь)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class NonVRPlayerController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Камера")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    
    [Header("Взаимодействие")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactableLayer = -1; // -1 = Everything
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool showDebug = true;
    
    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private GameObject currentTarget;
    private WateringCan heldWateringCan;
    private Rake heldRake;
    private Hoe heldHoe;
    private SeedBag heldSeedBag;
    private Vector3 originalCanPosition; // Исходная позиция лейки
    private Quaternion originalCanRotation; // Исходный поворот лейки
    private bool hasFood = false; // Есть ли зерно у игрока
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Блокировка и скрытие курсора
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }
    
    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleInteraction();
        
        // Обновляем позицию инструментов если держим их
        if (heldWateringCan != null && playerCamera != null)
        {
            // Лейка следует за камерой (справа-снизу, ДАЛЬШЕ от камеры)
            Vector3 offset = playerCamera.right * 0.5f + playerCamera.up * -0.5f + playerCamera.forward * 1.2f;
            heldWateringCan.transform.position = playerCamera.position + offset;
            heldWateringCan.transform.rotation = playerCamera.rotation * Quaternion.Euler(30, -10, 0);
        }
        
        if (heldRake != null && playerCamera != null)
        {
            // Грабли следуют за камерой
            Vector3 offset = playerCamera.right * 0.3f + playerCamera.up * -0.3f + playerCamera.forward * 1.0f;
            heldRake.transform.position = playerCamera.position + offset;
            heldRake.transform.rotation = playerCamera.rotation * Quaternion.Euler(45, -10, 0);
        }
        
        if (heldHoe != null && playerCamera != null)
        {
            // Тяпка следует за камерой
            Vector3 offset = playerCamera.right * 0.3f + playerCamera.up * -0.3f + playerCamera.forward * 1.0f;
            heldHoe.transform.position = playerCamera.position + offset;
            heldHoe.transform.rotation = playerCamera.rotation * Quaternion.Euler(45, -10, 0);
        }
        
        if (heldSeedBag != null && playerCamera != null)
        {
            // Пакет с семенами следует за камерой
            Vector3 offset = playerCamera.right * 0.3f + playerCamera.up * -0.3f + playerCamera.forward * 1.0f;
            heldSeedBag.transform.position = playerCamera.position + offset;
            heldSeedBag.transform.rotation = playerCamera.rotation * Quaternion.Euler(45, -10, 0);
        }
        
        // Q или ПКМ для сброса инструментов
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
        {
            if (heldWateringCan != null)
            {
                DropWateringCan();
            }
            else if (heldRake != null)
            {
                DropRake();
            }
            else if (heldHoe != null)
            {
                DropHoe();
            }
            else if (heldSeedBag != null)
            {
                DropSeedBag();
            }
        }
        
        // ESC для разблокировки курсора
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Клик для повторной блокировки
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    /// <summary>
    /// Обработка движения
    /// </summary>
    private void HandleMovement()
    {
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Направление движения
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        // Скорость (с учетом бега)
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        
        // Движение
        controller.Move(moveDirection * speed * Time.deltaTime);
        
        // Гравитация
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Обработка камеры
    /// </summary>
    private void HandleCamera()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        
        // Поворот по горизонтали
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        
        // Поворот по вертикали
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
        
        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }
    
    /// <summary>
    /// Обработка взаимодействия
    /// </summary>
    private void HandleInteraction()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera не найдена!");
            return;
        }
        
        // Луч от камеры
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;
        
        // Raycast с layer mask (если -1, то все слои)
        bool hitSomething = Physics.Raycast(ray, out hit, interactionRange, interactableLayer);
        
        if (showDebug)
        {
            // Отладочная визуализация луча
            Debug.DrawRay(playerCamera.position, playerCamera.forward * interactionRange, 
                hitSomething ? Color.green : Color.red);
        }
        
        if (hitSomething)
        {
            currentTarget = hit.collider.gameObject;
            
            if (showDebug)
            {
                Debug.Log($"Навели на: {currentTarget.name} (расстояние: {hit.distance:F2}м)");
            }
            
            // Показываем подсказку
            ShowInteractionHint(hit.collider.gameObject);
            
            // Взаимодействие на клавишу E или ЛКМ (только если курсор захвачен)
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0))
                {
                    if (showDebug)
                    {
                        Debug.Log($"Взаимодействие с: {currentTarget.name}");
                    }
                    InteractWithObject(hit.collider.gameObject);
                }
            }
        }
        else
        {
            currentTarget = null;
        }
    }
    
    /// <summary>
    /// Показать подсказку взаимодействия
    /// </summary>
    private void ShowInteractionHint(GameObject target)
    {
        string hint = "";
        
        if (target.GetComponent<PlantBed>() != null)
        {
            PlantBed bed = target.GetComponent<PlantBed>();
            if (bed.HasPlant())
            {
                if (bed.IsFullyGrown())
                {
                    hint = "[E] Собрать урожай 🥕";
                }
                else if (!bed.IsWatered())
                {
                    // Растение посажено, но не полито
                    if (heldWateringCan != null && !heldWateringCan.IsEmpty())
                    {
                        hint = "[E] Полить росток 💧";
                    }
                    else if (heldWateringCan != null)
                    {
                        hint = "Лейка пустая! Наполните у колодца 💧";
                    }
                    else
                    {
                        hint = "Возьмите лейку и полейте росток! 💧";
                    }
                }
                else if (bed.IsGrowing())
                {
                    // Растение растёт
                    float progress = bed.GetGrowthProgress() * 100f;
                    hint = $"Растёт... {progress:F0}% ⏳";
                }
                else
                {
                    hint = "Растение на грядке";
                }
            }
            else if (!bed.IsTilled())
            {
                if (heldHoe != null)
                {
                    hint = "[E] Взрыхлить грядку тяпкой 🌾";
                }
                else
                {
                    hint = "Возьмите тяпку! 🌾";
                }
            }
            else if (!bed.IsRaked())
            {
                if (heldRake != null)
                {
                    hint = "[E] Разрыхлить грядку граблями 🌾";
                }
                else
                {
                    hint = "Возьмите грабли! 🌾";
                }
            }
            else if (!bed.HasPlant())
            {
                if (heldSeedBag != null)
                {
                    hint = $"[E] Посадить семя {heldSeedBag.GetSeedType()} 🌱";
                }
                else
                {
                    hint = "Грядка готова! Возьмите семена для посадки 🌱";
                }
            }
            else
            {
                hint = "Грядка готова для посадки семян 🌱";
            }
        }
        else if (target.GetComponent<Animal>() != null)
        {
            if (hasFood)
            {
                hint = "[E] Покормить животное 🌾";
            }
            else
            {
                hint = "[E] Погладить животное ❤️ (возьмите зерно для кормления)";
            }
        }
        else if (target.GetComponent<FoodBucket>() != null)
        {
            if (hasFood)
            {
                hint = "Зерно уже взято";
            }
            else
            {
                hint = "[E] Взять зерно из ведра 🌾";
            }
        }
        else if (target.GetComponent<FeedingTrough>() != null)
        {
            if (hasFood)
            {
                hint = "[E] Положить сено в стог 🌾";
            }
            else
            {
                hint = "Стог сена (корова ест автоматически)";
            }
        }
        else if (target.GetComponent<WateringCan>() != null)
        {
            hint = "[E] Взять лейку 🚰";
        }
        else if (target.GetComponent<Rake>() != null)
        {
            hint = "[E] Взять грабли 🌾";
        }
        else if (target.GetComponent<Hoe>() != null)
        {
            hint = "[E] Взять тяпку 🔨";
        }
        else if (target.GetComponent<SeedBag>() != null)
        {
            SeedBag sb = target.GetComponent<SeedBag>();
            hint = "[E] " + sb.GetPickupHint() + " 🌾";
        }
        else if (target.GetComponent<WaterSource>() != null || target.CompareTag("Water"))
        {
            if (heldWateringCan != null)
            {
                hint = "[E] Наполнить лейку 💧";
            }
            else
            {
                hint = "Сначала возьмите лейку! 🚰";
            }
        }
        else
        {
            hint = $"[E] {target.name}";
        }
        
        // Показываем подсказку в UI
        if (SimpleGameManager.Instance != null && !string.IsNullOrEmpty(hint))
        {
            SimpleGameManager.Instance.ShowHint(hint, 0.1f);
        }
    }
    
    /// <summary>
    /// Взаимодействие с объектом
    /// </summary>
    private void InteractWithObject(GameObject target)
    {
        if (showDebug)
        {
            Debug.Log($"=== Попытка взаимодействия с: {target.name} ===");
        }
        
        // Грядка
        PlantBed bed = target.GetComponent<PlantBed>();
        if (bed != null)
        {
            if (showDebug) Debug.Log("Найдена грядка");
            
            // Если есть растение
            if (bed.HasPlant())
            {
                // Если растение полностью выросло - собираем урожай
                if (bed.IsFullyGrown())
                {
                    if (showDebug) Debug.Log("Собираем урожай");
                    bed.Harvest();
                }
                // Если растение не полито - поливаем
                else if (!bed.IsWatered() && heldWateringCan != null && !heldWateringCan.IsEmpty())
                {
                    if (showDebug) Debug.Log("Поливаем растение");
                    bed.Water();
                }
                // Если растение растёт - показываем прогресс
                else if (bed.IsGrowing())
                {
                    float progress = bed.GetGrowthProgress() * 100f;
                    if (showDebug) Debug.Log($"Растение ещё не выросло ({progress:F0}%)");
                    if (SimpleGameManager.Instance != null)
                    {
                        SimpleGameManager.Instance.ShowHint($"Растение растёт... {progress:F0}% ⏳");
                    }
                }
                // Если растение посажено, но не полито - подсказка
                else if (!bed.IsWatered())
                {
                    if (heldWateringCan != null && !heldWateringCan.IsEmpty())
                    {
                        if (SimpleGameManager.Instance != null)
                        {
                            SimpleGameManager.Instance.ShowHint("[E] Полить росток 💧", 2f);
                        }
                    }
                    else if (heldWateringCan != null)
                    {
                        if (SimpleGameManager.Instance != null)
                        {
                            SimpleGameManager.Instance.ShowHint("Лейка пустая! Наполните у колодца 💧", 2f);
                        }
                    }
                    else
                    {
                        if (SimpleGameManager.Instance != null)
                        {
                            SimpleGameManager.Instance.ShowHint("Возьмите лейку и полейте росток! 💧", 2f);
                        }
                    }
                }
            }
            // Если держим тяпку - взрыхляем тяпкой
            else if (!bed.IsTilled() && heldHoe != null)
            {
                if (showDebug) Debug.Log("Взрыхляем грядку тяпкой");
                bed.Till();
                // Воспроизводим звук тяпки
                heldHoe.PlayTillingSound();
                if (TutorialManager.Instance != null)
                    TutorialManager.Instance.OnBedTilled();
                // Подсказку показывает TutorialManager; вне туториала — своя
                if (SimpleGameManager.Instance != null && (TutorialManager.Instance == null || !TutorialManager.Instance.IsTutorialActive()))
                {
                    SimpleGameManager.Instance.ShowHint("Грядка взрыхлена тяпкой! Теперь разрыхлите граблями 🌾");
                }
            }
            // Если держим грабли - разрыхляем граблями
            else if (bed.IsTilled() && !bed.IsRaked() && heldRake != null)
            {
                if (showDebug) Debug.Log("Разрыхляем грядку граблями");
                bed.Rake();
                // Воспроизводим звук граблей
                heldRake.PlayTillingSound();
                if (TutorialManager.Instance != null)
                    TutorialManager.Instance.OnBedRaked();
                // Подсказку показывает TutorialManager; вне туториала — своя
                if (SimpleGameManager.Instance != null && (TutorialManager.Instance == null || !TutorialManager.Instance.IsTutorialActive()))
                {
                    SimpleGameManager.Instance.ShowHint("Грядка разрыхлена граблями! Теперь можно сажать семена 🌾");
                }
            }
            // Если грядка взрыхлена и разрыхлена, и держим семена - сажаем
            else if (bed.IsTilled() && bed.IsRaked() && !bed.HasPlant() && heldSeedBag != null)
            {
                if (showDebug) Debug.Log("Сажаем семя");
                bed.PlantSeed(heldSeedBag.GetSeedType());
                if (TutorialManager.Instance != null)
                    TutorialManager.Instance.OnSeedPlanted();
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint($"Семя {heldSeedBag.GetSeedType()} посажено! 🌱");
                }
            }
            // Если грядка взрыхлена, но не разрыхлена граблями - подсказка
            else if (bed.IsTilled() && !bed.IsRaked())
            {
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Грядка взрыхлена тяпкой! Разрыхлите её граблями 🌾");
                }
            }
            // Если грядка готова, но нет семян - подсказка
            else if (bed.IsTilled() && bed.IsRaked() && !bed.HasPlant())
            {
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Грядка готова! Возьмите семена для посадки 🌱");
                }
            }
            // Если грядка пустая и взрыхлена - поливаем (автоматически посадит)
            else if (!bed.IsWatered() && bed.IsTilled() && heldWateringCan != null && !heldWateringCan.IsEmpty())
            {
                if (showDebug) Debug.Log("Поливаем грядку (автопосадка)");
                bed.Water(); // Автоматически посадит растение
            }
            else
            {
                // Подсказки что делать
                if (!bed.IsTilled())
                {
                    if (heldRake == null && heldHoe == null)
                    {
                        if (SimpleGameManager.Instance != null)
                        {
                            SimpleGameManager.Instance.ShowHint("Возьмите грабли или тяпку, чтобы взрыхлить грядку! 🌾");
                        }
                    }
                }
                else if (!bed.IsWatered() && heldWateringCan == null)
                {
                    if (SimpleGameManager.Instance != null)
                    {
                        SimpleGameManager.Instance.ShowHint("Возьмите лейку, чтобы полить грядку! 🚰");
                    }
                }
                else if (!bed.IsWatered() && heldWateringCan != null && heldWateringCan.IsEmpty())
                {
                    if (SimpleGameManager.Instance != null)
                    {
                        SimpleGameManager.Instance.ShowHint("Лейка пустая! Наполните у бочки 💧");
                    }
                }
                
                if (showDebug) Debug.Log($"Грядка: взрыхлена={bed.IsTilled()}, полита={bed.IsWatered()}, растение={bed.HasPlant()}");
            }
            return;
        }
        
        // Животное
        Animal animal = target.GetComponent<Animal>();
        if (animal != null)
        {
            // Если есть зерно - кормим, иначе - гладим
            if (hasFood)
            {
                if (showDebug) Debug.Log("Кормим животное");
                animal.Feed();
                hasFood = false; // Зерно использовано
                
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Животное накормлено! 🌾❤️");
                }
            }
            else
            {
                if (showDebug) Debug.Log("Гладим животное");
                animal.Pet();
                
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Животное довольно! ❤️");
                }
            }
            return;
        }
        
        // Ведро с зерном
        FoodBucket bucket = target.GetComponent<FoodBucket>();
        if (bucket != null)
        {
            if (showDebug) Debug.Log("Берем зерно из ведра");
            
            if (bucket.TakeFood())
            {
                hasFood = true;
            }
            return;
        }
        
        // Стог сена (наполнение)
        FeedingTrough trough = target.GetComponent<FeedingTrough>();
        if (trough != null)
        {
            if (showDebug) Debug.Log("Наполняем стог сена");
            
            if (hasFood)
            {
                // Если у нас есть зерно - кладём в стог
                trough.Refill();
                hasFood = false;
                
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Стог наполнен! 🌾 Корова будет есть автоматически");
                }
            }
            else
            {
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Стог сена для коровы (ест автоматически)");
                }
            }
            return;
        }
        
        // Лейка
        WateringCan can = target.GetComponent<WateringCan>();
        if (can != null)
        {
            if (showDebug) Debug.Log("Берем лейку");
            PickUpWateringCan(can);
            return;
        }
        
        // Грабли
        Rake rake = target.GetComponent<Rake>();
        if (rake != null)
        {
            if (showDebug) Debug.Log("Берем грабли");
            PickUpRake(rake);
            return;
        }
        
        // Тяпка
        Hoe hoe = target.GetComponent<Hoe>();
        if (hoe != null)
        {
            if (showDebug) Debug.Log("Берем тяпку");
            PickUpHoe(hoe);
            return;
        }
        
        // Пакет с семенами
        SeedBag seedBag = target.GetComponent<SeedBag>();
        if (seedBag != null)
        {
            if (showDebug) Debug.Log("Берем пакет с семенами");
            PickUpSeedBag(seedBag);
            return;
        }
        
        // Источник воды (по компоненту)
        WaterSource waterSource = target.GetComponent<WaterSource>();
        if (waterSource != null)
        {
            if (showDebug) Debug.Log("Найден источник воды (WaterSource компонент)");
            
            if (heldWateringCan != null)
            {
                if (showDebug) Debug.Log("Наполняем лейку");
                heldWateringCan.Refill();
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Лейка наполнена! 💧 Теперь можно поливать грядки!");
                }
            }
            else
            {
                if (showDebug) Debug.Log("Лейка не взята!");
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Сначала возьмите лейку! 🚰");
                }
            }
            return;
        }
        
        // Источник воды (по тегу, запасной вариант)
        if (target.CompareTag("Water"))
        {
            if (showDebug) Debug.Log("Найден источник воды (тег Water)");
            
            if (heldWateringCan != null)
            {
                if (showDebug) Debug.Log("Наполняем лейку");
                heldWateringCan.Refill();
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Лейка наполнена! 💧");
                }
            }
            else
            {
                if (showDebug) Debug.Log("Лейка не взята!");
                if (SimpleGameManager.Instance != null)
                {
                    SimpleGameManager.Instance.ShowHint("Сначала возьмите лейку! 🚰");
                }
            }
            return;
        }
        
        // Если ничего не подошло
        if (showDebug)
        {
            Debug.Log($"Объект {target.name} не является интерактивным");
            Component[] components = target.GetComponents<Component>();
            string componentNames = string.Empty;
            for (int i = 0; i < components.Length; i++)
            {
                componentNames += components[i].GetType().Name;
                if (i < components.Length - 1) componentNames += ", ";
            }
            Debug.Log($"Компоненты: {componentNames}");
        }
    }
    
    /// <summary>
    /// Взять лейку
    /// </summary>
    private void PickUpWateringCan(WateringCan can)
    {
        if (heldWateringCan != null || heldRake != null || heldHoe != null || heldSeedBag != null)
        {
            if (showDebug) Debug.Log("Лейка уже взята!");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Лейка уже у вас!");
            }
            return;
        }
        
        heldWateringCan = can;
        
        // СОХРАНЯЕМ исходное состояние лейки (позиция и поворот)
        originalCanPosition = can.transform.position;
        originalCanRotation = can.transform.rotation;
        
        if (showDebug) Debug.Log($"Берем лейку! Исходная позиция: {originalCanPosition}, размер: {can.transform.localScale}");
        
        // Отключаем коллайдер, чтобы не мешал взаимодействию
        Collider canCollider = can.GetComponent<Collider>();
        if (canCollider != null)
        {
            canCollider.enabled = false;
        }
        
        if (showDebug) Debug.Log($"Лейка прикреплена! Локальная позиция: {can.transform.localPosition}");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Лейка взята! 🚰 Наполните её у бочки с водой 💧");
        }
        else
        {
            Debug.Log("ЛЕЙКА ВЗЯТА! Наполните у бочки с водой!");
        }
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnCanTaken();
        }
        
        // Вызываем OnGrabbed для WateringCan (если нужно)
        can.OnGrabbed();
    }
    
    /// <summary>
    /// Положить лейку
    /// </summary>
    public void DropWateringCan()
    {
        if (heldWateringCan != null)
        {
            if (showDebug) Debug.Log("Кладем лейку");
            
            // Кладём лейку ПЕРЕД игроком на землю (НЕ возвращаем на исходное место!)
            Vector3 dropPosition = transform.position + transform.forward * 2f;
            dropPosition.y = 0.3f; // Высота над землёй
            
            heldWateringCan.transform.position = dropPosition;
            heldWateringCan.transform.rotation = Quaternion.identity;
            
            if (showDebug) Debug.Log($"Лейка положена перед игроком: {dropPosition}");
            
            // Включаем коллайдер обратно
            Collider canCollider = heldWateringCan.GetComponent<Collider>();
            if (canCollider != null)
            {
                canCollider.enabled = true;
            }
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Лейка положена перед вами");
            }
            
            heldWateringCan = null;
        }
    }
    
    /// <summary>
    /// Взять грабли
    /// </summary>
    private void PickUpRake(Rake rake)
    {
        if (heldRake != null || heldHoe != null || heldWateringCan != null || heldSeedBag != null)
        {
            if (showDebug) Debug.Log("Уже держите инструмент!");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Сначала положите текущий инструмент (Q)");
            }
            return;
        }
        
        heldRake = rake;
        
        // Отключаем коллайдер
        Collider rakeCollider = rake.GetComponent<Collider>();
        if (rakeCollider != null)
        {
            rakeCollider.enabled = false;
        }
        
        // Отключаем Rigidbody
        Rigidbody rb = rake.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        if (showDebug) Debug.Log("Грабли взяты!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Грабли взяты! 🌾 Используйте ЛКМ для взрыхления грядок");
        }
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnRakeTaken();
        }
    }
    
    /// <summary>
    /// Положить грабли
    /// </summary>
    private void DropRake()
    {
        if (heldRake != null)
        {
            if (showDebug) Debug.Log("Кладем грабли");
            
            // Кладём грабли перед игроком
            Vector3 dropPosition = transform.position + transform.forward * 2f;
            dropPosition.y = 0.3f;
            
            heldRake.transform.position = dropPosition;
            heldRake.transform.rotation = Quaternion.identity;
            
            // Включаем коллайдер обратно
            Collider rakeCollider = heldRake.GetComponent<Collider>();
            if (rakeCollider != null)
            {
                rakeCollider.enabled = true;
            }
            
            // Включаем Rigidbody
            Rigidbody rb = heldRake.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Грабли положены");
            }
            
            heldRake = null;
        }
    }
    
    /// <summary>
    /// Взять тяпку
    /// </summary>
    private void PickUpHoe(Hoe hoe)
    {
        if (heldRake != null || heldHoe != null || heldWateringCan != null || heldSeedBag != null)
        {
            if (showDebug) Debug.Log("Уже держите инструмент!");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Сначала положите текущий инструмент (Q)");
            }
            return;
        }
        
        heldHoe = hoe;
        
        // Отключаем коллайдер
        Collider hoeCollider = hoe.GetComponent<Collider>();
        if (hoeCollider != null)
        {
            hoeCollider.enabled = false;
        }
        
        // Отключаем Rigidbody
        Rigidbody rb = hoe.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        if (showDebug) Debug.Log("Тяпка взята!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint("Тяпка взята! 🔨 Используйте ЛКМ для взрыхления грядок");
        }
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnHoeTaken();
        }
    }
    
    /// <summary>
    /// Положить тяпку
    /// </summary>
    private void DropHoe()
    {
        if (heldHoe != null)
        {
            if (showDebug) Debug.Log("Кладем тяпку");
            
            // Кладём тяпку перед игроком
            Vector3 dropPosition = transform.position + transform.forward * 2f;
            dropPosition.y = 0.3f;
            
            heldHoe.transform.position = dropPosition;
            heldHoe.transform.rotation = Quaternion.identity;
            
            // Включаем коллайдер обратно
            Collider hoeCollider = heldHoe.GetComponent<Collider>();
            if (hoeCollider != null)
            {
                hoeCollider.enabled = true;
            }
            
            // Включаем Rigidbody
            Rigidbody rb = heldHoe.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Тяпка положена");
            }
            
            heldHoe = null;
        }
    }
    
    /// <summary>
    /// Взять пакет с семенами
    /// </summary>
    private void PickUpSeedBag(SeedBag seedBag)
    {
        if (heldRake != null || heldHoe != null || heldWateringCan != null || heldSeedBag != null)
        {
            if (showDebug) Debug.Log("Уже держите инструмент!");
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Сначала положите текущий инструмент (Q)");
            }
            return;
        }
        
        heldSeedBag = seedBag;
        
        // Отключаем коллайдер
        Collider seedBagCollider = seedBag.GetComponent<Collider>();
        if (seedBagCollider != null)
        {
            seedBagCollider.enabled = false;
        }
        
        // Отключаем Rigidbody
        Rigidbody rb = seedBag.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        if (showDebug) Debug.Log("Пакет с семенами взят!");
        
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint($"Пакет с семенами {seedBag.GetSeedType()} взят! 🌾 Используйте ЛКМ для посадки на взрыхлённую грядку");
        }
        
        // Уведомляем TutorialManager
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnSeedsTaken();
        }
        
        // Вызываем OnGrabbed для SeedBag (если нужно)
        seedBag.OnGrabbed();
    }
    
    /// <summary>
    /// Положить пакет с семенами
    /// </summary>
    private void DropSeedBag()
    {
        if (heldSeedBag != null)
        {
            if (showDebug) Debug.Log("Кладем пакет с семенами");
            
            // Кладём пакет перед игроком
            Vector3 dropPosition = transform.position + transform.forward * 2f;
            dropPosition.y = 0.3f;
            
            heldSeedBag.transform.position = dropPosition;
            heldSeedBag.transform.rotation = Quaternion.identity;
            
            // Включаем коллайдер обратно
            Collider seedBagCollider = heldSeedBag.GetComponent<Collider>();
            if (seedBagCollider != null)
            {
                seedBagCollider.enabled = true;
            }
            
            // Включаем Rigidbody
            Rigidbody rb = heldSeedBag.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.ShowHint("Пакет с семенами положен");
            }
            
            heldSeedBag = null;
        }
    }
}
