using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gerenciamento_OLX_App.ViewModel.Behaviors_Vendas
{
    public class PesquisaBehavior : Behavior<DatePicker>
    {
        protected override void OnAttachedTo(DatePicker bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.DateSelected += Bindable_DateSelected;

        }

        private void Bindable_DateSelected(object sender, DateChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            var dt = datePicker?.BindingContext as VendasViewModel;

            var sd = e.NewDate.ToString();

            dt.PesquisarCommand.Execute(e.NewDate.ToString());


        }

        protected override void OnDetachingFrom(DatePicker bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.DateSelected -= Bindable_DateSelected;
        }

    }
}
