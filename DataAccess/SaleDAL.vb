Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

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
End Class