using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace EasyForensicReportWriter.Services
{
    public static class OcrService
    {
        public static async Task<string> PerformOcrAsync(byte[] imageBytes)
        {
            try
            {
                using var stream = new MemoryStream(imageBytes).AsRandomAccessStream();
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                var ocrEngine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en-US"));
                if (ocrEngine != null)
                {
                    var result = await ocrEngine.RecognizeAsync(softwareBitmap);
                    return result.Text;
                }
            }
            catch
            {
                // Fallback: "OCR unavailable"
            }
            return "OCR processing unavailable on this system. Ensure Windows 10/11 with English language pack.";
        }
    }
}