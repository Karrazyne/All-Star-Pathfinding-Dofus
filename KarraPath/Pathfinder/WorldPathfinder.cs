using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Map = KarraPath.D2PReader.D2p.Map;

namespace KarraPath.Pathfinder
{
    public class WorldPathfinder
    {
        private Node[] CellPos { get; }

        private Map Map { get; }

        private MainUc MainUc { get; }

        public WorldPathfinder(Map map, MainUc m)
        {
            CellPos = new Node[560];
            Map = map;
            MainUc = m;
            InitGrid();
        }

        private void InitGrid()
        {
            int loc2, loc3;
            int loc5;
            var loc1 = loc2 = loc3 = loc5 = 0;
            while (loc5 < Constant.MapHeight)
            {
                var loc4 = 0;
                while (loc4 < Constant.MapWidth)
                {
                    var tmpCell = Map.Cells[loc3];
                    CellPos[loc3] = new Node(loc3, loc1 + loc4, loc2 + loc4, tmpCell.Mov);
                    loc3++;
                    loc4++;
                }
                loc1++;
                loc4 = 0;
                while (loc4 < Constant.MapWidth)
                {
                    var tmpCell = Map.Cells[loc3];
                    CellPos[loc3] = new Node(loc3, loc1 + loc4, loc2 + loc4, tmpCell.Mov);
                    loc3++;
                    loc4++;
                }
                loc2--;
                loc5++;
            }
        }

        public bool GetPath(int startPos, int targetPos, bool useDiag)
        {

            var sw = new Stopwatch();

            var startNode = CellPos[startPos];
            var targetNode= CellPos[targetPos];

            var closedSet = new List<Node>();
            var openSet = new List<Node> {startNode};

            while (openSet.Count > 0)
            {
                var index = 0;
                for (var i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < openSet[index].FCost)
                        index = i;

                    if (openSet[i].FCost != openSet[index].FCost) continue;
                    if (openSet[i].GCost > openSet[index].GCost)
                        index = i;

                    if (useDiag) continue;
                    if (openSet[i].GCost == openSet[index].GCost)
                        index = i;
                }

                var current = openSet[index];

                if (current == targetNode)
                {
                    sw.Stop();
                    Console.WriteLine($@"Path find in {sw.ElapsedMilliseconds} ms");
                    RetracePath(startNode, targetNode);
                    return true;
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in GetNeighbours(current, useDiag))
                {
                    if (closedSet.Contains(neighbor) || !neighbor.Walkable) continue;

                    var tempG = current.GCost + GetDistance(neighbor, current, useDiag);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else if (tempG >= neighbor.GCost)
                        continue;

                    neighbor.GCost = tempG;
                    neighbor.HCost = GetDistance(neighbor, targetNode, useDiag);
                    neighbor.FCost = neighbor.GCost + neighbor.HCost;
                    neighbor.Parent = current;
                }

            }

            return false;
        }

        public void RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Add(startNode);

            path.Reverse();

            foreach (var tempCell in path)
            {
                var cell = MainUc.GetCell(tempCell.Id);
                cell.State = CellState.BluePlacement;
            }

            MainUc.Invalidate();
        }

        public List<Node> GetNeighbours(Node node, bool useDiagnonal)
        {
            var neighbours = new List<Node>();

            var rightCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X + 1 && nodec.Y == node.Y);
            var leftCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X - 1 && nodec.Y == node.Y);
            var bottomCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X && nodec.Y == node.Y + 1);
            var topCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X && nodec.Y == node.Y - 1);

            if (rightCell != null)
                neighbours.Add(rightCell);
            if (leftCell != null)
                neighbours.Add(leftCell);
            if (bottomCell != null)
                neighbours.Add(bottomCell);
            if (topCell != null)
                neighbours.Add(topCell);

            if (!useDiagnonal) return neighbours;


            var topLeftCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X - 1 && nodec.Y == node.Y - 1);
            var bottomRightCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X + 1 && nodec.Y == node.Y + 1);
            var bottomLeftCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X - 1 && nodec.Y == node.Y + 1);
            var topRightCell = CellPos.FirstOrDefault(nodec => nodec.X == node.X + 1 && nodec.Y == node.Y - 1);

            if (topLeftCell != null)
                neighbours.Add(topLeftCell);
            if (bottomRightCell != null)
                neighbours.Add(bottomRightCell);
            if (bottomLeftCell != null)
                neighbours.Add(bottomLeftCell);
            if (topRightCell != null)
                neighbours.Add(topRightCell);
            

            return neighbours;
        }

        public int GetDistance(Node a, Node b, bool useDiag)
        {
            if (useDiag)
                return (int) Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        }
    }
}
