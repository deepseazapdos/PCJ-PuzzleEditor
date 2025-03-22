using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCJ_Museum
{
    internal class PuzzleDraw
    {
        public Bitmap DrawPuzzle(int PuzzleIndex, List<bool> Pieces)
        {
            Bitmap Canvas = new Bitmap(200, 200);

            using (Graphics g = Graphics.FromImage(Canvas))
            {
                // Image names
                String[] Name = new String[]
                {
                    "narikin",
                    "yankee",
                    "hatumei",
                    "IT",
                    "Idol",
                    "tubame",
                    "soritiba",
                    "jockey"
                };

                // Drawing positions
                Point[] Position = new Point[]
                {
                    new Point(0, 0),    // Image 1
                    new Point(52, 0),   // Image 2
                    new Point(96, 0),   // Image 3
                    new Point(0, 52),   // Image 4
                    new Point(36, 56),  // Image 5
                    new Point(108, 52), // Image 6
                    new Point(0, 108),  // Image 7
                    new Point(52, 96),  // Image 8
                    new Point(96, 96)   // Image 9
                };

                // Draw puzzle
                for (int a = 0; a <= 8; a++)
                {
                    if (Pieces[a])
                    {
                        var CurrentImage = Properties.Resources.ResourceManager.GetObject(Name[PuzzleIndex] + "_0" + (a + 1).ToString());

                        if (CurrentImage is Image i)
                        {
                            g.DrawImage(i, Position[a]);
                        }
                    }
                }
            }
            return Canvas;
        }
    }
}