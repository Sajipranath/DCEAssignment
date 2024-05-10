using DCEAssignment.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DCEAssignment.Data
{
    public class CustomerRepository : ICustomerRepository, IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private void OpenConnection()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public IEnumerable<CustomerModel> GetCustomers()
        {
            List<CustomerModel> customers = new List<CustomerModel>();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Customer", _connection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerModel customer = new CustomerModel
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString()!,
                            Email = reader["Email"].ToString()!,
                            FirstName = reader["FirstName"].ToString()!,
                            LastName = reader["LastName"].ToString()!,
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };

                        customers.Add(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw new ApplicationException("An error occurred while retrieving customers.", ex);
            }
            finally
            {
                _connection?.Close();
            }

            return customers;
        }


        public int CreateCustomer(CustomerModel newCustomer)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("INSERT INTO Customer (Username, Email, FirstName, LastName, CreatedOn, IsActive) VALUES (@Username, @Email, @FirstName, @LastName, @CreatedOn, @IsActive); SELECT SCOPE_IDENTITY();", _connection);

                cmd.Parameters.AddWithValue("@Username", newCustomer.Username);
                cmd.Parameters.AddWithValue("@Email", newCustomer.Email);
                cmd.Parameters.AddWithValue("@FirstName", newCustomer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", newCustomer.LastName);
                cmd.Parameters.AddWithValue("@CreatedOn", newCustomer.CreatedOn);
                cmd.Parameters.AddWithValue("@IsActive", newCustomer.IsActive);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw new ApplicationException("An error occurred while creating a new customer.", ex);
            }
            finally
            {
                _connection?.Close();
            }
        }

        public void UpdateCustomer(int userId, CustomerModel updatedCustomer)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("UPDATE Customer SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, IsActive = @IsActive WHERE UserId = @UserId", _connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Username", updatedCustomer.Username);
                cmd.Parameters.AddWithValue("@Email", updatedCustomer.Email);
                cmd.Parameters.AddWithValue("@FirstName", updatedCustomer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", updatedCustomer.LastName);
                cmd.Parameters.AddWithValue("@IsActive", updatedCustomer.IsActive);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new ApplicationException($"No customer found with UserId {userId}.");
                }

            }
            catch (Exception ex)   // Handle or log the exception
            {
                throw new ApplicationException("An error occurred while updating the customer.", ex);
            }
            finally
            {
                _connection?.Close();
            }
        }

        public void DeleteCustomer(int userId)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("DELETE FROM Customer WHERE UserId = @UserId", _connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new ApplicationException($"No customer found with UserId {userId}.");
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw new ApplicationException("An error occurred while deleting the customer.", ex);
            }
            finally
            {
                _connection?.Close();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
