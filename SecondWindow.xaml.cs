using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Test_0801;

namespace Test_0801
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem != null)
                {
                    // 獲取所選行的 DataRowView
                    DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;

                    // 從 DataRowView 中獲取資料
                    int idToDelete = (int)selectedRow["ID"];

                    MySqlCommand deleteCommand = new MySqlCommand();
                    MySqlConnection connection = ((MainWindow)Application.Current.MainWindow).GetConnection();
                    deleteCommand.Connection = connection;

                    // 使用 DELETE 語句刪除選定的資料
                    string deleteQuery = "DELETE FROM calculator WHERE ID = @ID";
                    deleteCommand.CommandText = deleteQuery;

                    // 添加參數，防止 SQL 注入
                    deleteCommand.Parameters.AddWithValue("@ID", idToDelete);

                    deleteCommand.ExecuteNonQuery();

                    // 從 DataView 中移除選定的資料行
                    ((DataView)dataGrid.ItemsSource).Table.Rows.Remove(selectedRow.Row);
                    MessageBox.Show("Deletion complete!");
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting data: " + ex.Message);
            }
        }

    }
}
