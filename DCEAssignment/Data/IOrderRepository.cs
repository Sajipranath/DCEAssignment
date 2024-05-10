using DCEAssignment.Models;
using System.Collections.Generic;

namespace DCEAssignment.Data
{
    public interface IOrderRepository
    {
        IEnumerable<OrderModel> GetActiveOrdersByCustomer(int customerId);
    }
}
