using System;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Huffman huffman = new Huffman();
            //huffman.CompressFile("data.txt", "arch_data.bin");
            //huffman.DecompressFile("arch_data.bin", "data_dearch.txt");

            //huffman.CompressFile("cat.jpg", "archJPG.bin");
            //huffman.DecompressFile("archJPG.bin", "DeArchJPG.jpg");

            huffman.CompressFile("data_long.txt", "arch_data_long.bin");
            huffman.DecompressFile("arch_data_long.bin", "data_long_dearch.txt");
        }
    }
}
