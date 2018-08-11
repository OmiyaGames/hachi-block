using UnityEngine;

namespace Project
{
    [DisallowMultipleComponent]
    public class BlockCollection : MonoBehaviour
    {
        [SerializeField]
        Block[] allBlockTypes;

        // FIXME: create a second function that double-checks
        // previous blocks in the grid, and makes sure it
        // doesn't trigger a combo
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Block RandomBlockPrefab()
        {
            return allBlockTypes[Random.Range(0, allBlockTypes.Length)];
        }

        public void FillGrid(BlockGrid grid, int maxHeight)
        {
            for (int x = 0; x < grid.Width; ++x)
            {
                for (int y = 0; y < maxHeight; ++y)
                {
                    grid.CreateBlock(RandomBlockPrefab(), x, y);
                }
            }
        }
    }
}
