using UnityEngine;
using UnityEngine.UI;
using OmiyaGames;
using OmiyaGames.Menu;
using OmiyaGames.Settings;
using System;

namespace Project
{
    public class NewHighScoreMenu : IMenu
    {
        [Header("Common Settings")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("showBackground")]
        protected BackgroundMenu.BackgroundType background = BackgroundMenu.BackgroundType.SolidColor;
        [SerializeField]
        MenuNavigator navigator;

        [Header("Buttons")]
        [SerializeField]
        protected Button defaultButton = null;
        [SerializeField]
        InputField nameField;

        [Header("Labels")]
        [SerializeField]
        Text scorePlacementLabel;
        [SerializeField]
        Text scoreLabel;

        IRecord<int> newScore = null;
        string originalScorePlacementText = null;
        string originalScoreText = null;
        IRecord<float> newTime = null;
        string originalTimePlacementText = null;
        string originalTimeText = null;

        #region Non-abstract Properties
        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                Selectable returnObject = null;
                if (defaultButton != null)
                {
                    returnObject = defaultButton;
                }
                return returnObject;
            }
        }

        public override MenuNavigator Navigator
        {
            get
            {
                return navigator;
            }
        }

        public string EnteredName
        {
            get
            {
                return nameField.text;
            }
        }
        #endregion

        public void Setup(int highScorePlacement, IRecord<int> newScore)
        {
            Setup(highScorePlacement, newScore, -1, null);
        }

        public void Setup(int timePlacement, IRecord<float> newTime)
        {
            Setup(-1, null, timePlacement, newTime);
        }

        public void Setup(int highScorePlacement, IRecord<int> checkScore, int timePlacement, IRecord<float> checkTime)
        {
            newScore = null;
            if((highScorePlacement >= 0) && (checkScore != null))
            {
                newScore = checkScore;
            }
            newTime = null;
            if ((timePlacement >= 0) && (checkTime != null))
            {
                newTime = checkTime;
            }

            scorePlacementLabel.gameObject.SetActive(newScore != null);
            scoreLabel.gameObject.SetActive(newScore != null);
            if (newScore != null)
            {
                UpdateLabel(scorePlacementLabel, (highScorePlacement + 1).ToString(), ref originalScorePlacementText);
                UpdateLabel(scoreLabel, checkScore.Record.ToString(), ref originalScoreText);
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Check if this menu is going from hidden to visible
            if (to == VisibilityState.Visible)
            {
                // Use the last stored name
                nameField.text = Settings.LastEnteredName;
            }
            else if (to == VisibilityState.Hidden)
            {
                // Record the new name!
                if (newScore != null)
                {
                    newScore.Name = nameField.text;
                }
                if (newTime != null)
                {
                    newTime.Name = nameField.text;
                }

                // Store this name for the next gameplay as well
                Settings.LastEnteredName = nameField.text;
            }

            // Call base method
            base.OnStateChanged(from, to);
        }

        private void UpdateLabel(Text label, string info, ref string originalString)
        {
            if(string.IsNullOrEmpty(originalString) == true)
            {
                originalString = label.text;
            }
            label.text = string.Format(originalString, info);
        }
    }
}