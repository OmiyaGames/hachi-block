using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Community.UI;
using OmiyaGames;

namespace Project
{
    [RequireComponent(typeof(Selectable))]
    public class Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField]
        Image topLeft;
        [SerializeField]
        Image topRight;
        [SerializeField]
        Image bottomLeft;
        [SerializeField]
        Image bottomRight;
        [SerializeField]
        Image topLeftSymbol;
        [SerializeField]
        Image topRightSymbol;
        [SerializeField]
        Image bottomLeftSymbol;
        [SerializeField]
        Image bottomRightSymbol;

        [Header("Collections")]
        [SerializeField]
        BlockCollection allBlocks;
        [SerializeField]
        BlockCursor cursor;
        [SerializeField]
        InventoryCollection collection;
        [SerializeField]
        BlockGrid grid;

        Block topLeftBlock;
        Block topRightBlock;
        Block bottomLeftBlock;
        Block bottomRightBlock;
        Selectable selectable = null;
        bool isEnabled = true;

        #region Properties
        public Block TopLeftBlock
        {
            get
            {
                return topLeftBlock;
            }
            set
            {
                topLeftBlock = value;
                UpdateImage(topLeft, topLeftBlock.Graphic);
                UpdateImage(topLeftSymbol, topLeftBlock.Symbol);
            }
        }

        public Block TopRightBlock
        {
            get
            {
                return topRightBlock;
            }
            set
            {
                topRightBlock = value;
                UpdateImage(topRight, topRightBlock.Graphic);
                UpdateImage(topRightSymbol, topRightBlock.Symbol);
            }
        }

        public Block BottomLeftBlock
        {
            get
            {
                return bottomLeftBlock;
            }
            set
            {
                bottomLeftBlock = value;
                UpdateImage(bottomLeft, bottomLeftBlock.Graphic);
                UpdateImage(bottomLeftSymbol, bottomLeftBlock.Symbol);
            }
        }

        public Block BottomRightBlock
        {
            get
            {
                return bottomRightBlock;
            }
           set
            {
                bottomRightBlock = value;
                UpdateImage(bottomRight, bottomRightBlock.Graphic);
                UpdateImage(bottomRightSymbol, bottomRightBlock.Symbol);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return Selectable.interactable;
            }
            set
            {
                isEnabled = value;
                UpdateControl();
            }
        }

        private Selectable Selectable
        {
            get
            {
                if (selectable == null)
                {
                    selectable = GetComponent<Selectable>();
                }
                return selectable;
            }
        }
        #endregion

        public void Sync(Inventory other)
        {
            if (other != null)
            {
                TopLeftBlock = other.TopLeftBlock;
                TopRightBlock = other.TopRightBlock;
                BottomLeftBlock = other.BottomLeftBlock;
                BottomRightBlock = other.BottomRightBlock;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            collection.HoveredInventory = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (collection.HoveredInventory != this)
            {
                collection.HoveredInventory = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsEnabled == true)
            {
                cursor.ChangeCursorTo(this);
            }
        }

        public void Shuffle()
        {
            TopLeftBlock = allBlocks.RandomBlockPrefab(grid.NumberOfBlockTypes);
            TopRightBlock = allBlocks.RandomBlockPrefab(grid.NumberOfBlockTypes);
            BottomLeftBlock = allBlocks.RandomBlockPrefab(grid.NumberOfBlockTypes);
            BottomRightBlock = allBlocks.RandomBlockPrefab(grid.NumberOfBlockTypes);
        }

        public void Rotate()
        {
            Block swapBlock = TopLeftBlock;
            TopLeftBlock = TopRightBlock;
            TopRightBlock = BottomRightBlock;
            BottomRightBlock = BottomLeftBlock;
            BottomLeftBlock = swapBlock;
        }

        public void UpdateControl()
        {
            Selectable.interactable = (collection.IsAllEnabled && isEnabled);
        }

        private static void UpdateImage(Image image, SpriteRenderer prefab)
        {
            if ((image != null) && (prefab != null))
            {
                image.sprite = prefab.sprite;
                image.color = prefab.color;
            }
        }
    }
}
