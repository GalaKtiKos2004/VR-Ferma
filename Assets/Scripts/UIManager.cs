using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRFerma
{
    /// <summary>
    /// Управляет UI интерфейсом игры
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private GameObject achievementPanel;
        [SerializeField] private Transform achievementListParent;
        [SerializeField] private GameObject achievementPrefab;

        [Header("Canvas")]
        [SerializeField] private Canvas mainCanvas;

        private void Start()
        {
            // Подписка на события
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMoneyChanged += UpdateMoney;
                GameManager.Instance.OnLevelChanged += UpdateLevel;
                GameManager.Instance.OnGameTimeChanged += UpdateTime;
            }

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnInstructionChanged += UpdateInstruction;
                TutorialManager.Instance.OnHintChanged += UpdateHint;
            }

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked += ShowAchievement;
            }

            // Инициализация UI
            UpdateMoney(0);
            UpdateLevel(0);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMoneyChanged -= UpdateMoney;
                GameManager.Instance.OnLevelChanged -= UpdateLevel;
                GameManager.Instance.OnGameTimeChanged -= UpdateTime;
            }

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnInstructionChanged -= UpdateInstruction;
                TutorialManager.Instance.OnHintChanged -= UpdateHint;
            }

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked -= ShowAchievement;
            }
        }

        private void UpdateMoney(int money)
        {
            if (moneyText != null)
            {
                moneyText.text = $"Деньги: {money}";
            }
        }

        private void UpdateLevel(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Уровень: {level}";
            }
        }

        private void UpdateTime(float time)
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                timeText.text = $"Время: {minutes:00}:{seconds:00}";
            }
        }

        private void UpdateInstruction(string instruction)
        {
            if (instructionText != null)
            {
                instructionText.text = instruction;
            }
        }

        private void UpdateHint(string hint)
        {
            if (hintText != null)
            {
                hintText.text = hint;
            }
        }

        private void ShowAchievement(AchievementManager.Achievement achievement)
        {
            Debug.Log($"Achievement: {achievement.title} - {achievement.description}");
            
            // Здесь можно добавить визуальное отображение достижения
            if (achievementPanel != null)
            {
                achievementPanel.SetActive(true);
                // Можно добавить анимацию появления
            }
        }
    }
}
