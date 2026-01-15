using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.VR
{
    /// <summary>
    /// Компонент для взаимодействия с NPC (торговцем)
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class NPCInteractable : MonoBehaviour
    {
        [Header("NPC Settings")]
        [SerializeField] private string npcName = "Торговец";
        [SerializeField] private bool isTrader = true;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
        private TradingSystem tradingSystem;

        private void Awake()
        {
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            tradingSystem = FindObjectOfType<TradingSystem>();
            
            if (tradingSystem == null)
            {
                Debug.LogWarning("TradingSystem not found in scene!");
            }
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
            if (isTrader && tradingSystem != null)
            {
                OpenTradeMenu();
            }

            // Уведомление туториала
            TutorialManager tutorialManager = TutorialManager.Instance;
            if (tutorialManager != null)
            {
                tutorialManager.CompleteAction(TutorialManager.TutorialAction.InteractWithNPC);
            }
        }

        private void OpenTradeMenu()
        {
            // Здесь можно открыть UI меню торговли
            Debug.Log($"Открыто меню торговли с {npcName}");
            
            // Автоматическая продажа всех урожаев в инвентаре
            if (tradingSystem != null)
            {
                var inventory = tradingSystem.GetFullInventory();
                foreach (var item in inventory)
                {
                    tradingSystem.SellCrop(item.Key, item.Value);
                }
            }
        }

        public string GetNPCName() => npcName;
    }
}
