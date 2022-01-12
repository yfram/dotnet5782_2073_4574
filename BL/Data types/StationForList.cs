namespace BO
{
    public class StationForList
    {
        public StationForList()
        {
        }

        public StationForList(int id, string name, int amountOfEmptyPorts, int amountOfFullPorts)
        {
            Id = id;
            Name = name;
            AmountOfEmptyPorts = amountOfEmptyPorts;
            AmountOfFullPorts = amountOfFullPorts;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int AmountOfEmptyPorts { get; set; }
        public int AmountOfFullPorts { get; set; }
        public bool HasEmptyPorts => AmountOfEmptyPorts != 0;

        public override string ToString()
        {
            return $"Id: {Id}\nName: {Name}";
        }
    }
}
