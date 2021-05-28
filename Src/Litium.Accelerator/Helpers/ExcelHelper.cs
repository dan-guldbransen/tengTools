using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Helpers
{
    /// <summary>
    /// Contains helper methods for working with Excel files, requires the "EPPlus" nuget package. 
    /// </summary>
    public static class ExcelHelpers
    {
        #region Get Methods
        /// <summary>
        /// Get a data table from an Excel file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        /// <param name="firstRowIsHeaders">If the first row are headers</param>
        /// <param name="worksheetName">The worksheet name, if left empty the first worksheet is used</param>
        /// <returns>Data table</returns>
        public static DataTable GetDataTable(string filePath, bool firstRowIsHeaders, string worksheetName = null)
        {
            return GetDataTable(File.OpenRead(filePath), firstRowIsHeaders, worksheetName);
        }


        /// <summary>
        /// Get a data table from an Excel file stream
        /// </summary>
        /// <param name="fileStream">Excel file stream</param>
        /// <param name="firstRowIsHeaders">If the first row are headers</param>
        /// <param name="worksheetName">The worksheet name, if left empty the first worksheet is used</param>
        /// <returns>Data table</returns>
        public static DataTable GetDataTable(Stream fileStream, bool firstRowIsHeaders, string worksheetName = null)
        {
            if (fileStream == null || !fileStream.CanRead)
                return null;


            DataTable tbl;

            using (var pck = new ExcelPackage())
            {
                pck.Load(fileStream);


                try
                {
                    fileStream.Close();
                    fileStream.Dispose();

                    ExcelWorksheet ws;

                    if (string.IsNullOrEmpty(worksheetName))
                        ws = pck.Workbook.Worksheets.First();
                    else
                        ws = pck.Workbook.Worksheets[worksheetName];


                    tbl = new DataTable(ws.Name);


                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                        tbl.Columns.Add(firstRowIsHeaders ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));


                    var startRow = firstRowIsHeaders ? 2 : 1;

                    for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        var row = tbl.Rows.Add();

                        foreach (var cell in wsRow)
                        {
                            try
                            {
                                row[cell.Start.Column - 1] = cell.Text;
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.StartsWith("Cannot find column")) // When a row doesn't contain a column // TODO: Check why this is possible?
                                    throw ex;
                            }
                        }
                    }


                    return tbl;
                }
                catch { } // Fail could occur if there is a sheet marked as firstRowIsHeaders = true, but that dont have any column headers. If that's the case we skip the sheet.
            }


            return null;
        }


        /// <summary>
        /// Get all data tables from an Excel file stream
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        /// <param name="firstRowIsHeaders">If the first row are headers</param>
        /// <returns>Dictionary of data tables with worksheet names</returns>
        public static Dictionary<string, DataTable> GetAllDataTables(string filePath, bool firstRowIsHeaders)
        {
            return GetAllDataTables(File.OpenRead(filePath), firstRowIsHeaders);
        }


        /// <summary>
        /// Get all data tables from an Excel file stream
        /// </summary>
        /// <param name="fileStream">Excel file stream</param>
        /// <param name="firstRowIsHeaders">If the first row are headers</param>
        /// <returns>Dictionary of data tables with worksheet names</returns>
        public static Dictionary<string, DataTable> GetAllDataTables(Stream fileStream, bool firstRowIsHeaders)
        {
            if (fileStream == null || !fileStream.CanRead)
                return null;


            var dictionary = new Dictionary<string, DataTable>();


            using (var pck = new ExcelPackage())
            {
                pck.Load(fileStream);


                try
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
                catch { }


                foreach (var ws in pck.Workbook.Worksheets)
                {
                    try
                    {
                        var tbl = new DataTable(ws.Name);


                        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                            tbl.Columns.Add(firstRowIsHeaders ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));


                        var startRow = firstRowIsHeaders ? 2 : 1;

                        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                            var row = tbl.Rows.Add();

                            foreach (var cell in wsRow)
                            {
                                try
                                {
                                    row[cell.Start.Column - 1] = cell.Text;
                                }
                                catch (Exception ex)
                                {
                                    if (!ex.Message.StartsWith("Cannot find column")) // When a row doesn't contain a column // TODO: Check why this is possible?
                                        throw ex;
                                }
                            }
                        }


                        dictionary.Add(ws.Name, tbl);
                    }
                    catch (Exception ex) { } // Fail could occur if there is a sheet marked as firstRowIsHeaders = true, but that dont have any column headers. If that's the case we skip the sheet.
                }
            }


            return dictionary;
        }


        #endregion

        #region Save Methods


        /// <summary>
        /// Save a data table to an Excel file
        /// </summary>
        /// <param name="dataTable">Data table to save</param>
        /// <param name="filePath">Excel file path</param>
        /// <param name="autoFitColumnWidth">If we should automatically fit the column width</param>
        /// <param name="removeExistingFile">If we should remove the existing file</param>
        /// <returns>Success or failure</returns>
        public static bool SaveDataTable(DataTable dataTable, string filePath, bool autoFitColumnWidth = true, bool removeExistingFile = true)
        {
            return SaveDataTable(dataTable, filePath, Point.Empty, autoFitColumnWidth, removeExistingFile);
        }


        /// <summary>
        /// Save a data table to an Excel file
        /// </summary>
        /// <param name="dataTable">Data table to save</param>
        /// <param name="filePath">Excel file path</param>
        /// <param name="freezePanes">Coordinates (column, row) to freeze panes at</param>
        /// <param name="autoFitColumnWidth">If we should automatically fit the column width</param>
        /// <param name="removeExistingFile">If we should remove the existing file</param>
        /// <returns>Success or failure</returns>
        public static bool SaveDataTable(DataTable dataTable, string filePath, Point freezePanes, bool autoFitColumnWidth = true, bool removeExistingFile = true)
        {
            if (removeExistingFile && File.Exists(filePath))
                File.Delete(filePath);


            using (var pck = new ExcelPackage(new FileInfo(filePath)))
            {
                var ws = pck.Workbook.Worksheets[dataTable.TableName];

                if (ws == null)
                    ws = pck.Workbook.Worksheets.Add(dataTable.TableName);


                ws.Cells["A1"].LoadFromDataTable(dataTable, true);


                if (!freezePanes.IsEmpty)
                    ws.View.FreezePanes(freezePanes.X, freezePanes.Y);


                if (autoFitColumnWidth)
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();


                pck.Save();
            }


            return true;
        }


        /// <summary>
        /// Save a dictionary of data tables with worksheet names to an Excel file
        /// </summary>
        /// <param name="dictionary">Dictionary to save</param>
        /// <param name="filePath">Excel file path</param>
        /// <param name="autoFitColumnWidth">If we should automatically fit the column width</param>
        /// <param name="removeExistingFile">If we should remove the existing file</param>
        /// <returns>Success or failure</returns>
        public static bool SaveAllDataTables(Dictionary<string, DataTable> dictionary, string filePath, bool autoFitColumnWidth = true, bool removeExistingFile = true)
        {
            if (removeExistingFile && File.Exists(filePath))
                File.Delete(filePath);


            using (var pck = new ExcelPackage(new FileInfo(filePath)))
            {
                foreach (var keyValueItem in dictionary)
                {
                    var ws = pck.Workbook.Worksheets[keyValueItem.Key];

                    if (ws == null)
                        ws = pck.Workbook.Worksheets.Add(keyValueItem.Key);


                    ws.Cells["A1"].LoadFromDataTable(keyValueItem.Value, true);


                    if (autoFitColumnWidth && ws.Dimension?.Address != null)
                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                }

                pck.Save();
            }


            return true;
        }


        #endregion


    }
}
