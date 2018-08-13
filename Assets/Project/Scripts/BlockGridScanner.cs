using System.Collections;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Menu;

namespace Project
{
    public partial class BlockGridScanner : MonoBehaviour
    {
        public event System.Action<BlockGridScanner> OnMove;

        [System.Serializable]
        public class Delay
        {
            [SerializeField]
            float staggerFormation = 0.2f;
            [SerializeField]
            float maxDelayBetweenComboAndElimination = 0.5f;

            // TODO: figure out a much more elegant way to handle whether a block dropped to the right place or not
            [SerializeField]
            float maxDropWaitTime = 1f;

            WaitForSeconds waitForStaggerFormation = null;
            WaitForSeconds waitForDrop = null;

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
                    if (waitForStaggerFormation == null)
                    {
                        waitForStaggerFormation = new WaitForSeconds(StaggerFormation);
                    }
                    return waitForStaggerFormation;
                }
            }

            public float MaxDropWaitTime
            {
                get
                {
                    return maxDropWaitTime;
                }
            }

            public WaitForSeconds WaitForDrop
            {
                get
                {
                    if (waitForDrop == null)
                    {
                        waitForDrop = new WaitForSeconds(MaxDropWaitTime);
                    }
                    return waitForDrop;
                }
            }

            public float GetDelayBetweenComboAndElimination(int numBlocks)
            {
                float returnTime = maxDelayBetweenComboAndElimination;
                returnTime -= (numBlocks * StaggerFormation);
                return returnTime;
            }
        }

        [System.Serializable]
        public class ScoreCalculator
        {
            [SerializeField]
            int baseScore = 10;
            [SerializeField]
            float multiplierForExtraBlocks = 2;
            [SerializeField]
            float incrementMultiplierPerComboBy = 0.5f;
            [SerializeField]
            float maxMultiplier = 4f;

            public int GetScore(Block[] formation, int blocksInARow, int comboCounter)
            {
                // Calculate base formation score
                float returnScore = baseScore;
                returnScore += ((formation.Length - blocksInARow) * multiplierForExtraBlocks);

                // Calculate combo multiplier
                float comboMultiplier = 1f;
                comboMultiplier += (incrementMultiplierPerComboBy * comboCounter);
                if(comboMultiplier > maxMultiplier)
                {
                    comboMultiplier = maxMultiplier;
                }

                // Multiply by combo multiplier
                returnScore *= comboMultiplier;

                // Round up
                return Mathf.CeilToInt(returnScore);
            }
        }

        [SerializeField]
        BlockGrid grid;
        [SerializeField]
        [Range(3, 5)]
        int blocksInARow = 4;
        [SerializeField]
        [Range(3, 12)]
        int numBlocksToDrop = 4;
        [SerializeField]
        TMPro.TextMeshProUGUI movesLabel;

        [Header("Scoring")]
        [SerializeField]
        ScoreCalculator scoreCalculator;
        [SerializeField]
        TMPro.TextMeshProUGUI scoreLabel;

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

        ulong[] lastTopRowBlockIds = null;

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
                movesLabel.text = numMoves.ToString();
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
                scoreLabel.text = score.ToString();
            }
        }

        public int BlocksInARow
        {
            get
            {
                return blocksInARow;
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
            DropNewBlocks(numBlocksToDrop);
            yield return StartCoroutine(AnimateBlocksDropping());

            // Scan for any formations
            DiscoveredFormations formations = ScanFormations(comboCount, out isFormationFound);
            while (isFormationFound == true)
            {
                // Calculate the score
                formations.SortLists();
                UpdateScore(formations, ref comboCount);

                // Update blocks to indicate they're now in combo state
                yield return StartCoroutine(UpdateFormation(formations, Block.State.Combo));

                // Wait for the combo animation to finish
                float waitTime = animationDelays.GetDelayBetweenComboAndElimination(formations.NumBlocks);
                if (waitTime > 0)
                {
                    yield return new WaitForSeconds(waitTime);
                }

                // Update blocks to indicate they're eliminated
                yield return StartCoroutine(UpdateFormation(formations, Block.State.Eliminated));

                // Wait until the last block is hidden
                Block lastBlock = GetLastBlock(formations);
                while (lastBlock.CurrentState != Block.State.Hidden)
                {
                    yield return null;
                }

                // Remove all blocks
                RemoveFormations(formations);

                // Animate the blocks dropping
                yield return StartCoroutine(AnimateBlocksDropping());

                // Scan for the next formations
                isFormationFound = false;
                formations = ScanFormations(comboCount, out isFormationFound);
            }

            // Check if the player should end the game or not.
            CheckGameOver();

            // Re-enable inventory
            enable.IsAllEnabled = true;
            OnMove?.Invoke(this);
        }

        private void CheckGameOver()
        {
            // Check if we're already keeping track of the top row
            Block checkBlock;
            if (lastTopRowBlockIds != null)
            {
                // Go through the grid's top row
                bool isGameOver = false;
                for (int x = 0; x < grid.Width; ++x)
                {
                    // Check if this block was at the top row on the previous move
                    checkBlock = grid.Blocks[x, (grid.Height - 1)];
                    if ((checkBlock != null) && (checkBlock.Id == lastTopRowBlockIds[x]))
                    {
                        // If so, indicate game over
                        isGameOver = true;
                        break;
                    }
                }

                // Check if we got a game over
                if (isGameOver == true)
                {
                    // Show the game over screen
                    Singleton.Get<MenuManager>().Show<LevelFailedMenu>();

                    // Attempt to add a new high score
                    OmiyaGames.Settings.GameSettings settings = Singleton.Get<OmiyaGames.Settings.GameSettings>();
                    OmiyaGames.Settings.IRecord<int> record;
                    int placement = settings.HighScores.AddRecord(Score, settings.LastEnteredName, out record);

                    // Check if we got a new high score
                    if (placement >= 0)
                    {
                        // Also show the new high score menu
                        NewHighScoreMenu menu = Singleton.Get<MenuManager>().GetMenu<NewHighScoreMenu>();
                        menu.Setup(placement, record);
                        menu.Show();
                    }
                }
            }
            else
            {
                // Create cache
                lastTopRowBlockIds = new ulong[grid.Width];
            }

            // Go through the grid's top row
            for (int x = 0; x < grid.Width; ++x)
            {
                checkBlock = grid.Blocks[x, (grid.Height - 1)];
                if (checkBlock == null)
                {
                    lastTopRowBlockIds[x] = Block.IdNull;
                }
                else
                {
                    lastTopRowBlockIds[x] = checkBlock.Id;
                }
            }
        }

        private void UpdateScore(DiscoveredFormations formations, ref int comboCounter)
        {
            // Go through each formation
            foreach (Block[] formation in formations.AllFormations)
            {
                // Increment score
                Score += scoreCalculator.GetScore(formation, BlocksInARow, comboCounter);
                ++comboCounter;
            }
        }

        private void RemoveFormations(DiscoveredFormations formations)
        {
            foreach (Block block in formations.AllClearedBlocks)
            {
                grid.RemoveBlock(block.GridPosition);
            }
        }

        private IEnumerator UpdateFormation(DiscoveredFormations formations, Block.State toState)
        {
            foreach (Block block in formations.AllClearedBlocks)
            {
                if (UpdateFormationBlock(block, toState) == true)
                {
                    yield return animationDelays.WaitForStaggerFormation;
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
        private int DropNewBlocks(int numBlocksToDrop)
        {
            // FIXME: the argument should take in an array
            ++NumMoves;

            Block[] row = new Block[grid.Width];

            for (int x = 0; x < numBlocksToDrop; ++x)
            {
                row[x] = grid.AllBlocks.RandomBlockPrefab(grid.StartingNumberOfBlockTypes);
            }
            Utility.ShuffleList<Block>(row, numBlocksToDrop);

            // Actually drop blocks
            for (int x = 0; x < grid.Width; ++x)
            {
                // Make sure the row to drop blocks contains a block,
                // and the grid has an empty cell
                if ((row[x] != null) && (grid.Blocks[x, (grid.Height - 1)] == null))
                {
                    grid.CreateBlock(row[x], x, (grid.Height - 1));
                }
            }
            return NumMoves;
        }

        private Block GetLastBlock(DiscoveredFormations formation)
        {
            Block returnBlock = null;
            if (formation.ColumnFormations.Count > 0)
            {
                Block[] column = formation.ColumnFormations[formation.ColumnFormations.Count - 1];
                returnBlock = column[column.Length - 1];
            }
            else if (formation.RowFormations.Count > 0)
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
                while (x < (grid.Width - BlocksInARow + 1))
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
                while (y < (grid.Height - BlocksInARow + 1))
                {
                    y = ScanForCombos(x, y, false);
                }
            }
        }

        private IEnumerator AnimateBlocksDropping()
        {
            // Setup variables
            int emptyIndex = -1, gap = 0, maxGap = 0;
            Block checkBlock = null;

            // Go through each column
            for (int x = 0; x < grid.Width; ++x)
            {
                // Reset variables
                emptyIndex = -1;

                // Go through each cell, starting at the bottom
                for (int y = 0; y < grid.Height; ++y)
                {
                    // Grab the block
                    checkBlock = grid.Blocks[x, y];

                    // Check if we've found an empty cell yet
                    if ((emptyIndex < 0) && (checkBlock == null))
                    {
                        // If it's found, mark the y-coordinate
                        emptyIndex = y;
                    }
                    else if ((emptyIndex >= 0) && (checkBlock != null))
                    {
                        // Move the block into the bottom-most empty cell
                        grid.MoveBlock(checkBlock, x, emptyIndex, false);
                        checkBlock.CurrentState = Block.State.Fall;

                        // Get maximum gap
                        gap = (y - emptyIndex);
                        if (maxGap < gap)
                        {
                            maxGap = gap;
                        }

                        // Increment empty index
                        ++emptyIndex;
                    }
                }
            }

            // Check if there are any gaps
            if (maxGap > 0)
            {
                yield return animationDelays.WaitForDrop;
            }
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
            else if (LengthOfFormation(x, y, startIndex, ref endIndex, maxIndex, compareBlock, checkRow) >= BlocksInARow)
            {
                // At this point, we found a row. Look for a pre-existing hashset.
                Block[] newFormation = CreateNewFormation(x, y, startIndex, endIndex, checkRow);

                // Add this formation in the appropriate list
                ScannedFormations.AddFormation(newFormation, checkRow);
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
