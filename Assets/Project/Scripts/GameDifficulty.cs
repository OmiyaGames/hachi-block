using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class GameDifficulty : MonoBehaviour
    {
        [System.Serializable]
        public class Difficulty
        {
            //public
        }

        [SerializeField]
        [Range(3, 5)]
        int blocksInARow = 3;
        [SerializeField]
        Difficulty[] allDifficulties;

        public int BlocksInARow
        {
            get
            {
                return blocksInARow;
            }
        }
    }
}
