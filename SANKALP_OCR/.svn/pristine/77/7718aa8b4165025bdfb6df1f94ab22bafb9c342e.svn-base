using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using InvoiceNew.Models;
namespace InvoiceNew.DataAccess
{
  public interface IVendor : IDisposable
  {
    IEnumerable<Vendor> getVendorDetails();
    Vendor getVendorbyid(int vendorid);
    void CreateVendor(Vendor vendor);
    void UpdateVendor(Vendor vendor);
    void DeleteVendor(int vendorid);

  }
}