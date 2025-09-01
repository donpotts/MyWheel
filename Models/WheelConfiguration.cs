namespace MyWheelApp.Models
{
    public class WheelConfiguration
    {
        public string WheelName { get; set; } = string.Empty;
        public List<WheelItem> Items { get; set; } = new();
    }

    public class WheelItem
    {
        public string Text { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}