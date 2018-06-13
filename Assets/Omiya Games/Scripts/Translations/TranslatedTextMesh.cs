﻿using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioFinder.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>6/1/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Set translation text.
    /// </summary>
    /// <seealso cref="TranslatedTextMeshPro"/>
    [RequireComponent(typeof(TextMesh))]
    [System.Obsolete("Obsolete in favor of TranslatedTextMeshPro")]
    public class TranslatedTextMesh : MonoBehaviour
    {
        static readonly HashSet<TranslatedTextMesh> allTranslationScripts = new HashSet<TranslatedTextMesh>();

        public static IEnumerable<TranslatedTextMesh> AllTranslationScripts
        {
            get
            {
                return allTranslationScripts;
            }
        }

        private static TranslationManager Parser
        {
            get
            {
                return Singleton.Get<TranslationManager>();
            }
        }

        /// <summary>
        /// The key to the CSVLanguageParser.
        /// </summary>
        [SerializeField]
        string translationKey = "";

        /// <summary>
        /// The attached label.
        /// </summary>
        TextMesh label = null;
        object[] formatArgs = null;

        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(translationKey) == false);
            }
        }

        /// <summary>
        /// Gets the <c>Text</c> component.
        /// </summary>
        /// <value>The label.</value>
        public TextMesh Label
        {
            get
            {
                if (label == null)
                {
                    // Grab the label component
                    label = GetComponent<TextMesh>();
                }
                return label;
            }
        }

        /// <summary>
        /// Gets or sets the translation key.
        /// </summary>
        /// <value>The translation key.</value>
        public string TranslationKey
        {
            get
            {
                return translationKey;
            }
            set
            {
                translationKey = value;
                if (IsTranslating == true)
                {
                    // Add this script to the dictionary
                    if (allTranslationScripts.Contains(this) == false)
                    {
                        allTranslationScripts.Add(this);
                    }

                    // Update the label
                    UpdateLabel();
                }
                else if (allTranslationScripts.Contains(this) == true)
                {
                    // Remove this script from the dictionary
                    allTranslationScripts.Remove(this);
                }
            }
        }

        void Start()
        {
            if (IsTranslating == true)
            {
                // Add this script to the dictionary
                allTranslationScripts.Add(this);

                // Update the label
                UpdateLabel();
            }
        }

        void OnDestroy()
        {
            if (IsTranslating == true)
            {
                // Remove this script from the dictionary
                allTranslationScripts.Remove(this);
            }
        }

        public void UpdateLabel()
        {
            // Check if there's a CSV parser
            if ((Parser != null) && (Parser.ContainsKey(TranslationKey) == true))
            {
                // check if there's any formatting involved
                if ((formatArgs != null) && (formatArgs.Length > 0))
                {
                    // Set the label to the text directly
                    Label.text = string.Format(Parser[TranslationKey], formatArgs);
                }
                else
                {
                    // Set the label to the text directly
                    Label.text = Parser[TranslationKey];
                }
            }
        }

        public void SetLabelFormat(params object[] args)
        {
            // Update the member variable
            formatArgs = args;

            // Update the label
            UpdateLabel();
        }
    }
}
