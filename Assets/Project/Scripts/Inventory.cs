using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Community.UI;

namespace Project
{
    public class Inventory : MonoBehaviour
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
        BlockCollection allBlocks;
        [SerializeField]
        BlockCursor cursor;

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
        #endregion

        private void Start()
        {
            TopLeftBlock = allBlocks.RandomBlockPrefab();
            TopRightBlock = allBlocks.RandomBlockPrefab();
            BottomLeftBlock = allBlocks.RandomBlockPrefab();
            BottomRightBlock = allBlocks.RandomBlockPrefab();
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

        public void OnClick()
        {
            cursor.SelectedInventory = this;
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
