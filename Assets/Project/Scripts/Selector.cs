﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OmiyaGames;
using Community.UI;

namespace Project
{
    [DisallowMultipleComponent]
    public class Selector : MonoBehaviour//, ISelectHandler, IDeselectHandler
    {
        [Header("Animation")]
        [SerializeField]
        Animator hoverImage;
        [SerializeField]
        string visibilityFlag = "Visible";

        [Header("Debugging Info")]
        [SerializeField]
        [ReadOnly]
        Vector2Int gridPosition = new Vector2Int(-1, -1);

        public SelectionGrid Grid
        {
            get;
            set;
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

        public bool IsSelected
        {
            get
            {
                return (Grid.CurrentlySelectedSelector == this);
            }
        }

        public bool IsVisible
        {
            get
            {
                return hoverImage.GetBool(visibilityFlag);
            }
            set
            {
                hoverImage.SetBool(visibilityFlag, value);
            }
        }

        public void SetHovered(bool isHovered)
        {
            if(isHovered == true)
            {
                Grid.CurrentlySelectedSelector = this;
            }
            else if(IsSelected == true)
            {
                Grid.CurrentlySelectedSelector = null;
            }
        }

        public void OnPointerUp()
        {
            Debug.Log("ReplaceBlocks called");
            if (IsSelected == true)
            {
                Grid.ReplaceBlocks(GridPosition);
            }
        }
    }
}