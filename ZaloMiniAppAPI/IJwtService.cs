namespace ZaloMiniAppAPI
{
    public interface IJwtService
    {
        string GenerateJwtToken(int  customerId, bool isAdmin);
        bool ValidateJwtToken(string token);
    }
}
