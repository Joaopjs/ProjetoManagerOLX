using Gerenciamento_OLX_App.Banco;
using Gerenciamento_OLX_App.Banco.Configuracao;
using Gerenciamento_OLX_App.Banco.Vendas;
using Gerenciamento_OLX_App.Model;
using Gerenciamento_OLX_App.View;
using Gerenciamento_OLX_App.View.UtilView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Gerenciamento_OLX_App.ViewModel
{
    public class VendasViewModel : INotifyPropertyChanged
    {
        public Command GoChatCommand { get; set; }
        public Command GoEstoqueCommand { get; set; }
        public Command PesquisarCommand { get; set; }

        private List<Venda> _Vendas;
        private List<Venda> Lista = new List<Venda>();
        private string _cCapital;
        private string _vLucro;
        private string _vMargem;

        public string V_Margem
        {
            get { return _vMargem; }
            set { _vMargem = value; OnPropertyChanged("V_Margem"); }
        }


        public string V_Lucro
        {
            get { return _vLucro; }
            set { _vLucro = value; OnPropertyChanged("V_Lucro"); }
        }


        public string V_Investido
        {
            get { return _cCapital; }
            set { _cCapital = value; OnPropertyChanged("V_Investido"); }
        }

        public List<Venda> Vendas
        {
            get { return _Vendas; }
            set { _Vendas = value; OnPropertyChanged("Vendas"); }
        }

        private DateTime _dataInicio;

        public DateTime DataInicio
        {
            get { return _dataInicio; }
            set 
            { 
                _dataInicio = value; 
                OnPropertyChanged("DataInicio"); 
               
            }
        }

        private DateTime _dataFim;

        public DateTime DataFim
        {
            get { return _dataFim; }
            set
            { 
                _dataFim = value; 
                OnPropertyChanged("DataFim");

            }
        }

        public VendasViewModel()
        {
            
            string Configu = "";
            DateTime date = DateTime.Today;

            GoChatCommand = new Command(GoChat);
            GoEstoqueCommand = new Command(GoEstoque);
            PesquisarCommand = new Command<string>(Pesquisar);

            var mes = date.Month;

            try
            {
                //Configura pagina inicial do app
                Configu = ConfiguracaoDB.GetAllConfigApp().FirstOrDefault().PaginaInicial;
            }
            catch (Exception)
            {


            }


            if (string.IsNullOrEmpty(Configu))
            {
                try
                {
                    //Lista todas venda realizadas entre uma periodo configurado no banco de dados
                    Lista = VedasDB.GetAllVenda().Where(x => x.DataVendav.Month == mes).ToList();

                    //Mostra a data na tela
                    DataInicio = new DateTime(date.Year, date.Month, 01); //string.Format("{0}/{1}/{2}", 01, date.Month, date.Year); // new DateTime(date.Year, date.Month, 1).ToString(); 

                    int d = DateTime.DaysInMonth(date.Year, date.Month);

                    DataFim = new DateTime(date.Year, date.Month, d); //string.Format("{0}/{1}/{2}", d, date.Month, date.Year); //new DateTime(DateTime.DaysInMonth(date.Year, date.Month), date.Year, date.Month).ToString();


                }
                catch (Exception)
                {
                    //Evita uma exception em caso de nao haver nada no banco de dados
                    Lista = new List<Venda>() { };
                }
            }
            else
            {
                //Pega a configuração desejada pelo usuario
                var confg = ConfiguracaoDB.GetAllConfigApp().LastOrDefault();

                try
                {

                    int inicio = 1;
                    int fim = 1;
                    DateTime abreMes;
                    DateTime fechamento;

                    try
                    {
                        inicio = confg.DataFechamento.Day;
                        abreMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, inicio);
                    }
                    catch (Exception)
                    {

                        inicio = DateTime.DaysInMonth(date.Year, date.Month - 1);
                        abreMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, inicio);
                    }

                    try
                    {
                        fim = confg.DataFechamento.Day;
                        fechamento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, fim);
                    }
                    catch (Exception)
                    {

                        fim = DateTime.DaysInMonth(date.Year, date.Month);
                        fechamento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, fim);
                    }
                                       

                    Lista = VedasDB.GetAllVenda().Where(x => x.DataVendav >= abreMes && x.DataVendav <= fechamento).ToList();
                                      
                    //Mostra a data na tela
                    DataInicio = new DateTime(date.Year, date.Month - 1, inicio); //string.Format("{0}/{1}/{2}", 01, date.Month, date.Year); // new DateTime(date.Year, date.Month, 1).ToString(); 

                    //int d = DateTime.DaysInMonth(date.Year, date.Month);

                    DataFim = new DateTime(date.Year, date.Month, fim); //string.Format("{0}/{1}/{2}", d, date.Month, date.Year); //new DateTime(DateTime.DaysInMonth(date.Year, date.Month), date.Year, date.Month).ToString();


                }
                catch (Exception)
                {
                    if (confg.DataFechamento.Day >= 29 && confg.DataFechamento.Day >= 31)
                    {
                        var fechamento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        var abreMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);

                        Lista = VedasDB.GetAllVenda().Where(x => x.DataVendav >= abreMes && x.DataVendav <= fechamento).ToList();
                    }
                }

            }

            //formata numeros
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

            Vendas = Lista;

            V_Investido = Lista.Select(x => x.PrecoCusto).Sum().ToString();
            V_Lucro = Lista.Select(x => x.LucroVenda).Sum().ToString();
            V_Margem = (((double.Parse(V_Lucro) * 100) / double.Parse(V_Investido))).ToString("F", nfi);

                      
            
        }

        private void EntreDatas(string value)
        {
            if (Convert.ToDateTime(DataInicio) < Convert.ToDateTime(DataFim))
            {
                
                try
                {
                    
                    Lista = VedasDB.GetAllVenda().Where(x => x.DataVendav > Convert.ToDateTime(DataInicio) && x.DataVendav < Convert.ToDateTime(DataFim).AddDays(1)).ToList();
                    Vendas = Lista;
                }
                catch (Exception)
                {

                    App.Current.MainPage.DisplayAlert("Aviso", "    Não há vendas entre essa data ou ocorreu algum erro!", "Ok");
                }

            }
        }

        private void Pesquisar(string Texto)
        {
            if (Convert.ToDateTime(DataInicio) < Convert.ToDateTime(DataFim))
            {
                try
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                    Lista = VedasDB.GetAllVenda().Where(x => x.DataVendav > Convert.ToDateTime(DataInicio) && x.DataVendav < Convert.ToDateTime(DataFim).AddDays(1)).ToList();
                    V_Investido = Lista.Select(x => x.PrecoCusto).Sum().ToString();
                    V_Lucro = Lista.Select(x => x.LucroVenda).Sum().ToString();
                    V_Margem = (((double.Parse(V_Lucro) * 100) / double.Parse(V_Investido))).ToString("F", nfi);

                    Vendas = Lista;
                }
                catch (Exception)
                {

                    App.Current.MainPage.DisplayAlert("Aviso", " Não há vendas entre essa data ou ocorreu algum erro!", "Ok");
                }

            }
            else
            {
                Vendas = new List<Venda>();
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
