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

    Public Function AddSaleWithSupplier(sale As Sale) As Boolean
        Using conn As MySqlConnection = DatabaseConnection.GetConnection()
            If conn Is Nothing Then Return False

            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    ' Record the sale first
                    Dim saleQuery As String = "INSERT INTO sales (UserID, ProductID, SupplierID, Quantity, UnitPrice, TotalAmount, SaleDate, CustomerName, Notes) VALUES (@userID, @productID, @supplierID, @quantity, @unitPrice, @totalAmount, @saleDate, @customerName, @notes)"
                    Using cmd As New MySqlCommand(saleQuery, conn, transaction)
                        cmd.Parameters.Add("@userID", MySqlDbType.Int32).Value = sale.UserID
                        cmd.Parameters.Add("@productID", MySqlDbType.Int32).Value = sale.ProductID
                        cmd.Parameters.Add("@supplierID", MySqlDbType.Int32).Value = sale.SupplierID
                        cmd.Parameters.Add("@quantity", MySqlDbType.Decimal).Value = sale.Quantity
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

                    ' Update product stock using the simplified method
                    If Not UpdateStockWithTransactionSimplified(sale.ProductID, sale.Quantity, conn, transaction) Then
                        transaction.Rollback()
                        Return False
                    End If

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

    Private Function UpdateStockWithTransaction(productID As Integer, quantitySold As Decimal, conn As MySqlConnection, transaction As MySqlTransaction) As Boolean
        Try
            ' Get current product info within the same transaction
            Dim product As Product = GetProductByIdWithTransaction(productID, conn, transaction)
            If product Is Nothing Then
                MessageBox.Show("Product not found.")
                Return False
            End If

            ' Check if this is a boxed product
            If product.PricingUnit = "Box" Then
                ' Handle boxed products with 2-kilo logic
                Dim currentBoxQuantity As Decimal = If(product.BoxQuantity.HasValue AndAlso product.BoxQuantity.Value > 0, product.BoxQuantity.Value, product.BoxSize)

                ' Check if we have enough stock
                Dim totalAvailable As Decimal = currentBoxQuantity + ((product.Stock - 1) * product.BoxSize)
                If quantitySold > totalAvailable Then
                    MessageBox.Show($"Insufficient stock. Available: {totalAvailable:F2}kg, Requested: {quantitySold:F2}kg")
                    Return False
                End If

                If quantitySold <= currentBoxQuantity Then
                    ' Can fulfill from current box
                    Dim newBoxQuantity As Decimal = currentBoxQuantity - quantitySold

                    If newBoxQuantity <= 0 Then
                        ' Current box is empty, move to next box or set to 0 if no more boxes
                        If product.Stock > 1 Then
                            Dim query As String = "UPDATE products SET Stock = Stock - 1, BoxQuantity = @boxSize WHERE ProductID = @id"
                            Using cmd As New MySqlCommand(query, conn, transaction)
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                cmd.Parameters.Add("@boxSize", MySqlDbType.Decimal).Value = product.BoxSize
                                Return cmd.ExecuteNonQuery() > 0
                            End Using
                        Else
                            ' No more boxes
                            Dim query As String = "UPDATE products SET Stock = 0, BoxQuantity = 0 WHERE ProductID = @id"
                            Using cmd As New MySqlCommand(query, conn, transaction)
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                Return cmd.ExecuteNonQuery() > 0
                            End Using
                        End If
                    Else
                        ' Update remaining quantity in current box
                        Dim query As String = "UPDATE products SET BoxQuantity = @boxQuantity WHERE ProductID = @id"
                        Using cmd As New MySqlCommand(query, conn, transaction)
                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                            cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = newBoxQuantity
                            Return cmd.ExecuteNonQuery() > 0
                        End Using
                    End If
                Else
                    ' Need multiple boxes
                    Dim remainingNeeded As Decimal = quantitySold - currentBoxQuantity
                    Dim completeBoxesNeeded As Integer = CInt(Math.Floor(remainingNeeded / product.BoxSize))
                    Dim finalBoxAmount As Decimal = remainingNeeded Mod product.BoxSize

                    Dim newStock As Integer = product.Stock - 1 - completeBoxesNeeded
                    Dim newBoxQuantity As Decimal = If(finalBoxAmount > 0, product.BoxSize - finalBoxAmount, product.BoxSize)

                    If newStock < 0 Then
                        newStock = 0
                        newBoxQuantity = 0
                    End If

                    Dim query As String = "UPDATE products SET Stock = @stock, BoxQuantity = @boxQuantity WHERE ProductID = @id"
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                        cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = newStock
                        cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = newBoxQuantity
                        Return cmd.ExecuteNonQuery() > 0
                    End Using
                End If
            Else
                ' Handle regular products (kilo/piece)
                Dim query As String = "UPDATE products SET Stock = Stock - @quantity WHERE ProductID = @id AND Stock >= @quantity"
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                    cmd.Parameters.Add("@quantity", MySqlDbType.Decimal).Value = quantitySold
                    Dim rowsAffected = cmd.ExecuteNonQuery()
                    If rowsAffected <= 0 Then
                        MessageBox.Show("Insufficient stock for this sale.")
                        Return False
                    End If
                    Return True
                End Using
            End If

        Catch ex As MySqlException
            MessageBox.Show($"Database error updating stock: {ex.Message} (Error: {ex.Number})")
            Return False
        Catch ex As Exception
            MessageBox.Show($"Error updating stock: {ex.Message}")
            Return False
        End Try
    End Function

    Private Function UpdateStockWithTransactionSimplified(productID As Integer, quantitySold As Decimal, conn As MySqlConnection, transaction As MySqlTransaction) As Boolean
        Try
            ' Get current product info within the same transaction
            Dim product As Product = GetProductByIdWithTransaction(productID, conn, transaction)
            If product Is Nothing Then
                MessageBox.Show("Product not found.")
                Return False
            End If

            ' Check if this is a boxed product
            If product.PricingUnit = "Box" Then
                ' Handle boxed products with 2-kilo logic
                Dim currentBoxQuantity As Decimal = If(product.BoxQuantity.HasValue AndAlso product.BoxQuantity.Value > 0, product.BoxQuantity.Value, product.BoxSize)

                ' Check if we have enough stock
                Dim totalAvailable As Decimal = currentBoxQuantity + ((product.Stock - 1) * product.BoxSize)
                If quantitySold > totalAvailable Then
                    MessageBox.Show($"Insufficient stock. Available: {totalAvailable:F2}kg, Requested: {quantitySold:F2}kg")
                    Return False
                End If

                If quantitySold <= currentBoxQuantity Then
                    ' Can fulfill from current box
                    Dim newBoxQuantity As Decimal = currentBoxQuantity - quantitySold

                    If newBoxQuantity <= 0 Then
                        ' Current box is empty, move to next box or set to 0 if no more boxes
                        If product.Stock > 1 Then
                            Dim query As String = "UPDATE products SET Stock = Stock - 1, BoxQuantity = @boxSize WHERE ProductID = @id"
                            Using cmd As New MySqlCommand(query, conn, transaction)
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                cmd.Parameters.Add("@boxSize", MySqlDbType.Decimal).Value = product.BoxSize
                                Return cmd.ExecuteNonQuery() > 0
                            End Using
                        Else
                            ' No more boxes
                            Dim query As String = "UPDATE products SET Stock = 0, BoxQuantity = 0 WHERE ProductID = @id"
                            Using cmd As New MySqlCommand(query, conn, transaction)
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                Return cmd.ExecuteNonQuery() > 0
                            End Using
                        End If
                    Else
                        ' Update remaining quantity in current box
                        Dim query As String = "UPDATE products SET BoxQuantity = @boxQuantity WHERE ProductID = @id"
                        Using cmd As New MySqlCommand(query, conn, transaction)
                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                            cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = newBoxQuantity
                            Return cmd.ExecuteNonQuery() > 0
                        End Using
                    End If
                Else
                    ' Need multiple boxes
                    Dim remainingNeeded As Decimal = quantitySold - currentBoxQuantity
                    Dim completeBoxesNeeded As Integer = CInt(Math.Floor(remainingNeeded / product.BoxSize))
                    Dim finalBoxAmount As Decimal = remainingNeeded Mod product.BoxSize

                    Dim newStock As Integer = product.Stock - 1 - completeBoxesNeeded
                    Dim newBoxQuantity As Decimal = If(finalBoxAmount > 0, product.BoxSize - finalBoxAmount, product.BoxSize)

                    If newStock < 0 Then
                        newStock = 0
                        newBoxQuantity = 0
                    End If

                    Dim query As String = "UPDATE products SET Stock = @stock, BoxQuantity = @boxQuantity WHERE ProductID = @id"
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                        cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = newStock
                        cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = newBoxQuantity
                        Return cmd.ExecuteNonQuery() > 0
                    End Using
                End If
            Else
                ' Handle regular products (kilo/piece)
                Dim query As String = "UPDATE products SET Stock = Stock - @quantity WHERE ProductID = @id AND Stock >= @quantity"
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                    cmd.Parameters.Add("@quantity", MySqlDbType.Decimal).Value = quantitySold
                    Dim rowsAffected = cmd.ExecuteNonQuery()
                    If rowsAffected <= 0 Then
                        MessageBox.Show("Insufficient stock for this sale.")
                        Return False
                    End If
                    Return True
                End Using
            End If

        Catch ex As MySqlException
            MessageBox.Show($"Database error updating stock: {ex.Message} (Error: {ex.Number})")
            Return False
        Catch ex As Exception
            MessageBox.Show($"Error updating stock: {ex.Message}")
            Return False
        End Try
    End Function

    Private Function GetProductByIdWithTransaction(productID As Integer, conn As MySqlConnection, transaction As MySqlTransaction) As Product
        Try
            ' Try with all new columns first
            Try
                Dim query As String = "SELECT ProductID, ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate, PricingUnit, BoxQuantity, BoxSize FROM products WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New Product With {
                                .ProductID = Convert.ToInt32(reader("ProductID")),
                                .ProductName = Convert.ToString(reader("ProductName")),
                                .Category = Convert.ToString(reader("Category")),
                                .Price = Convert.ToDecimal(reader("Price")),
                                .Stock = Convert.ToInt32(reader("Stock")),
                                .Description = Convert.ToString(reader("Description")),
                                .SupplierID = Convert.ToInt32(reader("SupplierID")),
                                .CreatedDate = Convert.ToDateTime(reader("CreatedDate")),
                                .PricingUnit = If(IsDBNull(reader("PricingUnit")), "Kilo", Convert.ToString(reader("PricingUnit"))),
                                .BoxQuantity = If(IsDBNull(reader("BoxQuantity")), Nothing, Convert.ToDecimal(reader("BoxQuantity"))),
                                .BoxSize = If(IsDBNull(reader("BoxSize")), 2D, Convert.ToDecimal(reader("BoxSize")))
                            }
                        End If
                    End Using
                End Using
            Catch ex As MySqlException When ex.Number = 1054 ' Column doesn't exist
                ' Fallback for missing BoxSize column
                Dim query As String = "SELECT ProductID, ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate, PricingUnit, BoxQuantity FROM products WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New Product With {
                                .ProductID = Convert.ToInt32(reader("ProductID")),
                                .ProductName = Convert.ToString(reader("ProductName")),
                                .Category = Convert.ToString(reader("Category")),
                                .Price = Convert.ToDecimal(reader("Price")),
                                .Stock = Convert.ToInt32(reader("Stock")),
                                .Description = Convert.ToString(reader("Description")),
                                .SupplierID = Convert.ToInt32(reader("SupplierID")),
                                .CreatedDate = Convert.ToDateTime(reader("CreatedDate")),
                                .PricingUnit = If(IsDBNull(reader("PricingUnit")), "Kilo", Convert.ToString(reader("PricingUnit"))),
                                .BoxQuantity = If(IsDBNull(reader("BoxQuantity")), Nothing, Convert.ToDecimal(reader("BoxQuantity"))),
                                .BoxSize = 2D ' Default
                            }
                        End If
                    End Using
                End Using
            End Try
        Catch ex As Exception
            MessageBox.Show($"Error getting product: {ex.Message}")
            Return Nothing
        End Try
        Return Nothing
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
                                .Quantity = Convert.ToDecimal(reader("Quantity")),
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

    Public Function UpdateProductStock(productID As Integer, quantitySold As Decimal) As Boolean
        Try
            ' Use the ProductDAL method for standalone stock updates (not within transaction)
            Dim productDAL As New ProductDAL()
            Return productDAL.UpdateStockForBoxedProduct(productID, quantitySold)
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
                                .Quantity = Convert.ToDecimal(reader("Quantity")),
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
    Public Property Quantity As Decimal
    Public Property UnitPrice As Decimal
    Public Property TotalAmount As Decimal
    Public Property SaleDate As DateTime
    Public Property CustomerName As String
    Public Property SupplierName As String
    Public Property Notes As String
End Class
