USE [AppointmentsTest];
GO

SET IDENTITY_INSERT [dbo].[Appointments] ON;
GO

INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (200, 'Daily Standup', '2024-12-28 09:00:00', '2024-12-28 09:15:00', 'Quick sync with the team to discuss daily tasks', 1),
    (201, 'Server Maintenance', '2024-12-28 23:00:00', '2024-12-29 03:00:00', 'Scheduled maintenance for production server', 2),
    (202, 'Sprint Planning', '2024-12-29 10:00:00', '2024-12-29 11:30:00', 'Plan tasks for the next sprint with the team', 1),
    (203, 'Deployment Window', '2024-12-30 01:00:00', '2024-12-30 03:00:00', 'Deploying version 2.3 of the application', 1),
    (204, 'Retrospective Meeting', '2024-12-30 16:00:00', '2024-12-30 17:00:00', 'Review the last sprint and discuss improvements', 3);
GO

SET IDENTITY_INSERT [dbo].[Appointments] OFF;
GO