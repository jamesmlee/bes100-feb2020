using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Services
{
    // click class name --> screwdriver --> Extract Interface
    public class EmployeeIdGenerator : IGenerateEmployeeIds
    {
        public Guid GetNewEmployeeId()
        {
            return Guid.NewGuid();
        }
    }
}
