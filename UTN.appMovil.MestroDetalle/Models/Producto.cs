namespace UTN.appMovil.MestroDetalle.Models;

public class Producto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
