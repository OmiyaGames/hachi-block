using UnityEngine;
using Community.UI;
using OmiyaGames;
using System;

namespace Project
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class Block : IPooledObject
    {
        public enum BlockType
        {
            Invalid = -1,
            Base,
            Light,
            Dark,
            Strange,
            Quartz
        }

        public enum State
        {
            Fall = -1,
            Idle,
            Combo,
            Eliminated,
            Hidden
        }

        [SerializeField]
        BlockType type;
        [SerializeField]
        SpriteRenderer graphic;
        [SerializeField]
        float gravity = 9.81f;

        [Header("Animation")]
        [SerializeField]
        string stateFieldName = "State";
        [SerializeField]
        string placedTriggerName = "Placed";

        [Header("Debugging Info")]
        [SerializeField]
        [ReadOnly]
        Vector2Int gridPosition = new Vector2Int(-1, -1);
        [SerializeField]
        [ReadOnly]
        State state = State.Idle;

        Animator cacheAnimator = null;
        Action<float> everyFrame = null;
        float velocity = 0;

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

        public SpriteRenderer Graphic
        {
            get
            {
                return graphic;
            }
        }

        public BlockGrid Grid
        {
            get;
            set;
        }

        public State CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    state = value;
                    CacheAnimator.SetInteger(stateFieldName, (int)state);

                    AfterDeactivate(null);
                    if (state == State.Fall)
                    {
                        velocity = 0;
                        everyFrame = new Action<float>(AnimateFalling);
                        Singleton.Instance.OnUpdate += everyFrame;
                    }
                }
            }
        }

        Animator CacheAnimator
        {
            get
            {
                if (cacheAnimator == null)
                {
                    cacheAnimator = GetComponent<Animator>();
                }
                return cacheAnimator;
            }
        }
        #endregion

        public void MarkHidden()
        {
            CurrentState = State.Hidden;
        }

        public void PlayPlacedAnimation()
        {
            CurrentState = State.Idle;
            CacheAnimator.SetTrigger(placedTriggerName);
        }

        public void ResetPosition()
        {
            gridPosition.x = -1;
            gridPosition.y = -1;
            MarkHidden();
        }

        public override void AfterDeactivate(OmiyaGames.Global.PoolingManager pool)
        {
            base.AfterDeactivate(pool);
            if (everyFrame != null)
            {
                Singleton.Instance.OnUpdate -= everyFrame;
                everyFrame = null;
            }
        }

        private void AnimateFalling(float deltaTime)
        {
            if (Grid.IsValidGridPosition(GridPosition) == true)
            {
                // Get the target position
                Vector3 targetPosition = Grid.ConvertGridToWorldPosition(GridPosition);
                Vector3 currentPosition = transform.position;

                // Adjust velocity
                velocity -= (gravity * deltaTime);

                // Calculate position
                currentPosition.y += velocity;

                // Check the position
                if (currentPosition.y > targetPosition.y)
                {
                    // Set the position to current spot
                    transform.position = currentPosition;
                }
                else
                {
                    // If the object falls below the target position, snap to it
                    CurrentState = State.Idle;
                    transform.position = targetPosition;
                }
            }
            else
            {
                // If the object is at an invalid location, set to idle
                CurrentState = State.Idle;
            }
        }
    }
}
