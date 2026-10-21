using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

// 4.1. Получение всех клиентов
Console.WriteLine("--- 4.1. Все клиенты ---");
var clients = await db.Clients
    .AsNoTracking()
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var client in clients)
{
    Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName}; статус: {client.Status}");
}

// 4.2. Проекция в анонимный тип
Console.WriteLine("\n--- 4.2. Проекция (ФИО и Телефон) ---");
var projectedClients = await db.Clients
    .AsNoTracking()
    .OrderBy(c => c.LastName)
    .ThenBy(c => c.FirstName)
    .Select(c => new {
        c.ClientId,
        FullName = c.FirstName + " " + c.LastName,
        c.Phone
    })
    .ToListAsync();

foreach (var c in projectedClients)
{
    Console.WriteLine($"{c.ClientId}: {c.FullName}; тел.: {c.Phone ?? "не указан"}");
}

// 4.3. Различия методов поиска
Console.WriteLine("\n--- 4.3. Различия методов ---");
var firstOrNull = await db.Clients.FirstOrDefaultAsync(c => c.FirstName == "Анна");
Console.WriteLine($"FirstOrDefaultAsync: {(firstOrNull != null ? "Найдена Анна" : "Никого не нашли")}");

var singleOrNull = await db.Clients.SingleOrDefaultAsync(c => c.ClientId == 1);
Console.WriteLine($"SingleOrDefaultAsync: {(singleOrNull != null ? "Найден клиент с ID 1" : "Не найден")}");

var foundById = await db.Clients.FindAsync(2);
Console.WriteLine($"FindAsync: {(foundById != null ? $"По ID 2 найден {foundById.FirstName}" : "Не найден")}");