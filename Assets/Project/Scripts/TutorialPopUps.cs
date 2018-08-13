using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Menu;

namespace Project
{
    public class TutorialPopUps : MonoBehaviour
    {
        [System.Serializable]
        public class Stage
        {
            [SerializeField]
            [TextArea]
            string message;
            [SerializeField]
            bool useScanner;
            [SerializeField]
            int numMoves;

            public string GetMessage(BlockGridScanner scanner)
            {
                if(useScanner == false)
                {
                    return message;
                }
                else
                {
                    return string.Format(message, scanner.BlocksInARow);
                }
            }

            public int NumMoves
            {
                get
                {
                    return numMoves;
                }
            }
        }

        [SerializeField]
        float waitFor = 0.5f;
        [SerializeField]
        Stage[] stages;
        [SerializeField]
        BlockGridScanner scanner;

        int currentStage = 0;
        int nextShowStage = 0;
        ulong stageId = 0;

        PopUpManager PopUps
        {
            get
            {
                return Singleton.Get<MenuManager>().PopUps;
            }
        }

        Stage CurrentStage
        {
            get
            {
                if (currentStage < stages.Length)
                {
                    return stages[currentStage];
                }
                else
                {
                    return null;
                }
            }
        }

        // Use this for initialization
        IEnumerator Start()
        {
            // Bind to moves
            scanner.OnMove += Scanner_OnMove;

            // Wait for a few seconds
            yield return new WaitForSeconds(waitFor);

            // Show the first stage
            if(currentStage == 0)
            {
                nextShowStage = CurrentStage.NumMoves;
                stageId = PopUps.ShowNewDialog(CurrentStage.GetMessage(scanner));
            }
        }

        private void Scanner_OnMove(BlockGridScanner obj)
        {
            // Check if the number of moves exceed the stage
            if ((CurrentStage != null) && (obj.NumMoves >= nextShowStage))
            {
                // Hide the previous message
                PopUps.RemoveDialog(stageId);

                // Increment stage
                ++currentStage;

                // Check if there is a next stage
                if (CurrentStage != null)
                {
                    // Show the next message
                    nextShowStage += CurrentStage.NumMoves;
                    stageId = PopUps.ShowNewDialog(CurrentStage.GetMessage(scanner));
                }
            }
        }
    }
}