using System;
using System.Collections.Generic;
using System.Text;

namespace Huffman3
{
	public class TreeLoader
	{
		HuffmanLoader HuffmanLoader { get; set; }
		public HuffmanTree RootTree { get; private set; }

		public TreeLoader(HuffmanLoader hf)
		{
			HuffmanLoader = hf;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Null, if tree is not complete.</returns>
		public HuffmanTree Create()
		{
			HuffmanTree temp = HuffmanLoader.NextNode();
			if (temp.root.symbol != -1)
				return temp;
			if (temp.root.weight == 0)
				return null;
			temp.root.Left = Create();
			temp.root.Right = Create();
			if (temp.root.Left == null || temp.root.Right == null)
				return null;
			return temp;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>True, if tree was succesfully built. Otherwise false.</returns>
		public bool Build()
		{
			RootTree = Create();
			if (Create() != null)
			{
				RootTree = null;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
