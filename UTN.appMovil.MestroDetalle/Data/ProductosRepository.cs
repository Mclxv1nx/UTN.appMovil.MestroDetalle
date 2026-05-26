namespace UTN.appMovil.MestroDetalle.Data;

using Dapper;
using Microsoft.Data.Sqlite;
using UTN.appMovil.MestroDetalle.Models;

public class ProductosRepository : IDisposable
{
    private readonly SqliteConnection _connection;

    public ProductosRepository()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "panaderia.db");
        var connectionString = $"Data Source={dbPath};Cache=Shared;";
        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Productos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoriaId INTEGER NOT NULL,
                Nombre TEXT NOT NULL,
                Precio REAL NOT NULL,
                FOREIGN KEY(CategoriaId) REFERENCES Categorias(Id)
            )");
    }

    public Producto Create(Producto producto)
    {
        var sql = "INSERT INTO Productos (CategoriaId, Nombre, Precio) VALUES (@CategoriaId, @Nombre, @Precio)";
        var affected = _connection.Execute(sql, producto);
        if (affected == 0)
            throw new Exception("No se pudo crear el registro.");
            
        var id = _connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
        producto.Id = id;
        return producto;
    }

    public Producto? ReadById(int id)
    {
        return _connection.QueryFirstOrDefault<Producto>("SELECT * FROM Productos WHERE Id = @Id", new { Id = id });
    }

    public List<Producto> ReadAll()
    {
        return _connection.Query<Producto>("SELECT * FROM Productos").ToList();
    }

    public void Update(Producto producto)
    {
        var sql = "UPDATE Productos SET CategoriaId = @CategoriaId, Nombre = @Nombre, Precio = @Precio WHERE Id = @Id";
        var affected = _connection.Execute(sql, producto);
        if (affected == 0)
            throw new Exception("El ID no existe.");
    }

    public void Delete(int id)
    {
        var affected = _connection.Execute("DELETE FROM Productos WHERE Id = @Id", new { Id = id });
        if (affected == 0)
            throw new Exception("El ID no existe.");
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
