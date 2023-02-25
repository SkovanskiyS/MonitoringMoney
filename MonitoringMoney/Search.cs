//using MySql.Data.MySqlClient;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MonitoringMoney
//{
//    internal class Search
//    {
//        public void Get_Data_From_DB()
//        {
//            string con_link = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";

//            using(MySqlConnection con = new MySqlConnection(con_link))
//            {
//                con.Open();
//            }

//            string sql_cmd = @"SELECT * FROM debtordb";
//            var cmd = new MySqlCommand(sql_cmd, con);

//            MySqlDataReader reader = cmd.ExecuteReader();
//            object[] values_ = { };
//            var values = new List<object>();
//            var ID = new List<object>();
//            while (reader.Read())
//            {
//                for (int i = 0; i < reader.FieldCount; i++)
//                {
//                    values.Add(reader.GetValue(i));
//                }
//            }
//            con.Close();
//            foreach (var item in values)
//            {
//                if (item.ToString().ToLower().Contains("жасур"))
//                {
//                    object index_of_ID = values[values.IndexOf(item) - 2];
//                    ID.Add(index_of_ID);
//                }

//            }
//            foreach (var item in ID)
//            {
//                Console.WriteLine(item);
//            }

//        }


//    }
//}
