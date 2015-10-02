using ChangesetViewer.Core.Settings;
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
        private bool _textFormatted;

        #region FormattedText Dependency Property

        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register("FormattedText", typeof(string), typeof(RichTextboxCustomized),
            new PropertyMetadata(string.Empty, FormattedTextChangedCallback), FormattedTextValidateCallback);

        private static void FormattedTextChangedCallback(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextboxCustomized)obj)._textToApply = e.NewValue as string;
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
            ((RichTextboxCustomized)obj)._formattexttype = (TextTypes)e.NewValue;
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

        //#region ActionToInvoke Dependency Property

        //public static readonly DependencyProperty ActionToInvokeProperty = DependencyProperty.Register("ActionToInvoke", typeof(Action), typeof(RichTextboxCustomized),
        //    new PropertyMetadata(null, ActionToInvokeChangedCallback), ActionToInvokeValidateCallback);

        //private static void ActionToInvokeChangedCallback(
        //    DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //{
        //    (obj as RichTextboxCustomized)._actionToInvoke = (Action)e.NewValue;
        //    ApplyFormatting(obj as RichTextboxCustomized);
        //}

        //private static bool ActionToInvokeValidateCallback(object value)
        //{
        //    return value != null;
        //}

        //public Action ActionToInvoke
        //{
        //    get
        //    {
        //        return (Action)GetValue(ActionToInvokeProperty);
        //    }
        //    set
        //    {
        //        SetValue(ActionToInvokeProperty, value);
        //    }
        //}

        //#endregion


        private static void ApplyFormatting(RichTextboxCustomized obj)
        {
            if (obj._formattexttype == TextTypes.Comment && !string.IsNullOrEmpty(obj._textToApply) && !obj._textFormatted)
            {
                obj.Document = GetCustomDocument(obj._textToApply);
                obj._textFormatted = true;
                return;
            }

            if (obj._formattexttype != TextTypes.WorkItem || string.IsNullOrEmpty(obj._textToApply) ||
                obj._textFormatted) return;
            obj.Document = GenerateWorkItemsDocument(obj._textToApply);
            obj._textFormatted = true;
        }

        private static FlowDocument GetCustomDocument(string text)
        {
            var document = new FlowDocument();

            if (!SettingsStaticModelWrapper.FindJiraTicketsInComment ||
                    string.IsNullOrEmpty(SettingsStaticModelWrapper.JiraSearchRegexPattern) ||
                    string.IsNullOrEmpty(SettingsStaticModelWrapper.JiraTicketBrowseLink))
            {
                document.Blocks.Add(new Paragraph(new Run(text)));
            }
            else
            {
                var para = new Paragraph { Margin = new Thickness(0) };

                var closeIndex = 0;

                var m = Regex.Match(text, SettingsStaticModelWrapper.JiraSearchRegexPattern);

                if (m.Success)
                {
                    while (m.Success)
                    {
                        para.Inlines.Add(text.Substring(closeIndex, closeIndex > 0 ? m.Groups[0].Index - closeIndex : m.Groups[0].Index));

                        var link = new Hyperlink
                        {
                            Foreground = System.Windows.Media.Brushes.SkyBlue,
                            FontWeight = FontWeights.Bold,
                            IsEnabled = true
                        };
                        link.Inlines.Add(m.Groups[0].ToString());
                        link.NavigateUri = new Uri(SettingsStaticModelWrapper.JiraTicketBrowseLink + m.Groups[0]);
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
            var document = new FlowDocument();

            if (string.IsNullOrEmpty(text)) return document;
            var para = new Paragraph { Margin = new Thickness(0) };

            foreach (var workitem in text.Split(",".ToCharArray()))
            {
                var link = new Hyperlink
                {
                    Foreground = System.Windows.Media.Brushes.SkyBlue,
                    FontWeight = FontWeights.Bold,
                    IsEnabled = true
                };
                link.Inlines.Add(workitem);
                link.NavigateUri = new Uri(SettingsStaticModelWrapper.JiraTicketBrowseLink);
                link.RequestNavigate += (sender, args) => Process.Start(args.Uri.ToString());

                para.Inlines.Add(link);
            }
            document.Blocks.Add(para);

            return document;

        }
    }
}
