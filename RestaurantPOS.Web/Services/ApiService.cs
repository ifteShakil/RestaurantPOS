using RestaurantPOS.Shared.DTOs.Branch;
using RestaurantPOS.Shared.DTOs.Dashboard;
using RestaurantPOS.Shared.DTOs.Menu;
using RestaurantPOS.Shared.DTOs.Order;
using RestaurantPOS.Shared.DTOs.Payment;
using RestaurantPOS.Shared.DTOs.Table;
using RestaurantPOS.Shared.DTOs.User;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RestaurantPOS.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly AuthState _authState;

    public ApiService(IHttpClientFactory httpClientFactory, AuthState authState)
    {
        _http = httpClientFactory.CreateClient("RestaurantAPI");
        _authState = authState;
    }

    // ⭐ প্রতিটা কলের আগে এটা কল করে টোকেন হেডারে বসিয়ে নেওয়া হবে
    private void ApplyAuthHeader()
    {
        if (!string.IsNullOrEmpty(_authState.Token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
        else
        {
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }

    // ===== Auth =====
    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
    }

    // Branch
    public async Task<List<BranchDto>?> GetBranchesAsync()
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<List<BranchDto>>("api/branch");
    }

    public async Task<BranchDto?> GetBranchByIdAsync(int id)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<BranchDto>($"api/branch/{id}");
    }

    public async Task<bool> CreateBranchAsync(CreateBranchDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/branch", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBranchAsync(int id, CreateBranchDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/branch/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeactivateBranchAsync(int id)
    {
        ApplyAuthHeader();
        var response = await _http.DeleteAsync($"api/branch/{id}");
        return response.IsSuccessStatusCode;
    }

    // ===== Menu Category =====
    public async Task<List<MenuCategoryDto>?> GetCategoriesAsync()
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<List<MenuCategoryDto>>("api/menucategory");
    }

    public async Task<bool> CreateCategoryAsync(CreateMenuCategoryDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/menucategory", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCategoryAsync(int id, CreateMenuCategoryDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/menucategory/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        ApplyAuthHeader();
        var response = await _http.DeleteAsync($"api/menucategory/{id}");
        return response.IsSuccessStatusCode;
    }

    // Menu
    public async Task<List<MenuItemDto>?> GetMenuByBranchAsync(int branchId)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<List<MenuItemDto>>($"api/menu/branch/{branchId}");
    }


    public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<MenuItemDto>($"api/menu/{id}");
    }

    public async Task<bool> CreateMenuItemAsync(CreateMenuItemDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/menu", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateMenuItemAsync(int id, CreateMenuItemDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/menu/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        ApplyAuthHeader();
        var response = await _http.DeleteAsync($"api/menu/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetBranchPriceAsync(int menuItemId, SetBranchPriceDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync($"api/menu/{menuItemId}/branch-price", dto);
        return response.IsSuccessStatusCode;
    }

    // Table
    public async Task<List<TableDto>?> GetTablesByBranchAsync(int branchId)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<List<TableDto>>($"api/table/branch/{branchId}");
    }


    public async Task<bool> CreateTableAsync(CreateTableDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/table", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTableStatusAsync(int id, string status)
    {
        ApplyAuthHeader();
        var response = await _http.PatchAsJsonAsync($"api/table/{id}/status", new UpdateTableStatusDto { Status = status });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTableAsync(int id)
    {
        ApplyAuthHeader();
        var response = await _http.DeleteAsync($"api/table/{id}");
        return response.IsSuccessStatusCode;
    }

    // ===== User =====
    public async Task<List<UserDto>?> GetUsersByBranchAsync(int branchId)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<List<UserDto>>($"api/user/branch/{branchId}");
    }

    public async Task<bool> CreateUserAsync(CreateUserDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/user", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        ApplyAuthHeader();
        var response = await _http.DeleteAsync($"api/user/{id}");
        return response.IsSuccessStatusCode;
    }

    // Order
    public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/order", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderAsync(int orderId)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<OrderDto>($"api/order/{orderId}");
    }

    public async Task<List<OrderDto>?> GetActiveOrdersAsync(int branchId)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<List<OrderDto>>($"api/order/branch/{branchId}/active");
    }


    // Bill & Payment
    public async Task<BillDto?> GenerateBillAsync(int orderId)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsync($"api/bill/generate/{orderId}", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BillDto>();
    }

    public async Task<PaymentDto?> AddPaymentAsync(CreatePaymentDto dto)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/bill/payment", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentDto>();
    }


    // Dashboard
    public async Task<DashboardSummaryDto?> GetDashboardSummaryAsync(int branchId)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<DashboardSummaryDto>($"api/dashboard/branch/{branchId}/summary");
    }

}


//⚠️ গুরুত্বপূর্ণ: UpdateTableStatusAsync-এ PatchAsJsonAsync ব্যবহার করেছি, যেটা System.Net.Http.Json-এ.NET 7+ থেকে বিল্ট-ইন আছে। 
//    যদি এরর আসে, HttpMethod.Patch দিয়ে ম্যানুয়াল SendAsync করতে হতে পারে — জানাবেন যদি এই সমস্যা হয়।