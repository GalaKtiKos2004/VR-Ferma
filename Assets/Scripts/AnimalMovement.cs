using UnityEngine;

/// <summary>
/// Простое случайное движение животного
/// </summary>
public class AnimalMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;
    
    [Header("Зона перемещения")]
    [SerializeField] private Vector3 centerPoint = Vector3.zero;
    [SerializeField] private float wanderRadius = 5f;
    
    [Header("Настройки")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool canMove = true;
    
    private Vector3 targetPosition;
    private float waitTimer = 0f;
    private float currentWaitTime = 0f;
    private Animator animator;
    private Rigidbody rb;
    
    private static readonly int ParamIsWalking = Animator.StringToHash("IsWalking");
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 10f;
            rb.drag = 5f;
            rb.angularDrag = 5f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Устанавливаем центр зоны как текущую позицию
        if (centerPoint == Vector3.zero)
        {
            centerPoint = transform.position;
        }
        
        // Начинаем с ожидания
        StartWaiting();
    }
    
    private void Update()
    {
        if (!canMove) return;
        
        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            Wait();
        }
    }
    
    /// <summary>
    /// Движение к целевой точке
    /// </summary>
    private void MoveToTarget()
    {
        if (rb == null) return;
        
        // Направление к цели
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Движение только по горизонтали
        
        // Проверяем, достигли ли цели
        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(targetPosition.x, 0, targetPosition.z)
        );
        
        if (distance < 0.5f)
        {
            // Достигли цели - начинаем ждать
            StartWaiting();
            return;
        }
        
        // Двигаемся через Rigidbody для корректной физики столкновений
        Vector3 newPosition = rb.position + direction * moveSpeed * Time.deltaTime;
        newPosition.y = rb.position.y; // Сохраняем высоту
        rb.MovePosition(newPosition);
        
        // Поворачиваемся через Rigidbody
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(newRotation);
        }
        
        // Анимация ходьбы (только если есть контроллер)
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetBool(ParamIsWalking, true);
    }
    
    /// <summary>
    /// Ожидание перед следующим движением
    /// </summary>
    private void Wait()
    {
        waitTimer += Time.deltaTime;
        
        if (waitTimer >= currentWaitTime)
        {
            // Время ожидания вышло - выбираем новую точку
            ChooseNewTarget();
            isMoving = true;
        }
        
        // Анимация стояния (только если есть контроллер)
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetBool(ParamIsWalking, false);
    }
    
    /// <summary>
    /// Начать ожидание
    /// </summary>
    private void StartWaiting()
    {
        isMoving = false;
        waitTimer = 0f;
        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }
    
    /// <summary>
    /// Выбрать новую целевую точку
    /// </summary>
    private void ChooseNewTarget()
    {
        // Случайная точка в радиусе
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = centerPoint + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        // Высота на уровне земли
        targetPosition.y = transform.position.y;
        
        Debug.Log($"{gameObject.name} идёт к новой точке: {targetPosition}");
    }
    
    /// <summary>
    /// Остановить движение (например, при взаимодействии)
    /// </summary>
    public void StopMovement()
    {
        canMove = false;
        isMoving = false;
        
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetBool(ParamIsWalking, false);
    }
    
    /// <summary>
    /// Возобновить движение
    /// </summary>
    public void ResumeMovement()
    {
        canMove = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Визуализация зоны перемещения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPoint == Vector3.zero ? transform.position : centerPoint, wanderRadius);
        
        if (isMoving)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawSphere(targetPosition, 0.2f);
        }
    }
}
