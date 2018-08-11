using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    [DisallowMultipleComponent]
    public class SelectionGrid : MonoBehaviour
    {
        [SerializeField]
        BlockGrid grid;
        [SerializeField]
        BlockCursor cursor;

        [Header("UI")]
        [SerializeField]
        CanvasScaler canvas;
        [SerializeField]
        LayoutGroup layout;
        [SerializeField]
        Selector selector;
        [SerializeField]
        InventoryCollection inventories;

        [Header("Test variables, to remove")]
        [SerializeField]
        Block block;

        Selector currentlySelectedSelector = null;

        #region Properties
        public int UiWidth
        {
            get
            {
                return (BlockGrid.Width - 1);
            }
        }

        public float UiHeight
        {
            get
            {
                return (BlockGrid.Height - 1);
            }
        }

        public float UiCellLength
        {
            get
            {
                return (BlockGrid.CellLength * canvas.referencePixelsPerUnit);
            }
        }

        public Selector CurrentlySelectedSelector
        {
            get
            {
                return currentlySelectedSelector;
            }
            set
            {
                if (CurrentlySelectedSelector != null)
                {
                    CurrentlySelectedSelector.IsVisible = false;
                }
                currentlySelectedSelector = value;
                if (CurrentlySelectedSelector != null)
                {
                    CurrentlySelectedSelector.IsVisible = true;
                }
            }
        }

        public BlockGrid BlockGrid
        {
            get
            {
                return grid;
            }
        }

        public bool CanSelect
        {
            get
            {
                return cursor.IsDragging;
            }
        }
        #endregion

        // Use this for initialization
        void Start()
        {
            CurrentlySelectedSelector = null;

            RectTransform canvasTransform = canvas.transform as RectTransform;
            if (canvasTransform != null)
            {
                canvasTransform.sizeDelta = new Vector2((UiCellLength * UiWidth), (UiCellLength * UiHeight));
            }

            GameObject clone;
            Selector clonedScript;
            for (int y = 0; y < UiHeight; ++y)
            {
                for (int x = 0; x < UiWidth; ++x)
                {
                    // Clone the selector!
                    clone = Instantiate(selector.gameObject, layout.transform);
                    clone.transform.localScale = Vector3.one;

                    // Setup the script!
                    clonedScript = clone.GetComponent<Selector>();
                    clonedScript.GridPosition = new Vector2Int(x, y);
                    clonedScript.Grid = this;
                }
            }
        }

        private void Update()
        {
            // Check if the mouse button is up
            if (Input.GetMouseButtonUp(0) == true)
            {
                if (CurrentlySelectedSelector != null)
                {
                    ReplaceBlocks(CurrentlySelectedSelector);
                    CurrentlySelectedSelector.SetHovered(false);
                    inventories.HoveredInventory = null;
                }
            }
        }

        public void ReplaceBlocks(Selector selector)
        {
            if (selector != null)
            {
                ReplaceBlocks(selector.GridPosition);
            }
        }

        public void ReplaceBlocks(Vector2Int position)
        {
            BlockGrid.CreateBlock(block, position);
            position.x += 1;
            BlockGrid.CreateBlock(block, position);
            position.y += 1;
            BlockGrid.CreateBlock(block, position);
            position.x -= 1;
            BlockGrid.CreateBlock(block, position);
        }
    }
}
