using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<string> Solve(List<(string, string)> edges)
    {
        var graph = new Dictionary<string, HashSet<string>>();
        foreach (var (a, b) in edges)
        {
            if (!graph.ContainsKey(a)) graph[a] = new HashSet<string>();
            if (!graph.ContainsKey(b)) graph[b] = new HashSet<string>();
            graph[a].Add(b);
            graph[b].Add(a);
        }

        var virus = "a";
        var result = new List<string>();

        while (true)
        {
            var paths = BFS(graph, virus);
            if (paths.Count == 0) break;

            var cut = paths.OrderBy(p => $"{p.gateway}-{p.prev}").First();
            graph[cut.gateway].Remove(cut.prev);
            graph[cut.prev].Remove(cut.gateway);
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

    static List<(string gateway, string prev, string next)> BFS(Dictionary<string, HashSet<string>> graph, string start)
    {
        var queue = new Queue<string>();
        var parent = new Dictionary<string, string>();
        var distance = new Dictionary<string, int>();
        var gateways = new List<(string gateway, string prev, string next)>();
        var minDistance = int.MaxValue;

        queue.Enqueue(start);
        distance[start] = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (distance[node] > minDistance) break;

            foreach (var neighbour in graph[node].OrderBy(v => v))
            {
                if (distance.ContainsKey(neighbour)) continue;
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
                        var nextStep = GetNextStep(parent, start, neighbour);
                        gateways.Add((neighbour, parent[neighbour], nextStep));
                    }
                }
                else
                {
                    queue.Enqueue(neighbour);
                }
            }
        }

        return gateways;
    }

    static string GetNextStep(Dictionary<string, string> parent, string start, string node)
    {
        var prev = node;
        while (parent[prev] != start)
            prev = parent[prev];
        return prev;
    }

    static void Main()
    {
        var edges = new List<(string, string)>();
        string line;

        while ((line = Console.ReadLine()) != null)
        {
            line = line.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                var parts = line.Split('-');
                if (parts.Length == 2)
                    edges.Add((parts[0], parts[1]));
            }
        }

        var result = Solve(edges);
        foreach (var edge in result)
            Console.WriteLine(edge);
    }
}
