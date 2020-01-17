using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman2
{
	public class HuffmanLoader
	{
		BinaryReader binaryReader; //to fill readingBuffer
		byte[] readingBuffer; //buffer for reading bytes from input
		int bufferIndex; //index of buffer byte next to be read
		byte currentByte; //current byte being read
		int endOfBuffer; //practically, it sotres how many bytes are in readingBuffer 

		/// <summary>
		/// Method for reading next Node.
		/// </summary>
		/// <returns>Long representation of next Node.</returns>
		public HuffmanTree NextNode()
		{
			//TODO: Check for error in reading next byte!!

			ulong tempWeight = 0;
			ulong tempULong = 0;
			byte tempSymbol = 0;
			bool tempIsLeaf = false;
			ushort binShift = 0;

			for (int i = 0; i < 7; i++)
			{
				tempULong = NextByte();
				tempULong <<= binShift;
				tempWeight |= tempULong;
				binShift += 8;
			}
			if ((tempWeight | 1UL) == 1)
				tempIsLeaf = true;
			else
				tempIsLeaf = false;
			tempWeight >>= 1;
			tempSymbol = NextByte();

			if (tempIsLeaf)
				return new HuffmanTree(tempSymbol, (long)tempWeight);
			else
				return new HuffmanTree(-1, (long)tempWeight);
		}

		public void SkipHeader()
		{
			bufferIndex = 8;
		}

		public bool NextBit()
		{
			return false;
		}

		public byte[] ReadNextSegment()
		{
			byte[] toReturn = new byte[4096];
			try
			{
				endOfBuffer = binaryReader.Read(toReturn, 0, 4096);
			}
			catch
			{
				Console.WriteLine("File Error");
				binaryReader.Close();
				Environment.Exit(1);
			}
			return toReturn;
		}

		/// <summary>
		/// Will read next byte from reading Buffer to currentByte and advance bufferIndex. Will read next 4096 bytes long segment, if index reaches end.
		/// </summary>
		public byte NextByte()
		{
			if (bufferIndex == endOfBuffer || bufferIndex == -1)
			{
				readingBuffer = ReadNextSegment();
				SkipHeader();
			}
			currentByte = readingBuffer[bufferIndex];
			bufferIndex++;
			return currentByte;
		}
	}
}
