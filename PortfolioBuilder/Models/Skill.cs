
namespace PortfolioBuilder.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public int? Proficiency { get; set; } // 0-100
    }
}
