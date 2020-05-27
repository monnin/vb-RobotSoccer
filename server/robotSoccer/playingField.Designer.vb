<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class playingField
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(playingField))
        Me.actionTimer = New System.Windows.Forms.Timer(Me.components)
        Me.redScoreBox = New System.Windows.Forms.TextBox()
        Me.blueScoreBox = New System.Windows.Forms.TextBox()
        Me.redLabel = New System.Windows.Forms.Label()
        Me.blueLabel = New System.Windows.Forms.Label()
        Me.scoreLogo = New System.Windows.Forms.Label()
        Me.reminderBox = New System.Windows.Forms.TextBox()
        Me.oneSecondTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'actionTimer
        '
        Me.actionTimer.Enabled = True
        '
        'redScoreBox
        '
        Me.redScoreBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.redScoreBox.Enabled = False
        Me.redScoreBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.redScoreBox.ForeColor = System.Drawing.Color.White
        Me.redScoreBox.Location = New System.Drawing.Point(25, 19)
        Me.redScoreBox.Name = "redScoreBox"
        Me.redScoreBox.ReadOnly = True
        Me.redScoreBox.Size = New System.Drawing.Size(40, 29)
        Me.redScoreBox.TabIndex = 0
        Me.redScoreBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'blueScoreBox
        '
        Me.blueScoreBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.blueScoreBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.blueScoreBox.Enabled = False
        Me.blueScoreBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.blueScoreBox.ForeColor = System.Drawing.Color.White
        Me.blueScoreBox.Location = New System.Drawing.Point(608, 19)
        Me.blueScoreBox.Name = "blueScoreBox"
        Me.blueScoreBox.ReadOnly = True
        Me.blueScoreBox.Size = New System.Drawing.Size(40, 29)
        Me.blueScoreBox.TabIndex = 1
        Me.blueScoreBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'redLabel
        '
        Me.redLabel.AutoSize = True
        Me.redLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.redLabel.ForeColor = System.Drawing.Color.White
        Me.redLabel.Location = New System.Drawing.Point(28, -1)
        Me.redLabel.Name = "redLabel"
        Me.redLabel.Size = New System.Drawing.Size(37, 17)
        Me.redLabel.TabIndex = 2
        Me.redLabel.Text = "Red"
        '
        'blueLabel
        '
        Me.blueLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.blueLabel.AutoSize = True
        Me.blueLabel.BackColor = System.Drawing.Color.DarkGreen
        Me.blueLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.blueLabel.ForeColor = System.Drawing.Color.White
        Me.blueLabel.Location = New System.Drawing.Point(608, -1)
        Me.blueLabel.Name = "blueLabel"
        Me.blueLabel.Size = New System.Drawing.Size(40, 17)
        Me.blueLabel.TabIndex = 3
        Me.blueLabel.Text = "Blue"
        '
        'scoreLogo
        '
        Me.scoreLogo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.scoreLogo.AutoSize = True
        Me.scoreLogo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.scoreLogo.Font = New System.Drawing.Font("Microsoft Sans Serif", 99.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.scoreLogo.ForeColor = System.Drawing.Color.Yellow
        Me.scoreLogo.Location = New System.Drawing.Point(80, 110)
        Me.scoreLogo.Name = "scoreLogo"
        Me.scoreLogo.Size = New System.Drawing.Size(498, 151)
        Me.scoreLogo.TabIndex = 4
        Me.scoreLogo.Text = "Goal !!!"
        Me.scoreLogo.Visible = False
        '
        'reminderBox
        '
        Me.reminderBox.BackColor = System.Drawing.Color.GreenYellow
        Me.reminderBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.reminderBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.reminderBox.ForeColor = System.Drawing.Color.Maroon
        Me.reminderBox.Location = New System.Drawing.Point(251, 300)
        Me.reminderBox.Multiline = True
        Me.reminderBox.Name = "reminderBox"
        Me.reminderBox.Size = New System.Drawing.Size(407, 33)
        Me.reminderBox.TabIndex = 5
        Me.reminderBox.Text = "Reminder: Press [Start] to begin the game..."
        Me.reminderBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.reminderBox.Visible = False
        '
        'oneSecondTimer
        '
        Me.oneSecondTimer.Enabled = True
        Me.oneSecondTimer.Interval = 1000
        '
        'playingField
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DarkGreen
        Me.ClientSize = New System.Drawing.Size(670, 388)
        Me.Controls.Add(Me.reminderBox)
        Me.Controls.Add(Me.scoreLogo)
        Me.Controls.Add(Me.blueLabel)
        Me.Controls.Add(Me.redLabel)
        Me.Controls.Add(Me.blueScoreBox)
        Me.Controls.Add(Me.redScoreBox)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(100, 97)
        Me.Name = "playingField"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Playing Field"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents actionTimer As System.Windows.Forms.Timer
    Friend WithEvents redScoreBox As System.Windows.Forms.TextBox
    Friend WithEvents blueScoreBox As System.Windows.Forms.TextBox
    Friend WithEvents redLabel As System.Windows.Forms.Label
    Friend WithEvents blueLabel As System.Windows.Forms.Label
    Friend WithEvents scoreLogo As System.Windows.Forms.Label
    Friend WithEvents reminderBox As System.Windows.Forms.TextBox
    Friend WithEvents oneSecondTimer As System.Windows.Forms.Timer

End Class
