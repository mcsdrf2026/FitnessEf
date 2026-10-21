using FitnessEf;
using Microsoft.EntityFrameworkCore;

await using var db = new FitnessContext();

Console.WriteLine("--- Задание 8. Клиенты без Email (NULL) ---");

var clientsWithoutEmail = await db.Clients
    .AsNoTracking()
    .Where(c => c.Email == null)
    .OrderBy(c => c.ClientId)
    .ToListAsync();

if (clientsWithoutEmail.Count == 0)
{
    Console.WriteLine("Все клиенты имеют Email.");
}
else
{
    foreach (var client in clientsWithoutEmail)
    {
        Console.WriteLine($"{client.ClientId}: {client.FirstName} {client.LastName} (Email отсутствует)");
    }
}