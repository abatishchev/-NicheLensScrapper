alter table dbo.Product
	add constraint FK_Product_CategoryId_Category
	foreign key (CategoryId)
	references dbo.Category (CategoryId)
go

alter table dbo.Product
	check constraint FK_Product_CategoryId_Category
go