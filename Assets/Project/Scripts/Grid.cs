using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    public class Grid : MonoBehaviour
    {
        [Header("Grid Dimensions")]
        [SerializeField]
        int gridWidth = 10;
        [SerializeField]
        int gridHeight = 10;

        [Header("Cell Dimensions")]
        [SerializeField]
        float cellLength = 0.2f;

        // FIXME: probably want to come up with some initial values
        [Header("Start Grid")]
        [SerializeField]
        Block[] allBlockTypes;

        Block[,] grid = null;

        #region Properties
        public Block[,] BlockGrid
        {
            get
            {
                if(grid == null)
                {
                    grid = new Block[gridWidth, gridHeight];
                }
                return grid;
            }
        }

        public static PoolingManager Pool
        {
            get
            {
                return Singleton.Get<PoolingManager>();
            }
        }
        #endregion

        // Use this for initialization
        void Start()
        {
            for(int x = 0; x < gridWidth; ++x)
            {
                for(int y = 0; y < gridHeight; ++y)
                {
                    CreateBlock(RandomBlock(), x, y);
                }
            }
        }

        public bool IsValidGridPosition(int x, int y)
        {
            return (x >= 0) && (x < gridWidth) && (y >= 0) && (y < gridHeight);
        }

        public bool IsValidGridPosition(Vector2Int gridPosition)
        {
            return IsValidGridPosition(gridPosition.x, gridPosition.y);
        }

        // FIXME: create a second function that double-checks
        // previous blocks in the grid, and makes sure it
        // doesn't trigger a combo
        public Block RandomBlock()
        {
            return allBlockTypes[Random.Range(0, allBlockTypes.Length)];
        }

        public Vector3 ConvertGridToWorldPosition(Vector2Int gridPosition)
        {
            return new Vector3(ConvertGridToWorldUnit(gridPosition.x, gridWidth), ConvertGridToWorldUnit(gridPosition.y, gridHeight));
        }

        public Block CreateBlock(Block blockType, int x, int y)
        {
            Block returnBlock = null;
            if (IsValidGridPosition(x, y) == true)
            {
                // Grab a block from the pool
                returnBlock = Pool.GetInstance<Block>(blockType);
                MoveBlock(returnBlock, x, y);
            }
            return returnBlock;
        }

        public Block MoveBlock(Block block, int x, int y)
        {
            Block oldBlock = null;
            if ((block != null) && (block.isActiveAndEnabled == true) && (IsValidGridPosition(x, y) == true))
            {
                // Remove a block from this grid position
                oldBlock = RemoveBlock(x, y);

                // Update block position
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Convert to world position
                Vector3 worldPosition = ConvertGridToWorldPosition(gridPosition);

                // Update the transform
                block.transform.position = worldPosition;

                // Check if the block had a valid position
                if(IsValidGridPosition(block.GridPosition) == true)
                {
                    // If so, make sure the grid position is set to null
                    BlockGrid[block.GridPosition.x, block.GridPosition.y] = null;
                }

                // Update the member variables
                block.GridPosition = gridPosition;
                block.Grid = this;

                // Set this block location to block
                BlockGrid[x, y] = block;
            }
            return oldBlock;
        }

        public Block RemoveBlock(int x, int y)
        {
            Block oldBlock = null;
            if (IsValidGridPosition(x, y) == true)
            {
                // Check if there's a block in this location already
                oldBlock = BlockGrid[x, y];
                if (oldBlock != null)
                {
                    // If there is, return this block to the pool
                    PoolingManager.ReturnToPool(oldBlock);
                    BlockGrid[x, y] = null;
                }
            }
            return oldBlock;
        }

        private float ConvertGridToWorldUnit(int unit, int maxUnit)
        {
            return (unit * cellLength) - ((maxUnit * cellLength) / 2f);
        }
    }
}