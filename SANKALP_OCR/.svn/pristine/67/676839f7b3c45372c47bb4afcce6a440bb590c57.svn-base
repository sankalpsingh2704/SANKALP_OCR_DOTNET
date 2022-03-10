using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;


namespace IQinvoice.DataAccess
{
  public interface ICostUnit : IDisposable
  {
    IEnumerable<CostUnit> getCostUnitDetails();
    CostUnit getCostUnitbyid(int costunitid);
    void CreateCostUnit(CostUnit costunit);
    void UpdateCostUnit(CostUnit costunit);
    void DeleteCostUnit(int costunitid);
  }
}