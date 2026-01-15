namespace AtomicHabits.Models
{
    public class JwtKeys
    {
        public int Id { get; set; }
        public string Key { get; set; } // Base64 encoded
        public DateTime CreatedAt { get; set; }
        public string IsActive { get; set; }
    }

    public class JwtOptions
    {
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int AccessTokenExpiryMinutes { get; set; }
    }
}
