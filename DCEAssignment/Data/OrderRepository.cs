using DCEAssignment.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DCEAssignment.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<OrderModel> GetActiveOrdersByCustomer(int customerId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is null or empty.");
            }

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlCommand checkCustomerCmd = new SqlCommand("SELECT COUNT(*) FROM Customer WHERE UserId = @UserId", conn);
            checkCustomerCmd.Parameters.AddWithValue("@UserId", customerId);
            int customerCount = Convert.ToInt32(checkCustomerCmd.ExecuteScalar());

            if (customerCount == 0)
            {
                throw new ArgumentException($"Customer with UserId {customerId} does not exist.");
            }

            SqlCommand cmd = new SqlCommand("GetActiveOrdersByCustomer", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            List<OrderModel> orders = new List<OrderModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                OrderModel order = new OrderModel
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    ProductName = row["ProductName"]?.ToString() ?? "",
                    Username = row["Username"]?.ToString() ?? "",
                    OrderedOn = Convert.ToDateTime(row["OrderedOn"]),
                    OrderType = Convert.ToByte(row["OrderType"])
                };

                orders.Add(order);
            }

            return orders;
        }
    }
}
