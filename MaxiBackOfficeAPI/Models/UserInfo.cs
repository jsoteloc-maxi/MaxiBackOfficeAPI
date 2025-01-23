namespace MaxiBackOfficeAPI.Models
{
    public class UserInfo
    {
        public int IdUser { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }

        public string FrontSessionGuid { get; set; }
    }
}
