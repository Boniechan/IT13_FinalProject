Public Class UserSession
    Public Shared Property UserID As Integer
    Public Shared Property Username As String
    Public Shared Property UserType As String
    Public Shared Property FullName As String

    Public Shared Sub ClearSession()
        UserID = 0
        Username = String.Empty
        UserType = String.Empty
        FullName = String.Empty
    End Sub
End Class