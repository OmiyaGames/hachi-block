using System.Collections;
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

        public SelectionGrid Grid
        {
            get;
            set;
        }

        public Vector2Int GridPosition { get; set; } = new Vector2Int(-1, -1);

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
            if ((isHovered == true) && (Grid.CanSelect == true))
            {
                Grid.CurrentlySelectedSelector = this;
            }
            else if (IsSelected == true)
            {
                Grid.CurrentlySelectedSelector = null;
            }
        }
    }
}
