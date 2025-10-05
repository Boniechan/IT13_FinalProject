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

    Public Function CreateClientAccount(user As User) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "INSERT INTO users (Username, Password, UserType, FullName, Email, CreatedDate) VALUES (@username, @password, 'Client', @fullname, @email, NOW())"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", user.Username)
                    cmd.Parameters.AddWithValue("@password", user.Password)
                    cmd.Parameters.AddWithValue("@fullname", user.FullName)
                    cmd.Parameters.AddWithValue("@email", user.Email)
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                    Return rowsAffected > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error creating client account: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function IsUsernameAvailable(username As String) As Boolean
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "SELECT COUNT(*) FROM users WHERE Username = @username"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    Return count = 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error checking username availability: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function GetAllClients() As List(Of User)
        Dim clients As New List(Of User)
        Try
            Using conn As MySqlConnection = DatabaseConnection.GetConnection()
                Dim query As String = "SELECT UserID, Username, UserType, FullName, Email, CreatedDate FROM users WHERE UserType = 'Client' ORDER BY CreatedDate DESC"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim client As New User With {
                                .UserID = Convert.ToInt32(reader("UserID")),
                                .Username = reader("Username").ToString(),
                                .UserType = reader("UserType").ToString(),
                                .FullName = reader("FullName").ToString(),
                                .Email = reader("Email").ToString(),
                                .CreatedDate = Convert.ToDateTime(reader("CreatedDate"))
                            }
                            clients.Add(client)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error retrieving clients: " & ex.Message)
        End Try
        Return clients
    End Function
End Class