CREATE PROCEDURE [dbo].[InsertAccount]
	@username varchar(50),
	@password varchar(50)
AS
	insert into dbo.Account
	(Username,[Password])
	VALUES
	(LOWER(@username), @password)
RETURN 0
