using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.VR
{
    /// <summary>
    /// Компонент для взаимодействия с животными в VR
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class AnimalInteractable : MonoBehaviour
    {
        [Header("Interaction Type")]
        [SerializeField] private bool isFeedAction = true; // true = кормление, false = поение

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
        private FarmAnimal animal;

        private void Awake()
        {
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            animal = GetComponent<FarmAnimal>();
            
            if (animal == null)
                animal = GetComponentInParent<FarmAnimal>();
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
            if (animal == null)
                return;

            if (isFeedAction)
            {
                animal.Feed();
                
                AnimalManager animalManager = FindObjectOfType<AnimalManager>();
                if (animalManager != null)
                {
                    animalManager.FeedAnimal(animal);
                }

                // Уведомление туториала
                TutorialManager tutorialManager = TutorialManager.Instance;
                if (tutorialManager != null)
                {
                    tutorialManager.CompleteAction(TutorialManager.TutorialAction.FeedAnimal);
                }
            }
            else
            {
                animal.Water();
                
                AnimalManager animalManager = FindObjectOfType<AnimalManager>();
                if (animalManager != null)
                {
                    animalManager.WaterAnimal(animal);
                }

                // Уведомление туториала
                TutorialManager tutorialManager = TutorialManager.Instance;
                if (tutorialManager != null)
                {
                    tutorialManager.CompleteAction(TutorialManager.TutorialAction.WaterAnimal);
                }
            }
        }

        public void SetFeedAction(bool isFeed)
        {
            isFeedAction = isFeed;
        }
    }
}
