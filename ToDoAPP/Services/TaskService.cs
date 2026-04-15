using System.Text;
using System.Text.Json;
using ToDoAPP.Models;

namespace ToDoAPP.Services;

public class TaskService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    // Токен доступа
    private string? _accessToken;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

    public void Logout()
    {
        _accessToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public TaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    // Установить токен авторизации
    public void SetAccessToken(string token)
    {
        _accessToken = token;
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    // Сохранить токен в localStorage (вызывать из компонента)
    public string? GetAccessToken() => _accessToken;

    // Получить токен через client_credentials
    public async Task<string?> GetAccessToken(string clientId, string clientSecret)
    {
        try
        {
            var request = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var content = new FormUrlEncodedContent(request);
            var response = await _httpClient.PostAsync("/oauth2/token", content);
            Console.WriteLine($"Login response: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login response body: {json}");
                using var doc = JsonDocument.Parse(json);
                var token = doc.RootElement.GetProperty("access_token").GetString();
                SetAccessToken(token!);
                return token;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения токена: {ex.Message}");
        }
        return null;
    }

    // Получить все задачи
    public async Task<List<TaskItem>> GetAllTasks()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tasks");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions) ?? new List<TaskItem>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения всех задач: {ex.Message}");
            return new List<TaskItem>();
        }
    }


    // Добавить задачу
    public async Task<TaskItem> AddTask(TaskItem task)
    {
        try
        {
            // Java ожидает поле "content", не "title"
            var requestBody = new { content = task.Content, completed = task.Completed, dueDate = task.DueDate };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/tasks", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TaskItem>(json, _jsonOptions)!;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при добавлении задачи: {ex.Message}");
            throw;
        }
    }

    // Обновить задачу
    public async Task<TaskItem> UpdateTask(Guid id, TaskItem task)
    {
        try
        {
            var requestBody = new { content = task.Content, completed = task.Completed };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/tasks/{id}", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TaskItem>(json, _jsonOptions)!;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при обновлении задачи: {ex.Message}");
            throw;
        }
    }

    // Удалить задачу
    public async Task DeleteTask(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/tasks/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
            throw;
        }
    }

    // Получить задачи на конкретную дату
    public async Task<List<TaskItem>> GetTasksByDate(DateTime date)
    {
        try
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var dateString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");

            var url = $"/api/tasks/by-date?date={dateString}";
            Console.WriteLine($"Запрос к API: {url}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ API: {json}");
                return JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions) ?? new List<TaskItem>();
            }
            else
            {
                Console.WriteLine($"Ошибка API: {response.StatusCode}");
                return new List<TaskItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в GetTasksByDateAsync: {ex.Message}");
            return new List<TaskItem>();
        }
    }

    // Получить просроченные задачи
    public async Task<List<TaskItem>> GetOverdueTasks()
    {
        try
        {
            var url = "/api/tasks/overdue";
            Console.WriteLine($"Запрос к API: {url}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ API: {json}");
                return JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions) ?? new List<TaskItem>();
            }
            else
            {
                Console.WriteLine($"Ошибка API: {response.StatusCode}");
                return new List<TaskItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в GetOverdueTasksAsync: {ex.Message}");
            return new List<TaskItem>();
        }
    }

    // Регистрация нового клиента
    public async Task<bool> Register(string clientId, string clientSecret)
    {
        try
        {
            var request = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "client_credentials" }
            };

            var content = new FormUrlEncodedContent(request);
            var response = await _httpClient.PostAsync("/api/clients/register", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка регистрации: {ex.Message}");
            return false;
        }
    }
}