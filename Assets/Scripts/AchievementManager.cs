using System.Collections.Generic;
using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Управляет системой достижений
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [System.Serializable]
        public class Achievement
        {
            public string id;
            public string title;
            public string description;
            public bool isUnlocked;
            public Sprite icon;
        }

        [Header("Achievements")]
        [SerializeField] private List<Achievement> achievements = new List<Achievement>();

        private Dictionary<string, Achievement> achievementDictionary = new Dictionary<string, Achievement>();

        // Events
        public System.Action<Achievement> OnAchievementUnlocked;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            // Инициализация достижений из дизайн-документа
            if (achievements.Count == 0)
            {
                achievements.Add(new Achievement { id = "plant_crops", title = "Посади урожай!", description = "Посади первые культуры" });
                achievements.Add(new Achievement { id = "great_harvest", title = "Отличный урожай!", description = "Собери урожай" });
                achievements.Add(new Achievement { id = "plant_carrots", title = "Посади морковь!", description = "Посади морковь" });
                achievements.Add(new Achievement { id = "plant_tomatoes", title = "Посади помидоры!", description = "Посади помидоры" });
                achievements.Add(new Achievement { id = "plant_pumpkin", title = "Посади тыкву!", description = "Посади тыкву (если возможно)" });
                achievements.Add(new Achievement { id = "feed_chickens", title = "Покорми цыплят!", description = "Покорми цыплят" });
                achievements.Add(new Achievement { id = "ah_ah_ah", title = "Ах ах ах, ты цыпленок!", description = "Особое достижение" });
                achievements.Add(new Achievement { id = "great_farmer", title = "Отличный фермер!", description = "Ухаживай за животными" });
                achievements.Add(new Achievement { id = "animals_fed", title = "Животные накормлены!", description = "Накорми всех животных" });
                achievements.Add(new Achievement { id = "great_farmer_crops", title = "Отличный фермер урожай!", description = "Покорми и напои всех животных на ферме" });
            }

            foreach (var achievement in achievements)
            {
                achievementDictionary[achievement.id] = achievement;
            }
        }

        public void UnlockAchievement(string achievementId)
        {
            if (achievementDictionary.ContainsKey(achievementId))
            {
                Achievement achievement = achievementDictionary[achievementId];
                if (!achievement.isUnlocked)
                {
                    achievement.isUnlocked = true;
                    OnAchievementUnlocked?.Invoke(achievement);
                    Debug.Log($"Achievement Unlocked: {achievement.title}");
                }
            }
        }

        public void CheckCropAchievements(CropManager.CropType cropType)
        {
            switch (cropType)
            {
                case CropManager.CropType.Carrot:
                    UnlockAchievement("plant_carrots");
                    break;
                case CropManager.CropType.Tomato:
                    UnlockAchievement("plant_tomatoes");
                    break;
                case CropManager.CropType.Pumpkin:
                    UnlockAchievement("plant_pumpkin");
                    break;
            }

            // Общее достижение за посадку
            UnlockAchievement("plant_crops");
        }

        public void CheckAnimalAchievements()
        {
            UnlockAchievement("feed_chickens");
            UnlockAchievement("great_farmer");
            UnlockAchievement("animals_fed");
        }

        public void CheckHarvestAchievement()
        {
            UnlockAchievement("great_harvest");
        }

        public List<Achievement> GetAllAchievements() => achievements;
        public List<Achievement> GetUnlockedAchievements()
        {
            return achievements.FindAll(a => a.isUnlocked);
        }
    }
}
