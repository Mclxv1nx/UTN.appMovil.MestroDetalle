using UTN.appMovil.MestroDetalle.Views;

namespace UTN.appMovil.MestroDetalle
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CategoriasPage), typeof(CategoriasPage));
            Routing.RegisterRoute(nameof(ProductosPage), typeof(ProductosPage));
        }
    }
}
