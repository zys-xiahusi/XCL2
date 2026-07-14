Imports System.Net.Http
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports Microsoft.Win32

Public Class PageOtherSkin
    Private Shadows IsLoaded As Boolean = False
    Private BaseUrl As String = "http://192.168.1.104:3000"
    Private SelectedSkinPath As String = ""
    Private SelectedCapePath As String = ""

    Private Sub PageOtherSkin_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If IsLoaded Then Return
        IsLoaded = True
        BaseUrl = Settings.Get(Of String)("XenithAuthServer")
        If String.IsNullOrEmpty(BaseUrl) Then BaseUrl = "http://192.168.1.104:3000/"
        If Not BaseUrl.EndsWith("/") Then BaseUrl &= "/"
        LoadCurrentSkin()
    End Sub

    Private Async Sub LoadCurrentSkin()
        Dim token = Settings.Get(Of String)("LoginXenithToken")
        If String.IsNullOrEmpty(token) Then
            LabStatus.Text = "请先登录 XENITH 账号"
            Return
        End If
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Authorization = New Net.Http.Headers.AuthenticationHeaderValue("Bearer", token)
                Dim response = Await client.GetAsync($"{BaseUrl}api/auth/profile")
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim obj = JObject.Parse(json)
                If CBool(obj("success")) Then
                    Dim user = obj("user")
                    Dim skinUrl = user("skin_url")?.ToString()
                    Dim capeUrl = user("cape_url")?.ToString()
                    If Not String.IsNullOrEmpty(skinUrl) Then
                        ImgSkinPreview.Source = New System.Windows.Media.Imaging.BitmapImage(New Uri(skinUrl))
                        LabSkinStatus.Text = "已有皮肤"
                    Else
                        LabSkinStatus.Text = "暂无皮肤"
                    End If
                    If Not String.IsNullOrEmpty(capeUrl) Then
                        ImgCapePreview.Source = New System.Windows.Media.Imaging.BitmapImage(New Uri(capeUrl))
                        LabCapeStatus.Text = "已有披风"
                    Else
                        LabCapeStatus.Text = "暂无披风"
                    End If
                End If
            End Using
        Catch ex As Exception
            LabStatus.Text = "加载失败：" & ex.Message
        End Try
    End Sub

    Private Sub BtnSelectSkin_Click(sender As Object, e As RoutedEventArgs) Handles BtnSelectSkin.Click
        Dim dialog As New OpenFileDialog()
        dialog.Filter = "PNG 图片|*.png"
        dialog.Title = "选择皮肤文件"
        If dialog.ShowDialog() = True Then
            SelectedSkinPath = dialog.FileName
            ImgSkinPreview.Source = New System.Windows.Media.Imaging.BitmapImage(New Uri(SelectedSkinPath))
            LabSkinStatus.Text = "已选择：" & Path.GetFileName(SelectedSkinPath)
        End If
    End Sub

    Private Async Sub BtnUploadSkin_Click(sender As Object, e As RoutedEventArgs) Handles BtnUploadSkin.Click
        If String.IsNullOrEmpty(SelectedSkinPath) Then
            LabStatus.Text = "请先选择皮肤文件"
            Return
        End If
        Dim token = Settings.Get(Of String)("LoginXenithToken")
        If String.IsNullOrEmpty(token) Then
            LabStatus.Text = "请先登录 XENITH 账号"
            Return
        End If
        LabStatus.Text = "上传中..."
        BtnUploadSkin.IsEnabled = False
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Authorization = New Net.Http.Headers.AuthenticationHeaderValue("Bearer", token)
                Dim content = New MultipartFormDataContent()
                Dim fileStream = New FileStream(SelectedSkinPath, FileMode.Open)
                content.Add(New StreamContent(fileStream), "skin", Path.GetFileName(SelectedSkinPath))
                Dim response = Await client.PostAsync($"{BaseUrl}api/user/upload-skin", content)
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim obj = JObject.Parse(json)
                If CBool(obj("success")) Then
                    LabStatus.Text = "皮肤上传成功"
                    LabSkinStatus.Text = "皮肤已更新"
                Else
                    LabStatus.Text = "上传失败：" & obj("message").ToString()
                End If
            End Using
        Catch ex As Exception
            LabStatus.Text = "上传错误：" & ex.Message
        Finally
            BtnUploadSkin.IsEnabled = True
        End Try
    End Sub

    Private Sub BtnSelectCape_Click(sender As Object, e As RoutedEventArgs) Handles BtnSelectCape.Click
        Dim dialog As New OpenFileDialog()
        dialog.Filter = "PNG 图片|*.png"
        dialog.Title = "选择披风文件"
        If dialog.ShowDialog() = True Then
            SelectedCapePath = dialog.FileName
            ImgCapePreview.Source = New System.Windows.Media.Imaging.BitmapImage(New Uri(SelectedCapePath))
            LabCapeStatus.Text = "已选择：" & Path.GetFileName(SelectedCapePath)
        End If
    End Sub

    Private Async Sub BtnUploadCape_Click(sender As Object, e As RoutedEventArgs) Handles BtnUploadCape.Click
        If String.IsNullOrEmpty(SelectedCapePath) Then
            LabStatus.Text = "请先选择披风文件"
            Return
        End If
        Dim token = Settings.Get(Of String)("LoginXenithToken")
        If String.IsNullOrEmpty(token) Then
            LabStatus.Text = "请先登录 XENITH 账号"
            Return
        End If
        LabStatus.Text = "上传中..."
        BtnUploadCape.IsEnabled = False
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Authorization = New Net.Http.Headers.AuthenticationHeaderValue("Bearer", token)
                Dim content = New MultipartFormDataContent()
                Dim fileStream = New FileStream(SelectedCapePath, FileMode.Open)
                content.Add(New StreamContent(fileStream), "cape", Path.GetFileName(SelectedCapePath))
                Dim response = Await client.PostAsync($"{BaseUrl}api/user/upload-cape", content)
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim obj = JObject.Parse(json)
                If CBool(obj("success")) Then
                    LabStatus.Text = "披风上传成功"
                    LabCapeStatus.Text = "披风已更新"
                Else
                    LabStatus.Text = "上传失败：" & obj("message").ToString()
                End If
            End Using
        Catch ex As Exception
            LabStatus.Text = "上传错误：" & ex.Message
        Finally
            BtnUploadCape.IsEnabled = True
        End Try
    End Sub
End Class
