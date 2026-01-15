using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Контроллер для игры без VR (мышь и клавиатура)
    /// </summary>
    public class NonVRController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;

        [Header("Interaction")]
        [SerializeField] private float interactionDistance = 5f;

        private Camera playerCamera;
        private CharacterController characterController;
        private float verticalRotation = 0f;
        private Vector3 velocity;

        private void Start()
        {
            playerCamera = GetComponent<Camera>();
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            if (playerCamera == null)
            {
                Debug.LogError("NonVRController: Camera not found! Make sure this script is attached to a Camera.");
                enabled = false;
                return;
            }

            // Добавляем CharacterController для движения
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
                characterController.height = 1.8f;
                characterController.radius = 0.3f;
                characterController.center = new Vector3(0, 0.9f, 0);
            }

            // Устанавливаем позицию камеры
            if (transform.parent == null)
            {
                transform.position = new Vector3(0, 1.6f, 0);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Debug.Log("NonVRController: Initialized! Use WASD to move, Mouse to look around.");
        }

        private void Update()
        {
            // Проверяем, что камера все еще существует
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
                if (playerCamera == null)
                {
                    return;
                }
            }

            HandleMovement();
            HandleMouseLook();
            HandleInteraction();
        }

        private void HandleMovement()
        {
            // Читаем ввод с клавиатуры
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                horizontal -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                horizontal += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                vertical -= 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                vertical += 1f;

            // Fallback на Input.GetAxis если доступен
            try
            {
                if (horizontal == 0f)
                    horizontal = Input.GetAxis("Horizontal");
                if (vertical == 0f)
                    vertical = Input.GetAxis("Vertical");
            }
            catch (System.InvalidOperationException)
            {
                // Новый Input System активен, используем только прямое чтение клавиш
            }

            // Вычисляем направление движения относительно камеры
            Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
            moveDirection.Normalize();

            // Применяем гравитацию
            if (characterController != null && !characterController.isGrounded)
            {
                velocity.y -= 9.81f * Time.deltaTime;
            }
            else
            {
                velocity.y = 0f;
            }

            // Движение
            Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
            move.y = velocity.y * Time.deltaTime;

            if (characterController != null)
            {
                characterController.Move(move);
            }
            else
            {
                transform.position += move;
            }
        }

        private Vector3 lastMousePosition;
        private bool mouseLookInitialized = false;

        private void HandleMouseLook()
        {
            // Проверяем, что курсор заблокирован
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            float mouseX = 0f;
            float mouseY = 0f;

            // Пытаемся использовать Input.GetAxis
            try
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            }
            catch (System.InvalidOperationException)
            {
                // Новый Input System активен, вычисляем дельту вручную
                if (!mouseLookInitialized)
                {
                    lastMousePosition = Input.mousePosition;
                    mouseLookInitialized = true;
                    return;
                }

                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 mouseDelta = currentMousePosition - lastMousePosition;
                lastMousePosition = currentMousePosition;

                mouseX = mouseDelta.x * mouseSensitivity * 0.1f;
                mouseY = mouseDelta.y * mouseSensitivity * 0.1f;
            }

            // Горизонтальное вращение (поворот камеры вокруг оси Y)
            transform.Rotate(Vector3.up * mouseX);

            // Вертикальное вращение (наклон камеры вверх/вниз)
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            
            // Применяем вертикальное вращение
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localRotation = Quaternion.Euler(verticalRotation, currentRotation.y, 0f);
        }

        private void HandleInteraction()
        {
            // ESC - разблокировать курсор
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // ЛКМ после ESC - заблокировать курсор
            if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // ЛКМ - взаимодействие
            if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked)
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
            {
                GameObject hitObj = hit.collider.gameObject;

                // Проверяем наши кастомные компоненты взаимодействия
                var cropInteractable = hitObj.GetComponent<VRFerma.VR.CropInteractable>();
                if (cropInteractable != null)
                {
                    var crop = hitObj.GetComponent<PlantedCrop>();
                    if (crop != null && crop.IsReadyToHarvest())
                    {
                        crop.Harvest();
                        Debug.Log("Harvested crop: " + hitObj.name);
                    }
                    return;
                }

                var animalInteractable = hitObj.GetComponent<VRFerma.VR.AnimalInteractable>();
                if (animalInteractable != null)
                    {
                    var animal = hitObj.GetComponent<FarmAnimal>();
                    if (animal == null)
                        animal = hitObj.GetComponentInParent<FarmAnimal>();
                    
                    if (animal != null)
                    {
                        animal.Feed();
                        Debug.Log("Fed animal: " + hitObj.name);
                    }
                    return;
                }

                var npcInteractable = hitObj.GetComponent<VRFerma.VR.NPCInteractable>();
                if (npcInteractable != null)
                {
                    var tradingSystem = FindObjectOfType<TradingSystem>();
                    if (tradingSystem != null)
                    {
                        var inventory = tradingSystem.GetFullInventory();
                        foreach (var item in inventory)
                        {
                            tradingSystem.SellCrop(item.Key, item.Value);
                        }
                    }
                    Debug.Log("Interacted with NPC: " + hitObj.name);
                    return;
                }
            }
        }

        private void OnGUI()
        {
            // Показываем подсказки
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                GUI.color = Color.white;
                GUI.Label(new Rect(10, 10, 300, 20), "=== Управление (Клавиатура) ===");
                GUI.Label(new Rect(10, 30, 300, 20), "WASD или Стрелки: Движение");
                GUI.Label(new Rect(10, 50, 300, 20), "Мышь: Поворот камеры");
                GUI.Label(new Rect(10, 70, 300, 20), "ЛКМ: Взаимодействовать");
                GUI.Label(new Rect(10, 90, 300, 20), "ESC: Разблокировать курсор");
            }
            else
            {
                GUI.color = Color.yellow;
                GUI.Label(new Rect(10, 10, 400, 20), "Курсор разблокирован. Кликните для блокировки.");
            }
        }
    }
}
