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

        [Header("UI")]
        [SerializeField]
        CanvasScaler canvas;
        [SerializeField]
        LayoutGroup layout;
        [SerializeField]
        Selector selector;

        #region Properties
        public int UiWidth
        {
            get
            {
                return (grid.Width - 1);
            }
        }

        public float UiHeight
        {
            get
            {
                return (grid.Height - 1);
            }
        }

        public float UiCellLength
        {
            get
            {
                return (grid.CellLength * canvas.referencePixelsPerUnit);
            }
        }
        #endregion

        // Use this for initialization
        void Start()
        {
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
                    clone = Instantiate(selector.gameObject, layout.transform);
                    clone.transform.localScale = Vector3.one;
                    clonedScript = clone.GetComponent<Selector>();
                    clonedScript.GridPosition = new Vector2Int(x, y);
                }
            }
        }
    }
}
