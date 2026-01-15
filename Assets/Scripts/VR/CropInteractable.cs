using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.VR
{
    /// <summary>
    /// Компонент для взаимодействия с растениями в VR
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class CropInteractable : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
        private PlantedCrop crop;

        private void Awake()
        {
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            crop = GetComponent<PlantedCrop>();
            
            if (crop == null)
                crop = GetComponentInParent<PlantedCrop>();
        }

        private void OnEnable()
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
        }

        private void OnDisable()
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (crop != null && crop.IsReadyToHarvest())
            {
                crop.Harvest();

                // Уведомление туториала
                TutorialManager tutorialManager = TutorialManager.Instance;
                if (tutorialManager != null)
                {
                    tutorialManager.CompleteAction(TutorialManager.TutorialAction.HarvestCrop);
                }

                // Проверка достижений
                AchievementManager achievementManager = AchievementManager.Instance;
                if (achievementManager != null)
                {
                    achievementManager.CheckHarvestAchievement();
                }
            }
        }
    }
}
