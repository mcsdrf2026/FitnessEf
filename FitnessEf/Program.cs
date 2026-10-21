using FitnessEf;
using Microsoft.EntityFrameworkCore;

while (true)
{
    Console.WriteLine("\n--- МЕНЮ УПРАВЛЕНИЯ КЛИЕНТАМИ ---");
    Console.WriteLine("1. Список клиентов");
    Console.WriteLine("2. Найти клиента по ID");
    Console.WriteLine("3. Добавить клиента");
    Console.WriteLine("4. Изменить телефон/статус");
    Console.WriteLine("5. Удалить клиента");
    Console.WriteLine("6. Статистика");
    Console.WriteLine("0. Выход");
    Console.Write("Выбор: ");

    string choice = Console.ReadLine() ?? string.Empty;

    if (choice == "0") break;

    switch (choice)
    {
        case "1": await ListClientsAsync(); break;
        case "2": await FindClientAsync(); break;
        case "3": await AddClientAsync(); break;
        case "4": await UpdateClientAsync(); break;
        case "5": await DeleteClientAsync(); break;
        case "6": await ShowStatisticsAsync(); break;
        default: Console.WriteLine("Неизвестный пункт меню."); break;
    }
}

// 1. Вывод списка клиентов с сортировкой по фамилии и имени
static async Task ListClientsAsync()
{
    try
    {
        await using var db = new FitnessContext();
        var clients = await db.Clients
            .AsNoTracking()
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();

        Console.WriteLine("\n--- Список клиентов ---");
        if (clients.Count == 0) Console.WriteLine("База пуста.");

        foreach (var c in clients)
        {
            Console.WriteLine($"[{c.ClientId}] {c.LastName} {c.FirstName} | Тел: {c.Phone ?? "нет"} | Статус: {c.Status}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при чтении: {ex.Message}");
    }
}

// 2. Поиск клиента по ID
static async Task FindClientAsync()
{
    Console.Write("\nВведите ID клиента: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Неверный формат ID.");
        return;
    }

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client == null)
        Console.WriteLine("Клиент не найден.");
    else
        Console.WriteLine($"Найден: {client.LastName} {client.FirstName}, Тел: {client.Phone ?? "нет"}");
}

// 3. Добавление клиента
static async Task AddClientAsync()
{
    Console.Write("\nВведите Имя: ");
    string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

    Console.Write("Введите Фамилию: ");
    string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Имя и фамилия обязательны!");
        return;
    }

    try
    {
        await using var db = new FitnessContext();
        var client = new Client
        {
            FirstName = firstName,
            LastName = lastName,
            RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
            Status = "active"
        };

        db.Clients.Add(client);
        await db.SaveChangesAsync();
        Console.WriteLine($"Клиент успешно добавлен! Ему присвоен ID: {client.ClientId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка сохранения: {ex.Message}");
    }
}

// 4. Изменение телефона или статуса
static async Task UpdateClientAsync()
{
    Console.Write("\nВведите ID клиента для изменения: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client == null)
    {
        Console.WriteLine("Клиент не найден.");
        return;
    }

    Console.Write($"Текущий телефон ({client.Phone ?? "нет"}). Введите новый (или Enter, чтобы пропустить): ");
    string newPhone = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(newPhone)) client.Phone = newPhone;

    Console.Write($"Текущий статус ({client.Status}). Введите новый (active/inactive) (или Enter, чтобы пропустить): ");
    string newStatus = Console.ReadLine()?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(newStatus)) client.Status = newStatus;

    await db.SaveChangesAsync();
    Console.WriteLine("Данные клиента обновлены.");
}

// 5. Удаление клиента (с обработкой ошибки внешнего ключа)
static async Task DeleteClientAsync()
{
    Console.Write("\nВведите ID клиента для удаления: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    try
    {
        await using var db = new FitnessContext();
        var client = await db.Clients.FindAsync(id);

        if (client == null)
        {
            Console.WriteLine("Клиент не найден.");
            return;
        }

        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        Console.WriteLine("Клиент успешно удалён.");
    }
    catch (DbUpdateException)
    {
        // Это сработает, если у клиента есть посещения (настроено DeleteBehavior.Restrict)
        Console.WriteLine("Ошибка: Нельзя удалить клиента, так как у него есть связанные посещения.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

// 6. Статистика
static async Task ShowStatisticsAsync()
{
    await using var db = new FitnessContext();
    int total = await db.Clients.CountAsync();
    int active = await db.Clients.CountAsync(c => c.Status == "active");

    Console.WriteLine($"\n--- СТАТИСТИКА ---");
    Console.WriteLine($"Всего клиентов: {total}");
    Console.WriteLine($"Активных клиентов: {active}");
}