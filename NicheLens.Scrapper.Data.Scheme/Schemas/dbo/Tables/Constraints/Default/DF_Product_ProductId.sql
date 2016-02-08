alter table dbo.Product
	add constraint DF_Product_ProductId
	default NewSequentialId()
	for ProductId
