using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class BlockGridScanner : MonoBehaviour
    {
        public class DiscoveredFormations
        {
            public List<Block[]> RowFormations
            {
                get;
            } = new List<Block[]>();

            public List<Block[]> ColumnFormations
            {
                get;
            } = new List<Block[]>();

            public bool IsFormationFound
            {
                get
                {
                    return ((RowFormations.Count + ColumnFormations.Count) > 0);
                }
            }

            public void Reset()
            {
                RowFormations.Clear();
                ColumnFormations.Clear();
            }
        }

        [SerializeField]
        BlockGrid grid;
        [SerializeField]
        [Range(3, 5)]
        int blocksInARow = 4;

        [Header("Scoring")]
        [SerializeField]
        int baseScore = 3;
        [SerializeField]
        int scoreForExtraBlocks = 1;
        [SerializeField]
        float incrementMultiplierPerComboBy = 0.5f;
        [SerializeField]
        float maxMultiplier = 4f;

        [Header("Debug Info")]
        [SerializeField]
        [Community.UI.ReadOnly]
        int numMoves = 0;
        [SerializeField]
        [Community.UI.ReadOnly]
        int score = 0;

        #region Properties
        public DiscoveredFormations ScannedFormations
        {
            get;
        } = new DiscoveredFormations();

        public int NumMoves
        {
            get
            {
                return numMoves;
            }
            private set
            {
                numMoves = value;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            private set
            {
                score = value;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int DropNewBlocks()
        {
            ++NumMoves;
            return NumMoves;
        }

        public DiscoveredFormations ScanFormations(int numCombos, out bool isFormationFound)
        {
            // Clear the list
            ScannedFormations.Reset();

            // Drop the blocks, first
            DropBlocks();

            // Scan the grid for any formations
            ScanRowsForCombos(numCombos);
            ScanColumnsForCombos(numCombos);

            // Check how many formations were found
            isFormationFound = ScannedFormations.IsFormationFound;
            return ScannedFormations;
        }

        private void ScanRowsForCombos(int numCombos)
        {
            for (int y = 0; y < grid.Height; ++y)
            {
                int x = 0;
                while (x < (grid.Width - blocksInARow + 1))
                {
                    x = ScanForCombos(x, y, true);
                }
            }
        }

        private void ScanColumnsForCombos(int numCombos)
        {
            for (int x = 0; x < grid.Width; ++x)
            {
                int y = 0;
                while (y < (grid.Height - blocksInARow + 1))
                {
                    y = ScanForCombos(x, y, false);
                }
            }
        }

        private void DropBlocks()
        {

        }

        private int ScanForCombos(int x, int y, bool checkRow)
        {
            // Setup variables
            int startIndex = y, maxIndex = grid.Height;
            if (checkRow == true)
            {
                startIndex = x;
                maxIndex = grid.Width;
            }

            // Grab the first block
            Block compareBlock = grid.Blocks[x, y];

            // Before check anything, increment the end index on unit past the start
            int endIndex = (startIndex + 1);

            // Check the block
            if (compareBlock == null)
            {
                // Return the end index immediately if this is an empty cell
                return endIndex;
            }
            else if (LengthOfFormation(x, y, startIndex, ref endIndex, maxIndex, compareBlock, checkRow) >= blocksInARow)
            {
                // At this point, we found a row. Look for a pre-existing hashset.
                Block[] newFormation = CreateNewFormation(x, y, startIndex, endIndex, checkRow);

                // Add this formation in the appropriate list
                if (checkRow == true)
                {
                    ScannedFormations.RowFormations.Add(newFormation);
                }
                else
                {
                    ScannedFormations.ColumnFormations.Add(newFormation);
                }
            }
            return endIndex;
        }

        private Block[] CreateNewFormation(int x, int y, int startIndex, int endIndex, bool checkRow)
        {
            Block[] newFormation = new Block[(endIndex - startIndex)];
            for (int index = 0; index < newFormation.Length; ++index)
            {
                if (checkRow == true)
                {
                    newFormation[index] = grid.Blocks[(startIndex + index), y];
                }
                else
                {
                    newFormation[index] = grid.Blocks[x, (startIndex + index)];
                }
            }

            return newFormation;
        }

        private int LengthOfFormation(int x, int y, int startIndex, ref int endIndex, int maxIndex, Block compareBlock, bool checkRow)
        {
            // Go through each block to the right of compareBlock
            Block checkBlock;
            for (; endIndex < maxIndex; ++endIndex)
            {
                // Grab a block to check
                if (checkRow == true)
                {
                    checkBlock = grid.Blocks[endIndex, y];
                }
                else
                {
                    checkBlock = grid.Blocks[x, endIndex];
                }

                // Check if this block is like compareBlock
                if (checkBlock == null)
                {
                    // Nope, this is an empty cell
                    break;
                }
                else if (compareBlock.Type != checkBlock.Type)
                {
                    // Nope, this is a different block
                    break;
                }
            }

            return (endIndex - startIndex);
        }
    }
}
