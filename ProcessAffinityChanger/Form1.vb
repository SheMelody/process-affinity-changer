Public Class Form1

    Public Cores = 0

    Dim ProcessList As New List(Of Process)
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        RefreshProcessList()
    End Sub
    Sub RefreshProcessList()
        ListBox1.Items.Clear()
        ProcessList.Clear()
        For Each proc In Diagnostics.Process.GetProcesses()
            Try
                ListBox1.Items.Add(proc.ProcessName & " - " & proc.Id)
                ProcessList.Add(proc)
            Catch ex As Exception
                Continue For
            End Try
        Next
    End Sub

    Public BoolIntDictionary As New Dictionary(Of Boolean, Integer) From {
            {False, 0},
            {True, 1}
        }

    Public Function DecToBinary(ByVal dec As IntPtr) As List(Of Boolean)
        Dim d As UInteger = dec
        Dim bools As New List(Of Boolean)
        Dim str = ""
        While d > 0
            Dim r = d Mod 2
            d = d \ 2
            If r = 1 Then
                bools.Add(True)
            Else
                bools.Add(False)
            End If
            str = r & str
        End While
        ' MsgBox(str)
        Return bools
    End Function

    Public Function BinToDecimal(ByVal l As List(Of Boolean)) As IntPtr
        Dim d As UInteger = 0
        For i = 0 To l.Count - 1
            d = d + BoolIntDictionary(l(i)) * Math.Pow(2, i)
        Next
        Return d
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ListBox1.SelectedIndex < 0 Or ListBox1.SelectedIndex > ListBox1.Items.Count - 1 Then
            Exit Sub
        End If
        Dim affinity_form As New Form2
        Dim checkboxes As New List(Of CheckBox)
        Try
            Dim cores = Environment.ProcessorCount
            Dim proc = ProcessList(ListBox1.SelectedIndex)
            affinity_form.Label1.Text = "Set affinity of " & proc.ProcessName & " (PID " & proc.Id & ")"
            Dim affinity = proc.ProcessorAffinity
            Dim AssignedCores As List(Of Boolean) = DecToBinary(affinity)
            Dim y = 5
            For i = 1 To cores
                Dim cb As New CheckBox()
                cb.Text = "Core " & i - 1
                cb.Parent = affinity_form.Panel1
                cb.Location = New Point(5, y)
                cb.Checked = AssignedCores(i - 1)
                affinity_form.Panel1.Controls.Add(cb)
                checkboxes.Add(cb)
                y = y + 20
            Next
            affinity_form.ShowDialog()
            If affinity_form.Apply Then
                Dim ProcList As New List(Of Boolean)
                For Each c In checkboxes
                    ProcList.Add(c.Checked)
                Next
                proc.ProcessorAffinity = BinToDecimal(ProcList)
                MsgBox("The affinity was set successfully.", MsgBoxStyle.Information, "OK")
            End If
        Catch ex As Exception
            Try
                affinity_form.Dispose()
            Catch ex2 As Exception
            End Try
            MsgBox("Affinity could not be changed: " & ex.Message, MsgBoxStyle.Critical, "Error")
        End Try


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        RefreshProcessList()
    End Sub
End Class
