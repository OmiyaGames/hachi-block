﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Community.UI;
using OmiyaGames;

namespace Project
{
    public class Block : IPooledObject
    {
        public enum BlockType
        {
            T1,
            T2
        }

        [SerializeField]
        BlockType type;

        [Header("Debugging Info")]
        [SerializeField]
        [ReadOnly]
        Vector2Int gridPosition = new Vector2Int(-1, -1);

        #region Properties
        public BlockType Type
        {
            get
            {
                return type;
            }
        }

        public Vector2Int GridPosition
        {
            get
            {
                return gridPosition;
            }
            set
            {
                gridPosition = value;
            }
        }

        public Grid Grid
        {
            get;
            set;
        }
        #endregion
    }
}