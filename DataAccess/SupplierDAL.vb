Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class SupplierDAL
    Public Function GetAllSuppliers() As List(Of Supplier)
        Dim suppliers As New List(Of Supplier)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return suppliers

                Dim query As String = "SELECT SupplierID, SupplierName, ContactPerson, Phone, Email, Address, CreatedDate FROM suppliers ORDER BY SupplierName"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.CommandTimeout = 30
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim supplier As New Supplier With {
                                .SupplierID = Convert.ToInt32(reader("SupplierID")),
                                .SupplierName = If(IsDBNull(reader("SupplierName")), "", Convert.ToString(reader("SupplierName"))),
                                .ContactPerson = If(IsDBNull(reader("ContactPerson")), "", Convert.ToString(reader("ContactPerson"))),
                                .Phone = If(IsDBNull(reader("Phone")), "", Convert.ToString(reader("Phone"))),
                                .Email = If(IsDBNull(reader("Email")), "", Convert.ToString(reader("Email"))),
                                .Address = If(IsDBNull(reader("Address")), "", Convert.ToString(reader("Address"))),
                                .CreatedDate = If(IsDBNull(reader("CreatedDate")), DateTime.Now, Convert.ToDateTime(reader("CreatedDate")))
                            }
                            suppliers.Add(supplier)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            MessageBox.Show($"Database error retrieving suppliers: {ex.Message}")
        Catch ex As Exception
            MessageBox.Show($"Error retrieving suppliers: {ex.Message}")
        End Try
        Return suppliers
    End Function

    Public Function GetSupplierById(supplierID As Integer) As Supplier
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return Nothing

                Dim query As String = "SELECT SupplierID, SupplierName, ContactPerson, Phone, Email, Address, CreatedDate FROM suppliers WHERE SupplierID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = supplierID
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New Supplier With {
                                .SupplierID = Convert.ToInt32(reader("SupplierID")),
                                .SupplierName = If(IsDBNull(reader("SupplierName")), "", Convert.ToString(reader("SupplierName"))),
                                .ContactPerson = If(IsDBNull(reader("ContactPerson")), "", Convert.ToString(reader("ContactPerson"))),
                                .Phone = If(IsDBNull(reader("Phone")), "", Convert.ToString(reader("Phone"))),
                                .Email = If(IsDBNull(reader("Email")), "", Convert.ToString(reader("Email"))),
                                .Address = If(IsDBNull(reader("Address")), "", Convert.ToString(reader("Address"))),
                                .CreatedDate = If(IsDBNull(reader("CreatedDate")), DateTime.Now, Convert.ToDateTime(reader("CreatedDate")))
                            }
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error retrieving supplier: {ex.Message}")
        End Try
        Return Nothing
    End Function

    Public Function AddSupplier(supplier As Supplier) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim query As String = "INSERT INTO suppliers (SupplierName, ContactPerson, Phone, Email, Address, CreatedDate) VALUES (@name, @contact, @phone, @email, @address, @created)"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = supplier.SupplierName
                    cmd.Parameters.Add("@contact", MySqlDbType.VarChar, 100).Value = If(String.IsNullOrEmpty(supplier.ContactPerson), DBNull.Value, supplier.ContactPerson)
                    cmd.Parameters.Add("@phone", MySqlDbType.VarChar, 20).Value = If(String.IsNullOrEmpty(supplier.Phone), DBNull.Value, supplier.Phone)
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 100).Value = If(String.IsNullOrEmpty(supplier.Email), DBNull.Value, supplier.Email)
                    cmd.Parameters.Add("@address", MySqlDbType.Text).Value = If(String.IsNullOrEmpty(supplier.Address), DBNull.Value, supplier.Address)
                    cmd.Parameters.Add("@created", MySqlDbType.DateTime).Value = DateTime.Now

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error adding supplier: {ex.Message}")
            Return False
        End Try
    End Function

    Public Function UpdateSupplier(supplier As Supplier) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                Dim query As String = "UPDATE suppliers SET SupplierName = @name, ContactPerson = @contact, Phone = @phone, Email = @email, Address = @address WHERE SupplierID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = supplier.SupplierID
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = supplier.SupplierName
                    cmd.Parameters.Add("@contact", MySqlDbType.VarChar, 100).Value = If(String.IsNullOrEmpty(supplier.ContactPerson), DBNull.Value, supplier.ContactPerson)
                    cmd.Parameters.Add("@phone", MySqlDbType.VarChar, 20).Value = If(String.IsNullOrEmpty(supplier.Phone), DBNull.Value, supplier.Phone)
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 100).Value = If(String.IsNullOrEmpty(supplier.Email), DBNull.Value, supplier.Email)
                    cmd.Parameters.Add("@address", MySqlDbType.Text).Value = If(String.IsNullOrEmpty(supplier.Address), DBNull.Value, supplier.Address)

                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error updating supplier: {ex.Message}")
            Return False
        End Try
    End Function

    Public Function DeleteSupplier(supplierID As Integer) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                If conn Is Nothing Then Return False

                ' Check if supplier has associated sales
                Dim checkQuery As String = "SELECT COUNT(*) FROM sales WHERE SupplierID = @id"
                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.Add("@id", MySqlDbType.Int32).Value = supplierID
                    Dim salesCount As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                    If salesCount > 0 Then
                        MessageBox.Show("Cannot delete supplier. It has associated sales records.")
                        Return False
                    End If
                End Using

                Dim query As String = "DELETE FROM suppliers WHERE SupplierID = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = supplierID
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error deleting supplier: {ex.Message}")
            Return False
        End Try
    End Function
End Class