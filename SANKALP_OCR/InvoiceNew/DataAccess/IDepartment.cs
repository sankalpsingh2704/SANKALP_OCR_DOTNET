using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using InvoiceNew.Models;

namespace InvoiceNew.DataAccess
{
  public interface IDepartment:IDisposable
  {
    IEnumerable<Department> getDepartmentDetails();
    Department getDepartmentbyid(int departmentid);
    void CreateDepartment(Department dept);
    void UpdateDepartment(Department dept);
    void DeleteDepartment(int id);

  }
}