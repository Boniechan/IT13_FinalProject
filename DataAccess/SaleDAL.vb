Imports MySql.Data.MySqlClient
Imports System.Windows.Forms

Public Class SaleDAL
    Public Function AddSale(sale As Sale) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "INSERT INTO sales (UserID, ProductID, Quantity, UnitPrice, TotalAmount, SaleDate, CustomerName) VALUES (@userID, @productID, @quantity, @unitPrice, @totalAmount, @saleDate, @customerName)"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userID", sale.UserID)
                    cmd.Parameters.AddWithValue("@productID", sale.ProductID)
                    cmd.Parameters.AddWithValue("@quantity", sale.Quantity)
                    cmd.Parameters.AddWithValue("@unitPrice", sale.UnitPrice)
                    cmd.Parameters.AddWithValue("@totalAmount", sale.TotalAmount)
                    cmd.Parameters.AddWithValue("@saleDate", sale.SaleDate)
                    cmd.Parameters.AddWithValue("@customerName", sale.CustomerName)

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error recording sale: " & ex.Message)
            Return False
        End Try
    End Function

    ' New method for sales with supplier information
    Public Function AddSaleWithSupplier(sale As Sale) As Boolean
        Using conn As MySqlConnection = DatabaseConnection.GetConnection()
            If conn Is Nothing Then Return False

            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    ' Insert sale with supplier information
                    Dim saleQuery As String = "INSERT INTO sales (UserID, ProductID, SupplierID, Quantity, UnitPrice, TotalAmount, SaleDate, CustomerName, Notes) VALUES (@userID, @productID, @supplierID, @quantity, @unitPrice, @totalAmount, @saleDate, @customerName, @notes)"
                    Using cmd As New MySqlCommand(saleQuery, conn, transaction)
                        cmd.Parameters.Add("@userID", MySqlDbType.Int32).Value = sale.UserID
                        cmd.Parameters.Add("@productID", MySqlDbType.Int32).Value = sale.ProductID
                        cmd.Parameters.Add("@supplierID", MySqlDbType.Int32).Value = sale.SupplierID
                        cmd.Parameters.Add("@quantity", MySqlDbType.Int32).Value = sale.Quantity
                        cmd.Parameters.Add("@unitPrice", MySqlDbType.Decimal).Value = sale.UnitPrice
                        cmd.Parameters.Add("@totalAmount", MySqlDbType.Decimal).Value = sale.TotalAmount
                        cmd.Parameters.Add("@saleDate", MySqlDbType.DateTime).Value = sale.SaleDate
                        cmd.Parameters.Add("@customerName", MySqlDbType.VarChar).Value = sale.CustomerName
                        cmd.Parameters.Add("@notes", MySqlDbType.Text).Value = If(String.IsNullOrEmpty(sale.Notes), DBNull.Value, sale.Notes)

                        If cmd.ExecuteNonQuery() <= 0 Then
                            transaction.Rollback()
                            Return False
                        End If
                    End Using

                    ' Update product stock
                    Dim stockQuery As String = "UPDATE products SET Stock = Stock - @quantity WHERE ProductID = @productID AND Stock >= @quantity"
                    Using cmd As New MySqlCommand(stockQuery, conn, transaction)
                        cmd.Parameters.Add("@quantity", MySqlDbType.Int32).Value = sale.Quantity
                        cmd.Parameters.Add("@productID", MySqlDbType.Int32).Value = sale.ProductID

                        If cmd.ExecuteNonQuery() <= 0 Then
                            transaction.Rollback()
                            MessageBox.Show("Insufficient stock for this sale.")
                            Return False
                        End If
                    End Using

                    transaction.Commit()
                    Return True

                Catch ex As Exception
                    transaction.Rollback()
                    MessageBox.Show($"Error recording sale: {ex.Message}")
                    Return False
                End Try
            End Using
        End Using
    End Function

    Public Function GetSalesHistory() As List(Of Sale)
        Dim sales As New List(Of Sale)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "SELECT s.*, p.ProductName, u.FullName FROM sales s INNER JOIN products p ON s.ProductID = p.ProductID INNER JOIN users u ON s.UserID = u.UserID ORDER BY s.SaleDate DESC"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim sale As New Sale With {
                                .SaleID = Convert.ToInt32(reader("SaleID")),
                                .UserID = Convert.ToInt32(reader("UserID")),
                                .ProductID = Convert.ToInt32(reader("ProductID")),
                                .Quantity = Convert.ToInt32(reader("Quantity")),
                                .UnitPrice = Convert.ToDecimal(reader("UnitPrice")),
                                .TotalAmount = Convert.ToDecimal(reader("TotalAmount")),
                                .SaleDate = Convert.ToDateTime(reader("SaleDate")),
                                .CustomerName = reader("CustomerName").ToString()
                            }
                            sales.Add(sale)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving sales history: " & ex.Message)
        End Try
        Return sales
    End Function

    Public Function UpdateProductStock(productID As Integer, quantitySold As Integer) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "UPDATE products SET Stock = Stock - @quantity WHERE ProductID = @id AND Stock >= @quantity"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", productID)
                    cmd.Parameters.AddWithValue("@quantity", quantitySold)
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating stock: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function GetOrderHistory() As List(Of SaleHistoryItem)
        Dim history As New List(Of SaleHistoryItem)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return history

                ' Updated query to include supplier information
                Dim query As String = "SELECT s.SaleID, s.UserID, u.FullName, u.Username, s.ProductID, p.ProductName, " &
                                    "s.Quantity, s.UnitPrice, s.TotalAmount, s.SaleDate, s.CustomerName, " &
                                    "s.SupplierID, sup.SupplierName, s.Notes " &
                                    "FROM sales s " &
                                    "INNER JOIN products p ON s.ProductID = p.ProductID " &
                                    "INNER JOIN users u ON s.UserID = u.UserID " &
                                    "LEFT JOIN suppliers sup ON s.SupplierID = sup.SupplierID " &
                                    "ORDER BY s.SaleDate DESC"

                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            history.Add(New SaleHistoryItem With {
                                .SaleID = Convert.ToInt32(reader("SaleID")),
                                .UserID = Convert.ToInt32(reader("UserID")),
                                .FullName = reader("FullName").ToString(),
                                .Username = reader("Username").ToString(),
                                .ProductID = Convert.ToInt32(reader("ProductID")),
                                .ProductName = reader("ProductName").ToString(),
                                .Quantity = Convert.ToInt32(reader("Quantity")),
                                .UnitPrice = Convert.ToDecimal(reader("UnitPrice")),
                                .TotalAmount = Convert.ToDecimal(reader("TotalAmount")),
                                .SaleDate = Convert.ToDateTime(reader("SaleDate")),
                                .CustomerName = reader("CustomerName").ToString(),
                                .SupplierName = If(IsDBNull(reader("SupplierName")), "N/A", reader("SupplierName").ToString()),
                                .Notes = If(IsDBNull(reader("Notes")), "", reader("Notes").ToString())
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading order history: " & ex.Message)
        End Try
        Return history
    End Function
End Class

Public Class SaleHistoryItem
    Public Property SaleID As Integer
    Public Property UserID As Integer
    Public Property FullName As String
    Public Property Username As String
    Public Property ProductID As Integer
    Public Property ProductName As String
    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal
    Public Property TotalAmount As Decimal
    Public Property SaleDate As DateTime
    Public Property CustomerName As String
    Public Property SupplierName As String
    Public Property Notes As String
End Class
