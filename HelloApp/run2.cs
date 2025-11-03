using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<string> Solve(Dictionary<string, List<string>> edges)
    {
        return BFS(edges);
    }

    static void Main()
    {
        var edges = new Dictionary<string, List<string>>();
        string line;

        while ((line = Console.ReadLine()) != null && line != "")
        {
            line = line.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                var parts = line.Split('-');
                if (parts.Length == 2)
                {
                    if(!edges.ContainsKey(parts[0]))
                        edges[parts[0]] = new List<string>();
                    edges[parts[0]].Add(parts[1]);
                    if(!edges.ContainsKey(parts[1]))
                        edges[parts[1]] = new List<string>();
                    edges[parts[1]].Add(parts[0]);
                }
            }
        }

        var result = Solve(edges);
        foreach (var edge in result)
        {
            Console.WriteLine(edge);
        }
    }

    public static List<string> BFS(Dictionary<string, List<string>> graph)
    {
        var start = "a";
        var queue = new Queue<string>();
        var result = new List<string>();
        var visited = new HashSet<string>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if(visited.Contains(node))
                continue;
            visited.Add(node);
            var neighbours = graph[node];
            foreach (var neighbour in neighbours.OrderBy(v => v))
            {
                if (neighbour.Equals(neighbour.ToUpper()))
                {
                    result.Add($"{neighbour}-{node}");
                }
                else
                {
                    queue.Enqueue(neighbour);
                }
            }
        }
        return result;
    }
}
