using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IQinvoice.DataAccess
{
  public class CostUnitRepository : ICostUnit,IDisposable
  {

     private IMSEntities context;

     public CostUnitRepository(IMSEntities context)
    {
      this.context = context;
    }

    public IEnumerable<CostUnit> getCostUnitDetails()
    {
      return context.CostUnits.ToList();  
    }

    public CostUnit getCostUnitbyid(int costunitid)
    {
      return context.CostUnits.Find(costunitid);
    }

    public void CreateCostUnit(CostUnit costunit)
    {
      context.CostUnits.Add(costunit);
      context.SaveChanges();
    }

    public void UpdateCostUnit(CostUnit costunit)
    {
      context.Entry(costunit).State = System.Data.Entity.EntityState.Modified;
      context.SaveChanges();
    }

    public void DeleteCostUnit(int costunitid)
    {
      CostUnit costunitdelete = context.CostUnits.Find(costunitid);
      if (costunitdelete != null)
      {
        context.CostUnits.Remove(costunitdelete);
        context.SaveChanges();
      }
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}