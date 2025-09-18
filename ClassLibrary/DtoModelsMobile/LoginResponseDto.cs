namespace ClassLibrary.DtoModelsMobile
{
    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

        public string UserRole { get; set; }
    }
}
