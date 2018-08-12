using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Community.UI;
using OmiyaGames;

namespace Project
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class Block : IPooledObject
    {
        public enum BlockType
        {
            T1,
            T2,
            T3,
            T4
        }

        public enum State
        {
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

        [Header("Debugging Info")]
        [SerializeField]
        [ReadOnly]
        Vector2Int gridPosition = new Vector2Int(-1, -1);
        [SerializeField]
        [ReadOnly]
        State state = State.Idle;

        Animator cacheAnimator = null;

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
                if(state != value)
                {
                    state = value;
                    CacheAnimator.SetInteger(stateFieldName, (int)state);
                }
            }
        }

        public Animator CacheAnimator
        {
            get
            {
                if(cacheAnimator == null)
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
    }
}
