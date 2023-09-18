namespace Core.Persistence.DAOs
{
    public class UserDAO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int Bonuses { get; set; }
    }
}