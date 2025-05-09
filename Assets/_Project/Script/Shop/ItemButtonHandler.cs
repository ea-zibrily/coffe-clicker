using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Coffee.Data;
using Coffee.Ability;
using Coffee.Database;
using Coffee.Gameplay;
using Coffee.Quest;

namespace Coffee.Shop
{
    public class ItemButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields & Properties
        
        [Header("Stats")]
        [SerializeField] private ItemName itemName;
        [SerializeField] private bool canAfford;
        
        private ItemData[] _itemDataList;
        private int _itemLevel;
        private int _itemMaxLevel;
        
        public ItemData CurrentData { get; private set; }
        
        [Header("UI")]
        [SerializeField] private Button itemButton;
        [SerializeField] private Image itemImageUI;
        [SerializeField] private GameObject itemHighlightUI;
        [SerializeField] private GameObject itemGrayscaleUI;
        
        [Space]
        [SerializeField] private TextMeshProUGUI itemNameTextUI;
        [SerializeField] private TextMeshProUGUI itemCostTextUI;
        [SerializeField] private TextMeshProUGUI itemDescriptionTextUI;
        
        [Header("Reference")]
        [SerializeField] private ClickerManager clickerManager;
        [SerializeField] private AbilityManager abilityManager;
        [SerializeField] private QuestManager questManager;
        
        #endregion
        
        #region Methods
        
        // Unity Callbacks
        private void Start()
        {
            InitStats();
            ModifyItemText(CurrentData, isInitiate: true);
            
            itemButton.interactable = false;
            itemGrayscaleUI.SetActive(true);
            itemHighlightUI.SetActive(false);
            itemImageUI.color = Color.black;
            
            itemButton.onClick.AddListener(OnClicked);
        }
        
        private void Update()
        {
            if (!canAfford) return;
            var itemCost = CurrentData.ItemCost;
            if (clickerManager.CurrentCoffee < itemCost)
            {
                DisableItemButton();
            }
            else
            {
                EnableItemButton();
            }
        }
        
        private void InitStats()
        {
            _itemDataList = ItemDatabase.Instance.GetItemDatas(itemName);
            
            _itemLevel = 1;
            _itemMaxLevel = _itemDataList.Length;
            CurrentData = _itemDataList[0];
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!itemButton.interactable) return;
            itemHighlightUI.SetActive(true);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!itemButton.interactable) return;
            itemHighlightUI.SetActive(false);
        }
        
        /// <summary>
        /// Handle logic saat player membeli item.
        /// Dalam method ini, terdapat beberapa komponen seperti abilityManager,
        /// questManager, dan clickerManager.
        /// </summary>
        private void OnClicked()
        {
            if (clickerManager.CurrentCoffee < CurrentData.ItemCost) return;
            
            _itemLevel++;
            itemHighlightUI.SetActive(false);
            abilityManager.ApplyAbility(itemName);
            clickerManager.RemovePoint(CurrentData.ItemCost);
            questManager.OnAttractBuyItemQuest(itemName);
            
            CurrentData = _itemDataList[_itemLevel - 1];
            ModifyItemText(CurrentData);
            
            if (_itemLevel >= _itemMaxLevel)
            {
                canAfford = false;
                DisableItemButton();
            }
        }
        
        private void ModifyItemText(ItemData data, bool isInitiate = false)
        {
            itemNameTextUI.text = isInitiate ? "??" : data.GetItemStringName();
            itemCostTextUI.text = data.ItemCost.ToString();
            itemDescriptionTextUI.text = data.ItemDescription;
        }

        public void ActivateItemButton()
        {
            canAfford = true;
            itemImageUI.color = Color.white;
            
            EnableItemButton();
            ModifyItemText(CurrentData);
        }
        
        /// <summary>
        /// Handle logic saat item button diaktifkan.
        /// </summary>
        private void EnableItemButton()
        {
            itemButton.interactable = true;
            itemGrayscaleUI.SetActive(false);
        }
        
        /// <summary>
        /// Handle logic saat item button dinon-aktifkan.
        /// </summary>
        private void DisableItemButton()
        {
            itemButton.interactable = false;
            itemGrayscaleUI.SetActive(true);
        }
        
        #endregion
    }
}