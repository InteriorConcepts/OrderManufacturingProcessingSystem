using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OMPS
{
    public static class ColorPaletteHandler
    {
        public static class Palettes
        {
            public static readonly uint[] akc12_bytes = [0xFF201127, 0xFF201433, 0xFF1b1e34, 0xFF355d68, 0xFF6aaf9d, 0xFF94c5ac, 0xFFffeb99, 0xFFffc27a, 0xFFec9a6d, 0xFFd9626b, 0xFFc24b6e, 0xFFa73169];
            public static readonly Color[] akc12 = [.. akc12_bytes.Select(HexToColor)];

            public static readonly uint[] scrj_bytes = [0xFF0f101a, 0xFF363c57, 0xFF7786ac, 0xFFb9cae7, 0xFFedf3ff, 0xFF3a1537, 0xFF842350, 0xFFd0455a, 0xFFe99994, 0xFFedd3d1, 0xFF4d2b24, 0xFF8e4b27, 0xFFcb8a3c, 0xFFe2c35d, 0xFFede29e, 0xFF1a444c, 0xFF1d8353, 0xFF34c33e, 0xFF97e672, 0xFFc5ebc1, 0xFF17335d, 0xFF205b8f, 0xFF2c98bb, 0xFF39d1da, 0xFF8febea, 0xFF181d4d, 0xFF1f2e8b, 0xFF3264cd, 0xFF65a5ef, 0xFFa1d0f7, 0xFF371860, 0xFF6a2ac0, 0xFFaa64eb, 0xFFe2a5f7, 0xFF8b2aae, 0xFFd95dea];
            public static readonly Color[] scrj = [.. scrj_bytes.Select(HexToColor)];

            public static readonly uint[] win3_bytes = [0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF];
            public static readonly Color[] win3 = [.. win3_bytes.Select(HexToColor)];

            public static readonly uint[] imbPc_bytes = [0x000000, 0x0000AA, 0x00AA00, 0x00AAAA, 0xAA0000, 0xAA00AA, 0xAA5500, 0xAAAAAA, 0x555555, 0x5555FF, 0x55FF55, 0x55FFFF, 0xFF5555, 0xFF55FF, 0xFFFF55, 0xFFFFFF];
            public static readonly Color[] ibmPc = [.. imbPc_bytes.Select(HexToColor)];

            public static readonly uint[] rct2_bytes = [0xFF172323, 0xFF233333, 0xFF2f4343, 0xFF3f5353, 0xFF4b6363, 0xFF5b7373, 0xFF6f8383, 0xFF839797, 0xFF9fafaf, 0xFFb7c3c3, 0xFFd3dbdb, 0xFFeff3f3, 0xFF332f00, 0xFF3f3b00, 0xFF4f4b0b, 0xFF5b5b13, 0xFF6b6b1f, 0xFF777b2f, 0xFF878b3b, 0xFF979b4f, 0xFFa7af5f, 0xFFbbbf73, 0xFFcbcf8b, 0xFFdfe3a3, 0xFF432b07, 0xFF573b0b, 0xFF6f4b17, 0xFF7f571f, 0xFF8f6327, 0xFF9f7333, 0xFFb38343, 0xFFbf9757, 0xFFcbaf6f, 0xFFdbc787, 0xFFe7dba3, 0xFFf7efc3, 0xFF471b00, 0xFF5f2b00, 0xFF773f00, 0xFF8f5307, 0xFFa76f07, 0xFFbf8b0f, 0xFFd7a713, 0xFFf3cb1b, 0xFFffe72f, 0xFFfff35f, 0xFFfffb8f, 0xFFffffc3, 0xFF230000, 0xFF4f0000, 0xFF5f0707, 0xFF6f0f0f, 0xFF7f1b1b, 0xFF8f2727, 0xFFa33b3b, 0xFFb34f4f, 0xFFc76767, 0xFFd77f7f, 0xFFeb9f9f, 0xFFffbfbf, 0xFF1b3313, 0xFF233f17, 0xFF2f4f1f, 0xFF3b5f27, 0xFF476f2b, 0xFF577f33, 0xFF638f3b, 0xFF739b43, 0xFF83ab4b, 0xFF93bb53, 0xFFa3cb5f, 0xFFb7db67, 0xFF1f371b, 0xFF2f4723, 0xFF3b532b, 0xFF4b6337, 0xFF5b6f43, 0xFF6f874f, 0xFF879f5f, 0xFF9fb76f, 0xFFb7cf7f, 0xFFc3db93, 0xFFcfe7a7, 0xFFdff7bf, 0xFF0f3f00, 0xFF135300, 0xFF176700, 0xFF1f7b00, 0xFF278f07, 0xFF379f17, 0xFF47af27, 0xFF5bbf3f, 0xFF6fcf57, 0xFF8bdf73, 0xFFa3ef8f, 0xFFc3ffb3, 0xFF4f2b13, 0xFF63371b, 0xFF77472b, 0xFF8b573b, 0xFFa76343, 0xFFbb7353, 0xFFcf8363, 0xFFd79773, 0xFFe3ab83, 0xFFefbf97, 0xFFf7cfab, 0xFFffe3c3, 0xFF0f1337, 0xFF272b57, 0xFF333767, 0xFF3f4377, 0xFF53538b, 0xFF63639b, 0xFF7777af, 0xFF8b8bbf, 0xFF9f9fcf, 0xFFb7b7df, 0xFFd3d3ef, 0xFFefefff, 0xFF001b6f, 0xFF002797, 0xFF0733a7, 0xFF0f43bb, 0xFF1b53cb, 0xFF2b67df, 0xFF4387e3, 0xFF5ba3e7, 0xFF77bbef, 0xFF8fd3f3, 0xFFafe7fb, 0xFFd7f7ff, 0xFF0b2b0f, 0xFF0f3717, 0xFF17471f, 0xFF23532b, 0xFF2f633b, 0xFF3b734b, 0xFF4f875f, 0xFF639b77, 0xFF7baf8b, 0xFF93c7a7, 0xFFafdbc3, 0xFFcff3df, 0xFF3f005f, 0xFF4b0773, 0xFF530f7f, 0xFF5f1f8f, 0xFF6b2b9b, 0xFF7b3fab, 0xFF8753bb, 0xFF9b67c7, 0xFFab7fd7, 0xFFbf9be7, 0xFFd7c3f3, 0xFFf3ebff, 0xFF3f0000, 0xFF570000, 0xFF730000, 0xFF8f0000, 0xFFab0000, 0xFFc70000, 0xFFe30700, 0xFFff0700, 0xFFff4f43, 0xFFff7b73, 0xFFffaba3, 0xFFffdbd7, 0xFF4f2700, 0xFF6f3300, 0xFF933f00, 0xFFb74700, 0xFFdb4f00, 0xFFff5300, 0xFFff6f17, 0xFFff8b33, 0xFFffa34f, 0xFFffb76b, 0xFFffcb87, 0xFFffdba3, 0xFF00332f, 0xFF003f37, 0xFF004b43, 0xFF00574f, 0xFF076b63, 0xFF177f77, 0xFF2b938f, 0xFF47a7a3, 0xFF63bbbb, 0xFF83cfcf, 0xFFabe7e7, 0xFFcfffff, 0xFF3f001b, 0xFF670033, 0xFF7b0b3f, 0xFF8f174f, 0xFFa31f5f, 0xFFb7276f, 0xFFdb3b8f, 0xFFef5bab, 0xFFf377bb, 0xFFf797cb, 0xFFfbb7df, 0xFFffd7ef, 0xFF271300, 0xFF371f07, 0xFF472f0f, 0xFF5b3f1f, 0xFF6b5333, 0xFF7b674b, 0xFF8f7f6b, 0xFFa3937f, 0xFFbbab93, 0xFFcfc3ab, 0xFFe7dbc3, 0xFFfff3df, 0xFF374b4b, 0xFF435b5b, 0xFF536b6b, 0xFF637b7b, 0xFFffb700, 0xFFffdb00, 0xFFffff00, 0xFF0f776f, 0xFF1b837b, 0xFF278f87, 0xFF379b97, 0xFF53b3af, 0xFF73cbcb, 0xFF9be3e3, 0xFFc7ffff, 0xFF00005f, 0xFF1b2b8b, 0xFF273b97, 0xFF00534b, 0xFF005b53, 0xFF005f57, 0xFF00635b, 0xFF07675f, 0xFF0f736b, 0xFF2f9793, 0xFF339b97, 0xFF53afaf, 0xFF57b3b3, 0xFF7bcbcb, 0xFF000000];
            public static readonly Color[] rct2 = [.. rct2_bytes.Select(HexToColor)];

            public static readonly uint[] win95_bytes = [0xFF000000, 0xFF800000, 0xFF008000, 0xFF808000, 0xFF000080, 0xFF800080, 0xFF008080, 0xFFc0c0c0, 0xFFc0dcc0, 0xFFa6caf0, 0xFF2a3faa, 0xFF2a3fff, 0xFF2a5f00, 0xFF2a5f55, 0xFF2a5faa, 0xFF2a5fff, 0xFF2a7f00, 0xFF2a7f55, 0xFF2a7faa, 0xFF2a7fff, 0xFF2a9f00, 0xFF2a9f55, 0xFF2a9faa, 0xFF2a9fff, 0xFF2abf00, 0xFF2abf55, 0xFF2abfaa, 0xFF2abfff, 0xFF2adf00, 0xFF2adf55, 0xFF2adfaa, 0xFF2adfff, 0xFF2aff00, 0xFF2aff55, 0xFF2affaa, 0xFF2affff, 0xFF550000, 0xFF550055, 0xFF5500aa, 0xFF5500ff, 0xFF551f00, 0xFF551f55, 0xFF551faa, 0xFF551fff, 0xFF553f00, 0xFF553f55, 0xFF553faa, 0xFF553fff, 0xFF555f00, 0xFF555f55, 0xFF555faa, 0xFF555fff, 0xFF557f00, 0xFF557f55, 0xFF557faa, 0xFF557fff, 0xFF559f00, 0xFF559f55, 0xFF559faa, 0xFF559fff, 0xFF55bf00, 0xFF55bf55, 0xFF55bfaa, 0xFF55bfff, 0xFF55df00, 0xFF55df55, 0xFF55dfaa, 0xFF55dfff, 0xFF55ff00, 0xFF55ff55, 0xFF55ffaa, 0xFF55ffff, 0xFF7f0000, 0xFF7f0055, 0xFF7f00aa, 0xFF7f00ff, 0xFF7f1f00, 0xFF7f1f55, 0xFF7f1faa, 0xFF7f1fff, 0xFF7f3f00, 0xFF7f3f55, 0xFF7f3faa, 0xFF7f3fff, 0xFF7f5f00, 0xFF7f5f55, 0xFF7f5faa, 0xFF7f5fff, 0xFF7f7f00, 0xFF7f7f55, 0xFF7f7faa, 0xFF7f7fff, 0xFF7f9f00, 0xFF7f9f55, 0xFF7f9faa, 0xFF7f9fff, 0xFF7fbf00, 0xFF7fbf55, 0xFF7fbfaa, 0xFF7fbfff, 0xFF7fdf00, 0xFF7fdf55, 0xFF7fdfaa, 0xFF7fdfff, 0xFF7fff00, 0xFF7fff55, 0xFF7fffaa, 0xFF7fffff, 0xFFaa0000, 0xFFaa0055, 0xFFaa00aa, 0xFFaa00ff, 0xFFaa1f00, 0xFFaa1f55, 0xFFaa1faa, 0xFFaa1fff, 0xFFaa3f00, 0xFFaa3f55, 0xFFaa3faa, 0xFFaa3fff, 0xFFaa5f00, 0xFFaa5f55, 0xFFaa5faa, 0xFFaa5fff, 0xFFaa7f00, 0xFFaa7f55, 0xFFaa7faa, 0xFFaa7fff, 0xFFaa9f00, 0xFFaa9f55, 0xFFaa9faa, 0xFFaa9fff, 0xFFaabf00, 0xFFaabf55, 0xFFaabfaa, 0xFFaabfff, 0xFFaadf00, 0xFFaadf55, 0xFFaadfaa, 0xFFaadfff, 0xFFaaff00, 0xFFaaff55, 0xFFaaffaa, 0xFFaaffff, 0xFFd40000, 0xFFd40055, 0xFFd400aa, 0xFFd400ff, 0xFFd41f00, 0xFFd41f55, 0xFFd41faa, 0xFFd41fff, 0xFFd43f00, 0xFFd43f55, 0xFFd43faa, 0xFFd43fff, 0xFFd45f00, 0xFFd45f55, 0xFFd45faa, 0xFFd45fff, 0xFFd47f00, 0xFFd47f55, 0xFFd47faa, 0xFFd47fff, 0xFFd49f00, 0xFFd49f55, 0xFFd49faa, 0xFFd49fff, 0xFFd4bf00, 0xFFd4bf55, 0xFFd4bfaa, 0xFFd4bfff, 0xFFd4df00, 0xFFd4df55, 0xFFd4dfaa, 0xFFd4dfff, 0xFFd4ff00, 0xFFd4ff55, 0xFFd4ffaa, 0xFFd4ffff, 0xFFff0055, 0xFFff00aa, 0xFFff1f00, 0xFFff1f55, 0xFFff1faa, 0xFFff1fff, 0xFFff3f00, 0xFFff3f55, 0xFFff3faa, 0xFFff3fff, 0xFFff5f00, 0xFFff5f55, 0xFFff5faa, 0xFFff5fff, 0xFFff7f00, 0xFFff7f55, 0xFFff7faa, 0xFFff7fff, 0xFFff9f00, 0xFFff9f55, 0xFFff9faa, 0xFFff9fff, 0xFFffbf00, 0xFFffbf55, 0xFFffbfaa, 0xFFffbfff, 0xFFffdf00, 0xFFffdf55, 0xFFffdfaa, 0xFFffdfff, 0xFFffff55, 0xFFffffaa, 0xFFccccff, 0xFFffccff, 0xFF33ffff, 0xFF66ffff, 0xFF99ffff, 0xFFccffff, 0xFF007f00, 0xFF007f55, 0xFF007faa, 0xFF007fff, 0xFF009f00, 0xFF009f55, 0xFF009faa, 0xFF009fff, 0xFF00bf00, 0xFF00bf55, 0xFF00bfaa, 0xFF00bfff, 0xFF00df00, 0xFF00df55, 0xFF00dfaa, 0xFF00dfff, 0xFF00ff55, 0xFF00ffaa, 0xFF2a0000, 0xFF2a0055, 0xFF2a00aa, 0xFF2a00ff, 0xFF2a1f00, 0xFF2a1f55, 0xFF2a1faa, 0xFF2a1fff, 0xFF2a3f00, 0xFF2a3f55, 0xFFfffbf0, 0xFFa0a0a4, 0xFF808080, 0xFFff0000, 0xFF00ff00, 0xFFffff00, 0xFF0000ff, 0xFFff00ff, 0xFF00ffff, 0xFFffffff];
            public static readonly Color[] win95 = [.. win95_bytes.Select(HexToColor)];
        }

        public static Color HexToColor(uint argb)
        {
            return Color.FromArgb(
                (byte)((argb & 0xff000000) >> 0x18),
                (byte)((argb & 0xff0000) >> 0x10),
                (byte)((argb & 0xff00) >> 0x08),
                (byte)(argb & 0xff)
            );
        }

        public static void SaveToFile(this WriteableBitmap b, string outputPath)
        {
            using FileStream stream = new(outputPath, FileMode.Create);
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(b));
            encoder.Save(stream);
        }

        public static WriteableBitmap ConvertImageFromUri(string path, Color[] palette)
        {
            Uri uri = new(path);
            BitmapImage bitmapImage = new(uri);
            WriteableBitmap writeableBitmap = new(bitmapImage);
            var newBmp = LoopPixelsIndexed8(
                writeableBitmap,
                (c) =>
                    palette[GetClosestPaletteIndexMatch(c, palette)]
            );
            return writeableBitmap;
        }

        public static void LoopPixels(WriteableBitmap wb, Func<Color, Color> perPixelCallback)
        {
            
            if (wb.Format != PixelFormats.Bgr32 && wb.Format != PixelFormats.Bgra32 && wb.Format != PixelFormats.Bgr24)
            {
                throw new ArgumentException("Only Bgr32, Bgra32, and Bgr24 pixel formats are supported.\nProvided format was " + wb.Format.ToString());
            }
            
            int pixelSize = wb.Format.BitsPerPixel / 8;
            int stride = wb.BackBufferStride;
            int bytesPerPixel = pixelSize;
            int totalBytes = wb.PixelHeight * stride;

            byte[] bytes = new byte[totalBytes];

            // Copy pixels to array
            wb.CopyPixels(bytes, stride, 0);

            int index = 0;

            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
                    byte b = bytes[index];
                    byte g = bytes[index + 1];
                    byte r = bytes[index + 2];
                    byte a = bytesPerPixel == 4 ? bytes[index + 3] : (byte)255;

                    Color pixelColor = Color.FromArgb(a, r, g, b);
                    Color adjColor = perPixelCallback(pixelColor);

                    bytes[index] = adjColor.B;     // B component
                    bytes[index + 1] = adjColor.G; // G component
                    bytes[index + 2] = adjColor.R; // R component

                    if (bytesPerPixel == 4)
                    {
                        bytes[index + 3] = adjColor.A; // A component
                    }

                    index += bytesPerPixel;
                }

                // Handle stride padding
                index += stride - (wb.PixelWidth * bytesPerPixel);
            }

            // Write back the modified pixels
            wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), bytes, stride, 0);
        }

        public static WriteableBitmap? LoopPixelsIndexed8(WriteableBitmap wb, Func<Color, Color> perPixelCallback)
        {
            if (wb.Format != PixelFormats.Indexed8)
                return null;

            // Get current pixel data
            int stride = (wb.PixelWidth + 3) & ~3; // 4-byte alignment
            byte[] pixelData = new byte[wb.PixelHeight * stride];
            wb.CopyPixels(pixelData, stride, 0);

            // Process palette
            IList<Color> originalPalette = wb.Palette.Colors;
            if (originalPalette == null || originalPalette.Count == 0)
                throw new InvalidOperationException("Indexed8 bitmap has no palette");

            List<Color> newPalette = new List<Color>();
            foreach (Color color in originalPalette)
            {
                newPalette.Add(perPixelCallback(color));
            }

            // Create new bitmap with modified palette but same pixel data
            WriteableBitmap newBitmap = new WriteableBitmap(
                wb.PixelWidth, wb.PixelHeight, wb.DpiX, wb.DpiY,
                PixelFormats.Indexed8, new BitmapPalette(newPalette));

            newBitmap.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight),
                                 pixelData, stride, 0);

            // Replace the original bitmap's data (this is a workaround)
            ReplaceWriteableBitmapData(wb, newBitmap);
            return newBitmap;
        }

        // Helper method to copy bitmap data between WriteableBitmaps
        private static void ReplaceWriteableBitmapData(WriteableBitmap target, WriteableBitmap source)
        {
            if (target.PixelWidth != source.PixelWidth || target.PixelHeight != source.PixelHeight)
                throw new ArgumentException("Bitmap dimensions must match");

            int stride = source.BackBufferStride;
            byte[] pixelData = new byte[source.PixelHeight * stride];
            source.CopyPixels(pixelData, stride, 0);

            target.WritePixels(new Int32Rect(0, 0, target.PixelWidth, target.PixelHeight),
                              pixelData, stride, 0);
        }

        public static int GetClosestPaletteIndexMatch(Color col, Color[] colorPalette)
        {
            int colorMatch = 0;
            int leastDistance = int.MaxValue;
            int red = col.R;
            int green = col.G;
            int blue = col.B;
            for (int i = 0; i < colorPalette.Length; ++i)
            {
                Color paletteColor = colorPalette[i];
                int distR = paletteColor.R - red;
                int distG = paletteColor.G - green;
                int distB = paletteColor.B - blue;
                int distance = (distR * distR) + (distG * distG) + (distB * distB);
                if (distance >= leastDistance)
                    continue;
                colorMatch = i;
                leastDistance = distance;
                if (distance == 0)
                    return i;
            }
            return colorMatch;
        }
    }
}
