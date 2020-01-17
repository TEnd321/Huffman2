using System;
using System.Collections.Generic;
using System.Text;

namespace Huffman3
{
	public class HuffmanTree : IComparable
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
}
