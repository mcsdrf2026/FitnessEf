using FitnessEf;
using Microsoft.EntityFrameworkCore;

try
{
    // 11.2. Асинхронное выполнение и короткоживущий контекст
    await using var db = new FitnessContext();

    // 11.3. Сравнение отслеживаемого и нетрекаемого запроса
    Console.WriteLine("--- 11.3. Запрос без отслеживания (AsNoTracking) ---");
    var list = await db.Clients
        .AsNoTracking()
        .ToListAsync();
    Console.WriteLine($"Загружено клиентов (без отслеживания): {list.Count}");

    Console.WriteLine("\n--- 11.3. Отслеживаемый запрос (с изменением) ---");
    int id = 1;
    var client = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == id);

    if (client is not null)
    {
        Console.WriteLine($"До изменения: {client.FirstName}, статус: {client.Status}");
        client.Status = "inactive";
        await db.SaveChangesAsync(); // 11.2. Сохранение асинхронно
        Console.WriteLine($"После изменения: статус изменен на 'inactive'");
    }

    // 11.4. Диагностика SQL
    Console.WriteLine("\n--- 11.4. Диагностика SQL (ToQueryString) ---");
    var query = db.Clients
        .Where(c => c.Status == "active")
        .OrderBy(c => c.LastName);

    // Этот метод покажет SQL-запрос, который EF Core сгенерирует для базы PostgreSQL, 
    // но НЕ выполнит его.
    Console.WriteLine(query.ToQueryString());
}
// 11.1. Базовая обработка ошибок
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Ошибка конфигурации: {ex.Message}");
}
catch (DbUpdateException ex)
{
    Console.WriteLine("Ошибка сохранения данных.");
    Console.WriteLine(ex.GetBaseException().Message);
}
catch (Exception ex)
{
    Console.WriteLine("Произошла непредвиденная ошибка.");
    Console.WriteLine(ex.Message);
}