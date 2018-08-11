using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class BlockCollection : MonoBehaviour
    {
        [SerializeField]
        Block[] allBlockTypes;
        [SerializeField]
        Grid grid;

        #region Unity Events
        // Use this for initialization
        void Start()
        {
            FillGrid();
        }
        #endregion

        // FIXME: create a second function that double-checks
        // previous blocks in the grid, and makes sure it
        // doesn't trigger a combo
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Block RandomBlock()
        {
            return allBlockTypes[Random.Range(0, allBlockTypes.Length)];
        }

        public void FillGrid()
        {
            for (int x = 0; x < grid.Width; ++x)
            {
                for (int y = 0; y < grid.Height; ++y)
                {
                    grid.CreateBlock(RandomBlock(), x, y);
                }
            }
        }
    }
}
