using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Community.UI;
using OmiyaGames;
using System;

namespace Project
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Block : IPooledObject
    {
        public const float DetectFallDelay = 0.5f;

        public enum BlockType
        {
            T1,
            T2,
            T3,
            T4
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
        Rigidbody2D cacheBody = null;
        Action<float> everyFrame = null;
        float fallStart = 0;

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

                    OnDestroy(null);
                    if (state == State.Fall)
                    {
                        CacheBody.bodyType = RigidbodyType2D.Dynamic;
                        fallStart = Time.time;
                        everyFrame = new Action<float>(Instance_OnUpdate);
                        Singleton.Instance.OnUpdate += everyFrame;
                    }
                    else
                    {
                        CacheBody.bodyType = RigidbodyType2D.Static;
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

        Rigidbody2D CacheBody
        {
            get
            {
                if (cacheBody == null)
                {
                    cacheBody = GetComponent<Rigidbody2D>();
                }
                return cacheBody;
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

        public override void OnDestroy(OmiyaGames.Global.PoolingManager pool)
        {
            base.OnDestroy(pool);
            if (everyFrame != null)
            {
                Singleton.Instance.OnUpdate -= everyFrame;
                everyFrame = null;
            }
        }

        private void Instance_OnUpdate(float obj)
        {
            // Check the velocity
            if (((Time.time - fallStart) > DetectFallDelay) && (CacheBody.velocity.y > 0.001f))
            {
                // If the object is not falling anymore, switch back to idle
                CurrentState = State.Idle;

                // Snap the block's position
                if(Grid.IsValidGridPosition(GridPosition) == true)
                {
                    transform.position = Grid.ConvertGridToWorldPosition(GridPosition);
                }
            }
        }
    }
}
