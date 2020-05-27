<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MessageLog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.logView = New System.Windows.Forms.ListView
        Me.timeCol = New System.Windows.Forms.ColumnHeader
        Me.fromCol = New System.Windows.Forms.ColumnHeader
        Me.toCol = New System.Windows.Forms.ColumnHeader
        Me.mesgCol = New System.Windows.Forms.ColumnHeader
        Me.clearButton = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.maxQuestions = New System.Windows.Forms.NumericUpDown
        Me.saveButton = New System.Windows.Forms.Button
        CType(Me.maxQuestions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'logView
        '
        Me.logView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.logView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.timeCol, Me.fromCol, Me.toCol, Me.mesgCol})
        Me.logView.Location = New System.Drawing.Point(12, 12)
        Me.logView.Name = "logView"
        Me.logView.Size = New System.Drawing.Size(523, 342)
        Me.logView.TabIndex = 0
        Me.logView.UseCompatibleStateImageBehavior = False
        Me.logView.View = System.Windows.Forms.View.Details
        '
        'timeCol
        '
        Me.timeCol.Text = "Time"
        Me.timeCol.Width = 150
        '
        'fromCol
        '
        Me.fromCol.Text = "From"
        Me.fromCol.Width = 50
        '
        'toCol
        '
        Me.toCol.Text = "To"
        Me.toCol.Width = 50
        '
        'mesgCol
        '
        Me.mesgCol.Text = "Message"
        Me.mesgCol.Width = 600
        '
        'clearButton
        '
        Me.clearButton.Location = New System.Drawing.Point(320, 368)
        Me.clearButton.Name = "clearButton"
        Me.clearButton.Size = New System.Drawing.Size(75, 23)
        Me.clearButton.TabIndex = 1
        Me.clearButton.Text = "Clear"
        Me.clearButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 373)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(148, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Max # of Message (0=no limit)"
        '
        'maxQuestions
        '
        Me.maxQuestions.Location = New System.Drawing.Point(180, 371)
        Me.maxQuestions.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.maxQuestions.Name = "maxQuestions"
        Me.maxQuestions.Size = New System.Drawing.Size(69, 20)
        Me.maxQuestions.TabIndex = 3
        Me.maxQuestions.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'saveButton
        '
        Me.saveButton.Location = New System.Drawing.Point(422, 368)
        Me.saveButton.Name = "saveButton"
        Me.saveButton.Size = New System.Drawing.Size(75, 23)
        Me.saveButton.TabIndex = 4
        Me.saveButton.Text = "Save..."
        Me.saveButton.UseVisualStyleBackColor = True
        '
        'MessageLog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(547, 414)
        Me.Controls.Add(Me.saveButton)
        Me.Controls.Add(Me.maxQuestions)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.clearButton)
        Me.Controls.Add(Me.logView)
        Me.Name = "MessageLog"
        Me.Text = "MessageLog"
        CType(Me.maxQuestions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents logView As System.Windows.Forms.ListView
    Friend WithEvents fromCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents clearButton As System.Windows.Forms.Button
    Friend WithEvents toCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents mesgCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents timeCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents maxQuestions As System.Windows.Forms.NumericUpDown
    Friend WithEvents saveButton As System.Windows.Forms.Button
End Class
