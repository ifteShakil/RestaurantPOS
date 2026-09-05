using RestaurantPOS.Shared.DTOs.User;

namespace RestaurantPOS.Web.Services;

public class AuthState
{
    public string? Token { get; private set; }
    public UserDto? CurrentUser { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Token) && CurrentUser != null;

    public event Action? OnChange;

    public void SetAuth(string token, UserDto user)
    {
        Token = token;
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void Clear()
    {
        Token = null;
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
