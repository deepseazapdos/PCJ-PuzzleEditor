using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCJ_Museum
{
    public partial class Form1 : Form
    {
        Savefile Save = new Savefile();
        PuzzleDraw Draw = new PuzzleDraw();
        BinaryTasks BinTasks = new BinaryTasks();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }


        // -----------------------------------------------
        // ToolStrip related functions
        // _______________________________________________
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save.Open())
            {
                FormEnabled(true);
                SetFormValues();
            }
        }

        // Enable or disable form elements
        private void FormEnabled(bool State)
        {
            listBox1.Enabled = State;
            checkedListBox1.Enabled = State;
            pictureBox1.Enabled = State;
            checkBox1.Enabled = State;
            checkBox2.Enabled = State;
        }

        // Set form values after opening
        private void SetFormValues()
        {
            // Selected puzzle
            listBox1.SelectedIndex = 0;
            pictureBox1.Image = Draw.DrawPuzzle(listBox1.SelectedIndex, BinTasks.PuzzlePiecesToList(GetPuzzlePieces(listBox1.SelectedIndex)));

            // Puzzle complete flags
            List<bool> CurrentNEW = GetNEWPuzzleFlags();
            checkBox1.Checked = CurrentNEW[7 - listBox1.SelectedIndex];
            checkBox2.Checked = BinTasks.IsPuzzleComplete(GetPuzzlePieces(listBox1.SelectedIndex));

            // Bonus
            List<bool> Bonus = BinTasks.GetPuzzleBonus(Save.PuzzleBonusValues);

            for (int a = 5; a >= 0; a--)
            {
                checkedListBox1.SetItemChecked(a, Bonus[a]);
            }

            checkedListBox1.SetItemChecked(6, Bonus[7]);
            checkedListBox1.SetItemChecked(7, Bonus[6]);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save.IsOpen)
            {
                if (Save.Export())
                    MessageBox.Show("Exported successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("No save file has been opened.", "No save file opened", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEnabled(false);
            Save.IsOpen = false;

            Save.PuzzlePieceValues.Clear();
            Save.PuzzleNEWValues = 0;
            Save.PuzzleBonusValues = 0;
        }

        private void aboutThisSoftwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pocket Card Jockey (3DS) puzzle piece editor\nMade with <3 for the Pocket Card Jockey community.", "About");
        }


        // -----------------------------------------------
        // Form related functions
        // _______________________________________________

        // Selected puzzle from listBox1
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Selected puzzle
            ushort inputValue = GetPuzzlePieces(listBox1.SelectedIndex);
            pictureBox1.Image = Draw.DrawPuzzle(listBox1.SelectedIndex, BinTasks.PuzzlePiecesToList(inputValue));

            // Puzzle complete flag
            bool isComplete = BinTasks.IsPuzzleComplete(inputValue);
            checkBox2.Checked = isComplete;

            // NEW puzzle flag
            List<bool> CurrentChecked = GetNEWPuzzleFlags();
            checkBox1.Checked = CurrentChecked[7 - listBox1.SelectedIndex];
        }

        // Add or remove pieces
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Check mouse position
            MouseEventArgs me = (MouseEventArgs) e;
            Point MousePos = me.Location;

            List<bool> Pieces = BinTasks.PuzzlePiecesToList(GetPuzzlePieces(listBox1.SelectedIndex));

            // Check and add/remove the pieces
            int Position = 0;

            for (int a = 0; a <= 2; a++)
            {
                for (int b = 0; b <= 2; b++)
                {
                    if (CheckBounds(MousePos, new Point(b, a)))
                    {
                        Pieces[Position] = !Pieces[Position];
                    }
                    Position++;
                }
            }

            // Change save file values
            // If all pieces are obtained...
            if (!Pieces.Contains(false))
            {
                for (int a = 0; a <= 8; a++)
                {
                    Pieces[a] = false;
                }
                // ...apply changes to save file values,
                // including NEW and complete flags
                CompletePuzzle(listBox1.SelectedIndex);

            }
            else
            {
                // Apply piece changes only
                for (int a = 0; a <= 8; a++)
                {
                    BinTasks.EditBit_ListUInt16(a, Pieces[a], ref Save.PuzzlePieceValues, listBox1.SelectedIndex);
                }
            }

            // Draw the puzzle
            pictureBox1.Image = Draw.DrawPuzzle(listBox1.SelectedIndex, Pieces);
        }

        // Check if the mouse is in bounds of a puzzle piece
        private bool CheckBounds(Point MousePos, Point Offset)
        {
            if (MousePos.X >= 0 + (56 * Offset.X) &&
                MousePos.X < 56 + (56 * Offset.X) &&
                MousePos.Y >= 0 + (56 * Offset.Y) &&
                MousePos.Y < 56 + (56 * Offset.Y))
                return true;
            else
                return false;
        }

        // Complete a puzzle and change the NEW and complete flags
        private void CompletePuzzle(int Index)
        {
            // NEW
            checkBox1.Checked = true;
            BinTasks.EditBit_Byte(Index, true, ref Save.PuzzleNEWValues);

            // Complete
            checkBox2.Checked = true;
            Save.PuzzlePieceValues[Index] = 0b00000010_00000000;

            // Bonus effect
            checkedListBox1.SetItemChecked(listBox1.SelectedIndex, true);
            checkedListBox1.SelectedIndex = listBox1.SelectedIndex;
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            BinTasks.EditBit_Byte(listBox1.SelectedIndex, checkBox1.Checked, ref Save.PuzzleNEWValues);
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            BinTasks.EditBit_ListUInt16(9, checkBox2.Checked, ref Save.PuzzlePieceValues, listBox1.SelectedIndex);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // bits 2 - 7
            for (int a = 2; a <= 7; a++)
            {
                BinTasks.EditBit_UInt16(9 - a, checkedListBox1.GetItemChecked(a - 2), ref Save.PuzzleBonusValues);
            }
            // bits 8 - 9
            for (int a = 8; a <= 9; a++)
            {
                BinTasks.EditBit_UInt16(a, checkedListBox1.GetItemChecked(a - 2), ref Save.PuzzleBonusValues);
            }
        }


        // -----------------------------------------------
        // Other functions (to get and reflect
        // Form changes into the save file)
        // _______________________________________________

        // Puzzle pieces & Puzzle complete flag
        private ushort GetPuzzlePieces(int Index)
        {
            return Save.PuzzlePieceValues.ElementAt(Index);
        }

        // NEW Puzzle flags
        private List<bool> GetNEWPuzzleFlags()
        {
            return BinTasks.IsNEW(((byte)Save.PuzzleNEWValues));
        }
    }
}