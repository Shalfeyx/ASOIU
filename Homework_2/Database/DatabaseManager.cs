using Microsoft.Data.Sqlite;
using RestaurantMenuApp_Console.Models;

namespace RestaurantMenuApp_Console.Database;

public class DatabaseManager
{
    private string _connectionString;
    
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }
    
    public void InitializeDatabase(string restaurantsCsvPath, string menuItemsCsvPath)
    {
        CreateTables();
        
        if (GetAllRestaurants().Count == 0 && File.Exists(restaurantsCsvPath))
        {
            ImportRestaurantsFromCsv(restaurantsCsvPath);
            Console.WriteLine($"[OK] Загружены рестораны из {restaurantsCsvPath}");
        }
        
        if (GetAllMenuItems().Count == 0 && File.Exists(menuItemsCsvPath))
        {
            ImportMenuItemsFromCsv(menuItemsCsvPath);
            Console.WriteLine($"[OK] Загружены блюда из {menuItemsCsvPath}");
        }
    }
    
    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS restaurants (
                restaurant_id INTEGER PRIMARY KEY AUTOINCREMENT,
                restaurant_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS menu_items (
                item_id INTEGER PRIMARY KEY AUTOINCREMENT,
                restaurant_id INTEGER NOT NULL,
                item_name TEXT NOT NULL,
                price REAL NOT NULL,
                FOREIGN KEY (restaurant_id) REFERENCES restaurants(restaurant_id)
            );
        ";
        cmd.ExecuteNonQuery();
    }
    
    private void ImportRestaurantsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO restaurants (restaurant_id, restaurant_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }
    
    private void ImportMenuItemsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO menu_items (item_id, restaurant_id, item_name, price)
                VALUES (@id, @restId, @name, @price)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@restId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@price", double.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }
    
    public List<Restaurant> GetAllRestaurants()
    {
        var result = new List<Restaurant>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT restaurant_id, restaurant_name FROM restaurants ORDER BY restaurant_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Restaurant(reader.GetInt32(0), reader.GetString(1)));
        }
        return result;
    }
    
    public List<MenuItem> GetAllMenuItems()
    {
        var result = new List<MenuItem>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT item_id, restaurant_id, item_name, price FROM menu_items ORDER BY item_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new MenuItem(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                (decimal)reader.GetDouble(3)
            ));
        }
        return result;
    }
    
    public MenuItem? GetMenuItemById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT item_id, restaurant_id, item_name, price FROM menu_items WHERE item_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new MenuItem(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                (decimal)reader.GetDouble(3)
            );
        }
        return null;
    }
    
    public void AddMenuItem(MenuItem item)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO menu_items (restaurant_id, item_name, price)
            VALUES (@restId, @name, @price)";
        cmd.Parameters.AddWithValue("@restId", item.RestaurantId);
        cmd.Parameters.AddWithValue("@name", item.Name);
        cmd.Parameters.AddWithValue("@price", (double)item.Price);
        cmd.ExecuteNonQuery();
    }
    
    public void UpdateMenuItem(MenuItem item)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE menu_items
            SET restaurant_id = @restId, item_name = @name, price = @price
            WHERE item_id = @id";
        cmd.Parameters.AddWithValue("@id", item.Id);
        cmd.Parameters.AddWithValue("@restId", item.RestaurantId);
        cmd.Parameters.AddWithValue("@name", item.Name);
        cmd.Parameters.AddWithValue("@price", (double)item.Price);
        cmd.ExecuteNonQuery();
    }
    
    public void DeleteMenuItem(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM menu_items WHERE item_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
    
    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();
        
        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);
        
        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }
        return (columns, rows);
    }
    
    public List<MenuItem> GetMenuItemsByRestaurant(int restaurantId)
    {
        var result = new List<MenuItem>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT item_id, restaurant_id, item_name, price
            FROM menu_items WHERE restaurant_id = @restId ORDER BY item_name";
        cmd.Parameters.AddWithValue("@restId", restaurantId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new MenuItem(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                (decimal)reader.GetDouble(3)
            ));
        }
        return result;
    }
}
