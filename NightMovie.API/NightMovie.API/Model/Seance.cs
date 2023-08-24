namespace NightMovie.API.Model
{
    public class Seance
    {
        public string? Id { get; set; }

        public IEnumerable<User>? Users {get;set;}

        public Film? Film { get; set;}



    }
}
