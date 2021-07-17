﻿Public Class POSMAIN
    Private Sub PRODUKToolStripMenuItem_Click(sender As Object, e As EventArgs)
        FormProduk.MdiParent = Me
        FormProduk.Show()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
        LabelStation.Text = Environment.GetEnvironmentVariable("station")
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim waktu As New Date
        lbljam.Text = Format(Now, "HH:mm:ss")
        lbldate.Text = Format(Now, "ddd, dd MMM yyyy")
    End Sub

    Private Sub btnkasir_Click_1(sender As Object, e As EventArgs) Handles btnkasir.Click
        Cursor = Cursors.WaitCursor
        FormKasir.MdiParent = Me
        FormProduk.Close()
        FormLaporan.Close()
        FormKasir.Show()
        Cursor = Cursors.Default
    End Sub

    Private Sub btnproduk_Click(sender As Object, e As EventArgs) Handles btnproduk.Click
        Cursor = Cursors.WaitCursor
        FormProduk.MdiParent = Me
        FormKasir.Close()
        FormLaporan.Close()
        FormProduk.Show()
        Cursor = Cursors.Default
    End Sub

    Private Sub btnlaporan_Click(sender As Object, e As EventArgs) Handles btnlaporan.Click
        Cursor = Cursors.WaitCursor
        FormLaporan.MdiParent = Me
        FormKasir.Close()
        FormProduk.Close()
        FormLaporan.Show()
        Cursor = Cursors.Default
    End Sub

    Private Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click

        Dim nik = TextBoxLoginUsername.Text
        Dim password = TextBoxLoginPassword.Text

        If String.IsNullOrEmpty(nik) Or String.IsNullOrEmpty(password) Then
            MessageBox.Show("Harap isi NIK dan Password", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Try
                Call conecDB()
                comDB = New MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM users where nik='" + nik + "' and password='" + md5(password) + "'", connDB)
                rdDB = comDB.ExecuteReader
                If rdDB.HasRows Then
                    rdDB.Read()
                    LabelUser.Text = rdDB.Item("nama")
                    GroupBoxLogin.Visible = False
                    GroupBoxMenu.Visible = True
                    PanelIdentitas.Visible = True
                    rdDB.Close()

                    'AMBIL IDENTITAS TOKO
                    Try
                        comDB = New MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM toko", connDB)
                        rdDB = comDB.ExecuteReader
                        If rdDB.HasRows Then
                            rdDB.Read()
                            LabelToko.Text = rdDB.Item("nama_toko")
                            LabelSubToko.Text = rdDB.Item("alamat") + ". Telp. " + rdDB.Item("telepon")
                            rdDB.Close()
                        End If
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                Else
                    MessageBox.Show("Username tidak ditemukan", "Perhatian", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                rdDB.Close()
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If
    End Sub

    Public Function md5(sPassword As String) As String
        Dim x As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim bs As Byte() = System.Text.Encoding.UTF8.GetBytes(sPassword)
        bs = x.ComputeHash(bs)
        Dim s As New System.Text.StringBuilder()
        For Each b As Byte In bs
            s.Append(b.ToString("x2").ToLower())
        Next
        Return s.ToString()
    End Function

End Class