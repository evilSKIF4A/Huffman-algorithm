using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    internal class Node
    {
        public byte symbol; // символ
        public int freq; // частота
        public Node leftNode; // левый узел
        public Node rightNode; // правый узел

        public Node(byte symbol, int freq)
        {
            // конструктор для создания листа
            this.symbol = symbol;
            this.freq = freq;
        }

        public Node(Node leftNode, Node rightNode, int freq)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.freq = freq;
        }
    }
}
