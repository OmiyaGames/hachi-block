using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    [DisallowMultipleComponent]
    public class BlockGrid : MonoBehaviour
    {
        [Header("Grid Dimensions")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("gridWidth")]
        [Range(2, 30)]
        int width = 10;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("gridHeight")]
        [Range(2, 20)]
        int height = 10;

        [Header("Cell Dimensions")]
        [SerializeField]
        float cellLength = 0.2f;
        [SerializeField]
        Color gizmoColor = Color.cyan;

        Block[,] grid = null;

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public Block[,] Blocks
        {
            get
            {
                if(grid == null)
                {
                    grid = new Block[Width, Height];
                }
                return grid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
        }

        public float CellLength
        {
            get
            {
                return cellLength;
            }
        }

        static PoolingManager Pool
        {
            get
            {
                return Singleton.Get<PoolingManager>();
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsValidGridPosition(int x, int y)
        {
            return (x >= 0) && (x < Width) && (y >= 0) && (y < Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool IsValidGridPosition(Vector2Int gridPosition)
        {
            return IsValidGridPosition(gridPosition.x, gridPosition.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Vector3 ConvertGridToWorldPosition(Vector2Int gridPosition)
        {
            Vector3 returnPosition = transform.position;
            returnPosition.x += ConvertGridToWorldUnit(gridPosition.x, Width);
            returnPosition.y += ConvertGridToWorldUnit(gridPosition.y, Height);
            return returnPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockType"></param>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Block CreateBlock(Block blockType, Vector2Int gridPosition)
        {
            return CreateBlock(blockType, gridPosition.x, gridPosition.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
                    Blocks[block.GridPosition.x, block.GridPosition.y] = null;
                }

                // Update the member variables
                block.GridPosition = gridPosition;
                block.Grid = this;

                // Set this block location to block
                Blocks[x, y] = block;
            }
            return oldBlock;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Block MoveBlock(Block block, Vector2Int gridPosition)
        {
            return MoveBlock(block, gridPosition.x, gridPosition.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Block RemoveBlock(int x, int y)
        {
            Block oldBlock = null;
            if (IsValidGridPosition(x, y) == true)
            {
                // Check if there's a block in this location already
                oldBlock = Blocks[x, y];
                if (oldBlock != null)
                {
                    // If there is, return this block to the pool
                    PoolingManager.ReturnToPool(oldBlock);
                    Blocks[x, y] = null;
                }
            }
            return oldBlock;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Block RemoveBlock(Vector2Int gridPosition)
        {
            return RemoveBlock(gridPosition.x, gridPosition.y);
        }

        #region Helper Methods
        private float ConvertGridToWorldUnit(int unit, int maxUnit)
        {
            return (unit * CellLength) - (((maxUnit - 1) * CellLength) / 2f);
        }

        private void OnDrawGizmos()
        {
            Vector3 center;
            Vector3 size = new Vector3(CellLength, CellLength);
            Vector2Int gridPos = Vector2Int.zero;
            Gizmos.color = gizmoColor;
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    gridPos.x = x;
                    gridPos.y = y;

                    center = ConvertGridToWorldPosition(gridPos);

                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
        #endregion
    }
}