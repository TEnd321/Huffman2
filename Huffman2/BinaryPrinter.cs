using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman3
{
	class BinaryPrinter
	{
		class BinaryStream
		{
			public int NoOfBits { get; set; }
			public ulong path;

			public BinaryStream(int ending, ulong path)
			{
				NoOfBits = ending;
				this.path = path;
			}

			public BinaryStream(BinaryStream input)
			{
				NoOfBits = input.NoOfBits;
				path = input.path;
			}
		}

		FileStream OutStream;
		BinaryStream[] paths = new BinaryStream[256];
		BinaryStream tempStream;
		string input;

		public HuffmanTree Tree { get; private set; }

		public BinaryPrinter(string output, string input, HuffmanTree tree)
		{
			this.input = input;
			Tree = tree;
			tempStream = new BinaryStream(0, 0);
			try
			{
				OutStream = new FileStream(output, FileMode.Create);
			}
			catch
			{
				Console.WriteLine("File Error");
				OutStream = null;
			}
		}

		/// <summary>
		/// Will print tree header.
		/// </summary>
		public void PrintTreeHeader()
		{
			//                          | 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 | ~~ printing is Little-Endian
			byte[] toPrint = new byte[] { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
			OutStream.Write(toPrint, 0, toPrint.Length);
			OutStream.Flush();
		}

		/// <summary>
		/// Will print nodes of Huffman tree.
		/// </summary>
		/// <param name="Tree"></param>
		public void PrintTreeNodes(HuffmanTree Tree)
		{
			ulong headerPart = 0;
			byte[] part;

			if (Tree.IsLeaf())
			{
				paths[Tree.root.symbol] = new BinaryStream(tempStream);

				headerPart |= (ushort)Tree.root.symbol;
				headerPart <<= 55;

				//TODO:aodjsfghnbiausydb
				headerPart |= (ulong)Tree.root.weight;
				headerPart <<= 1;

				headerPart |= 1;

				part = BitConverter.GetBytes(headerPart);
				OutStream.Write(part, 0, part.Length);
				OutStream.Flush();
			}
			else
			{
				headerPart |= (ulong)Tree.root.weight;
				headerPart <<= 1;

				part = BitConverter.GetBytes(headerPart);
				OutStream.Write(part, 0, part.Length);
				OutStream.Flush();
			}

			if (Tree.root.Left != null)
			{
				tempStream.NoOfBits++;
				tempStream.path <<= 1;
				PrintTreeNodes(Tree.root.Left);
				tempStream.path ^= 1; //last bit is set form 0 to 1
				PrintTreeNodes(Tree.root.Right);
				tempStream.path >>= 1;
				tempStream.NoOfBits--;
			}
		}

		/// <summary>
		/// Function shall process Input file once again and process it into bit representation of Huffman tree.
		/// </summary>
		public void PrintTreeData()
		{
			byte[] part = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
			OutStream.Write(part, 0, part.Length);
			OutStream.Flush();

			int correctlyRead;
			part = new byte[4096];
			byte[] printingArray = new byte[4096];
			short printingArrayI = 0;
			byte counter = 0;
			byte printingBuffer = 0;
			FileStream InStream = new FileStream(input, FileMode.Open);

			while ((correctlyRead = InStream.Read(part, 0, 4096)) != 0)
			{
				for (int i = 0; i < correctlyRead; i++)
				{
					BinaryStream binaryStream = new BinaryStream(paths[part[i]]);

					ulong temp;

					while (binaryStream.NoOfBits > 0)
					{
						if (counter == 8)
						{
							printingArray[printingArrayI] = printingBuffer;
							printingArrayI++;
							if (printingArrayI == 4096)
							{
								OutStream.Write(printingArray, 0, 4096);
								OutStream.Flush();
								printingArrayI = 0;
							}
							printingBuffer = 0;
							counter = 0;
						}

						counter++;
						temp = binaryStream.path;
						temp >>= paths[part[i]].NoOfBits - 1;
						temp <<= 7;
						printingBuffer |= (byte)temp;

						if (counter != 8)
						{
							printingBuffer >>= 1;
						}
						binaryStream.path <<= 1;
						binaryStream.NoOfBits--;
					}
				}
			}
			if ((8 - (counter + 1)) > 0)
				printingBuffer >>= 8 - (counter + 1);
			printingArray[printingArrayI] = printingBuffer;
			printingArrayI++;
			OutStream.Write(printingArray, 0, printingArrayI);
			OutStream.Flush();
		}
	}
}
