Imports System.Drawing.Imaging

Public Class Form1
    Dim FileName1 As String
    Dim FileName2 As String
    Friend img As Image
    Dim bmp As Bitmap
    Dim rect As Rectangle
    Dim bmpData As System.Drawing.Imaging.BitmapData
    Dim ptr As IntPtr
    Dim bytes As Integer
    Dim rgbValues() As Byte
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            FileName1 = OpenFileDialog1.FileName
            Label1.Text = FileName1
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            FileName2 = OpenFileDialog1.FileName
            Label2.Text = FileName2
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim y As Integer
        Try
            Dim moto As New Bitmap(FileName1)
            y = moto.Size.Height
        Catch
            MsgBox("画像が読み込めませんでした。" + vbCrLf + "未対応の画像か、画像が設定されていない可能性があります", , "画像読み込みエラー")
            Exit Sub
        End Try

        ProgressBar1.Value = 0 '0％にしておく
        Me.ProgressBar1.Maximum = y
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Button3.Enabled = False 'ボタンを無効に
        RadioButton1.Enabled = False
        RadioButton2.Enabled = False
        RadioButton3.Enabled = False
        RadioButton4.Enabled = False
        RadioButton5.Enabled = False

        'バックグラウンド操作の実行を開始する
        Me.BackgroundWorker1.RunWorkerAsync()
    End Sub

    Function Max(ByVal ParamArray Numbers() As Integer) '複数の数から一番大きい数を返す
        Dim kekka As Integer
        For i As Integer = 0 To Numbers.Length - 2
            If Numbers(i) > Numbers(i + 1) Then
                kekka = (Numbers(i))
            ElseIf Numbers(i + 1) > Numbers(i) Then
                kekka = (Numbers(i + 1))
            End If
        Next
        If kekka = 0 Then
            kekka = Numbers(0)
        End If
        Return kekka
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Label1.Text = ""
        Label2.Text = ""
    End Sub

    Private Sub BackgroundWorker1_DoWork( _
    ByVal sender As Object, _
    ByVal e As System.ComponentModel.DoWorkEventArgs) _
    Handles BackgroundWorker1.DoWork
        '変換処理
        Dim x As Integer
        Dim y As Integer
        Dim moto As New Bitmap(FileName1)
        Dim ato As New Bitmap(FileName1)
        Dim MOTOIRO(moto.Width, moto.Height) As Color
        MOTOIRO = getPixelColor(moto)
        Dim PCo As Color
        Dim P As New Pen(Color.White)
        Dim S As Integer '明るさ　または　z値の大きさ
        Dim mask As New Bitmap(FileName2)
        Dim MASKIRO(mask.Width, mask.Height) As Color
        MASKIRO = getPixelColor(mask)
        Dim flug As Integer
        Dim flug2 As Integer
        Dim Value As Integer = NumericUpDown1.Value
        x = moto.Size.Width
        y = moto.Size.Height
        Me.ProgressBar1.Maximum = y

        '変換処理
        If RadioButton3.Checked Then
            Dim g As Graphics
            Try
                g = Graphics.FromImage(ato)
            Catch
                MsgBox("使えない画像です。ほかの画像にして下さい。")
                Exit Sub
            End Try
            For i As Integer = 0 To y - 1
                For i2 As Integer = 0 To x - 1
                    P.Color = MOTOIRO(i2, i) 'ペンの色決定

                    PCo = MASKIRO(i2, i) 'そのピクセルのカラーを指定
                    S = Max(PCo.R, PCo.G, PCo.B) / Value

                    If S < 0 Then
                        S = 0
                    End If
                    If S <> 0 Then '最後にもう1ピクセル動かす処理
                        flug = 1
                        flug2 = S
                    End If
                    If S = 0 And flug = 1 Then
                        flug = 0
                        S = flug2
                    End If

                    '幅1の線を引く
                    If S <> 0 Then
                        g.DrawLine(P, i2, i, i2 - S, i)
                    End If
                Next
                '進捗状況の報告
                Me.BackgroundWorker1.ReportProgress(i + 1)
            Next
            g.Dispose()
        ElseIf RadioButton4.Checked Then
            Dim g As Graphics
            Dim gg As Graphics
            Dim en As Integer '途中からの加算のため
            Try
                g = Graphics.FromImage(ato)
                gg = Graphics.FromImage(moto)
            Catch
                MsgBox("使えない画像です。ほかの画像にして下さい。")
                Exit Sub
            End Try
            For i As Integer = 0 To y - 1
                For i2 As Integer = 0 To x - 1
                    P.Color = MOTOIRO(i2, i) 'ペンの色決定
                    PCo = MASKIRO(i2, i) 'そのピクセルのカラーを指定
                    S = Max(PCo.R, PCo.G, PCo.B) / Value

                    If S < 0 Then
                        S = 0
                    End If
                    If S <> 0 Then '最後にもう1ピクセル動かす処理
                        flug = 1
                        flug2 = S
                    End If
                    If S = 0 And flug = 1 Then
                        flug = 0
                        S = flug2
                    End If

                    '幅1の線を引く
                    If S <> 0 Then
                        g.DrawLine(P, i2, i, i2 - S, i)
                    End If
                Next
                '進捗状況の報告
                Me.BackgroundWorker1.ReportProgress((i + 1) / 2)
                en = i / 2 '次の画像の開始位置(バー)
            Next
            'もう一枚の処理
            For i As Integer = y - 1 To 0 Step -1
                For i2 As Integer = x - 1 To 0 Step -1
                    P.Color = MOTOIRO(i2, i) 'ペンの色決定
                    PCo = MASKIRO(i2, i) 'そのピクセルのカラーを指定
                    S = Max(PCo.R, PCo.G, PCo.B) / Value
                    If S < 0 Then
                        S = 0
                    End If
                    If S <> 0 Then '最後にもう1ピクセル動かす処理
                        flug = 1
                        flug2 = S
                    End If
                    If S = 0 And flug = 1 Then
                        flug = 0
                        S = flug2
                    End If

                    '幅1の線を引く
                    If S <> 0 Then
                        gg.DrawLine(P, i2, i, i2 + S, i)
                    End If
                Next
                '進捗状況の報告
                Me.BackgroundWorker1.ReportProgress(en + (((y - 1) - (i + 1)) / 2))
            Next
            g.Dispose()
            gg.Dispose()
        End If
        Me.BackgroundWorker1.ReportProgress(y)


        Dim canvas As New Bitmap(x * 2, y) '変更画像と元画像を合わせる
        Dim g2 As Graphics = Graphics.FromImage(canvas)
        Dim f1 As Boolean = False
        If RadioButton5.Checked Then
            f1 = True '赤青アナグリフかどうかのフラグ
            Dim canvas2 As New Bitmap(x, y)
            g2 = Graphics.FromImage(canvas2)
            'ColorMatrixオブジェクトの作成
            Dim cm As New System.Drawing.Imaging.ColorMatrix()
            'ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
            cm.Matrix00 = 0
            cm.Matrix11 = 1
            cm.Matrix22 = 1
            cm.Matrix33 = 0.5F
            cm.Matrix44 = 1

            'ImageAttributesオブジェクトの作成
            Dim ia As New System.Drawing.Imaging.ImageAttributes()
            'ColorMatrixを設定する
            ia.SetColorMatrix(cm)

            'ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
            cm.Matrix00 = 1
            cm.Matrix11 = 0
            cm.Matrix22 = 0
            cm.Matrix33 = 1.0F
            cm.Matrix44 = 1
            'ColorMatrixを設定する
            Dim ia2 As New System.Drawing.Imaging.ImageAttributes()
            ia2.SetColorMatrix(cm)

            'ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
            cm.Matrix00 = 2
            cm.Matrix11 = 2
            cm.Matrix22 = 2
            cm.Matrix33 = 1.0F
            cm.Matrix44 = 1
            'ColorMatrixを設定する
            Dim ia3 As New System.Drawing.Imaging.ImageAttributes()
            ia3.SetColorMatrix(cm)

            g2.DrawImage(ato, New Rectangle(0, 0, ato.Width, ato.Height), _
0, 0, x, y, GraphicsUnit.Pixel, ia2)
            g2.DrawImage(moto, New Rectangle(0, 0, moto.Width, moto.Height), _
    0, 0, x, y, GraphicsUnit.Pixel, ia)
            g2.DrawImage(canvas2, New Rectangle(0, 0, moto.Width, moto.Height), _
0, 0, x, y, GraphicsUnit.Pixel, ia3)

            img = New Bitmap(canvas2)

        ElseIf RadioButton1.Checked Then '交差法
            g2.DrawImage(moto, x, 0, x, y)
            g2.DrawImage(ato, 0, 0, x, y)
        ElseIf RadioButton2.Checked Then '平行法
            g2.DrawImage(moto, 0, 0, x, y)
            g2.DrawImage(ato, x, 0, x, y)
        End If

        g2.Dispose()
        If f1 = False Then
            img = canvas.Clone '完成したものを保存
        End If

        canvas.Dispose()

    End Sub

    Private Sub BackgroundWorker1_ProgressChanged( _
    ByVal sender As System.Object, _
    ByVal e As System.ComponentModel.ProgressChangedEventArgs) _
    Handles BackgroundWorker1.ProgressChanged

        '進捗状況をプログレスバーに表示
        Me.ProgressBar1.Value = e.ProgressPercentage
    End Sub

    'バックグラウンド処理の終了を判断する
    Private Sub BackgroundWorker1_RunWorkerCompleted( _
        ByVal sender As System.Object, _
        ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) _
        Handles BackgroundWorker1.RunWorkerCompleted
        MsgBox("完了")
        Form2.Show()
        Form2.PictureBox1.Image = img
        Button3.Enabled = True 'ボタンを有効に
        Button4.Enabled = True
        Button5.Enabled = True
        RadioButton1.Enabled = True
        RadioButton2.Enabled = True
        RadioButton3.Enabled = True
        RadioButton4.Enabled = True
        RadioButton5.Enabled = True
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        '保存処理
        If SaveFileDialog1.ShowDialog = DialogResult.OK Then
            img.Save(SaveFileDialog1.FileName)
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        'プレビュー処理
        Form2.Show()
        Form2.PictureBox1.Image = img.Clone
    End Sub
    Public Function getPixelColor( _
    ByVal img As Bitmap)

        Dim color(img.Width, img.Height) As Color

        '1ピクセルあたりのバイト数を取得する
        Dim pixelSize As Integer = 3
        If img.PixelFormat = PixelFormat.Format24bppRgb Then
            pixelSize = 3
        ElseIf img.PixelFormat = PixelFormat.Format32bppArgb OrElse _
            img.PixelFormat = PixelFormat.Format32bppPArgb OrElse _
            img.PixelFormat = PixelFormat.Format32bppRgb Then
            pixelSize = 4
        Else
            Throw New ArgumentException( _
                "1ピクセルあたり24または32ビットの形式のイメージのみ有効です。", _
                "img")
        End If

        'Bitmapをロックする
        Dim bmpDate As BitmapData = _
            img.LockBits(New Rectangle(0, 0, img.Width, img.Height), _
                         ImageLockMode.ReadWrite, img.PixelFormat)

        'ピクセルデータをバイト型配列で取得する
        Dim ptr As IntPtr = bmpDate.Scan0
        Dim pixels As Byte() = New Byte(bmpDate.Stride * img.Height - 1) {}
        System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, pixels.Length)

        'すべてのピクセルの色を補正する
        For y As Integer = 0 To bmpDate.Height - 1
            For x As Integer = 0 To bmpDate.Width - 1
                'ピクセルデータでのピクセル(x,y)の開始位置を計算する
                Dim pos As Integer = y * bmpDate.Stride + x * pixelSize

                color(x, y) = System.Drawing.ColorTranslator.FromOle(RGB(pixels(pos + 2), pixels(pos + 1), pixels(pos)))

            Next
        Next

        'ピクセルデータを元に戻す
        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, pixels.Length)

        'ロックを解除する
        img.UnlockBits(bmpDate)

        Return color
    End Function

End Class