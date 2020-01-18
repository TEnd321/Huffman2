using System;
using System.Collections.Generic;
using System.Text;

namespace Huffman3
{
    public class TreeLoader
    {
        public HuffmanLoader HuffmanLoader { get; private set; }
        public HuffmanTree RootTree { get; private set; }
        Stack<HuffmanTree> LoadingStack { get; set; }

        public TreeLoader(HuffmanLoader hf)
        {
            HuffmanLoader = hf;
            LoadingStack = new Stack<HuffmanTree>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null, if tree is not complete.</returns>
        public HuffmanTree Create()
        {
            HuffmanTree temp = HuffmanLoader.NextNode();
            if ((temp.root.symbol == -1) && (temp.root.weight == 0))
            {
                return null;
            }
            if (temp.root.symbol != -1)
                return temp;
            temp.root.Left  = Create();
            temp.root.Right = Create();
            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True, if tree was succesfully built. Otherwise false.</returns>
        public bool Build()
        {
            RootTree = Create();
            if ((RootTree == null) || (Create() != null))
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
