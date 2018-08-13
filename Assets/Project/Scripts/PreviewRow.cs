using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class PreviewRow : MonoBehaviour
    {
        [SerializeField]
        PreviewCell cell;
        [SerializeField]
        BlockGrid grid;
        [SerializeField]
        BlockGridScanner scanner;

        public PreviewCell[] Cells
        {
            get;
            set;
        }

        // Use this for initialization
        void Start()
        {
            // Setup the first cell
            PreviewCell updateCell = cell;
            updateCell.Block = null;

            // Setup the array
            Cells = new PreviewCell[grid.Width];
            Cells[0] = updateCell;
            for(int x = 1; x < grid.Width; ++x)
            {
                // Clone this cell
                GameObject clone = Instantiate(cell.gameObject, cell.transform.parent);
                clone.transform.localScale = Vector3.one;

                // Add this cell to array
                updateCell = clone.GetComponent<PreviewCell>();
                updateCell.Block = null;
                Cells[x] = updateCell;
            }

            if (LastGameSettings.Instance.RestorePreviewSettings() == false)
            {
                Shuffle();
            }
        }

        public void Shuffle()
        {
            Block[] row = new Block[Cells.Length];

            for (int x = 0; x < scanner.NumBlocksToDrop; ++x)
            {
                row[x] = grid.AllBlocks.RandomBlockPrefab(grid.NumberOfBlockTypes);
            }
            OmiyaGames.Utility.ShuffleList<Block>(row, scanner.NumBlocksToDrop);

            for (int x = 0; x < Cells.Length; ++x)
            {
                Cells[x].Block = row[x];
            }
        }
    }
}
