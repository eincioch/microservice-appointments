USE [AppointmentsTest];
GO

SET IDENTITY_INSERT [dbo].[Appointments] ON;
GO

-- Success Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (300, 'Put Team Sync', '2024-12-28 10:00:00', '2024-12-28 11:00:00', 'Weekly team sync meeting', 1),
    (301, 'Put Client Presentation', '2024-12-29 14:00:00', '2024-12-29 15:00:00', 'Presentation to the client', 2);

-- Validation Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (302, 'Put Invalid Meeting', '2024-12-28 12:00:00', '2024-12-28 12:00:00', 'Start and end times are the same', 1);

SET IDENTITY_INSERT [dbo].[Appointments] OFF;
GO