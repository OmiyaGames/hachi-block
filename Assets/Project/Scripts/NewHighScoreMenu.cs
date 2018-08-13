using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Menu;
using OmiyaGames.Settings;
using TMPro;

namespace Project
{
    public class NewHighScoreMenu : IMenu
    {
        [Header("Common Settings")]
        [SerializeField]
        BackgroundMenu.BackgroundType background = BackgroundMenu.BackgroundType.SolidColor;
        [SerializeField]
        MenuNavigator navigator;

        [Header("Buttons")]
        [SerializeField]
        Button defaultButton = null;
        [SerializeField]
        TMP_InputField nameField;

        [Header("Labels")]
        [SerializeField]
        TextMeshProUGUI scorePlacementLabel;
        [SerializeField]
        TextMeshProUGUI scoreLabel;

        IRecord<int> newScore = null;
        string originalScorePlacementText = null;
        string originalScoreText = null;

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
            scorePlacementLabel.gameObject.SetActive(newScore != null);
            scoreLabel.gameObject.SetActive(newScore != null);
            if (newScore != null)
            {
                UpdateLabel(scorePlacementLabel, (highScorePlacement + 1).ToString(), ref originalScorePlacementText);
                UpdateLabel(scoreLabel, newScore.Record.ToString(), ref originalScoreText);
            }
        }

        public void OnRestartClicked()
        {
            if (IsListeningToEvents == true)
            {
                // FIXME: Notify the player that they'll lose their unsaved progress.
                // Transition to the current level
                SceneChanger.ReloadCurrentScene();
                Hide();
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

                // Store this name for the next gameplay as well
                Settings.LastEnteredName = nameField.text;
            }

            // Call base method
            base.OnStateChanged(from, to);
        }

        private void UpdateLabel(TextMeshProUGUI label, string info, ref string originalString)
        {
            if(string.IsNullOrEmpty(originalString) == true)
            {
                originalString = label.text;
            }
            label.text = string.Format(originalString, info);
        }
    }
}