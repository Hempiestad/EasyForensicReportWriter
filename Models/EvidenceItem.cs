using System;
using System.Windows.Media.Imaging;

namespace EasyForensicReportWriter.Models
{
    public class EvidenceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Screenshot";
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
        public BitmapImage? Image { get; set; }
        public byte[]? RawBytes { get; set; }
        public string Md5 { get; set; } = "";
        public string Sha256 { get; set; } = "";
        public string OcrText { get; set; } = "";
        public string FigureCaption => $"Figure {FigureNumber}: {Name} ({CapturedAt:yyyy-MM-dd HH:mm:ss} UTC)";
        public int FigureNumber { get; set; }
    }
}