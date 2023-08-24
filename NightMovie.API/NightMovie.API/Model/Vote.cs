using SQLite;
using SQLiteNetExtensions.Attributes;

namespace NightMovie.API.Model
{
    [Table("Vote")]
    public class Vote
    {
        public int? Id { get; set; }
        public User? User { get;set; }
        public int? Note { get; set; }
    }
}
