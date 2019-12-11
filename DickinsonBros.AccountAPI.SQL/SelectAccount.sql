CREATE PROCEDURE [dbo].[SelectAccount]
	@username varchar(50)
AS
	select 
		Username,
		[Password]
	from dbo.Account
	where Username = @username
RETURN 0
