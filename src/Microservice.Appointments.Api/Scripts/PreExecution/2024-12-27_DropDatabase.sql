USE master;
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'AppointmentsTest')
BEGIN
    ALTER DATABASE [AppointmentsTest] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [AppointmentsTest];
END;