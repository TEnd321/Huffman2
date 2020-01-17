using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Huffman2
{
	class Program
	{

		class Node
		{
			public HuffmanTree Left, Right, Parent;
			public long weight;
			public short symbol;

			public Node()
			{
				Left = Right = Parent = null;
				weight = symbol = 0;
			}

			public Node(short symbol)
			{
				Left = Right = Parent = null;
				this.symbol = symbol;
				weight = 0;
			}

			public Node(short symbol, long weight)
			{
				Left = Right = Parent = null;
				this.symbol = symbol;
				this.weight = weight;
			}
		}

		class HuffmanTree : IComparable
		{
			static ulong counter = 0;
			public ulong counterIndex;
			public Node root;

			public HuffmanTree()
			{
				counterIndex = counter;
				root = new Node();
				counter++;
			}

			public HuffmanTree(short symbol)
			{
				root = new Node(symbol);
			}

			public HuffmanTree(short symbol, long weight)
			{
				root = new Node(symbol, weight);
			}

			public bool IsLeaf()
			{
				return ((root.Left == null) && (root.Right == null));
			}

			public int CompareTo(object obj)
			{
				HuffmanTree tree = obj as HuffmanTree;
				Node node = tree.root;

				if (this.IsLeaf() && tree.IsLeaf())
				{
					int comparisonResult = this.root.weight.CompareTo(node.weight);
					if (comparisonResult == 0)
					{
						return this.root.symbol.CompareTo(node.symbol);
					}
					else
					{
						return comparisonResult;
					}
				}
				else if (this.IsLeaf() && !tree.IsLeaf())
				{
					int comparisonResult = this.root.weight.CompareTo(node.weight);
					if (comparisonResult == 0)
					{
						return -1;
					}
					else
					{
						return comparisonResult;
					}
				}
				else if (!this.IsLeaf() && tree.IsLeaf())
				{
					int comparisonResult = this.root.weight.CompareTo(node.weight);
					if (comparisonResult == 0)
					{
						return 1;
					}
					else
					{
						return comparisonResult;
					}
				}
				else
				{
					int comparisonResult = this.root.weight.CompareTo(node.weight);
					if (comparisonResult == 0)
					{
						return this.counterIndex.CompareTo(tree.counterIndex);
					}
					else
					{
						return comparisonResult;
					}
				}
			}
		}

		class Forest
		{
			List<HuffmanTree> forest;

			public bool Load(BinLoader loader)
			{
				if ((forest = loader.LoadBinary()) == null)
					return false;

				return true;
			}

			public void Sort()
			{
				forest.Sort();
			}

			public HuffmanTree GetTop()
			{
				if (forest[0].root == null)
					return null;

				return forest[0];
			}

			/// <summary>
			/// Process forest, so only one tree will remain in list after funcion returns.
			/// </summary>
			public void CutDown()
			{
				while (forest.Count > 1)
				{
					forest.Sort();

					HuffmanTree newRoot = new HuffmanTree();
					HuffmanTree tree1 = forest[0];
					HuffmanTree tree2 = forest[1];

					tree1.root.Parent = newRoot;
					tree2.root.Parent = newRoot;
					newRoot.root.Left = tree1;
					newRoot.root.Right = tree2;
					newRoot.root.weight = tree2.root.weight + tree1.root.weight;
					newRoot.root.symbol = -1;

					forest.RemoveAt(0);
					forest.RemoveAt(0);

					forest.Add(newRoot);
				}
			}

			public void Print(HuffmanTree tree, TextWriter writer)
			{
				if (!tree.IsLeaf())
				{
					writer.Write("{0} ", tree.root.weight);
					writer.Flush();
				}
				else
				{
					writer.Write("*{0}:{1} ", tree.root.symbol, tree.root.weight);
					writer.Flush();
				}
				if (tree.root.Left != null)
					Print(tree.root.Left, writer);
				if (tree.root.Right != null)
					Print(tree.root.Right, writer);
			}
		}



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

		class BinLoader
		{
			public FileStream reader;
			List<HuffmanTree> forest = new List<HuffmanTree>();

			public BinLoader(string path)
			{
				try
				{
					reader = new FileStream(path, FileMode.Open);
				}
				catch
				{
					reader = null;
					Console.WriteLine("File Error");
				}
			}

			public void ForestInit()
			{
				for (int i = 0; i < 256; i++)
				{
					forest.Add(new HuffmanTree());

				}
				for (int i = 0; i < 256; i++)
				{
					forest[i].root.symbol = (short)i;
				}
			}

			public void ForestClean()
			{
				for (int i = 255; i >= 0; i--)
				{
					if (forest[i].root.weight == 0)
					{
						forest.RemoveAt(i);
					}
				}
			}

			public List<HuffmanTree> LoadBinary()
			{
				byte[] loadedBytes = new byte[4096];
				int bytesRead;
				ForestInit();

				try
				{
					bytesRead = reader.Read(loadedBytes, 0, 4096);
				}
				catch
				{
					Console.WriteLine("File Error");
					return null;
				}

				while ( bytesRead != 0 )
				{
					for (int i = 0; i < bytesRead; i++)
					{
						forest[loadedBytes[i]].root.weight++;
					}

					try
					{
						bytesRead = reader.Read(loadedBytes, 0, 4096);
					}
					catch
					{
						Console.WriteLine("File Error");
						return null;
					}
				}

				reader.Close();
				ForestClean();
				return forest;
			}


		}

		/// <summary>
		/// Struct for storing information about Node found in .huff file.
		/// </summary>
		public struct NodeInfo
		{
			public byte Symbol { get; set; }
			public ulong Weight { get; set; }
			public bool IsLeaf { get; set; }

			public NodeInfo(byte symbol, ulong weight, bool isLeaf)
			{
				Symbol = symbol;
				Weight = weight;
				IsLeaf = isLeaf;
			}
		}

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
			public NodeInfo NextNode()
			{
				//TODO: Check for error in reading next byte!!

				ulong tempWeight = 0;
				ulong tempULong = 0;
				byte tempSymbol = 0;
				bool tempIsLeaf = false;
				ushort binShift = 8;

				tempWeight = NextByte();
				if ((tempWeight | 1UL) == 1)
					tempIsLeaf = true;
				else
					tempIsLeaf = false;
				for (int i = 0; i < 7; i++)
				{
					tempULong = NextByte();
					tempULong <<= binShift;
					tempWeight |= tempULong;
					binShift *= 8;
				}
				tempWeight >>= 1;
				tempSymbol = NextByte();

				return new NodeInfo(tempSymbol, tempWeight, tempIsLeaf);
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

		public class TreeLoader
		{
			HuffmanLoader huffmanLoader;
		}

		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Argument Error");
				return;
			}
			BinLoader loader = new BinLoader(args[0]);
			if (loader.reader == null)
			{
				return;
			}
			TextWriter writer = Console.Out;
			//TextWriter writer = new StreamWriter("out.txt");
			Forest forest = new Forest();

			if (!forest.Load(loader))
			{
				return;
			}
			forest.CutDown();
			HuffmanTree tree = forest.GetTop();
			BinaryPrinter printer = new BinaryPrinter(String.Concat(args[0] + ".huff"), args[0], tree);
			if (tree.root != null)
			{
				printer.PrintTreeHeader();
				printer.PrintTreeNodes(tree);
				printer.PrintTreeData();
			}
		}
	}
}
