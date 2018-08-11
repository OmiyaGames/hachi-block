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
    }
}
