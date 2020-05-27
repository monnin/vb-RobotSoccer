Option Strict On
Option Explicit On

Imports System.Windows.Forms

Public Class Parameters


    Const def_interval As Integer = 100
    Const def_numrobots As Integer = 10

    Const def_ball_mass As Single = 0.01
    Const def_ball_momentum As Single = 0.98
    Const def_ball_size As Single = 15.0
    Const def_ball_max_speed As Single = 15
    Const def_ball_min_speed As Single = 0.05
    Const def_ball_wallbounce As Single = 0.5
    Const def_ball_cornerbounce As Single = 0.5

    Const def_goal_mass As Single = 10000000
    Const def_goal_size As Single = 50.0
    Const def_goal_momentum As Single = 0
    Const def_goal_max_speed As Single = 0
    Const def_goal_min_speed As Single = 100
    Const def_goal_wallbounce As Single = 0
    Const def_goal_cornerbounce As Single = 0.5

    Const def_robot_mass As Single = 1000
    Const def_robot_momentum As Single = 0.85
    Const def_robot_size As Single = 20
    Const def_robot_max_speed As Single = 5
    Const def_robot_min_speed As Single = 0.05
    Const def_robot_wallbounce As Single = 0.5
    Const def_robot_cornerbounce As Single = 0.5

    Private var_interval As Integer = def_interval
    Private var_numrobots As Integer = def_numrobots

    Private var_ball_mass As Single = def_ball_mass
    Private var_ball_momentum As Single = def_ball_momentum
    Private var_ball_size As Single = def_ball_size
    Private var_ball_max_speed As Single = def_ball_max_speed
    Private var_ball_min_speed As Single = def_ball_min_speed
    Private var_ball_wallbounce As Single = def_ball_wallbounce
    Private var_ball_cornerbounce As Single = def_ball_cornerbounce

    Private var_goal_mass As Single = def_goal_mass
    Private var_goal_size As Single = def_goal_size
    Private var_goal_momentum As Single = def_goal_momentum
    Private var_goal_max_speed As Single = def_goal_max_speed
    Private var_goal_min_speed As Single = def_goal_min_speed
    Private var_goal_wallbounce As Single = def_goal_wallbounce
    Private var_goal_cornerbounce As Single = def_goal_cornerbounce

    Private var_robot_mass As Single = def_robot_mass
    Private var_robot_momentum As Single = def_robot_momentum
    Private var_robot_size As Single = def_robot_size
    Private var_robot_max_speed As Single = def_robot_max_speed
    Private var_robot_min_speed As Single = def_robot_min_speed
    Private var_robot_wallbounce As Single = def_robot_wallbounce
    Private var_robot_cornerbounce As Single = def_robot_cornerbounce

    ReadOnly Property ball_mass() As Single
        Get
            Return var_ball_mass
        End Get
    End Property

    ReadOnly Property ball_momentum() As Single
        Get
            Return var_ball_momentum
        End Get
    End Property

    ReadOnly Property ball_size() As Single
        Get
            Return var_ball_size
        End Get
    End Property

    ReadOnly Property ball_max_speed() As Single
        Get
            Return var_ball_max_speed
        End Get
    End Property

    ReadOnly Property ball_min_speed() As Single
        Get
            Return var_ball_min_speed
        End Get
    End Property

    ReadOnly Property ball_wallbounce() As Single
        Get
            Return var_ball_wallbounce
        End Get
    End Property

    ReadOnly Property ball_cornerbounce() As Single
        Get
            Return var_ball_cornerbounce
        End Get
    End Property

    ReadOnly Property goal_mass() As Single
        Get
            Return var_goal_mass
        End Get
    End Property

    ReadOnly Property goal_size() As Single
        Get
            Return var_goal_size
        End Get
    End Property

    ReadOnly Property goal_momentum() As Single
        Get
            Return var_goal_momentum
        End Get
    End Property

    ReadOnly Property goal_max_speed() As Single
        Get
            Return var_goal_max_speed
        End Get
    End Property

    ReadOnly Property goal_min_speed() As Single
        Get
            Return var_goal_min_speed
        End Get
    End Property

    ReadOnly Property goal_wallbounce() As Single
        Get
            Return var_goal_wallbounce
        End Get
    End Property

    ReadOnly Property goal_cornerbounce() As Single
        Get
            Return var_goal_cornerbounce
        End Get
    End Property

    ReadOnly Property robot_mass() As Single
        Get
            Return var_robot_mass
        End Get
    End Property

    ReadOnly Property robot_momentum() As Single
        Get
            Return var_robot_momentum
        End Get
    End Property

    ReadOnly Property robot_size() As Single
        Get
            Return var_robot_size
        End Get
    End Property

    ReadOnly Property robot_max_speed() As Single
        Get
            Return var_robot_max_speed
        End Get
    End Property

    ReadOnly Property robot_min_speed() As Single
        Get
            Return var_robot_min_speed
        End Get
    End Property

    ReadOnly Property robot_wallbounce() As Single
        Get
            Return var_robot_wallbounce
        End Get
    End Property

    ReadOnly Property robot_cornerbounce() As Single
        Get
            Return var_robot_cornerbounce
        End Get
    End Property

    ReadOnly Property interval() As Integer
        Get
            Return var_interval
        End Get
    End Property


    ReadOnly Property numrobots() As Integer
        Get
            Return var_numrobots
        End Get
    End Property

    '
    '-------------------------------------------------------------------------------
    '

    '
    '   Allow a value only if it a reasonable value (0 or greater)
    '
    Private Function SetIfPositive(ByVal newstr As String, ByRef newVal As Single) As Boolean
        Dim testVal As Single
        Dim changed As Boolean = False

        If (Single.TryParse(newstr, testVal)) Then
            If (testVal >= 0) Then
                changed = (newVal <> testVal)
                newVal = testVal
            End If
        End If

        Return changed
    End Function

    '
    '-------------------------------------------------------------------------------
    '

    Private Function SetIfPositive(ByVal newstr As String, ByRef newVal As Integer) As Boolean
        Dim testVal As Single
        Dim changed As Boolean = False

        If (Single.TryParse(newstr, testVal)) Then
            If (testVal >= 0) Then
                changed = (newVal <> testVal)
                newVal = CInt(testVal)
            End If
        End If

        Return changed
    End Function

    '
    '-------------------------------------------------------------------------------
    '

    '
    '   Allow a value only if it a reasonable value (between 0 and 1)
    '
    Private Function SetIfZeroToOne(ByVal newstr As String, ByRef newVal As Single) As Boolean
        Dim testVal As Single
        Dim changed As Boolean = False

        If (Single.TryParse(newstr, testVal)) Then
            If ((testVal >= 0) And (testVal <= 1)) Then
                changed = (newVal <> testVal)
                newVal = testVal
            End If
        End If

        Return changed
    End Function

    '
    '-------------------------------------------------------------------------------
    '

    '
    '   ApplyParams
    '
    '       Handle changes in the Parameters DataGrid
    '
    '

    Private Function ApplyParams() As Boolean
        Dim i As Integer
        Dim changed As Boolean = False

        For i = 0 To params.Rows.Count - 1
            Select Case params.Item(0, i).Value.ToString
                Case "Max. Speed"
                    changed = changed Or SetIfPositive(params.Item(1, i).Value.ToString, var_ball_max_speed)
                    changed = changed Or SetIfPositive(params.Item(2, i).Value.ToString, var_robot_max_speed)
                    changed = changed Or SetIfPositive(params.Item(3, i).Value.ToString, var_goal_max_speed)

                Case "Min. Speed"
                    changed = changed Or SetIfPositive(params.Item(1, i).Value.ToString, var_ball_min_speed)
                    changed = changed Or SetIfPositive(params.Item(2, i).Value.ToString, var_robot_min_speed)
                    changed = changed Or SetIfPositive(params.Item(3, i).Value.ToString, var_goal_min_speed)

                Case "Weight"
                    changed = changed Or SetIfPositive(params.Item(1, i).Value.ToString, var_ball_mass)
                    changed = changed Or SetIfPositive(params.Item(2, i).Value.ToString, var_robot_mass)
                    changed = changed Or SetIfPositive(params.Item(3, i).Value.ToString, var_goal_mass)

                Case "Wall Bounce (percent)"
                    changed = changed Or SetIfZeroToOne(params.Item(1, i).Value.ToString, var_ball_wallbounce)
                    changed = changed Or SetIfZeroToOne(params.Item(2, i).Value.ToString, var_robot_wallbounce)
                    changed = changed Or SetIfZeroToOne(params.Item(3, i).Value.ToString, var_goal_wallbounce)

                Case "Corner Bounce (percent)"
                    changed = changed Or SetIfZeroToOne(params.Item(1, i).Value.ToString, var_ball_cornerbounce)
                    changed = changed Or SetIfZeroToOne(params.Item(2, i).Value.ToString, var_robot_cornerbounce)
                    changed = changed Or SetIfZeroToOne(params.Item(3, i).Value.ToString, var_goal_cornerbounce)

                Case "Momentum (percent)"
                    changed = changed Or SetIfZeroToOne(params.Item(1, i).Value.ToString, var_ball_momentum)
                    changed = changed Or SetIfZeroToOne(params.Item(2, i).Value.ToString, var_robot_momentum)
                    changed = changed Or SetIfZeroToOne(params.Item(3, i).Value.ToString, var_goal_momentum)

                Case "Size"
                    changed = changed Or SetIfPositive(params.Item(1, i).Value.ToString, var_ball_size)
                    changed = changed Or SetIfPositive(params.Item(2, i).Value.ToString, var_robot_size)
                    changed = changed Or SetIfPositive(params.Item(3, i).Value.ToString, var_goal_size)

                Case Else
                    MsgBox("*** Hey! Unknown param '" & params.Item(0, 1).Value.ToString & "' ***")
            End Select
        Next i

        '
        '  The next two do not represent changes that affect individual components
        '
        SetIfPositive(intervalTextBox.Text, var_interval)
        SetIfPositive(maxRobotsBox.Text, var_numrobots)

        Return changed
    End Function


    '
    '-------------------------------------------------------------------------------
    '

    '
    '   ApplyLocations
    '
    '       Handle changes in the Location DataGrid
    '

    Private Sub ApplyLocations()
        Dim i As Integer

        For i = 0 To locations.Rows.Count - 1
            Select Case locations.Item(0, i).Value.ToString
                Case "Red Goal"
                    playingField.Set_All_Loc(locations.Item(1, i).Value.ToString, _
                                            locations.Item(2, i).Value.ToString, _
                                            playerOrObject.playerType.goal, _
                                            playingField.teamSide.RED_TEAM)

                Case "Blue Goal"
                    playingField.Set_All_Loc(locations.Item(1, i).Value.ToString, _
                                            locations.Item(2, i).Value.ToString, _
                                            playerOrObject.playerType.goal, _
                                            playingField.teamSide.BLUE_TEAM)
            End Select
        Next i

    End Sub

    '
    '-------------------------------------------------------------------------------
    '

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If (ApplyParams()) Then
            If (MsgBox("Changes will affect all new objects created." & vbCrLf & _
                       "Do you also want to apply the changes to all existing objects?", MsgBoxStyle.YesNo, "Apply Changes") = MsgBoxResult.Yes) Then
                playingField.Apply_Parameters()
            End If

        End If

        ApplyLocations()

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Hide()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Hide()
    End Sub

    '
    '-------------------------------------------------------------------------------
    '

    Private Sub load_gui()
        Params.rows.clear()  ' Empty the contents if this is a reload

        params.Rows.Add("Max. Speed", var_ball_max_speed, var_robot_max_speed, var_goal_max_speed)
        params.Rows.Add("Min. Speed", var_ball_min_speed, var_robot_min_speed, var_goal_min_speed)
        params.Rows.Add("Momentum (percent)", var_ball_momentum, var_robot_momentum, var_goal_momentum)
        params.Rows.Add("Wall Bounce (percent)", var_ball_wallbounce, var_robot_wallbounce, var_goal_wallbounce)
        params.Rows.Add("Corner Bounce (percent)", var_ball_cornerbounce, var_robot_cornerbounce, var_goal_cornerbounce)
        params.Rows.Add("Weight", var_ball_mass, var_robot_mass, var_goal_mass)
        params.Rows.Add("Size", var_ball_size, var_robot_size, var_goal_size)

        locations.Rows.Add("Red Goal", "0%", "50%")
        locations.Rows.Add("Blue Goal", "100%", "50%")

        'locations.Rows.Add("Balls", "50+/-R", "50+/-R")
        'locations.Rows.Add("Blue Robots", "60", "S")
        'locations.Rows.Add("Red Robots", "40", "S")

        intervalTextBox.Text = var_interval.ToString
        maxRobotsBox.Text = var_numrobots.ToString
    End Sub

    '
    '   reset_defaults
    '
    '    Take all of the parameters and set them back
    '    to their initial values
    '
    Public Sub reset_defaults()

        var_interval = def_interval
        var_numrobots = def_numrobots

        var_ball_mass = def_ball_mass
        var_ball_momentum = def_ball_momentum
        var_ball_size = def_ball_size
        var_ball_max_speed = def_ball_max_speed
        var_ball_min_speed = def_ball_min_speed
        var_ball_wallbounce = def_ball_wallbounce

        var_goal_mass = def_goal_mass
        var_goal_size = def_goal_size
        var_goal_momentum = def_goal_momentum
        var_goal_max_speed = def_goal_max_speed
        var_goal_min_speed = def_goal_min_speed
        var_goal_wallbounce = def_goal_wallbounce

        var_robot_mass = def_robot_mass
        var_robot_momentum = def_robot_momentum
        var_robot_size = def_robot_size
        var_robot_max_speed = def_robot_max_speed
        var_robot_min_speed = def_robot_min_speed
        var_robot_wallbounce = def_robot_wallbounce

        load_gui()
    End Sub




    Private Sub Parameters_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        load_gui()
    End Sub



    Private Sub Parameters_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        controlForm.ParametersToolStripMenuItem.Checked = Me.Visible
    End Sub
End Class
