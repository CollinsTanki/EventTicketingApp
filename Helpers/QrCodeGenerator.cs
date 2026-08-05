// Helpers/QrCodeGenerator.cs
using QRCoder;

namespace EventTicketingApp.Helpers
{
    public static class QrCodeGenerator
    {
        /// <summary>
        /// Generates a PNG QR code (as a byte array) encoding the given ticket code.
        /// </summary>
        public static byte[] GenerateQrCode(Guid ticketCode)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(ticketCode.ToString(), QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrCodeData);
            return pngQrCode.GetGraphic(20); // 20 = pixels per module
        }

        /// <summary>
        /// Convenience overload — returns a base64 string ready for embedding
        /// directly in an  <img src="data:image/png;base64,..."> tag on the React side.
        /// </summary>
        public static string GenerateQrCodeBase64(Guid ticketCode)
        {
            var bytes = GenerateQrCode(ticketCode);
            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}