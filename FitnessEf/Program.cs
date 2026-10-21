using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

// 9.4. Добавление посещения
Console.WriteLine("--- 9.4. Добавление посещения ---");
int clientId = 1;

// Проверяем, существует ли клиент с таким ID
var clientExists = await db.Clients.AnyAsync(c => c.ClientId == clientId);

if (!clientExists)
{
    Console.WriteLine("Клиента с таким ID нет.");
}
else
{
    // Создаем новое посещение и привязываем к клиенту
    db.Visits.Add(new Visit
    {
        ClientId = clientId,
        VisitDate = DateOnly.FromDateTime(DateTime.Today),
        Note = "Первичное Задание"
    });

    await db.SaveChangesAsync();
    Console.WriteLine($"Посещение для клиента с ID {clientId} успешно добавлено!");
}

// 9.5. Чтение клиента вместе с посещениями (Include)
Console.WriteLine("\n--- 9.5. Клиенты и их посещения ---");
var clients = await db.Clients
    .AsNoTracking()
    .Include(c => c.Visits) // Именно Include подгружает связанные данные из таблицы visits
    .OrderBy(c => c.ClientId)
    .ToListAsync();

foreach (var client in clients)
{
    Console.WriteLine($"{client.FirstName} {client.LastName}");

    // Выводим посещения для этого клиента
    foreach (var visit in client.Visits)
    {
        Console.WriteLine($"  {visit.VisitDate}: {visit.Note}");
    }
}