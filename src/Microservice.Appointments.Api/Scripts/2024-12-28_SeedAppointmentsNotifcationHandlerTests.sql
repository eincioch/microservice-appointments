USE [AppointmentsTest];
GO

SET IDENTITY_INSERT [dbo].[Appointments] ON;
GO

-- Success Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (600, 'Notification Test Meeting', '2024-12-28 10:00:00', '2024-12-28 11:00:00', 'Meeting for notification test', 1);

-- Validation Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (601, 'Notification Invalid Event Type', '2024-12-29 14:00:00', '2024-12-29 15:00:00', 'Event type mismatch test', 1),
    (602, 'Notification Non-updatable Appointment', '2024-12-30 12:00:00', '2024-12-30 13:00:00', 'Test for validation errors', 2);

SET IDENTITY_INSERT [dbo].[Appointments] OFF;
GO