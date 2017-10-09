namespace KarraPath.Pathfinder
{
    public class Node
    {
        public Node(int id, int x, int y, bool w)
        {
            Id = id;
            X = x;
            Y = y;
            Walkable = w;
        }

        public int Id { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public bool Walkable { get; set; }

        public Node Parent { get; set; }

        public int HCost { get; set; }
        public int GCost { get; set; }
        public int FCost { get; set; }
    }
}
