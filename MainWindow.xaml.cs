using ExpressionTreeExample;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace Test_0801
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string expression = "";
        private bool finishcalculate = false;
        private bool numberornot = false;
        private string decimal_result = "";
        MySqlConnection connection = null;

        public MainWindow()
        {
            InitializeComponent();
            int status = Make_Connection();
            if (status == 1)
                MessageBox.Show("Connection failed...");
        }

        private int Make_Connection()
        {
            string connectionString = "Server=127.0.0.1;port=3306;Database=calculator;Uid=root;Pwd=\"\";";
            connection = new MySqlConnection(connectionString);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
                return 0;
            else
                return 1;
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberornot == false)
            {
                Button button = (Button)sender;
                if (ResultTextBox.Text == "0")
                {
                    expression = (string)button.Content;
                    ResultTextBox.Text = expression;
                }
                else if (finishcalculate == false)
                {
                    expression += button.Content;
                    ResultTextBox.Text += button.Content;
                }
                else
                {
                    finishcalculate = false;
                    expression = (string)button.Content;
                    ResultTextBox.Text = expression;
                }
            }
            numberornot = true;
        }

        private void ZeroButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberornot == false)
            {
                Button button = (Button)sender;
                if (ResultTextBox.Text == "0")
                {
                    //do nothing...
                }
                else if (finishcalculate == false)
                {
                    expression += "0";
                    ResultTextBox.Text += "0";
                }
                else
                {
                    finishcalculate = false;
                    expression = "0";
                    ResultTextBox.Text = expression;
                }
            }
            numberornot = true;
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (expression != "0" && numberornot == true)
            {
                finishcalculate = false;
                Button button = (Button)sender;
                ResultTextBox.Text += button.Content;

                if ((string)button.Content == "－") {
                    expression += "-";
                }
                else if ((string)button.Content == "＋")
                {
                    expression += "+";
                }
                else if ((string)button.Content == "×")
                {
                    expression += "*";
                }
                else if ((string)button.Content == "÷")
                {
                    expression += "/";
                }
            }
            numberornot = false;
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            finishcalculate = false;
            numberornot = false;
            expression = "0";
            ResultTextBox.Text = expression;
        }

        private void EqualButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 使用DataTable計算表達式來獲取計算結果
                DataTable dt = new DataTable();
                var result = dt.Compute(expression, "");

                // 顯示計算結果
                ResultTextBox.Text = Math.Floor(Convert.ToDouble(result)).ToString();
                decimal_result = ResultTextBox.Text;
                string binaryResult = Convert.ToString(Convert.ToInt32(ResultTextBox.Text), 2);
                BinaryBox.Text = binaryResult;

                ExpressionTree expressionTree = new ExpressionTree(expression);

                string preorderExpression = expressionTree.GetPreorder();
                string postorderExpression = expressionTree.GetPostorder();

                PreorderBox.Text = preorderExpression;
                PostorderBox.Text = postorderExpression;
                InorderBox.Text = expression;

                // 將計算結果保存為新的表達式
                expression = result.ToString();

                finishcalculate = true;
                numberornot = false;
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = "Error";
                // 在實際應用中，你可以根據需要處理計算錯誤的情況
            }
        }

        // 其他事件處理程序...
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand selectCommand = new MySqlCommand();
                selectCommand.Connection = connection;

                // 使用 SELECT 語句查詢是否有相同的資料
                string selectQuery = "SELECT COUNT(*) FROM calculator WHERE Infix = @Infix";
                selectCommand.CommandText = selectQuery;

                // 添加參數，防止 SQL 注入
                selectCommand.Parameters.AddWithValue("@Infix", InorderBox.Text);
                selectCommand.Parameters.AddWithValue("@Prefix", PreorderBox.Text);
                selectCommand.Parameters.AddWithValue("@Postfix", PostorderBox.Text);
                selectCommand.Parameters.AddWithValue("@DecValue", decimal_result);
                selectCommand.Parameters.AddWithValue("@Bin", BinaryBox.Text);

                int rowCount = Convert.ToInt32(selectCommand.ExecuteScalar());

                if (decimal_result == "")
                    MessageBox.Show("Please enter some value...");
                else if (rowCount > 0)
                {
                    MessageBox.Show("Data already exists.");
                }
                else
                {
                    MySqlCommand insert = new MySqlCommand();
                    insert.Connection = connection;

                    // 確保在 SQL 查詢中使用單引號將字符串值包裹起來
                    string insertQuery = "INSERT INTO calculator (Infix, Prefix, Postfix, DecValue, Bin) VALUES (@Infix, @Prefix, @Postfix, @DecValue, @Bin)";
                    insert.CommandText = insertQuery;

                    // 添加參數，防止 SQL 注入
                    insert.Parameters.AddWithValue("@Infix", InorderBox.Text);
                    insert.Parameters.AddWithValue("@Prefix", PreorderBox.Text);
                    insert.Parameters.AddWithValue("@Postfix", PostorderBox.Text);
                    insert.Parameters.AddWithValue("@DecValue", decimal_result);
                    insert.Parameters.AddWithValue("@Bin", BinaryBox.Text);

                    insert.ExecuteNonQuery();

                    MessageBox.Show("Insertion complete!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting data: " + ex.Message);
            }
        }
        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlCommand selectCommand = new MySqlCommand();
                selectCommand.Connection = connection;

                // 使用 SELECT 語句查詢所有資料
                string selectQuery = "SELECT * FROM calculator";
                selectCommand.CommandText = selectQuery;

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(selectCommand);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // 創建新的 QueryResultWindow 實例
                Window1 queryResultWindow = new Window1();

                // 將查詢結果繫結到 DataGrid
                queryResultWindow.dataGrid.ItemsSource = dataTable.DefaultView;

                // 顯示 QueryResultWindow 視窗
                queryResultWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error querying data: " + ex.Message);
            }
        }


    }
}

