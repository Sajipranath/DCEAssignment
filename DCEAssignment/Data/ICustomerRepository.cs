using DCEAssignment.Models;
using System.Collections.Generic;

namespace DCEAssignment.Data
{
    public interface ICustomerRepository
    {
        IEnumerable<CustomerModel> GetCustomers();
        int CreateCustomer(CustomerModel newCustomer);
        void UpdateCustomer(int userId, CustomerModel updatedCustomer);
        void DeleteCustomer(int userId);
    }
}
