using UTN.appMovil.MestroDetalle.Data;
using UTN.appMovil.MestroDetalle.Models;

namespace UTN.appMovil.MestroDetalle.Views;

public partial class ProductosPage : ContentPage
{
    private readonly ProductosRepository _productosRepository;
    private readonly CategoriasRepository _categoriasRepository;
    private Producto? _productoSeleccionado;

    public ProductosPage(ProductosRepository productosRepository, CategoriasRepository categoriasRepository)
    {
        InitializeComponent();
        _productosRepository = productosRepository;
        _categoriasRepository = categoriasRepository;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarListas();
    }

    private void CargarListas()
    {
        var productos = _productosRepository.ReadAll();
        pickerProductos.ItemsSource = productos;

        var categorias = _categoriasRepository.ReadAll();
        pickerCategoriaProducto.ItemsSource = categorias;
        
        Limpiar();
    }

    private void CargarSoloCategorias()
    {
        var categorias = _categoriasRepository.ReadAll();
        pickerCategoriaProducto.ItemsSource = categorias;
    }

    private void OnProductoSelected(object sender, EventArgs e)
    {
        _productoSeleccionado = pickerProductos.SelectedItem as Producto;
        if (_productoSeleccionado != null)
        {
            txtProdNombre.Text = _productoSeleccionado.Nombre;
            txtProdPrecio.Text = _productoSeleccionado.Precio.ToString();

            // Set Categoria Picker
            var categorias = pickerCategoriaProducto.ItemsSource as List<Categoria>;
            if (categorias != null)
            {
                pickerCategoriaProducto.SelectedItem = categorias.FirstOrDefault(c => c.Id == _productoSeleccionado.CategoriaId);
            }
        }
    }

    private async void BtnProdCrear_Clicked(object sender, EventArgs e)
    {
        var catSeleccionada = pickerCategoriaProducto.SelectedItem as Categoria;
        if (catSeleccionada == null)
        {
            await DisplayAlert("Error", "Debe seleccionar una Categoría.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtProdNombre.Text) || !decimal.TryParse(txtProdPrecio.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Nombre y Precio (numérico) son obligatorios.", "OK");
            return;
        }

        try
        {
            var prod = new Producto
            {
                CategoriaId = catSeleccionada.Id,
                Nombre = txtProdNombre.Text,
                Precio = precio
            };
            _productosRepository.Create(prod);
            await DisplayAlert("Éxito", "Producto creado correctamente.", "OK");
            CargarListas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BtnProdActualizar_Clicked(object sender, EventArgs e)
    {
        if (_productoSeleccionado == null)
        {
            await DisplayAlert("Error", "Seleccione un producto para actualizar.", "OK");
            return;
        }

        var catSeleccionada = pickerCategoriaProducto.SelectedItem as Categoria;
        if (catSeleccionada == null)
        {
            await DisplayAlert("Error", "Debe seleccionar una Categoría.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtProdNombre.Text) || !decimal.TryParse(txtProdPrecio.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Nombre y Precio (numérico) son obligatorios.", "OK");
            return;
        }

        try
        {
            _productoSeleccionado.CategoriaId = catSeleccionada.Id;
            _productoSeleccionado.Nombre = txtProdNombre.Text;
            _productoSeleccionado.Precio = precio;
            
            _productosRepository.Update(_productoSeleccionado);
            await DisplayAlert("Éxito", "Producto actualizado correctamente.", "OK");
            CargarListas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BtnProdBorrar_Clicked(object sender, EventArgs e)
    {
        if (_productoSeleccionado == null)
        {
            await DisplayAlert("Error", "Seleccione un producto para borrar.", "OK");
            return;
        }

        try
        {
            _productosRepository.Delete(_productoSeleccionado.Id);
            await DisplayAlert("Éxito", "Producto borrado correctamente.", "OK");
            CargarListas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // --- LÓGICA CATEGORÍA INLINE ---
    
    private void BtnMostrarNuevaCat_Clicked(object sender, EventArgs e)
    {
        panelNuevaCategoria.IsVisible = true;
    }

    private void BtnCancelarNuevaCat_Clicked(object sender, EventArgs e)
    {
        panelNuevaCategoria.IsVisible = false;
        txtNuevaCatNombre.Text = string.Empty;
        txtNuevaCatDesc.Text = string.Empty;
    }

    private async void BtnGuardarNuevaCat_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNuevaCatNombre.Text) || string.IsNullOrWhiteSpace(txtNuevaCatDesc.Text))
        {
            await DisplayAlert("Error", "Nombre y Descripción son obligatorios para la nueva categoría.", "OK");
            return;
        }

        try
        {
            var cat = new Categoria
            {
                Nombre = txtNuevaCatNombre.Text,
                Descripcion = txtNuevaCatDesc.Text
            };
            var creada = _categoriasRepository.Create(cat);
            
            CargarSoloCategorias();
            
            // Auto-seleccionar la nueva
            var categorias = pickerCategoriaProducto.ItemsSource as List<Categoria>;
            if (categorias != null)
            {
                pickerCategoriaProducto.SelectedItem = categorias.FirstOrDefault(c => c.Id == creada.Id);
            }

            BtnCancelarNuevaCat_Clicked(sender, e); // ocultar panel
            await DisplayAlert("Éxito", "Categoría agregada y seleccionada.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void BtnLimpiar_Clicked(object sender, EventArgs e)
    {
        Limpiar();
    }

    private void Limpiar()
    {
        pickerProductos.SelectedItem = null;
        pickerCategoriaProducto.SelectedItem = null;
        _productoSeleccionado = null;
        txtProdNombre.Text = string.Empty;
        txtProdPrecio.Text = string.Empty;
    }
}
