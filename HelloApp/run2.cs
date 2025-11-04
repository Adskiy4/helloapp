using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<string> Solve(Dictionary<string, List<string>> graph)
    {
        string virus = "a";
        var result = new List<string>();

        while (true)
        {
            var paths = BFS(graph, virus);
            if (paths.Count == 0) break;

            var cut = paths
                .OrderBy(p => $"{p.gateway}-{p.prev}")
                .First();

            DeleteEdge(graph, cut.gateway, cut.prev);
            result.Add($"{cut.gateway}-{cut.prev}");

            var pathsAfter = BFS(graph, virus);
            if (pathsAfter.Count == 0) break;

            var next = pathsAfter
                .OrderBy(p => p.gateway)
                .ThenBy(p => p.next)
                .First();

            virus = next.next;
        }

        return result;
    }

    static void DeleteEdge(Dictionary<string, List<string>> edges, string from, string to)
    {
        if (edges.ContainsKey(from)) edges[from].Remove(to);
        if (edges.ContainsKey(to)) edges[to].Remove(from);
    }

    static List<(string gateway, string prev, string next)> BFS(Dictionary<string, List<string>> graph, string start)
    {
        var queue = new Queue<string>();
        var parent = new Dictionary<string, string>();
        var distance = new Dictionary<string, int>();
        var gateways = new List<(string, string, string)>();
        var minDistance = int.MaxValue;

        queue.Enqueue(start);
        distance[start] = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (distance[node] > minDistance) break;

            foreach (var neighbour in graph[node].OrderBy(v => v))
            {
                if (!distance.ContainsKey(neighbour))
                {
                    distance[neighbour] = distance[node] + 1;
                    parent[neighbour] = node;
                    if (char.IsUpper(neighbour[0]))
                    {
                        if (distance[neighbour] < minDistance)
                        {
                            minDistance = distance[neighbour];
                            gateways.Clear();
                        }
                        if (distance[neighbour] == minDistance)
                        {
                            gateways.Add((neighbour, node, GetNextStep(parent, start, node)));
                        }
                    }
                    else
                    {
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }

        return gateways;
    }

    public static string GetNextStep(Dictionary<string, string> parent, string start, string node)
    {
        var path = new Stack<string>();
        while (node != start)
        {
            path.Push(node);
            node = parent[node];
        }
        return path.Count > 0 ? path.Peek() : start;
    }

    static void Main()
    {
        var graph = new Dictionary<string, List<string>>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            var parts = line.Trim().Split('-');
            if (parts.Length == 2)
            {
                if (!graph.ContainsKey(parts[0])) graph[parts[0]] = new List<string>();
                graph[parts[0]].Add(parts[1]);
                if (!graph.ContainsKey(parts[1])) graph[parts[1]] = new List<string>();
                graph[parts[1]].Add(parts[0]);
            }
        }

        var result = Solve(graph);
        foreach (var edge in result)
        {
            Console.WriteLine(edge);
        }
    }
}
