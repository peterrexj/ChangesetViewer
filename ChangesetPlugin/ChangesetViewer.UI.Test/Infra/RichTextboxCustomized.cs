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

        #region FormattedText Dependency Property

        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register("FormattedText", typeof(string), typeof(RichTextboxCustomized),
            new PropertyMetadata(string.Empty, FormattedTextChangedCallback), FormattedTextValidateCallback);

        private static void FormattedTextChangedCallback(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as RichTextboxCustomized).Document = GetCustomDocument(e.NewValue as string);
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

        static string patternToMatch = @"[a-zA-Z]+[-]\d+";

        private static FlowDocument GetCustomDocument(string text)
        {
            FlowDocument document = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Margin = new Thickness(0); // remove indent between paragraphs

            Match m;
            int closeIndex = 0;

            m = Regex.Match(text, patternToMatch);

            if (m.Success)
            {
                while (m.Success)
                {
                    para.Inlines.Add(text.Substring(closeIndex, closeIndex > 0 ? m.Groups[0].Index - closeIndex : m.Groups[0].Index));

                    Hyperlink link = new Hyperlink();
                    link.IsEnabled = true;
                    link.Inlines.Add(m.Groups[0].ToString());
                    link.NavigateUri = new Uri("https://janisoncls.atlassian.net/browse/" + m.Groups[0].ToString());
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
           
            return document;
        }
    }
}
