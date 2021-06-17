namespace WhaleApp.GameServer.Models
{
    public class Player
    {
        public Player(User user, int health)
        {
            User = user;
            Health = health;
        }
        
        public User User { get; private set; }

        public int Health { get; set; }

        public void SendMessage(string message)
        {
            User.SendMessage(message);
        }

        public override string ToString()
         => $"Name: {User.Name} | Health: {Health}";
    }
}