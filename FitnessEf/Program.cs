using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

Console.WriteLine("--- Задание 7 ---");

// 7.1. Изменение отслеживаемого объекта
Console.WriteLine("\n--- 7.1. Изменение телефона (ID=1) ---");
int idToUpdate = 1;
var clientToUpdate = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == idToUpdate);

if (clientToUpdate is null)
{
    Console.WriteLine("Клиент не найден.");
}
else
{
    clientToUpdate.Phone = "79990001122";
    await db.SaveChangesAsync();
    Console.WriteLine("Телефон изменён.");
}

// 7.4. Несколько действий перед сохранением
Console.WriteLine("\n--- 7.4. Несколько действий перед сохранением ---");
db.Clients.Add(new Client
{
    FirstName = "Анна",
    LastName = "Смирнова",
    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
    Status = "active"
});

var existing = await db.Clients.FindAsync(idToUpdate);
if (existing is not null)
{
    existing.Status = "inactive";
}

await db.SaveChangesAsync();
Console.WriteLine("Добавлен новый клиент и изменён статус ID=1 (всё за один SaveChanges).");

// 7.2. Удаление
Console.WriteLine("\n--- 7.2. Удаление ---");
Console.Write("Введите ID клиента для удаления (например, 2): ");
if (int.TryParse(Console.ReadLine(), out int idToDelete))
{
    var clientToDelete = await db.Clients.FindAsync(idToDelete);

    if (clientToDelete is null)
    {
        Console.WriteLine("Клиент не найден.");
    }
    else
    {
        db.Clients.Remove(clientToDelete);
        await db.SaveChangesAsync();
        Console.WriteLine($"Клиент с ID {idToDelete} удалён.");
    }
}
else
{
    Console.WriteLine("Некорректный ID. Удаление отменено.");
}