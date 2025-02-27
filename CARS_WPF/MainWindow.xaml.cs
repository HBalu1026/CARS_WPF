using MySqlConnector;
using Mysqlx.Crud;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CARS_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadElso();
        LoadMasodik();
        LoadHarmadik();
    }

    private void LoadElso()
    {
        string connectionString = "Server=localhost;Database=classicmodels;User ID=root;Password=";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT productCode, productName FROM products ORDER BY productName;";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    List<string> products = new List<string>();

                    while (reader.Read())
                    {
                        string productCode = reader["productCode"].ToString();
                        string productName = reader["productName"].ToString();
                        products.Add($"{productCode} - {productName}");
                    }

                    lbElso.ItemsSource = products;
                }
            }
        }
    }

    private void ElsoKivalaszt (object sender, SelectionChangedEventArgs e)
    {
        if (lbElso.SelectedItem != null)
        {
            string selectedProduct = lbElso.SelectedItem.ToString();
            string productCode = selectedProduct.Split('-')[0].Trim();

            string connectionString = "Server=localhost;Database=classicmodels;User ID=root;Password=";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM orderdetails WHERE productCode = @productCode;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@productCode", productCode);
                    int orderCount = Convert.ToInt32(command.ExecuteScalar());

                    if (orderCount > 0)
                    {
                        lblElso.Content = $"Rendelések száma: {orderCount}";
                    }
                    else
                    {
                        MessageBox.Show("Erre a termékre jelenleg nincs rendelés!", "Nincs rendelés", MessageBoxButton.OK, MessageBoxImage.Warning);
                        lblElso.Content = "Rendelések száma: 0";
                    }
                }
            }
        }
    }


    private void LoadMasodik()
    {
        string connectionString = "Server=localhost;Database=classicmodels;User ID=root;Password=";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT DISTINCT country FROM customers ORDER BY country;";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    List<string> countries = new List<string>();

                    while (reader.Read())
                    {
                        countries.Add(reader["country"].ToString());
                    }

                    cbMasodik.ItemsSource = countries;
                }
            }
        }
    }

    private void MasodikKivalaszt(object sender, SelectionChangedEventArgs e)
    {
        if (cbMasodik.SelectedItem != null)
        {
            string selectedCountry = cbMasodik.SelectedItem.ToString();
            string connectionString = "Server=localhost;Database=classicmodels;User ID=root;Password=";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT customerName, phone, city FROM customers WHERE country = @country;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@country", selectedCountry);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        dgMasodik.ItemsSource = dt.DefaultView;
                    }
                }
            }
        }
    }

    private void LoadHarmadik()
    {
        string connectionString = "Server=localhost;Database=classicmodels;User ID=root;Password=";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM orders;", connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dgHarmadik.ItemsSource = dataTable.DefaultView;
        }
    }

    private void HarmadikKivalaszt(object sender, SelectionChangedEventArgs e)
    {
        if (dgHarmadik.SelectedItem != null)
        {
            DataRowView selectedRow = (DataRowView)dgHarmadik.SelectedItem;
            int selectedOrderNumber = Convert.ToInt32(selectedRow["orderNumber"]);

            string connectionString = "Server=localhost;Database=classicmodels;User ID=root;Password=";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT p.productName, p.buyPrice FROM orderdetails od JOIN products p ON od.productCode = p.productCode WHERE od.orderNumber = @selectedOrderNumber  ORDER BY p.productName;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@selectedOrderNumber ", selectedOrderNumber);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> products = new List<string>();

                        while (reader.Read())
                        {
                            string productName = reader["productName"].ToString();
                            decimal buyPrice = reader.GetDecimal("buyPrice");
                            products.Add($"{productName} - ${buyPrice}");
                        }

                        lbHarmadik.ItemsSource = products;
                    }
                }
            }
        }
    }
}