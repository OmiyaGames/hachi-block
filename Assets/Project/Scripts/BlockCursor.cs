using UnityEngine;
using Community.UI;

namespace Project
{
    public class BlockCursor : MonoBehaviour
    {
        [SerializeField]
        Camera mainCamera;
        [SerializeField]
        Animator cursorAnimator;
        [SerializeField]
        string visibilityField = "Visible";

        [Header("Inventory")]
        [SerializeField]
        Inventory cursorInventory;
        [SerializeField]
        [ReadOnly]
        Inventory selectedInventory;

        Vector3 originalLocation;
        RaycastHit hit;
        Ray ray;
        Plane plane;
        float distance;

        #region Properties
        public bool IsDragging
        {
            get;
            private set;
        } = false;

        public Inventory SelectedInventory
        {
            get
            {
                return selectedInventory;
            }
            set
            {
                if(selectedInventory != null)
                {
                    selectedInventory.Selectable.interactable = true;
                }
                selectedInventory = value;
                if(selectedInventory != null)
                {
                    selectedInventory.Selectable.interactable = false;
                    cursorInventory.Sync(selectedInventory);
                    IsDragging = true;
                }
                else
                {
                    IsDragging = false;
                }
                cursorAnimator.SetBool(visibilityField, IsDragging);
            }
        }
        #endregion

        private void Start()
        {
            originalLocation = transform.position;
            plane = new Plane(Vector3.back, originalLocation);
        }

        // Update is called once per frame
        void Update()
        {
            if(IsDragging == true)
            {
                ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if(plane.Raycast(ray, out distance) == true)
                {
                    transform.position = ray.GetPoint(distance);
                }
                if(Input.GetMouseButtonUp(1) == true)
                {
                    cursorInventory.Rotate();
                    SelectedInventory.Rotate();
                }
            }
        }

        public void Deselect()
        {
            SelectedInventory = null;
        }
    }
}
