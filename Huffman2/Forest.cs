using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman3
{
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
}
