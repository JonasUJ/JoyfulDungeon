using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomTileType
{
    Blank,
    Block,
    Platform,
    PlatformLeft,
    PlatformRight,
    PlatformSingle,
}

public enum LinkType
{
    Straight,
    Fall,
    Jump,
}

public struct Node : IEquatable<Node>
{
    public Vector2 pos;
    public RoomTileType type;
    public int index;
    public float cost;
    public List<Edge> edges;
    public bool IsWalkable { get => type != RoomTileType.Blank && type != RoomTileType.Block; }
    public Node(Vector2 pos, RoomTileType type, int index)
    {
        this.pos = pos;
        this.type = type;
        this.index = index;
        this.cost = float.PositiveInfinity;
        this.edges = new List<Edge>();
    }
    public override int GetHashCode()
    {
        return pos.GetHashCode() + (int)type + index;
    }
    public bool Equals(Node other)
    {
        return GetHashCode() == other.GetHashCode();
    }
}

public struct JumpInfo
{
    public Vector2 velocity;
    public float time;
    public JumpInfo(Vector2 velocity, float time)
    {
        this.velocity = velocity;
        this.time = time;
    }
    public IEnumerable GetPoints(Vector2 start, int resolution = 10)
    {
        Vector2 prev = start;
        for (int i = 1; i <= resolution; i++)
        {
            float simTime = i / (float)resolution * time;
            Vector2 dis = velocity * simTime + Vector2.up * NavMeshGenerator.Gravity * simTime * simTime / 2f;
            Vector2 cur = start + dis;
            yield return cur;
            prev = cur;
        }
    }
}

public struct Edge : IEquatable<Edge>
{
    public Node start;
    public Node end;
    public LinkType type;
    public JumpInfo jump;
    public float cost
    {
        get {
            if (type == LinkType.Jump)
                return (end.pos - start.pos).magnitude * 2;
            else
                return (end.pos - start.pos).magnitude;
        }
    }
    public Edge(Node start, Node end, LinkType type)
    {
        this.start = start;
        this.end = end;
        this.type = type;
        this.jump = new JumpInfo();
    }
    public Edge(Node start, Node end, JumpInfo jump)
    {
        this.start = start;
        this.end = end;
        this.type = LinkType.Jump;
        this.jump = jump;
    }
    public void DrawPath(Vector2 offset)
    {
        if (type == LinkType.Jump)
        {
            Vector2 prev = start.pos;
            foreach (Vector2 point in jump.GetPoints(start.pos))
            {
                Gizmos.DrawLine(prev + offset, point + offset);
                prev = point;
            }
        }
        else
            Gizmos.DrawLine(start.pos + offset, end.pos + offset);
    }
    public override int GetHashCode()
    {
        return start.GetHashCode() + end.GetHashCode() + (int)type;
    }
    public bool Equals(Edge other)
    {
        return GetHashCode() == other.GetHashCode();
    }
}


public static class NavMeshGenerator
{
    public static float Gravity = -25f;
    public static Vector2 MaxJumpVelocity = new Vector2(6f, 24f);
    public static Vector2 Offset;

    public static Node[,] GenerateNavMesh(bool[,] map)
    {
        Node[,] nodes = GetNodes(map);
        SetEdges(ref nodes);
        return nodes;
    }

