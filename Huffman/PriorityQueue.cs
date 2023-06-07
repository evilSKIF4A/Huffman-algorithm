using System.Collections.Generic;

namespace Huffman
{
    internal class PriorityQueue<Node>
    {
        private int size; // размер очереди
        SortedDictionary<int, Queue<Node>> storage; // отсортированный словарь (приоритет - очередь)
        public PriorityQueue()
        {
            storage = new SortedDictionary<int, Queue<Node>>();
            size = 0;

        }

        public int getSizeQueue() => size; // получить размер очереди

        public void Enqueue(int priority, Node item)
        {
            // Добавление элемент в очередь
            if (!storage.ContainsKey(priority)) // если ключа нет в словаре, то мы создаем очередь с таким приоритетом(ключём)
                storage.Add(priority, new Queue<Node>());
            storage[priority].Enqueue(item);
            size++;
        }

        public Node Dequeue()
        {
            // извлекаем элемент из очереди
            if (size == 0) throw new System.Exception("Queue is empty");
            size--;
            foreach(Queue<Node> q in storage.Values)
                if(q.Count > 0) return q.Dequeue();
            throw new System.Exception("Error");
        }
    }
}