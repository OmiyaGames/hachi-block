using UnityEngine;
using System.Collections.Generic;

namespace Project
{
    [DisallowMultipleComponent]
    public class BlockCollection : MonoBehaviour
    {
        [SerializeField]
        Block[] allBlockTypes;

        readonly List<int> availableIndexes = new List<int>();
        readonly HashSet<Block.BlockType> blocksToAvoid = new HashSet<Block.BlockType>();

        // FIXME: create a second function that double-checks
        // previous blocks in the grid, and makes sure it
        // doesn't trigger a combo
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Block RandomBlockPrefab(int maxNumBlockTypes = 3)
        {
            maxNumBlockTypes = Mathf.Clamp(maxNumBlockTypes, 1, allBlockTypes.Length);
            return allBlockTypes[Random.Range(0, maxNumBlockTypes)];
        }

        public Block RandomBlockPrefab(HashSet<Block.BlockType> blocksToAvoid, int maxNumBlockTypes)
        {
            // Clamp max number of block types
            maxNumBlockTypes = Mathf.Clamp(maxNumBlockTypes, 1, allBlockTypes.Length);

            // Search for valid blocks to consider
            availableIndexes.Clear();
            for(int index = 0; index < maxNumBlockTypes; ++index)
            {
                if(blocksToAvoid.Contains(allBlockTypes[index].Type) == false)
                {
                    // Add its index to a list
                    availableIndexes.Add(index);
                }
            }

            // Grab a random index
            int randomIndex = 0;
            if(availableIndexes.Count == 1)
            {
                randomIndex = availableIndexes[0];
            }
            else if (availableIndexes.Count > 1)
            {
                randomIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];
            }
            return allBlockTypes[randomIndex];
        }

        public void FillGrid(BlockGrid grid, int maxHeight, int maxNumBlockTypes, int blocksInARow)
        {
            Block.BlockType checkType = Block.BlockType.Invalid;
            for (int x = 0; x < grid.Width; ++x)
            {
                for (int y = 0; y < maxHeight; ++y)
                {
                    blocksToAvoid.Clear();
                    if (x >= (blocksInARow - 1))
                    {
                        checkType = grid.Blocks[(x - 1), y].Type;
                        for (int checkX = (x - 2); checkX > (x - blocksInARow); --checkX)
                        {
                            if(checkType != grid.Blocks[checkX, y].Type)
                            {
                                checkType = Block.BlockType.Invalid;
                            }
                        }
                        if(checkType != Block.BlockType.Invalid)
                        {
                            blocksToAvoid.Add(checkType);
                        }
                    }
                    if (y >= (blocksInARow - 1))
                    {
                        checkType = grid.Blocks[x, (y - 1)].Type;
                        for (int checkY = (y - 2); checkY > (y - blocksInARow); --checkY)
                        {
                            if (checkType != grid.Blocks[x, checkY].Type)
                            {
                                checkType = Block.BlockType.Invalid;
                            }
                        }
                        if (checkType != Block.BlockType.Invalid)
                        {
                            blocksToAvoid.Add(checkType);
                        }
                    }
                    grid.CreateBlock(RandomBlockPrefab(blocksToAvoid, maxNumBlockTypes), x, y).PlayPlacedAnimation();
                }
            }
        }
    }
}
