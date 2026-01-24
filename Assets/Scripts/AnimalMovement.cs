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
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        
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
        
        // Двигаемся
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Поворачиваемся
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Анимация ходьбы
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
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
        
        // Анимация стояния
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
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
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
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
