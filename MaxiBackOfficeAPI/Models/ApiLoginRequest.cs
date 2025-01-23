namespace MaxiBackOfficeAPI.Models
{
    public class ApiLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int IdUser { get; set; }
        public string SessionGuid { get; set; }
    }
}