namespace ExpressionTreeExample
{
    class TreeNode
    {
        public char Value;
        public TreeNode Left;
        public TreeNode Right;

        public TreeNode(char value)
        {
            Value = value;
        }
    }

    class ExpressionTree
    {
        private TreeNode root;

        public ExpressionTree(string infix)
        {
            BuildTreeFromInfix(infix);
        }

        private int GetOperatorPrecedence(char op)
        {
            switch (op)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                default:
                    return 0;
            }
        }

        private void BuildTreeFromInfix(string infix)
        {
            Stack<TreeNode> operandStack = new Stack<TreeNode>();
            Stack<char> operatorStack = new Stack<char>();

            foreach (char c in infix)
            {
                if (char.IsLetterOrDigit(c))
                {
                    operandStack.Push(new TreeNode(c));
                }
                else if (c == '(')
                {
                    operatorStack.Push(c);
                }
                else if (c == ')')
                {
                    while (operatorStack.Peek() != '(')
                    {
                        char op = operatorStack.Pop();
                        TreeNode right = operandStack.Pop();
                        TreeNode left = operandStack.Pop();
                        TreeNode newNode = new TreeNode(op);
                        newNode.Left = left;
                        newNode.Right = right;
                        operandStack.Push(newNode);
                    }
                    operatorStack.Pop(); // Pop '('
                }
                else
                {
                    while (operatorStack.Count > 0 && GetOperatorPrecedence(c) <= GetOperatorPrecedence(operatorStack.Peek()))
                    {
                        char op = operatorStack.Pop();
                        TreeNode right = operandStack.Pop();
                        TreeNode left = operandStack.Pop();
                        TreeNode newNode = new TreeNode(op);
                        newNode.Left = left;
                        newNode.Right = right;
                        operandStack.Push(newNode);
                    }
                    operatorStack.Push(c);
                }
            }

            while (operatorStack.Count > 0)
            {
                char op = operatorStack.Pop();
                TreeNode right = operandStack.Pop();
                TreeNode left = operandStack.Pop();
                TreeNode newNode = new TreeNode(op);
                newNode.Left = left;
                newNode.Right = right;
                operandStack.Push(newNode);
            }

            root = operandStack.Pop();
        }

        private void GeneratePreorder(TreeNode node, StringBuilder sb)
        {
            if (node != null)
            {
                sb.Append(node.Value);
                GeneratePreorder(node.Left, sb);
                GeneratePreorder(node.Right, sb);
            }
        }

        private void GeneratePostorder(TreeNode node, StringBuilder sb)
        {
            if (node != null)
            {
                GeneratePostorder(node.Left, sb);
                GeneratePostorder(node.Right, sb);
                sb.Append(node.Value);
            }
        }

        public string GetPreorder()
        {
            StringBuilder sb = new StringBuilder();
            GeneratePreorder(root, sb);
            return sb.ToString();
        }

        public string GetPostorder()
        {
            StringBuilder sb = new StringBuilder();
            GeneratePostorder(root, sb);
            return sb.ToString();
        }
    }
}
