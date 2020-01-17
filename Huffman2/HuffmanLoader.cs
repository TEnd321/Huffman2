using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman3
{
	public class HuffmanLoader
	{
		Stream ReadingStream { get; set; } //to fill readingBuffer
		byte[] readingBuffer; //buffer for reading bytes from input
		int bufferIndex = 0; //index of buffer byte next to be read
		byte currentByte = 0; //current byte being read
		int endOfBuffer = 0; //practically, it sotres how many bytes are in readingBuffer 

		public HuffmanLoader(Stream stream)
		{
			ReadingStream = stream;
		}

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

		public bool NextBit()
		{
			return false;
		}

		public byte[] ReadNextSegment()
		{
			byte[] toReturn = new byte[4096];
			try
			{
				endOfBuffer = ReadingStream.Read(toReturn, 0, 4096);
			}
			catch
			{
				Console.WriteLine("File Error");
				ReadingStream.Dispose();
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
			}
			currentByte = readingBuffer[bufferIndex];
			bufferIndex++;
			return currentByte;
		}
	}
}
