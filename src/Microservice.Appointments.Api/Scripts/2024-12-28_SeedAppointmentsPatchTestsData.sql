USE [AppointmentsTest];
GO

SET IDENTITY_INSERT [dbo].[Appointments] ON;
GO

-- Success Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (400, 'Patch Team Sync', '2024-12-28 10:00:00', '2024-12-28 11:00:00', 'Weekly team sync meeting', 1),
    (401, 'Patch Client Presentation', '2024-12-29 14:00:00', '2024-12-29 15:00:00', 'Presentation to the client', 1);

-- Validation Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (402, 'Patch Already Canceled Meeting', '2024-12-30 12:00:00', '2024-12-30 13:00:00', 'This meeting is already canceled', 3);

SET IDENTITY_INSERT [dbo].[Appointments] OFF;
GO