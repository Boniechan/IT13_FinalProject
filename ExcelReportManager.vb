Imports System.IO
Imports System.Windows.Forms
Imports System.Text

Public Class ExcelReportManager
    Private ReadOnly saleDal As New SaleDAL()
    Private ReadOnly productDal As New ProductDAL()

    Public Sub New()
        ' Constructor
    End Sub

    ' Generate Daily Sales Report
    Public Function GenerateDailySalesReport(reportDate As DateTime, Optional savePath As String = Nothing) As Boolean
        Try
            Dim sales = GetDailySales(reportDate)
            If sales.Count = 0 Then
                MessageBox.Show($"No sales data found for {reportDate:MM/dd/yyyy}", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If

            Return CreateCsvReport(sales, $"Daily Sales Report - {reportDate:MM-dd-yyyy}", savePath)
        Catch ex As Exception
            MessageBox.Show($"Error generating daily report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Generate Weekly Sales Report
    Public Function GenerateWeeklySalesReport(startDate As DateTime, Optional savePath As String = Nothing) As Boolean
        Try
            Dim endDate = startDate.AddDays(6)
            Dim sales = GetWeeklySales(startDate, endDate)
            If sales.Count = 0 Then
                MessageBox.Show($"No sales data found for week {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If

            Return CreateCsvReport(sales, $"Weekly Sales Report - {startDate:MM-dd-yyyy} to {endDate:MM-dd-yyyy}", savePath)
        Catch ex As Exception
            MessageBox.Show($"Error generating weekly report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Generate Monthly Sales Report
    Public Function GenerateMonthlySalesReport(year As Integer, month As Integer, Optional savePath As String = Nothing) As Boolean
        Try
            Dim startDate = New DateTime(year, month, 1)
            Dim endDate = startDate.AddMonths(1).AddDays(-1)
            Dim sales = GetSalesForDateRange(startDate, endDate)

            If sales.Count = 0 Then
                MessageBox.Show($"No sales data found for {startDate:MMMM yyyy}", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If

            Return CreateCsvReport(sales, $"Monthly Sales Report - {startDate:MMMM yyyy}", savePath)
        Catch ex As Exception
            MessageBox.Show($"Error generating monthly report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Generate Product Inventory Report
    Public Function GenerateInventoryReport(Optional savePath As String = Nothing) As Boolean
        Try
            Dim products = productDal.GetAllProducts()
            If products.Count = 0 Then
                MessageBox.Show("No products found", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If

            Return CreateInventoryCsvReport(products, $"Inventory Report - {DateTime.Now:MM-dd-yyyy}", savePath)
        Catch ex As Exception
            MessageBox.Show($"Error generating inventory report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Create CSV Report for Sales (can be opened in Excel)
    Private Function CreateCsvReport(sales As List(Of SaleHistoryItem), reportTitle As String, Optional savePath As String = Nothing) As Boolean
        Try
            Dim csv As New StringBuilder()

            ' Add title and generated date
            csv.AppendLine(reportTitle)
            csv.AppendLine($"Generated on: {DateTime.Now:MM/dd/yyyy hh:mm tt}")
            csv.AppendLine() ' Empty line

            ' Add headers
            csv.AppendLine("Sale ID,Date,Customer,Product,Quantity,Unit Price,Total Amount,Staff,Notes")

            ' Add data rows
            For Each sale In sales
                csv.AppendLine($"{sale.SaleID},""{sale.SaleDate:MM/dd/yyyy hh:mm tt}"",""{EscapeCsvValue(sale.CustomerName)}"",""{EscapeCsvValue(sale.ProductName)}"",{sale.Quantity},{sale.UnitPrice},{sale.TotalAmount},""{EscapeCsvValue(sale.FullName)}"",""{EscapeCsvValue(sale.Notes)}""")
            Next

            ' Add summary
            csv.AppendLine() ' Empty line
            csv.AppendLine("SUMMARY")
            csv.AppendLine($"Total Sales:,{sales.Count}")
            csv.AppendLine($"Total Revenue:,{sales.Sum(Function(s) s.TotalAmount):F2}")

            ' Save file
            Dim fileName As String
            If String.IsNullOrEmpty(savePath) Then
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                      $"{reportTitle.Replace(" ", "_").Replace("-", "_")}.csv")
            Else
                fileName = savePath.Replace(".xlsx", ".csv")
            End If

            File.WriteAllText(fileName, csv.ToString(), Encoding.UTF8)
            MessageBox.Show($"Report saved successfully to: {fileName}{Environment.NewLine}This file can be opened in Excel.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            MessageBox.Show($"Error creating CSV report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Create CSV Report for Inventory
    Private Function CreateInventoryCsvReport(products As List(Of Product), reportTitle As String, Optional savePath As String = Nothing) As Boolean
        Try
            Dim csv As New StringBuilder()

            ' Add title and generated date
            csv.AppendLine(reportTitle)
            csv.AppendLine($"Generated on: {DateTime.Now:MM/dd/yyyy hh:mm tt}")
            csv.AppendLine() ' Empty line

            ' Add headers
            csv.AppendLine("Product ID,Product Name,Category,Price,Stock,Pricing Unit,Status,Description")

            ' Add data rows
            For Each product In products
                csv.AppendLine($"{product.ProductID},""{EscapeCsvValue(product.ProductName)}"",""{EscapeCsvValue(product.Category)}"",{product.Price},""{EscapeCsvValue(product.StockDisplay)}"",""{EscapeCsvValue(product.PricingUnit)}"",""{GetStockStatus(product)}"",""{EscapeCsvValue(product.Description)}""")
            Next

            ' Add summary
            csv.AppendLine() ' Empty line
            csv.AppendLine("INVENTORY SUMMARY")
            Dim lowStockProducts = products.Where(Function(p) GetStockStatus(p) = "Low Stock").Count()
            Dim outOfStockProducts = products.Where(Function(p) GetStockStatus(p) = "Out of Stock").Count()

            csv.AppendLine($"Total Products:,{products.Count}")
            csv.AppendLine($"Low Stock Items:,{lowStockProducts}")
            csv.AppendLine($"Out of Stock Items:,{outOfStockProducts}")

            ' Save file
            Dim fileName As String
            If String.IsNullOrEmpty(savePath) Then
                fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                      $"{reportTitle.Replace(" ", "_").Replace("-", "_")}.csv")
            Else
                fileName = savePath.Replace(".xlsx", ".csv")
            End If

            File.WriteAllText(fileName, csv.ToString(), Encoding.UTF8)
            MessageBox.Show($"Inventory report saved successfully to: {fileName}{Environment.NewLine}This file can be opened in Excel.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            MessageBox.Show($"Error creating inventory CSV report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ' Helper method to escape CSV values
    Private Function EscapeCsvValue(value As String) As String
        If String.IsNullOrEmpty(value) Then Return ""
        ' Replace quotes with double quotes and handle commas
        Return value.Replace("""", """""")
    End Function

    ' Helper method to get stock status
    Private Function GetStockStatus(product As Product) As String
        If product.Stock <= 0 Then
            Return "Out of Stock"
        ElseIf product.Stock <= 5 Then
            Return "Low Stock"
        Else
            Return "In Stock"
        End If
    End Function

    ' Get daily sales data
    Private Function GetDailySales(reportDate As DateTime) As List(Of SaleHistoryItem)
        Try
            Dim allSales = saleDal.GetOrderHistory()
            Return allSales.Where(Function(s) s.SaleDate.Date = reportDate.Date).ToList()
        Catch ex As Exception
            MessageBox.Show($"Error retrieving daily sales: {ex.Message}")
            Return New List(Of SaleHistoryItem)()
        End Try
    End Function

    ' Get weekly sales data
    Private Function GetWeeklySales(startDate As DateTime, endDate As DateTime) As List(Of SaleHistoryItem)
        Try
            Dim allSales = saleDal.GetOrderHistory()
            Return allSales.Where(Function(s) s.SaleDate.Date >= startDate.Date AndAlso s.SaleDate.Date <= endDate.Date).ToList()
        Catch ex As Exception
            MessageBox.Show($"Error retrieving weekly sales: {ex.Message}")
            Return New List(Of SaleHistoryItem)()
        End Try
    End Function

    ' Get sales for date range
    Private Function GetSalesForDateRange(startDate As DateTime, endDate As DateTime) As List(Of SaleHistoryItem)
        Try
            Dim allSales = saleDal.GetOrderHistory()
            Return allSales.Where(Function(s) s.SaleDate.Date >= startDate.Date AndAlso s.SaleDate.Date <= endDate.Date).ToList()
        Catch ex As Exception
            MessageBox.Show($"Error retrieving sales for date range: {ex.Message}")
            Return New List(Of SaleHistoryItem)()
        End Try
    End Function

    ' Schedule automatic daily report generation
    Public Sub ScheduleDailyReport()
        Try
            Dim today = DateTime.Today
            Dim yesterday = today.AddDays(-1)

            Dim folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Fisheries_Reports", "Daily")
            Directory.CreateDirectory(folderPath)

            Dim fileName = Path.Combine(folderPath, $"Daily_Sales_Report_{yesterday:yyyy_MM_dd}.csv")
            GenerateDailySalesReport(yesterday, fileName)
        Catch ex As Exception
            Console.WriteLine($"Error in ScheduleDailyReport: {ex.Message}")
        End Try
    End Sub

    ' Schedule automatic weekly report generation
    Public Sub ScheduleWeeklyReport()
        Try
            ' Generate report for last week (Monday to Sunday)
            Dim today = DateTime.Today
            Dim daysToSubtract = (CInt(today.DayOfWeek) + 6) Mod 7 ' Get Monday of current week
            Dim currentWeekMonday = today.AddDays(-daysToSubtract)
            Dim lastWeekMonday = currentWeekMonday.AddDays(-7)

            Dim folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Fisheries_Reports", "Weekly")
            Directory.CreateDirectory(folderPath)

            Dim fileName = Path.Combine(folderPath, $"Weekly_Sales_Report_{lastWeekMonday:yyyy_MM_dd}.csv")
            GenerateWeeklySalesReport(lastWeekMonday, fileName)
        Catch ex As Exception
            Console.WriteLine($"Error in ScheduleWeeklyReport: {ex.Message}")
        End Try
    End Sub
End Class