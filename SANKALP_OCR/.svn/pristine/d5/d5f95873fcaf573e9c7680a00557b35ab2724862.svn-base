using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace IQinvoice.DataAccess
{
  public class ProfileRepository //: IProfile,IDisposable
  {

    public int ComparePassword(int UserID, string Password, string NewPassword)
    {

      ObjectParameter Passwordstatus = new ObjectParameter("Passwordstatus", typeof(Int32));
  //    context.Sp_ComparePassword(UserID, NewPassword, Passwordstatus);

      return Convert.ToInt32(Passwordstatus.Value);
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}