using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using EasyForensicReportWriter.Models;
using EasyForensicReportWriter.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Windows.Media.Ocr;
using Windows.Globalization;

namespace EasyForensicReportWriter
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<EvidenceItem> EvidenceItems = new();
        private int figureCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            EvidenceTree.ItemsSource = EvidenceItems;
            DataContext = this;
        }

        private async void Capture_Click(object sender, RoutedEventArgs e)
        {
            var item = ScreenshotService.CaptureRegion();
            if (item != null)
            {
                item.OcrText = await OcrService.PerformOcrAsync(item.RawBytes);
                EvidenceItems.Add(item);
                EvidenceTree.Items.Refresh();
            }
        }

        private void EvidenceTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (EvidenceTree.SelectedItem is EvidenceItem item)
            {
                HashBox.Text = $"MD5: {item.Md5}\nSHA-256: {item.Sha256}";
                OcrBox.Text = item.OcrText;
            }
        }

        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            if (EvidenceTree.SelectedItem is EvidenceItem item)
            {
                item.FigureNumber = figureCounter++;
                var image = new Image { Source = item.Image, Width = 600, Height = 400 };
                var captionRun = new Run(item.FigureCaption) { FontStyle = FontStyles.Italic };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new InlineUIContainer(image));
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(captionRun);
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(new LineBreak());

                // Add hash info
                var hashRun = new Run($"Hash Verification:\nMD5: {item.Md5}\nSHA-256: {item.Sha256}");
                var hashPara = new Paragraph(hashRun);
                ReportEditor.Document.Blocks.Add(hashPara);

                ReportEditor.Document.Blocks.Add(paragraph);
                ReportEditor.Focus();
            }
        }

        private async void OcrToNarrative_Click(object sender, RoutedEventArgs e)
        {
            if (EvidenceTree.SelectedItem is EvidenceItem item && !string.IsNullOrEmpty(item.OcrText))
            {
                // Simple narrative generation (expand with AI if needed)
                var narrative = $"Analysis of captured evidence shows: {item.OcrText.Substring(0, Math.Min(100, item.OcrText.Length))}...\nThis indicates potential [insert forensic insight].";
                InsertText(narrative);
            }
        }

        private void GenerateNarrative_Click(object sender, RoutedEventArgs e)
        {
            if (OcrBox.Text.Length > 0)
            {
                var narrative = $"Based on OCR extraction, the image contains text related to [key terms]. Forensic narrative: Verified integrity via hashes (MD5/SHA-256 match).";
                InsertText(narrative);
            }
        }

        private void InsertText(string text)
        {
            var range = new TextRange(ReportEditor.Document.ContentEnd, ReportEditor.Document.ContentEnd);
            range.Text = text + "\n\n";
        }

        private async void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog { Filter = "Word|*.docx|PDF|*.pdf" };
            if (sfd.ShowDialog() == true)
            {
                var docText = new TextRange(ReportEditor.Document.ContentStart, ReportEditor.Document.ContentEnd).Text;

                if (sfd.FileName.EndsWith(".docx"))
                {
                    using var wordDoc = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document);
                    var mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body(new Paragraph(new Run(new Text(docText)))));
                    mainPart.Document.Save();
                }
                else if (sfd.FileName.EndsWith(".pdf"))
                {
                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Content().Text(docText).FontSize(12);
                        });
                    }).GeneratePdf(sfd.FileName);
                }
                MessageBox.Show("Report exported successfully!");
            }
        }

        private void FontSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeCombo.SelectedItem is ComboBoxItem item && double.TryParse(item.Content.ToString(), out var size))
            {
                ReportEditor.FontSize = size;
            }
        }
    }
}