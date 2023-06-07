using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    internal class Huffman
    {
        public void CompressFile(string dataFileName, string archFileName)
        {
            byte[] data = File.ReadAllBytes(dataFileName);
            byte[] arch = CompressBytes(data);
            File.WriteAllBytes(archFileName, arch);
        }

        private byte[] CompressBytes(byte[] data)
        {
            int[] freqs = CalculateFreqs(data); // таблица частот
            Node root = CreateHuffmanTree(freqs);
            string[] codes = CreateHuffmanCode(root); // можно оптимизировать
            byte[] bits = Compress(data, codes);
            byte[] head = CreateHeader(data.Length, freqs);
            return head.Concat(bits).ToArray();
        }

        private byte[] CreateHeader(int dataLength, int[] freqs)
        {
            // создание заголовка
            List<byte> head = new List<byte>();
            head.Add((byte)(dataLength & 255));
            head.Add((byte)(dataLength >> 8 & 255));
            head.Add((byte)(dataLength >> 16 & 255));
            head.Add((byte)(dataLength >> 24 & 255)); // первые 4 байта это размер текста
            for (int i = 0; i < 256; ++i)
                head.Add((byte)freqs[i]); // записываю таблицу частот
            return head.ToArray();
        }

        private byte[] Compress(byte[] data, string[] codes)
        {
            // сжатие данных
            List<byte> bits = new List<byte>();
            byte sum = 0;
            byte bit = 1;
            foreach(byte symbol in data)
                foreach(char ch in codes[symbol])
                {
                    if (ch == '1')
                        sum |= bit; // операция дизъюнкции
                    if (bit < 128) // сдвигаю на один байт влево (2, 4, 8, 16, 32, ..., 128) 
                        bit <<= 1;
                    else
                    {
                        bits.Add(sum); // сформированный бит кладем в массив
                        sum = 0;
                        bit = 1;
                    }
                }
            if (bit > 1)
                bits.Add(sum);
            return bits.ToArray();
        }

        private string[] CreateHuffmanCode(Node root)
        {
            string[] codes = new string[256];
            HuffmanCode(root, "");
            return codes;

            void HuffmanCode(Node node, string code)
            {
                if (node.leftNode == null && node.rightNode == null) // если это лист
                    codes[node.symbol] = code;
                else
                {
                    HuffmanCode(node.leftNode, code + "0");
                    HuffmanCode(node.rightNode, code + "1");
                }
            }
        }

        private int[] CalculateFreqs(byte[] data)
        {
            int[] freqs = new int[256];
            foreach (byte bit in data)
                freqs[bit]++;
            NormalizeFreqs();
            return freqs;

            void NormalizeFreqs()
            {
                int max = freqs.Max();
                if (max <= 255) return;
                for (int i = 0; i < 256; ++i)
                    if (freqs[i] > 0)
                        freqs[i] = 1 + freqs[i] * 255 / (max + 1);
            }
        }
        private Node CreateHuffmanTree(int[] freqs)
        {
            PriorityQueue<Node> pq = new PriorityQueue<Node>(); // очередь с приоритетом
            for(int i = 0; i < 256; i++)
                if (freqs[i] > 0)
                    pq.Enqueue(freqs[i], new Node((byte)i, freqs[i]));
            while(pq.getSizeQueue() > 1)
            {
                Node leftNode = pq.Dequeue(); // будет с 0 битом
                Node rightNode = pq.Dequeue(); // будет с 1 битом
                int newFreq = leftNode.freq + rightNode.freq; // новая частота
                Node newNode = new Node(leftNode, rightNode, newFreq); // новый узел
                pq.Enqueue(newFreq, newNode); // добавляем новые данные в очередь

            }
            return pq.Dequeue(); // возвращаем последний элемент в очереди. Это будет корень.
        }


        // -------------------- Decompress ----------------------------------


        internal void DecompressFile(string archFileName, string dataFileName)
        {
            byte[] arch = File.ReadAllBytes(archFileName);
            byte[] data = DecompressBytes(arch);
            File.WriteAllBytes(dataFileName, data);
        }

        private byte[] DecompressBytes(byte[] arch)
        {
            ParseHeader(arch, out int dataLength, out int startIndex, out int[] freqs); // читаем заголовок
            Node root = CreateHuffmanTree(freqs);
            byte[] data = Decompress(arch, startIndex, dataLength, root); // архив, откуда начинать, количество символом, чтобы остановиться вовремя, дерево
            return data;
        }

        private byte[] Decompress(byte[] arch, int startIndex, int dataLength, Node root)
        {
            int size = 0;
            Node currentNode = root;
            List<byte> data = new List<byte>();
            for(int i = startIndex; i < arch.Length; ++i)
                for(int bit = 1; bit <= 128; bit <<= 1)
                {
                    bool zero = (arch[i] & bit) == 0;
                    if (zero)
                        currentNode = currentNode.leftNode;
                    else
                        currentNode = currentNode.rightNode;
                    if (currentNode.leftNode != null && currentNode.rightNode != null)
                        continue;
                    if(size++ < dataLength)    
                        data.Add(currentNode.symbol);
                    currentNode = root;
                }
            return data.ToArray();
        }

        private void ParseHeader(byte[] arch, out int dataLength, out int startIndex, out int[] freqs)
        {
            dataLength = arch[0] | arch[1] << 8 | arch[2] << 16 | arch[3] << 24;
            freqs = new int[256];
            for (int i = 0; i < 256; i++)
                freqs[i] = arch[4 + i];
            startIndex = 4 + 256;
        }
    }
}
