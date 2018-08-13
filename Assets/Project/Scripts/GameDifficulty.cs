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
            public int scoreThreshold = 500;
            public int numberOfBlocksToDrop = 4;
            public int NumberOfBlockTypes = 4;
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

        public Difficulty GetDifficulty(int score)
        {
            int scoreSum = 0;
            Difficulty difficulty = null;
            foreach(Difficulty nextDifficulty in allDifficulties)
            {
                scoreSum = nextDifficulty.scoreThreshold;
                difficulty = nextDifficulty;
                if(score < scoreSum)
                {
                    break;
                }
            }
            return difficulty;
        }
    }
}
