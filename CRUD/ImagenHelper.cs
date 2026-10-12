using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ProyectoProductos
{
    public static class ImagenHelper
    {
        // Convierte una imagen en un arreglo de bytes (PNG) para guardarla en MySQL
        public static byte[] ImageToByteArray(Image imagen)
        {
            if (imagen == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                imagen.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        // Convierte los bytes de MySQL de vuelta a Image.
        // Si se pasan ancho y alto, devuelve una miniatura de ese tamaño.
        public static Image ByteArrayToImage(byte[] bytes, int ancho = 0, int alto = 0)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            using (MemoryStream ms = new MemoryStream(bytes))
            using (Bitmap original = new Bitmap(ms))
            {
                if (ancho > 0 && alto > 0)
                    return new Bitmap(original, new Size(ancho, alto));

                // Clona para poder liberar el MemoryStream
                return new Bitmap(original);
            }
        }
    }
}
