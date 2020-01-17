using System;
using System.Collections.Generic;
using System.Text;

namespace Huffman3
{
	public class Node
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
}
