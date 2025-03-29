namespace NewWithApi.Helpers.filter
{
    public class APIResault<T>
    {
        public string Massage { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Data { get; set; }
    }
}