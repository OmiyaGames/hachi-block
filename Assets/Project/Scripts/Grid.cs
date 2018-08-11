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

        // Use this for initialization
        void Start()
        {
            Block newBlock = null;

            for(int x = 0; x < gridWidth; ++x)
            {
                for(int y = 0; y < gridHeight; ++y)
                {
                    newBlock = CreateBlock(RandomBlock(), x, y);
                    BlockGrid[x, y] = newBlock;
                }
            }
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
            // FIXME: update position to be more centered on the screen
            Vector3 worldPosition = new Vector3((gridPosition.x * cellLength), (gridPosition.y * cellLength));
            return worldPosition;
        }

        private Block CreateBlock(Block newBlock, int x, int y)
        {
            Block returnBlock = null;
            if ((x >= 0) && (x < gridWidth) && (y >= 0) && (y < gridHeight))
            {
                // Update block position
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Convert to world position
                Vector3 worldPosition = ConvertGridToWorldPosition(gridPosition);

                // FIXME: update the 
                returnBlock = Pool.GetInstance<Block>(newBlock, worldPosition, Quaternion.identity);
                returnBlock.GridPosition = gridPosition;
                returnBlock.Grid = this;
            }
            return returnBlock;
        }
    }
}