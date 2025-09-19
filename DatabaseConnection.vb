Imports System.Windows.Forms
Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class DatabaseConnection
    ' Updated connection string without SSL requirement for local development
    Private Shared ReadOnly connectionString As String = "Server=localhost;Database=fisheries_pos;Uid=root;Pwd=;SslMode=None;CharSet=utf8mb4;AllowUserVariables=True;"
    Private Shared connection As MySqlConnection
    Private Shared ReadOnly lockObject As New Object()

    Public Shared Function GetConnection() As MySqlConnection
        SyncLock lockObject
            Try
                If connection Is Nothing OrElse connection.State = ConnectionState.Closed Then
                    connection = New MySqlConnection(connectionString)
                    connection.Open()

                    ' Set session variables for additional security
                    Using cmd As New MySqlCommand("SET SESSION sql_mode = 'STRICT_TRANS_TABLES,NO_ZERO_DATE,NO_ZERO_IN_DATE,ERROR_FOR_DIVISION_BY_ZERO';", connection)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
                Return connection
            Catch ex As MySqlException
                LogSecurityEvent($"Database connection failed: Error {ex.Number} - {ex.Message}")
                MessageBox.Show($"Database connection error: {ex.Message}{Environment.NewLine}Error Code: {ex.Number}")
                Return Nothing
            Catch ex As Exception
                LogSecurityEvent($"Unexpected database error: {ex.Message}")
                MessageBox.Show($"Database connection error: {ex.Message}")
                Return Nothing
            End Try
        End SyncLock
    End Function

    Public Shared Sub CloseConnection()
        SyncLock lockObject
            If connection IsNot Nothing AndAlso connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End SyncLock
    End Sub

    Private Shared Sub LogSecurityEvent(message As String)
        Try
            Dim logPath As String = Path.Combine(Application.StartupPath, "security.log")
            File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}")
        Catch ex As Exception

            Try
                EventLog.WriteEntry("FisheriesPOS", $"Logging error: {ex.Message}", EventLogEntryType.Warning)
            Catch

            End Try
        End Try
    End Sub

    ' Method to validate and sanitize input (additional layer)
    Public Shared Function SanitizeInput(input As String) As String
        If String.IsNullOrEmpty(input) Then Return String.Empty

        ' Remove potentially dangerous characters
        Dim sanitized As String = input.Replace("'", "").Replace("""", "").Replace(";", "").Replace("--", "").Replace("/*", "").Replace("*/", "")

        ' Limit length to prevent buffer overflow attempts
        If sanitized.Length > 255 Then
            sanitized = sanitized.Substring(0, 255)
        End If

        Return sanitized.Trim()
    End Function


    Public Shared Function ValidateNumericInput(value As Object, minValue As Integer, maxValue As Integer) As Boolean
        If IsNumeric(value) Then
            Dim numValue As Integer = Convert.ToInt32(value)
            Return numValue >= minValue AndAlso numValue <= maxValue
        End If
        Return False
    End Function
End Class