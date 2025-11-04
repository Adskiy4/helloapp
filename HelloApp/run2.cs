using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<string> Solve(Dictionary<string, List<string>> edges)
    {
        var start = "a";
        var result = new List<string>();
        var count = 0;
        while (true)
        {
            var paths = BFS(edges, start);
            if(paths.Count == 0)
                break;
            var path = paths
                .OrderBy(p => p.Item1)
                .ThenBy(p => p.Item2.Count > 1 ? p.Item2[1] : p.Item2[0])
                .First();
            if (count != 0 && path.Item2.Count > 1)
                start = path.Item2[1];
            var concatNode = path.Item2[^1];
            DeleteEdge(edges, concatNode, path.Item1);
            result.Add($"{path.Item1}-{concatNode}");
            count++;
        }
        return result;
    }

    public static void DeleteEdge(Dictionary<string, List<string>> edges, string from, string to)
    {
        edges[from].Remove(to);
        edges[to].Remove(from);
    }

    static void Main()
    {
        var graph = new Dictionary<string, List<string>>();
        string line;

        while ((line = Console.ReadLine()) != null && line != "")
        {
            line = line.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                var parts = line.Split('-');
                if (parts.Length == 2)
                {
                    if(!graph.ContainsKey(parts[0]))
                        graph[parts[0]] = new List<string>();
                    graph[parts[0]].Add(parts[1]);
                    if(!graph.ContainsKey(parts[1]))
                        graph[parts[1]] = new List<string>();
                    graph[parts[1]].Add(parts[0]);
                }
            }
        }

        var result = Solve(graph);
        foreach (var edge in result)
        {
            Console.WriteLine(edge);
        }
    }

    public static List<(string, List<string>)> BFS(Dictionary<string, List<string>> graph, string start)
    {
        var queue = new Queue<List<string>>();
        var visited = new HashSet<string>();
        queue.Enqueue(new List<string>(){start});
        var paths = new List<(string, List<string>)>();
        var minLength = int.MaxValue;
        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var node = path[^1];
            if ( path.Count > minLength)
                continue;
            visited.Add(node);
            var neighbours = graph[node];
            foreach (var neighbour in neighbours.OrderBy(v => v))
            {
                if(visited.Contains(neighbour))
                    continue;
                if (neighbour.Equals(neighbour.ToUpper()))
                {
                    if(path.Count == minLength)
                        paths.Add((neighbour,path));
                    else if (path.Count < minLength)
                    {
                        minLength = path.Count;
                        paths = new List<(string, List<string>)>();
                        paths.Add((neighbour, path));
                    }
                }
                else
                {
                    var newPath = new List<string>(path) { neighbour };
                    queue.Enqueue(newPath);
                }
            }
        }

        return paths;
    }
}
