namespace RestaurantPOS.Web.Services;

public class AppState
{
    public int CurrentBranchId { get; private set; } = 1;
    public string CurrentBranchName { get; private set; } = "Loading...";
    public string CurrentCurrencyCode { get; private set; } = "";

    public event Action? OnChange;

    public void SetBranch(int branchId, string branchName, string currencyCode)
    {
        CurrentBranchId = branchId;
        CurrentBranchName = branchName;
        CurrentCurrencyCode = currencyCode;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

}
