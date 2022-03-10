using InvoiceNew.Models;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace InvoiceNew.Controllers
{
    public class SalaryController : Controller
    {
		static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
		static SqlConnection con = new SqlConnection(constr);
		// GET: Salary

		#region Salary
		public JsonResult TallySalUpdate(string LocationId,string Month,string Year , string TallyComment)
		{
			return Json(new { });
		}
		public JsonResult CheckTallyExist(string LocationId, string Month,string Year)
		{
			var exist = false;
			try
			{
				
				con.Open();
				SqlCommand cmd = new SqlCommand("checkexisttally", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@LocationId", LocationId);
				cmd.Parameters.AddWithValue("@Month", Month);
				cmd.Parameters.AddWithValue("@Year", Year);
				using (SqlDataReader sdr = cmd.ExecuteReader())
				{
					while (sdr.Read())
					{
						exist = bool.Parse(sdr["TallyUpdated"].ToString());
					}
				}
				con.Close();
			}
			catch(Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
			}
			return Json(new { exist });
		}

		private string getStringCell(ICell cell)
		{
			switch (cell.CellType)
			{
				case CellType.Unknown:
					return"";
				case CellType.Numeric:
					return cell.NumericCellValue.ToString();	
				case CellType.String:
					return cell.StringCellValue;
				case CellType.Formula:
					return "Error";
				case CellType.Blank:
					return "";
				case CellType.Boolean:
					return "";
				case CellType.Error:
					return "";
				default:
					return cell.StringCellValue;
			}
		}
		private int ReadSalaryXLS(FileStream fs, SalaryInputModel model, string id)
		{
			List<DataTable> tablelist = new List<DataTable>();
			DataTable dt = new DataTable();
			var wb = new HSSFWorkbook(fs);

				var sh = (HSSFSheet)wb[0];

				List<TwoDContainer> textarray = new List<TwoDContainer>();
				List<string> excelHeaders = new List<string>() { "Emp ID", "Employee Name", "Father's Name", "Date of Joining", "Date of Leaving", "PF No.", "ESI No.", "PAN No.", "Bank Name", "Bank A/c No.", "IFSC", "Designation", "Blood Group", "Department", "PF Date", "Category", "Branch", "Sal. Calendar Days", "UAN", "Aadhar No","Pay Days","Present Days","BASIC","HRA","LTA","MEDICAL AL","EDUCATION","Total Earning","PF","ESI","PT","TDS","OTHERS","ADVANCE.","Total Dedn.","Net Amount","Remarks if any and Signature"};
				DataTable table = new DataTable();

				var staffcol = new TwoDContainer();
				for (var i = 0; sh.GetRow(i) != null; i++)
				{
					var temp = i; 
					var head = 0;
					for (var j = 0; sh.GetRow(i).GetCell(j) != null; j++)
					{
						var data = getStringCell(sh.GetRow(i).GetCell(j));

						if (data == "Employee Name")
						{
							head++;
						}
						else
						{
							var text = data;
						}
						TwoDContainer d2array = new TwoDContainer();
						if (i > 6 && j > 2 && j < 39)
						{
							
							d2array.Row = i;
							d2array.Coloumn = j;
							d2array.Content = data.Replace(",", "");
							textarray.Add(d2array);
						}
						if (i > 6  && j == 6 || j == 7 || j == 17)
						{
						if (d2array.Content == "")
							d2array.Content = DateTime.Now.ToString();
						else
						{
							if(j != 17)
								d2array.Content = sh.GetRow(i).GetCell(j).DateCellValue.ToString();
							else
							{
								d2array.Content = sh.GetRow(i).GetCell(j).StringCellValue;
							}
						}


						}
						if (i > 6 && j > 2 && j == 39)
						{
							
							d2array.Row = i;
							d2array.Coloumn = j;
							d2array.Content = "";
							textarray.Add(d2array);
						}
						
						if (d2array.Content == "")
							d2array.Content = "0";
					}

					if (head > 3)
					{
						textarray.RemoveAll(x => x.Row == i);
					}



				}

				foreach (var headers in excelHeaders)
				{
					table.Columns.Add(headers);
				}
				//table.Columns.Add("Nid");
				var srow = textarray.FirstOrDefault().Row;
				var icol = textarray.FirstOrDefault().Coloumn;

				var p = 0;
				while (p <= textarray.Count - 1)
				{


					var j = 3;
					var nrow = table.NewRow();
					var skip = true;
					while (textarray.FindIndex(x => x.Coloumn == j && x.Row == srow) != -1 && j < excelHeaders.Count + 3)
					{
						var idx = textarray.FindIndex(x => x.Row == srow && x.Coloumn == j);
						if (textarray[idx].Content == "")
							textarray[idx].Content = "0";
						nrow.SetField<string>(j - 3, textarray[idx].Content);
						p++;
						j++;
						skip = false;
					}

					//p++;
					if (!skip)
					{
						//nrow.SetField<string>(j - 4, id);
						table.Rows.Add(nrow);
					}
					else
					{
						p++;
					}
					srow++;


				}
				
			
			var nid = 0;
			var rowcount = 0;
		
			sh = (HSSFSheet)wb[0];
			con.Open();
			SqlCommand com = new SqlCommand("InsertSalary", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@salTable", table);
			com.Parameters.AddWithValue("@LocId", model.LocationId);
			com.Parameters.AddWithValue("@Month", model.Month);
			com.Parameters.AddWithValue("@Year", model.Year);
			com.Parameters.AddWithValue("@Id", id);
			com.Parameters.AddWithValue("@Update", model.Update);
			com.ExecuteNonQuery();
			rowcount = table.Rows.Count;
			con.Close();
			
			return rowcount;


		}

		/// <summary>
		/// Uploads Excel Data into DataBase
		/// </summary>
		/// <param name="model">Salary Input Model</param>							   
		/// <returns>Json Response</returns>
		public JsonResult Excelupload(SalaryInputModel model)
		{
			int rows = 0;
			try
			{
				string id = "0";

				if (model.Update)
					id = model.UpdateId.ToString();
				var file = Request.Files[0];
				if (ModelState.IsValid)
				{
					if (file == null)
					{
						ModelState.AddModelError("File", "Please Upload Your file");
					}
					else if (file.ContentLength > 0)
					{
						int MaxContentLength = 1024 * 1024 * 3; //3 MB
						string[] AllowedFileExtensions = new string[] { ".xlsx",".xls" };
						var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
						if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
						{
							ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
						}

						else if (file.ContentLength > MaxContentLength)
						{
							ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
						}
						else
						{
							//TO:DO

							ModelState.Clear();
							ViewBag.Message = "File uploaded successfully";

							var fileName = Path.GetFileName("XLSFILE.XLS");
							var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
							ModelState.Clear();
							if (ext == ".xls")
							{
								file.SaveAs(path);
								var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
								rows = ReadSalaryXLS(fs, model, id);
							}
							else
							{


								ExcelPackage ep = new ExcelPackage(file.InputStream);
								ExcelWorksheet workSheet = ep.Workbook.Worksheets.First();
								List<TwoDContainer> textarray = new List<TwoDContainer>();
								List<string> excelHeaders = new List<string>();
								List<string> staff = new List<string>();
								DataTable table = new DataTable();
								var staffcol = new TwoDContainer();
								for (var i = workSheet.Dimension.Start.Row; i <= workSheet.Dimension.End.Row; i++)
								{
									var head = workSheet.Dimension.Start.Column;
									for (var j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
									{
										if (workSheet.Cells[i, j].Text == "")
										{
											head++;
										}
										if (i == 6 && j >= 4)
										{
											if (workSheet.Cells[i, j].Text != "#NAME?")
												excelHeaders.Add(workSheet.Cells[i, j].Text);
										}
										if (i > 7 && j >= 4 && workSheet.Cells[i, j].Text != "#NAME?")
										{
											TwoDContainer d2array = new TwoDContainer();
											d2array.Row = i;
											if (j > 39)
											{
												d2array.Coloumn = 40;
											}
											else
											{
												d2array.Coloumn = j;
											}
											d2array.Content = workSheet.Cells[i, j].Text.Replace(",", "");
											if (j == 7 || j == 8 || j == 18)
											{
												if (d2array.Content == "")
													d2array.Content = DateTime.Now.ToString();
											}
											if (d2array.Content == "")
												d2array.Content = "0";

											textarray.Add(d2array);
										}

										if (j == 39)
										{
											j = j + 189;
										}
									}
									if (head >= 20)
									{
										textarray.RemoveAll(x => x.Row == i);
									}

								}

								foreach (var headers in excelHeaders)
								{
									table.Columns.Add(headers);
								}

								var srow = textarray.FirstOrDefault().Row;
								var icol = textarray.FirstOrDefault().Coloumn;

								var p = 0;
								while (p <= textarray.Count - 1)
								{

									var j = 4;
									var nrow = table.NewRow();
									var skip = true;
									while (textarray.FindIndex(x => x.Coloumn == j && x.Row == srow) != -1)
									{
										var idx = textarray.FindIndex(x => x.Row == srow && x.Coloumn == j);
										nrow.SetField<string>(j - 4, textarray[idx].Content);
										p++;
										j++;
										skip = false;
									}
									if (!skip)
									{

										table.Rows.Add(nrow);
									}
									else
									{
										p++;
									}
									srow++;
								}

								con.Open();
								SqlCommand com = new SqlCommand("InsertSalary", con);
								com.CommandType = CommandType.StoredProcedure;
								com.Parameters.AddWithValue("@salTable", table);
								com.Parameters.AddWithValue("@LocId", model.LocationId);
								com.Parameters.AddWithValue("@Month", model.Month);
								com.Parameters.AddWithValue("@Year", model.Year);
								com.Parameters.AddWithValue("@Id", id);
								com.Parameters.AddWithValue("@Update", model.Update);
								com.ExecuteNonQuery();
								con.Close();
								rows = table.Rows.Count;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
				Logger.Write(ex.Message);
			}
			//return RedirectToAction("Index", "DashBoard"); ;
			return Json(new { rows });
		}
		/// <summary>
		/// Salary Page
		/// </summary>
		/// <returns>Returns Salary Page as Result</returns>
		public ActionResult Salary()
		{
			SalaryInputModel sim = new SalaryInputModel();
			LocationDrp(sim);
			sim.Month = DateTime.Now.Month;
			sim.Year = DateTime.Now.Year;
			sim.Update = false;
			return View(sim);
		}
		/// <summary>
		/// Salary Search Page
		/// </summary>
		/// <returns>Returns Salary Search Page</returns>
		public ActionResult SalarySearch()
		{
			var searchList = new[] { "Select Search", "EmpId", "EmpName", "Designation", "Department", "Category", "UAN", "PAN" };
			SalarySearchModel ssm = new SalarySearchModel();
			con.Open();
			SqlCommand com = new SqlCommand("getBranchLocationDropDown", con);
			com.CommandType = CommandType.StoredProcedure;

			DataTable btable = new DataTable();
			btable.Columns.Add("BranchID");
			btable.Rows.Add(13);
			com.Parameters.AddWithValue("@BranchList", btable);
			ssm.LocationList = new List<SelectListItem>();
			ssm.LocationList.Add(new SelectListItem { Text = "All", Value = "0" });
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					ssm.LocationList.Add(new SelectListItem { Text = sdr["LocationName"].ToString(), Value = sdr["Id"].ToString() });
				}
			}
			com.Parameters.Clear();
			ssm.SearchList = new List<SelectListItem>();
			foreach (var cat in searchList)
			{
				ssm.SearchList.Add(new SelectListItem { Text = cat, Value = cat });
			}
			com = new SqlCommand("getSearchCategory", con);
			com.CommandType = CommandType.StoredProcedure;
			ssm.SearchColumn = new List<SearchCategory>();
			var idx = 0;
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					ssm.SearchColumn.Add(new SearchCategory { Sno = idx + 1, Search = sdr["Search"].ToString(), CheckBox = false });
					idx++;
				}
			}
			con.Close();
			ssm.Month = DateTime.Now.Month;
			ssm.Year = DateTime.Now.Year;

			return View(ssm);
		}
		/// <summary>
		/// Check Uploaded Excel file for a given date Exist or Not
		/// </summary>
		/// <param name="Model">Salary Input Model</param>
		/// <returns>Returns Json Response whether data exist or not</returns>
		[HttpPost]
		public JsonResult CheckExcelExist(SalaryInputModel Model)
		{
			con.Open();
			SqlCommand com = new SqlCommand("checkFilterExist", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@LocId", Model.LocationId);
			com.Parameters.AddWithValue("@Month", Model.Month);
			com.Parameters.AddWithValue("@Year", Model.Year);
			var re = com.ExecuteScalar();
			con.Close();
			bool update = false;
			if (re != DBNull.Value && re != null)			
				update = true;
			if (update == true)
				return Json(new { id = re, update });
			else
				return Json(new { id = 0, update });
		}
		/// <summary>
		/// Searches Database for specified Search Query and Giving Output
		/// </summary>
		/// <param name="Model">ExcelQueryModel</param>
		/// <returns>Json Result as Table</returns>
		[HttpPost]
		public JsonResult ExcelQuery(ExcelQueryModel Model)
		{
			var Month = Model.Month;
			var cols = string.Join(", ", Model.SearchList);
			var rowList = new List<List<string>>();
			con.Open();
			SqlCommand com = new SqlCommand("getExcelQuery", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@Col", cols);
			com.Parameters.AddWithValue("@LocationId", Model.LocationId);
			com.Parameters.AddWithValue("@Month", Model.Month);
			com.Parameters.AddWithValue("@Year", Model.Year);
			com.Parameters.AddWithValue("@Category", Model.SelectedCategory);
			com.Parameters.AddWithValue("@SearchValue", Model.SearchValue);
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					var collist = new List<string>();
					foreach (var col in Model.SearchList)
					{

						if (col == "DOJ" || col == "DOL" || col == "PFDate")
						{
							collist.Add(DateTime.Parse(sdr[col].ToString()).ToShortDateString());
						}
						else
						{
							collist.Add(sdr[col].ToString());
						}
					}
					rowList.Add(collist);
				}
			}
			if (rowList.Count != 0)
				rowList.Insert(0, Model.SearchList);
			con.Close();
			return Json(new { rowList });
		}
		/// <summary>
		/// Exporting Salary Data with given filters
		/// </summary>
		/// <param name="Model">Salary Input Model</param>
		/// <returns></returns>
		[HttpGet]
		public FileResult ExportExcel(string Model)
		{

			JavaScriptSerializer ser = new JavaScriptSerializer();
			ExcelQueryModel model = (ExcelQueryModel)ser.Deserialize(Model, typeof(ExcelQueryModel));

			var Month = model.Month;
			var cols = string.Join(", ", model.SearchList);
			var dt = new DataTable();

			con.Open();
			SqlCommand com = new SqlCommand("getExcelQuery", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@Col", cols);
			com.Parameters.AddWithValue("@LocationId", model.LocationId);
			com.Parameters.AddWithValue("@Month", model.Month);
			com.Parameters.AddWithValue("@Year", model.Year);
			com.Parameters.AddWithValue("@Category", model.SelectedCategory);
			com.Parameters.AddWithValue("@SearchValue", model.SearchValue);
			byte[] result = null;

			
			string FileName = "Invoice" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
			using (ExcelPackage pck = new ExcelPackage())
			{
				ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ExportSheet");
				foreach (var col in model.SearchList)
				{
					dt.Columns.Add(col);
				}
				SqlDataAdapter sda = new SqlDataAdapter();
				sda.SelectCommand = com;
				sda.Fill(dt);
				//dt.Columns.Add()
				//dt.Rows.InsertAt(new DataRow(),0)
				/*
				string attachment = "attachment; filename=city.xls";
				Response.ClearContent();
				Response.AddHeader("content-disposition", attachment);
				Response.ContentType = "application/vnd.ms-excel";
				string tab = "";
				foreach (DataColumn dc in dt.Columns)
				{
					Response.Write(tab + dc.ColumnName);
					tab = "\t";
				}
				Response.Write("\n");
				int i;
				foreach (DataRow dr in dt.Rows)
				{
					tab = "";
					for (i = 0; i < dt.Columns.Count; i++)
					{
						Response.Write(tab + dr[i].ToString());
						tab = "\t";
					}
					Response.Write("\n");
				}
				Response.End();
				 */
				
				ws.Cells["A1"].LoadFromDataTable(dt, true);
				ws.Row(1).Style.Font.Bold = true;
				ws.Row(1).Style.Font.Name = "Arial";
				ws.Row(1).Style.Font.Size = 11;
				ws.View.FreezePanes(2, 1);
				result = pck.GetAsByteArray();
				
			}

			con.Close();
			return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
			//Response.Clear();
			//Response.Buffer = true;
			//Response.ContentType = "application/vnd.ms-excel";
			//Response.AddHeader("content-disposition", "attachment; filename=Statement_" + "Downloadfile" + ".xlxs");
			//Response.Write(file);
			//Response.Flush();
			//return file;
			//return Json(new { }) ;
		}
		#endregion
		#region OverTime
		
		public JsonResult TallyOTUpdate(string LocationId, string Month, string Year, string TallyComment)
		{
			DataTable dt = new DataTable();
			con.Open();
			SqlCommand cmm = new SqlCommand("GetOverTimeByYearMonthLocation", con);
			cmm.Parameters.AddWithValue("@LocationId", LocationId);
			cmm.Parameters.AddWithValue("@Month", Month);
			cmm.Parameters.AddWithValue("@Year", Year);
			SqlDataAdapter sda = new SqlDataAdapter();
			sda.SelectCommand = cmm;
			sda.Fill(dt);
			con.Close();
			return Json(new { });
		}
		public JsonResult CheckOTTallyExist(string LocationId, string Month, string Year)
		{
			var exist = false;
			try
			{
				con.Open();
				SqlCommand cmd = new SqlCommand("checkOTexisttally", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@LocationId", LocationId);
				cmd.Parameters.AddWithValue("@Month", Month);
				cmd.Parameters.AddWithValue("@Year", Year);
				using (SqlDataReader sdr = cmd.ExecuteReader())
				{
					while (sdr.Read())
					{
						exist = bool.Parse(sdr["TallyUpdated"].ToString());
					}
				}
				con.Close();
			}
			catch(Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
				return Json(new { exist });
			}
			return Json(new { exist });
		}
		/// <summary>
		/// Shows OverTime Page
		/// </summary>
		/// <returns>Returns  OverTime Page</returns>
		public ActionResult OverTime()
		{
			SalaryInputModel sim = new SalaryInputModel();
			LocationDrp(sim);
			sim.Month = DateTime.Now.Month;
			sim.Year = DateTime.Now.Year;
			sim.Update = false;
			return View(sim);

		}
		/// <summary>
		/// OverTIme Search Page
		/// </summary>
		/// <returns></returns>
		public ActionResult OverTimeSearch()
		{
			var searchList = new[] { "Select Search", "EmpName", "Designation", "Category" };
			SalarySearchModel ssm = new SalarySearchModel();
			con.Open();
			SqlCommand com = new SqlCommand("getBranchLocationDropDown", con);
			com.CommandType = CommandType.StoredProcedure;

			DataTable btable = new DataTable();
			btable.Columns.Add("BranchID");
			btable.Rows.Add(13);
			com.Parameters.AddWithValue("@BranchList", btable);
			ssm.LocationList = new List<SelectListItem>();
			ssm.LocationList.Add(new SelectListItem { Text = "ALL",Value="0" });
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					ssm.LocationList.Add(new SelectListItem { Text = sdr["LocationName"].ToString(), Value = sdr["Id"].ToString() });
				}
			}
			com.Parameters.Clear();
			ssm.SearchList = new List<SelectListItem>();
			foreach (var cat in searchList)
			{
				ssm.SearchList.Add(new SelectListItem { Text = cat, Value = cat });
			}
			com = new SqlCommand("getOTSearchCategory", con);
			com.CommandType = CommandType.StoredProcedure;
			ssm.SearchColumn = new List<SearchCategory>();
			var idx = 0;
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					ssm.SearchColumn.Add(new SearchCategory { Sno = idx + 1, Search = sdr["Search"].ToString(), CheckBox = false });
					idx++;
				}
			}
			con.Close();
			ssm.Month = DateTime.Now.Month;
			ssm.Year = DateTime.Now.Year;

			return View(ssm);
		}
		
		/// <summary>
		/// Excel OverTime Search Based on Filter
		/// </summary>
		/// <param name="Model">ExcelQueryModel</param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult ExcelOTQuery(ExcelQueryModel Model)
		{
			var Month = Model.Month;
			var cols = string.Join(", ", Model.SearchList);
			var rowList = new List<List<string>>();

			con.Open();
			SqlCommand com = new SqlCommand("getOTExcelQuery", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@Col", cols);
			com.Parameters.AddWithValue("@LocationId", Model.LocationId);
			com.Parameters.AddWithValue("@Month", Model.Month);
			com.Parameters.AddWithValue("@Year", Model.Year);
			com.Parameters.AddWithValue("@Category", Model.SelectedCategory);
			com.Parameters.AddWithValue("@SearchValue", Model.SearchValue);
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					var collist = new List<string>();
					foreach (var col in Model.SearchList)
					{


						collist.Add(sdr[col].ToString());

					}
					rowList.Add(collist);
				}
			}
			if (rowList.Count != 0)
				rowList.Insert(0, Model.SearchList);
			con.Close();
			return Json(new { rowList });
		}
		/// <summary>
		/// Excel OverTime Search Based on Filter
		/// </summary>
		/// <param name="Model">ExcelQueryModel</param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult ExcelIncQuery(ExcelQueryModel Model)
		{
			var Month = Model.Month;
			var cols = string.Join(", ", Model.SearchList);
			var rowList = new List<List<string>>();

			con.Open();
			SqlCommand com = new SqlCommand("getIncExcelQuery", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@Col", cols);
			com.Parameters.AddWithValue("@LocationId", Model.LocationId);
			com.Parameters.AddWithValue("@Month", Model.Month);
			com.Parameters.AddWithValue("@Year", Model.Year);
			com.Parameters.AddWithValue("@Category", Model.SelectedCategory);
			com.Parameters.AddWithValue("@SearchValue", Model.SearchValue);
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					var collist = new List<string>();
					foreach (var col in Model.SearchList)
					{			 
						collist.Add(sdr[col].ToString());		
					}
					rowList.Add(collist);
				}
			}
			if (rowList.Count != 0)
				rowList.Insert(0, Model.SearchList);
			con.Close();
			return Json(new { rowList });
		}
		/// <summary>
		/// Exporting OverTime Excel Model
		/// </summary>
		/// <param name="Model">Returns Excel File with Filtered Data</param>
		/// <returns></returns>
		public FileResult ExportOTExcel(string Model)
		{

			JavaScriptSerializer ser = new JavaScriptSerializer();
			ExcelQueryModel model = (ExcelQueryModel)ser.Deserialize(Model, typeof(ExcelQueryModel));

			var Month = model.Month;
			var cols = string.Join(", ", model.SearchList);
			var dt = new DataTable();

			con.Open();
			SqlCommand com = new SqlCommand("getOTExcelQuery", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@Col", cols);
			com.Parameters.AddWithValue("@LocationId", model.LocationId);
			com.Parameters.AddWithValue("@Month", model.Month);
			com.Parameters.AddWithValue("@Year", model.Year);
			com.Parameters.AddWithValue("@Category", model.SelectedCategory);
			com.Parameters.AddWithValue("@SearchValue", model.SearchValue);
			byte[] result = null;
			string FileName = "Invoice" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
			using (ExcelPackage pck = new ExcelPackage())
			{
				ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ExportSheet");
				foreach (var col in model.SearchList)
				{
					dt.Columns.Add(col);
				}
				SqlDataAdapter sda = new SqlDataAdapter();
				sda.SelectCommand = com;
				sda.Fill(dt);

				ws.Cells["A1"].LoadFromDataTable(dt, true);

				ws.Row(1).Style.Font.Bold = true;
				ws.Row(1).Style.Font.Name = "Arial";
				ws.Row(1).Style.Font.Size = 11;
				ws.View.FreezePanes(2, 1);
				//ws.Cells[1, 1].Style.Font.Bold = true;

				result = pck.GetAsByteArray();
			}

			con.Close();
			return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
			//Response.Clear();
			//Response.Buffer = true;
			//Response.ContentType = "application/vnd.ms-excel";
			//Response.AddHeader("content-disposition", "attachment; filename=Statement_" + "Downloadfile" + ".xlxs");
			//Response.Write(file);
			//Response.Flush();
			//return file;
			//return Json(new { }) ;
		}
		/// <summary>
		/// Exporting OverTime Excel Model
		/// </summary>
		/// <param name="Model">Returns Excel File with Filtered Data</param>
		/// <returns></returns>
		public FileResult ExportIncExcel(string Model)
		{

			JavaScriptSerializer ser = new JavaScriptSerializer();
			ExcelQueryModel model = (ExcelQueryModel)ser.Deserialize(Model, typeof(ExcelQueryModel));

			var Month = model.Month;
			var cols = string.Join(", ", model.SearchList);
			var dt = new DataTable();

			con.Open();
			SqlCommand com = new SqlCommand("getIncExcelQuery", con);
			com.CommandType = CommandType.StoredProcedure;
			com.Parameters.AddWithValue("@Col", cols);
			com.Parameters.AddWithValue("@LocationId", model.LocationId);
			com.Parameters.AddWithValue("@Month", model.Month);
			com.Parameters.AddWithValue("@Year", model.Year);
			com.Parameters.AddWithValue("@Category", model.SelectedCategory);
			com.Parameters.AddWithValue("@SearchValue", model.SearchValue);
			byte[] result = null;
			string FileName = "Invoice" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
			using (ExcelPackage pck = new ExcelPackage())
			{
				ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ExportSheet");
				foreach (var col in model.SearchList)
				{
					dt.Columns.Add(col);
				}
				SqlDataAdapter sda = new SqlDataAdapter();
				sda.SelectCommand = com;
				sda.Fill(dt);

				ws.Cells["A1"].LoadFromDataTable(dt, true);

				ws.Row(1).Style.Font.Bold = true;
				ws.Row(1).Style.Font.Name = "Arial";
				ws.Row(1).Style.Font.Size = 11;
				ws.View.FreezePanes(2, 1);
				//ws.Cells[1, 1].Style.Font.Bold = true;

				result = pck.GetAsByteArray();
			}

			con.Close();
			return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
			
		}
		/// <summary>
		/// Checking OverTime data Exist for given filters
		/// </summary>
		/// <param name="Model">Salary Input Model</param>
		/// <returns>Return JsonResult to Confirm Data Exist or Not</returns>
		[HttpPost]
		public JsonResult CheckOTExist(SalaryInputModel Model)
		{
			try
			{
				con.Open();
				SqlCommand com = new SqlCommand("checkOTFilterExist", con);
				com.CommandType = CommandType.StoredProcedure;
				com.Parameters.AddWithValue("@LocId", Model.LocationId);
				com.Parameters.AddWithValue("@Month", Model.Month);
				com.Parameters.AddWithValue("@Year", Model.Year);
				var re = com.ExecuteScalar();
				con.Close();
				bool update = false;
				if (re != DBNull.Value && re != null)
					update = true;
				if (update == true)
					return Json(new { id = re, update });
				else
					return Json(new { id = 0, update });
			}
			catch(Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
				return Json(new {ex.Message });
			}
		}
		[HttpPost]
		public JsonResult CheckIncExist(SalaryInputModel Model)
		{
			try
			{
				con.Open();
				SqlCommand com = new SqlCommand("checkIncFilterExist", con);
				com.CommandType = CommandType.StoredProcedure;
				com.Parameters.AddWithValue("@LocId", Model.LocationId);
				com.Parameters.AddWithValue("@Month", Model.Month);
				com.Parameters.AddWithValue("@Year", Model.Year);
				var re = com.ExecuteScalar();
				con.Close();
				bool update = false;
				if (re != DBNull.Value && re != null)
					update = true;
				if (update == true)
					return Json(new { id = re, update });
				else
					return Json(new { id = 0, update });
			}
			catch (Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
				return Json(new { ex.Message }) ;
			}
		}
		/// <summary>
		/// XLS Cell to String Converter
		/// </summary>
		/// <param name="cell">ICell as Input</param>
		/// <returns>Returns String Value of Cell</returns>
		private string CellToString(ICell cell)
		{
			if (cell != null)
			{
				// TODO: you can add more cell types capability, e. g. formula
				switch (cell.CellType)
				{
					case NPOI.SS.UserModel.CellType.Numeric:
						return cell.NumericCellValue.ToString();

					case NPOI.SS.UserModel.CellType.String:
						return cell.StringCellValue;
					case NPOI.SS.UserModel.CellType.Formula:
						return cell.NumericCellValue.ToString();
					default:
						return cell.StringCellValue.ToString();
				}

			}
			else
			{
				return "";
			}
		}
		/// <summary>
		/// Reads data of Old XLS FIle
		/// </summary>
		/// <param name="fs">FileStram as Parameter</param>
		/// <param name="model">SalaryInputModel as Parameter</param>
		/// <param name="id">Id as Parameter</param>
		private int ReadXLS(FileStream fs, SalaryInputModel model, string id)
		{
			int rows = 0;
			List<DataTable> tablelist = new List<DataTable>();
			DataTable dt = new DataTable();
			var wb = new HSSFWorkbook(fs);
			//var sh = (HSSFSheet)wb[1];
			//var i = 0;
			/*
			while(sh.GetRow(i)!= null)
			for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
			{
				var cell = sh.GetRow(i).GetCell(j);

				if (cell != null)
				{
					// TODO: you can add more cell types capability, e. g. formula
					switch (cell.CellType)
					{
						case NPOI.SS.UserModel.CellType.Numeric:
							var text = sh.GetRow(i).GetCell(j).NumericCellValue;
							break;
						case NPOI.SS.UserModel.CellType.String:
							var tex = sh.GetRow(i).GetCell(j).StringCellValue;
							break;
					}							
				}
			}
			//i++;
			*/
			for (var z = 0; z < wb.NumberOfSheets - 1; z++)
			{

				var sh = (HSSFSheet)wb[z];

				List<TwoDContainer> textarray = new List<TwoDContainer>();
				List<string> excelHeaders = new List<string>() { "EmpName", "Designation", "GrossSalary", "Basic", "PerDaySalary", "PerHourSalary", "TotalHours", "DoubleHours", "OTAmount", "ESI", "NetAmount", "BankAcNo", "Bank" };
				DataTable table = new DataTable();

				var staffcol = new TwoDContainer();
				for (var i = 0; sh.GetRow(i) != null; i++)
				{

					var head = 0;
					for (var j = 0; sh.GetRow(i).GetCell(j) != null; j++)
					{
						var data = CellToString(sh.GetRow(i).GetCell(j));
						if (data == "")
						{
							head++;
						}
						else
						{
							var text = data;
						}
						/*
						if (i == 4 && j >= 1)
						{
							//table.Columns.Add(workSheet.Cells[i, j].Text);
							if (data != "")
								excelHeaders.Add(data);
						}*/
						if (i > 5 && j >= 1)
						{
							TwoDContainer d2array = new TwoDContainer();
							d2array.Row = i;
							if (j > 39)
							{
								d2array.Coloumn = 40;
							}
							else
							{
								d2array.Coloumn = j;
							}

							d2array.Content = data.Replace(",", "");
							textarray.Add(d2array);

						}
					}

					if (head > 3)
					{
						textarray.RemoveAll(x => x.Row == i);
					}



				}

				foreach (var headers in excelHeaders)
				{
					table.Columns.Add(headers);
				}
				//table.Columns.Add("Nid");
				var srow = textarray.FirstOrDefault().Row;
				var icol = textarray.FirstOrDefault().Coloumn;

				var p = 0;
				while (p <= textarray.Count - 1)
				{


					var j = 1;
					var nrow = table.NewRow();
					var skip = true;
					while (textarray.FindIndex(x => x.Coloumn == j && x.Row == srow) != -1 && j < excelHeaders.Count + 1)
					{
						var idx = textarray.FindIndex(x => x.Row == srow && x.Coloumn == j);
						if (textarray[idx].Content == "")
							textarray[idx].Content = "0";
						nrow.SetField<string>(j - 1, textarray[idx].Content);
						p++;
						j++;
						skip = false;
					}

					//p++;
					if (!skip)
					{
						//nrow.SetField<string>(j - 4, id);
						table.Rows.Add(nrow);
					}
					else
					{
						p++;
					}
					srow++;


				}
				tablelist.Add(table);
			}
			var nid = 0;
			for (var z = 0; z < wb.NumberOfSheets - 1; z++)
			{
				var sh = wb[z];
				con.Open();
				SqlCommand com = new SqlCommand("InsertOT", con);
				com.CommandType = CommandType.StoredProcedure;
				com.Parameters.AddWithValue("@tblovertime", tablelist[z]);
				com.Parameters.AddWithValue("@LocId", model.LocationId);
				com.Parameters.AddWithValue("@Month", model.Month);
				com.Parameters.AddWithValue("@Year", model.Year);
				com.Parameters.AddWithValue("@Id", id);
				com.Parameters.AddWithValue("@Update", model.Update);
				com.Parameters.AddWithValue("@Cat", sh.SheetName);
				com.Parameters.AddWithValue("@Loop", z);
				com.Parameters.AddWithValue("@Nid", nid);
				nid = (int)com.ExecuteScalar();
				model.Update = false;
				rows += tablelist[z].Rows.Count;
				con.Close();
			}

			return rows;

		}
		/// <summary>
		/// Upload Action for OverTime Excels
		/// </summary>
		/// <param name="model">SalaryInputModel as Parameter</param>
		/// <returns>Returns JsonResult as Confirmation</returns>
		[HttpPost]
		public JsonResult OverTimeUpload(SalaryInputModel model)
		{
			int rows = 0;
			try
			{
				string id = "0";

				if (model.Update)
					id = model.UpdateId.ToString();
				var file = Request.Files[0];
				if (ModelState.IsValid)
				{
					if (file == null)
					{
						ModelState.AddModelError("File", "Please Upload Your file");
					}
					else if (file.ContentLength > 0)
					{
						int MaxContentLength = 1024 * 1024 * 7; //7 MB
						string[] AllowedFileExtensions = new string[] { ".xlsx", ".xls" };
						var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
						if (!AllowedFileExtensions.Contains(ext))
						{
							ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
						}

						else if (file.ContentLength > MaxContentLength)
						{
							ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
						}
						else
						{
							//TO:DO
							var fileName = Path.GetFileName("XLSFILE.XLS");
							var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
							ModelState.Clear();




							if (ext == ".xls")
							{
								file.SaveAs(path);
								var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
								rows  = ReadXLS(fs, model, id);
							}

							else
							{

								ViewBag.Message = "File uploaded successfully";
								ExcelPackage ep = new ExcelPackage(file.InputStream);
								List<string> staff = new List<string>();
								List<DataTable> tablelist = new List<DataTable>();
								for (var z = 1; z <= ep.Workbook.Worksheets.Count - 1; z++)
								{
									ExcelWorksheet workSheet = ep.Workbook.Worksheets[z];
									List<TwoDContainer> textarray = new List<TwoDContainer>();
									List<string> excelHeaders = new List<string>() { "EmpName", "Designation", "GrossSalary", "Basic", "PerDaySalary", "PerHourSalary", "TotalHours", "DoubleHours", "OTAmount", "ESI", "NetAmount", "BankAcNo", "Bank" };
									DataTable table = new DataTable();

									var staffcol = new TwoDContainer();
									for (var i = workSheet.Dimension.Start.Row; i <= workSheet.Dimension.End.Row; i++)
									{

										var head = workSheet.Dimension.Start.Column;
										for (var j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
										{
											if (workSheet.Cells[i, j].Text == "")
											{
												head++;
											}
											/*
											if (i == 5 && j >= 2)
											{
												//table.Columns.Add(workSheet.Cells[i, j].Text);
												if (workSheet.Cells[i, j].Text != "")
													excelHeaders.Add(workSheet.Cells[i, j].Text);
											} */
											if (i > 6 && j >= 2)
											{
												TwoDContainer d2array = new TwoDContainer();
												d2array.Row = i;
												if (j > 39)
												{
													d2array.Coloumn = 40;
												}
												else
												{
													d2array.Coloumn = j;
												}

												d2array.Content = workSheet.Cells[i, j].Text.Replace(",", "");
												textarray.Add(d2array);
												if (workSheet.Dimension.End.Column == j)
												{
													TwoDContainer d2 = new TwoDContainer();
													//d2array.Row = i;
													d2.Row = i;
													d2.Coloumn = j + 1;
													d2.Content = "";
													textarray.Add(d2);
												}
											}
										}

										if (head > 3)
										{
											textarray.RemoveAll(x => x.Row == i);
										}
									}

									foreach (var headers in excelHeaders)
									{
										table.Columns.Add(headers);

									}
									//table.Columns.Add("Bank");
									//table.Columns.Add("Nid");
									var srow = textarray.FirstOrDefault().Row;
									var icol = textarray.FirstOrDefault().Coloumn;

									var p = 0;
									while (p <= textarray.Count - 1)
									{
										var j = 2;
										var nrow = table.NewRow();
										var skip = true;
										while (textarray.FindIndex(x => x.Coloumn == j && x.Row == srow) != -1 && j < excelHeaders.Count + 2)
										{
											var idx = textarray.FindIndex(x => x.Row == srow && x.Coloumn == j);
											nrow.SetField<string>(j - 2, textarray[idx].Content);
											//nrow.SetField<string>(j - 1, "");
											p++;
											j++;
											skip = false;
										}

								
										if (!skip)
										{
											table.Rows.Add(nrow);
										}
										else
										{
											p++;
										}
										srow++;
									}

									tablelist.Add(table);
								}

								var nid = 0;
								for (var z = 0; z < ep.Workbook.Worksheets.Count - 1; z++)
								{
									var sh = ep.Workbook.Worksheets[z + 1];
									con.Open();
									SqlCommand com = new SqlCommand("InsertOT", con);
									com.CommandType = CommandType.StoredProcedure;
									com.Parameters.AddWithValue("@tblovertime", tablelist[z]);
									com.Parameters.AddWithValue("@LocId", model.LocationId);
									com.Parameters.AddWithValue("@Month", model.Month);
									com.Parameters.AddWithValue("@Year", model.Year);
									com.Parameters.AddWithValue("@Id", id);
									com.Parameters.AddWithValue("@Update", model.Update);
									com.Parameters.AddWithValue("@Cat", sh.Name);
									com.Parameters.AddWithValue("@Loop", z);
									com.Parameters.AddWithValue("@Nid", nid);
									nid = (int)com.ExecuteScalar();
									model.Update = false;
									rows += tablelist[z].Rows.Count;
									con.Close();
								}

								/*
								con.Open();
								SqlCommand com = new SqlCommand("InsertSalary", con);
								com.CommandType = CommandType.StoredProcedure;
								com.Parameters.AddWithValue("@salTable", table);
								com.Parameters.AddWithValue("@LocId", model.LocationId);
								com.Parameters.AddWithValue("@Month", model.Month);
								com.Parameters.AddWithValue("@Year", model.Year);
								com.Parameters.AddWithValue("@Id", id);
								com.Parameters.AddWithValue("@Update", model.Update);
								com.ExecuteNonQuery();


								con.Close();
								*/
								/*
								var cell = textarray.Where(x => x.Content == "Executives");
							   // var r = cell.Row;//6
							   // var c = cell.Coloumn;//3 // 
								var final  =  excelHeaders;*/

							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
				Logger.Write(ex.Message);
			}
			//return RedirectToAction("Index", "DashBoard"); ;
			return Json(new { rows });
		}
		#endregion
		#region Incentives
		public JsonResult TallyIncUpdate(string LocationId, string Month, string Year, string TallyComment)
		{
			DataTable dt = new DataTable();
			con.Open();
			SqlCommand cmm = new SqlCommand("GetIncentivesByYearMonthLocation", con);
			cmm.Parameters.AddWithValue("@LocationId",LocationId);
			cmm.Parameters.AddWithValue("@Month",Month);
			cmm.Parameters.AddWithValue("@Year",Year);
			SqlDataAdapter sda = new SqlDataAdapter();
			sda.SelectCommand = cmm;
			sda.Fill(dt);
			con.Close();
			return Json(new { });
		}
		public JsonResult CheckIncTallyExist(string LocationId, string Month, string Year)
		{
			var exist = false;
			try
			{
				con.Open();
				SqlCommand cmd = new SqlCommand("checkIncexisttally", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@LocationId", LocationId);
				cmd.Parameters.AddWithValue("@Month", Month);
				cmd.Parameters.AddWithValue("@Year", Year);
				using (SqlDataReader sdr = cmd.ExecuteReader())
				{
					while (sdr.Read())
					{
						exist = bool.Parse(sdr["TallyUpdated"].ToString());
					}
				}
				con.Close();
			}
			catch(Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
			}
			return Json(new { exist });
		}
		public ActionResult Incentives()
		{
			SalaryInputModel sim = new SalaryInputModel();
			LocationDrp(sim);
			sim.Month = DateTime.Now.Month;
			sim.Year = DateTime.Now.Year;
			sim.Update = false;
			return View(sim);

		}
		public ActionResult IncentiveSearch()
		{
			var searchList = new[] { "Select Search", "EmpName", "Designation", "Category" };
			SalarySearchModel ssm = new SalarySearchModel();
			con.Open();
			SqlCommand com = new SqlCommand("getBranchLocationDropDown", con);
			com.CommandType = CommandType.StoredProcedure;

			DataTable btable = new DataTable();
			btable.Columns.Add("BranchID");
			btable.Rows.Add(13);
			com.Parameters.AddWithValue("@BranchList", btable);
			ssm.LocationList = new List<SelectListItem>();
			ssm.LocationList.Add(new SelectListItem { Text = "All", Value = "0" });
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					ssm.LocationList.Add(new SelectListItem { Text = sdr["LocationName"].ToString(), Value = sdr["Id"].ToString() });
				}
			}
			com.Parameters.Clear();
			ssm.SearchList = new List<SelectListItem>();
			foreach (var cat in searchList)
			{
				ssm.SearchList.Add(new SelectListItem { Text = cat, Value = cat });
			}
			com = new SqlCommand("getIncSearchCategory", con);
			com.CommandType = CommandType.StoredProcedure;
			ssm.SearchColumn = new List<SearchCategory>();
			var idx = 0;
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					ssm.SearchColumn.Add(new SearchCategory { Sno = idx + 1, Search = sdr["Search"].ToString(), CheckBox = false });
					idx++;
				}
			}
			con.Close();
			ssm.Month = DateTime.Now.Month;
			ssm.Year = DateTime.Now.Year;

			return View(ssm);
		}
		private int ReadIncentiveXLS(FileStream fs, SalaryInputModel model, string id)
		{
			List<DataTable> tablelist = new List<DataTable>();
			DataTable dt = new DataTable();
			var wb = new HSSFWorkbook(fs);
			
			for (var z = 0; z < wb.NumberOfSheets -2; z++)
			{

				var sh = (HSSFSheet)wb[z];

				List<TwoDContainer> textarray = new List<TwoDContainer>();
				List<string> excelHeaders = new List<string>() { "Emp ID","Employee Name", "Designation", "Bank A/c No.", "Points","Incentive amount"};
				DataTable table = new DataTable();

				var staffcol = new TwoDContainer();
				for (var i = 0; sh.GetRow(i) != null; i++)
				{
					
					var head = 0;
					for (var j = 0; sh.GetRow(i).GetCell(j) != null; j++)
					{
						var data = CellToString(sh.GetRow(i).GetCell(j,MissingCellPolicy.RETURN_NULL_AND_BLANK));
						if (data == "")
						{
							head++;
						}
						else
						{
							var text = data;
						}
						if (i > 5 && j >= 1)
						{
							TwoDContainer d2array = new TwoDContainer();
							d2array.Row = i;			
							d2array.Coloumn = j;
							d2array.Content = data.Replace(",", "");
							textarray.Add(d2array);
						}
					}

					if (head > 3)
					{
						textarray.RemoveAll(x => x.Row == i);
					}



				}

				foreach (var headers in excelHeaders)
				{
					table.Columns.Add(headers);
				}
				//table.Columns.Add("Nid");
				var srow = textarray.FirstOrDefault().Row;
				var icol = textarray.FirstOrDefault().Coloumn;

				var p = 0;
				while (p <= textarray.Count - 1)
				{


					var j = 1;
					var nrow = table.NewRow();
					var skip = true;
					while (textarray.FindIndex(x => x.Coloumn == j && x.Row == srow) != -1 && j < excelHeaders.Count + 1)
					{
						var idx = textarray.FindIndex(x => x.Row == srow && x.Coloumn == j);
						if (textarray[idx].Content == "")
							textarray[idx].Content = "0";
						nrow.SetField<string>(j - 1, textarray[idx].Content);
						p++;
						j++;
						skip = false;
					}

					//p++;
					if (!skip)
					{
						//nrow.SetField<string>(j - 4, id);
						table.Rows.Add(nrow);
					}
					else
					{
						p++;
					}
					srow++;


				}
				tablelist.Add(table);
			}
			var nid = 0;
			var rowcount = 0;
			for (var z = 0; z < wb.NumberOfSheets -2; z++)
			{
				var sh = wb[z];
				con.Open();
				SqlCommand com = new SqlCommand("InsertIncentive", con);
				com.CommandType = CommandType.StoredProcedure;
				com.Parameters.AddWithValue("@incentiveTable", tablelist[z]);
				com.Parameters.AddWithValue("@LocId", model.LocationId);
				com.Parameters.AddWithValue("@Month", model.Month);
				com.Parameters.AddWithValue("@Year", model.Year);
				com.Parameters.AddWithValue("@Id", id);
				com.Parameters.AddWithValue("@Update", model.Update);
				com.Parameters.AddWithValue("@Cat", sh.SheetName);
				com.Parameters.AddWithValue("@Loop", z);
				com.Parameters.AddWithValue("@Nid", nid);
				nid = (int)com.ExecuteScalar();
				model.Update = false;
			    rowcount += tablelist[z].Rows.Count;
				con.Close();
			}
			return rowcount;


		}
		public JsonResult IncentiveUpload(SalaryInputModel model)
		{
			var rows = 0;
			try
			{
				string id = "0";
				
				if (model.Update)
					id = model.UpdateId.ToString();
				var file = Request.Files[0];
				if (ModelState.IsValid)
				{
					if (file == null)
					{
						ModelState.AddModelError("File", "Please Upload Your file");
					}
					else if (file.ContentLength > 0)
					{
						int MaxContentLength = 1024 * 1024 * 3; //3 MB
						string[] AllowedFileExtensions = new string[] { ".xlsx", ".xls" };
						var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
						if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
						{
							ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
						}

						else if (file.ContentLength > MaxContentLength)
						{
							ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
						}
						else
						{
							//TO:DO
							
							var fileName = Path.GetFileName("XLSFILE.XLS");
							var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
							ModelState.Clear();
							if (ext == ".xls")
							{
								file.SaveAs(path);
								var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
							    rows  = ReadIncentiveXLS(fs, model, id);
							}	    
							else
							{

								
								ExcelPackage ep = new ExcelPackage(file.InputStream);
								ExcelWorksheet workSheet = ep.Workbook.Worksheets.First();
								List<TwoDContainer> textarray = new List<TwoDContainer>();
								List<string> staff = new List<string>();
								
								var staffcol = new TwoDContainer();
								var tablelist = new List<DataTable>();
								List<string> excelHeaders = new List<string>() { "Emp ID", "Employee Name", "Designation", "Bank A/c No.", "Points", "Incentive amount" };
								for (var z = 0; z <= ep.Workbook.Worksheets.Count - 2; z++)
								{

									DataTable table = new DataTable();
									for (var i = workSheet.Dimension.Start.Row; i <= workSheet.Dimension.End.Row; i++)
									{
										var head = workSheet.Dimension.Start.Column;
										for (var j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
										{
											if (workSheet.Cells[i, j].Text == "")
											{
												head++;
											}

											if (i > 6 && j >= 2 && workSheet.Cells[i, j].Text != "#NAME?")
											{
												TwoDContainer d2array = new TwoDContainer();
												d2array.Row = i;

												d2array.Coloumn = j;
												d2array.Content = workSheet.Cells[i, j].Text.Replace(",", "");
												textarray.Add(d2array);
											}

										}
										if (head > 3)
										{
											textarray.RemoveAll(x => x.Row == i);
										}

									}
									
									foreach (var headers in excelHeaders)
									{
										table.Columns.Add(headers);
									}
									
									var srow = textarray.FirstOrDefault().Row;
									var icol = textarray.FirstOrDefault().Coloumn;

									var p = 0;
									while (p <= textarray.Count - 1)
									{

										var j = 2;
										var nrow = table.NewRow();
										var skip = true;
										while (textarray.FindIndex(x => x.Coloumn == j && x.Row == srow) != -1)
										{
											var idx = textarray.FindIndex(x => x.Row == srow && x.Coloumn == j);
											nrow.SetField<string>(j - 2, textarray[idx].Content);
											p++;
											j++;
											skip = false;
										}
										if (!skip)
										{

											table.Rows.Add(nrow);
										}
										else
										{
											p++;
										}
										srow++;
									}
									tablelist.Add(table);
								}
								var nid = 0;
								for (var z = 0; z <= ep.Workbook.Worksheets.Count - 2; z++)
								{
									con.Open();
									SqlCommand com = new SqlCommand("InsertIncentive", con);
									com.CommandType = CommandType.StoredProcedure;
									com.Parameters.AddWithValue("@incentiveTable", tablelist[z]);
									com.Parameters.AddWithValue("@LocId", model.LocationId);
									com.Parameters.AddWithValue("@Month", model.Month);
									com.Parameters.AddWithValue("@Year", model.Year);
									com.Parameters.AddWithValue("@Id", id);
									com.Parameters.AddWithValue("@Update", model.Update);
									com.Parameters.AddWithValue("@Cat", workSheet.Name);
									com.Parameters.AddWithValue("@Loop", z);
									com.Parameters.AddWithValue("@Nid", nid);
									nid = (int)com.ExecuteScalar();
									model.Update = false;
									rows += tablelist[z].Rows.Count;
									con.Close();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (con.State == ConnectionState.Open)
					con.Close();
				Logger.Write(ex.Message);
			}
			//return RedirectToAction("Index", "DashBoard"); ;
			return Json(new { rows });
		}
		#endregion
		#region DropDown
		/// <summary>
		/// Populating Location Dropdown
		/// </summary>
		/// <param name="sim">SalaryInputModel as Parameter</param>
		private void LocationDrp(SalaryInputModel sim)
		{
			con.Open();
			SqlCommand com = new SqlCommand("getBranchLocationDropDown", con);
			com.CommandType = CommandType.StoredProcedure;

			DataTable btable = new DataTable();
			var blist = ((List<int>)Session["BranchList"])[0];
			btable.Columns.Add("BranchID");
			btable.Rows.Add(13);
			com.Parameters.AddWithValue("@BranchList", btable);

			sim.LocationList = new List<SelectListItem>();
			//sim.LocationList.Add(new SelectListItem { Text = "All", Value = "0"});
			using (SqlDataReader sdr = com.ExecuteReader())				  
			{
				while (sdr.Read())
				{
					sim.LocationList.Add(new SelectListItem { Text = sdr["LocationName"].ToString(), Value = sdr["Id"].ToString() });
				}
			}
			con.Close();
		}
		#endregion
	}
}