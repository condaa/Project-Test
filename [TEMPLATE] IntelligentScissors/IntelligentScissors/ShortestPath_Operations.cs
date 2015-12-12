using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentScissors
{



    public static class ShortestPath_Operations
    {
        //private static List<List<int>> All_Paths ;
        /// <summary>
        /// generate list of all shortest paths beween all nodes
        /// </summary>
        /// <param name="Graph"></param>
        /// <returns>list of all shortest paths beween all nodes</returns>
        public static List<List<int>> Generate_All_Paths(List<List<Edge>> Graph)
        {

            int V = Graph.Count; // number of all vertcies/nodes in the graph
            List<List<int>> All_Paths = new List<List<int>>(); // list of all paths
            for (int i = 0; i < V; i++)
            {
                //SP : list of  shortest path beteewn node i and all other nodes
                List<int> SP = new List<int>(); //reset the list
                SP = Dijkstra(Graph, i);

                // add shortest paths list of node i  in it's place in all_Paths list
                All_Paths.Add(SP);
            }

            return All_Paths; //return all paths
        }
       
        /// <summary>
        /// backtracing the shortestpath btween 2 nodes
        /// </summary>
        /// <param name="All_Paths"></param>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        /// <returns>shortestpath</returns>
        public static List<int> GenerateShortestPath(List<List<int>> All_Paths, int Source, int Dest)
         {
             List<int> ShortestPath = new List<int>(); // shortest path bteewn Source node and destination

             Stack<int> RevPath = new Stack<int>();   //the reversed shortest path bteewn Source node and destination   
             
             RevPath.Push(Dest); // push the destination node 
 
             int previous = All_Paths[Source][Dest]; // previous of the destination (current node)

            // backtracking the shortest path from all paths
             while (previous != -1){
               RevPath.Push(previous); // push last node in the path   
               previous = All_Paths[Source][previous]; //previous of current node
             }
            
            //revrese the revesed path 
            while(RevPath.Count!=0){
                ShortestPath.Add(RevPath.Pop());
            }

            // return shortest path bteewn Source node and destination
            return ShortestPath;
         }
        /// <summary>
        /// Dijkstra algorithm
        /// </summary>
        /// <param name="Graph"></param>
        /// <param name="Source"></param>
        /// <returns>list of all shortest paths btween a source node and all nodes </returns>
        private static List<int> Dijkstra(List<List<Edge>> Graph, int Source)
        {
            const double oo = 1000000; // infity value
            //Distance : the minimum cost between the source node and all the others nodes
            //initialized with infinty value
            List<double> Distance = Enumerable.Repeat(oo, Graph.Count).ToList();

            //Previous : saves the previous node that lead to the shortest path from the src node to current node 
            List<int> Previous = Enumerable.Repeat(-1, Graph.Count).ToList();

            // SP between src and it self costs 0 
            Distance[Source] = 0;

            // PeriorityQueue : always return the shortest bath btween src node and specific node  
            PeriorityQueue MinimumDistances = new PeriorityQueue();

            MinimumDistances.Push(new Edge(-1, Source, 0));

            while (!MinimumDistances.IsEmpty())
            {
                // get the shortest path so far 
                Edge CurrentEdge = MinimumDistances.Top();
                MinimumDistances.Pop();

                // check if this SP is vaild (i didn't vist this node with a less cost)
                if (CurrentEdge.Weight > Distance[CurrentEdge.To])
                    continue;

                // save the previous 
                Previous[CurrentEdge.To] = CurrentEdge.From;

                // Relaxing 
                for (int i = 0; i < Graph[CurrentEdge.To].Count; i++)
                {
                    Edge HeldEdge = Graph[CurrentEdge.To][i];
                    // if the relaxed path cost of a neighbour node is less than  it's previous one
                    if (Distance[HeldEdge.To] > Distance[HeldEdge.From] + HeldEdge.Weight)
                    {
                        // set the relaxed cost to Distance  && pash it to the PQ
                        HeldEdge.Weight = Distance[HeldEdge.To] = Distance[HeldEdge.From] + HeldEdge.Weight;
                        MinimumDistances.Push(HeldEdge);
                    }
                }
            }

            return Previous;  // re turn th shortest paths from src to all nodes
        }

    }
    public class PeriorityQueue
    {

        private List<Edge> Heap = new List<Edge>();
        private void swap(int x, int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }
        private int Left(int Node)
        {
            return Node * 2 + 1;
        }
        private int Right(int Node)
        {
            return Node * 2 + 2;
        }
        private int Parent(int Node)
        {
            return (Node - 1) / 2;
        }
        private void SiftUp(int Node)
        {
            if (Node == 0 || Heap[Node].Weight >= Heap[Parent(Node)].Weight)
                return;
            //swap(Heap[Parent(Node)], Heap[Node]);

            Edge temp = Heap[Parent(Node)];
            Heap[Parent(Node)] = Heap[Node];
            Heap[Node] = temp;

            SiftUp(Parent(Node));
        }
        private void SiftDown(int Node)
        {
            if (Left(Node) >= Heap.Count
                || (Left(Node) < Heap.Count && Right(Node) >= Heap.Count && Heap[Left(Node)].Weight >= Heap[Node].Weight)
                || (Left(Node) < Heap.Count && Right(Node) < Heap.Count && Heap[Left(Node)].Weight >= Heap[Node].Weight && Heap[Right(Node)].Weight >= Heap[Node].Weight))
                return;
            if (Right(Node) < Heap.Count && Heap[Right(Node)].Weight <= Heap[Left(Node)].Weight)
            {
                Edge temp = Heap[Right(Node)];
                Heap[Right(Node)] = Heap[Node];
                Heap[Node] = temp;
                SiftDown(Right(Node));
            }
            else
            {
                Edge temp = Heap[Left(Node)];
                Heap[Left(Node)] = Heap[Node];
                Heap[Node] = temp;
                SiftDown(Left(Node));
            }
        }
        public PeriorityQueue()
        {
        }
        public void Push(Edge Node)
        {
            Heap.Add(Node);
            SiftUp(Heap.Count - 1);
        }
        public Edge Pop()
        {
            Edge temp = Heap[0];
            Heap[0] = Heap[Heap.Count - 1];
            Heap.RemoveAt(Heap.Count - 1);
            SiftDown(0);
            return temp;
        }
        public bool IsEmpty()
        {
            if (Heap.Count == 0)
                return true;
            return false;
        }
        public int Size()
        {
            return Heap.Count;
        }
        public Edge Top()
        {
            return Heap[0];
        }
    }



}
