USE ansvdp_ims;

SET IDENTITY_INSERT Ranges ON;

INSERT INTO Ranges (Id, Name, Code, HeadquarterLocation, CreatedAt, IsActive)
VALUES (1, 'Test Range', 'TR-01', 'Test Location', GETDATE(), 1);

SET IDENTITY_INSERT Ranges OFF;

SELECT * FROM Ranges WHERE Id = 1;
