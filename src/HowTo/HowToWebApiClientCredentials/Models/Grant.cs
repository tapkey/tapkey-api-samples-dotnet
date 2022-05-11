namespace HowToWebApiClientCredentials.Models
{
    public class Grant
    {
        public string boundLockId { get; set; }
        public Contact contact { get; set; }
        public DateTime? validFrom { get; set; }
        public DateTime? validBefore { get; set; }
    }
}
