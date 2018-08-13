using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class DiscoveredFormations
    {
        public List<Block[]> RowFormations
        {
            get;
        } = new List<Block[]>(4);

        public List<Block[]> ColumnFormations
        {
            get;
        } = new List<Block[]>(4);

        public List<Block[]> AllFormations
        {
            get;
        } = new List<Block[]>(8);

        public List<Block> AllClearedBlocks
        {
            get;
        } = new List<Block>(24);

        public void AddFormation(Block[] formation, bool isRow)
        {
            // Check which list to add the formation to
            if (isRow == true)
            {
                RowFormations.Add(formation);
            }
            else
            {
                ColumnFormations.Add(formation);
            }

            // Add formation to all formations
            AllFormations.Add(formation);

            // Add each block in formation to all cleared blocks
            foreach (Block block in formation)
            {
                AllClearedBlocks.Add(block);
            }
        }

        public bool IsFormationFound
        {
            get
            {
                return ((RowFormations.Count + ColumnFormations.Count) > 0);
            }
        }

        public int NumBlocks
        {
            get
            {
                int returnNum = 0;
                foreach (Block[] formation in RowFormations)
                {
                    returnNum = formation.Length;
                }
                foreach (Block[] formation in ColumnFormations)
                {
                    returnNum = formation.Length;
                }
                return returnNum;
            }
        }

        public void Reset()
        {
            RowFormations.Clear();
            ColumnFormations.Clear();
            AllFormations.Clear();
            AllClearedBlocks.Clear();
        }

        public void SortLists()
        {
            AllFormations.Sort(SortFormations);
            AllClearedBlocks.Sort(SortBlocks);
        }

        private int SortBlocks(Block left, Block right)
        {
            // Get distance of each block from the bottom-left hand corner
            int leftDistance = (left.GridPosition.x * left.GridPosition.x) + (left.GridPosition.y * left.GridPosition.y);
            int rightDistance = (right.GridPosition.x * right.GridPosition.x) + (right.GridPosition.y * right.GridPosition.y);

            // Check if the distances are not equal
            if(leftDistance != rightDistance)
            {
                // If so, return the difference
                return leftDistance - rightDistance;
            }
            else
            {
                // If not, sort by x coordinate
                return left.GridPosition.x - right.GridPosition.x;
            }
        }

        private int SortFormations(Block[] left, Block[] right)
        {
            return left.Length - right.Length;
        }
    }
}
