Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class UserDAL
    Public Function Authenticate(username As String, password As String) As User
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "SELECT UserID, Username, Password, UserType, FullName, Email, CreatedDate FROM users WHERE Username = @username AND Password = @password LIMIT 1"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@password", password)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim u As New User With {
                                .UserID = Convert.ToInt32(reader("UserID")),
                                .Username = reader("Username").ToString(),
                                .Password = reader("Password").ToString(),
                                .UserType = reader("UserType").ToString(),
                                .FullName = reader("FullName").ToString(),
                                .Email = reader("Email").ToString(),
                                .CreatedDate = Convert.ToDateTime(reader("CreatedDate"))
                            }
                            Return u
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error during authentication: " & ex.Message)
        End Try
        Return Nothing
    End Function
End Class