namespace ApiTemplate.Dtos
{
    public class JWTSetting
    {
        public string securitykey { get; set; }
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
    }
}
