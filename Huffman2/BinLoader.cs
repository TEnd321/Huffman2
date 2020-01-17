using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman2
{
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

			while (bytesRead != 0)
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
}
