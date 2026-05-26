using UTN.appMovil.MestroDetalle.Views;

namespace UTN.appMovil.MestroDetalle;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void BtnCategorias_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CategoriasPage));
    }

    private async void BtnProductos_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProductosPage));
    }
}
