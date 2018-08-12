using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class InventoryCollection : MonoBehaviour
    {
        [SerializeField]
        Inventory[] allInventories;
        [SerializeField]
        BlockGridScanner scanner;

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

            StartCoroutine(scanner.AnimateScan(this));
        }

        public bool IsAllEnabled
        {
            set
            {
                // FIXME: actually implement!
                //throw new System.NotImplementedException();
            }
        }
    }
}
