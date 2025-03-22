using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCJ_Museum
{
    public class Savefile
    {
        private FileStream SavefileStream;
        public bool IsOpen = new bool { };

        public List<UInt16> PuzzlePieceValues = new List<UInt16>(8) { };
        public byte PuzzleNEWValues = new byte { };
        public UInt16 PuzzleBonusValues = new UInt16 { };


        // -----------------------------------------------
        // Open + export save file
        // _______________________________________________

        // Open a gamedata save file
        public bool Open()
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Dialog.Title = "Open save file";
            Dialog.Filter = "Pocket Card Jockey gamedata save files||All files (*.*)|*.*";
            Dialog.FilterIndex = 1;
            Dialog.RestoreDirectory = true;
            Dialog.Multiselect = false;

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                SavefileStream = File.OpenRead(Dialog.FileName);

                ReadAll();

                SavefileStream.Flush();
                SavefileStream.Close();

                IsOpen = true;
                return true;
            }
            return false;
        }

        // Export save file
        public bool Export()
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Dialog.Title = "Export save file";
            Dialog.Filter = "Pocket Card Jockey gamedata save files||All files (*.*)|*.*";
            Dialog.FilterIndex = 1;
            Dialog.RestoreDirectory = true;
            Dialog.Multiselect = false;

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                SavefileStream = File.OpenWrite(Dialog.FileName);

                WriteAll();

                SavefileStream.Flush();
                SavefileStream.Close();

                return true;
            }
            return false;
        }


        // -----------------------------------------------
        // Read + write offsets
        // _______________________________________________

        // Read from all the relevant save file offsets
        public void ReadAll()
        {
            PuzzlePieceValues.Clear();
            PuzzleNEWValues = 0;
            PuzzleBonusValues = 0;

            BinaryReader reader = new BinaryReader(SavefileStream);

            // Puzzle piece values
            // Read 2 bytes (8 bits * 2) starting from 0x21F0 0x21F1 until 0x21FE 0x21FF
            for (var Current = 0x21F0; Current <= 0x21FE; Current += 0x2)
            {
                SavefileStream.Seek(Current, SeekOrigin.Begin);
                PuzzlePieceValues.Add(reader.ReadUInt16());
            }

            // NEW values
            SavefileStream.Seek(0x2203, SeekOrigin.Begin);
            PuzzleNEWValues = reader.ReadByte();

            // Bonus values
            SavefileStream.Seek(0x3CA0, SeekOrigin.Begin);
            PuzzleBonusValues = reader.ReadUInt16();
        }

        // Write to all relevant save file offsets
        public void WriteAll()
        {
            BinaryWriter writer = new BinaryWriter(SavefileStream);

            // Puzzle piece values
            var CurrentOffset = 0;
            for (var Current = 0x21F0; Current <= 0x21FE; Current += 0x2)
            {
                SavefileStream.Seek(Current, SeekOrigin.Begin);
                writer.Write(PuzzlePieceValues.ElementAt(CurrentOffset));
                CurrentOffset++;
            }

            // NEW values
            SavefileStream.Seek(0x2203, SeekOrigin.Begin);
            writer.Write(PuzzleNEWValues);

            // Bonus values
            SavefileStream.Seek(0x3CA0, SeekOrigin.Begin);
            writer.Write(PuzzleBonusValues);
        }
    }
}