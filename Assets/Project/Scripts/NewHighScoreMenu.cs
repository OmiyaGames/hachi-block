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

        [Header("Labels")]
        [SerializeField]
        TMP_InputField nameField;
        [SerializeField]
        TextMeshProUGUI scorePlacementLabel;
        [SerializeField]
        TextMeshProUGUI scoreLabel;

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
                return nameField;
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

        public IRecord<int> NewScore
        {
            get;
            set;
        } = null;
        #endregion

        public void Setup(int highScorePlacement, IRecord<int> score)
        {
            NewScore = score;
            scorePlacementLabel.gameObject.SetActive(NewScore != null);
            scoreLabel.gameObject.SetActive(NewScore != null);
            if (NewScore != null)
            {
                UpdateLabel(scorePlacementLabel, (highScorePlacement + 1).ToString(), ref originalScorePlacementText);
                UpdateLabel(scoreLabel, NewScore.Record.ToString(), ref originalScoreText);
            }
        }

        public void OnRestartClicked()
        {
            if (IsListeningToEvents == true)
            {
                RecordNewHighScore();

                // Transition to the current level
                SceneChanger.ReloadCurrentScene();
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
                RecordNewHighScore();
            }

            // Call base method
            base.OnStateChanged(from, to);
        }

        private void RecordNewHighScore()
        {
            // Record the new name!
            if (NewScore != null)
            {
                NewScore.Name = nameField.text;
            }

            // Store this name for the next gameplay as well
            Settings.LastEnteredName = nameField.text;
            Settings.SaveSettings();
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