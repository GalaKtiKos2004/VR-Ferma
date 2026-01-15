using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.VR
{
    /// <summary>
    /// Компонент для семени, которое можно посадить
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class Seed : MonoBehaviour
    {
        [Header("Seed Settings")]
        private CropManager.CropType cropType;
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
        private bool isPlanted = false;

        [Header("Planting")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float plantingDistance = 0.2f;

        private void Awake()
        {
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        private void OnEnable()
        {
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }

        private void OnDisable()
        {
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }

        public void Initialize(CropManager.CropType type)
        {
            cropType = type;
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            if (!isPlanted)
            {
                TryPlant();
            }
        }

        private void TryPlant()
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = Vector3.down;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, 1f, groundLayer))
            {
                CropManager cropManager = FindObjectOfType<CropManager>();
                if (cropManager != null)
                {
                    if (cropManager.PlantCrop(cropType, hit.point))
                    {
                        isPlanted = true;
                        Destroy(gameObject);

                        // Уведомление туториала
                        TutorialManager tutorialManager = TutorialManager.Instance;
                        if (tutorialManager != null)
                        {
                            tutorialManager.CompleteAction(TutorialManager.TutorialAction.PlantSeed);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.down * 1f);
        }
    }
}
