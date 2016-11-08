using System;
using System.Drawing;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Windows.Forms;

using BizRAD.BizXml;
using BizRAD.BizCommon;
using BizRAD.BizBase;
using BizRAD.BizVoucher;
using BizRAD.BizDocument;
using BizRAD.BizDetail;
using BizRAD.BizApplication;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizTools;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizControls.BizDateTimePicker;

using QueryADataSet;

namespace BizRAD.BizAccounts
{
	/// <summary>
	/// Class containing Functions used import/export of datatables to/from files
	/// </summary>
	public class BizIO
	{
		#region CreateFolder

		private static void CreateFolder(string destination)
		{
			if (!Directory.Exists(@destination))
			{
				Directory.CreateDirectory(@destination);
			}
		}

		#endregion

		#region ExportToCSV

		public static void ExportToCSV(DataTable dataTable, string filepath, string filename, bool withHeaders) 
		{
			if(filename == String.Empty || filename.IndexOf(".txt")<=0)
			{
				MessageBox.Show("Filename is empty or does not end with .txt","Bizpoint International",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
				return;
			}
			if (dataTable.Rows.Count==0)
			{
				return;
			}

			try
			{
				CreateFolder(filepath);
				// Create the CSV file to which grid data will be exported.
				StreamWriter sw = new StreamWriter(@filepath+@filename,false);
				int iColCount = dataTable.Columns.Count;;

				if(withHeaders)
				{
					// First we will write the headers.
					for(int i = 0; i < iColCount; i++)
					{
						sw.Write(dataTable.Columns[i]);
						if (i < iColCount - 1)
						{
							sw.Write(",");
						}
					}
				}
				sw.Write(sw.NewLine);
				
				// Now write all the rows.
				foreach (DataRow dr in dataTable.Rows)
				{
					for (int i = 0; i < iColCount; i++)
					{
						if (!Convert.IsDBNull(dr[i]))
						{
							sw.Write(dr[i].ToString());
						}
						if ( i < iColCount - 1)
						{
							sw.Write(",");
						}
					}
					sw.Write(sw.NewLine);
				}
				sw.Close();
			}
			catch(Exception eName)
			{
				MessageBox.Show(eName.Message+"\n"+eName.Source+"\n"+eName.StackTrace,"Bizpoint International",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
			}
		}

		#endregion

		#region ExportToExcel

		public static void ExportToExcel(DataTable dataTable, string filepath, string filename, bool withHeaders) 
		{
			DataTable source = dataTable;

			if(filename == String.Empty || filename.IndexOf(".xls")<=0)
			{
				MessageBox.Show("Filename is empty or does not end with .xls","Bizpoint International", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			CreateFolder(filepath);
			StreamWriter excelDoc = new StreamWriter(@filepath+@filename);
			const string startExcelXML = "<xml version>\r\n<Workbook " + 
					  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" + 
					  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " + 
					  "xmlns:x=\"urn:schemas-    microsoft-com:office:" + 
					  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" + 
					  "office:spreadsheet\">\r\n <Styles>\r\n " + 
					  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " + 
					  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" + 
					  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" + 
					  "\r\n <Protection/>\r\n </Style>\r\n " + 
					  "<Style ss:ID=\"BoldColumn\">\r\n <Font " + 
					  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " + 
					  "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" + 
					  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " + 
					  "ss:ID=\"Decimal\">\r\n <NumberFormat " + 
					  "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " + 
					  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " + 
					  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " + 
					  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " + 
					  "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " + 
					  "</Styles>\r\n ";
			const string endExcelXML = "</Workbook>";

			int rowCount = 0;
			int sheetCount = 1;
			/*
			   <xml version>
			   <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
			   xmlns:o="urn:schemas-microsoft-com:office:office"
			   xmlns:x="urn:schemas-microsoft-com:office:excel"
			   xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
			   <Styles>
			   <Style ss:ID="Default" ss:Name="Normal">
				 <Alignment ss:Vertical="Bottom"/>
				 <Borders/>
				 <Font/>
				 <Interior/>
				 <NumberFormat/>
				 <Protection/>
			   </Style>
			   <Style ss:ID="BoldColumn">
				 <Font x:Family="Swiss" ss:Bold="1"/>
			   </Style>
			   <Style ss:ID="StringLiteral">
				 <NumberFormat ss:Format="@"/>
			   </Style>
			   <Style ss:ID="Decimal">
				 <NumberFormat ss:Format="0.0000"/>
			   </Style>
			   <Style ss:ID="Integer">
				 <NumberFormat ss:Format="0"/>
			   </Style>
			   <Style ss:ID="DateLiteral">
				 <NumberFormat ss:Format="mm/dd/yyyy;@"/>
			   </Style>
			   </Styles>
			   <Worksheet ss:Name="Sheet1">
			   </Worksheet>
			   </Workbook>
			   */
			excelDoc.Write(startExcelXML);
			excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
			excelDoc.Write("<Table>");

			if(withHeaders)
			{
				excelDoc.Write("<Row>");
				for(int x = 0; x < source.Columns.Count; x++)
				{
					excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
					excelDoc.Write(source.Columns[x].ColumnName);
					excelDoc.Write("</Data></Cell>");
				}
				excelDoc.Write("</Row>");
			}

			foreach(DataRow x in source.Rows)
			{
				rowCount++;
				//if the number of rows is > 64000 create a new page to continue output
				if(rowCount==64000) 
				{
					rowCount = 0;
					sheetCount++;
					excelDoc.Write("</Table>");
					excelDoc.Write(" </Worksheet>");
					excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
					excelDoc.Write("<Table>");
				}
				excelDoc.Write("<Row>"); //ID=" + rowCount + "
				for(int y = 0; y < source.Columns.Count; y++)
				{
					System.Type rowType;
					rowType = x[y].GetType();
					switch(rowType.ToString())
					{
						case "System.String":
							string XMLstring = x[y].ToString();
							XMLstring = XMLstring.Trim();
							XMLstring = XMLstring.Replace("&","&amp;");
							XMLstring = XMLstring.Replace(">",">");
							XMLstring = XMLstring.Replace("<","<");
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" + 
								"<Data ss:Type=\"String\">");
							excelDoc.Write(XMLstring);
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.DateTime":
							//Excel has a specific Date Format of YYYY-MM-DD followed by  
							//the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
							//The Following Code puts the date stored in XMLDate 
							//to the format above
							DateTime XMLDate = (DateTime)x[y];
							string XMLDatetoString = ""; //Excel Converted Date
							XMLDatetoString = XMLDate.Year.ToString() +
								"-" + 
								(XMLDate.Month < 10 ? "0" + 
								XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
								"-" +
								(XMLDate.Day < 10 ? "0" + 
								XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
								"T" +
								(XMLDate.Hour < 10 ? "0" + 
								XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
								":" +
								(XMLDate.Minute < 10 ? "0" + 
								XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
								":" +
								(XMLDate.Second < 10 ? "0" + 
								XMLDate.Second.ToString() : XMLDate.Second.ToString()) + 
								".000";
							excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" + 
								"<Data ss:Type=\"DateTime\">");
							excelDoc.Write(XMLDatetoString);
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.Boolean":
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" + 
								"<Data ss:Type=\"String\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.Int16":
						case "System.Int32":
						case "System.Int64":
						case "System.Byte":
							excelDoc.Write("<Cell ss:StyleID=\"Integer\">" + 
								"<Data ss:Type=\"Number\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.Decimal":
						case "System.Double":
							excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" + 
								"<Data ss:Type=\"Number\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.DBNull":
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" + 
								"<Data ss:Type=\"String\">");
							excelDoc.Write("");
							excelDoc.Write("</Data></Cell>");
							break;
						default:
							throw(new Exception(rowType.ToString() + " not handled."));
					}
				}
				excelDoc.Write("</Row>");
			}
			excelDoc.Write("</Table>");
			excelDoc.Write(" </Worksheet>");
			excelDoc.Write(endExcelXML);
			excelDoc.Close();
		}

		#endregion

		#region ImportFromCSV

		public static DataTable ImportFromCSV(string FileName, string delimiters, bool firstLineIsColumnNames)
		{		
			if(!FileName.EndsWith(".txt"))
				FileName = FileName + ".txt";

			if(File.Exists(FileName))
			{
				StreamReader sr = new StreamReader(FileName);
				string line = "";
				DataTable dt = new DataTable();
				int linecounter = 1;
				
				try
				{	
					// Read and display lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null) 
					{
						if(delimiters == String.Empty)
							delimiters = ",";
						char [] delimiter = delimiters.ToCharArray();
						string [] split = line.Split(delimiter);
						
						if(linecounter==1 && firstLineIsColumnNames)
						{
							foreach(string s in split)
							{
								dt.Columns.Add(s);
							}
						}
						else
						{
							int columnposition = 0;
							if(linecounter==1 && !firstLineIsColumnNames)
							{
								int i = 0;
								foreach(string s in split)
								{
									dt.Columns.Add("column"+i.ToString());
									i++;
								}
							}
							foreach(string s in split)
							{
								DataRow newRow = dt.NewRow();								
								newRow[columnposition] = s;
								dt.Rows.Add(newRow);
								columnposition++;
							}
						}
						linecounter++;
					}				
				} 
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message);
					return null;
				}
				finally
				{
					sr.Close();
				}
				return dt;
			}
			else
			{
				MessageBox.Show("Invalid file : "+FileName,"Bizpoint International",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
				return null;
			}
		}
		#endregion
	}
}
