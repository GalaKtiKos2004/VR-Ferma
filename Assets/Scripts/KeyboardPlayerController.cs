using UnityEngine;

/// <summary>
/// Управление игроком с клавиатуры (для тестирования без VR)
/// WASD - движение, Мышь - взгляд, E - взаимодействие, ЛКМ - кормление/поглаживание
/// </summary>
public class KeyboardPlayerController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float jumpForce = 5f;
    
    [Header("Камера")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    
    [Header("Взаимодействие")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private bool hasFood = false;
    
    [Header("UI")]
    [SerializeField] private GameObject crosshair;
    
    private CharacterController characterController;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    
    private void Start()
    {
        // Находим или создаём камеру
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                GameObject camObj = new GameObject("PlayerCamera");
                camObj.transform.SetParent(transform);
                camObj.transform.localPosition = new Vector3(0, 1.6f, 0);
                playerCamera = camObj.AddComponent<Camera>();
                playerCamera.tag = "MainCamera";
            }
        }
        
        // Добавляем CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.3f;
            characterController.center = new Vector3(0, 0.9f, 0);
        }
        
        // Скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Управление: WASD - движение, Мышь - взгляд, E - взять зерно, ЛКМ - взаимодействие, ESC - курсор");
    }
    
    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleInteraction();
        HandleInput();
    }
    
    /// <summary>
    /// Обработка движения WASD
    /// </summary>
    private void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        
        // Получаем ввод
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Направление движения
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // Спринт
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= sprintMultiplier;
        
        // Движение
        characterController.Move(move * speed * Time.deltaTime);
        
        // Прыжок
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
        
        // Гравитация
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
    
    /// <summary>
    /// Обработка камеры мышью
    /// </summary>
    private void HandleCamera()
    {
        // Горизонтальное вращение (тело)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        
        // Вертикальное вращение (камера)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    
    /// <summary>
    /// Обработка взаимодействия с объектами
    /// </summary>
    private void HandleInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            // Показываем подсказку
            if (hit.collider.CompareTag("Animal") || hit.collider.GetComponent<Animal>() != null)
            {
                if (hasFood)
                    ShowHint("ЛКМ - Покормить 🌾");
                else
                    ShowHint("ЛКМ - Погладить ✋");
            }
            else if (hit.collider.GetComponent<FoodBucket>() != null)
            {
                ShowHint("E - Взять зерно 🪣");
            }
            
            // ЛКМ - взаимодействие с животным
            if (Input.GetMouseButtonDown(0))
            {
                Animal animal = hit.collider.GetComponent<Animal>();
                if (animal != null)
                {
                    if (hasFood)
                    {
                        animal.Feed();
                        hasFood = false;
                        ShowHint("🌾 Животное накормлено!");
                    }
                    else
                    {
                        animal.Pet();
                        ShowHint("❤️ Животное погладили!");
                    }
                }
            }
            
            // E - взять зерно из ведра
            if (Input.GetKeyDown(KeyCode.E))
            {
                FoodBucket bucket = hit.collider.GetComponent<FoodBucket>();
                if (bucket != null && bucket.TakeFood())
                {
                    hasFood = true;
                    ShowHint("🌾 Зерно взято! ЛКМ на животное чтобы покормить");
                }
            }
        }
        
        // Debug - показываем raycast
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.yellow);
    }
    
    /// <summary>
    /// Обработка общего ввода
    /// </summary>
    private void HandleInput()
    {
        // ESC - показать курсор
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
        // Q - выбросить зерно
        if (Input.GetKeyDown(KeyCode.Q) && hasFood)
        {
            hasFood = false;
            ShowHint("Зерно выброшено");
        }
    }
    
    /// <summary>
    /// Показать подсказку на экране
    /// </summary>
    private void ShowHint(string message)
    {
        if (SimpleGameManager.Instance != null)
        {
            SimpleGameManager.Instance.ShowHint(message);
        }
        else
        {
            Debug.Log($"[Hint] {message}");
        }
    }
    
    private void OnGUI()
    {
        // Прицел
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float size = 10f;
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(Screen.width / 2 - size / 2, Screen.height / 2 - size / 2, size, size), Texture2D.whiteTexture);
        }
        
        // Статус зерна
        if (hasFood)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, Screen.height - 30, 200, 30), "🌾 Зерно в руке");
        }
        
        // Подсказки управления
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 400, 20), "WASD - движение | Мышь - взгляд | E - взять зерно | ЛКМ - взаимодействие");
        GUI.Label(new Rect(10, 30, 400, 20), "Q - выбросить зерно | Shift - бег | Space - прыжок | ESC - курсор");
    }
}
