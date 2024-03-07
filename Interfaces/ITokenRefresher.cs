namespace dotnet.Models
{
    public interface ITokenRefresher
    {
        TokenResponse Refresh(RefreshCred refreshCred);
    }
}
