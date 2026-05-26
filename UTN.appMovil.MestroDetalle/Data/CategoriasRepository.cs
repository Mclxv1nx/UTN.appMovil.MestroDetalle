namespace UTN.appMovil.MestroDetalle.Data;

using Dapper;
using Microsoft.Data.Sqlite;
using UTN.appMovil.MestroDetalle.Models;

public class CategoriasRepository : IDisposable
{
    private readonly SqliteConnection _connection;

    public CategoriasRepository()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "panaderia.db");
        var connectionString = $"Data Source={dbPath};Cache=Shared;";
        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Categorias (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Descripcion TEXT NOT NULL
            )");
    }

    public Categoria Create(Categoria categoria)
    {
        var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
        var affected = _connection.Execute(sql, categoria);
        if (affected == 0)
            throw new Exception("No se pudo crear el registro.");
        
        var id = _connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
        categoria.Id = id;
        return categoria;
    }

    public Categoria? ReadById(int id)
    {
        return _connection.QueryFirstOrDefault<Categoria>("SELECT * FROM Categorias WHERE Id = @Id", new { Id = id });
    }

    public List<Categoria> ReadAll()
    {
        return _connection.Query<Categoria>("SELECT * FROM Categorias").ToList();
    }

    public void Update(Categoria categoria)
    {
        var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id = @Id";
        var affected = _connection.Execute(sql, categoria);
        if (affected == 0)
            throw new Exception("El ID no existe.");
    }

    public void Delete(int id)
    {
        var affected = _connection.Execute("DELETE FROM Categorias WHERE Id = @Id", new { Id = id });
        if (affected == 0)
            throw new Exception("El ID no existe.");
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
