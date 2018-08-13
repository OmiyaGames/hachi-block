using UnityEngine;
using Community.UI;
using OmiyaGames;
using System;
using OmiyaGames.Global;

namespace Project
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class Block : IPooledObject
    {
        public const uint IdNull = 0;

        public enum BlockType
        {
            Invalid = -1,
            Hitotsu,
            Futatsu,
            Mittsu,
            Yottsu,
            Itutsu,
            Muttsu,
            Nanatsu,
            Yattsu
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
        SpriteRenderer symbol;
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

        static uint nextId = (IdNull + 1);

        float velocity = 0;
        Animator cacheAnimator = null;
        Action<float> everyFrame = null;

        #region Properties
        public static uint NextId
        {
            get
            {
                uint returnId = nextId;
                if(nextId < uint.MaxValue)
                {
                    ++nextId;
                }
                else
                {
                    nextId = (IdNull + 1);
                }
                return returnId;
            }
        }

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

                    UnbindFromSingleton();
                    if (state == State.Fall)
                    {
                        velocity = 0;
                        everyFrame = new Action<float>(AnimateFalling);
                        Singleton.Instance.OnUpdate += everyFrame;
                    }
                }
            }
        }

        public uint Id
        {
            get;
            private set;
        } = IdNull;

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

        public SpriteRenderer Symbol
        {
            get
            {
                return symbol;
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

        public override void Initialized(PoolingManager manager)
        {
            base.Initialized(manager);
            Id = NextId;
        }

        public override void Activated(PoolingManager manager)
        {
            base.Activated(manager);
            Id = NextId;
        }

        public override void AfterDeactivate(PoolingManager pool)
        {
            base.AfterDeactivate(pool);
            Id = IdNull;
            ResetPosition();
            UnbindFromSingleton();
        }

        private void UnbindFromSingleton()
        {
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
