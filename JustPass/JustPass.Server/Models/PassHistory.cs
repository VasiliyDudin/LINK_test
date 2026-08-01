namespace JustPass.Server.Models
{
    public class PassHistory
    {
        public int ID { get; set; }
        public string Pwd { get; set; }
        public int SafetyPwd { get; set; }
        public DateTime DateGenerated { get; set; }
    }
}
