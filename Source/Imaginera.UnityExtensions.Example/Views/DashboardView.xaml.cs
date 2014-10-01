namespace Imaginera.UnityExtensions.Example.Views
{
    using Imaginera.UnityExtensions.Example.ViewModels;

    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardView"/> class.
        /// </summary>
        public DashboardView(DashboardViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }
    }
}
