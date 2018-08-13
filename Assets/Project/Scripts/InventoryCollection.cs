using UnityEngine;

namespace Project
{
    public class InventoryCollection : MonoBehaviour
    {
        [SerializeField]
        Inventory[] allInventories;
        [SerializeField]
        UnityEngine.UI.Button shuffleButton;
        [SerializeField]
        BlockGridScanner scanner;
        [SerializeField]
        BlockCursor cursor;

        bool isAllEnabled = true, isShuffleEnabled = true;

        public Inventory[] AllInventories
        {
            get
            {
                return allInventories;
            }
        }

        public Inventory HoveredInventory
        {
            get;
            set;
        }

        public bool IsAllEnabled
        {
            get
            {
                return isAllEnabled;
            }
            set
            {
                isAllEnabled = value;
                foreach (Inventory item in AllInventories)
                {
                    item.UpdateControl();
                }
                UpdateShuffleButton();
            }
        }

        public bool IsShuffleEnabled
        {
            get
            {
                return isShuffleEnabled;
            }
            set
            {
                isShuffleEnabled = value;
                UpdateShuffleButton();
            }
        }

        private void Start()
        {
            if(LastGameSettings.Instance.RestoreInventorySettings() == false)
            {
                // Shuffle the inventory
                foreach (Inventory inventory in AllInventories)
                {
                    inventory.Shuffle();
                }
            }
        }

        public void ShuffleAll()
        {
            // Reset the cursor
            cursor.HideCursor(BlockCursor.SoundType.Shuffle);

            // Shuffle the inventory
            foreach (Inventory inventory in AllInventories)
            {
                inventory.Shuffle();
            }

            // Forcefully proceed a step
            StartCoroutine(scanner.AnimateScan(this));
        }

        private void UpdateShuffleButton()
        {
            // Update the shuffle button
            if (IsAllEnabled == true)
            {
                shuffleButton.interactable = IsShuffleEnabled;
            }
            else
            {
                shuffleButton.interactable = false;
            }
        }
    }
}
