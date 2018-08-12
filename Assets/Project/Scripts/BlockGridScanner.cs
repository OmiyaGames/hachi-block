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

            public int NumBlocks
            {
                get
                {
                    int returnNum = 0;
                    foreach (Block[] formation in RowFormations)
                    {
                        returnNum = formation.Length;
                    }
                    foreach (Block[] formation in ColumnFormations)
                    {
                        returnNum = formation.Length;
                    }
                    return returnNum;
                }
            }

            public void Reset()
            {
                RowFormations.Clear();
                ColumnFormations.Clear();
            }
        }

        [System.Serializable]
        public class Delay
        {
            [SerializeField]
            float staggerFormation = 0.2f;
            [SerializeField]
            float maxDelayBetweenComboAndElimination = 0.5f;

            // FIXME: figure out a much more elegant way to handle whether a block dropped to the right place or not
            [SerializeField]
            float maxDropWaitTime = 1f;

            WaitForSeconds waitForStaggerFormation = null;

            public float StaggerFormation
            {
                get
                {
                    return staggerFormation;
                }
            }

            public WaitForSeconds WaitForStaggerFormation
            {
                get
                {
                    if(waitForStaggerFormation == null)
                    {
                        waitForStaggerFormation = new WaitForSeconds(StaggerFormation);
                    }
                    return waitForStaggerFormation;
                }
            }

            public float GetDelayBetweenComboAndElimination(int numBlocks)
            {
                float returnTime = maxDelayBetweenComboAndElimination;
                returnTime -= (numBlocks * StaggerFormation);
                return returnTime;
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

        [Header("Animations")]
        [SerializeField]
        Delay animationDelays = new Delay();

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

        public IEnumerator AnimateScan(InventoryCollection enable)
        {
            // Setup variables
            int comboCount = 0;
            bool isFormationFound = false;

            // Disable inventory
            enable.IsAllEnabled = false;

            // Drop blocks first; let them contribute to the combos
            DropNewBlocks();
            yield return StartCoroutine(AnimateBlocksDropping());

            // Scan for any formations
            DiscoveredFormations formations = ScanFormations(comboCount, out isFormationFound);
            while (isFormationFound == true)
            {
                // Update combo counter
                comboCount += formations.RowFormations.Count;
                comboCount += formations.ColumnFormations.Count;

                // Update blocks to indicate they're now in combo state
                yield return StartCoroutine(UpdateFormation(formations, Block.State.Combo));

                // Wait for the combo animation to finish
                float waitTime = animationDelays.GetDelayBetweenComboAndElimination(formations.NumBlocks);
                if(waitTime > 0)
                {
                    yield return new WaitForSeconds(waitTime);
                }

                // Update blocks to indicate they're eliminated
                yield return StartCoroutine(UpdateFormation(formations, Block.State.Eliminated));

                // Wait until the last block is hidden
                Block lastBlock = GetLastBlock(formations);
                while(lastBlock.CurrentState != Block.State.Hidden)
                {
                    yield return null;
                }

                // Animate the blocks dropping
                yield return StartCoroutine(AnimateBlocksDropping());

                // Scan for the next formations
                formations = ScanFormations(comboCount, out isFormationFound);
            }

            // Re-enable inventory
            enable.IsAllEnabled = true;
        }

        private IEnumerator UpdateFormation(DiscoveredFormations formations, Block.State toState)
        {
            foreach (Block[] formation in formations.RowFormations)
            {
                foreach (Block block in formation)
                {
                    if (UpdateFormationBlock(block, toState) == true)
                    {
                        yield return animationDelays.WaitForStaggerFormation;
                    }
                }
            }
            foreach (Block[] formation in formations.ColumnFormations)
            {
                foreach (Block block in formation)
                {
                    if (UpdateFormationBlock(block, toState) == true)
                    {
                        yield return animationDelays.WaitForStaggerFormation;
                    }
                }
            }
        }

        private bool UpdateFormationBlock(Block checkBlock, Block.State toState)
        {
            // Mark the block as detected
            bool returnFlag = false;
            if (((int)checkBlock.CurrentState) < ((int)toState))
            {
                checkBlock.CurrentState = toState;
                returnFlag = true;
            }
            return returnFlag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int DropNewBlocks()
        {
            // FIXME: actually drop blocks
            ++NumMoves;
            return NumMoves;
        }

        private Block GetLastBlock(DiscoveredFormations formation)
        {
            Block returnBlock = null;
            if(formation.ColumnFormations.Count > 0)
            {
                Block[] column = formation.ColumnFormations[formation.ColumnFormations.Count - 1];
                returnBlock = column[column.Length - 1];
            }
            else if(formation.RowFormations.Count > 0)
            {
                Block[] row = formation.RowFormations[formation.RowFormations.Count - 1];
                returnBlock = row[row.Length - 1];
            }
            return returnBlock;
        }

        private DiscoveredFormations ScanFormations(int numCombos, out bool isFormationFound)
        {
            // Clear the list
            ScannedFormations.Reset();

            // Drop the blocks, first
            AnimateBlocksDropping();

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

        private IEnumerator AnimateBlocksDropping()
        {
            yield return null;
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
