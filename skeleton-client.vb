Dim robo As New RobotClient

Robo.Connect(server, port)
Robo.CreatePlayer(name, teamSides.red)

Robo.SetSpeed(5,5)

Robo.Disconnect()
