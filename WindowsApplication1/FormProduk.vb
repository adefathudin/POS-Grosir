﻿Public Class FormProduk

    Private Sub FormProduk_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim query = ""
        load_data_produk(query)
        PanelDetailProduk.Visible = False
    End Sub

    Private Sub TextBoxPencarianProduk_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxPencarianProduk.KeyPress
        If e.KeyChar = Chr(13) Then
            Dim query = TextBoxPencarianProduk.Text()
            load_data_produk(query)
            TextBoxPencarianProduk.SelectAll()
        End If
    End Sub

    Public Sub load_data_produk(query)

        Cursor = Cursors.WaitCursor
        Call conecDB()
        dt = New DataTable
        If String.IsNullOrEmpty(query) Then
            da = New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT prdcd,desc2,stok,rak FROM prodmast order by desc2", connDB)
        Else
            da = New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT prdcd,desc2,stok,rak FROM prodmast where barcd like '%" + query + "%' or prdcd like '%" + query + "%' or singkatan like '%" + query + "%' or desc2 like '%" + query + "%' or rak like '%" + query + "%' or kategori like '%" + query + "%'", connDB)
        End If
        Try
            comBuilderDB = New MySql.Data.MySqlClient.MySqlCommandBuilder(da) 'untuk bisa edit datagridview
            da.Fill(dt)
            DataGridViewBarang.DataSource = dt
            DataGridViewBarang.Columns(0).HeaderText = "PLU"
            DataGridViewBarang.Columns(1).HeaderText = "Deskripsi"
            DataGridViewBarang.Columns(2).HeaderText = "Stok"
            DataGridViewBarang.Columns(3).HeaderText = "Rak"
            'Zebra Table
            Me.DataGridViewBarang.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue
            Me.DataGridViewBarang.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            LabelTotalItem.Text = dt.Rows.Count.ToString() + " item"
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Cursor = Cursors.Default

    End Sub

    Private Sub ButtonInputBarang_Click(sender As Object, e As EventArgs) Handles ButtonInputBarang.Click
        ButtonInputBarang.Enabled = False
        PanelDetailProduk.Visible = True
        TextBoxBarcode.Focus()
        ClearTextBoxProduk()
        Cursor = Cursors.WaitCursor
        Dim plu = TextBoxProdukPLU.Text
        AmbilKategoriHargaUsed(plu)
        Call conecDB()

        'AMBIL PLU TERAKHIR
        Try
            comDB = New MySql.Data.MySqlClient.MySqlCommand("SELECT prdcd FROM prodmast order by prdcd desc limit 1", connDB)
            rdDB = comDB.ExecuteReader
            If rdDB.HasRows Then
                rdDB.Read()

                'GENERATE PLU BARU
                TextBoxProdukPLU.Text = rdDB.Item("prdcd") + 1

            End If
            rdDB.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        AmbilKategoriHarga()

        Cursor = Cursors.Default
    End Sub

    Private Sub ButtonProdukClose_Click(sender As Object, e As EventArgs) Handles ButtonProdukClose.Click
        PanelDetailProduk.Visible = False
        ButtonInputBarang.Enabled = True
    End Sub

    'MODUL TAMBAH ATAU EDIT BARANG

    Private Sub ButtonProdukSave_Click(sender As Object, e As EventArgs) Handles ButtonProdukSave.Click
        Dim plu = TextBoxProdukPLU.Text
        Dim barcd = TextBoxBarcode.Text
        Dim desc2 = TextBoxProdukDesc.Text
        Dim stok = TextBoxProdukStok.Text
        Dim rak = TextBoxRak.Text
        Dim kategori = TextBoxKategoriProduk.Text
        Dim acost = TextBoxProdukHargaBeli.Text
        Dim price = TextBoxProdukHargaJual.Text
        If ButtonInputBarang.Enabled = False Then
            Dim ask As DialogResult = MessageBox.Show("Simpan data produk?", "Perhatian", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If ask = DialogResult.Yes Then
                Try
                    Call conecDB()
                    comDB = New MySql.Data.MySqlClient.MySqlCommand("INSERT INTO prodmast (barcd,prdcd,desc2,stok,acost,price,rak,kategori) values ('" + barcd + "','" + plu + "','" + desc2 + "','" + stok + "','" + acost + "','" + price + "','" + rak + "','" + kategori + "')", connDB)
                    comDB.ExecuteNonQuery()
                    MessageBox.Show("Data produk berhasil disimpan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Dim query = ""
                    load_data_produk(query)
                    ClearTextBoxProduk()
                    PanelDetailProduk.Visible = False
                    ButtonInputBarang.Enabled = True
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
        Else
            Dim result As DialogResult = MessageBox.Show("Simpan perubahan pada produk ini?", "Perhatian", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Try
                    Call conecDB()
                    comDB = New MySql.Data.MySqlClient.MySqlCommand("UPDATE prodmast SET desc2='" + desc2.Replace("'", "''") + "', stok='" + stok + "', rak='" + rak + "',acost='" + acost + "',price='" + price + "',kategori='" + kategori + "' where prdcd='" + plu + "'", connDB)
                    comDB.ExecuteNonQuery()
                    MessageBox.Show("Data produk berhasil dirubah", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Dim query = ""
                    load_data_produk(query)
                    ClearTextBoxProduk()
                    PanelDetailProduk.Visible = False
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
        End If
    End Sub

    Private Sub ButtonProdukKategoriHarga_Click(sender As Object, e As EventArgs) Handles ButtonProdukKategoriHarga.Click
        FormProduk_KategoriHarga.ShowDialog()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        FormProduk_Rak.ShowDialog()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FormProduk_KategoriProduk.ShowDialog()
    End Sub

    Private Sub ButtonSaveKategoriHarga_Click(sender As Object, e As EventArgs) Handles ButtonSaveKategoriHarga.Click
        Dim plu = TextBoxProdukPLU.Text
        Dim kategoriharga_id = ComboBoxKategoriHarga.SelectedValue.ToString
        Try
            Call conecDB()
            comDB = New MySql.Data.MySqlClient.MySqlCommand("SELECT id FROM rel_kategoriharga WHERE plu='" + plu + "' AND kategoriharga_id='" + kategoriharga_id + "'", connDB)
            rdDB = comDB.ExecuteReader
            If rdDB.HasRows Then
                MessageBox.Show("Kategori harga ini sudah pernah ditambahkan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                Try
                    rdDB.Close()
                    Call conecDB()
                    comDB = New MySql.Data.MySqlClient.MySqlCommand("INSERT INTO rel_kategoriharga (kategoriharga_id,plu,harga) values ('" + kategoriharga_id + "'," + plu + ",'" + TextBoxKategoriHarga_harga.Text + "')", connDB)
                    comDB.ExecuteNonQuery()
                    AmbilKategoriHargaUsed(plu)
                    ClearTextBoxKategoriHarga()
                    rdDB.Close()
                Catch ex As Exception
                    MsgBox("INSERT KATEGORI HARGA : " + e.ToString)
                End Try
            End If
        Catch ex As Exception
            MsgBox("CEK EKSIST : " + ex.ToString)
        End Try
        rdDB.Close()
    End Sub

    Sub AmbilKategoriHargaUsed(plu)
        'AMBIL DATA KATEGORI HARGA

        Call conecDB()
        dt = New DataTable
        If String.IsNullOrEmpty(plu) Then
            DataGridViewKategoriHarga.DataSource = Nothing
        Else
            da = New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT b.id,a.nama_kategori,b.harga FROM ref_kategoriharga a join rel_kategoriharga b on a.id=b.kategoriharga_id where b.plu='" + plu + "' ", connDB)
            Try
                comBuilderDB = New MySql.Data.MySqlClient.MySqlCommandBuilder(da)
                da.Fill(dt)
                DataGridViewKategoriHarga.DataSource = dt
                DataGridViewKategoriHarga.Columns(0).HeaderText = "ID"
                DataGridViewKategoriHarga.Columns(1).HeaderText = "Kategori"
                DataGridViewKategoriHarga.Columns(2).HeaderText = "Harga"
                'Zebra Table
                Me.DataGridViewKategoriHarga.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue
                Me.DataGridViewKategoriHarga.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If
    End Sub

    Sub AmbilKategoriHarga()
        'AMBIL DATA KATEGORI HARGA

        Call conecDB()
        dt = New DataTable
        da = New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT * FROM ref_kategoriharga", connDB)

        Try
            comBuilderDB = New MySql.Data.MySqlClient.MySqlCommandBuilder(da)
            da.Fill(dt)
            ComboBoxKategoriHarga.DataSource = dt
            ComboBoxKategoriHarga.DisplayMember = "nama_kategori"
            ComboBoxKategoriHarga.ValueMember = "id"
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Sub DataGridViewBarang_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridViewBarang.CellMouseClick
        If e.RowIndex < 0 Then
            Exit Sub
        End If
        Dim plu = DataGridViewBarang.Rows(e.RowIndex).Cells(0).Value
        PanelDetailProduk.Visible = True
        Try
            Call conecDB()
            comDB = New MySql.Data.MySqlClient.MySqlCommand("SELECT barcd,prdcd,desc2,stok,rak,acost,price,kategori FROM prodmast where prdcd='" + plu + "'", connDB)
            rdDB = comDB.ExecuteReader
            If rdDB.HasRows Then
                rdDB.Read()
                TextBoxBarcode.Text = rdDB.Item("barcd")
                TextBoxProdukPLU.Text = rdDB.Item("prdcd")
                TextBoxProdukPLU.ReadOnly = True
                TextBoxProdukDesc.Text = rdDB.Item("desc2")
                TextBoxProdukStok.Text = rdDB.Item("stok")
                TextBoxProdukHargaBeli.Text = rdDB.Item("acost")
                TextBoxProdukHargaJual.Text = rdDB.Item("price")
                TextBoxRak.Text = rdDB.Item("rak")
                TextBoxKategoriProduk.Text = rdDB.Item("kategori")
            End If
            rdDB.Close()
            AmbilKategoriHarga()
            AmbilKategoriHargaUsed(plu)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub ButtonDeleteKategoriHarga_Click(sender As Object, e As EventArgs) Handles ButtonDeleteKategoriHarga.Click
        Dim plu = TextBoxProdukPLU.Text
        Dim result As DialogResult = MessageBox.Show("Hapus kategori terpilih?", "Perhatian", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Try
                Call conecDB()
                comDB = New MySql.Data.MySqlClient.MySqlCommand("DELETE FROM rel_kategoriharga WHERE id='" + TextBoxKategoriHargaID.Text + "'", connDB)
                comDB.ExecuteNonQuery()
                AmbilKategoriHargaUsed(plu)
                ClearTextBoxKategoriHarga()
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If
    End Sub

    Private Sub DataGridViewKategoriHarga_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridViewKategoriHarga.CellMouseClick
        If e.RowIndex < 0 Then
            Exit Sub
        End If
        Dim id_kategoriharga = DataGridViewKategoriHarga.Rows(e.RowIndex).Cells(0).Value
        TextBoxKategoriHargaID.Text = id_kategoriharga
    End Sub

    Sub ClearTextBoxKategoriHarga()
        TextBoxKategoriHargaID.Clear()
        TextBoxKategoriHarga_harga.Clear()
    End Sub

    Sub ClearTextBoxProduk()
        TextBoxBarcode.Clear()
        TextBoxProdukPLU.Clear()
        TextBoxProdukDesc.Clear()
        TextBoxProdukHargaBeli.Clear()
        TextBoxProdukHargaJual.Clear()
        TextBoxProdukStok.Clear()
        TextBoxKategoriHargaID.Clear()
        TextBoxKategoriHarga_harga.Clear()
        TextBoxRak.Clear()
        TextBoxKategoriProduk.Clear()
    End Sub

    Private Sub TextBoxBarcode_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxBarcode.KeyDown
        Select e.KeyCode
            Case Keys.Enter
                Try
                    Dim barcd = TextBoxBarcode.Text
                    Call conecDB()
                    comDB = New MySql.Data.MySqlClient.MySqlCommand("SELECT desc2 FROM prodmast WHERE barcd='" + barcd + "'", connDB)
                    rdDB = comDB.ExecuteReader
                    If rdDB.HasRows Then
                        rdDB.Read()
                        MessageBox.Show("Barang sudah pernah didaftarkan. (" + rdDB.Item("desc2") + ")", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Else
                        TextBoxProdukDesc.Focus()
                    End If
                Catch ex As Exception
                    MsgBox(e.ToString)
                End Try
                rdDB.Close()
        End Select
    End Sub

    Private Sub TextBoxProdukDesc_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxProdukDesc.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                TextBoxProdukHargaBeli.Focus()
        End Select
    End Sub

    Private Sub TextBoxProdukHargaBeli_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxProdukHargaBeli.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                TextBoxProdukHargaJual.Focus()
        End Select
    End Sub

    Private Sub TextBoxProdukHargaJual_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxProdukHargaJual.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                TextBoxProdukStok.Focus()
        End Select
    End Sub

    Private Sub TextBoxRak_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBoxRak.MouseClick
        FormProduk_Rak_Select.ShowDialog()
    End Sub

    Private Sub TextBoxKategoriProduk_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBoxKategoriProduk.MouseClick
        FormProduk_Kategori_Select.ShowDialog()
    End Sub

    Private Sub ButtonDeleteProduk_Click(sender As Object, e As EventArgs) Handles ButtonDeleteProduk.Click
        Dim plu = TextBoxProdukPLU.Text
        Dim query = ""
        Dim result As DialogResult = MessageBox.Show("Hapus produk terpilih?", "Perhatian", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Try
                Call conecDB()
                comDB = New MySql.Data.MySqlClient.MySqlCommand("DELETE FROM prodmast WHERE prdcd='" + plu + "'", connDB)
                comDB.ExecuteNonQuery()

                Try
                    Call conecDB()
                    comDB = New MySql.Data.MySqlClient.MySqlCommand("DELETE FROM rel_kategoriharga WHERE plu='" + plu + "'", connDB)
                    comDB.ExecuteNonQuery()
                    load_data_produk(query)
                    PanelDetailProduk.Visible = False
                Catch ex As Exception
                    MsgBox("Kategori Harga : " + ex.ToString)
                End Try

                load_data_produk(query)
                PanelDetailProduk.Visible = False
            Catch ex As Exception
                MsgBox("Prodmast : " + ex.ToString)
            End Try
        End If
    End Sub

    Private Sub ButtonExportExcelProduk_Click(sender As Object, e As EventArgs) Handles ButtonExportExcelProduk.Click
        Cursor = Cursors.WaitCursor
        ButtonExportExcelProduk.Enabled = False
        ButtonExportExcelProduk.Text = "mohon tunggu"
        Try
            Dim ExcelApp As Microsoft.Office.Interop.Excel.Application
            Dim ExcelWorkBook As Microsoft.Office.Interop.Excel.Workbook
            Dim ExcelWorkSheet As Microsoft.Office.Interop.Excel.Worksheet
            Dim misValue As Object = System.Reflection.Missing.Value
            Dim a As Integer
            Dim b As Integer

            ExcelApp = New Microsoft.Office.Interop.Excel.Application
            ExcelWorkBook = ExcelApp.Workbooks.Add(misValue)
            ExcelWorkSheet = ExcelWorkBook.Sheets("sheet1")

            ExcelWorkSheet.Cells(1, 1) = POSMAIN.LabelToko.Text
            ExcelWorkSheet.Cells(2, 1) = POSMAIN.LabelSubToko.Text
            ExcelWorkSheet.Cells(4, 1) = "Laporan Posisi Stok Produk " & Date.Today
            ExcelWorkSheet.Cells(5, 1) = "-------------------------------------------------------"

            For a = 0 To DataGridViewBarang.RowCount - 1
                For b = 0 To DataGridViewBarang.ColumnCount - 1
                    For c As Integer = 1 To DataGridViewBarang.Columns.Count
                        ExcelWorkSheet.Cells(6, c) = DataGridViewBarang.Columns(c - 1).HeaderText
                        ExcelWorkSheet.Cells(a + 7, b + 1) = DataGridViewBarang(b, a).Value.ToString()
                    Next
                Next
            Next
            Dim FileNameExcel = "D:\Kasgros\Laporan\Produk_" + Date.Today.ToString("dd_MM_yy") + ".xlsx"
            ExcelWorkSheet.SaveAs(FileNameExcel)
            ExcelWorkBook.Close()
            ExcelApp.Quit()

            releaseObject(ExcelApp)
            releaseObject(ExcelWorkBook)
            releaseObject(ExcelWorkSheet)

            ButtonExportExcelProduk.Enabled = True
            ButtonExportExcelProduk.Text = "Export Excel"
            Dim ask As DialogResult = MessageBox.Show("Data produk berhasil disimpan. Apakah anda akan membuka file " + FileNameExcel + "?", "Informasi", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If ask = Windows.Forms.DialogResult.Yes Then
                Dim BukaFile As New System.Diagnostics.Process
                BukaFile = Process.Start(FileNameExcel)
            End If
        Catch ex As Exception
            MsgBox("error export laporan produk : " + ex.ToString)
        End Try
        Cursor = Cursors.Default
    End Sub

    Private Sub releaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub
End Class