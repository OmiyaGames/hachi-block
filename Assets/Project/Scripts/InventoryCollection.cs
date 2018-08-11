using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class InventoryCollection : MonoBehaviour
    {
        [SerializeField]
        Inventory[] allInventories;

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

        public void ShuffleAll()
        {
            foreach(Inventory inventory in AllInventories)
            {
                inventory.Shuffle();
            }
        }
    }
}
