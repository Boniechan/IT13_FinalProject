Imports System.Windows.Forms
Imports MySql.Data.MySqlClient
Imports System.ComponentModel.DataAnnotations
Imports System.IO

Public Class ProductDAL
    ' Input validation constants
    Private Const MAX_NAME_LENGTH As Integer = 100
    Private Const MAX_CATEGORY_LENGTH As Integer = 50
    Private Const MAX_DESCRIPTION_LENGTH As Integer = 500
    Private Const MIN_PRICE As Decimal = 0.01
    Private Const MAX_PRICE As Decimal = 999999.99

    Public Function GetAllProducts() As List(Of Product)
        Dim products As New List(Of Product)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return products

                ' Use more specific column selection instead of *
                Dim query As String = "SELECT ProductID, ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate FROM products ORDER BY ProductName LIMIT 1000"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.CommandTimeout = 30 ' Set timeout
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
                                .CreatedDate = Convert.ToDateTime(reader("CreatedDate"))
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
        ' Input validation
        If Not ValidateProduct(product) Then
            Return False
        End If

        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim query As String = "INSERT INTO products (ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate) VALUES (@name, @category, @price, @stock, @description, @supplierID, @createdDate)"
                Using cmd As New MySqlCommand(query, conn)
                    ' Use proper parameter types
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, MAX_NAME_LENGTH).Value = product.ProductName.Trim()
                    cmd.Parameters.Add("@category", MySqlDbType.VarChar, MAX_CATEGORY_LENGTH).Value = product.Category.Trim()
                    cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = product.Price
                    cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = product.Stock
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, MAX_DESCRIPTION_LENGTH).Value = If(String.IsNullOrEmpty(product.Description), "", product.Description.Trim())
                    cmd.Parameters.Add("@supplierID", MySqlDbType.Int32).Value = product.SupplierID
                    cmd.Parameters.Add("@createdDate", MySqlDbType.DateTime).Value = DateTime.Now

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
        ' Input validation
        If Not ValidateProduct(product) OrElse product.ProductID <= 0 Then
            Return False
        End If

        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim query As String = "UPDATE products SET ProductName = @name, Category = @category, Price = @price, Stock = @stock, Description = @description, SupplierID = @supplierID WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = product.ProductID
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, MAX_NAME_LENGTH).Value = product.ProductName.Trim()
                    cmd.Parameters.Add("@category", MySqlDbType.VarChar, MAX_CATEGORY_LENGTH).Value = product.Category.Trim()
                    cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = product.Price
                    cmd.Parameters.Add("@stock", MySqlDbType.Int32).Value = product.Stock
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, MAX_DESCRIPTION_LENGTH).Value = If(String.IsNullOrEmpty(product.Description), "", product.Description.Trim())
                    cmd.Parameters.Add("@supplierID", MySqlDbType.Int32).Value = product.SupplierID

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

    Public Function DeleteProduct(productID As Integer) As Boolean
        ' Validate input
        If productID <= 0 Then
            MessageBox.Show("Invalid product ID.")
            Return False
        End If

        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                ' Check if product exists and has no dependencies first
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
        ' Null checks
        If product Is Nothing Then
            MessageBox.Show("Product data is required.")
            Return False
        End If

        ' Name validation
        If String.IsNullOrWhiteSpace(product.ProductName) OrElse product.ProductName.Length > MAX_NAME_LENGTH Then
            MessageBox.Show($"Product name is required and must be less than {MAX_NAME_LENGTH} characters.")
            Return False
        End If

        ' Category validation
        If String.IsNullOrWhiteSpace(product.Category) OrElse product.Category.Length > MAX_CATEGORY_LENGTH Then
            MessageBox.Show($"Category is required and must be less than {MAX_CATEGORY_LENGTH} characters.")
            Return False
        End If

        ' Price validation
        If product.Price < MIN_PRICE OrElse product.Price > MAX_PRICE Then
            MessageBox.Show($"Price must be between {MIN_PRICE:C} and {MAX_PRICE:C}.")
            Return False
        End If

        ' Stock validation
        If product.Stock < 0 OrElse product.Stock > 999999 Then
            MessageBox.Show("Stock must be between 0 and 999,999.")
            Return False
        End If

        ' Supplier ID validation
        If product.SupplierID <= 0 Then
            MessageBox.Show("Valid Supplier ID is required.")
            Return False
        End If

        ' Description validation
        If Not String.IsNullOrEmpty(product.Description) AndAlso product.Description.Length > MAX_DESCRIPTION_LENGTH Then
            MessageBox.Show($"Description must be less than {MAX_DESCRIPTION_LENGTH} characters.")
            Return False
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