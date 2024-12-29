USE [AppointmentsTest];
GO

SET IDENTITY_INSERT [dbo].[Appointments] ON;
GO

-- Success Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (500, 'Delete Daily Meeting', '2024-12-28 10:00:00', '2024-12-28 10:30:00', 'Morning sync meeting', 1),
    (501, 'Delete Backlog Refinement', '2024-12-28 11:00:00', '2024-12-28 11:30:00', 'Morning sync meeting', 1);

-- Validation Appointments
INSERT INTO [dbo].[Appointments] ([Id], [Title], [StartTime], [EndTime], [Description], [Status])
VALUES 
    (502, 'Delete Client Presentation', '2024-12-29 14:00:00', '2024-12-29 15:00:00', 'Presentation scheduled, cannot delete', 2);

SET IDENTITY_INSERT [dbo].[Appointments] OFF;
GO