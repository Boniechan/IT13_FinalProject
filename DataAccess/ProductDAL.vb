Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class ProductDAL
    Public Function GetAllProducts() As List(Of Product)
        Dim products As New List(Of Product)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "SELECT * FROM products ORDER BY ProductName"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim product As New Product With {
                                .ProductID = Convert.ToInt32(reader("ProductID")),
                                .ProductName = reader("ProductName").ToString(),
                                .Category = reader("Category").ToString(),
                                .Price = Convert.ToDecimal(reader("Price")),
                                .Stock = Convert.ToInt32(reader("Stock")),
                                .Description = reader("Description").ToString(),
                                .SupplierID = Convert.ToInt32(reader("SupplierID")),
                                .CreatedDate = Convert.ToDateTime(reader("CreatedDate"))
                            }
                            products.Add(product)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving products: " & ex.Message)
        End Try
        Return products
    End Function

    Public Function AddProduct(product As Product) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "INSERT INTO products (ProductName, Category, Price, Stock, Description, SupplierID, CreatedDate) VALUES (@name, @category, @price, @stock, @description, @supplierID, @createdDate)"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", product.ProductName)
                    cmd.Parameters.AddWithValue("@category", product.Category)
                    cmd.Parameters.AddWithValue("@price", product.Price)
                    cmd.Parameters.AddWithValue("@stock", product.Stock)
                    cmd.Parameters.AddWithValue("@description", product.Description)
                    cmd.Parameters.AddWithValue("@supplierID", product.SupplierID)
                    cmd.Parameters.AddWithValue("@createdDate", DateTime.Now)

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error adding product: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function UpdateProduct(product As Product) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "UPDATE products SET ProductName = @name, Category = @category, Price = @price, Stock = @stock, Description = @description, SupplierID = @supplierID WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", product.ProductID)
                    cmd.Parameters.AddWithValue("@name", product.ProductName)
                    cmd.Parameters.AddWithValue("@category", product.Category)
                    cmd.Parameters.AddWithValue("@price", product.Price)
                    cmd.Parameters.AddWithValue("@stock", product.Stock)
                    cmd.Parameters.AddWithValue("@description", product.Description)
                    cmd.Parameters.AddWithValue("@supplierID", product.SupplierID)

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating product: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function DeleteProduct(productID As Integer) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "DELETE FROM products WHERE ProductID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", productID)
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error deleting product: " & ex.Message)
            Return False
        End Try
    End Function
End Class