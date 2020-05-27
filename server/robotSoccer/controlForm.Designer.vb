<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class controlForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(controlForm))
        Me.startStop = New System.Windows.Forms.Button()
        Me.northWestButton = New System.Windows.Forms.Button()
        Me.southWestButton = New System.Windows.Forms.Button()
        Me.leftButton = New System.Windows.Forms.Button()
        Me.upButton = New System.Windows.Forms.Button()
        Me.createPlayerButton = New System.Windows.Forms.Button()
        Me.rightButton = New System.Windows.Forms.Button()
        Me.northEastButton = New System.Windows.Forms.Button()
        Me.downButton = New System.Windows.Forms.Button()
        Me.stopButton = New System.Windows.Forms.Button()
        Me.southEastButton = New System.Windows.Forms.Button()
        Me.playerNum = New System.Windows.Forms.NumericUpDown()
        Me.stepButton = New System.Windows.Forms.Button()
        Me.deleteButton = New System.Windows.Forms.Button()
        Me.resetGameButton = New System.Windows.Forms.Button()
        Me.hostInfoLabel = New System.Windows.Forms.Label()
        Me.ipAddrLabel = New System.Windows.Forms.Label()
        Me.netStartStop = New System.Windows.Forms.Button()
        Me.portLabel = New System.Windows.Forms.Label()
        Me.hostNameLabel = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.portNum = New System.Windows.Forms.NumericUpDown()
        Me.fieldControls = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.speedNum = New System.Windows.Forms.NumericUpDown()
        Me.idLabel = New System.Windows.Forms.Label()
        Me.moveBallRandomButton = New System.Windows.Forms.Button()
        Me.createBall = New System.Windows.Forms.Button()
        Me.ResetBall = New System.Windows.Forms.Button()
        Me.ServerLabel = New System.Windows.Forms.Label()
        Me.serverName = New System.Windows.Forms.TextBox()
        Me.startViewerButton = New System.Windows.Forms.Button()
        Me.UpdateStatusTimer = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlayfieldToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ParametersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MessageLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplotchFieldToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.DynamicLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.startupTimer = New System.Windows.Forms.Timer(Me.components)
        CType(Me.playerNum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.portNum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.fieldControls.SuspendLayout()
        CType(Me.speedNum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'startStop
        '
        Me.startStop.Location = New System.Drawing.Point(0, 345)
        Me.startStop.Name = "startStop"
        Me.startStop.Size = New System.Drawing.Size(66, 37)
        Me.startStop.TabIndex = 0
        Me.startStop.Text = "Start"
        Me.startStop.UseVisualStyleBackColor = True
        '
        'northWestButton
        '
        Me.northWestButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.northWestButton.Location = New System.Drawing.Point(3, 36)
        Me.northWestButton.Name = "northWestButton"
        Me.northWestButton.Size = New System.Drawing.Size(39, 39)
        Me.northWestButton.TabIndex = 1
        Me.northWestButton.Text = "NW"
        Me.northWestButton.UseVisualStyleBackColor = True
        '
        'southWestButton
        '
        Me.southWestButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.southWestButton.Location = New System.Drawing.Point(3, 126)
        Me.southWestButton.Name = "southWestButton"
        Me.southWestButton.Size = New System.Drawing.Size(39, 39)
        Me.southWestButton.TabIndex = 2
        Me.southWestButton.Text = "SW"
        Me.southWestButton.UseVisualStyleBackColor = True
        '
        'leftButton
        '
        Me.leftButton.Location = New System.Drawing.Point(3, 81)
        Me.leftButton.Name = "leftButton"
        Me.leftButton.Size = New System.Drawing.Size(39, 39)
        Me.leftButton.TabIndex = 3
        Me.leftButton.Text = "W"
        Me.leftButton.UseVisualStyleBackColor = True
        '
        'upButton
        '
        Me.upButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.upButton.Location = New System.Drawing.Point(48, 36)
        Me.upButton.Name = "upButton"
        Me.upButton.Size = New System.Drawing.Size(39, 39)
        Me.upButton.TabIndex = 4
        Me.upButton.Text = "N"
        Me.upButton.UseVisualStyleBackColor = True
        '
        'createPlayerButton
        '
        Me.createPlayerButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.createPlayerButton.Location = New System.Drawing.Point(0, 208)
        Me.createPlayerButton.Name = "createPlayerButton"
        Me.createPlayerButton.Size = New System.Drawing.Size(56, 41)
        Me.createPlayerButton.TabIndex = 5
        Me.createPlayerButton.Text = "Create a Player"
        Me.createPlayerButton.UseVisualStyleBackColor = True
        '
        'rightButton
        '
        Me.rightButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rightButton.Location = New System.Drawing.Point(93, 81)
        Me.rightButton.Name = "rightButton"
        Me.rightButton.Size = New System.Drawing.Size(39, 39)
        Me.rightButton.TabIndex = 6
        Me.rightButton.Text = "E"
        Me.rightButton.UseVisualStyleBackColor = True
        '
        'northEastButton
        '
        Me.northEastButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.northEastButton.Location = New System.Drawing.Point(93, 36)
        Me.northEastButton.Name = "northEastButton"
        Me.northEastButton.Size = New System.Drawing.Size(39, 39)
        Me.northEastButton.TabIndex = 7
        Me.northEastButton.Text = "NE"
        Me.northEastButton.UseVisualStyleBackColor = True
        '
        'downButton
        '
        Me.downButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.downButton.Location = New System.Drawing.Point(48, 126)
        Me.downButton.Name = "downButton"
        Me.downButton.Size = New System.Drawing.Size(39, 39)
        Me.downButton.TabIndex = 8
        Me.downButton.Text = "S"
        Me.downButton.UseVisualStyleBackColor = True
        '
        'stopButton
        '
        Me.stopButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.stopButton.Location = New System.Drawing.Point(48, 81)
        Me.stopButton.Name = "stopButton"
        Me.stopButton.Size = New System.Drawing.Size(39, 39)
        Me.stopButton.TabIndex = 9
        Me.stopButton.Text = "Stop"
        Me.stopButton.UseVisualStyleBackColor = True
        '
        'southEastButton
        '
        Me.southEastButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.southEastButton.Location = New System.Drawing.Point(93, 126)
        Me.southEastButton.Name = "southEastButton"
        Me.southEastButton.Size = New System.Drawing.Size(39, 39)
        Me.southEastButton.TabIndex = 10
        Me.southEastButton.Text = "SE"
        Me.southEastButton.UseVisualStyleBackColor = True
        '
        'playerNum
        '
        Me.playerNum.Location = New System.Drawing.Point(3, 171)
        Me.playerNum.Maximum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.playerNum.Name = "playerNum"
        Me.playerNum.Size = New System.Drawing.Size(54, 20)
        Me.playerNum.TabIndex = 11
        Me.playerNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'stepButton
        '
        Me.stepButton.Location = New System.Drawing.Point(72, 345)
        Me.stepButton.Name = "stepButton"
        Me.stepButton.Size = New System.Drawing.Size(66, 37)
        Me.stepButton.TabIndex = 12
        Me.stepButton.Text = "Step"
        Me.stepButton.UseVisualStyleBackColor = True
        '
        'deleteButton
        '
        Me.deleteButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.deleteButton.Location = New System.Drawing.Point(0, 255)
        Me.deleteButton.Name = "deleteButton"
        Me.deleteButton.Size = New System.Drawing.Size(138, 33)
        Me.deleteButton.TabIndex = 13
        Me.deleteButton.Text = "Delete a Player/Ball"
        Me.deleteButton.UseVisualStyleBackColor = True
        '
        'resetGameButton
        '
        Me.resetGameButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.resetGameButton.Location = New System.Drawing.Point(0, 388)
        Me.resetGameButton.Name = "resetGameButton"
        Me.resetGameButton.Size = New System.Drawing.Size(138, 25)
        Me.resetGameButton.TabIndex = 14
        Me.resetGameButton.Text = "Restart Game"
        Me.resetGameButton.UseVisualStyleBackColor = True
        '
        'hostInfoLabel
        '
        Me.hostInfoLabel.AutoSize = True
        Me.hostInfoLabel.Location = New System.Drawing.Point(10, 24)
        Me.hostInfoLabel.Name = "hostInfoLabel"
        Me.hostInfoLabel.Size = New System.Drawing.Size(53, 13)
        Me.hostInfoLabel.TabIndex = 15
        Me.hostInfoLabel.Text = "Host Info:"
        '
        'ipAddrLabel
        '
        Me.ipAddrLabel.AutoSize = True
        Me.ipAddrLabel.Location = New System.Drawing.Point(186, 24)
        Me.ipAddrLabel.Name = "ipAddrLabel"
        Me.ipAddrLabel.Size = New System.Drawing.Size(36, 13)
        Me.ipAddrLabel.TabIndex = 16
        Me.ipAddrLabel.Text = "ipaddr"
        '
        'netStartStop
        '
        Me.netStartStop.Location = New System.Drawing.Point(176, 67)
        Me.netStartStop.Name = "netStartStop"
        Me.netStartStop.Size = New System.Drawing.Size(110, 38)
        Me.netStartStop.TabIndex = 17
        Me.netStartStop.Text = "Start As Server"
        Me.netStartStop.UseVisualStyleBackColor = True
        '
        'portLabel
        '
        Me.portLabel.AutoSize = True
        Me.portLabel.Location = New System.Drawing.Point(195, 45)
        Me.portLabel.Name = "portLabel"
        Me.portLabel.Size = New System.Drawing.Size(26, 13)
        Me.portLabel.TabIndex = 19
        Me.portLabel.Text = "Port"
        '
        'hostNameLabel
        '
        Me.hostNameLabel.AutoSize = True
        Me.hostNameLabel.Location = New System.Drawing.Point(76, 24)
        Me.hostNameLabel.Name = "hostNameLabel"
        Me.hostNameLabel.Size = New System.Drawing.Size(55, 13)
        Me.hostNameLabel.TabIndex = 20
        Me.hostNameLabel.Text = "Hostname"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.hostNameLabel)
        Me.Panel1.Controls.Add(Me.ipAddrLabel)
        Me.Panel1.Controls.Add(Me.hostInfoLabel)
        Me.Panel1.Location = New System.Drawing.Point(5, 26)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(275, 14)
        Me.Panel1.TabIndex = 21
        '
        'portNum
        '
        Me.portNum.Location = New System.Drawing.Point(220, 41)
        Me.portNum.Maximum = New Decimal(New Integer() {16000, 0, 0, 0})
        Me.portNum.Minimum = New Decimal(New Integer() {1025, 0, 0, 0})
        Me.portNum.Name = "portNum"
        Me.portNum.Size = New System.Drawing.Size(66, 20)
        Me.portNum.TabIndex = 22
        Me.portNum.Value = New Decimal(New Integer() {8080, 0, 0, 0})
        '
        'fieldControls
        '
        Me.fieldControls.Controls.Add(Me.Label1)
        Me.fieldControls.Controls.Add(Me.speedNum)
        Me.fieldControls.Controls.Add(Me.idLabel)
        Me.fieldControls.Controls.Add(Me.moveBallRandomButton)
        Me.fieldControls.Controls.Add(Me.createBall)
        Me.fieldControls.Controls.Add(Me.ResetBall)
        Me.fieldControls.Controls.Add(Me.resetGameButton)
        Me.fieldControls.Controls.Add(Me.deleteButton)
        Me.fieldControls.Controls.Add(Me.stepButton)
        Me.fieldControls.Controls.Add(Me.playerNum)
        Me.fieldControls.Controls.Add(Me.southEastButton)
        Me.fieldControls.Controls.Add(Me.stopButton)
        Me.fieldControls.Controls.Add(Me.downButton)
        Me.fieldControls.Controls.Add(Me.northEastButton)
        Me.fieldControls.Controls.Add(Me.rightButton)
        Me.fieldControls.Controls.Add(Me.createPlayerButton)
        Me.fieldControls.Controls.Add(Me.upButton)
        Me.fieldControls.Controls.Add(Me.leftButton)
        Me.fieldControls.Controls.Add(Me.southWestButton)
        Me.fieldControls.Controls.Add(Me.northWestButton)
        Me.fieldControls.Controls.Add(Me.startStop)
        Me.fieldControls.Location = New System.Drawing.Point(69, 111)
        Me.fieldControls.Name = "fieldControls"
        Me.fieldControls.Size = New System.Drawing.Size(145, 427)
        Me.fieldControls.TabIndex = 24
        Me.fieldControls.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(15, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 13)
        Me.Label1.TabIndex = 32
        Me.Label1.Text = "Speed"
        '
        'speedNum
        '
        Me.speedNum.DecimalPlaces = 1
        Me.speedNum.Location = New System.Drawing.Point(59, 10)
        Me.speedNum.Name = "speedNum"
        Me.speedNum.Size = New System.Drawing.Size(63, 20)
        Me.speedNum.TabIndex = 31
        Me.speedNum.Value = New Decimal(New Integer() {4, 0, 0, 0})
        '
        'idLabel
        '
        Me.idLabel.AutoSize = True
        Me.idLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.idLabel.Location = New System.Drawing.Point(63, 173)
        Me.idLabel.Name = "idLabel"
        Me.idLabel.Size = New System.Drawing.Size(0, 13)
        Me.idLabel.TabIndex = 30
        '
        'moveBallRandomButton
        '
        Me.moveBallRandomButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.moveBallRandomButton.Location = New System.Drawing.Point(72, 299)
        Me.moveBallRandomButton.Name = "moveBallRandomButton"
        Me.moveBallRandomButton.Size = New System.Drawing.Size(66, 40)
        Me.moveBallRandomButton.TabIndex = 29
        Me.moveBallRandomButton.Text = "Move Ball Randomly"
        Me.moveBallRandomButton.UseVisualStyleBackColor = True
        '
        'createBall
        '
        Me.createBall.Location = New System.Drawing.Point(79, 208)
        Me.createBall.Name = "createBall"
        Me.createBall.Size = New System.Drawing.Size(59, 41)
        Me.createBall.TabIndex = 28
        Me.createBall.Text = "Create a Ball"
        Me.createBall.UseVisualStyleBackColor = True
        '
        'ResetBall
        '
        Me.ResetBall.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ResetBall.Location = New System.Drawing.Point(0, 299)
        Me.ResetBall.Name = "ResetBall"
        Me.ResetBall.Size = New System.Drawing.Size(66, 40)
        Me.ResetBall.TabIndex = 15
        Me.ResetBall.Text = "Move Ball to Center"
        Me.ResetBall.UseVisualStyleBackColor = True
        '
        'ServerLabel
        '
        Me.ServerLabel.AutoSize = True
        Me.ServerLabel.Location = New System.Drawing.Point(12, 44)
        Me.ServerLabel.Name = "ServerLabel"
        Me.ServerLabel.Size = New System.Drawing.Size(38, 13)
        Me.ServerLabel.TabIndex = 25
        Me.ServerLabel.Text = "Server"
        '
        'serverName
        '
        Me.serverName.Location = New System.Drawing.Point(47, 41)
        Me.serverName.Name = "serverName"
        Me.serverName.Size = New System.Drawing.Size(149, 20)
        Me.serverName.TabIndex = 26
        '
        'startViewerButton
        '
        Me.startViewerButton.Location = New System.Drawing.Point(12, 67)
        Me.startViewerButton.Name = "startViewerButton"
        Me.startViewerButton.Size = New System.Drawing.Size(110, 38)
        Me.startViewerButton.TabIndex = 27
        Me.startViewerButton.Text = "Start As Viewer"
        Me.startViewerButton.UseVisualStyleBackColor = True
        '
        'UpdateStatusTimer
        '
        Me.UpdateStatusTimer.Enabled = True
        Me.UpdateStatusTimer.Interval = 2000
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ViewToolStripMenuItem, Me.DebugToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(286, 24)
        Me.MenuStrip1.TabIndex = 28
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlayfieldToolStripMenuItem, Me.ParametersToolStripMenuItem, Me.MessageLogToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'PlayfieldToolStripMenuItem
        '
        Me.PlayfieldToolStripMenuItem.Name = "PlayfieldToolStripMenuItem"
        Me.PlayfieldToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.PlayfieldToolStripMenuItem.Text = "Playfield"
        '
        'ParametersToolStripMenuItem
        '
        Me.ParametersToolStripMenuItem.Name = "ParametersToolStripMenuItem"
        Me.ParametersToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.ParametersToolStripMenuItem.Text = "Parameters"
        '
        'MessageLogToolStripMenuItem
        '
        Me.MessageLogToolStripMenuItem.Name = "MessageLogToolStripMenuItem"
        Me.MessageLogToolStripMenuItem.Size = New System.Drawing.Size(143, 22)
        Me.MessageLogToolStripMenuItem.Text = "Message Log"
        '
        'DebugToolStripMenuItem
        '
        Me.DebugToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SplotchFieldToolStripMenuItem})
        Me.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem"
        Me.DebugToolStripMenuItem.Size = New System.Drawing.Size(54, 20)
        Me.DebugToolStripMenuItem.Text = "Debug"
        '
        'SplotchFieldToolStripMenuItem
        '
        Me.SplotchFieldToolStripMenuItem.Name = "SplotchFieldToolStripMenuItem"
        Me.SplotchFieldToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
        Me.SplotchFieldToolStripMenuItem.Text = "Splotch Field"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.AboutToolStripMenuItem.Text = "About..."
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel, Me.DynamicLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 541)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(286, 22)
        Me.StatusStrip1.TabIndex = 29
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(121, 17)
        Me.StatusLabel.Text = "ToolStripStatusLabel1"
        '
        'DynamicLabel
        '
        Me.DynamicLabel.Name = "DynamicLabel"
        Me.DynamicLabel.Size = New System.Drawing.Size(0, 17)
        '
        'startupTimer
        '
        Me.startupTimer.Interval = 500
        '
        'controlForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(286, 563)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.startViewerButton)
        Me.Controls.Add(Me.serverName)
        Me.Controls.Add(Me.ServerLabel)
        Me.Controls.Add(Me.fieldControls)
        Me.Controls.Add(Me.portNum)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.portLabel)
        Me.Controls.Add(Me.netStartStop)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "controlForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Robot Controls"
        CType(Me.playerNum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.portNum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.fieldControls.ResumeLayout(False)
        Me.fieldControls.PerformLayout()
        CType(Me.speedNum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents startStop As System.Windows.Forms.Button
    Friend WithEvents northWestButton As System.Windows.Forms.Button
    Friend WithEvents southWestButton As System.Windows.Forms.Button
    Friend WithEvents leftButton As System.Windows.Forms.Button
    Friend WithEvents upButton As System.Windows.Forms.Button
    Friend WithEvents createPlayerButton As System.Windows.Forms.Button
    Friend WithEvents rightButton As System.Windows.Forms.Button
    Friend WithEvents northEastButton As System.Windows.Forms.Button
    Friend WithEvents downButton As System.Windows.Forms.Button
    Friend WithEvents stopButton As System.Windows.Forms.Button
    Friend WithEvents southEastButton As System.Windows.Forms.Button
    Friend WithEvents playerNum As System.Windows.Forms.NumericUpDown
    Friend WithEvents stepButton As System.Windows.Forms.Button
    Friend WithEvents deleteButton As System.Windows.Forms.Button
    Friend WithEvents resetGameButton As System.Windows.Forms.Button
    Friend WithEvents hostInfoLabel As System.Windows.Forms.Label
    Friend WithEvents ipAddrLabel As System.Windows.Forms.Label
    Friend WithEvents netStartStop As System.Windows.Forms.Button
    Friend WithEvents portLabel As System.Windows.Forms.Label
    Friend WithEvents hostNameLabel As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents portNum As System.Windows.Forms.NumericUpDown
    Friend WithEvents fieldControls As System.Windows.Forms.Panel
    Friend WithEvents ServerLabel As System.Windows.Forms.Label
    Friend WithEvents serverName As System.Windows.Forms.TextBox
    Friend WithEvents startViewerButton As System.Windows.Forms.Button
    Friend WithEvents ResetBall As System.Windows.Forms.Button
    Friend WithEvents createBall As System.Windows.Forms.Button
    Friend WithEvents UpdateStatusTimer As System.Windows.Forms.Timer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlayfieldToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ParametersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents moveBallRandomButton As System.Windows.Forms.Button
    Friend WithEvents MessageLogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents idLabel As System.Windows.Forms.Label
    Friend WithEvents startupTimer As System.Windows.Forms.Timer
    Friend WithEvents DynamicLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents speedNum As System.Windows.Forms.NumericUpDown
    Friend WithEvents DebugToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplotchFieldToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
