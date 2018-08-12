using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project
{
    public class BlockCursorDeselector : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        BlockCursor cursor;


        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                cursor.Deselect();
            }
        }
    }
}
