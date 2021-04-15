using Gerenciamento_OLX_App.Banco.Configuracao;
using Gerenciamento_OLX_App.View;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Gerenciamento_OLX_App.ViewModel
{
    public class ConfiguracaoViewModel : INotifyPropertyChanged
    {
        public Command ConfigurarCommmand { get; set; }
        public Command LimparCommmand { get; set; }
        public Command ResetCommmand { get; set; }
        public Command GoChatCommand { get; set; }
        public Command GoEstoqueCommand { get; set; }

        public const string Const_PagIniciao = "https://sp.olx.com.br/regiao-de-sorocaba";
        public const string Const_PagChat = "https://sp.olx.com.br/regiao-de-sorocaba";
        public DateTime Const_DataFecha = new DateTime(2020,09,01);
        public const string Const_CotaInve = "10%";

        private string _paginaInicial;
        private string _paginaChat;
        private DateTime _dataFechamento;
        private string _cotaInvestimento;

        public string PaginaInicial
        {
            get { return _paginaInicial; }
            set { _paginaInicial = value; OnPropertyChanged("PaginaInicial"); }
        }

        public string PaginaChat
        {
            get { return _paginaChat; }
            set { _paginaChat = value; OnPropertyChanged("PaginaChat"); }
        }

        public DateTime DataFechamento
        {
            get { return _dataFechamento; }
            set { _dataFechamento = value; OnPropertyChanged("DataFechamento"); }
        }

        public string CotaInvestimento
        {
            get { return _cotaInvestimento; }
            set { _cotaInvestimento = value; OnPropertyChanged("CotaInvestimento"); }
        }

        public ConfiguracaoViewModel()
        {
            ConfigurarCommmand = new Command(ConfigurarMethod);
            LimparCommmand = new Command(LimparMethod);
            ResetCommmand = new Command(ResetarMethod);
            GoChatCommand = new Command(GoChat);
            GoEstoqueCommand = new Command(GoEstoque);

            DataFechamento = DateTime.Today;

        }

        private async void ResetarMethod(object obj)
        {
            var resultado = await App.Current.MainPage.DisplayAlert("Aviso", "Tem Certeza Que Deseja Resetar a Data de Fechamento do mês", "Ok", "Cancelar");

            if (resultado == true)
            {
                int res = ConfiguracaoDB.TruncateTableConfigApp();
                if (res > 0)
                {
                    await App.Current.MainPage.DisplayAlert("Aviso", "Fechamendo do mês reconfigurado para Padrão Inicial", "Ok");

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Aviso", "Fechamendo do mês já em estado inicial", "Ok");

                }
            }
        }

        private void LimparMethod(object obj)
        {
            PaginaInicial = "";
            PaginaChat = "";
            DataFechamento = Const_DataFecha;
            CotaInvestimento = "";

        }

        private async void ConfigurarMethod(object obj)
        {
            if (!string.IsNullOrEmpty(PaginaInicial) && !string.IsNullOrEmpty(PaginaChat) && !DataFechamento.Equals(null) && !string.IsNullOrEmpty(CotaInvestimento))
            {                
                int resultado = ConfiguracaoDB.AddConfig(new Model.ConfigApp() { 
                    PaginaInicial = PaginaInicial, 
                    PaginaChat = PaginaChat, 
                    DataFechamento = DataFechamento, 
                    CotaInvestimento = CotaInvestimento 
                });

                if (resultado > 0)
                {
                    //int res = ConfiguracaoDB.TruncateTableConfigApp();
             
                        await App.Current.MainPage.DisplayAlert("Aviso", "Configurado Com Sucesso", "Ok");

                }

            }
            
        }

        private void GoEstoque(object obj)
        {
            ((MasterPage)App.Current.MainPage).Detail = new NavigationPage(new EstoquePage());
        }

        private void GoChat(object obj)
        {
            ((MasterPage)App.Current.MainPage).Detail.Navigation.PushAsync(new ChatPage());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string texto)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(texto)); 
        }

    }
}