    public static Node[,] GetNodes(bool[,] map)
    {
        int index = 0;
        Node[,] nodes = new Node[map.GetLength(0), map.GetLength(1)];
        for (int y = 0; y < map.GetLength(1); y++)
            for (int x = 0; x < map.GetLength(0); x++)
                if (map[x, y])
                    nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.Block, 0);
                else if (mapGet(map, x, y - 1, false)) // down
                    if (mapGet(map, x - 1, y - 1, false) && mapGet(map, x + 1, y - 1, false)) // down left and down right
                        if (mapGet(map, x - 1, y, false)) // left
                            if (mapGet(map, x + 1, y, false)) // right
                                nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.PlatformSingle, ++index);
                            else
                                nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.PlatformLeft, ++index);
                        else if (mapGet(map, x + 1, y, false)) // left
                            nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.PlatformRight, index);
                        else
                            nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.Platform, index);
                    else if (mapGet(map, x - 1, y, false) || !mapGet(map, x - 1, y - 1, false)) // left or not down left
                        if (mapGet(map, x + 1, y, false) || !mapGet(map, x + 1, y - 1, false)) // right or not down right
                            nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.PlatformSingle, ++index);
                        else
                            nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.PlatformLeft, ++index);
                    else
                        nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.PlatformRight, index);
                else
                    nodes[x, y] = new Node(new Vector2(x, y), RoomTileType.Blank, 0);
        return nodes;
    }

    static T mapGet<T>(T[,] map, int x, int y, T none)
    {
        if (x < 0 || x > map.GetLength(0) - 1 ||
            y < 0 || y > map.GetLength(1) - 1)
            return none;
        return map[x,y];
    }

    public static void SetEdges(ref Node[,] nodes)
    {
        for (int y = 0; y < nodes.GetLength(1); y++)
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            Node cur = nodes[x, y];
            if (cur.IsWalkable)
            {
                Node left = mapGet(nodes, x - 1, y, new Node()); // left
                Node right = mapGet(nodes, x + 1, y, new Node()); // right
                if (cur.type == RoomTileType.Platform)
                {
                    cur.edges.Add(new Edge(cur, left, LinkType.Straight));
                    cur.edges.Add(new Edge(cur, right, LinkType.Straight));
                }
                else
                {
                    if (cur.type == RoomTileType.PlatformLeft)
                    {
                        cur.edges.Add(new Edge(cur, right, LinkType.Straight));
                        Node? fall = TopOfColumn(nodes, x - 1, y);
                        if (fall != null)
                            cur.edges.Add(new Edge(cur, (Node)fall, LinkType.Fall));

                    }
                    else if (cur.type == RoomTileType.PlatformRight)
                    {
                        cur.edges.Add(new Edge(cur, left, LinkType.Straight));
                        Node? fall = TopOfColumn(nodes, x + 1, y);
                        if (fall != null)
                            cur.edges.Add(new Edge(cur, (Node)fall, LinkType.Fall));
                    }
                    else if (cur.type == RoomTileType.PlatformSingle)
                    {
                        Node? fall = TopOfColumn(nodes, x - 1, y);
                        if (fall != null)
                            cur.edges.Add(new Edge(cur, (Node)fall, LinkType.Fall));
                        fall = TopOfColumn(nodes, x + 1, y);
                        if (fall != null)
                            cur.edges.Add(new Edge(cur, (Node)fall, LinkType.Fall));
                    }
                }

            }
        }

        foreach (Node start in nodes)
        {
            if (!start.IsWalkable) continue;
            foreach (Node end in nodes)
            {
                if (start.Equals(end) || !end.IsWalkable || start.index == end.index) continue;
                JumpInfo? jump = GetJump(start, end);
                if (jump != null)
                {
                    bool discard = false;
                    foreach (Vector2 point in ((JumpInfo)jump).GetPoints(start.pos))
                    if (HasCollision(nodes, point))
                    {
                        discard = true;
                        break;
                    }
                    if (!discard)
                        start.edges.Add(new Edge(start, end, (JumpInfo)jump));
                }
            }
        }
    }

    static Node? TopOfColumn(Node[,] nodes, int x, int y)
    {
        Node? node = nodes.Cast<Node>()
                      .Where(n => n.IsWalkable && n.pos.x == x && n.pos.y < y)
                      .OrderByDescending(n => n.pos.y)
                      .FirstOrDefault();
        return node.Equals(default(Node)) ? null : node;
    }

    static JumpInfo? GetJump(Node start, Node end)
    {
        Vector2 dis = end.pos - start.pos;
        float h = Mathf.Max(dis.y * 1.5f, 1);
        float time = Mathf.Sqrt(-2 * h / Gravity) + Mathf.Sqrt(2 * (dis.y - h) / Gravity);

        Vector2 vel = new Vector2(
            dis.x / time,
            Mathf.Sqrt(-2 * Gravity * h));

        return Mathf.Abs(vel.x) < MaxJumpVelocity.x && Mathf.Abs(vel.y) < MaxJumpVelocity.y ? (JumpInfo?)new JumpInfo(vel, time) : null;
    }

    static bool HasCollision(Node[,] nodes, Vector2 point)
    {
        return nodes[Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y)].type == RoomTileType.Block ||
               nodes[Mathf.CeilToInt(point.x), Mathf.FloorToInt(point.y)].type == RoomTileType.Block ||
               nodes[Mathf.FloorToInt(point.x), Mathf.CeilToInt(point.y)].type == RoomTileType.Block ||
               nodes[Mathf.CeilToInt(point.x), Mathf.CeilToInt(point.y)].type == RoomTileType.Block;
    }
}
