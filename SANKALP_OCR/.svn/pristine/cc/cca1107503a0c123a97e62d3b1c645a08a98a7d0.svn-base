using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using InvoiceNew.Models;

namespace InvoiceNew.DataAccess
{
  public interface ICostCenter : IDisposable
  {
    IEnumerable<CostCenter> getCostCenterDetails();
    CostCenter getCostCenterbyid(int costcenterid);
    void CreateCostCenter(CostCenter costcenter);
    void UpdateCostCenter(CostCenter costcenter);
    void DeleteCostCenter(int costcenterid);

  }
}