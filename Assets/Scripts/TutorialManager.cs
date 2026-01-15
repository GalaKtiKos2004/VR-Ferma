using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Управляет туториалом и уровнями обучения
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [System.Serializable]
        public class TutorialStep
        {
            public string instructionText;
            public string hintText;
            public TutorialAction requiredAction;
            public bool isCompleted;
        }

        public enum TutorialAction
        {
            None,
            PlantSeed,
            WaterPlant,
            FeedAnimal,
            WaterAnimal,
            HarvestCrop,
            InteractWithNPC
        }

        [Header("Tutorial Settings")]
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private List<TutorialStep> level0Steps = new List<TutorialStep>();
        [SerializeField] private List<TutorialStep> level1Steps = new List<TutorialStep>();
        [SerializeField] private List<TutorialStep> level2Steps = new List<TutorialStep>();
        [SerializeField] private List<TutorialStep> level3Steps = new List<TutorialStep>();
        [SerializeField] private List<TutorialStep> level4Steps = new List<TutorialStep>();

        private List<TutorialStep> currentSteps = new List<TutorialStep>();
        private int currentStepIndex = 0;

        // Events
        public System.Action<string> OnInstructionChanged;
        public System.Action<string> OnHintChanged;
        public System.Action<int> OnLevelCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeTutorialSteps();
        }

        private void InitializeTutorialSteps()
        {
            // Level 0: Введение и настройка
            level0Steps = new List<TutorialStep>
            {
                new TutorialStep
                {
                    instructionText = "Добро пожаловать на ферму!",
                    hintText = "Изучите окружение. Вы видите землю, воду, семена и животных.",
                    requiredAction = TutorialAction.None
                }
            };

            // Level 1: Посадка урожая
            level1Steps = new List<TutorialStep>
            {
                new TutorialStep
                {
                    instructionText = "Посади первую культуру!",
                    hintText = "Возьми семена и посади их в землю",
                    requiredAction = TutorialAction.PlantSeed
                },
                new TutorialStep
                {
                    instructionText = "Полив: Подойди к Сара. Инструкция: 'Подойди к воде и наполни ведро'",
                    hintText = "Подойди к воде и наполни ведро",
                    requiredAction = TutorialAction.WaterPlant
                },
                new TutorialStep
                {
                    instructionText = "Полив: Подойди к растению. Инструкция: 'Поливай и ухаживай за растением'",
                    hintText = "Поливай растение водой",
                    requiredAction = TutorialAction.WaterPlant
                },
                new TutorialStep
                {
                    instructionText = "Посадка: Подойди к растению. Инструкция: 'Посади семена в землю рядом с растением'",
                    hintText = "Посади семена рядом с растением",
                    requiredAction = TutorialAction.PlantSeed
                },
                new TutorialStep
                {
                    instructionText = "Посадка: Подойди к растению. Инструкция: 'Поливай и ухаживай за растением'",
                    hintText = "Поливай растение",
                    requiredAction = TutorialAction.WaterPlant
                },
                new TutorialStep
                {
                    instructionText = "Посадка: Подойди к растению. Инструкция: 'Подойди к растению и собери урожай'",
                    hintText = "Собери урожай",
                    requiredAction = TutorialAction.HarvestCrop
                }
            };

            // Level 2: Уход за животными
            level2Steps = new List<TutorialStep>
            {
                new TutorialStep
                {
                    instructionText = "Покорми животных!",
                    hintText = "Подойди к животным и покорми их",
                    requiredAction = TutorialAction.FeedAnimal
                },
                new TutorialStep
                {
                    instructionText = "Напои животных!",
                    hintText = "Подойди к животным и напои их",
                    requiredAction = TutorialAction.WaterAnimal
                }
            };

            // Level 3: Посадка и сбор урожая
            level3Steps = new List<TutorialStep>
            {
                new TutorialStep
                {
                    instructionText = "Посади урожай на продажу!",
                    hintText = "Посади культуры для продажи",
                    requiredAction = TutorialAction.PlantSeed
                },
                new TutorialStep
                {
                    instructionText = "Собери урожай!",
                    hintText = "Собери выращенные культуры",
                    requiredAction = TutorialAction.HarvestCrop
                }
            };

            // Level 4: Торговля и продажа
            level4Steps = new List<TutorialStep>
            {
                new TutorialStep
                {
                    instructionText = "Продай урожай!",
                    hintText = "Подойди к торговцу и продай урожай",
                    requiredAction = TutorialAction.InteractWithNPC
                }
            };
        }

        public void StartTutorial(int level)
        {
            currentLevel = level;
            currentStepIndex = 0;

            switch (level)
            {
                case 0:
                    currentSteps = level0Steps;
                    break;
                case 1:
                    currentSteps = level1Steps;
                    break;
                case 2:
                    currentSteps = level2Steps;
                    break;
                case 3:
                    currentSteps = level3Steps;
                    break;
                case 4:
                    currentSteps = level4Steps;
                    break;
                default:
                    currentSteps = new List<TutorialStep>();
                    break;
            }

            if (currentSteps.Count > 0)
            {
                ShowCurrentStep();
            }
        }

        private void ShowCurrentStep()
        {
            if (currentStepIndex < currentSteps.Count)
            {
                TutorialStep step = currentSteps[currentStepIndex];
                OnInstructionChanged?.Invoke(step.instructionText);
                OnHintChanged?.Invoke(step.hintText);
            }
        }

        public void CompleteAction(TutorialAction action)
        {
            if (currentStepIndex < currentSteps.Count)
            {
                TutorialStep step = currentSteps[currentStepIndex];
                if (step.requiredAction == action && !step.isCompleted)
                {
                    step.isCompleted = true;
                    currentStepIndex++;

                    if (currentStepIndex >= currentSteps.Count)
                    {
                        // Уровень завершен
                        OnLevelCompleted?.Invoke(currentLevel);
                        Debug.Log($"Level {currentLevel} completed!");
                    }
                    else
                    {
                        ShowCurrentStep();
                    }
                }
            }
        }

        public int GetCurrentLevel() => currentLevel;
        public int GetCurrentStepIndex() => currentStepIndex;
    }
}
