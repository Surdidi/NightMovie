namespace NightMovie.API.Model
{
    public class Vote
    {
        public int? ID { get; set; }
        public User? User { get;set; }
        public int? Note { get; set; }
    }
}
