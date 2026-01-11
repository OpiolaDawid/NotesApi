namespace NotesApi
{
    
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public int UserId { get; set; }
    }
}