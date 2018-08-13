using UnityEngine;
using Community.UI;
using OmiyaGames.Audio;

namespace Project
{
    public class BlockCursor : MonoBehaviour
    {
        public enum SoundType
        {
            Cancel,
            Drop,
            Shuffle
        }

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

        [Header("Sounds")]
        [SerializeField]
        SoundEffect pickUpSound;
        [SerializeField]
        SoundEffect rotateSound;
        [SerializeField]
        SoundEffect cancelSound;
        [SerializeField]
        SoundEffect dropSound;
        [SerializeField]
        SoundEffect shuffleSound;

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
            private set
            {
                if (selectedInventory != null)
                {
                    selectedInventory.IsEnabled = true;
                }
                selectedInventory = value;
                if (selectedInventory != null)
                {
                    selectedInventory.IsEnabled = false;
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

        private void Awake()
        {
            originalLocation = transform.position;
            plane = new Plane(Vector3.back, originalLocation);
        }

        // Update is called once per frame
        void Update()
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance) == true)
            {
                transform.position = ray.GetPoint(distance);
            }
            if (IsDragging == true)
            {
                if ((Input.GetMouseButtonUp(1) == true) || (Input.GetButtonUp("Rotate") == true) || (Input.GetButtonUp("Submit") == true))
                {
                    cursorInventory.Rotate();
                    SelectedInventory.Rotate();
                    Play(rotateSound);
                }
            }
        }

        public void ChangeCursorTo(Inventory inventory)
        {
            SelectedInventory = inventory;
        }

        public void HideCursor(SoundType sound)
        {
            SelectedInventory = null;
            switch (sound)
            {
                case SoundType.Shuffle:
                    Play(shuffleSound);
                    break;
                case SoundType.Drop:
                    Play(dropSound);
                    break;
                default:
                    Play(cancelSound);
                    break;
            }
        }

        private static void Play(SoundEffect sound)
        {
            if (sound != null)
            {
                sound.Play();
            }
        }

        private static void Stop(SoundEffect sound)
        {
            if (sound != null)
            {
                sound.Stop();
            }
        }
    }
}
