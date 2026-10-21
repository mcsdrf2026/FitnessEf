using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

Console.WriteLine("--- Задание 6: Добавление клиента ---");

// 6.2. Проверка пользовательского ввода
Console.Write("Имя: ");
string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

Console.Write("Фамилия: ");
string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
{
    Console.WriteLine("Имя и фамилия обязательны. Добавление отменено.");
    return; // Выходим из программы, ничего не добавляем
}

// 6.1. Добавление сущности
var client = new Client
{
    FirstName = firstName,
    LastName = lastName,
    BirthDate = new DateOnly(2000, 5, 15), // Пример даты
    Phone = "79995554433",
    Email = "test@example.com",
    RegistrationDate = DateOnly.FromDateTime(DateTime.Today),
    Status = "active"
};

// 6.3. Обработка DbUpdateException
try
{
    db.Clients.Add(client);
    int saved = await db.SaveChangesAsync();

    Console.WriteLine($"\nСохранено изменений: {saved}");
    Console.WriteLine($"Клиент добавлен. Новый ID: {client.ClientId}");
}
catch (DbUpdateException ex)
{
    Console.WriteLine("\nНе удалось сохранить клиента. Проверьте данные и ограничения БД.");
    Console.WriteLine(ex.GetBaseException().Message); // только учебная диагностика
}