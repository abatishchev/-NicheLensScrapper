ALTER TABLE dbo.Products
	ADD CONSTRAINT UX_Products_Asin
	UNIQUE ([Asin])
