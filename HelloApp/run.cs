// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// class Program
// {
//     static int Solve(List<string> lines)
//     {
//         Graph.RoomDepth = lines.Count > 5 ? 4 : 2;
//
//         var start = Graph.ParseLines(lines);
//         var end = Graph.CreateTarget();
//
//         var queue = new PriorityQueue<Graph, int>();
//         var bestCosts = new Dictionary<Graph, int>();
//
//         queue.Enqueue(start, 0);
//         bestCosts[start] = 0;
//
//         while (queue.Count > 0)
//         {
//             queue.TryDequeue(out var currentGraph, out var currentCost);
//
//             if (currentGraph.Equals(end))
//                 return currentCost;
//
//             foreach (var (nextState, stepCost) in currentGraph.GetNeighbours())
//             {
//                 var newCost = currentCost + stepCost;
//                 if (!bestCosts.TryGetValue(nextState, out var existingCost) || newCost < existingCost)
//                 {
//                     bestCosts[nextState] = newCost;
//                     queue.Enqueue(nextState, newCost);
//                 }
//             }
//         }
//
//         return -1;
//     }
//
//     static void Main()
//     {
//         var lines = new List<string>();
//         string line;
//         while ((line = Console.ReadLine()) != null && line != "")
//             lines.Add(line);
//
//         var minimalEnergy = Solve(lines);
//         Console.WriteLine(minimalEnergy);
//     }
// }
//
// public class Graph
// {
//     public static int RoomCount = 4;
//     public static int RoomDepth = 2;
//     public static int CorridorLength = 11;
//     public static int[] RoomEntries = { 2, 4, 6, 8 };
//     public static int[] CorridorPositions = { 0, 1, 3, 5, 7, 9, 10 };
//
//     public static Dictionary<char, int> CostEnergy = new()
//     {
//         { 'A', 1 },
//         { 'B', 10 }, 
//         { 'C', 100 },
//         { 'D', 1000 }
//     };
//
//     public char[] Corridor;
//     public char[,] Rooms;
//     private readonly int _hash;
//
//     public Graph(char[] corridor, char[,] rooms)
//     {
//         Corridor = (char[])corridor.Clone();
//         Rooms = (char[,])rooms.Clone();
//         _hash = ComputeHash();
//     }
//
//     private int ComputeHash()
//     {
//         var hash = 17;
//         foreach (var c in Corridor)
//             hash = hash * 31 + c;
//         for (var roomIndex = 0; roomIndex < RoomCount; roomIndex++)
//         for (var roomDepthIndex = 0; roomDepthIndex < RoomDepth; roomDepthIndex++)
//             hash = hash * 31 + Rooms[roomIndex, roomDepthIndex];
//         return hash;
//     }
//
//     public override bool Equals(object obj)
//     {
//         if (obj is not Graph other || _hash != other._hash)
//             return false;
//
//         if (!Corridor.SequenceEqual(other.Corridor))
//             return false;
//
//         for (var i = 0; i < RoomCount; i++)
//         for (var j = 0; j < RoomDepth; j++)
//             if (Rooms[i, j] != other.Rooms[i, j])
//                 return false;
//
//         return true;
//     }
//
//     public override int GetHashCode() => _hash;
//
//     public static Graph ParseLines(List<string> lines)
//     {
//         var corridor = lines[1].Substring(1, CorridorLength).ToCharArray();
//         var rooms = new char[RoomCount, RoomDepth];
//
//         for (var roomDepth = 0; roomDepth < RoomDepth; roomDepth++)
//         {
//             var lineIndex = 2 + roomDepth;
//             for (var roomIndex = 0; roomIndex < RoomCount; roomIndex++)
//             {
//                 var position = 3 + 2 * roomIndex;
//                 rooms[roomIndex, roomDepth] = lines[lineIndex][position];
//             }
//         }
//
//         return new Graph(corridor, rooms);
//     }
//
//     public static Graph CreateTarget()
//     {
//         var corridor = new string('.', CorridorLength).ToCharArray();
//         var rooms = new char[RoomCount, RoomDepth];
//
//         for (var i = 0; i < RoomCount; i++)
//         for (var j = 0; j < RoomDepth; j++)
//             rooms[i, j] = (char)('A' + i);
//
//         return new Graph(corridor, rooms);
//     }
//
//     public List<(Graph, int)> GetNeighbours()
//     {
//         var neighbours = new List<(Graph, int)>();
//
//         for (var corridorIndex = 0; corridorIndex < CorridorLength; corridorIndex++)
//         {
//             var item = Corridor[corridorIndex];
//             if (!CostEnergy.ContainsKey(item))
//                 continue;
//
//             var room = item - 'A';
//             var roomEntry = RoomEntries[room];
//
//             var step = roomEntry > corridorIndex ? 1 : -1;
//             var isClear = true;
//             for (var i = corridorIndex + step; i != roomEntry + step; i += step)
//                 if (Corridor[i] != '.')
//                     isClear = false;
//
//             if (!isClear)
//                 continue;
//
//             var canEnter = true;
//             var availableDepth = -1;
//             for (var depth = RoomDepth - 1; depth >= 0; depth--)
//             {
//                 if (Rooms[room, depth] == '.' && availableDepth == -1)
//                     availableDepth = depth;
//                 else if (Rooms[room, depth] != '.' && Rooms[room, depth] != item)
//                     canEnter = false;
//             }
//
//             if (!canEnter || availableDepth == -1)
//                 continue;
//
//             var stepsCount = Math.Abs(corridorIndex - roomEntry) + availableDepth + 1;
//             var cost = stepsCount * CostEnergy[item];
//
//             var newCorridor = (char[])Corridor.Clone();
//             newCorridor[corridorIndex] = '.';
//             var newRooms = (char[,])Rooms.Clone();
//             newRooms[room, availableDepth] = item;
//
//             neighbours.Add((new Graph(newCorridor, newRooms), cost));
//         }
//
//         for (var roomIndex = 0; roomIndex < RoomCount; roomIndex++)
//         {
//             var roomEntry = RoomEntries[roomIndex];
//             var firstDepth = -1;
//             var item = '.';
//
//             for (var depth = 0; depth < RoomDepth; depth++)
//             {
//                 if (Rooms[roomIndex, depth] != '.')
//                 {
//                     firstDepth = depth;
//                     item = Rooms[roomIndex, depth];
//                     break;
//                 }
//             }
//
//             if (firstDepth == -1)
//                 continue;
//
//             var room = item - 'A';
//             if (room == roomIndex)
//             {
//                 var blocking = false;
//                 for (var depth = firstDepth + 1; depth < RoomDepth; depth++)
//                     if (Rooms[roomIndex, depth] != item)
//                     {
//                         blocking = true;
//                     }
//
//                 if (!blocking)
//                 {
//                     continue;
//                 }
//             }
//
//             foreach (var corridorIndex in CorridorPositions)
//             {
//                 var step = corridorIndex > roomEntry ? 1 : -1;
//                 var pathClear = true;
//                 for (var i = roomEntry; i != corridorIndex + step; i += step)
//                     if (Corridor[i] != '.')
//                     {
//                         pathClear = false;
//                     }
//
//                 if (!pathClear)
//                 {
//                     continue;
//                 }
//
//                 var stepsToTarget = Math.Abs(corridorIndex - roomEntry) + firstDepth + 1;
//                 var cost = stepsToTarget * CostEnergy[item];
//
//                 var newCorridor = (char[])Corridor.Clone();
//                 newCorridor[corridorIndex] = item;
//                 var newRooms = (char[,])Rooms.Clone();
//                 newRooms[roomIndex, firstDepth] = '.';
//
//                 neighbours.Add((new Graph(newCorridor, newRooms), cost));
//             }
//         }
//
//         return neighbours;
//     }
// }