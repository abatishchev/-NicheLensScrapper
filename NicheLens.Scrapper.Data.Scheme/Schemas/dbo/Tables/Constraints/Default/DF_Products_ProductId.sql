ALTER TABLE dbo.Products
	ADD CONSTRAINT DF_Products_ProductId
	DEFAULT NewSequentialId()
	FOR ProductId
