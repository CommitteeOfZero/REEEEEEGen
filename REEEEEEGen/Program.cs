using Imaging.DDSReader;
using SciAdvNet.SC3Script;
using SciAdvNet.SC3Script.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REEEEEEGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = SC3Game.ChaosChild;
            var charset = new CharacterSet(game, "");

            const int outlinePadding = 4;
            const int cellWidth = 32;
            const int cellHeight = 32;
            const int outlineCellWidth = 38;
            const int outlineCellHeight = 38;
            const double scaleFactor = 1.5;
            const int colCount = 64;

            var fontA = DDS.LoadImage(Assets.FONT_A);
            var fontB = DDS.LoadImage(Assets.FONT_B);
            var fontOutlineA = DDS.LoadImage(Assets.font_outline_A);
            var fontOutlineB = DDS.LoadImage(Assets.font_outline_B);

            var font = new Bitmap(fontA.Width, fontA.Height + fontB.Height);
            var fontOutline = new Bitmap(fontOutlineA.Width, fontOutlineA.Height + fontOutlineB.Height);

            var fontGraphics = Graphics.FromImage(font);
            fontGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            fontGraphics.DrawImage(fontA, new Point(0, 0));
            fontGraphics.DrawImage(fontB, new Point(0, fontA.Height));
            var fontOutlineGraphics = Graphics.FromImage(fontOutline);
            fontOutlineGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            fontOutlineGraphics.DrawImage(fontOutlineA, new Point(0, 0));
            fontOutlineGraphics.DrawImage(fontOutlineB, new Point(0, fontOutlineA.Height));

            float[][] blackColorMatrixElements = {
               new float[] {0,  0,  0,  0, 0},
               new float[] {0,  0,  0,  0, 0},
               new float[] {0,  0,  0,  0, 0},
               new float[] {0,  0,  0,  1, 0},
               new float[] {0, 0, 0, 0, 1}};
            ColorMatrix blackColorMatrix = new ColorMatrix(blackColorMatrixElements);
            float[][] tipColorMatrixElements = {
               new float[] {144.0f/255.0f,  0,  0,  0, 0},
               new float[] {0,  1,  0,  0, 0},
               new float[] {0,  0,  1,  0, 0},
               new float[] {0,  0,  0,  1, 0},
               new float[] {0, 0, 0, 0, 1}};
            ColorMatrix tipColorMatrix = new ColorMatrix(tipColorMatrixElements);

            var outlineAttrs = new ImageAttributes();
            var normalFontAttrs = new ImageAttributes();
            var tipFontAttrs = new ImageAttributes();
            outlineAttrs.SetColorMatrix(
               blackColorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            tipFontAttrs.SetColorMatrix(
                tipColorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap
                );

            int lineId = 0;
            string line;
            StreamReader reader = new StreamReader("input.txt");
            while ((line = reader.ReadLine()) != null)
            {

                int totalWidth = 32;
                int totalHeight = 72;
                int curX = 16;
                int curY = 10;
                bool isTipMode = false;

                foreach (char c in line)
                {
                    if (c == '|') continue;
                    ushort charId = (ushort)(charset.EncodeCharacter(c) & 0x7FFF);
                    totalWidth += (int)Math.Round(Assets.widths[charId] * scaleFactor);
                }

                var output = new Bitmap(totalWidth, totalHeight);
                var outputGraphics = Graphics.FromImage(output);
                outputGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                foreach (char c in line)
                {
                    if (c == '|')
                    {
                        isTipMode = !isTipMode;
                        continue;
                    }

                    ushort charId = (ushort)(charset.EncodeCharacter(c) & 0x7FFF);
                    int row = charId / colCount;
                    int col = charId % colCount;

                    int glyphWidth = (int)Math.Round(Assets.widths[charId] * scaleFactor);

                    var outlineDestRect = new Rectangle(
                        curX - outlinePadding,
                        curY - outlinePadding,
                        (int)(outlineCellWidth * scaleFactor),
                        (int)(outlineCellHeight * scaleFactor));

                    var destRect = new Rectangle(
                        curX,
                        curY,
                        (int)(cellWidth * scaleFactor),
                        (int)(cellHeight * scaleFactor));

                    outputGraphics.DrawImage(
                        fontOutline,
                        outlineDestRect,
                        (int)(col * outlineCellWidth * scaleFactor),
                        (int)(row * outlineCellHeight * scaleFactor),
                        (int)(outlineCellWidth * scaleFactor),
                        (int)(outlineCellHeight * scaleFactor),
                        GraphicsUnit.Pixel,
                        outlineAttrs);

                    outputGraphics.DrawImage(
                        font,
                        destRect,
                        (int)(col * cellWidth * scaleFactor),
                        (int)(row * cellHeight * scaleFactor),
                        (int)(cellWidth * scaleFactor),
                        (int)(cellHeight * scaleFactor),
                        GraphicsUnit.Pixel,
                        isTipMode ? tipFontAttrs : normalFontAttrs);

                    curX += glyphWidth;
                }

                output.Save(string.Format("line{0}.png", lineId));
                lineId++;
            }
        }
    }
}
