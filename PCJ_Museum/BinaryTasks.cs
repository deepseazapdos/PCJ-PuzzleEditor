using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCJ_Museum
{
    public class BinaryTasks
    {
        // Edit bit (Byte)
        public void EditBit_Byte(int Index, bool NewVal, ref byte TargetVariable)
        {
            if (NewVal)
                TargetVariable |= (byte)(1 << Index);
            else
                TargetVariable &= (byte)~(1 << Index);
        }

        // Edit bit (UInt16)
        public void EditBit_UInt16(int Index, bool NewVal, ref UInt16 TargetVariable)
        {
            if (NewVal)
                TargetVariable |= (UInt16)(1 << Index);
            else
                TargetVariable &= (UInt16)~(1 << Index);
        }

        // Edit bit (List<UInt16>)
        public void EditBit_ListUInt16(int Index, bool NewVal, ref List<UInt16> TargetList, int ListIndex)
        {
            UInt16 TargetVariable = TargetList[ListIndex];

            if (NewVal)
                TargetVariable |= (UInt16)(1 << Index);
            else
                TargetVariable &= (UInt16)~(1 << Index);
            TargetList[ListIndex] = TargetVariable;
        }


        // Return the pieces into a bool list that can
        // be used by PuzzleDraw to draw a whole puzzle
        public List<bool> PuzzlePiecesToList(ushort Input)
        {
            List<bool> bits = new List<bool>();

            // From bit 9 (index 8) to bit 1 (index 0)
            for (int a = 8; a >= 0; a--)
            {
                bool isBitSet = (Input & (1 << a)) != 0;
                bits.Add(isBitSet);
            }
            bits.Reverse();
            return bits;
        }


        // Check if the puzzle is complete or not
        public bool IsPuzzleComplete(ushort Input)
        {
            ushort Mask = 0b00000010_00000000;
            return (Input & Mask) != 0;
        }


        // Check if the puzzle is NEW
        public List<bool> IsNEW(Int32 Input)
        {
            List<bool> Bits = new List<bool>();

            for (int a = 7; a >= 0; a--)
            {
                bool isBitSet = (Input & (1 << a)) != 0;
                Bits.Add(isBitSet);
            }
            return Bits;
        }


        // Retrieve bonus
        public List<bool> GetPuzzleBonus(UInt16 input)
        {
            List<bool> Bits = new List<bool>();

            for (int a = 7; a >= 2; a--)
            {
                Bits.Add((input & (1 << a)) != 0);
            }

            for (int a = 9; a >= 8; a--)
            {
                Bits.Add((input & (1 << a)) != 0);
            }
            return Bits;
        }
    }
}
