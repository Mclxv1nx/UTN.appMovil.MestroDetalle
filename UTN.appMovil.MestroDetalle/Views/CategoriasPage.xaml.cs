using UTN.appMovil.MestroDetalle.Data;
using UTN.appMovil.MestroDetalle.Models;

namespace UTN.appMovil.MestroDetalle.Views;

public partial class CategoriasPage : ContentPage
{
    private readonly CategoriasRepository _categoriasRepository;
    private Categoria? _categoriaSeleccionada;

    public CategoriasPage(CategoriasRepository categoriasRepository)
    {
        InitializeComponent();
        _categoriasRepository = categoriasRepository;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarCategorias();
    }

    private void CargarCategorias()
    {
        var categorias = _categoriasRepository.ReadAll();
        pickerCategorias.ItemsSource = categorias;
        pickerCategorias.SelectedItem = null;
        Limpiar();
    }

    private void OnCategoriaSelected(object sender, EventArgs e)
    {
        _categoriaSeleccionada = pickerCategorias.SelectedItem as Categoria;
        if (_categoriaSeleccionada != null)
        {
            txtCatNombre.Text = _categoriaSeleccionada.Nombre;
            txtCatDescripcion.Text = _categoriaSeleccionada.Descripcion;
        }
    }

    private async void BtnCatCrear_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCatNombre.Text) || string.IsNullOrWhiteSpace(txtCatDescripcion.Text))
        {
            await DisplayAlert("Error", "Nombre y Descripción son obligatorios.", "OK");
            return;
        }

        try
        {
            var cat = new Categoria
            {
                Nombre = txtCatNombre.Text,
                Descripcion = txtCatDescripcion.Text
            };
            _categoriasRepository.Create(cat);
            await DisplayAlert("Éxito", "Categoría creada correctamente.", "OK");
            CargarCategorias();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BtnCatActualizar_Clicked(object sender, EventArgs e)
    {
        if (_categoriaSeleccionada == null)
        {
            await DisplayAlert("Error", "Seleccione una categoría para actualizar.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtCatNombre.Text) || string.IsNullOrWhiteSpace(txtCatDescripcion.Text))
        {
            await DisplayAlert("Error", "Nombre y Descripción son obligatorios.", "OK");
            return;
        }

        try
        {
            _categoriaSeleccionada.Nombre = txtCatNombre.Text;
            _categoriaSeleccionada.Descripcion = txtCatDescripcion.Text;
            
            _categoriasRepository.Update(_categoriaSeleccionada);
            await DisplayAlert("Éxito", "Categoría actualizada correctamente.", "OK");
            CargarCategorias();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BtnCatBorrar_Clicked(object sender, EventArgs e)
    {
        if (_categoriaSeleccionada == null)
        {
            await DisplayAlert("Error", "Seleccione una categoría para borrar.", "OK");
            return;
        }

        try
        {
            _categoriasRepository.Delete(_categoriaSeleccionada.Id);
            await DisplayAlert("Éxito", "Categoría borrada correctamente.", "OK");
            CargarCategorias();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void BtnLimpiar_Clicked(object sender, EventArgs e)
    {
        pickerCategorias.SelectedItem = null;
        Limpiar();
    }

    private void Limpiar()
    {
        _categoriaSeleccionada = null;
        txtCatNombre.Text = string.Empty;
        txtCatDescripcion.Text = string.Empty;
    }
}
