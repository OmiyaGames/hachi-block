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
                if(value != null)
                {
                    topLeft.sprite = value.Graphic;
                }
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
                if (value != null)
                {
                    topRight.sprite = value.Graphic;
                }
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
                if (value != null)
                {
                    bottomLeft.sprite = value.Graphic;
                }
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
                if (value != null)
                {
                    bottomRight.sprite = value.Graphic;
                }
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
    }
}
