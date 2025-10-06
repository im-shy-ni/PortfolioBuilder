
namespace PortfolioBuilder.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
    }
}
