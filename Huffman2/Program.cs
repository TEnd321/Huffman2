using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Huffman2
{
	class Program {

		///// <summary>
		///// Struct for storing information about Node found in .huff file.
		///// </summary>
		//public struct NodeInfo
		//{
		//	public byte Symbol { get; set; }
		//	public ulong Weight { get; set; }
		//	public bool IsLeaf { get; set; }

		//	public NodeInfo(byte symbol, ulong weight, bool isLeaf)
		//	{
		//		Symbol = symbol;
		//		Weight = weight;
		//		IsLeaf = isLeaf;
		//	}
		//}

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
