﻿using System.Collections;
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

        [Header("Collections")]
        [SerializeField]
        BlockCollection allBlocks;
        [SerializeField]
        BlockCursor cursor;
        [SerializeField]
        InventoryCollection collection;

        [Header("Debug Info")]
        [SerializeField]
        [ReadOnly]
        Block topLeftBlock;
        [SerializeField]
        [ReadOnly]
        Block topRightBlock;
        [SerializeField]
        [ReadOnly]
        Block bottomLeftBlock;
        [SerializeField]
        [ReadOnly]
        Block bottomRightBlock;

        Selectable selectable = null;

        #region Properties
        public Block TopLeftBlock
        {
            get
            {
                return topLeftBlock;
            }
            private set
            {
                topLeftBlock = value;
                UpdateImage(topLeft, value);
            }
        }

        public Block TopRightBlock
        {
            get
            {
                return topRightBlock;
            }
            private set
            {
                topRightBlock = value;
                UpdateImage(topRight, value);
            }
        }

        public Block BottomLeftBlock
        {
            get
            {
                return bottomLeftBlock;
            }
            private set
            {
                bottomLeftBlock = value;
                UpdateImage(bottomLeft, value);
            }
        }

        public Block BottomRightBlock
        {
            get
            {
                return bottomRightBlock;
            }
            private set
            {
                bottomRightBlock = value;
                UpdateImage(bottomRight, value);
            }
        }

        public Selectable Selectable
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

        private void Start()
        {
            Shuffle();
        }

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
            cursor.SelectedInventory = this;
        }

        public void Shuffle()
        {
            TopLeftBlock = allBlocks.RandomBlockPrefab();
            TopRightBlock = allBlocks.RandomBlockPrefab();
            BottomLeftBlock = allBlocks.RandomBlockPrefab();
            BottomRightBlock = allBlocks.RandomBlockPrefab();
        }

        public void Rotate()
        {
            Block swapBlock = TopLeftBlock;
            TopLeftBlock = TopRightBlock;
            TopRightBlock = BottomRightBlock;
            BottomRightBlock = BottomLeftBlock;
            BottomLeftBlock = swapBlock;
        }

        private static void UpdateImage(Image image, Block prefab)
        {
            if ((image != null) && (prefab != null))
            {
                image.sprite = prefab.Graphic.sprite;
                image.color = prefab.Graphic.color;
            }
        }
    }
}