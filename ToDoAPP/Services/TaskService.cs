using System.Text;
using System.Text.Json;
using ToDoAPP.Models;

namespace ToDoAPP.Services;

public class TaskService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    // Получить все задачи
    public async Task<List<TaskItem>> GetAllTasksAsync()
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
    public async Task<TaskItem> AddTaskAsync(TaskItem task)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(task, _jsonOptions), Encoding.UTF8, "application/json");
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
    public async Task<TaskItem> UpdateTaskAsync(long id, TaskItem task)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(task, _jsonOptions), Encoding.UTF8, "application/json");
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
    public async Task DeleteTaskAsync(long id)
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
    public async Task<List<TaskItem>> GetTasksByDateAsync(DateTime date)
    {
        try
        {
            // Форматируем дату в ISO 8601 формате, который ожидает Java LocalDateTime
            // Отправляем дату с временем 00:00:00
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
    public async Task<List<TaskItem>> GetOverdueTasksAsync()
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
}