using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Gerenciamento_OLX_App.Banco.Configuracao
{
    public class ConfiguracaoDB 
    {
        public static int AddConfig(Model.ConfigApp config)
        {
            using (Database database = new Database())
            {
                try
                {
                    return database._connection.InsertOrReplace(config);
                }
                catch (Exception)
                {
                    return 0;

                }
            }
        }

        public static List<Model.ConfigApp> GetAllConfigApp()
        {

            using (Database database = new Database())
            {
                try
                {
                    return database._connection.Table<Model.ConfigApp>().ToList();
                }
                catch (Exception)
                {

                    return new List<Model.ConfigApp>() { new Model.ConfigApp() { } };
                }
            }
        }

        public static int TruncateTableConfigApp()
        {

            using (Database database = new Database())
            {
                try
                {
                    
                    int res = database._connection.DeleteAll<Model.ConfigApp>();

                    return res;
                        
                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }


    }
}
