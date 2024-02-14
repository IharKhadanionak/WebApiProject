namespace WebApi.Models
{
    public class Filter
    {
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;
        public string Search { get; set; } = "string";
    }
}
