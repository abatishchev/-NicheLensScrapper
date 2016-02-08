alter table dbo.Product
	add constraint UX_Product_Asin
	unique ([Asin])
