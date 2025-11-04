using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Program
{
    static List<string> Solve(Dictionary<string, HashSet<string>> graph)
    {
        var result = new List<string>();
        var visitedStates = new HashSet<string>();
        var virusStart = "a";

        DFS(graph, virusStart, result, visitedStates);
        return result;
    }

    public static bool DFS(Dictionary<string, HashSet<string>> graph, string virusNode, List<string> result, HashSet<string> visitedStates)
    {
        var state = SerializeGraph(graph, virusNode);
        if (visitedStates.Contains(state))
            return false;

        var gatewayEdges = GetGatewayEdges(graph);
        if (gatewayEdges.Count == 0)
            return true;

        foreach (var (gateway, neighbor) in gatewayEdges)
        {
            RemoveEdge(graph, gateway, neighbor);

            var target = BFS(graph, virusNode);
            var failed = false;
            var nextVirusNode = virusNode;

            if (target != null)
            {
                if (target.Value.dist == 1)
                    failed = true;
                else
                    nextVirusNode = target.Value.nextNode;
            }

            if (!failed)
            {
                result.Add($"{gateway}-{neighbor}");
                if (DFS(graph, nextVirusNode, result, visitedStates))
                    return true;
                result.RemoveAt(result.Count - 1);
            }

            AddEdge(graph, gateway, neighbor);
        }

        visitedStates.Add(state);
        return false;
    }

    public static (string gateway, string nextNode, int dist)? BFS(Dictionary<string, HashSet<string>> graph, string start)
    {
        var queue = new Queue<string>();
        var distance = new Dictionary<string, int>();
        var parent = new Dictionary<string, string>();
        var gateways = new List<string>();
        var minDist = int.MaxValue;

        queue.Enqueue(start);
        distance[start] = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (!graph.TryGetValue(node, out var neighbors)) continue;
            if (distance[node] > minDist) continue;

            foreach (var neighbor in neighbors.OrderBy(n => n))
            {
                if (distance.ContainsKey(neighbor)) 
                    continue;

                distance[neighbor] = distance[node] + 1;
                parent[neighbor] = node;

                if (char.IsUpper(neighbor[0]))
                {
                    if (distance[neighbor] < minDist)
                    {
                        minDist = distance[neighbor];
                        gateways.Clear();
                    }
                    if (distance[neighbor] == minDist)
                        gateways.Add(neighbor);
                }
                else
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (gateways.Count == 0) 
            return null;

        gateways.Sort();
        var chosenGateway = gateways[0];
        var path = new Stack<string>();
        var current = chosenGateway;

        while (current != start)
        {
            path.Push(current);
            current = parent[current];
        }

        var nextNode = path.Count > 0 ? path.Pop() : start;
        return (chosenGateway, nextNode, minDist);
    }


    public static List<(string gateway, string node)> GetGatewayEdges(Dictionary<string, HashSet<string>> graph)
    {
        var edges = new List<(string, string)>();
        foreach (var (node, neighbors) in graph)
        {
            if (!char.IsUpper(node[0])) continue;
            foreach (var neighbor in neighbors)
                edges.Add((node, neighbor));
        }

        edges = edges
            .OrderBy(e => e.Item1)
            .ThenBy(e => e.Item2)
            .ToList();

        return edges;
    }

    public static void RemoveEdge(Dictionary<string, HashSet<string>> graph, string a, string b)
    {
        graph[a].Remove(b);
        graph[b].Remove(a);
    }

    public static void AddEdge(Dictionary<string, HashSet<string>> graph, string a, string b)
    {
        graph[a].Add(b);
        graph[b].Add(a);
    }

    public static string SerializeGraph(Dictionary<string, HashSet<string>> graph, string virus)
    {
        var builder = new StringBuilder();
        builder.Append(virus).Append('|');
        var edges = new List<string>();

        foreach (var (node, neighbors) in graph)
        {
            if (!char.IsUpper(node[0])) 
                continue;
            foreach (var neighbor in neighbors)
                edges.Add($"{node}-{neighbor}");
        }

        edges.Sort();
        foreach (var edge in edges)
            builder.Append(edge);

        return builder.ToString();
    }

    static void Main()
    {
        
        var graph = new Dictionary<string, HashSet<string>>();
        string line;
        
        while ((line = Console.ReadLine()) != null && line != "")
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var parts = line.Split('-');
            if (parts.Length == 2)
            {
                if (!graph.ContainsKey(parts[0])) 
                    graph[parts[0]] = new HashSet<string>();
                if (!graph.ContainsKey(parts[1])) 
                    graph[parts[1]] = new HashSet<string>();
                graph[parts[0]].Add(parts[1]);
                graph[parts[1]].Add(parts[0]);
                
            }
        }

        var res = Solve(graph);
        foreach (var edge in res)
        {
            Console.WriteLine(edge);
        }
    }
}
