using System.Collections.Generic;
using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Главный менеджер игры, управляет общим состоянием игры
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private int money = 0;
        [SerializeField] private float gameTime = 0f;

        [Header("References")]
        [SerializeField] private CropManager cropManager;
        [SerializeField] private AnimalManager animalManager;
        [SerializeField] private AchievementManager achievementManager;
        [SerializeField] private TutorialManager tutorialManager;
        [SerializeField] private UIManager uiManager;

        // Events
        public System.Action<int> OnMoneyChanged;
        public System.Action<int> OnLevelChanged;
        public System.Action<float> OnGameTimeChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            gameTime += Time.deltaTime;
            OnGameTimeChanged?.Invoke(gameTime);
        }

        private void InitializeGame()
        {
            // Инициализация всех систем
            if (cropManager == null)
                cropManager = FindObjectOfType<CropManager>();
            
            if (animalManager == null)
                animalManager = FindObjectOfType<AnimalManager>();
            
            if (achievementManager == null)
                achievementManager = FindObjectOfType<AchievementManager>();
            
            if (tutorialManager == null)
                tutorialManager = FindObjectOfType<TutorialManager>();
            
            if (uiManager == null)
                uiManager = FindObjectOfType<UIManager>();

            // Запуск туториала
            if (tutorialManager != null)
            {
                tutorialManager.StartTutorial(currentLevel);
            }
        }

        public void AddMoney(int amount)
        {
            money += amount;
            OnMoneyChanged?.Invoke(money);
        }

        public bool SpendMoney(int amount)
        {
            if (money >= amount)
            {
                money -= amount;
                OnMoneyChanged?.Invoke(money);
                return true;
            }
            return false;
        }

        public int GetMoney() => money;

        public void SetLevel(int level)
        {
            currentLevel = level;
            OnLevelChanged?.Invoke(currentLevel);
        }

        public int GetCurrentLevel() => currentLevel;

        public float GetGameTime() => gameTime;
    }
}
