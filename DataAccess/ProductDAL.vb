Imports System.Windows.Forms
Imports MySql.Data.MySqlClient
Imports System.IO

Public Class ProductDAL
    ' Input validation constants
    Private Const MAX_NAME_LENGTH As Integer = 100
    Private Const MAX_CATEGORY_LENGTH As Integer = 50
    Private Const MAX_DESCRIPTION_LENGTH As Integer = 500
    Private Const MIN_PRICE As Decimal = 0.01
    Private Const MAX_PRICE As Decimal = 999999.99
    Private Const DEFAULT_BOX_SIZE As Decimal = 2D ' 2 kilos per box

    Public Function GetAllProducts() As List(Of Product)
        Dim products As New List(Of Product)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return products

                Dim query As String = "SELECT ProductID, ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate, PricingUnit, BoxQuantity, BoxSize FROM products ORDER BY ProductName LIMIT 1000"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.CommandTimeout = 30
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim product As New Product With {
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
                                .BoxSize = If(IsDBNull(reader("BoxSize")), DEFAULT_BOX_SIZE, Convert.ToDecimal(reader("BoxSize")))
                            }
                            products.Add(product)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            LogError($"Database error in GetAllProducts: {ex.Number}")
            MessageBox.Show("Error retrieving products. Please try again.")
        Catch ex As Exception
            LogError($"Unexpected error in GetAllProducts: {ex.Message}")
            MessageBox.Show("Error retrieving products. Please try again.")
        End Try
        Return products
    End Function

    Public Function AddProduct(product As Product) As Boolean
        If Not ValidateProduct(product) Then
            Return False
        End If

        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim query As String = "INSERT INTO products (ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate, PricingUnit, BoxQuantity, BoxSize) VALUES (@name, @category, @price, @stock, @description, @supplierID, @createdDate, @pricingUnit, @boxQuantity, @boxSize)"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, MAX_NAME_LENGTH).Value = product.ProductName.Trim()
                    cmd.Parameters.Add("@category", MySqlDbType.VarChar, MAX_CATEGORY_LENGTH).Value = product.Category.Trim()
                    cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = product.Price
                    cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = product.Stock
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, MAX_DESCRIPTION_LENGTH).Value = If(String.IsNullOrEmpty(product.Description), "", product.Description.Trim())
                    cmd.Parameters.Add("@supplierID", MySqlDbType.Int32).Value = product.SupplierID
                    cmd.Parameters.Add("@createdDate", MySqlDbType.DateTime).Value = DateTime.Now
                    cmd.Parameters.Add("@pricingUnit", MySqlDbType.VarChar, 20).Value = If(String.IsNullOrEmpty(product.PricingUnit), "Kilo", product.PricingUnit)
                    cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = If(product.BoxQuantity.HasValue, product.BoxQuantity.Value, If(product.PricingUnit = "Box", product.BoxSize, DBNull.Value))
                    cmd.Parameters.Add("@boxSize", MySqlDbType.Decimal).Value = If(product.BoxSize > 0, product.BoxSize, DEFAULT_BOX_SIZE)

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As MySqlException
            LogError($"Database error in AddProduct: {ex.Number}")
            MessageBox.Show("Error adding product. Please check your input and try again.")
            Return False
        Catch ex As Exception
            LogError($"Unexpected error in AddProduct: {ex.Message}")
            MessageBox.Show("Error adding product. Please try again.")
            Return False
        End Try
    End Function

    Public Function UpdateProduct(product As Product) As Boolean
        If Not ValidateProduct(product) OrElse product.ProductID <= 0 Then
            Return False
        End If

        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim query As String = "UPDATE products SET ProductName = @name, Category = @category, Price = @price, Stock = @stock, Description = @description, SupplierID = @supplierID, PricingUnit = @pricingUnit, BoxQuantity = @boxQuantity, BoxSize = @boxSize WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = product.ProductID
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, MAX_NAME_LENGTH).Value = product.ProductName.Trim()
                    cmd.Parameters.Add("@category", MySqlDbType.VarChar, MAX_CATEGORY_LENGTH).Value = product.Category.Trim()
                    cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = product.Price
                    cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = product.Stock
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, MAX_DESCRIPTION_LENGTH).Value = If(String.IsNullOrEmpty(product.Description), "", product.Description.Trim())
                    cmd.Parameters.Add("@supplierID", MySqlDbType.Int32).Value = product.SupplierID
                    cmd.Parameters.Add("@pricingUnit", MySqlDbType.VarChar, 20).Value = If(String.IsNullOrEmpty(product.PricingUnit), "Kilo", product.PricingUnit)
                    cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = If(product.BoxQuantity.HasValue, product.BoxQuantity.Value, DBNull.Value)
                    cmd.Parameters.Add("@boxSize", MySqlDbType.Decimal).Value = If(product.BoxSize > 0, product.BoxSize, DEFAULT_BOX_SIZE)

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As MySqlException
            LogError($"Database error in UpdateProduct: {ex.Number}")
            MessageBox.Show("Error updating product. Please check your input and try again.")
            Return False
        Catch ex As Exception
            LogError($"Unexpected error in UpdateProduct: {ex.Message}")
            MessageBox.Show("Error updating product. Please try again.")
            Return False
        End Try
    End Function

    Public Function UpdateStockForBoxedProduct(productID As Integer, quantitySold As Decimal) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim product As Product = GetProductById(productID)
                If product Is Nothing Then Return False

                If product.IsSoldByBox Then
                    ' Handle boxed products with 2-kilo box logic
                    Dim currentBoxQuantity As Decimal = If(product.BoxQuantity.HasValue, product.BoxQuantity.Value, product.BoxSize)

                    If quantitySold <= currentBoxQuantity Then
                        ' Sale can be fulfilled from the current box
                        Dim newBoxQuantity As Decimal = currentBoxQuantity - quantitySold

                        If newBoxQuantity <= 0 Then
                            ' Current box is now empty, move to next box
                            If product.Stock > 1 Then
                                ' Move to next box
                                Dim query As String = "UPDATE products SET Stock = Stock - 1, BoxQuantity = @boxSize WHERE ProductID = @id AND Stock > 1"
                                Using cmd As New MySqlCommand(query, conn)
                                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                    cmd.Parameters.Add("@boxSize", MySqlDbType.Decimal).Value = product.BoxSize
                                    Return cmd.ExecuteNonQuery() > 0
                                End Using
                            Else
                                ' No more boxes available
                                Dim query As String = "UPDATE products SET Stock = 0, BoxQuantity = 0 WHERE ProductID = @id"
                                Using cmd As New MySqlCommand(query, conn)
                                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                    Return cmd.ExecuteNonQuery() > 0
                                End Using
                            End If
                        Else
                            ' Update remaining quantity in current box
                            Dim query As String = "UPDATE products SET BoxQuantity = @boxQuantity WHERE ProductID = @id"
                            Using cmd As New MySqlCommand(query, conn)
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                                cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = newBoxQuantity
                                Return cmd.ExecuteNonQuery() > 0
                            End Using
                        End If
                    Else
                        ' Need multiple boxes
                        Dim totalAvailable As Decimal = currentBoxQuantity + ((product.Stock - 1) * product.BoxSize)

                        If quantitySold > totalAvailable Then
                            MessageBox.Show($"Insufficient stock. Available: {totalAvailable:F1}kg, Requested: {quantitySold:F1}kg")
                            Return False
                        End If

                        ' Calculate how many complete boxes we need beyond the current one
                        Dim remainingNeeded As Decimal = quantitySold - currentBoxQuantity
                        Dim completeBoxesNeeded As Integer = CInt(Math.Floor(remainingNeeded / product.BoxSize))
                        Dim finalBoxAmount As Decimal = remainingNeeded Mod product.BoxSize

                        Dim newStock As Integer = product.Stock - 1 - completeBoxesNeeded ' Remove current box + complete boxes
                        Dim newBoxQuantity As Decimal = product.BoxSize - finalBoxAmount

                        If finalBoxAmount = 0 Then
                            ' We used exact boxes, so current box quantity should be full box size
                            newBoxQuantity = product.BoxSize
                        End If

                        If newStock <= 0 Then
                            newStock = 0
                            newBoxQuantity = 0
                        End If

                        Dim query As String = "UPDATE products SET Stock = @stock, BoxQuantity = @boxQuantity WHERE ProductID = @id"
                        Using cmd As New MySqlCommand(query, conn)
                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                            cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = newStock
                            cmd.Parameters.Add("@boxQuantity", MySqlDbType.Decimal).Value = newBoxQuantity
                            Return cmd.ExecuteNonQuery() > 0
                        End Using
                    End If
                Else
                    ' For non-boxed products, use traditional stock reduction
                    Dim query As String = "UPDATE products SET Stock = Stock - @quantity WHERE ProductID = @id AND Stock >= @quantity"
                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                        cmd.Parameters.Add("@quantity", MySqlDbType.Decimal).Value = quantitySold
                        Return cmd.ExecuteNonQuery() > 0
                    End Using
                End If
            End Using
        Catch ex As Exception
            LogError($"Error updating stock for boxed product: {ex.Message}")
            MessageBox.Show("Error updating product stock.")
            Return False
        End Try
    End Function

    Public Function GetProductById(productID As Integer) As Product
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return Nothing

                Dim query As String = "SELECT ProductID, ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate, PricingUnit, BoxQuantity, BoxSize FROM products WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn)
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
                                .BoxSize = If(IsDBNull(reader("BoxSize")), DEFAULT_BOX_SIZE, Convert.ToDecimal(reader("BoxSize")))
                            }
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            LogError($"Error getting product by ID: {ex.Message}")
        End Try
        Return Nothing
    End Function

    Public Function DeleteProduct(productID As Integer) As Boolean
        If productID <= 0 Then
            MessageBox.Show("Invalid product ID.")
            Return False
        End If

        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim checkQuery As String = "SELECT COUNT(*) FROM sales WHERE ProductID = @id"
                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                    Dim salesCount As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                    If salesCount > 0 Then
                        MessageBox.Show("Cannot delete product. It has associated sales records.")
                        Return False
                    End If
                End Using

                Dim query As String = "DELETE FROM products WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = productID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As MySqlException
            LogError($"Database error in DeleteProduct: {ex.Number}")
            MessageBox.Show("Error deleting product. Please try again.")
            Return False
        Catch ex As Exception
            LogError($"Unexpected error in DeleteProduct: {ex.Message}")
            MessageBox.Show("Error deleting product. Please try again.")
            Return False
        End Try
    End Function

    Private Function ValidateProduct(product As Product) As Boolean
        If product Is Nothing Then
            MessageBox.Show("Product data is required.")
            Return False
        End If

        If String.IsNullOrWhiteSpace(product.ProductName) OrElse product.ProductName.Length > MAX_NAME_LENGTH Then
            MessageBox.Show($"Product name is required and must be less than {MAX_NAME_LENGTH} characters.")
            Return False
        End If

        If String.IsNullOrWhiteSpace(product.Category) OrElse product.Category.Length > MAX_CATEGORY_LENGTH Then
            MessageBox.Show($"Category is required and must be less than {MAX_CATEGORY_LENGTH} characters.")
            Return False
        End If

        If product.Price < MIN_PRICE OrElse product.Price > MAX_PRICE Then
            MessageBox.Show($"Price must be between {MIN_PRICE:C} and {MAX_PRICE:C}.")
            Return False
        End If

        If product.Stock < 0 OrElse product.Stock > 999999 Then
            MessageBox.Show("Stock must be between 0 and 999,999.")
            Return False
        End If

        If product.SupplierID <= 0 Then
            MessageBox.Show("Valid Supplier ID is required.")
            Return False
        End If

        If Not String.IsNullOrEmpty(product.Description) AndAlso product.Description.Length > MAX_DESCRIPTION_LENGTH Then
            MessageBox.Show($"Description must be less than {MAX_DESCRIPTION_LENGTH} characters.")
            Return False
        End If

        If String.IsNullOrWhiteSpace(product.PricingUnit) Then
            product.PricingUnit = "Kilo"
        ElseIf Not {"Kilo", "Box", "Piece"}.Contains(product.PricingUnit) Then
            MessageBox.Show("Pricing unit must be Kilo, Box, or Piece.")
            Return False
        End If

        If product.PricingUnit = "Box" Then
            If product.BoxSize <= 0 Then
                product.BoxSize = DEFAULT_BOX_SIZE
            End If
            If product.BoxQuantity.HasValue AndAlso (product.BoxQuantity.Value < 0 OrElse product.BoxQuantity.Value > product.BoxSize) Then
                MessageBox.Show($"Box quantity must be between 0 and {product.BoxSize}.")
                Return False
            End If
        End If

        Return True
    End Function

    Private Sub LogError(message As String)
        Try
            Dim logPath As String = Path.Combine(Application.StartupPath, "error.log")
            File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}")
        Catch
            ' Fail silently
        End Try
    End Sub
End Class