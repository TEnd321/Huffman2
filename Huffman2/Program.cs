using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Huffman3
{
    class Program
    {

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
            string huffExt = args[0].Substring(args[0].Length - 5, 5);
            string prefix = args[0].Substring(0, args[0].Length - 5);
            if ((huffExt != ".huff") || (prefix == ""))
            {
                Console.WriteLine("Argument Error");
                return;
            }
            HuffmanLoader huffmanLoader;
            try
            {
                huffmanLoader = new HuffmanLoader(new FileStream(args[0], FileMode.Open));
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                return;
            }

            if (!huffmanLoader.CheckHeader())
            {
                Console.WriteLine("File Error");
                return;
            }

            TreeLoader treeLoader = new TreeLoader(huffmanLoader);
            if (!treeLoader.Build())
            {
                Console.WriteLine("File Error");
                return;
            }
            DataLoader dataLoader = new DataLoader(treeLoader, prefix);
            dataLoader.LoadData();
            dataLoader.WriteToFile(0, true);
        }
    }
}
