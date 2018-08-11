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

        [Header("Debugging Info")]
        [SerializeField]
        [ReadOnly]
        Vector2Int gridPosition = new Vector2Int(-1, -1);

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

        public SelectionGrid Grid
        {
            get;
            set;
        }

        //public void OnSelect(BaseEventData eventData)
        //{
        //    hoverImage.SetBool(visibilityFlag, true);
        //}

        //public void OnDeselect(BaseEventData eventData)
        //{
        //    hoverImage.SetBool(visibilityFlag, false);
        //}

        public void SetVisible(bool visible)
        {
            hoverImage.SetBool(visibilityFlag, visible);
        }
    }
}
