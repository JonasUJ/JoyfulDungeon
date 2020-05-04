using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathingAI : MonoBehaviour
{
    public bool Draw = false;
    public bool DestinationReached { get; private set; } = true;
    Stack<Edge> Path;
    Node PrevEnd;

    public Edge NextEdge()
    {
        Edge next = Path.Pop();
        DestinationReached = Path.Count == 0;
        return next;
    }

    public void FindPath(Vector2 source, Vector2 dest, bool force = false)
    {
        Node end = GameController.Nodes.Cast<Node>()
                                       .Where(n => n.IsWalkable)
                                       .OrderBy(n => Vector2.Distance(n.pos + NavMeshGenerator.Offset, dest))
                                       .First();

        if (!force && end.Equals(PrevEnd))
            return;

        Node start = GameController.Nodes.Cast<Node>()
                                         .Where(n => n.IsWalkable)
                                         .OrderBy(n => Vector2.Distance(n.pos + NavMeshGenerator.Offset, source))
                                         .First();
        Path = Dijkstra(GameController.Nodes, start, end);
        DestinationReached = Path.Count == 0;
        PrevEnd = end;
    }

    Stack<Edge> Dijkstra(Node[,] nodes, Node source, Node dest)
    {
        HashSet<Node> unvisited = new HashSet<Node>(nodes.Cast<Node>());
        Dictionary<Edge, Edge> prev = new Dictionary<Edge, Edge>();
        source.cost = 0f;
        (Node, Edge) cur = (source, new Edge());
        List<(Node, Edge)> pri = new List<(Node, Edge)> { cur };
        bool found = false;

        while (pri.Count != 0)
        {
            cur = pri.OrderBy(n => n.Item1.cost).First();
            pri.Remove(cur);
            if (cur.Item1.pos == dest.pos)
            {
                found = true;
                break;
            }
            if (!unvisited.Remove(cur.Item1))
                continue;
            for (int i = 0; i < cur.Item1.edges.Count; i++)
            {
                Edge edge = cur.Item1.edges[i];
                if (!unvisited.Contains(edge.end))
                    continue;

                float alt;
                if (edge.end.cost == float.PositiveInfinity)
                    alt = cur.Item1.cost + edge.cost;
                else
                    alt = cur.Item1.cost + edge.cost + edge.end.cost;

                if (alt < edge.end.cost)
                {
                    edge.end.cost = alt;
                    prev[edge] = cur.Item2;
                }

                pri.Add((edge.end, edge));
            }
        }

        Stack<Edge> path = new Stack<Edge>();
        if (found)
        {
            Edge u = cur.Item2;
            while(prev.ContainsKey(u) && !path.Contains(u))
            {
                path.Push(u);
                u = prev[u];
            }
        }

        return path;
    }

    void OnDrawGizmos() {
        if (Path == null || Path.Count == 0 || !Draw) return;
        foreach (Edge u in Path)
        {
            u.DrawPath(NavMeshGenerator.Offset);
        }
    }
}
