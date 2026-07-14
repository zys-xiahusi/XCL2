Imports System.Net.Http
Imports Newtonsoft.Json.Linq

Public Class PageOtherXenith
    Private Shadows IsLoaded As Boolean = False
    Private BaseUrl As String = "http://192.168.1.104:3000"

    Private Sub PageOtherXenith_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If IsLoaded Then Return
        IsLoaded = True
        TextServerUrl.Text = Settings.Get(Of String)("XenithAuthServer")
        If String.IsNullOrEmpty(TextServerUrl.Text) Then
            TextServerUrl.Text = "http://192.168.1.104:3000"
        End If
        BaseUrl = TextServerUrl.Text
    End Sub

    Private Async Sub BtnLogin_Click(sender As Object, e As RoutedEventArgs) Handles BtnLogin.Click
        BaseUrl = TextServerUrl.Text.Trim
        If Not BaseUrl.EndsWith("/") Then BaseUrl &= "/"
        Settings.Set("XenithAuthServer", BaseUrl)
        Dim user = TextUsername.Text.Trim
        Dim pass = TextPassword.Password.Trim
        If String.IsNullOrEmpty(user) OrElse String.IsNullOrEmpty(pass) Then
            LabStatus.Text = "请输入账号和密码"
            Return
        End If
        LabStatus.Text = "登录中..."
        BtnLogin.IsEnabled = False
        Try
            Using client As New HttpClient()
                Dim content As New StringContent(
                    JsonConvert.SerializeObject(New With {.username = user, .password = pass}),
                    Encoding.UTF8,
                    "application/json"
                )
                Dim response = Await client.PostAsync($"{BaseUrl}api/auth/login", content)
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim obj = JObject.Parse(json)
                If CBool(obj("success")) Then
                    Dim token = obj("token").ToString()
                    Dim username = obj("username").ToString()
                    Dim uuid = obj("uuid").ToString()
                    Settings.Set("LoginXenithAccount", username)
                    Settings.Set("LoginXenithToken", token)
                    Settings.Set("LoginXenithUuid", uuid)
                    Settings.Set("LoginType", McLoginType.XENITH)
                    LabStatus.Text = "登录成功"
                    FormMain.RefreshLoginState()
                Else
                    LabStatus.Text = "登录失败：" & obj("message").ToString()
                End If
            End Using
        Catch ex As Exception
            LabStatus.Text = "网络错误：" & ex.Message
        Finally
            BtnLogin.IsEnabled = True
        End Try
    End Sub

    Private Async Sub BtnRegister_Click(sender As Object, e As RoutedEventArgs) Handles BtnRegister.Click
        BaseUrl = TextServerUrl.Text.Trim
        If Not BaseUrl.EndsWith("/") Then BaseUrl &= "/"
        Settings.Set("XenithAuthServer", BaseUrl)
        Dim user = TextUsername.Text.Trim
        Dim pass = TextPassword.Password.Trim
        If String.IsNullOrEmpty(user) OrElse String.IsNullOrEmpty(pass) Then
            LabStatus.Text = "请输入账号和密码"
            Return
        End If
        If user.Length < 3 OrElse pass.Length < 6 Then
            LabStatus.Text = "用户名至少3字符，密码至少6字符"
            Return
        End If
        LabStatus.Text = "注册中..."
        BtnRegister.IsEnabled = False
        Try
            Using client As New HttpClient()
                Dim content As New StringContent(
                    JsonConvert.SerializeObject(New With {.username = user, .password = pass}),
                    Encoding.UTF8,
                    "application/json"
                )
                Dim response = Await client.PostAsync($"{BaseUrl}api/auth/register", content)
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim obj = JObject.Parse(json)
                If CBool(obj("success")) Then
                    Dim token = obj("token").ToString()
                    Dim username = obj("username").ToString()
                    Dim uuid = obj("uuid").ToString()
                    Settings.Set("LoginXenithAccount", username)
                    Settings.Set("LoginXenithToken", token)
                    Settings.Set("LoginXenithUuid", uuid)
                    Settings.Set("LoginType", McLoginType.XENITH)
                    LabStatus.Text = "注册成功，已自动登录"
                    FormMain.RefreshLoginState()
                Else
                    LabStatus.Text = "注册失败：" & obj("message").ToString()
                End If
            End Using
        Catch ex As Exception
            LabStatus.Text = "网络错误：" & ex.Message
        Finally
            BtnRegister.IsEnabled = True
        End Try
    End Sub
End Class
