namespace Dht22Reader{
    public class Dht22Settings
    {
        public int Pin { get; set; }
        public required string ExecutablePath { get; set;}
        public required int IntervalInMinutes {get; set;}
    }
}
