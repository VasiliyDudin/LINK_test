namespace JustPass.Server.Models
{
    public class PassResponse
    {
        public string Pwd { get; set; }
        public int LengthPwd { get; set; }
        public int SafetyPwd { get; set; }
        public DateTime DateGenerated { get; set; }
    }
}
