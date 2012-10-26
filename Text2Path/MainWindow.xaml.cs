using System;
using System.Collections;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Text2Path
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Populate();
            SetDefaults();
        }

        private void Populate()
        {
            CultureComboBox.ItemsSource = Helper.GetAvailableCultures();
            DirectionComboBox.ItemsSource = Helper.GetDirections();
            FontComboBox.ItemsSource = Helper.GetFonts();
            FontSizeComboBox.ItemsSource = Helper.GetFontSizes();
        }

        private void SetDefaults()
        {
            TextTextBlock.Text = "Convert this text!";
            CultureComboBox.SelectedItem = "en-US";
            DirectionComboBox.SelectedItem = "Left to right";
            FontComboBox.SelectedItem = "Segoe UI";
            FontSizeComboBox.SelectedItem = "48";
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;

                var path = Helper.Text2Path(TextTextBlock.Text,
                                            CultureComboBox.Text,
                                            (DirectionComboBox.Text == "Left to right"),
                                            FontComboBox.Text,
                                            Int32.Parse(FontSizeComboBox.Text));

                OutputTextBox.Text = string.Format("<Path Stroke={0}Black{0} Fill={0}Black{0} Data={0}{1}{0} />", '"', path);
                PathTextBlock.Visibility = Visibility.Visible;
                CopyButton.Visibility = Visibility.Visible;

                OutputPath.Data = Geometry.Parse(path);
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = ex.Message;
            }
            finally
            {
                OutputTextBox.Visibility = Visibility.Visible;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://stevenhollidge.blogspot.com");
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, OutputTextBox.Text);
                ClipboardResponse.Text = "Success";
            }
            catch
            {
                ClipboardResponse.Text = "Failed";
            }
        }
    }

    public class Helper
    {
        public static string Text2Path(String text, string culture, bool leftToRight, string font, int fontSize)
        {
            if (culture == "") culture = "en-us";

            var ci = new CultureInfo(culture);
            var fd = leftToRight ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
            var ff = new FontFamily(font);
            var tf = new Typeface(ff, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var t = new FormattedText(text, ci, fd, tf, fontSize, Brushes.Black);
            var g = t.BuildGeometry(new Point(0, 0));
            var p = g.GetFlattenedPathGeometry();

            return p.ToString();
        }

        public static IEnumerable GetAvailableCultures()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures)
                              .Select(x => x.Name)
                              .ToList();
        }

        public static IEnumerable GetDirections()
        {
            return new [] {"Left to right", "Right to left" };
        }

        public static IEnumerable GetFonts()
        {
            return new InstalledFontCollection().Families.Select(font => font.Name).ToList();
        }

        public static IEnumerable GetFontSizes()
        {
            return new [] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 26, 28, 36, 48, 72 }.Select(x => x.ToString());
        }
    }
}
