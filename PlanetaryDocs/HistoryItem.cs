using System;

namespace PlanetaryDocs
{
    public class HistoryItem
    {
        public HistoryItem(string location)
        {
            Location = location;
            Refresh();
        }

        public long Ticks { get; private set; }
        public string Location { get; private set; }

        public void Refresh() => Ticks = DateTime.UtcNow.Ticks;

        public override string ToString() => $"{Location} ({Ticks})";

        public override bool Equals(object obj) => 
            obj is HistoryItem hi
            && hi.Location == Location;

        public override int GetHashCode() => Location.GetHashCode();
    }
}
