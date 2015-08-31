﻿using ChangesetViewer.Core.Settings;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ChangesetViewer.UI
{
    public class RichTextboxCustomized : RichTextBox
    {
        private TextTypes _formattexttype;
        private string _textToApply;
        private bool _textFormatted = false;

        #region FormattedText Dependency Property

        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register("FormattedText", typeof(string), typeof(RichTextboxCustomized),
            new PropertyMetadata(string.Empty, FormattedTextChangedCallback), FormattedTextValidateCallback);

        private static void FormattedTextChangedCallback(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as RichTextboxCustomized)._textToApply = e.NewValue as string;
            ApplyFormatting(obj as RichTextboxCustomized);
        }

        private static bool FormattedTextValidateCallback(object value)
        {
            return value != null;
        }

        public string FormattedText
        {
            get
            {
                return (string)GetValue(FormattedTextProperty);
            }
            set
            {
                SetValue(FormattedTextProperty, value);
            }
        }

        #endregion

        #region FormattedTextType Dependency Property

        public enum TextTypes
        {
            None,
            Comment,
            WorkItem
        }

        public static readonly DependencyProperty FormattedTextTypeProperty = DependencyProperty.Register("FormattedTextType", typeof(TextTypes), typeof(RichTextboxCustomized),
            new PropertyMetadata(TextTypes.None, FormattedTextTypeChangedCallback), FormattedTextTypeValidateCallback);

        private static void FormattedTextTypeChangedCallback(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as RichTextboxCustomized)._formattexttype = (TextTypes)e.NewValue;
            ApplyFormatting(obj as RichTextboxCustomized);
        }

        private static bool FormattedTextTypeValidateCallback(object value)
        {
            return value != null;
        }

        public TextTypes FormattedTextType
        {
            get
            {
                return (TextTypes)GetValue(FormattedTextTypeProperty);
            }
            set
            {
                SetValue(FormattedTextTypeProperty, value);
            }
        }

        #endregion

        private static void ApplyFormatting(RichTextboxCustomized obj)
        {
            if (obj._formattexttype == TextTypes.Comment && !string.IsNullOrEmpty(obj._textToApply) && !obj._textFormatted)
            {
                obj.Document = GetCustomDocument(obj._textToApply);
                obj._textFormatted = true;
                return;
            }

            if (obj._formattexttype == TextTypes.WorkItem && !string.IsNullOrEmpty(obj._textToApply) && !obj._textFormatted)
            {
                obj.Document = GenerateWorkItemsDocument(obj._textToApply);
                obj._textFormatted = true;
                return;
            }
        }

        private static FlowDocument GetCustomDocument(string text)
        {
            FlowDocument document = new FlowDocument();

            if (!SettingsStaticModelWrapper.FindJiraTicketsInComment ||
                    string.IsNullOrEmpty(SettingsStaticModelWrapper.JiraSearchRegexPattern) ||
                    string.IsNullOrEmpty(SettingsStaticModelWrapper.JiraTicketBrowseLink))
            {
                document.Blocks.Add(new Paragraph(new Run(text)));
            }
            else
            {
                Paragraph para = new Paragraph();
                para.Margin = new Thickness(0); // remove indent between paragraphs

                Match m;
                int closeIndex = 0;

                m = Regex.Match(text, SettingsStaticModelWrapper.JiraSearchRegexPattern);

                if (m.Success)
                {
                    while (m.Success)
                    {
                        para.Inlines.Add(text.Substring(closeIndex, closeIndex > 0 ? m.Groups[0].Index - closeIndex : m.Groups[0].Index));

                        Hyperlink link = new Hyperlink();
                        link.Foreground = System.Windows.Media.Brushes.Green;
                        link.FontWeight = FontWeights.Bold;
                        link.IsEnabled = true;
                        link.Inlines.Add(m.Groups[0].ToString());
                        link.NavigateUri = new Uri(SettingsStaticModelWrapper.JiraTicketBrowseLink + m.Groups[0].ToString());
                        link.RequestNavigate += (sender, args) => Process.Start(args.Uri.ToString());
                        para.Inlines.Add(link);

                        closeIndex = m.Groups[0].Index + m.Groups[0].ToString().Length;

                        m = m.NextMatch();
                    }
                    if (closeIndex != text.Length)
                    {
                        para.Inlines.Add(text.Substring(closeIndex, text.Length - closeIndex));
                    }
                }
                else
                {
                    para.Inlines.Add(text);
                }
                document.Blocks.Add(para);
            }
            return document;
        }

        private static FlowDocument GenerateWorkItemsDocument(string text)
        {
            FlowDocument document = new FlowDocument();
            
            document.Blocks.Add(new Paragraph(new Run(text)));

            return document;

        }
    }
}
