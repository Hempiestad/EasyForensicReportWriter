using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using EasyForensicReportWriter.Controls;
using EasyForensicReportWriter.Models;
using EasyForensicReportWriter.Services;

namespace EasyForensicReportWriter.Services
{
    public static class ScreenshotService
    {
        public static EvidenceItem? CaptureRegion()
        {
            var overlay = new CaptureOverlayWindow();
            if (overlay.ShowDialog() == true && overlay.CapturedBitmap != null)
            {
                using var ms = new MemoryStream();
                overlay.CapturedBitmap.Save(ms, ImageFormat.Png);
                var bytes = ms.ToArray();

                var (md5, sha256) = HashService.ComputeHashes(bytes);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(bytes);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return new EvidenceItem
                {
                    Image = bitmapImage,
                    RawBytes = bytes,
                    Md5 = md5,
                    Sha256 = sha256,
                    Name = $"Evidence_{DateTime.UtcNow:yyyyMMdd_HHmmss}"
                };
            }
            return null;
        }
    }
}