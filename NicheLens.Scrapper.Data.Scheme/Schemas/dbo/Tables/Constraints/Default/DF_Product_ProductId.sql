alter table dbo.Product
	add constraint DF_Products_ProductId
	default NewSequentialId()
	for ProductId
